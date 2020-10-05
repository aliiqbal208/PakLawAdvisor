using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
       
using System.Collections;

namespace PakLawAdvisor.Helpers
{

    public class PagedData
      {
        public IEnumerable Data { get; set; }
        public int NumberOfPages { get; set; }
        public int CurrentPage { get; set; }
      }
}
    
