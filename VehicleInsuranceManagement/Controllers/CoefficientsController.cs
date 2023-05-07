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
    public class CoefficientsController : Controller
    {
        private VehicleInsuranceEntities db = new VehicleInsuranceEntities();

        // GET: Coefficients
        public ActionResult Index()
        {
            return View(db.Coefficients.ToList());
        }

        // GET: Coefficients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coefficient coefficient = db.Coefficients.Find(id);
            if (coefficient == null)
            {
                return HttpNotFound();
            }
            return View(coefficient);
        }

        // GET: Coefficients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Coefficients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CoefficientID,SeatNumber,Coefficient1")] Coefficient coefficient)
        {
            if (ModelState.IsValid)
            {
                db.Coefficients.Add(coefficient);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(coefficient);
        }

        // GET: Coefficients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coefficient coefficient = db.Coefficients.Find(id);
            if (coefficient == null)
            {
                return HttpNotFound();
            }
            return View(coefficient);
        }

        // POST: Coefficients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CoefficientID,SeatNumber,Coefficient1")] Coefficient coefficient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(coefficient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(coefficient);
        }

        // GET: Coefficients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coefficient coefficient = db.Coefficients.Find(id);
            if (coefficient == null)
            {
                return HttpNotFound();
            }
            return View(coefficient);
        }

        // POST: Coefficients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Coefficient coefficient = db.Coefficients.Find(id);
            db.Coefficients.Remove(coefficient);
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
