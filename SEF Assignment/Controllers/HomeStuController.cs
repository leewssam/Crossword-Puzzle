using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEF_Assignment.Controllers
{
    public class HomeStuController : Controller
    {
        // GET: HomeStu
        public ActionResult Index(string u)
        {
            Session["userID"] = u;

            return RedirectToAction("HomeStu") ;
        }

        public ActionResult HomeStu()
        {
            
            return View();
        }
        

        public ActionResult PuzzleStu()
        {

            return View();
        }

        public ActionResult RankingBoardStu()
        {

            return View();
        }

        public ActionResult DiscussionBoardStu()
        { 

            return View();
        }
    }
}
