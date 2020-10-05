using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Text;
using System.Data;
using System.Web.Hosting;
using PakLawAdvisor.Models;
using appointments365.Controllers;

namespace PakLawAdvisor.Controllers
{
    public class genericsController : Controller
    {
        pladbEntities db = new pladbEntities();
        public List<SelectListItem> Get_Crime_List()
        {
           var crime_list_tbl = db.law_catagry.ToList().OrderBy(m => m.catgry_name);
           List<SelectListItem> crime_list = new List<SelectListItem>();
           foreach (var crm_lst in crime_list_tbl)
           {
               crime_list.Add(new SelectListItem() { Value = crm_lst.catgry_name, Text = crm_lst.catgry_name });
           }

          // ViewBag.crime_list = new SelectList(crime_list, "Text", "Value");
           return crime_list;
        }
         public List<SelectListItem> Get_City_list()
        {
         var Citytbl = db.cities.ToList().OrderBy(m => m.City_Name);
            List<SelectListItem> citylist = new List<SelectListItem>();
            foreach (var state_list in Citytbl)
            {
                citylist.Add(new SelectListItem() { Value = state_list.City_Name, Text = state_list.City_Name });
            }

            return citylist;
        }





        /*
        public ActionResult GetCountryList()
        {
            using (PLAdbEntities db = new PLAdbEntities())
            {
                var rows;

                var countryList = (from x in rows
                                   select new
                                   {
                                       CountryName = x.Country_name,
                                       CountryCode = x.Country_iso_code
                                   }).OrderBy(x => x.CountryName).ToList();

                return this.Response(countryList);
            }
        }

        public ActionResult GetStateList(string country)
        {
            if (string.IsNullOrEmpty(country))
                return this.ErrorResponse("Country name required");

            using (PLAdbEntities db = new PLAdbEntities())
            {
                var stateList = (from x in db.Get_state_list(country)
                                 where x.Country_name.CompareTo(country) == 0
                                 group x by x.Subdivision_name into y
                                 select new
                                 {
                                     StateName = y.First().Subdivision_name,
                                     StateCode = y.First().Subdivision_iso_code
                                 }).ToArray();

                return this.Response(stateList);
            }
        }

        public ActionResult GetCityList(string state)
        {
            if (string.IsNullOrEmpty(state))
                return this.ErrorResponse("State name required");

            using (PLAdbEntities db = new PLAdbEntities())
            {
                var cityList = (from x in db.Get_state_city_list(state)
                                where x.Subdivision_name.CompareTo(state) == 0
                                group x by x.City_name into y
                                select new
                                {
                                    CityName = y.First().City_name
                                }).ToArray();

                return this.Response(cityList);
            }
        }

        
        private JsonResult Response(object responseObject)
        {
            return Json(new { Success = true, Error = "", Payload = responseObject });
        }

        private JsonResult ErrorResponse(string error)
        {
            return Json(new { Success = false, Error = error });
        }
        */
        public bool SendEmail(string Name, string Email, string Description)
        {

            string Template = GetEmailTempalte();
            StringBuilder sb = new StringBuilder();
            MailMessage Message = new MailMessage();
            Message.To.Add("mzuhairqadir@gmail.com");
            Message.From = new MailAddress(Email);
            Message.IsBodyHtml = true;
            Message.Subject = "[Appointments365] Contact Us";
            Message.Priority = MailPriority.High;
            sb.Append("An Email has been sent from ContactUs Page of Appointments365 by <strong> " + Name.ToString() + "</strong>, Email address: <strong>" + Email.ToString() + "</strong>.<br/><br/>");
            sb.Append("Here is the message sent by the user: <strong>" + Description + "</strong>");
            Template = Template.Replace("[subject]", "Contact Us");
            Template = Template.Replace("[body]", sb.ToString());
            Message.Body = Template;
            SmtpClient client = new SmtpClient();
            
            try
            {
                client.Send(Message);
            }
            catch (Exception e)
            {
                throw (e);
            }

            return true;
        }
        public string GetEmailTempalte()
        {
            string HTMLTemplatePath = HostingEnvironment.MapPath("/Views/EmailTemplates/Default.html");
            string HTMLBody = "";
            HTMLBody = System.IO.File.ReadAllText(HTMLTemplatePath);
            return HTMLBody;
        }

