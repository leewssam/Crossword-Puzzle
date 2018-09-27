using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.ApplicationInsights.Web;
using SEF_Assignment.Models;
using SEF_Assignment.ViewModel;
using MySql.Data.MySqlClient;

namespace SEF_Assignment.Controllers
{
    public class ManageClassController : Controller
    {
        private SEF_AssignmentEntities db = new SEF_AssignmentEntities();

        [HttpGet]
        // GET:ManageClass
        public ActionResult Index()
        {
            return RedirectToAction("ManageClass");
        }

        [HttpGet]
        public ActionResult RankingBoard()
        {
            Session["LecID"] = Session["LecID"];
            TempData["step"] = "rankingboard";
            return RedirectToAction("ClassList");

        }
        [HttpGet]

        public ActionResult ManageClass()
        {
            Session["LecID"] = Session["LecID"];
            return View();
            
        }

        [HttpPost]
        //POST: GETACTION FROM MANAGE CLASS
        public ActionResult ManageClass(string action)
        {
            Session["LecID"] = Session["LecID"];
            if (action.Equals("regclass"))
            {
                return RedirectToAction("RegisterClass");
            }

            else if (action.Equals("genattendance"))
            {
                TempData["step"] = "attendance";
                return RedirectToAction("GenerateAttendance");
            }

            else if (action.Equals("genstudent"))
            {
                TempData["step"] = "generate";
                return RedirectToAction("ClassList");
            }

            else if (action.Equals("removestudent"))
            {
                TempData["step"] = "remove";
                return RedirectToAction("ClassList");
            }


            else if (action.Equals("home"))
            {
                return RedirectToAction("ChooseIdentity", "login");
            }

            else if (action.Equals("back"))
            {
                return RedirectToAction("HomeLec", "HomeLec");
            }

            return View();
        }


        [HttpGet]
        //GET: RegisterClass
        public ActionResult RegisterClass()
        {
            Session["LecID"] = Session["LecID"];
            Random rnd = new Random();
            int random;
            int classid;

            string connectionString = @"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            sqlConnection.Open();
            Boolean exists = true;
            while (exists == true)
            {
                random = rnd.Next(1, 999);
                System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT COUNT(Class_ID) FROM Class WHERE Class_ID='CL"+random+"';");
                sqlCommand.Connection = sqlConnection;
                Int32 count = (Int32)sqlCommand.ExecuteScalar();
                if (count ==0)
                {
                    exists = false;
                    classid = random;
                    string newclass = "CL"+classid.ToString();
                    var model = new RegisterClass
                    {
                        Class_ID = newclass
                    };
                    sqlConnection.Close();
                    return View(model);
                }

                else
                {
                    exists = true;
                }
            }

            return View();
        }

       [HttpPost]
        //POST:
        public ActionResult RegisterClass(string action,RegisterClass f)
        {
            Session["LecID"] = Session["LecID"];
            String LecID = Session["LecID"].ToString();
            LecID = LecID.ToString();

            if (action.Equals("home"))
            {
                return RedirectToAction("HomeLec", "HomeLec");
            }

            else if (action.Equals("back"))
            {
                return RedirectToAction("ManageClass");
            }

            else if (action.Equals("register"))
            {
                String ClassID = f.Class_ID;
                String ClassName = f.Class_Name;


                string connectionString = @"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);

                sqlConnection.Open();
                System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("INSERT INTO [SEF_AssignmentEntities].[dbo].[CLASS](\"CLASS_ID\", \"CLASS_NAME\", \"LEC_ID\") VALUES('" + ClassID+"','"+ClassName+"', '"+ LecID + "')");

                sqlCommand.Connection = sqlConnection;

                sqlCommand.ExecuteScalar();
                
                sqlConnection.Close();


                TempData["AlertMessage"] = "Registration was successful";

                return RedirectToAction("ManageClass");

            }

            return View();
        }

