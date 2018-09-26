﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using SEF_Assignment.Models;
using SEF_Assignment.ViewModel;
using trycross.Model;

namespace trycross.Views.Home
{
    public class GeneratePuzzleController : Controller
    {
        [HttpGet]
        public ActionResult Generate()
        {
            Session["StuID"] = Session["StuID"];
            Session["ClassID"] = Session["ClassID"];
            Session["LecID"] = Session["LecID"];
            Session["PuzzleID"] = Session["PuzzleID"];

            string PuzzleID = Session["PuzzleID"].ToString();
            List<String> AnswerList = new List<String>();
            List<String> QuestionList = new List<String>();


            string connectionString = @"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);
            sqlConnection.Open();

            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Answer_Content FROM [SEF_AssignmentEntities].[dbo].[Answer] WHERE Puzzle_ID='" + PuzzleID + "'");
            sqlCommand.Connection = sqlConnection;

            SqlDataReader myreader = sqlCommand.ExecuteReader();
            while (myreader.Read())
            {
                AnswerList.Add(myreader[0].ToString());
            }

            Session["TotalAns"] = AnswerList.Count;
            string words = string.Join(" ", AnswerList.ToArray());
            myreader.Close();

            sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Question_Content FROM [SEF_AssignmentEntities].[dbo].[Question] WHERE Puzzle_ID='" + PuzzleID + "'");
            sqlCommand.Connection = sqlConnection;
            SqlDataReader newreader = sqlCommand.ExecuteReader();

            while (newreader.Read())
            {
                QuestionList.Add(newreader[0].ToString().ToUpper());
            }

            string[] input = AnswerList.ToArray();

            for (ushort i = 0; i < input.Length; i++)
            {
                input[i] = Regex.Replace((i+1)+input[i], "[^\\w\\d]", "");
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

            var model = new PlayPuzzle
            {
                QuestionList = QuestionList,
                
            };
            return View(model); 
            return View("Generate");
        }

        [HttpPost]
        public JsonResult Ans(string[] a, string[] b)
        {
            Session["StuID"] = Session["StuID"];
            Session["ClassID"] = Session["ClassID"];
            Session["LecID"] = Session["LecID"];
            Session["PuzzleID"] = Session["PuzzleID"];
            return Json(RedirectToAction("Check", new { a = a, b = b }));
        }

        public ActionResult Check(string[] a, string[] b)
        {
            Session["StuID"] = Session["StuID"];
            Session["ClassID"] = Session["ClassID"];
            Session["LecID"] = Session["LecID"];
            Session["PuzzleID"] = Session["PuzzleID"];
            String StudentID = Session["StuID"].ToString();

            string PuzzleID = Session["PuzzleID"].ToString();
            string connectionString = @"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);
            sqlConnection.Open();

            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT Puzzle_Score FROM [SEF_AssignmentEntities].[dbo].[Puzzle] WHERE Puzzle_ID='" + PuzzleID + "'");
            sqlCommand.Connection = sqlConnection;

            int fullmark = Convert.ToInt32(sqlCommand.ExecuteScalar());

            sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT COUNT(*) FROM [SEF_AssignmentEntities].[dbo].[Answer] WHERE Puzzle_ID='" + PuzzleID + "'");
            sqlCommand.Connection = sqlConnection;

            int sumquestion = Convert.ToInt32(sqlCommand.ExecuteScalar());

            
            float permark = fullmark / sumquestion;

            string test = sumquestion.ToString();


            float summark = 0;

            string at = a[0].ToString().ToLower();
            at = at.Replace(",,", "\n");
            at = at.Replace(",", "");

            string[] attempt = at.Split(null);

            string an = b[0].ToString().ToLower();
            an = an.Replace(",,", "\n");

            an = an.Replace(",", "");

            string[] ans = an.Split(null);




            foreach (string s in attempt)
            {
                foreach (string n in ans)
                {
                    if (s.Equals(n))
                    {
                        summark = summark + permark;
                    }
                }
            }            /*for (int i = 0; i < attempt.Count; i++)
            {
                for (int x = 0; x < ans.Count; x++)
                {
                    if (attempt[i].Equals(ans[x]))
                    {
                        summark =summark+ permark;
                    }

                }
            }*/
          

            sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT COUNT(*) FROM [SEF_AssignmentEntities].[dbo].[Attempt] WHERE \"STU_ID\" ='" + StudentID+"' AND \"PUZZLE_ID\" = '"+PuzzleID+"'");
            sqlCommand.Connection = sqlConnection;
            Int32 existscheck = (Int32)sqlCommand.ExecuteScalar();
           // int existscheck = existscheck.ExecuteNonQuery();
            string exist = existscheck.ToString();


            if (existscheck <= 0)
            {

                DateTime myDateTime = DateTime.Now;
                string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                string datee = sqlFormattedDate;
                
                sqlCommand = new System.Data.SqlClient.SqlCommand("INSERT INTO [SEF_AssignmentEntities].[dbo].[Attempt] (\"STU_ID\",\"PUZZLE_ID\",\"PUZZLE_STATUS\",\"ATTEMPT_SCORE\",\"DATETIME_STAMP\") VALUES ('"+StudentID+"','"+PuzzleID+"',"+"1,"+ summark+ ",'"+datee+"')");
                sqlCommand.Connection = sqlConnection;
                int mark = sqlCommand.ExecuteNonQuery();

            }

            else
            {
                sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT ATTEMPT_SCORE FROM [SEF_AssignmentEntities].[dbo].[Attempt] WHERE \"STU_ID\" ='" + StudentID + "' AND \"PUZZLE_ID\" = '" + PuzzleID + "'");
                sqlCommand.Connection = sqlConnection;
                int markbefore = Convert.ToInt32(sqlCommand.ExecuteScalar());
                if (markbefore <= summark)
                {
                    DateTime myDateTime = DateTime.Now;
                    string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    string datee = sqlFormattedDate;
                    sqlCommand = new System.Data.SqlClient.SqlCommand("UPDATE [SEF_AssignmentEntities].[dbo].[Attempt] SET \"ATTEMPT_SCORE\"="+summark+" ,\"DATETIME_STAMP\"='"+datee+"' WHERE(\"STU_ID\" ='" + StudentID+"' AND \"PUZZLE_ID\"='"+PuzzleID+"')");

                    sqlCommand.Connection = sqlConnection;
                    int mark = sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    
                }
            }



            string summm = summark.ToString();

            return Content("Sum of mark = "+ summm);
        }
    }
}