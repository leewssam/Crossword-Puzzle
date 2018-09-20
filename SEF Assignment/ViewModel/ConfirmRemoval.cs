using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEF_Assignment.ViewModel
{
    public class ConfirmRemoval
    {
        public String StudentID { get; set; }
        public String StudentName { get; set; }


    }

    public class RemoveList
    {
        public List<ConfirmRemoval> ViewRemoveList { get; set; }
    }
}