        [HttpGet]
        public ActionResult ClassList()
        {
            Session["LecID"] = Session["LecID"];
            String LecID = Session["LecID"].ToString();
            LecID = LecID.ToUpper();
            string connectionString = @"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            sqlConnection.Open();

            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT CLASS_ID FROM [SEF_AssignmentEntities].[dbo].[Class] WHERE CLASS.LEC_ID='" + LecID+"'");
            sqlCommand.Connection = sqlConnection;
            SqlDataReader myreader = sqlCommand.ExecuteReader();
            List<String> ClassIDList = new List<String>();
            while (myreader.Read())
            {
                ClassIDList.Add(myreader[0].ToString());
            }

            myreader.Close();

            sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT CLASS_NAME FROM [SEF_AssignmentEntities].[dbo].[Class] WHERE CLASS.LEC_ID='" + LecID + "'");
            sqlCommand.Connection = sqlConnection;

            SqlDataReader newmyreader = sqlCommand.ExecuteReader();
            List<String> ClassNameList = new List<String>();


            while (newmyreader.Read())
            {
                ClassNameList.Add(newmyreader[0].ToString());
            }

            sqlConnection.Close();

            List<ClassList> Class = new List<ClassList>();
            for (int i = 0; i < ClassIDList.Count; i++)
            {
                Class.Add(new ClassList() {ClassID = ClassIDList[i], ClassName = ClassNameList[i], IsCheck = false});
            }

            ClassesList CL = new ClassesList();
            CL.ViewClassList = Class;
            
            return View(CL);
        }


        [HttpPost]
        public ActionResult ClassList (ClassesList cl, string action)
        {

            if (action.Equals("home"))
            {
                return RedirectToAction("HomeLec", "HomeLec");
            }

            else if (action.Equals("back"))
            {
                String currentstep = TempData["step"].ToString();

                if (currentstep.Equals("rankingboard"))
                {
                    return RedirectToAction("HomeLec", "HomeLec");
                }
                else
                {
                    return RedirectToAction("ManageClass");
                }
            }

            else if (action.Equals("submit"))
            {
                StringBuilder sb = new StringBuilder();
                int counter = 0;
                foreach (var item in cl.ViewClassList)
                {
                    if (item.IsCheck)
                    {
                        sb.Append(item.ClassID);
                        counter++;

                    }
                }

                String content = sb.ToString();

                if (counter > 1)
                {
                    TempData["AlertMessage"] = "Please only insert one boxes! Returning to main menu.";
                    return RedirectToAction("ManageClass");
                }

                TempData["Class"] = content;
                Session["Class"] = content;
                String currentstep = TempData["step"].ToString();
                if (currentstep.Equals("generate"))
                {
                    return RedirectToAction("StudentList");
                }

                else if (currentstep.Equals("remove"))
                {
                    return RedirectToAction("RemoveStudent");
                }

                else if (currentstep.Equals("attendance"))
                {
                    return RedirectToAction("PuzzleList");
                }

                else if (currentstep.Equals("rankingboard"))
                {
                    return RedirectToAction("CreateRanking");
                }



            }
            return View();
        }

        [HttpGet]
        public ActionResult CreateRanking()
        {
            String ClassID = Session["Class"].ToString();
            string connectionString = @"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            sqlConnection.Open();

            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Stu_ID FROM [SEF_AssignmentEntities].[dbo].[Student] WHERE Class_Id='" + ClassID + "' ORDER BY STU_TOTALSCORE DESC");
            sqlCommand.Connection = sqlConnection;
            SqlDataReader myreader = sqlCommand.ExecuteReader();

            List<String> StudentIDList = new List<String>();
            List<String> StudentNameList = new List<String>();
            List<String> TotalScoreList = new List<string>();
            while (myreader.Read())
            {
                StudentIDList.Add(myreader[0].ToString());
            }
            myreader.Close();

            sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Stu_Name FROM [SEF_AssignmentEntities].[dbo].[Student] WHERE Class_Id='" + ClassID + "' ORDER BY STU_TOTALSCORE DESC");
            sqlCommand.Connection = sqlConnection;

            SqlDataReader newreader = sqlCommand.ExecuteReader();

            while (newreader.Read())
            {
                StudentNameList.Add(newreader[0].ToString());
            }
            newreader.Close();

            sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Stu_TotalScore FROM [SEF_AssignmentEntities].[dbo].[Student] WHERE Class_Id='" + ClassID + "' ORDER BY STU_TOTALSCORE DESC");
            sqlCommand.Connection = sqlConnection;

            SqlDataReader mynewreader = sqlCommand.ExecuteReader();

            while (mynewreader.Read())
            {
                TotalScoreList.Add(mynewreader[0].ToString());
            }

            mynewreader.Close();

            List<CreateRanking> Student = new List<CreateRanking>();

            for (int i = 0; i < StudentIDList.Count; i++)
            {
                Student.Add(new CreateRanking() { StudentID = StudentIDList[i], StudentName = StudentNameList[i], TotalScore = TotalScoreList[i] });
            }

            RankingsList CL = new RankingsList();
            CL.ViewRankingList = Student;
            return View(CL);
        }

