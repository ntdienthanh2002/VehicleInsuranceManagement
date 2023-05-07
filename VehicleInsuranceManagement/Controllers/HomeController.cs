using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VehicleInsuranceManagement.Models;

namespace VehicleInsuranceManagement.Controllers
{
    public class HomeController : Controller
    {
        private VehicleInsuranceEntities db = new VehicleInsuranceEntities();

        //GET: Home
        public ActionResult Index()
        {
            var feedbacks = db.Feedbacks.Where(f => f.Type.Equals("Feedback") && f.Status.Equals("Enable"));
            return View(feedbacks.ToList());
        }

        //GET: Register
        public ActionResult Register()
        {
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var check = db.Customers.FirstOrDefault(a => a.Username == customer.Username);
                if (check == null)
                {
                    customer.Status = "Enable";
                    db.Customers.Add(customer);
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Registered successfully";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["AlertMessage"] = "Username already exists";
                    return View();
                }
            }
            return View();
        }

        //GET: Login
        public ActionResult Login()
        {
            return View();
        }

        //POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                var data = db.Customers.Where(a => a.Username.Equals(username) && a.Password.Equals(password)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    Session["CusID"] = data.FirstOrDefault().CustomerID;
                    Session["Username"] = data.FirstOrDefault().Username;
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["AlertMessage"] = "Username or password incorrect";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }

        //Logout
        public ActionResult Logout()
        {
            //remove session
            Session.Clear();
            return RedirectToAction("Login");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult MyAccount()
        {
            if (Session["CusID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}