        public void SendEmailHardCoded(string name, string email, string subject, string body)
        {
            MailMessage Message = new MailMessage();
            Message.To.Add(email);
            //Message.From = new MailAddress("");
            Message.IsBodyHtml = true;
            Message.Subject = subject;
            Message.Priority = MailPriority.High;
            Message.Body = body;
            SmtpClient client = new SmtpClient();
            try
            {
                client.Send(Message);
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public string ActivtionEmail(ulong User_ID, string UserEmail, string url, string type = "user")
        {
            string Template = GetEmailTempalte();
            EncDec key = new EncDec();
            StringBuilder sb = new StringBuilder();
            MailMessage Message = new MailMessage();
            Message.To.Add(UserEmail);
            Message.IsBodyHtml = true;
            Message.Subject = "Account Activation";
            Message.Priority = MailPriority.High;
            sb.Append("An account was set up on Apppointments365.com using your email address.<br/><br/>");
            sb.Append("<a href='http://" + url + "/account/Activate?id=" + key.enc(User_ID.ToString()) + "&type=" + type + "&email=" + key.enc(UserEmail.ToString()) + "'target='_blank'>http://" + url + "/account/Activate?id=" + key.enc(User_ID.ToString()) + "&type=" + type + "&email=" + key.enc(UserEmail.ToString()) + "</a>");
            sb.Append("<br/><br/>Email:" + UserEmail + "<br/>");
            Template = Template.Replace("[subject]", "Account Activation");
            Template = Template.Replace("[body]", sb.ToString());
            Message.Body = Template;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            try
            {
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new System.Net.NetworkCredential("sociomania.team4@gmail.com", "dssdproject");
                client.Send(Message);
                return null;
            }
            catch (Exception exp)
            {
                return exp.Message;
            }
        }

        //added message to contact your mail support if you did not intend to get this email from app365
        public string ActivtionEmailProvider(string UserEmail, string url, string password)
        {
            string Template = GetEmailTempalte();
            EncDec key = new EncDec();
            StringBuilder sb = new StringBuilder();
            MailMessage Message = new MailMessage();
            Message.To.Add(UserEmail);
            Message.IsBodyHtml = true;
            Message.Subject = "Account Activated and Password";
            Message.Priority = MailPriority.High;
            sb.Append("An account was set up on Apppointments365.com using your email address.<br/><br/>");
            sb.Append("<br/><br/>Username:" + UserEmail + "<br/>");
            sb.Append("Your Password is:"+ password +"<br/><br/>");
            sb.Append("<a href='http://" + url + "/admin/login 'target='_blank'> Click here to Login.  </a> <br/><br/>");
            sb.Append("<br/><br/>If you did not create this account, then please contact your mail support and report the problem. Thank you for using Appointment365.");


            Template = Template.Replace("[subject]", "Account Activated and PasswordS");
            Template = Template.Replace("[body]", sb.ToString());
            Message.Body = Template;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            try
            {
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new System.Net.NetworkCredential("sociomania.team4@gmail.com", "dssdproject");
                client.Send(Message);
                return null;
            }
            catch (Exception exp)
            {
                return exp.Message;
            }
        }

        //LATER
        public void ForgotPasswordEmail(string MailTo, string Password)
        {
            string Template = GetEmailTempalte();
            StringBuilder sb = new StringBuilder();
            MailMessage Message = new MailMessage();
            Message.To.Add(MailTo);
            Message.From = new MailAddress("mzuhairqadir@gmail.com");
            Message.IsBodyHtml = true;
            Message.Subject = "New Password";
            Message.Priority = MailPriority.High;
            sb.Append("<table><tr><td colspan=\"3\" align=\"center\"><strong>Your new Password is:</strong>&nbsp; <font style=\"font:Arial, Helvetica, sans-serif; font-weight:bold; font-size:14px;\">" + Password + "</font></td></tr><tr><td align=\"center\">Click here to go to <a href=\"http://www.dev.SocioMania.com\"><font style=\"font:Arial, Helvetica, sans-serif; font-weight:bold; color:#0033FF font-size:14px;\">Stage Click</font></a> Site.  </td></tr></table>");
            Template = Template.Replace("[subject]", "New Password");
            Template = Template.Replace("[body]", sb.ToString());
            Message.Body = Template;
            SmtpClient client = new SmtpClient();
            try
            {
                client.Send(Message);
            }
            catch
            {

            }
        }
        public void ResetPassowrdEmail(string MailTo, string pPassword)
        {
            string Template = GetEmailTempalte();
            EncDec key = new EncDec();
            StringBuilder sb = new StringBuilder();
            MailMessage Message = new MailMessage();
            Message.To.Add(MailTo);
            Message.IsBodyHtml = true;
            Message.Subject = "[SocioManaia] Reset password";
            Message.Priority = MailPriority.High;
            sb.Append("Following is your new password.<br/><br/>");
            sb.Append("<table><tr><td colspan=\"3\" align=\"center\"><strong>Your new Password is:</strong>&nbsp; <font style=\"font:Arial, Helvetica, sans-serif; font-weight:bold; font-size:14px;\">" + pPassword.ToString() + "</font></td></tr><tr><td align=\"center\">  </td></tr></table>");
            sb.Append("<br/><br/>Use it to Log In back to SocioMania and change your password.<br/><br/>");
            Template = Template.Replace("[subject]", "[SocioManaia] Reset password");
            Template = Template.Replace("[body]", sb.ToString());
            Message.Body = Template;
            SmtpClient client = new SmtpClient();
            try
            {
                client.EnableSsl = true;
                client.Send(Message);
            }
            catch
            {

            }
        }

    }
}
