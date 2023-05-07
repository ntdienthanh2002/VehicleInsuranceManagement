using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using VehicleInsuranceManagement.Models;

namespace VehicleInsuranceManagement.Controllers
{
    public class PurchaseInsuranceProcessesController : Controller
    {
        private VehicleInsuranceEntities db = new VehicleInsuranceEntities();

        // GET: PurchaseInsuranceProcesses
        public ActionResult Index()
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        public List<InsuranceType> GetInsurances()
        {
            List<InsuranceType> insurances = db.InsuranceTypes.ToList();
            return insurances;
        }

        public List<Coefficient> GetCoefficients()
        {
            List<Coefficient> coefficients = db.Coefficients.ToList();
            return coefficients;
        }

        public List<Vehicle> GetVehiclesByCustomerID(int customerID)
        {
            List<Vehicle> vehicles = (from v in db.Vehicles
                                      where v.CustomerID == customerID && v.Status == "Enable"
                                      select v).ToList();
            return vehicles;
        }


        public ActionResult _PartialEstimate()
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                dynamic mymodel = new ExpandoObject();
                mymodel.Vehicles = GetVehiclesByCustomerID((int)Session["CusID"]);
                mymodel.Coefficients = GetCoefficients();
                return PartialView("_PartialEstimate", mymodel);
            }
        }

        public ActionResult _PartialEstimateTable(double c)
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewData["Coefficient"] = c;
            return PartialView("_PartialEstimateTable", db.InsuranceTypes.ToList());
        }

        public ActionResult _PartialEstimateVehicleDetails(int? vehicleID)
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            List<Vehicle> vehicles = GetVehiclesByCustomerID((int)Session["CusID"]);
            List<VehicleModel> models = db.VehicleModels.ToList();
            var vehicleRecord = from v in vehicles
                                join m in models on v.VehicleModelID equals m.VehicleModelID into table1
                                from m in table1.ToList()
                                select new VehicleModelView
                                {
                                    vehicle = v,
                                    model = m
                                };
            VehicleModelView vehicle = vehicleRecord.FirstOrDefault(v => v.vehicle.VehicleID == vehicleID);
            return PartialView("_PartialEstimateVehicleDetails", vehicle);
        }

        public ActionResult _PartialInsuranceInfo(int? vehicleID)
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            dynamic mymodel = new ExpandoObject();
            mymodel.InsuranceTypes = GetInsurances();
            List<Vehicle> vehicles = GetVehiclesByCustomerID((int)Session["CusID"]);
            List<VehicleModel> models = db.VehicleModels.ToList();
            var vehicleRecord = from v in vehicles
                                join m in models on v.VehicleModelID equals m.VehicleModelID into table1
                                from m in table1.ToList()
                                select new VehicleModelView
                                {
                                    vehicle = v,
                                    model = m
                                };
            VehicleModelView vehicle = vehicleRecord.FirstOrDefault(v => v.vehicle.VehicleID == vehicleID);
            mymodel.Vehicle = vehicle;
            return PartialView("_PartialInsuranceInfo", mymodel);
        }

        public ActionResult _PartialPaymentInfo(int? vehicleID, int? insuranceID)
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            dynamic mymodel = new ExpandoObject();
            List<InsuranceType> insuranceTypes = GetInsurances();
            InsuranceType insuranceType = insuranceTypes.FirstOrDefault(i => i.InsuranceTypeID == insuranceID);
            mymodel.InsuranceType = insuranceType;
            List<Vehicle> vehicles = GetVehiclesByCustomerID((int)Session["CusID"]);
            List<VehicleModel> models = db.VehicleModels.ToList();
            var vehicleRecord = from v in vehicles
                                join m in models on v.VehicleModelID equals m.VehicleModelID into table1
                                from m in table1.ToList()
                                select new VehicleModelView
                                {
                                    vehicle = v,
                                    model = m
                                };
            VehicleModelView vehicle = vehicleRecord.FirstOrDefault(v => v.vehicle.VehicleID == vehicleID);
            mymodel.Vehicle = vehicle;
            return PartialView("_PartialPaymentInfo", mymodel);
        }

        public ActionResult _PartialOrderSummary(int? vehicleID, int? insuranceID)
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            dynamic mymodel = new ExpandoObject();
            List<InsuranceType> insuranceTypes = GetInsurances();
            InsuranceType insuranceType = insuranceTypes.FirstOrDefault(i => i.InsuranceTypeID == insuranceID);
            mymodel.InsuranceType = insuranceType;
            List<Vehicle> vehicles = GetVehiclesByCustomerID((int)Session["CusID"]);
            List<VehicleModel> models = db.VehicleModels.ToList();
            var vehicleRecord = from v in vehicles
                                join m in models on v.VehicleModelID equals m.VehicleModelID into table1
                                from m in table1.ToList()
                                select new VehicleModelView
                                {
                                    vehicle = v,
                                    model = m
                                };
            VehicleModelView vehicle = vehicleRecord.FirstOrDefault(v => v.vehicle.VehicleID == vehicleID);
            mymodel.Vehicle = vehicle;
            OrderPolicy orderPolicy = new OrderPolicy();
            mymodel.OrderPolicy = orderPolicy;
            List<Bill> bills = new List<Bill>();
            mymodel.Bills = bills;
            return PartialView("_PartialOrderSummary", mymodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PartialOrderSummary([Bind(Include = "VehicleID,InsuranceTypeID,InsurancePrice,OrderDate,PolicyDate,PolicyDuration," +
            "PaymentType,Status")] OrderPolicy orderPolicy)
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var now = DateTime.Now;
            orderPolicy.OrderDate = now;
            orderPolicy.Status = "Enable";

            db.OrderPolicies.Add(orderPolicy);
            int count = int.Parse(Request.Form["count"]);
            decimal amount = Decimal.Parse(Request.Form["amount"]);
            for (int i = 0; i < count; i++)
            {
                Bill bill = new Bill();
                bill.OrderPolicy = orderPolicy;
                string s = Request.Form["Date-" + i];
                DateTime dt = DateTime.ParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                bill.ExpectedDate = dt;
                bill.Amount = amount;
                bill.Status = "Unpaid";
                db.Bills.Add(bill);
                db.SaveChanges();
            }

            db.SaveChanges();

            return Json(new { Success = true });
        }

        public ActionResult _PartialInvoice()
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            int cusID = (int)Session["CusID"];
            dynamic mymodel = new ExpandoObject();
            OrderPolicy lastOrder = (from orders in db.OrderPolicies
                                     where orders.Vehicle.CustomerID == cusID
                                     select orders).ToList().LastOrDefault();
            List<Bill> lastOrderBills = (from b in db.Bills
                                         where b.OrderPolicy.PolicyNumber == lastOrder.PolicyNumber
                                         select b).ToList();

            Coefficient c = db.Coefficients.FirstOrDefault(v => v.SeatNumber == lastOrder.Vehicle.SeatNumber);

            mymodel.Bills = lastOrderBills;
            mymodel.OrderPolicy = lastOrder;
            mymodel.Coefficient = c;
            return PartialView("_PartialInvoice", mymodel);
        }

        public ActionResult _PartialInvoicePdf(int? id)
        {
            dynamic mymodel = new ExpandoObject();
            OrderPolicy lastOrder = (from orders in db.OrderPolicies
                                     where orders.Vehicle.CustomerID == id
                                     select orders).ToList().LastOrDefault();
            List<Bill> lastOrderBills = (from b in db.Bills
                                         where b.OrderPolicy.PolicyNumber == lastOrder.PolicyNumber
                                         select b).ToList();

            Coefficient c = db.Coefficients.FirstOrDefault(v => v.SeatNumber == lastOrder.Vehicle.SeatNumber);

            mymodel.Bills = lastOrderBills;
            mymodel.OrderPolicy = lastOrder;
            mymodel.Coefficient = c;
            return PartialView("_PartialInvoicePdf", mymodel);
        }

        public ActionResult PrintInvoice()
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var report = new Rotativa.ActionAsPdf("_PartialInvoicePdf", new { id = (int)Session["CusID"] })
            {
                FileName = "Invoice.pdf"
            };
            return report;
        }
    }
}