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
    public class CustomersController : Controller
    {
        private VehicleInsuranceEntities db = new VehicleInsuranceEntities();

        // GET: Customers
        public ActionResult Index()
        {
            return View(db.Customers.ToList());
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,Username,Password,CustomerName,CustomerADD,CustomerPhoneNumber,Email,CitizenIdentityCard,Status")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,Username,Password,CustomerName,CustomerADD,CustomerPhoneNumber,Email,CitizenIdentityCard,Status")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        public ActionResult _PartialUpdatePersonal()
        {
            if (Session["CusID"] != null)
            {
                Customer customer = db.Customers.Find((int)Session["CusID"]);
                if (customer == null)
                {
                    return HttpNotFound();
                }
                return PartialView("_PartialUpdatePersonal", customer);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PartialUpdatePersonal(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_PartialUpdatePersonal", customer);
            }

            db.Entry(customer).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Success = true });
        }

        public ActionResult _PartialChangePassword()
        {
            if (Session["CusID"] != null)
            {
                return PartialView("_PartialChangePassword");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PartialChangePassword(ChangePassword changePassword)
        {
            Customer customer = db.Customers.Find((int)Session["CusID"]);

            if (!ModelState.IsValid)
            {
                return PartialView("_PartialChangePassword", changePassword);
            }

            if (changePassword.OldPassword != customer.Password)
            {
                ModelState.AddModelError("OldPassword", "Old Password is incorrect");
                return PartialView("_PartialChangePassword", changePassword);
            }

            if (changePassword.Password == customer.Password)
            {
                ModelState.AddModelError("Password", "New Password and Old Password are the same");
                return PartialView("_PartialChangePassword", changePassword);
            }

            customer.Password = changePassword.Password;
            customer.ConfirmPassword = changePassword.Password;
            db.Entry(customer).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { Success = true });
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
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