        [HttpPost]
        public ActionResult CreateRanking(String action)
        {
            if (action.Equals("home"))
            {
                return RedirectToAction("HomeLec", "HomeLec");
            }

            else if (action.Equals("back"))
            {

                return RedirectToAction("HomeLec", "HomeLec");

            }

            return View();
        }
        
        [HttpGet]
        public ActionResult PuzzleList()
        {
            Session["Class"] = Session["Class"];
            Session["LecID"] = Session["LecID"];
            TempData["Class"] = TempData["Class"];
            TempData["StartDate"] = TempData["StartDate"];
            TempData["EndDate"] = TempData["EndDate"];
            String LecID = Session["LecID"].ToString();
            LecID = LecID.ToUpper();

            string connectionString = @"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            sqlConnection.Open();

            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Puzzle_ID FROM [SEF_AssignmentEntities].[dbo].[Puzzle] WHERE LEC_ID='" + LecID + "'");
            sqlCommand.Connection = sqlConnection;
            SqlDataReader myreader = sqlCommand.ExecuteReader();
            List<String> PuzzleIDList = new List<String>();
            while (myreader.Read())
            {
                PuzzleIDList.Add(myreader[0].ToString());
            }

            myreader.Close();

            sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Puzzle_Name FROM [SEF_AssignmentEntities].[dbo].[Puzzle] WHERE LEC_ID='" + LecID + "'");
            sqlCommand.Connection = sqlConnection;

            SqlDataReader newmyreader = sqlCommand.ExecuteReader();
            List<String> PuzzleNameList = new List<String>();
            while (newmyreader.Read())
            {
                PuzzleNameList.Add(newmyreader[0].ToString());
            }
            newmyreader.Close();

            List<PuzzleList> Puzzle = new List<PuzzleList>();

            for (int i = 0; i < PuzzleIDList.Count; i++)
            {
                Puzzle.Add(new PuzzleList() { PuzzleID = PuzzleIDList[i], PuzzleName = PuzzleNameList[i], IsCheck = false });
            }

            PuzzlesList CL = new PuzzlesList();
            CL.ViewPuzzleList = Puzzle;
            return View(CL);
        }


        [HttpPost]
        public ActionResult PuzzleList(PuzzlesList cl, string action)
        {
            Session["Class"] = Session["Class"];
            if (action.Equals("home"))
            {
                return RedirectToAction("HomeLec", "HomeLec");
            }

            else if (action.Equals("back"))
            {
                return RedirectToAction("ManageClass");
            }

            else if (action.Equals("genattendane"))
            {
                StringBuilder sb = new StringBuilder();
                int counter = 0;
                foreach (var item in cl.ViewPuzzleList)
                {
                    if (item.IsCheck)
                    {
                        sb.Append(item.PuzzleID);
                        counter++;

                    }
                }

                String content = sb.ToString();

                if (counter > 1)
                {
                    TempData["AlertMessage"] = "Please only insert one boxes! Returning to main menu.";
                    return RedirectToAction("ManageClass");
                }

                TempData["Puzzle"] = content;
                TempData["Class"] = TempData["Class"];
                Session["Class"] = Session["Class"];
                TempData["StartDate"] = TempData["StartDate"];
                TempData["EndDate"] = TempData["EndDate"];
                return RedirectToAction("AttendanceList");
            }

            return View();
        }



