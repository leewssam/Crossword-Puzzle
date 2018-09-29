using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using SEF_Assignment.ViewModel;
using trycross.Model;

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
        
        [HttpGet]
        public ActionResult ManagePuzzleLec()
        {
            Session["LecID"] = Session["LecID"];


            return View();
        }

        [HttpPost]
        public ActionResult ManagePuzzleLec(string action)
        {
            Session["LecID"] = Session["LecID"];

            if (action.Equals("home"))
            {
                return RedirectToAction("HomeLec", "HomeLec");
            }

            else if (action.Equals("back"))
            {
                return RedirectToAction("HomeLec", "HomeLec");
            }

            else if(action.Equals("create"))
            {
                return RedirectToAction("CreatePuz");
            }

            else if (action.Equals("remove"))
            {
                return RedirectToAction("RemovePuz");
            }

            else if (action.Equals("view"))
            {
                Session["ActionEquals"] = "ViewPuz";
                return RedirectToAction("LecPuzzleList");
            }

            return View();
        }

        [HttpGet]
        public ActionResult LecPuzzleList()
        {
            Session["LecID"] = Session["LecID"];
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
        public ActionResult LecPuzzleList(PuzzlesList cl, string action)
        {

            Session["LecID"] = Session["LecID"];
            if (action.Equals("home"))
            {
                return RedirectToAction("HomeLec", "HomeLec");
            }

            else if (action.Equals("back"))
            {

                return RedirectToAction("HomeLec");
            }

            else if (action.Equals("show"))
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
                    return RedirectToAction("HomeLec");
                }
                Session["Puzzle"] = content;


                return RedirectToAction("ShowPuzLec");
            }

            return View();
        }
        [HttpGet]
        public ActionResult ShowPuzLec()
        {
            Session["LecID"] = Session["LecID"];
            Session["Puzzle"] = Session["Puzzle"];

            string PuzzleID = Session["Puzzle"].ToString();
            List<String> AnswerList = new List<String>();
            List<String> QuestionList = new List<String>();


            string connectionString =
                @"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            System.Data.SqlClient.SqlConnection sqlConnection =
                new System.Data.SqlClient.SqlConnection(connectionString);
            sqlConnection.Open();

            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand(
                "SELECT Answer_Content FROM [SEF_AssignmentEntities].[dbo].[Answer] WHERE Puzzle_ID='" + PuzzleID +
                "'");
            sqlCommand.Connection = sqlConnection;

            SqlDataReader myreader = sqlCommand.ExecuteReader();
            while (myreader.Read())
            {
                AnswerList.Add(myreader[0].ToString());
            }

            Session["TotalAns"] = AnswerList.Count;
            string words = string.Join(" ", AnswerList.ToArray());
            myreader.Close();

            sqlCommand = new System.Data.SqlClient.SqlCommand(
                "SELECT Question_Content FROM [SEF_AssignmentEntities].[dbo].[Question] WHERE Puzzle_ID='" + PuzzleID +
                "'");
            sqlCommand.Connection = sqlConnection;
            SqlDataReader newreader = sqlCommand.ExecuteReader();

            while (newreader.Read())
            {
                QuestionList.Add(newreader[0].ToString().ToUpper());
            }
            newreader.Close();
            string[] input = AnswerList.ToArray();

            for (ushort i = 0; i < input.Length; i++)
            {
                input[i] = Regex.Replace((i + 1) + input[i], "[^\\w\\d]", "");
            }

            ushort width = 30;
            ushort height = 30;

            Canvas canvas = new CrosswordFactory(input, width, height, true).createCanvas();

            canvas.Build();

            ICanvasWriter canvasWriter = new HtmlCanvasWriter();

            ViewData["Width"] = width;
            ViewData["Height"] = height;
            ViewData["Words"] = words;
            ViewData["Canvas"] = canvas;
            ViewData["CanvasWriter"] = canvasWriter;

            sqlCommand = new System.Data.SqlClient.SqlCommand(
                "SELECT COUNT(*) FROM [SEF_AssignmentEntities].[dbo].[Answer] WHERE Puzzle_ID='" + PuzzleID + "'");
            sqlCommand.Connection = sqlConnection;

            int sumquestion = Convert.ToInt32(sqlCommand.ExecuteScalar());
            TempData["SumQuestion"] = sumquestion;
            var model = new PlayPuzzle
            {
                QuestionList = QuestionList,

            };
            return View(model);
        }

        [HttpGet]
        public ActionResult RemovePuz()
        {
            Session["LecID"] = Session["LecID"];
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
        public ActionResult RemovePuz(PuzzlesList cl,string action)
        {
            Session["LecID"] = Session["LecID"];

            if (action.Equals("home"))
            {
                return RedirectToAction("HomeLec", "HomeLec");
            }

            else if (action.Equals("back"))
            {
                return RedirectToAction("ManagePuzzleLec");
            }

            else if (action.Equals("remove"))
            {

                List<String> removelistid = new List<String>();
                List<String> removelistname = new List<string>();
                int counter = 0;
                foreach (var item in cl.ViewPuzzleList)
                {
                    if (item.IsCheck)
                    {
                        removelistname.Add(item.PuzzleName);
                        removelistid.Add(item.PuzzleID);
                        counter++;

                    }
                }

                if (counter == 0)
                {
                    TempData["AlertMessage"] = "You selected none puzzle to be removed!";
                    return RedirectToAction("HomeLec");
                }

                TempData["RemovalPuzzleID"] = removelistid;
                TempData["RemovalPuzzleName"] = removelistname;
                return RedirectToAction("ConfirmRemove");
            }

            return View();
        }

        [HttpGet]
        public ActionResult ConfirmRemove()
        {
            Session["LecID"] = Session["LecID"];
            List<String> removelistname = new List<String>();
            List<String> removelistid = new List<String>();

            TempData["RemovalPuzzleID"] = TempData["RemovalPuzzleID"];
            TempData["RemovalPuzzleName"] = TempData["RemovalPuzzleName"];

            foreach (var i in (dynamic)TempData["RemovalPuzzleName"])
            {
                removelistname.Add(i.ToString());
            }

            foreach (var i in (dynamic)TempData["RemovalPuzzleID"])
            {
                removelistid.Add(i.ToString());
            }

            List<ConfirmRemoval> Puzzle = new List<ConfirmRemoval>();

            for (int i = 0; i < removelistid.Count; i++)
            {
                Puzzle.Add(new ConfirmRemoval() { StudentID = removelistid[i], StudentName = removelistname[i] });
            }

            RemoveList CL = new RemoveList();
            CL.ViewRemoveList = Puzzle;

            return View(CL);
        }

        [HttpPost]
        public ActionResult ConfirmRemove(String action, RemoveList cl)
        {
            if (action.Equals("home"))
            {
                return RedirectToAction("HomeLec", "HomeLec");
            }

            else if (action.Equals("back"))
            {
                return RedirectToAction("ManagePuzzleLec");
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
                    sqlCommand = new System.Data.SqlClient.SqlCommand("UPDATE [SEF_AssignmentEntities].[dbo].[Puzzle] Set Lec_ID ='DEL000' WHERE \"Puzzle_ID\" = '" + removelistid[i] + "'");
                    sqlCommand.Connection = sqlConnection;
                    tempcounter = sqlCommand.ExecuteNonQuery();
                }

                TempData["AlertMessage"] = "Removal successful";

                return RedirectToAction("HomeLec", "HomeLec");
            }

            return View();

        }
        [HttpGet]
        public ActionResult CreatePuz()
        {
            Session["LecID"] = Session["LecID"];

            var model = new CreatePuz
            {
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult CreatePuz(string action, CreatePuz cl)
        {
            Session["LecID"] = Session["LecID"];
            String LecID = Session["LecID"].ToString();
            LecID = LecID.ToUpper();
            if (action.Equals("home"))
            {
                return RedirectToAction("Index");
            }
            
            else if (action.Equals("back"))
            {
                return RedirectToAction("ManagePuzzleLec");
            }


            else if (action.Equals("create"))
            {
                if (ModelState.IsValid)
                {
                    string[] Answer = cl.Answer.Split('\n');
                    string[] Question = cl.Question.Split('\n');
                    string PuzzleName = cl.PuzzleName;
                    int PuzzleScore = cl.PuzzleScore;
                    int PuzzleHint = cl.PuzzleHint;

                    int counter;
                    int tempcounter;
                    String AnswerID;
                    String QuestionID;


                    string connectionString =@"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                    System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);

                    sqlConnection.Open();

                    System.Data.SqlClient.SqlCommand sqlCommand =new System.Data.SqlClient.SqlCommand("SELECT COUNT(*) FROM [SEF_AssignmentEntities].[dbo].[Puzzle]");
                    sqlCommand.Connection = sqlConnection;
                    int count = Convert.ToInt32(sqlCommand.ExecuteScalar()) + 1;
                    String PuzzleID = "PZ" + count.ToString("000");

                    sqlCommand = new System.Data.SqlClient.SqlCommand("INSERT INTO [SEF_AssignmentEntities].[dbo].[Puzzle] (\"Puzzle_ID\", \"Puzzle_Name\", Puzzle_Score, Hint_Score,\"LEC_ID\") VALUES('" +PuzzleID + "','" + PuzzleName + "'," + PuzzleScore + "," + PuzzleHint + ",'" + LecID +"')");
                    //return Content("INSERT INTO [SEF_AssignmentEntities].[dbo].[Puzzle] (\"Puzzle_ID\", \"Puzzle_Name\", Puzzle_Score, Hint_Score,\"LEC_ID\") VALUES('" + PuzzleID + "','" + PuzzleName + "'," + PuzzleScore + "," + PuzzleHint + ",'" + LecID + "')");
                    sqlCommand.Connection = sqlConnection;
                    count = Convert.ToInt32(sqlCommand.ExecuteScalar());
                  

                    for (int i = 0; i < Answer.Length; i++)
                    {
                        sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT COUNT(*) FROM [SEF_AssignmentEntities].[dbo].[Answer]");
                        sqlCommand.Connection = sqlConnection;
                        counter = Convert.ToInt32(sqlCommand.ExecuteScalar()) + 1;
                        AnswerID = "ANS" + counter.ToString("000");

                        sqlCommand = new System.Data.SqlClient.SqlCommand("INSERT INTO [SEF_AssignmentEntities].[dbo].[Answer] (\"Answer_ID\", \"Answer_Content\", \"Puzzle_ID\") VALUES('" + AnswerID + "','" + Answer[i] + "','"  + PuzzleID + "')");
                        sqlCommand.Connection = sqlConnection;
                        tempcounter = Convert.ToInt32(sqlCommand.ExecuteScalar());

                    }

                    for (int i = 0; i < Question.Length; i++)
                    {
                        sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT COUNT(*) FROM [SEF_AssignmentEntities].[dbo].[Question]");
                        sqlCommand.Connection = sqlConnection;
                        counter = Convert.ToInt16(sqlCommand.ExecuteScalar()) + 1;
                        QuestionID = "QS" + counter.ToString("000");

                        sqlCommand = new System.Data.SqlClient.SqlCommand("INSERT INTO [SEF_AssignmentEntities].[dbo].[Question] (\"Question_ID\", \"Question_Content\", \"Puzzle_ID\") VALUES('" + QuestionID + "','" + Question[i] + "','" + PuzzleID + "')");
                        sqlCommand.Connection = sqlConnection;
                        tempcounter = Convert.ToInt32(sqlCommand.ExecuteScalar());
                    }
                    TempData["AlertMessage"] = "Successfully created puzzle! Returning to main menu.";
                    return RedirectToAction("HomeLec");
                }

                else
                {
                    return View("CreatePuz", cl);
                }
            }


            return View("CreatePuz",cl);
        }

        public ActionResult RankingBoardLec()
        {
            Session["LecID"] = Session["LecID"];
            TempData["userID"] = TempData["userID"];
            return RedirectToAction("RankingBoard", "ManageClass");
        }

        [HttpGet]
        public ActionResult DiscussionBoardLec()
        { 

            return RedirectToAction("IndexLec", "DiscussionBoardLec");
        }

        public ActionResult ManageClassLec()
        {

            return RedirectToAction("ManageClass","ManageClass");
        }
        
    }
}