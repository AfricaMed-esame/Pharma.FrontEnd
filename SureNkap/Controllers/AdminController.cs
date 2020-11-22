using Hangfire;
using NodaTime;
using SureNkap.Attributes;
using SureNkap.EsameService;
using SureNkap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SureNkap.Controllers
{
   
    public class AdminController : Controller
    {
        DataClient client = new DataClient();
        
        public static List<ProformaViewModel> _proforma;

        #region Methodes

        [LoggedOrAuthorized]
        public ActionResult OrderDetails(int id)
        {
            var ord = client.GetOrderAsPdf(id,true);
            return View(ord);
        }

        [LoggedOrAuthorized]
        public ActionResult Orders()
        {
            ViewData["title"] = "Mes Commandes";
            return View();
        }

        public ActionResult LogOut()
        {
            Session.Remove("user");
            return RedirectToAction("Login");
        }
        [LoggedOrAuthorized]
        public ActionResult Settings()
        {
            ViewData["title"] = "Paramètres";
            return View();
        }

        [LoggedOrAuthorized]
        [ActionName("Paiements")]
        public ActionResult Invoices()
        {
            var usr = (Pharmacies)Session["user"];
            ViewData["title"] = "Paiements & Factures";
            return View("~/views/admin/Invoices.cshtml",usr);
        }

        public JsonResult GetQuoteStatus(int id)
        {
            var quote = client.GetQuoteById(id);
            return Json(quote.Status,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(int id,string returnUrl)
        {
            string decodedUrl = "";
            if (!string.IsNullOrEmpty(returnUrl))
                decodedUrl = Server.UrlDecode(returnUrl);

            //Login logic...

            if (!string.IsNullOrEmpty(decodedUrl) &&  Url.IsLocalUrl(decodedUrl))
            {
                return Redirect(decodedUrl);
            }
            var usr = client.FindPharmacieById(id);
            ViewData["title"] = "Accueil";
            Session["user"] = usr;
            ViewBag.countDemande = client.GetQuoteRequestPerPharmacy(usr.Id).Count();
            ViewBag.countProf = client.GetQuotesPerPharmacy(usr.Id).Count();
            ViewBag.countOrd = client.GetOrders(usr.Id, false).Count();
            ViewBag.countBils  = client.ListPharmacyInvoices(usr.Id).Where(x => x.Status == InvoiceStatus.Paid).Count();
            return View(usr);
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            ViewData["title"] = "Connexion";
            return View();
        }

        [LoggedOrAuthorizedAttribute]
        public ActionResult Profil(int id)
        {
            var p = client.FindPharmacieById(id);
            var countries = new SelectList(Counries(), "Id", "Name");
            var currencies = client._GetCurrencies();
            var cur = new SelectList(currencies, "Code", "Name_fr", p.Currency);
            ViewBag.countries = countries;
            ViewBag.currencies = cur;
            var tz = DateTimeZoneProviders.Tzdb.Ids.ToList().ConvertAll<DropDown>(s=>new DropDown {
                Name = s
            });
            var TimezoneList = new SelectList(tz, "Name", "Name");
            ViewBag.timezones = TimezoneList;
          
            if(TempData["success"]!=null)
            {
                ViewBag.message = TempData["success"];
            }
            if (TempData["error"] != null)
            {
                ViewBag.error = TempData["error"];
            }
            ViewData["title"] = "Modifier le profile";
            return View(p);
        }

        [LoggedOrAuthorizedAttribute]
        public ActionResult Demandes()
        {
            ViewData["title"] = "Mes demandes de proforma";
            return View();
        }

        [LoggedOrAuthorized]
        public ActionResult Quotes()
        {
            ViewData["title"] = "Mes proformas";
            return View();
        }

        [LoggedOrAuthorized]
        public ActionResult CreateQuote(int id)
        {
            _proforma = null;
            var pharm = (Pharmacies)Session["user"];
            var request = client.GetRequestDetails(id);
            var req = client.GetQuoteRequestById(id);
            ViewBag.request = req;
            ViewData["title"] = "Creér une nouvelle proforma";
            return View(pharm);
        }

        public JsonResult QuoteRequestSample(int id)
        {
            var request = client.GetRequestDetails(id);
            if (_proforma == null)
            {
                _proforma = new List<ProformaViewModel>();
                var rs = request.ToList().ConvertAll<ProformaViewModel>(d => new ProformaViewModel
                {
                    Product = d.ProductName + ", " + d.Quantity,
                    ProductId = d.Id,
                    UnitPrice = 0,
                    Quantity = 0,
                    Unit = "",
                    Equivalent = "",
                    Total = 0

                });
                _proforma.AddRange(rs);
            }
            return Json(new { data = _proforma });
        }

        public JsonResult EditRow(int productId)
        {
            return Json(_proforma.SingleOrDefault(x => x.ProductId == productId));
        }

        public JsonResult SaveRow(string eq,decimal p,int q,string n,int id)
        {
            try
            {
                var name = _proforma.SingleOrDefault(x => x.ProductId == id).Product;
                _proforma.Remove(_proforma.SingleOrDefault(x => x.ProductId == id));
                var row = new ProformaViewModel
                {
                    ProductId = id,
                    Equivalent = eq,
                    Product = name,
                    Unit = n,
                    UnitPrice = p,
                    Quantity = q,
                    Total = p*q
                };
                _proforma.Add(row);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [ActionName("Devis-details")]
        [LoggedOrAuthorized]
        public ActionResult DevisDetails(int id)
        {
            ViewData["title"] = "Devis-#" + id;
            return View("~/views/admin/DevisDetails.cshtml");
        }

        [ActionName("Account-edit")]
        [LoggedOrAuthorized]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdatePharmacy(FormCollection form)
        {
            try
            {
                var pharm = new Pharmacies
                {
                    Id = Convert.ToInt32(form.Get("Id")),
                    Address = form.Get("adress"),
                    City = form.Get("city"),
                    Contact = form.Get("contact"),
                    CommercialName = form.Get("names"),
                    Country = Convert.ToInt32(form.Get("country")),
                    ManagerContact = form.Get("mcontact"),
                    ManagerNames = form.Get("manager"),
                    Rccm = form.Get("rccm"),
                    TimeZone = form.Get("timezone"),
                    Currency = form.Get("currency")
                };
                client.UpdatePharmacy(pharm);
                TempData["success"] = "Modification effectuée avec succès";
                return RedirectToAction("Profil", new { id = form.Get("Id") });
            }
            catch (FaultException ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Profil", new { id = form.Get("Id") });
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Profil", new { id = form.Get("Id") });
            }
        }
        
        [AllowAnonymous]
        [ActionName("Inscription")]
        public ActionResult Register()
        {
            var countries = new SelectList(Counries(), "Id", "Name");
            ViewBag.countries = countries;
            var tzs = DateTimeZoneProviders.Tzdb.Ids.ToList().ConvertAll<DropDown>(s => new DropDown
            {
                Name = s
            });
            var timezoneList = new SelectList(tzs, "Name", "Name");
            ViewBag.timezones = timezoneList;
            var currencies = client._GetCurrencies();
            var cur = new SelectList(currencies, "Id", "Name");
            ViewBag.currencies = cur;
            if (TempData["error"]!=null)
            {
                ViewBag.message = TempData["error"];
            }
            return View("~/views/admin/Register.cshtml");
        }

        [AllowAnonymous]
        public JsonResult CreatePharmacie(FormCollection form)
        {
            try
            {
                var rs = client.PharmacyExists(form.Get("email"));
                if(rs)
                {
                    return Json(new { success = false, message = "Une pharmacie avec cette addresse email exist déjà" });
                }
                var p = new Pharmacies
                {
                    Username = form.Get("email").ToLower(),
                    Password = EasyEncryption.MD5.ComputeMD5Hash(form.Get("password")),
                    Active = true,
                    Address = form.Get("address"),
                    City = form.Get("city"),
                    CommercialName = form.Get("firstname"),
                    Rccm = form.Get("rccm"),
                    Contact = form.Get("contact"),
                    EmailAddress = form.Get("email"),
                    Country = Convert.ToInt32(form.Get("country")),
                    ManagerContact = form.Get("mcontact"),
                    ManagerNames = form.Get("manager"),
                    TimeZone = form.Get("timezone"),
                    Description = form.Get("desc"),
                    Latitude = form.Get("lat"),
                    Longitude = form.Get("long"),
                    Tva = decimal.Parse("10"),
                    Currency = form.Get("currency")
                };
                var uid = client.CreatePharmacy(p);
                return Json( new {success = true, id = uid });
            }
            catch (FaultException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [ActionName("Insciption-success")]
        public ActionResult Success(string username)
        {
            ViewBag.message = "Votre compte à bien été crée.";
            ViewBag.username = username;
            return View("~/views/admin/Success.cshtml");
        }
        
        #endregion


        #region Ajax

        public JsonResult Validate(int id)
        {
            try
            {
                var usr = (Pharmacies)Session["user"];
                client.MarkAsReadyForPayment(id);
                // send mail to patient
                var ord = client.GetOrderAsPdf(id, true);
                var pat = client._GetPatientById(ord.PatientId);
                var Message = Files.ReadContent("OrderReady.html");
                var callback = "https://secondavismedical.eu/orders/details/" + ord.Id;
                Message = Message.Replace("{url}", callback);
                Message = Message.Replace("{pharmacy}", usr.CommercialName);
                Message = Message.Replace("{number}", ord.OrderNomber);
                Mails.Send("CN°" + ord.OrderNomber + " vérifiée", pat.AdressEmail, Message);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult MarkDelivered(int id)
        {
            try
            {
                var usr = (Pharmacies)Session["user"];
                client.MarkAsDelivered(id);
                // send mail to patient
                var ord = client.GetOrderAsPdf(id, true);
                var pat = client._GetPatientById(ord.PatientId);
                var Message = Files.ReadContent("Delivered.html");
                var callback = "https://secondavismedical.eu/Downloads/orders/" + ord.Id + ".pdf";
                Message = Message.Replace("{url}", callback);
                Message = Message.Replace("{pharmacy}", usr.CommercialName);
                Mails.Send("CN°" + ord.OrderNomber + " livrée", pat.AdressEmail, Message);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult Login(string username, string password)
        {
            try
            {
                var logged = client.OpenSesson(username,EasyEncryption.MD5.ComputeMD5Hash(password));
                if (logged == null)
                {
                    return Json(new { success = false, message = "Login et/ou mot de passe incorrecte" });
                }
                Session["user"] = logged;
                return Json(new { success = true, message = logged.Id });
            }
            catch (FaultException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch(CommunicationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult GetRequestDetails(int id)
        {
            var ls = client.GetRequestDetails(id).ToList();
            return Json(new { data = ls });
        }

        public JsonResult GetOrders()
        {
            var u = (Pharmacies)Session["user"];
            var ls = client.GetOrders(u.Id, false);
            return Json(new { data = ls });
        }

        public JsonResult GetOrderDetails(int id)
        {
            var ls = client.GetOrderDetails(id);
            return Json(new { data = ls });
        }

        public JsonResult SaveProforma(int req, int patientId, string productIds)
        {
            try
            {
                var usr = (Pharmacies)Session["user"];
                var product_ids = productIds.Split(',').ToList();
                var details = new List<ProformaDetails>();
                var request = client.GetRequestById(req);
                foreach (var s in _proforma)
                {
                    details.Add(new ProformaDetails
                    {
                        Product = s.Product,
                        Unit = s.Unit,
                        UnitPrice = s.UnitPrice,
                        Quantity = s.Quantity,
                        ProductId = s.ProductId,
                        Equivalent = s.Equivalent
                    });
                }
                Instant now = SystemClock.Instance.GetCurrentInstant();
                var utcNow = now.InZone(DateTimeZoneProviders.Tzdb[usr.TimeZone]).ToDateTimeUtc();
                var profo = new Proforma
                {
                    Amount = details.Sum(x=>(x.UnitPrice * x.Quantity)),
                    Currency = usr.Currency, // la proforma est faite sen la devise de la pharmacie.
                    PatientId = patientId,
                    PharmacyId = usr.Id,
                    OrdonnanceId = request.OrdonnanceId,
                    DateCreated = utcNow,
                    Status = 0
                };
                client.CreateProforma(profo,details.ToArray());
                _proforma = null;
                return Json(new { success = true});
            }
            catch (FaultException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult SendQuote(int requestId)
        {
            try
            {
                var usr = (Pharmacies)Session["user"];
                var em = client.MarkAsSent(requestId);
                var Message = Files.ReadContent("devisDone.html");
                var callback = "https://secondavismedical.eu/devis/details/" + requestId;
                Message = Message.Replace("{url}", callback);
                Message = Message.Replace("{name}", usr.CommercialName);
                Mails.Send("Proforma", em, Message);
                return Json(new { success = true });
            }
            catch (FaultException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult DelQuote(int requestId)
        {
            try
            {
                client.DeleteQuote(requestId);
                return Json(new { success = true });
            }
            catch (FaultException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult DelPrescriptions(string id)
        {
            try
            {
                client.DeletePrescriptions(id);
                return Json(new { success = true });
            }
            catch (FaultException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult GetQuoteRequest()
        {
            var usr = (Pharmacies)Session["user"];
            var ls = client.GetQuoteRequestPerPharmacy(usr.Id).ToList();
            return Json(new { data = ls });

        }


        public JsonResult GetQuotes()
        {
            var usr = (Pharmacies)Session["user"];
            var ls = client.GetQuotesPerPharmacy(usr.Id).ToList();
            return Json(new { data = ls });
        }

        public JsonResult GetQuoteDetails(int id)
        {
            if (id == 0)
                return Json(new { data = new List<ProformaDetails>() });
            var ls = client.GetQuoteDetails(id,true);
            return Json(new { data = ls });
        }

        public JsonResult GetLast10Request()
        {
            var usr = (Pharmacies)Session["user"];
            var ls = client.GetQuoteRequestPerPharmacy(usr.Id).OrderByDescending(x => x.Id).Take(10).ToList();
            return Json(new { data = ls });
        }

        public JsonResult GetInvoices(int id)
        {
            var ls = client.ListPharmacyInvoices(id);
            return Json(new { data = ls });
        }

        [HttpGet]
        public ActionResult PasswordRecovery()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PasswordRecovery(FormCollection data)
        {
            try
            {
                var email = data.Get("email");
                // get link to reset password
                var result = client.GetPasswordRequestLink2(email);
                if (result.ReturnCode == 0)
                {
                    ModelState.AddModelError(string.Empty, "Cet utilsateur n'existe pas");
                    return View();
                }
                else
                {
                    var Message = Files.ReadContent("PasswordReset.html");
                    Message = Message.Replace("{email}", result.Email);
                    var callback = Url.Action("ResetPassword", "admin", new { uid = result.UniqueId.Value }, protocol: Request.Url.Scheme);
                    Message = Message.Replace("{url}", callback);
                    Mails.Send("Réinitialiser le mot de passe", result.Email, Message);
                    return View("~/Views/admin/PasswordResetSuccess.cshtml");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(Guid uid)
        {
            if(!client.IsValidPasswordResetLink2(uid))
            {
                return View("~/Views/admin/ExpiredLink.cshtml");
            }
            ViewBag.uid = uid;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetPassword(FormCollection data)
        {
            try
            {
                var pwd = data.Get("password");
                var cpwd = data.Get("cpassword");
                if (pwd != cpwd)
                {
                    ModelState.AddModelError(string.Empty, "Les mot de pase saisie ne sont pas identique");
                    return View();
                }
                var uid = Guid.Parse(data.Get("uid"));
                var Id = client.ResetPassword(uid, EasyEncryption.MD5.ComputeMD5Hash(pwd));
                return RedirectToAction("index", "admin", new { id = Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.uid = data.Get("uid");
                return View();
            }
        }

        #endregion

        #region Functions

        [NonAction]
        public List<Countries> Counries()
        {
            var list = client._GetAllCountries().ToList();
            return list;
        }
        #endregion

    }
}