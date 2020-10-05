using System;
using PagedList;
using PagedList.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using PakLawAdvisor.Models;
using System.IO;
using PakLawAdvisor.Helpers;
using System.Net.Mail;
using System.Net;

namespace PakLawAdvisor.Controllers
{
    public class LawyerController : Controller
    {
        private pladbEntities db = new pladbEntities();
        genericsController gn = new genericsController();

        public Boolean Check_Session()
        {
            int id = Convert.ToInt32(Session["UserId"]);
            lawyer lwr = db.lawyers.Find(id);
            if (Session["UserEmail"] != null && Session["UserPassword"] != null && Session["UserId"].Equals(lwr.lwr_id.ToString()) ) 
            {
                return true;
            }

            return false;
        }
        //
        // GET: /Lawyer/

        public ActionResult Index()
        {
            return View(db.lawyers.ToList());
        }


        //
        // GET: /Lawyer/Details/5
        public const int PageSize = 5;

        public ActionResult Case_list(int page = 1)
        {
            if (Check_Session())
            {
                var case_exmple = new PagedData();


                int id = Convert.ToInt32(Session["Userid"]);
                lawyer lawyer = db.lawyers.Find(id);
                var lawyer_id = lawyer.lwr_id;
                //case_exmpl cx = new case_exmpl();
                var related_cases = db.case_exmpl.Where(m => m.lwr_id.Equals(id)).ToList();
                case_exmple.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)db.case_exmpl.Count() / PageSize));
                case_exmple.CurrentPage = 1;
                return PartialView(related_cases.OrderBy(v => v.Date_time).Take(PageSize));

            }
            return RedirectToAction("SignIn", "user");
        }


       /* public ActionResult Lwr_Details()
        {
            if (Check_Session())
            {
                var case_exmple = new PagedData<case_exmpl>();
               

                int id = Convert.ToInt32(Session["Userid"]);
                lawyer lawyer = db.lawyers.Find(id);
                var lawyer_id = lawyer.lwr_id;
                case_exmpl cx = new case_exmpl();
                var related_lawyer = db.lawyers.Where(m => m.lwr_id.Equals(id)).ToList();
                var rlt_lwr = related_lawyer.OrderByDescending(m => m.case_exmpl.OrderBy(n => n.Date_time));
                case_exmple.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)db.case_exmpl.Count() / PageSize));
                case_exmple.CurrentPage = 1;
               case_exmple.Data= rlt_lwr.OrderByDescending(v => v.DOB).Take(PageSize);
                return View();

            }
            return RedirectToAction("SignIn", "user");
        }*/

        public lawyer getLoginLawyer() {
            int id = Convert.ToInt32(Session["UserId"]);
            lawyer lawyer = db.lawyers.Find(id);
            return lawyer;
        }
        
        public ActionResult Lwr_Details()
        {
         
            if (Check_Session() )
            {
                int id = Convert.ToInt32(Session["Userid"]);
                lawyer lawyer = db.lawyers.Find(id);
                    var lawyer_id = lawyer.lwr_id;
                    ViewData["User"] = lawyer.First_Name;
                    ViewBag.lawyer = getLoginLawyer();
                    //case_exmpl cx = new case_exmpl();
                    //var related_lawyer = db.lawyers.Where(m=> m.lwr_id.Equals(id)).ToList();
                    //var rlt_lwr=related_lawyer.OrderByDescending(m=>m.case_exmpl.OrderBy(n=>n.Date_time));
                    //return View(rlt_lwr.OrderByDescending(v => v.DOB).ToPagedList(page, 3));

                    return View(lawyer);
            }
            return RedirectToAction("SignIn", "user");
        }
        public PartialViewResult singed_user() {
            return PartialView("_singed_user");
        }
        //
        // GET: /Lawyer/Create

       // public ActionResult Create()
        //{
          //  return View();
        //}

        //
        // POST: /Lawyer/Create

        [AllowAnonymous]
        //public ActionResult Create(lawyer user)
        //{


        //        Session["userid"] = lwr.lwr_id;
        //        Session["UserEmail"]=lwr.EMAIL;
        //        Session["UserPassword"] =lwr.PASSWORD;


        //        return RedirectToAction("edit_profile", "Lawyer");


        //    return View(user);
        //}



        //
        // GET: /Lawyer/Edit/5
      
        public ActionResult Edit_Profile()
        {
            ViewBag.lawyer = getLoginLawyer();
            int id = Convert.ToInt32(Session["userid"]);
            lawyer lawyer = db.lawyers.Find(id);
            if (lawyer == null)
            {
                return RedirectToAction("SignIn","user");
            }
            law_catagry law_ct = new law_catagry();
            ViewBag.categories = db.law_catagry.ToList();
            ViewBag.statevalues = new SelectList(gn.Get_City_list(), "Text", "Value");
            
            
            return View(lawyer);
        }
        [HttpPost]
        public JsonResult GetCities(string Id)
        {
            var state_id = db.states.Where(m => m.State_Name == Id).First();
            //var related_cities = db.cities.Where(c => c.State_id.Equals(state_id.State_id)).ToList(); 
            List<SelectListItem> citieslist = new List<SelectListItem>();
            var cities = new List<string>();
            var value = new List<string>();
            foreach (var related_city in state_id.cities)
            {
                citieslist.Add(new SelectListItem() { Value = related_city.City_Name, Text = related_city.City_Name });
            
            }

            return Json(new SelectList(citieslist.ToArray(), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        public ActionResult confirmLawyer(string Verfy_code)
        {
            
            lawyer lwr = db.lawyers.Where(lr => lr.VERIFY_CODE == Verfy_code).FirstOrDefault();
            lwr.IS_ACTIVE = true;
            db.Entry(lwr).State = EntityState.Modified;
            db.SaveChanges();
            return View(lwr);
        }
        public ActionResult ConfirmationMessage() {
           

            return View();
        }
        public bool SendMail( lawyer lwr)
        {

            string from = "Sufyan.pyxel@gmail.com";
            using (MailMessage mail = new MailMessage(from, lwr.EMAIL))
            {
                mail.Subject = "Verification Mail";
                
                mail.Body = "Hi Dear "+lwr.First_Name+"! <html><body><a href='http://localhost:22118/Lawyer/confirmLawyer/?Verfy_code="+lwr.VERIFY_CODE + "'>Click Here</a></body></html>";
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




        [HttpPost]
        public ActionResult Edit_Profile(HttpPostedFileBase file)
        {
            try
            {
                int id = Convert.ToInt32(Session["userid"]);

                /*Geting the file name*/
                string filename = System.IO.Path.GetFileName(file.FileName);
                /*Saving the file in server folder*/
                file.SaveAs(Server.MapPath("~/images/"+filename));
                string filepathtosave = "~/images/" +filename;
                /*Storing image path to show preview*/
                ViewBag.ImageURL = filepathtosave;
                /*
                 * HERE WILL BE YOUR CODE TO SAVE THE FILE DETAIL IN DATA BASE
                 *
                 */
                lawyer lawyr = db.lawyers.Find(id);
                if (lawyr != null)
                {
                    lawyr.photo = filepathtosave;
                    db.Entry(lawyr).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Message = "File Uploaded successfully.";
                    ViewBag.lawyer = getLoginLawyer();
                    return RedirectToAction("Edit_Profile", "lawyer", new { id = lawyr.lwr_id });
                }
               
            }
            catch
            {
                ViewBag.Message = "Error while uploading the files.";
            }
            return View();
                
        }
        [HttpPost]
        public ActionResult Save_profile(lawyer lr)
        {

            int id = Convert.ToInt32(Session["userid"]);
            lawyer lawyr = db.lawyers.Find(id);
            if (lawyr != null)
            {
                lawyr.Address = lr.Address;
                lawyr.First_Name = lr.First_Name;
                lawyr.court_Level = lr.court_Level;
                lawyr.Experience = lr.Experience;
                lawyr.Second_Name = lr.Second_Name;
                lawyr.DOB = lr.DOB;
                lawyr.Area = lr.Area;
                lawyr.Vision = lr.Vision;
                lawyr.Objective = lr.Objective;
                db.Entry(lawyr).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.lawyer = getLoginLawyer();
                return RedirectToAction("Lwr_Details", "Lawyer", new { id = lawyr.lwr_id });
            }
            else
            { return RedirectToAction("Edit_Profile", "Lawyer"); }
        }
        public ActionResult AddNewRecord() {
            int id = 0;
                id=Convert.ToInt32(Session["userid"]);
            lawyer lawyer = db.lawyers.Find(id);
            if (lawyer == null)
            {
                return RedirectToAction("SignIn", "user");
            }
            var crime_list = db.law_catagry.ToList().OrderBy(m => m.catgry_name);
            List<SelectListItem> crime_listItem = new List<SelectListItem>();
            foreach (var crime in crime_list)
            {
                crime_listItem.Add(new SelectListItem() { Value = crime.catgry_name, Text = crime.catgry_name });
            }
            ViewBag.lawyer = getLoginLawyer();
            ViewBag.crim_list_dp = new SelectList(crime_listItem, "Text", "Value");  
            return View();
        }
        [HttpPost]
        public ActionResult addnewrecord(FormCollection form1)
        {

            int id = Convert.ToInt32(Session["userid"]);
            lawyer lawyr = db.lawyers.Find(id);
            if (lawyr != null)
            {
                string cat_name = form1["crime"].ToString();
                string summary = form1["summary"];
                string discription = form1["addDescription"].ToString();
                case_exmpl case_record = new case_exmpl();
                var v = db.law_catagry.Where(m => m.catgry_name.Equals(cat_name)).First();
                case_record.lwr_id = lawyr.lwr_id;
                case_record.Cat_name = v.catgry_name;
                case_record.Summary = summary;
                case_record.cat_id = v.Law_cat_id;
                case_record.discription = discription;
                case_record.Date_time = DateTime.Now;
                db.case_exmpl.Add(case_record);
                db.SaveChanges();
                ViewBag.message = "Successfully Added....!! ";
                ViewBag.lawyer = getLoginLawyer();
            }
            return View();
        }
 



        //
        // GET: /Lawyer/Delete/5

        public ActionResult Delete(int id = 0)
        {
            lawyer lawyer = db.lawyers.Find(id);
            if (lawyer == null)
            {
                return HttpNotFound();
            }
            return View(lawyer);
        }

        //
        // POST: /Lawyer/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            lawyer lawyer = db.lawyers.Find(id);
            db.lawyers.Remove(lawyer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult SaveCategries(FormCollection form)
        {
            string ctgry = form["category"];
            int lrID=Int32.Parse(Session["userid"].ToString());
            List<int> ctr = ctgry.Split(',').Select(Int32.Parse).ToList();
            foreach(var c in ctr)
            {
                lwr_catgry lwr = db.lwr_catgry.Where(l => l.cat_id == c && l.lwr_id == lrID).FirstOrDefault();
                if (lwr==null) {
                    lwr_catgry lawctr = new lwr_catgry();
                    lawctr.cat_id = c;
                    lawctr.lwr_id = Int32.Parse(Session["userid"].ToString());
                    db.lwr_catgry.Add(lawctr);
                    db.SaveChanges();
                    ViewBag.lawyer = getLoginLawyer();
                
                }
               
            }

            return RedirectToAction("Lwr_Details");
        }



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}