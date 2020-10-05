using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PakLawAdvisor.Models;
using System.Net.Mail;
using System.Net;

namespace PakLawAdvisor.Controllers
{
    public class userController : Controller
    {
        private pladbEntities db = new pladbEntities();


        //
        // GET: /user/SignUp

        public ActionResult SignUp()
        {
            return View();
        }

        //
        // POST: /user/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(SignUp su)
        {
                    SignUpUser spu = new SignUpUser();

                    if (spu.SignUpBO(su)) {
                        pladbEntities pladb = new pladbEntities();
                        lawyer lwr = pladb.lawyers.Where(lr => lr.EMAIL == su.Email).FirstOrDefault();
                        LawyerController lrc = new LawyerController();
                        lrc.SendMail(lwr);
                        pladb.Dispose();
                        return RedirectToAction("ConfirmationMessage", "lawyer");
                    }
                    
                    return View(su);        
        }
        public ActionResult ConfirmationSignUp() {

            return View();
        }
        //
        // GET: /user/SigIn
        public ActionResult SignIn()
        {

            return View();
        }
        [HttpPost]
        public ActionResult SignIn(FormCollection form)
        {
            var Email = form["email_login"].ToString();
            var Password = form["pass_login"].ToString();
            var type=form["domain"].ToString();
            if(type=="1"){
                        var v = db.lawyers.Where(m => m.EMAIL.Equals(Email) && m.PASSWORD.Equals(Password)&& m.IS_ACTIVE==true).FirstOrDefault();
                        if (v != null)
                        {
                            Session["UserEmail"] = v.EMAIL.ToString();
                            Session["UserId"] = v.lwr_id.ToString();
                            Session["UserPassword"] = v.PASSWORD.ToString();
                            return RedirectToAction("Lwr_Details", "lawyer");
                        }

                        return RedirectToAction("Signin", "user");
 
                      }
                else if(type=="2"){
                    var v = db.admins.Where(m => m.EMAIL.Equals(Email) && m.PASSWORD.Equals(Password)).FirstOrDefault();
                           if(v!=null){
                             Session["AdminEmail"] = v.EMAIL.ToString();
                             Session["AdminId"] = v.ADMIN_ID.ToString();
                             Session["AdminPassword"] = v.PASSWORD.ToString();
                             return RedirectToAction("index", "Admin");
                            }
                     return RedirectToAction("index", "Home");
                       }
                return View();
            }
              
        public ActionResult signout()
        {

            Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(lawyer lwr)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lwr).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lwr);
        }
        public ActionResult Account_setting()
        {
            int id = Convert.ToInt32(Session["UserId"]);
            var u = db.lawyers.Find(id);
            //LawyerController lr = new LawyerController();
            // = lr.getLoginLawyer();
            if (u == null)
            {
                return RedirectToAction("SignIn", "user");
            }
            ViewBag.lawyer = u;
            return View();
        }

        [HttpPost]
        public ActionResult Account_setting(FormCollection set)
        {
            int id = Convert.ToInt32(Session["userid"]);
           lawyer lr = db.lawyers.Find(id);
            if (lr != null)
            {


                string oldpswd = set["oldpassword"].ToString();
                string newpswd = set["newpassword"].ToString();
                if (oldpswd == lr.PASSWORD)
                {
                    lr.PASSWORD = newpswd;
                    db.Entry(lr).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.message = "Password Successfully changed..!";
                    ViewBag.lawyer = lr;
                    return View();
                }
                else
                {
                    ViewBag.message = "Enter correct Old password..!";
                    return View();
                }
            }

            RedirectToAction("SignIn", "user");
            return View();
        }
        public ActionResult account_deactive(FormCollection fc)
        {
            int id = Convert.ToInt32(Session["userid"]);
            lawyer u = db.lawyers.Find(id);
            if (u != null)
            {
                string pswd = fc["password"].ToString();
                if (pswd == u.PASSWORD)
                {
                    u.IS_ACTIVE = false;
                    db.Entry(u).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.message = "Account Deactivate Successfully..!";
                    return RedirectToAction("index", "home");
                }
                else
                {
                    ViewBag.message = "Sorry your account is not deactivated..";
                    return RedirectToAction("user", "Account_setting");
                }
            }
            return RedirectToAction("user", "Account_setting");
        }
        public PartialViewResult changepswd()
        {

            return PartialView("_changepswd");
        }
        public PartialViewResult deactive_act()
        {

            return PartialView("_deactive_act");
        }
        public ActionResult ForgetPassword() {

            return View();
        }
        [HttpPost]
        public ActionResult ForgetPasswordMessage(FormCollection form)
        {
            string Lwr_email=form["email"];
            AccountBO acbo = new AccountBO();
            if(acbo.ForgotPassword(Lwr_email))
            {
                ViewBag.ForgotMsg = true;
                return View();
            }
            else
            {
                ViewBag.ForgotMsg = false;
                return View();
            }
        }
     

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}