using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VehicleInsuranceManagement.Models;

namespace VehicleInsuranceManagement.Controllers
{
    public class OrderPoliciesController : Controller
    {
        private VehicleInsuranceEntities db = new VehicleInsuranceEntities();

        // GET: OrderPolicies
        public ActionResult Index()
        {
            var orderPolicies = db.OrderPolicies.Include(o => o.InsuranceType).Include(o => o.Vehicle);
            return View(orderPolicies.ToList());
        }

        // GET: OrderPolicies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderPolicy orderPolicy = db.OrderPolicies.Find(id);
            if (orderPolicy == null)
            {
                return HttpNotFound();
            }
            return View(orderPolicy);
        }

        // GET: OrderPolicies/Create
        public ActionResult Create()
        {
            ViewBag.InsuranceTypeID = new SelectList(db.InsuranceTypes, "InsuranceTypeID", "InsuranceTypeName");
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "VehicleOwnerName");
            return View();
        }

        // POST: OrderPolicies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PolicyNumber,VehicleID,InsuranceTypeID,OrderDate,PolicyDate,PolicyDuration,PaymentType,Status")] OrderPolicy orderPolicy)
        {
            if (ModelState.IsValid)
            {
                db.OrderPolicies.Add(orderPolicy);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.InsuranceTypeID = new SelectList(db.InsuranceTypes, "InsuranceTypeID", "InsuranceTypeName", orderPolicy.InsuranceTypeID);
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "VehicleOwnerName", orderPolicy.VehicleID);
            return View(orderPolicy);
        }

        // GET: OrderPolicies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderPolicy orderPolicy = db.OrderPolicies.Find(id);
            if (orderPolicy == null)
            {
                return HttpNotFound();
            }
            ViewBag.InsuranceTypeID = new SelectList(db.InsuranceTypes, "InsuranceTypeID", "InsuranceTypeName", orderPolicy.InsuranceTypeID);
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "VehicleOwnerName", orderPolicy.VehicleID);
            return View(orderPolicy);
        }

        // POST: OrderPolicies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PolicyNumber,VehicleID,InsuranceTypeID,OrderDate,PolicyDate,PolicyDuration,PaymentType,Status")] OrderPolicy orderPolicy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderPolicy).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.InsuranceTypeID = new SelectList(db.InsuranceTypes, "InsuranceTypeID", "InsuranceTypeName", orderPolicy.InsuranceTypeID);
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "VehicleOwnerName", orderPolicy.VehicleID);
            return View(orderPolicy);
        }

        // GET: OrderPolicies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderPolicy orderPolicy = db.OrderPolicies.Find(id);
            if (orderPolicy == null)
            {
                return HttpNotFound();
            }
            return View(orderPolicy);
        }

        // POST: OrderPolicies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderPolicy orderPolicy = db.OrderPolicies.Find(id);
            db.OrderPolicies.Remove(orderPolicy);
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

        public List<OrderPolicy> GetOrdersByCustomerID(int customerID)
        {
            List<OrderPolicy> orders = (from o in db.OrderPolicies
                                        where o.Vehicle.Customer.CustomerID == customerID
                                        select o).ToList();
            return orders;
        }

        public ActionResult _PartialOrderList()
        {
            if (Session["CusID"] == null)
            {
               return RedirectToAction("Login", "Home");
            }
            else
            {
                List<OrderPolicy> orders = GetOrdersByCustomerID((int)Session["CusID"]);

                return PartialView("_PartialOrderList", orders);
            }
        }

        public ActionResult _PartialOrderDetails(int? orderID)
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (orderID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderPolicy order = db.OrderPolicies.Find(orderID);
            if (order == null)
            {
                return HttpNotFound();
            }

            dynamic mymodel = new ExpandoObject();

            List<Bill> orderBills = (from b in db.Bills
                                     where b.OrderPolicy.PolicyNumber == order.PolicyNumber
                                     select b).ToList();

            Coefficient c = db.Coefficients.FirstOrDefault(v => v.SeatNumber == order.Vehicle.SeatNumber);

            mymodel.Bills = orderBills;
            mymodel.OrderPolicy = order;
            mymodel.Coefficient = c;

            return PartialView("_PartialOrderDetails", mymodel);
        }

        public ActionResult _PartialInvoice(int? orderID)
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            dynamic mymodel = new ExpandoObject();
            OrderPolicy order = db.OrderPolicies.Find(orderID);
            List<Bill> orderBills = (from b in db.Bills
                                     where b.OrderPolicy.PolicyNumber == order.PolicyNumber
                                     select b).ToList();

            Coefficient c = db.Coefficients.FirstOrDefault(v => v.SeatNumber == order.Vehicle.SeatNumber);

            mymodel.Bills = orderBills;
            mymodel.OrderPolicy = order;
            mymodel.Coefficient = c;

            return PartialView("_PartialInvoice", mymodel);
        }

        public ActionResult _PartialInvoicePdf(int? orderID)
        {
            dynamic mymodel = new ExpandoObject();

            OrderPolicy order = db.OrderPolicies.Find(orderID);

            List<Bill> orderBills = (from b in db.Bills
                                     where b.OrderPolicy.PolicyNumber == order.PolicyNumber
                                     select b).ToList();

            Coefficient c = db.Coefficients.FirstOrDefault(v => v.SeatNumber == order.Vehicle.SeatNumber);

            mymodel.Bills = orderBills;
            mymodel.OrderPolicy = order;
            mymodel.Coefficient = c;
            return PartialView("_PartialInvoicePdf", mymodel);
        }

        public ActionResult PrintInvoice(int? orderID)
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var report = new Rotativa.ActionAsPdf("_PartialInvoicePdf", new { orderID = orderID })
            {
                FileName = "Invoice.pdf"
            };
            return report;
        }
    }
}
