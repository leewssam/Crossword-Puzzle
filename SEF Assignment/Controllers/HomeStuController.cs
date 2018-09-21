using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SEF_Assignment.ViewModel;

namespace SEF_Assignment.Controllers
{
    public class HomeStuController : Controller
    {
        // GET: HomeStu
        public ActionResult Index(string u)
        {
            Session["userID"] = u;
            Session["StuID"] = Session["userID"];
            String StuID = Session["StuID"].ToString();
            String ClassID = "";
            String LecID ="";
            string connectionString = @"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            sqlConnection.Open();
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Class_ID FROM [SEF_AssignmentEntities].[dbo].[Student] WHERE STU_ID='" + StuID + "'");
            sqlCommand.Connection = sqlConnection;
            SqlDataReader myreader = sqlCommand.ExecuteReader();

            while (myreader.Read())
            {
                ClassID = myreader[0].ToString();
            }

            myreader.Close();

            sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Lec_ID FROM [SEF_AssignmentEntities].[dbo].[Class] WHERE Class_Id='" + ClassID + "'");
            sqlCommand.Connection = sqlConnection;
            SqlDataReader mynewreader = sqlCommand.ExecuteReader();

            while (mynewreader.Read())
            {
                LecID = mynewreader[0].ToString();
            }

            mynewreader.Close();

            Session["StuID"] = Session["userID"];
            Session["ClassID"] = ClassID;
            Session["LecID"] = LecID;


            return RedirectToAction("HomeStu") ;
        }

        public ActionResult HomeStu()
        {

            Session["StuID"] = Session["StuID"];
            Session["ClassID"] = Session["ClassID"];
            Session["LecID"] = Session["LecID"];


            return View();
        }

        [HttpGet]
        public ActionResult PuzzleStu()
        {

            Session["StuID"] = Session["StuID"];
            Session["ClassID"] = Session["ClassID"];
            Session["LecID"] = Session["LecID"];
            

            String LecID = Session["LecID"].ToString();
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
        public ActionResult PuzzleStu(PuzzlesList cl, string action)
        {
            Session["StuID"] = Session["StuID"];
            Session["ClassID"] = Session["ClassID"];
            Session["LecID"] = Session["LecID"];

            if (action.Equals("home"))
            {
                return RedirectToAction("HomeStu");
            }

            else if (action.Equals("back"))
            {
                return RedirectToAction("HomeStu");

            }

            else if (action.Equals("play"))
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
                    return RedirectToAction("HomeStu");
                }

                Session["PuzzleID"] = content;
                return RedirectToAction("PlayPuzzle");
            }

            return View();
        }

        [HttpPost]
        public ActionResult PlayPuzzle()
        {
            Session["StuID"] = Session["StuID"];
            Session["ClassID"] = Session["ClassID"];
            Session["LecID"] = Session["LecID"];
            Session["PuzzleID"] = Session["PuzzleID"];

        }

        [HttpGet]
        public ActionResult RankingBoardStu()
        {

            Session["StuID"] = Session["StuID"];
            Session["ClassID"] = Session["ClassID"];
            Session["LecID"] = Session["LecID"];

            String StuID = Session["StuID"].ToString();
            String ClassID = Session["ClassID"].ToString();

            List<String> StudentIDList = new List<String>();
            List<String> StudentNameList = new List<String>();
            List<String> TotalScoreList = new List<string>();
            

            string connectionString = @"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);
            sqlConnection.Open();

            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Stu_ID FROM [SEF_AssignmentEntities].[dbo].[Student] WHERE Class_Id='" + ClassID + "'");
            sqlCommand.Connection = sqlConnection;
            SqlDataReader mynewreader = sqlCommand.ExecuteReader();

            while (mynewreader.Read())
            {
                StudentIDList.Add(mynewreader[0].ToString());
            }

            mynewreader.Close();

            sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Stu_Name FROM [SEF_AssignmentEntities].[dbo].[Student] WHERE Class_Id='" + ClassID + "'");
            sqlCommand.Connection = sqlConnection;

            SqlDataReader newreader = sqlCommand.ExecuteReader();

            while (newreader.Read())
            {
                StudentNameList.Add(newreader[0].ToString());
            }
            newreader.Close();

            sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Stu_TotalScore FROM [SEF_AssignmentEntities].[dbo].[Student] WHERE Class_Id='" + ClassID + "'");
            sqlCommand.Connection = sqlConnection;

            SqlDataReader newmyreader = sqlCommand.ExecuteReader();

            while (newmyreader.Read())
            {
                TotalScoreList.Add(newmyreader[0].ToString());
            }

            newmyreader.Close();

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
        public ActionResult RankingBoardStu(string action)
        {
            Session["StuID"] = Session["StuID"];
            Session["ClassID"] = Session["ClassID"];
            Session["LecID"] = Session["LecID"];

            if (action.Equals("home"))
            {
                return RedirectToAction("HomeStu");
            }

            else if (action.Equals("back"))
            {
                return RedirectToAction("HomeStu");

            }

            return View();
        }

        public ActionResult DiscussionBoardStu()
        { 

            return View();
        }
    }
}
