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
    public class ClassList
    {
        public String ClassID { get; set; }
        public String ClassName { get; set; }
        public Boolean IsCheck { get; set; }

    }

    public class ClassesList
    {
        public List<ClassList> ViewClassList { get; set; }
    }
}