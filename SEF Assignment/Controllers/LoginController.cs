using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SEF_Assignment.Models;
using SEF_Assignment.ViewModel;

namespace SEF_Assignment.Controllers
{
    public class LoginController : Controller
    {
        private SEF_AssignmentEntities db = new SEF_AssignmentEntities();

        // GET: choose identity
        [HttpGet]
        public ActionResult ChooseIdentity()
        {
            return View();
        }

        // POST: choose identity
        [HttpPost]
        public ActionResult ChooseIdentity(string identity)
        {
            if (identity.Equals("lecturer"))
            {
                Session["userIdentity"] = "Lecturer";
                return RedirectToAction("ChooseAction", "Login");
            }
            else if (identity.Equals("student"))
            {
                Session["userIdentity"] = "Student";
                return RedirectToAction("ChooseAction", "Login");
            }

            return View();
        }

        //GET: choose action
        [HttpGet]
        public ActionResult ChooseAction()
        {
            return View();
        }

        //POST: choose action
        [HttpPost]
        public ActionResult ChooseAction(string action)
        {

            if (action.Equals("login"))
            {
                return RedirectToAction("Login");
            }
            else if (action.Equals("register"))
            {
                if (Session["userIdentity"].Equals("Student"))
                {
                    return RedirectToAction("RegisterStu");
                }
                else if (Session["userIdentity"].Equals("Lecturer"))
                {
                    return RedirectToAction("RegisterLec");
                }

            }

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string action, Login f)
        {
            string userID = f.UserID;
            if (Session["userIdentity"].Equals("Student"))
            {
                Student stuLogin = db.Students.Find(f.UserID);

                if (stuLogin == null)
                {
                    ModelState.AddModelError("UserID", "This account does not exist.");
                }
                else
                {
                    if (stuLogin.Stu_Pass == f.UserPassword)
                    {
                        return RedirectToAction("Index", "HomeStu", new { u = userID });
                    }
                    else
                    {
                        ModelState.AddModelError("UserPassword", "The password is incorrect.");
                    }
                }
            }
            else if (Session["userIdentity"].Equals("Lecturer"))
            {
                Lecturer lecLogin = db.Lecturers.Find(f.UserID);

                if (lecLogin == null)
                {
                    ModelState.AddModelError("UserID", "This account does not exist.");
                }
                else
                {
                    if (lecLogin.Lec_Pass == f.UserPassword)
                    {
                        return RedirectToAction("Index", "HomeLec", new { u = userID });
                    }
                    else
                    {
                        ModelState.AddModelError("UserPassword", "The password is incorrect.");
                    }
                }
            }

            return View();
        }

        // GET: Register for lecturer
        [HttpGet]
        public ActionResult RegisterLec()
        {
            string connectionString = @"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            sqlConnection.Open();
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT COUNT(*) FROM Lecturer");
            sqlCommand.Connection = sqlConnection;

            int count = Convert.ToInt32(sqlCommand.ExecuteScalar()) + 1;
            string newID = "LEC" + count.ToString("000");
            Lecturer existLec = db.Lecturers.Find(newID);
            while (existLec != null)
            {
                count++;
                newID = "LEC" + count.ToString("000");
                existLec = db.Lecturers.Find(newID);

            }

            var model = new RegisterLec
            {
                Lec_ID = newID
            };

            return View(model);
        }

        // POST: Register for lecturer
        [HttpPost]
        public ActionResult RegisterLec(string action, RegisterLec f)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            else
            {
                if (f.Lec_Pass != f.Lec_Pass_Confirm)
                {
                    ModelState.AddModelError("Lec_Pass_Confirm", "The password you entered does not match the above password.");
                }
                else if (f.Lec_Pass.Length < 8)
                {
                    ModelState.AddModelError("Lec_Pass", "Your password must more than 8 characters.");
                }
                else
                {
                    Lecturer lec = new Lecturer();
                    lec.Lec_ID = f.Lec_ID;
                    lec.Lec_Name = f.Lec_Name;
                    lec.Lec_Pass = f.Lec_Pass;
                    lec.Lec_Email = f.Lec_Email;
                    lec.Lec_PhoneNo = f.Lec_PhoneNo;
                    db.Lecturers.Add(lec);
                    db.SaveChanges();

                    return RedirectToAction("Login");
                }

            }

            return View();
        }

        // GET: Register for student
        [HttpGet]
        public ActionResult RegisterStu()
        {
            string connectionString = @"Data Source=SAM-7559\SQLEXPRESS;Initial Catalog=SEF_AssignmentEntities;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            sqlConnection.Open();
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("SELECT COUNT(*) FROM Student");
            sqlCommand.Connection = sqlConnection;

            int count = Convert.ToInt32(sqlCommand.ExecuteScalar()) + 1;
            string newID = "STU" + count.ToString("000");
            Student existStu = db.Students.Find(newID);
            while (existStu != null)
            {
                count++;
                newID = "STU" + count.ToString("000");
                existStu = db.Students.Find(newID);

            }

            var model = new RegisterStu
            {
                Stu_ID = newID
            };

            return View(model);
        }

        // POST: Register for student
        [HttpPost]
        public ActionResult RegisterStu(string action, RegisterStu f)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            else
            {
                Class classid = db.Classes.Find(f.Class_ID);

                if (f.Stu_Pass != f.Stu_Pass_Confirm)
                {
                    ModelState.AddModelError("Stu_Pass_Confirm", "The password you entered does not match the above password");
                }
                else if (f.Stu_Pass.Length < 8)
                {
                    ModelState.AddModelError("Stu_Pass", "Your password must more than 8 characters.");
                }
                else if (classid == null)
                {
                    ModelState.AddModelError("Class_ID", "This class ID does not exists.");
                }
                else
                {
                    Student stu = new Student();
                    stu.Stu_ID = f.Stu_ID;
                    stu.Stu_Name = f.Stu_Name;
                    stu.Stu_Pass = f.Stu_Pass;
                    stu.Stu_Email = f.Stu_Email;
                    stu.Stu_PhoneNo = f.Stu_PhoneNo;
                    stu.Class_ID = f.Class_ID.ToUpper();

                    db.Students.Add(stu);
                    db.SaveChanges();

                    return RedirectToAction("Login");
                }

            }

            return View();
        }

    }
}
