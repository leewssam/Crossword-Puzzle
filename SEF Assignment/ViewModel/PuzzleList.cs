using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SEF_Assignment.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace SEF_Assignment.ViewModel
{
    public class PuzzleList
    {
        public String PuzzleName { get; set; }
        public String PuzzleID { get; set; }
        public Boolean IsCheck { get; set; }

    }

    public class PuzzlesList
    {
        public List<PuzzleList> ViewPuzzleList { get; set; }
    }
}