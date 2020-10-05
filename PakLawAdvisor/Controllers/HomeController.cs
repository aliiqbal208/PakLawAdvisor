using System;
using PagedList;
using PagedList.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using PakLawAdvisor.Models;
namespace PakLawAdvisor.Controllers
{
    public class HomeController : Controller
    {
        private pladbEntities db = new pladbEntities();
        genericsController gn = new genericsController();
        //
        // GET: /Home/

        public ActionResult Index()
        {
            ViewBag.books = db.books.Take(5).ToList();
            ViewBag.news = db.news.OrderBy(m => m.date_time).Take(3).ToList();
            return View();
        }

        public ActionResult news_detail(int id)
        {
            var v = db.news.Where(m => m.news_id.Equals(id)).First();
            return View(v);
        }
        public FileResult Download(int id)
        {

            var v = db.books.Where(m => m.Book_id == id).First();
            return File("~/Books/" + v.name, System.Net.Mime.MediaTypeNames.Application.Octet);
        }
        public ActionResult GetLawHelp(string crime = "Abetment")
        {
            ViewBag.crime_list =new SelectList(gn.Get_Crime_List(), "Text", "Value");

            var law = db.law_catagry.Where(m=>m.catgry_name==crime||crime==null).ToList();
            return View(law);
        }
        public ActionResult view_detail(string id)
        {
            var v=db.ccps.Where(m => m.Sec_id.Equals(id)).First();
            return View(v);
        }

        public ActionResult getbestlwr()
        {
          
            ViewBag.city_list_dp = new SelectList(gn.Get_City_list(), "Text", "Value");  
            // drop down for Crime catagry list  
            ViewBag.crim_list_dp = new SelectList(gn.Get_Crime_List(), "Text", "Value");  

            return View();
        }
        public ActionResult LawyerProfile(int id) {
         lawyer lwr=db.lawyers.Find(id);

            return View(lwr);
        }
        [HttpPost]
        public PartialViewResult getbestlwr(FormCollection obj)
        {
            string cat_name = obj["crime"].ToString();
            string cityname = obj["city"].ToString();
            var v = db.law_catagry.Where(m => m.catgry_name.Equals(cat_name)).First();
            var cat_id = v.Law_cat_id;
            //var cat_id = v.Law_cat_id;
            var lwrids = db.lwr_catgry.Where(lrct => lrct.cat_id == cat_id).ToList();
            List<int> ids=new List<int>();
            foreach(var lst in lwrids)
            {
            int a=(int)lst.lwr_id;
            ids.Add(a);
            }
            var related_lawyers = db.lawyers.Where(item => ids.Contains(item.lwr_id) && item.Area.Equals(cityname)).ToList();


            //var related_lawyers =
            //       (from lr in db.lawyers
            //       join ctgry in db.lwr_catgry on  lr.lwr_id equals ctgry.lwr_id

            //       where ctgry.cat_id==cat_id
            //       select new  lawyer{
            //       Address=lr.Address,
            //       Area=lr.Area,
            //       //case_exmpl=lr.case_exmpl,
            //       court_Level=lr.court_Level,
            //       DOB=lr.DOB,
            //       EMAIL=lr.EMAIL,
            //       Experience=lr.Experience,
            //       First_Name=lr.First_Name,
            //       IS_ACTIVE=lr.IS_ACTIVE,
            //       //lwr_catgry=lr.lwr_catgry,
            //       Objective=lr.Objective,
            //       lwr_id=lr.lwr_id,
            //       Phone_No=lr.Phone_No,
            //       photo=lr.photo,
            //       Second_Name=lr.Second_Name,
            //       VERIFY_CODE=lr.VERIFY_CODE,
            //       Vision=lr.Vision

            //       }).ToList(); 

           // var related_lawyers = db.lawyers.Where(n => n.Area.Equals(cityname) && n.catgry_id == cat_id).ToList();
            //var related_lawyers = all_lawyers.Where(m => m.lwr_catgry.All(o => o.cat_id.Equals( cat_id))).ToList();
            return PartialView("_lwr_list", related_lawyers.OrderByDescending(lu => lu.DOB));
        }
        public ActionResult GetBestLawyer()
        {
            return View();
        }
        [HttpPost]
        public PartialViewResult GetBestLawyer2(FormCollection obj)
        {
            string cat_name = obj["dd_case"].ToString();
            string cityname = obj["dd_city"].ToString();
            var v = db.law_catagry.Where(m => m.catgry_name.Equals(cat_name)).First();
            var cat_id=v.Law_cat_id;
            var all_lawyers = db.lawyers.Where(n => n.Area.Equals(cityname)).ToList();
            var related_lawyers = all_lawyers.Where(m => m.lwr_catgry.All(o => o.cat_id==cat_id)).ToList();



            /*ViewBag["listst"]= lwr_ct.ToString();*/

            return PartialView(related_lawyers);
        }

