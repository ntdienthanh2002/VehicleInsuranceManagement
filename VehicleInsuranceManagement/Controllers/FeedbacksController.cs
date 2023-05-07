using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VehicleInsuranceManagement.Models;

namespace VehicleInsuranceManagement.Controllers
{
    public class FeedbacksController : Controller
    {
        private VehicleInsuranceEntities db = new VehicleInsuranceEntities();

        public ActionResult Index()
        {
            var feedbacks = db.Feedbacks.Where(f => f.Type.Equals("Feedback") && f.Status.Equals("Enable"));
            return View(feedbacks.ToList());
        }

        // GET: Feedbacks/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username");
            return View();
        }

        // POST: Feedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FeedbackID,CustomerID,Description,Datetime,Type,Status")] Feedback feedback)
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (ModelState.IsValid)
            {
                feedback.CustomerID = (int)Session["CusID"];
                feedback.Datetime = DateTime.Now;
                feedback.Type = "Feedback";
                feedback.Status = "Disable";
                db.Feedbacks.Add(feedback);
                db.SaveChanges();
                TempData["AlertMessage"] = "Feedback sent successfully!";
                return RedirectToAction("Create", "Feedbacks");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", feedback.CustomerID);
            return View(feedback);
        }

        // GET: Feedbacks/Contact
        public ActionResult Contact()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username");
            return View();
        }

        // POST: Feedbacks/Contact
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact([Bind(Include = "FeedbackID,CustomerID,Description,Datetime,Type,Status")] Feedback feedback)
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (ModelState.IsValid)
            {
                feedback.CustomerID = (int)Session["CusID"];
                feedback.Datetime = DateTime.Now;
                feedback.Type = "Contact";
                feedback.Status = "Disable";
                db.Feedbacks.Add(feedback);
                db.SaveChanges();
                TempData["AlertMessage"] = "Contact sent successfully!";
                return RedirectToAction("Contact", "Feedbacks");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", feedback.CustomerID);
            return View(feedback);
        }

        // POST: Feedbacks/CreateFeedback
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFeedback([Bind(Include = "FeedbackID,CustomerID,Description,Datetime,Type,Status")] Feedback feedback)
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (ModelState.IsValid)
            {
                feedback.CustomerID = (int)Session["CusID"];
                feedback.Datetime = DateTime.Now;
                feedback.Type = "Feedback";
                feedback.Status = "Disable";
                db.Feedbacks.Add(feedback);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", feedback.CustomerID);
            return View(feedback);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
