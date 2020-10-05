using PakLawAdvisor.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PakLawAdvisor.Controllers
{
    public class AdminController : Controller
    {
        private pladbEntities db = new pladbEntities();
        //
        // GET: /Admin/
        public Boolean Check_Session(){
             int id = Convert.ToInt32(Session["AdminId"]);
             admin adm=db.admins.Find(id);
             if (Session["AdminEmail"] != null && Session["AdminPassword"] != null && Session["AdminId"].Equals(adm.ADMIN_ID.ToString()))
               {
                return true;
               }
        
                return false;
           }

        public ActionResult AddCitation() {
            genericsController gn = new genericsController();
            ViewBag.crime_list = new SelectList(gn.Get_Crime_List(), "Text", "Value");
            return View();
        }

        public ActionResult Index()
        {
            if (!Check_Session())
            {
               return RedirectToAction("SignIn", "user");
            }
            return View(db.books.ToList());
        }
        [HttpPost]
        public ActionResult add_news(FormCollection fc)
        {
            string newstitle = fc["news_title"];
            string summary = fc["news_summary"];
            string discription = fc["news_description"];
            news add_news = new news();
            add_news.Title = newstitle;
            add_news.Summary = summary;
            add_news.discription = discription;
            add_news.date_time = DateTime.Now;
           
            db.news.Add(add_news);
            db.SaveChanges();
            return RedirectToAction("index");
        }
        [HttpPost]
        public ActionResult upload_book(HttpPostedFileBase file)
        {

            /*Geting the file name*/
            string filename = System.IO.Path.GetFileName(file.FileName);
            /*Saving the file in server folder*/
            file.SaveAs(Server.MapPath("~/books/" + filename));
            string filepathtosave = "~/books/" + filename;
            /*Storing image path to show preview*/
            //   ViewBag.ImageURL = filepathtosave;
            
            book bks = new book();
            bks.name = file.FileName;
            bks.file = filepathtosave;
            db.books.Add(bks);
            // db.Entry(bks).State = EntityState.Added;
            db.SaveChanges();
            return RedirectToAction("index");
        }
        public ActionResult Delete_Book(int id) {
            book bok = db.books.Find(id);
            db.books.Remove(bok);
            db.SaveChanges();
            return RedirectToAction("index");
        }

        public ActionResult Admin_setting() {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }
            int id = Convert.ToInt32(Session["userid"]);
            var u = db.lawyers.Find(id);
            if (u == null)
            {
                return RedirectToAction("SignIn", "user");
            }
            return View();
        }


        [HttpPost]
        public ActionResult Admin_setting(FormCollection set)
        {
            int id = Convert.ToInt32(Session["Adminid"]);
            admin u = db.admins.Find(id);
            if (u != null)
            {
                string oldpswd = set["oldpassword"].ToString();
                string newpswd = set["newpassword"].ToString();
                if (oldpswd == u.PASSWORD)
                {
                    u.PASSWORD = newpswd;
                    db.Entry(u).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.message = "Password Successfully changed..!";
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
        public PartialViewResult changepswd()
        {

            return PartialView("_changepswd");
        }

        public ActionResult admin_Details()
            {
                if (!Check_Session())
                {
                    return RedirectToAction("SignIn", "user");
                }
                return View();
            }

        public PartialViewResult searchLawyer()
        {

            return PartialView("_searchLawyer");
        }
        public ActionResult User_List()
        {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }
            return View(db.lawyers.ToList());
        }
        public ActionResult unblock_lawyer(int id)
        {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }
            lawyer usr = db.lawyers.Find(id);
            if (usr == null)
            {
                return HttpNotFound();
            }

            usr.IS_ACTIVE = true;
            db.Entry(usr).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("User_List");
        }

        public ActionResult block_lawyer(int id)
        {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }
            lawyer usr = db.lawyers.Find(id);
            if (usr == null)
            {
                return HttpNotFound();
            }
            usr.IS_ACTIVE = false;
            db.Entry(usr).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("User_List");
        }
        public ActionResult LawyerProfile(int id) {

            lawyer lawyer= db.lawyers.Find(id);
            ViewBag.adminlogin = Check_Session();

            return View(lawyer);
        }
        public ActionResult lawyer_list() {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }
            var v = db.lawyers.ToList();
            return View(v);
        }

        [HttpPost]
        public PartialViewResult searchLawyer(FormCollection fc)
        {
            
            string name = fc["lwr_search"].ToString();
            var related_lawyer = db.lawyers.Where(m=>m.First_Name.Equals(name)).ToList();
            return PartialView("_lawyer_list",related_lawyer);
        }
        //get
        [HttpPost]
        public ActionResult delete_lawyer(int id)
        {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }
            lawyer lwr = db.lawyers.Find(id);
            if (lwr == null)
            {
                return HttpNotFound();
            }
            db.lawyers.Remove(lwr);
            db.SaveChanges();
            return RedirectToAction("lawyer_list");
        }
        public ActionResult delete_lawyer()
        {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }
            return View();
        }
        //get
        public ActionResult confirm_delete()
        {

            return View();
        }
        //get
        public ActionResult account_setting()
        {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }
            return View();
        }
        public ActionResult account_setting(FormCollection fc)
        {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }
            return View();
        }
        public ActionResult Add_news() {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }
            return View();
        }
        
    
    }
}
