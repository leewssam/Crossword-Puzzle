using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SEF_Assignment.Models;

namespace SEF_Assignment.Controllers
{
    public class DiscussionBoardStuController : Controller
    {
        SEF_AssignmentEntities db = new SEF_AssignmentEntities();

        public ActionResult IndexStu()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateNewTopicStu()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateNewTopicStu(Topic t)
        { 
            string str = string.Empty;
            str = Convert.ToString(Session["StuID"]).ToUpper();

            int count = 1;
            foreach(Topic topic in db.Topics)
            {
                count++;
            }

            string newTopicID = "TOP" + count.ToString("000");
            Topic existTopicID = db.Topics.Find(newTopicID);

            if(existTopicID != null)
            {
                while(existTopicID != null)
                {
                    count++;
                    newTopicID = "TOP" + count.ToString("000");
                    existTopicID = db.Topics.Find(newTopicID);
                }
            }

            if(!ModelState.IsValid)
            {
                return View();
            }
            else
            {
                Topic newTopic = new Topic();
                newTopic.Topic_ID = newTopicID;
                newTopic.Topic_Title = t.Topic_Title;
                newTopic.Topic_Content = t.Topic_Content;
                newTopic.Stu_ID = str;
                db.Topics.Add(newTopic);
                db.SaveChanges();

                return RedirectToAction("IndexStu");

            }
        }

        [HttpGet]
        public ActionResult ViewAllTopicsStu()
        {
            var allTopic = db.Topics;
            return View(allTopic.ToList());
        }

        [HttpPost]
        public ActionResult ViewAllTopicsStu(string search)
        {
            var topics = from t in db.Topics select t;
            if(!string.IsNullOrWhiteSpace(search))
            {
                topics = topics.Where(t => t.Topic_Title.Contains(search));
            }

            return View(topics);
        }

        [HttpGet]
        public ActionResult ViewThisTopicStu(string t)
        {
            Topic thisTopic = db.Topics.Find(t);
            Session["ThisTopicID"] = t;

            string str = string.Empty;
            str = Convert.ToString(Session["StuID"]).ToUpper();
            ViewBag.UserID = str;

            return View(thisTopic);
        }

        
        [HttpPost] 
        public ActionResult ViewThisTopicStu(string t, string comment)
        {
            string userID = string.Empty;
            userID = Convert.ToString(Session["StuID"]).ToUpper();
            
            string topicID = Convert.ToString(Session["ThisTopicID"]);

            int count = 1;
            foreach(Comment c in db.Comments)
            {
                count++;
            }

            string newCommentID = "COM" + count.ToString("000");
            Comment existCommentID = db.Comments.Find(newCommentID);

            if(existCommentID != null)
            {
                while(existCommentID != null)
                {
                    count++;
                    newCommentID = "COM" + count.ToString("000");
                    existCommentID = db.Comments.Find(newCommentID);
                }
            }
            
            Comment newCom = new Comment();
            newCom.Comment_Content = comment;
            newCom.Comment_ID = newCommentID;
            newCom.Topic_ID = topicID;
            newCom.Stu_ID = userID;
            db.Comments.Add(newCom);
            db.SaveChanges();

            return RedirectToAction("ViewThisTopicStu", "DiscussionBoardStu", new { t = topicID });

        }

        public ActionResult ConfirmDeleteTopicStu()
        {
            string topicID = Convert.ToString(Session["ThisTopicID"]);
            Topic deleteTopic = db.Topics.Find(topicID);

            foreach(Comment c in deleteTopic.Comments.ToList())
            {
                db.Comments.Remove(c);
                db.SaveChanges();
            }
   
            db.Topics.Remove(deleteTopic);
            db.SaveChanges();

            return RedirectToAction("ViewAllTopicsStu");
        }

        public ActionResult ConfirmDeleteCommentStu(string id)
        {
            string topicID = Convert.ToString(Session["ThisTopicID"]);
            Comment deleteComment = db.Comments.Find(id);
            db.Comments.Remove(deleteComment);
            db.SaveChanges();
            return RedirectToAction("ViewThisTopicStu", new { t = topicID });
        }
    }
}