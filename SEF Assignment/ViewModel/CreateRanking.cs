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
    public class CreateRanking
    {
        public String StudentName { get; set; }
        public String StudentID { get; set; }
        public String TotalScore { get; set; }

    }

    public class RankingsList
    {
        public List<CreateRanking> ViewRankingList { get; set; }
    }
}