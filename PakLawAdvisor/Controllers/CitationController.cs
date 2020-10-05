using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PakLawAdvisor.Models;

namespace PakLawAdvisor.Controllers
{
    public class CitationController : Controller
    {
        private pladbEntities db = new pladbEntities();


        public Boolean Check_Session()
        {
            int id = Convert.ToInt32(Session["AdminId"]);
            admin adm = db.admins.Find(id);
            if (Session["AdminEmail"] != null && Session["AdminPassword"] != null && Session["AdminId"].Equals(adm.ADMIN_ID.ToString()))
            {
                return true;
            }

            return false;
        }
        //
        // GET: /Citation/

        public ActionResult Index()
        {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }
            genericsController gn = new genericsController();
            ViewBag.crime_list = new SelectList(gn.Get_Crime_List(), "Text", "Value");
            return View(db.citations.ToList());
        }

        //
        // GET: /Citation/Details/5

        public ActionResult Details(int id = 0)
        {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }
            citation citation = db.citations.Find(id);
            if (citation == null)
            {
                return HttpNotFound();
            }
            return View(citation);
        }
  
        public ActionResult AddCitation()
        {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }
            genericsController gn = new genericsController();
            ViewBag.crime_list = new SelectList(gn.Get_Crime_List(), "Text", "Value");
            return View();
        }
        [HttpPost]
        public ActionResult AddCitation(citation cit)
        {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }
                cit.DATE_TIME = DateTime.Now;
                db.citations.Add(cit);
                db.SaveChanges();
                return RedirectToAction("Index");
        }

        //
        // GET: /Citation/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }
            citation citation = db.citations.Find(id);
            if (citation == null)
            {
                return HttpNotFound();
            }
            genericsController gn = new genericsController();
            ViewBag.crime_list = new SelectList(gn.Get_Crime_List(), "Text", "Value");
            return View(citation);
        }

        //
        // POST: /Citation/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(citation citation)
        {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }

            if (ModelState.IsValid)
            {
                db.Entry(citation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            genericsController gn = new genericsController();
            ViewBag.crime_list = new SelectList(gn.Get_Crime_List(), "Text", "Value");
            return View(citation);
        }

        //
        // GET: /Citation/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }
            citation citation = db.citations.Find(id);
            if (citation == null)
            {
                return HttpNotFound();
            }
            genericsController gn = new genericsController();
            ViewBag.crime_list = new SelectList(gn.Get_Crime_List(), "Text", "Value");
            return View(citation);
        }

        //
        // POST: /Citation/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!Check_Session())
            {
                return RedirectToAction("SignIn", "user");
            }
            citation citation = db.citations.Find(id);
            db.citations.Remove(citation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}