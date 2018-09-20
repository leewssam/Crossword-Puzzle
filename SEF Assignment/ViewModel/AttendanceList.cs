using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEF_Assignment.ViewModel
{
    public class AttendanceList
    {
        public String StudentName { get; set; }
        public String StudentID { get; set; }
        public String Score { get; set; }
        public String DateTime { get; set; }


    }

    public class AttendancesList
    {
        public List<AttendanceList> ViewAttendanceList { get; set; }
    }
}