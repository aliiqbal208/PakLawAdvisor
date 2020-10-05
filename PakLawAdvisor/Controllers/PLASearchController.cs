using PakLawAdvisor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PakLawAdvisor.Controllers
{
    public class PLASearchController : Controller
    {
        //
        // GET: /PLASearch/

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult StartSearch(FormCollection form)
        {
            string search = form["search_field"];
            SearchBO srchbo = new SearchBO();
          
             
                List<lawyer> Searchedlwr = srchbo.SearchLawyers(search);
                ViewBag.lawyers = Searchedlwr;
           
                List<law_catagry> Searchedlaws = srchbo.SearchLaws(search);
                ViewBag.laws = Searchedlaws;  
            return View();
        }
    }
}
