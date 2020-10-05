using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PakLawAdvisor.Models
{
    public class SignUpUser
    {
           pladbEntities pladb = new pladbEntities();
          SignUp su = new SignUp();
          public int TotalUserCount()
          {

              return pladb.lawyers.Count();
          }
          public bool SignUpBO(SignUp su )
          {
              lawyer lwr = new lawyer();
             
              if (pladb.lawyers.Where(usr => usr.EMAIL == su.Email).FirstOrDefault()==null)
              {
                  
                  lwr.First_Name = su.First_Name;
                  lwr.Second_Name = su.Second_Name;
                  lwr.EMAIL = su.Email;
                  lwr.PASSWORD = su.Password;
                  lwr.IS_ACTIVE = false;
                  lwr.VERIFY_CODE =(DateTime.UtcNow.Ticks/6).ToString();
                  lwr.Phone_No = "Add Your Contact No #";
                  lwr.photo = "~/images/logo.png";
                  lwr.court_Level = "Add your court Level";
                  lwr.DOB = DateTime.Now;
                  lwr.Experience = "Not Set";
                  lwr.Area = "Area not set";
                  lwr.Address = "Address not set";
                  pladb.lawyers.Add(lwr);
                  pladb.SaveChanges();
                  
                  return true;
              }
              else
                  return false;
          }

         

    }
     
}