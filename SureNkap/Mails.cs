using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace SureNkap
{
    public class Mails
    {
        public static void Send(string Subject, string Reciever, string Message)
        {

            using (SmtpClient smtp = new SmtpClient())
            {
                // replace with sender's email

                MailMessage mail = new MailMessage();
                mail.To.Add(Reciever);
                mail.From = new MailAddress(ConfigurationManager.AppSettings["Email"], ConfigurationManager.AppSettings["SenderName"]);
                //message.Sender = new MailAddress(m.Email);
                mail.Subject = Subject;
                mail.Body = Message;
                mail.IsBodyHtml = true;
                mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(Message, null, MediaTypeNames.Text.Html));
                NetworkCredential credential = new NetworkCredential
                {
                    UserName = ConfigurationManager.AppSettings["Email"],     // replace with sender's email
                    Password = ConfigurationManager.AppSettings["password"]       // replace with sender's password
                };
                smtp.Credentials = credential;
                smtp.Host = ConfigurationManager.AppSettings["mailserver"];  //  for hotmail replace it as needed
                smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["mailport"]);               // for hotmail replace it as needed
                smtp.EnableSsl = false;
                try
                {
                    smtp.Send(mail);
                }
                catch (SmtpException)
                {
                    throw;
                }
            }
        }
    }
}