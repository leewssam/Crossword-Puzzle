using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SEF_Assignment.Models;

namespace SEF_Assignment.Controllers
{
    public class DiscussionBoardLecController : Controller
    {
        SEFASSIGNMENT db = new SEFASSIGNMENT();

        public ActionResult IndexLec()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateNewTopicLec()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateNewTopicLec(Topic t)
        { 
            string str = string.Empty;
            str = Convert.ToString(Session["LecID"]).ToUpper();

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
                newTopic.Lec_ID = str;
                db.Topics.Add(newTopic);
                db.SaveChanges();

                return RedirectToAction("IndexLec");

            }
        }

        [HttpGet]
        public ActionResult ViewAllTopicsLec()
        {
            var allTopic = db.Topics;
            return View(allTopic.ToList());
        }

        [HttpPost]
        public ActionResult ViewAllTopicsLec(string search)
        {
            var topics = from t in db.Topics select t;
            if(!string.IsNullOrWhiteSpace(search))
            {
                topics = topics.Where(t => t.Topic_Title.Contains(search));
            }

            return View(topics);
        }

        [HttpGet]
        public ActionResult ViewThisTopicLec(string t)
        {
            Topic thisTopic = db.Topics.Find(t);
            Session["ThisTopicID"] = t;

            string str = string.Empty;
            str = Convert.ToString(Session["LecID"]).ToUpper();
            ViewBag.UserID = str;

            return View(thisTopic);
        }

        
        [HttpPost] 
        public ActionResult ViewThisTopicLec(string t, string comment)
        {
            string userID = string.Empty;
            userID = Convert.ToString(Session["LecID"]).ToUpper();
            
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
            newCom.Lec_ID = userID;
            db.Comments.Add(newCom);
            db.SaveChanges();

            return RedirectToAction("ViewThisTopicLec", "DiscussionBoardLec", new { t = topicID });

        }

        public ActionResult ConfirmDeleteTopicLec()
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

            return RedirectToAction("ViewAllTopicsLec");
           
        }
       
        public ActionResult ConfirmDeleteCommentLec(string id)
        {
            string topicID = Convert.ToString(Session["ThisTopicID"]);
            Comment deleteComment = db.Comments.Find(id);
            db.Comments.Remove(deleteComment);
            db.SaveChanges();
            return RedirectToAction("ViewThisTopicLec", new { t = topicID });
        }
    }
}