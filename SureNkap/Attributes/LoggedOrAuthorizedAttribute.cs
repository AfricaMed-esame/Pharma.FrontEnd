using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SureNkap.Attributes
{
    public class LoggedOrAuthorizedAttribute : AuthorizeAttribute
    {
        public LoggedOrAuthorizedAttribute()
        {
            View = "error";
            Master = string.Empty;
        }

        public string View { get; set; }
        public string Master { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["user"] == null)
                return false;
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.Url != null)
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Admin",
                            action = "Login",
                            returnUrl = filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery,
                                UriFormat.SafeUnescaped)
                        }));
        }
    }
}