        [HttpPost]
        public ActionResult GetLawPredictionComp(FormCollection form) {
            Prediction pre = new Prediction();
            string catgry_ID = form["crime"];
            int Have_witness = Int32.Parse(form["IsRelitive"]);
            int number_witness =Int32.Parse(form["witness"]);
            int Is_witnessRelitive =Int32.Parse(form["IsRelitive"]);          
            int Have_otherevidence = Int32.Parse(form["other_evidence"]);
            int OathStatement = Int32.Parse(form["Statement"]);
            pre.Complainentvalue=calculatePredictionForComplainent(catgry_ID, Have_witness, number_witness, Is_witnessRelitive, Have_otherevidence, OathStatement);
            pre.Suspetedvalue = 100 - pre.Complainentvalue;
            return View(pre);
        }
        public ActionResult GetLawPredictionSuspected(FormCollection form)
        {
            Prediction pre = new Prediction();
            string catgry_ID = form["crime"];
            int IS_Conflict = Int32.Parse(form["IS_Conflict"]);
            int IsAtCrimePlace = Int32.Parse(form["IsAtCrimePlace"]);
            int IsEvidence = Int32.Parse(form["IsEvidence"]);
            int IsCrimanal = Int32.Parse(form["IsCrimanal"]);
            int Have_otherevidence = Int32.Parse(form["other_evidence"]);
            int OathStatement = Int32.Parse(form["Statement"]);
            pre.Suspetedvalue=calculatePredictionForSuspect(catgry_ID, IS_Conflict, IsAtCrimePlace,IsEvidence , IsCrimanal, Have_otherevidence, OathStatement);
            pre.Complainentvalue = 100 - pre.Suspetedvalue;

            return View(pre);
        }
        public float calculatePredictionForSuspect(string cat, int IS_Conflict, int IsAtCrimePlace, int IsEvidence, int IsCrimanal, int Have_otherevidence, int isOth)
        {
           
            int citationcount1 = db.citations.Where(c => c.CAT_ID == cat && c.RESULT == 1).Count();
            int citationcount2 = db.citations.Where(c => c.CAT_ID == cat && c.RESULT == 2).Count();
            float result = 0; Boolean iscat = false;
            int total = citationcount1 + citationcount2;
            if (total != 0)
            {
                iscat = true;
                float resCitation = (citationcount2 / total) * 100;
                if (resCitation > 90) { result += 9; }
                else if (resCitation > 80) { result += 8; }
                else if (resCitation > 70) { result += 7; }
                else if (resCitation > 60) { result += 6; }
                else if (resCitation > 50) { result += 5; }
                else if (resCitation > 40) { result += 4; }
                else if (resCitation > 30) { result += 3; }
                else if (resCitation > 20) { result += 2; }
                else if (resCitation > 10) { result += 1; }
                else if (resCitation > 0) { result += 0; }
            }

            //...............
            if (IS_Conflict == 1) { result += 8; }
            else { result += 3; }
            //............
            if (IsAtCrimePlace == 1) { result += 2; }
            else if (IsAtCrimePlace == 2) { result += 8; }
          
            //.............................
            if (IsEvidence == 1) { result += 8; }
            else if (IsEvidence == 2) { result += 2; }
           
            //............................
            if (IsCrimanal == 1) { result += 3; }
            else if (IsCrimanal == 2) { result += 7; }
            //..................
            if (Have_otherevidence == 1) { result += 8; }
            else if (Have_otherevidence == 2) { result += 3; }
            //............................ 
            if (isOth == 1) { result += 8; }
            else if (isOth == 2) { result += 2; }

            //..................calculation
            int total_Q = 50;
            if (iscat == true) { total_Q = 60; }
            float final_res = (result / total_Q) * 100;

            return final_res;
        }
        public float  calculatePredictionForComplainent(string cat,int Have_w,int numWitns,int IswinessRel,int haveothr,int isOth) {
              int citationcount1= db.citations.Where(c => c.CAT_ID == cat &&c.RESULT==1).Count();
              int citationcount2 = db.citations.Where(c => c.CAT_ID == cat && c.RESULT == 2).Count();
              float result = 0; Boolean iscat = false;  
              int total = citationcount1 + citationcount2;
              if (total != 0)
              {
                  iscat = true;
                  float resCitation = (citationcount1 / total) * 100;
                  if (resCitation > 90) { result += 9; }
                  else if (resCitation > 80) { result += 8; }
                  else if (resCitation > 70) { result += 7; }
                  else if (resCitation > 60) { result += 6; }
                  else if (resCitation > 50) { result += 5; }
                  else if (resCitation > 40) { result += 4; }
                  else if (resCitation > 30) { result += 3; }
                  else if (resCitation > 20) { result += 2; }
                  else if (resCitation > 10) { result += 1; }
                  else if (resCitation > 0)  { result += 0; }
              }
              
            //...............
              if (Have_w == 1){result += 8;}
              else {result += 3;}
            //............
                   if (numWitns == 0) { result += 0; }
              else if (numWitns == 1) { result += 4; }
              else if (numWitns == 2) { result += 6; }
              else if (numWitns ==3) { result += 8; }
            //.............................
                   if (IswinessRel == 1) { result +=5 ; }
              else if (IswinessRel == 2) { result += 9; }
              else if (IswinessRel == 3) { result += 7; }
            //............................
                   if (haveothr == 1) {result+=8; }
              else if (haveothr == 2) { result += 4; }
            //............................
                   if (isOth == 1) { result += 8; }
              else if (isOth == 2) { result += 2; }

            //..................calculation
                   int total_Q = 50;
                   if (iscat == true) { total_Q = 60; }
            float final_res= (result/total_Q)*100;

            return final_res;
        }
        public ActionResult GetLawPrediction()
        {
            var crime_list = db.law_catagry.ToList().OrderBy(m => m.catgry_name);
            List<SelectListItem> crime_listItem = new List<SelectListItem>();
            foreach (var crime in crime_list)
            {
                crime_listItem.Add(new SelectListItem() { Value = crime.catgry_name, Text = crime.Law_cat_id.ToString() });
            }
            ViewBag.crim_list_dp = new SelectList(crime_listItem, "Text", "Value");
            return View();
        }
        public ActionResult AboutUs()
        {
            return View();
        }

    }
}
