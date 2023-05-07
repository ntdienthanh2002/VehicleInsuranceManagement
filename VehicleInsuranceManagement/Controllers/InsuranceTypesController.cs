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
    public class InsuranceTypesController : Controller
    {
        private VehicleInsuranceEntities db = new VehicleInsuranceEntities();

        // GET: InsuranceTypes
        public ActionResult Index()
        {
            return View(db.InsuranceTypes.ToList());
        }

        public ActionResult _PartialInsurance(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InsuranceType insuranceType = db.InsuranceTypes.Find(id);
            if (insuranceType == null)
            {
                return HttpNotFound();
            }
            return PartialView("_PartialInsurance", insuranceType);
        }

        // GET: InsuranceTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InsuranceType insuranceType = db.InsuranceTypes.Find(id);
            if (insuranceType == null)
            {
                return HttpNotFound();
            }
            return View(insuranceType);
        }

        // GET: InsuranceTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InsuranceTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "InsuranceTypeID,InsuranceTypeName,Price,Description")] InsuranceType insuranceType)
        {
            if (ModelState.IsValid)
            {
                db.InsuranceTypes.Add(insuranceType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insuranceType);
        }

        // GET: InsuranceTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InsuranceType insuranceType = db.InsuranceTypes.Find(id);
            if (insuranceType == null)
            {
                return HttpNotFound();
            }
            return View(insuranceType);
        }

        // POST: InsuranceTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InsuranceTypeID,InsuranceTypeName,Price,Description")] InsuranceType insuranceType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insuranceType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuranceType);
        }

        // GET: InsuranceTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InsuranceType insuranceType = db.InsuranceTypes.Find(id);
            if (insuranceType == null)
            {
                return HttpNotFound();
            }
            return View(insuranceType);
        }

        // POST: InsuranceTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InsuranceType insuranceType = db.InsuranceTypes.Find(id);
            db.InsuranceTypes.Remove(insuranceType);
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
