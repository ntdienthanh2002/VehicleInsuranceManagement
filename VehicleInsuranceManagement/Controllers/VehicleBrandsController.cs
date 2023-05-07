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
    public class VehicleBrandsController : Controller
    {
        private VehicleInsuranceEntities db = new VehicleInsuranceEntities();

        // GET: VehicleBrands
        public ActionResult Index()
        {
            return View(db.VehicleBrands.ToList());
        }

        // GET: VehicleBrands/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VehicleBrand vehicleBrand = db.VehicleBrands.Find(id);
            if (vehicleBrand == null)
            {
                return HttpNotFound();
            }
            return View(vehicleBrand);
        }

        // GET: VehicleBrands/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VehicleBrands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VehicleBrandID,VehicleBrandName")] VehicleBrand vehicleBrand)
        {
            if (ModelState.IsValid)
            {
                db.VehicleBrands.Add(vehicleBrand);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vehicleBrand);
        }

        // GET: VehicleBrands/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VehicleBrand vehicleBrand = db.VehicleBrands.Find(id);
            if (vehicleBrand == null)
            {
                return HttpNotFound();
            }
            return View(vehicleBrand);
        }

        // POST: VehicleBrands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VehicleBrandID,VehicleBrandName")] VehicleBrand vehicleBrand)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vehicleBrand).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vehicleBrand);
        }

        // GET: VehicleBrands/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VehicleBrand vehicleBrand = db.VehicleBrands.Find(id);
            if (vehicleBrand == null)
            {
                return HttpNotFound();
            }
            return View(vehicleBrand);
        }

        // POST: VehicleBrands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VehicleBrand vehicleBrand = db.VehicleBrands.Find(id);
            db.VehicleBrands.Remove(vehicleBrand);
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