        [HttpGet]
        public ActionResult RemoveStudent()
        {
            String selectedclass = TempData["Class"].ToString();
            string connectionString = @"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            sqlConnection.Open();

            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Stu_ID FROM [SEF_AssignmentEntities].[dbo].[Student] WHERE CLASS_ID='" + selectedclass + "'");
            sqlCommand.Connection = sqlConnection;

            SqlDataReader myreader = sqlCommand.ExecuteReader();
            List<String> StudentIDList = new List<String>();
            while (myreader.Read())
            {
                StudentIDList.Add(myreader[0].ToString());
            }

            myreader.Close();

            sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Stu_Name FROM [SEF_AssignmentEntities].[dbo].[Student] WHERE CLASS_ID='" + selectedclass + "'");
            sqlCommand.Connection = sqlConnection;
            SqlDataReader newmyreader = sqlCommand.ExecuteReader();
            List<String> StudentNameList = new List<String>();

            while (newmyreader.Read())
            {
                StudentNameList.Add(newmyreader[0].ToString());
            }

            newmyreader.Close();
            sqlConnection.Close();

            List<RemoveStudent> Student = new List<RemoveStudent>();
            for (int i = 0; i < StudentIDList.Count; i++)
            {
                Student.Add(new RemoveStudent() { StudentID = StudentIDList[i], StudentName = StudentNameList[i], IsCheck = false });
            }

            RemovalList CL = new RemovalList();
            CL.ViewRemoveList = Student;

            return View(CL);

        }

        [HttpPost]
        public ActionResult RemoveStudent(RemovalList cl, string action)
        {
            if (action.Equals("home"))
            {
                return RedirectToAction("HomeLec", "HomeLec");
            }

            else if (action.Equals("back"))
            {
                return RedirectToAction("ManageClass");
            }

            else if (action.Equals("remove"))
            {
                int counter = 0;
                List<String> removelistid = new List<String>();
                List<String> removelistname = new List<String>();
                foreach (var item in cl.ViewRemoveList)
                {
                    if (item.IsCheck)
                    {
                        counter++;
                        removelistid.Add(item.StudentID);
                        removelistname.Add(item.StudentName);
                    }
                }

                if (counter == 0)
                {
                    TempData["AlertMessage"] = "You selected none student to be removed!";

                    return RedirectToAction("ManageClass");
                }

                else
                {
                    TempData["StudentRemovalID"] = removelistid;
                    TempData["StudentRemovalName"] = removelistname;

                    return RedirectToAction("ConfirmRemoval");
                }



            }
            return View();
        }

        [HttpGet]
        public ActionResult ConfirmRemoval()
        {
            List<String>removelistname = new List<String>();
            List<String>removelistid = new List<String>();
            TempData["StudentRemovalID"] = TempData["StudentRemovalID"];

            TempData["StudentRemovalName"] =TempData ["StudentRemovalName"];

            foreach (var i in (dynamic)TempData["StudentRemovalName"])
            {
                removelistname.Add(i.ToString());
            }

            foreach (var i in (dynamic)TempData["StudentRemovalID"])
            {
                removelistid.Add(i.ToString());
            }

            List<ConfirmRemoval> Student = new List<ConfirmRemoval>();

            for (int i = 0; i < removelistid.Count; i++)
            {
                Student.Add(new ConfirmRemoval() { StudentID = removelistid[i], StudentName = removelistname[i] });
            }

            RemoveList CL = new RemoveList();
            CL.ViewRemoveList = Student;

            return View(CL);
        }

        [HttpPost]
        public ActionResult ConfirmRemoval(String action, RemoveList cl)
        {
            if (action.Equals("home"))
            {
                return RedirectToAction("HomeLec", "HomeLec");
            }

            else if (action.Equals("back"))
            {
                return RedirectToAction("ManageClass");
            }

            else if (action.Equals("remove"))
            {

                List<String> removelistname = new List<String>();
                List<String> removelistid = new List<String>();
                foreach (var item in cl.ViewRemoveList)
                {
                    removelistid.Add(item.StudentID);
                }

                string connectionString = @"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);
                System.Data.SqlClient.SqlCommand sqlCommand;
                int tempcounter;
                sqlConnection.Open();
                for (int i = 0; i < removelistid.Count; i++)
                {
                    sqlCommand = new System.Data.SqlClient.SqlCommand("Delete from [SEF_AssignmentEntities].[dbo].[Student] WHERE \"STU_ID\" = '"+removelistid[i]+"'");
                    sqlCommand.Connection = sqlConnection;
                    tempcounter = sqlCommand.ExecuteNonQuery();
                }

                TempData["AlertMessage"] = "Removal successful";

                return RedirectToAction("ManageClass");
            }

            return View();
        }

