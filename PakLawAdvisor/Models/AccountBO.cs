using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace PakLawAdvisor.Models
{
    public class AccountBO
    {
        pladbEntities pladb;
        public bool ForgotPassword(String Email) {
             pladb = new pladbEntities();
             lawyer lr= pladb.lawyers.Where(lwr => lwr.EMAIL == Email).FirstOrDefault();
             if (lr != null)
             {
                 lr.PASSWORD = (DateTime.UtcNow.Ticks / 6).ToString();
                 pladb.SaveChanges();
                 SendForGotPasswordEmail(Email,lr.PASSWORD);
                 return true;
             }
             else
            return false;
        }
        //public bool SendMail(lawyer lwr)
        //{

        //    string from = "Sufyan.pyxel@gmail.com";
        //    using (MailMessage mail = new MailMessage(from, lwr.EMAIL))
        //    {
        //        mail.Subject = "Verification Mail";

        //        mail.Body = "Hi Dear " + lwr.First_Name + "! <html><body><a href='http://localhost:22118/Lawyer/confirmLawyer/?Verfy_code=" + lwr.VERIFY_CODE + "'>Click Here</a></body></html>";
        //        mail.IsBodyHtml = true;
        //        SmtpClient smtp = new SmtpClient();
        //        smtp.Host = "smtp.gmail.com";
        //        smtp.EnableSsl = true;
        //        NetworkCredential networkCredential = new NetworkCredential(from, "pyxel757673");
        //        smtp.UseDefaultCredentials = true;
        //        smtp.Credentials = networkCredential;
        //        smtp.Port = 587;
        //        smtp.Send(mail);
        //    }
        //    return true;
        //}
        public bool SendForGotPasswordEmail(string Email,string password) {

            string from = "Sufyan.pyxel@gmail.com";
            using (MailMessage mail = new MailMessage(from, Email))
            {
                mail.Subject = "Password Change";

                mail.Body = "Hi Dear ! <html><body><p>Your Password Reset on Your Request " +password+ "</p></body></html>";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential networkCredential = new NetworkCredential(from, "pyxel757673");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = networkCredential;
                smtp.Port = 587;
                smtp.Send(mail);
            }
            return true;
        }


    }
}