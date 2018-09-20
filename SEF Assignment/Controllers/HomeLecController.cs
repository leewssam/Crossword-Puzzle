using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEF_Assignment.Controllers
{
    public class HomeLecController : Controller
    {
        [HttpGet]
        public ActionResult Index(string u)
        {
            Session["LecID"] = u;
            TempData["userID"] = u;
            return RedirectToAction("HomeLec") ;
        }

        public ActionResult HomeLec()
        {
            
            return View();
        }
        

        public ActionResult ManagePuzzleLec()
        {

            return View();
        }

        public ActionResult RankingBoardLec()
        {

            return View();
        }

        public ActionResult DiscussionBoardLec()
        { 

            return View();
        }

        public ActionResult ManageClassLec()
        {

            return RedirectToAction("ManageClass","ManageClass");
        }
        
    }
}