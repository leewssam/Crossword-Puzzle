using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEF_Assignment.ViewModel
{
    public class RemoveStudent
    {
        public String StudentID { get; set; }
        public String StudentName { get; set; }
        public Boolean IsCheck { get; set; }
  
    }

    public class RemovalList
    {
        public List<RemoveStudent> ViewRemoveList { get; set; }
    }
}