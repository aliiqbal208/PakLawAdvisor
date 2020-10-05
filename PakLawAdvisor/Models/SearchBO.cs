using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PakLawAdvisor.Models
{
    public class mydata {
        public int lwrid { get; set; }
        public string lwrname { get; set; }

        public string City { get; set; }

        public string Experience { get; set; }

        public string Email { get; set; }
        public string ContactNo { get; set; }

    }
    public class SearchBO
    {
        pladbEntities db;


        public List<law_catagry> SearchLaws(string search)
        {
            db = new pladbEntities();
            // List<mydata> myd = new List<mydata>();
            //List<lawyer> searchedLawyerList = db.lawyers.Where(lwr => lwr.First_Name.Contains(search)).ToList();

            var SearchResult = (from law in db.law_catagry.AsEnumerable()
                                where law.catgry_name.Contains(search) 
                                select new law_catagry
                                {
                                 Law_cat_id=law.Law_cat_id,
                                 ccps=law.ccps,
                                 catgry_name=law.catgry_name,
                                 case_exmpl=law.case_exmpl,
                                 lwr_catgry=law.lwr_catgry                                

                                }).Take(5).ToList();
            return SearchResult;
        }
        public List<lawyer> SearchLawyers(string search)
        {
       db=new pladbEntities();
      // List<mydata> myd = new List<mydata>();
       //List<lawyer> searchedLawyerList = db.lawyers.Where(lwr => lwr.First_Name.Contains(search)).ToList();

       var SearchResult = (from lwr in db.lawyers.AsEnumerable()
                   where lwr.First_Name.Contains(search)|| lwr.Second_Name.Contains(search)
                   select new lawyer
               {
                   lwr_id=lwr.lwr_id,
                   First_Name = lwr.First_Name+" "+lwr.Second_Name,
                   photo=lwr.photo,
                   Area=lwr.Area,
                   EMAIL=lwr.EMAIL,
                   Experience=lwr.Experience,
                  // case_exmpl=lwr.case_exmpl,
                   //lwr_catgry=lwr.lwr_catgry,
                   Phone_No=lwr.Phone_No
                   
                    
               }).Take(5).ToList();
       return SearchResult; 
      }

    }
}