        [HttpGet]
        public ActionResult AttendanceList()
        {
            Session["Class"] = Session["Class"];
            String PuzzleID = TempData["Puzzle"].ToString();
            String Class = TempData["Class"].ToString();
            String StartDate = TempData["StartDate"].ToString();
            String EndDate = TempData["EndDate"].ToString();


            string connectionString = @"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            sqlConnection.Open();

            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT STU_ID FROM [SEF_AssignmentEntities].[dbo].[Attempt] WHERE \"PUZZLE_ID\"='" + PuzzleID +"' AND CONVERT(VARCHAR(25), datetime_stamp, 126) BETWEEN '" + StartDate + "%' AND '" + EndDate + "%'");

            sqlCommand.Connection = sqlConnection;
            SqlDataReader myreader = sqlCommand.ExecuteReader();
            List<String> StudentIDList = new List<String>();
            while (myreader.Read())
            {
                StudentIDList.Add(myreader[0].ToString());
            }

            myreader.Close();

            sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Attempt_Score FROM [SEF_AssignmentEntities].[dbo].[Attempt] WHERE \"PUZZLE_ID\"='" + PuzzleID + "' AND CONVERT(VARCHAR(25), datetime_stamp, 126) BETWEEN '" + StartDate + "%' AND '" + EndDate + "%'");
            sqlCommand.Connection = sqlConnection;
            SqlDataReader newreader = sqlCommand.ExecuteReader();
            List<String> AttemptScore = new List<String>();
            while (newreader.Read())
            {
                AttemptScore.Add(newreader[0].ToString());
            }

            newreader.Close();

            sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT CONVERT(VARCHAR(25), datetime_stamp, 126) DateTime_Stamp FROM [SEF_AssignmentEntities].[dbo].[Attempt] WHERE \"PUZZLE_ID\"='" + PuzzleID + "' AND CONVERT(VARCHAR(25), datetime_stamp, 126) BETWEEN '" + StartDate + "%' AND '" + EndDate + "%'");
            sqlCommand.Connection = sqlConnection;
            SqlDataReader oldreader = sqlCommand.ExecuteReader();
            List<String> DateTime = new List<String>();
            while (oldreader.Read())
            {
                DateTime.Add(oldreader[0].ToString());
            }

            oldreader.Close();

            SqlDataReader newmyreader;
            List<String> StudentNameList = new List<String>();

            for (int i = 0; i < StudentIDList.Count; i++)
            {
                sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT STU_NAME FROM [SEF_AssignmentEntities].[dbo].[Student] WHERE \"Stu_ID\"='" + StudentIDList[i]+"'");
                sqlCommand.Connection = sqlConnection;
                newmyreader = sqlCommand.ExecuteReader();
                while (newmyreader.Read())
                {
                    StudentNameList.Add(newmyreader[0].ToString());
                }

                newmyreader.Close();

            }

            SqlDataReader reader;

            List<String> ClassNameList = new List<String>();

            for (int i = 0; i < StudentIDList.Count; i++)
            {
                sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT CLASS_ID FROM [SEF_AssignmentEntities].[dbo].[Student] WHERE \"Stu_ID\"='" + StudentIDList[i] + "'");
                sqlCommand.Connection = sqlConnection;
                reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    ClassNameList.Add(reader[0].ToString());
                }

                reader.Close();

            }

            for (int i = StudentIDList.Count - 1; i >= 0; --i)
            {
                ClassNameList[i] = ClassNameList[i].ToUpper();
                Class = Class.ToUpper();
                if (ClassNameList[i].Equals(Class))
                {

                }
                else
                {
                    ClassNameList.RemoveAt(i);
                    StudentNameList.RemoveAt(i);
                    DateTime.RemoveAt(i);
                    StudentIDList.RemoveAt(i);
                    AttemptScore.RemoveAt(i);
                }
            }

            Session["Class"] = Class;


            List<AttendanceList> List = new List<AttendanceList>();

            for (int i = 0; i < StudentNameList.Count; i++)
            {
                List.Add(new AttendanceList() { StudentID = StudentIDList[i], StudentName = StudentNameList[i], DateTime = DateTime[i], Score = AttemptScore[i]});
            }

            AttendancesList CL = new AttendancesList();
            CL.ViewAttendanceList = List;
            return View(CL);

        }

        [HttpPost]
        public ActionResult AttendanceList(String action, AttendancesList cl)
        {

            var doc1 = new Document();
            string path = String.Format("D:\\Users\\Sam\\Desktop\\Projects\\SEF\\SEF Assignment\\PDF");
            Session["LecID"] = Session["LecID"];
            TempData["Class"] = TempData["Class"];
            Session["Class"] = Session["Class"];

            String LecID = Session["LecID"].ToString();
            String selectedclass = Session["Class"].ToString();
            PdfWriter.GetInstance(doc1,
            new FileStream(path + "\\" + selectedclass + "_AttendanceList.pdf", FileMode.Create));
            LecID = LecID.ToString();

            if (action.Equals("home"))
            {
                return RedirectToAction("HomeLec", "HomeLec");
            }

            else if (action.Equals("back"))
            {
                return RedirectToAction("ManageClass");
            }

            else if (action.Equals("download"))
            {
                List<String> StudentIDList = new List<String>();
                List<String> StudentNameList = new List<String>();
                List<String> ScoreList = new List<String>();
                List<String> DateList = new List<string>();
                try
                {
                    foreach (var item in cl.ViewAttendanceList)
                    {
                        StudentIDList.Add(item.StudentID);
                        StudentNameList.Add(item.StudentName);
                        ScoreList.Add(item.Score);
                        DateList.Add(item.DateTime);
                    }

                    doc1.Open();
                    PdfPTable table = new PdfPTable(5);

                    table.TotalWidth = 500f;
                    table.LockedWidth = true;

                    float[] widths = new float[] {50f, 100f, 150f, 100f, 100f};

                    table.SetWidths(widths);
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 20f;
                    table.SpacingAfter = 30f;

                    int counter = 1;
                    PdfPCell cell = new PdfPCell(new Phrase("Attendance List for Class: " + selectedclass));
                    String stcounter;
                    cell.Colspan = 5;

                    cell.Border = 0;

                    cell.HorizontalAlignment = 1;

                    table.AddCell(cell);

                    for (int i = 0; i < StudentIDList.Count; i++)
                    {
                        stcounter = counter.ToString();
                        table.AddCell(stcounter);
                        counter++;
                        table.AddCell(StudentIDList[i]);
                        table.AddCell(StudentNameList[i]);
                        table.AddCell(ScoreList[i]);
                        table.AddCell(DateList[i]);
                    }

                    doc1.Add(table);
                    doc1.Close();

                    string filename = (selectedclass + "_AttendanceList.pdf");
                    Response.ContentType = "application/octet-stream";
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + filename);
                    string filepath =
                        String.Format("D:\\Users\\Sam\\Desktop\\Projects\\SEF\\SEF Assignment\\PDF\\" + filename);
                    //      string filepath = Server.MapPath("~/SavedFolder/" + filename);
                    Response.TransmitFile(filepath);
                    Response.End();
                    return RedirectToAction("ManageClass");
                }

                catch (Exception ex)
                {
                    return RedirectToAction("Error");
                }

            }


            return View();
        }

        [HttpGet]
        public ActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public ActionResult StudentList()
        {
            TempData["Class"] = TempData["Class"];
            Session["Class"] = TempData["Class"];
            String selectedclass = TempData["Class"].ToString();
            string connectionString = @"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            sqlConnection.Open();

            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Stu_ID FROM [SEF_AssignmentEntities].[dbo].[Student] WHERE CLASS_ID='" + selectedclass + "'");
            sqlCommand.Connection = sqlConnection;

            SqlDataReader myreader = sqlCommand.ExecuteReader();
            List<String> StudentIDList = new List<String>();
            while (myreader.Read())
            {
                StudentIDList.Add(myreader[0].ToString());
            }

            myreader.Close();

            sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Stu_Name FROM [SEF_AssignmentEntities].[dbo].[Student] WHERE CLASS_ID='" + selectedclass + "'");
            sqlCommand.Connection = sqlConnection;
            SqlDataReader newmyreader = sqlCommand.ExecuteReader();
            List<String> StudentNameList = new List<String>();

            while (newmyreader.Read())
            {
                StudentNameList.Add(newmyreader[0].ToString());
            }
            newmyreader.Close();
            sqlConnection.Close();

            var model = new StudentList
            {

                StudentNameList = StudentNameList,
                StudentIDList = StudentIDList
            }; return View(model);

        }

        [HttpPost]
        public ActionResult StudentList(string action)
        {
            var doc1 = new Document();
            string path = String.Format("D:\\Users\\Sam\\Desktop\\Projects\\SEF\\SEF Assignment\\PDF");

            Session["LecID"] = Session["LecID"];
            Session["Class"] = Session["Class"];
            String LecID = Session["LecID"].ToString();
            String selectedclass = Session["Class"].ToString();
            PdfWriter.GetInstance(doc1, new FileStream(path + "\\"+selectedclass+"_StudentList.pdf", FileMode.Create));

            LecID = LecID.ToString();

            if (action.Equals("home"))
            {
                return RedirectToAction("HomeLec", "HomeLec");
            }

            else if (action.Equals("back"))
            {
                return RedirectToAction("ManageClass");
            }

            else if (action.Equals("download"))
            {
                doc1.Open();
                PdfPTable table = new PdfPTable(3);

                table.TotalWidth = 300f;
                table.LockedWidth = true;



                //relative col widths in proportions - 1/3 and 2/3

                float[] widths = new float[] { 50f, 100f, 150f, };

                table.SetWidths(widths);

                table.HorizontalAlignment = 0;

                //leave a gap before and after the table

                table.SpacingBefore = 20f;

                table.SpacingAfter = 30f;


                int counter = 1;
                PdfPCell cell = new PdfPCell(new Phrase("Students List for Class: "+ selectedclass));
                String stcounter;
                cell.Colspan = 3;

                cell.Border = 0;

                cell.HorizontalAlignment = 1;

                table.AddCell(cell);



                string connect = @"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

                using (SqlConnection conn = new SqlConnection(connect))

                {

                    string query = "SELECT Stu_ID,Stu_Name FROM [SEF_AssignmentEntities].[dbo].[Student] WHERE CLASS_ID='" + selectedclass + "'";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    try

                    {

                        conn.Open();

                        using (SqlDataReader rdr = cmd.ExecuteReader())

                        {

                            while (rdr.Read())

                            {
                                stcounter = counter.ToString();
                                table.AddCell(stcounter);
                                counter++;
                                table.AddCell(rdr[0].ToString());
                                table.AddCell(rdr[1].ToString());

                            }

                        }

                    }

                    catch (Exception ex)

                    {

                        Response.Write(ex.Message);

                    }

                    doc1.Add(table);
                    doc1.Close();
                }

                string filename = (selectedclass + "_StudentList.pdf");
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + filename);
                string filepath = String.Format("D:\\Users\\Sam\\Desktop\\Projects\\SEF\\SEF Assignment\\PDF\\" + filename);
          //      string filepath = Server.MapPath("~/SavedFolder/" + filename);
                Response.TransmitFile(filepath);
                Response.End();
                return RedirectToAction("ManageClass");

            }

            return View();
        }


        [HttpGet]
        public ActionResult GenerateAttendance()
        {
            Session["LecID"] = Session["LecID"];
            TempData["step"] = TempData["step"];

            return View();
        }

        [HttpPost]
        public ActionResult GenerateAttendance(string action, GenerateAttendance cl)
        {
            Session["LecID"] = Session["LecID"];
            TempData["step"] = TempData["step"];
            
            if (action.Equals("home"))
            {
                return RedirectToAction("HomeLec", "HomeLec");
            }

            else if (action.Equals("back"))
            {
                return RedirectToAction("ManageClass");
            }

            else if (action.Equals("confirmdate"))
            {
                
                DateTime StartDate = cl.StartingDate;
                
                DateTime EndDate = cl.EndingDate;
                String stDate = StartDate.ToString("yyy-MM-dd");
                String endDate = EndDate.ToString("yyyy-MM-dd");

                TempData["StartDate"] = stDate;
                TempData["EndDate"] = endDate;
                return RedirectToAction("ClassList");
            }

            return View();
        }


    }
}
