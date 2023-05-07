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
    public class ClaimsController : Controller
    {
        private VehicleInsuranceEntities db = new VehicleInsuranceEntities();

        // GET: Claims
        public ActionResult Index()
        {
            var claims = db.Claims.Include(c => c.OrderPolicy);
            return View(claims.ToList());
        }

        public List<Vehicle> GetVehiclesByCustomerID(int customerID)
        {
            List<Vehicle> vehicles = (from v in db.Vehicles
                                      where v.CustomerID == customerID
                                      select v).ToList();
            return vehicles;
        }

        // GET: Claims/Create
        public ActionResult Create()
        {

            if (Session["CusID"] != null)
            {
                List<Vehicle> vehicles = GetVehiclesByCustomerID((int)Session["CusID"]);
                List<OrderPolicy> orderPolicies = db.OrderPolicies.ToList();
                List<InsuranceType> insuranceTypes = db.InsuranceTypes.ToList();
                ViewBag.Vehicle = vehicles;
                ViewBag.OrderPolicy = orderPolicies;
                ViewBag.InsuranceTypes = insuranceTypes;
                IList<PolicyNumberView> PolyciNumberList = new List<PolicyNumberView>();
                foreach (Vehicle v in ViewBag.Vehicle)
                {
                    foreach (OrderPolicy o in ViewBag.OrderPolicy)
                    {
                        if (v.VehicleID == o.VehicleID)
                        {
                            foreach (InsuranceType i in ViewBag.InsuranceTypes)
                            {
                                if (o.InsuranceTypeID == i.InsuranceTypeID)
                                {
                                    PolyciNumberList.Add(new PolicyNumberView(o.PolicyNumber, "Vehicle Number: " + v.VehicleNumber + " ( InsuranceType: " + i.InsuranceTypeName + " )"));
                                }
                            }
                        }
                    }
                }
                ViewBag.PolicyNumber = new SelectList(PolyciNumberList, "PolicyNumber", "Description");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // POST: Claims/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClaimNumber,PolicyNumber,PlaceOfAccident,DateOfAccident,InsuredAmount,ClaimableAmount,Status")] Claim claim)
        {
            if (ModelState.IsValid)
            {
                claim.InsuredAmount = null;
                claim.Status = "Unconfirm";
                db.Claims.Add(claim);
                db.SaveChanges();
                return RedirectToAction("Create", "Claim");
            }

            List<Vehicle> vehicles = GetVehiclesByCustomerID((int)Session["CusID"]);
            List<OrderPolicy> orderPolicies = db.OrderPolicies.ToList();
            List<InsuranceType> insuranceTypes = db.InsuranceTypes.ToList();
            ViewBag.Vehicle = vehicles;
            ViewBag.OrderPolicy = orderPolicies;
            ViewBag.InsuranceTypes = insuranceTypes;
            IList<PolicyNumberView> PolyciNumberList = new List<PolicyNumberView>();
            foreach (Vehicle v in ViewBag.Vehicle)
            {
                foreach (OrderPolicy o in ViewBag.OrderPolicy)
                {
                    if (v.VehicleID == o.VehicleID)
                    {
                        foreach (InsuranceType i in ViewBag.InsuranceTypes)
                        {
                            if (o.InsuranceTypeID == i.InsuranceTypeID)
                            {
                                PolyciNumberList.Add(new PolicyNumberView(o.PolicyNumber, "Vehicle Number: " + v.VehicleNumber + " ( InsuranceType: " + i.InsuranceTypeName + " )"));
                            }
                        }
                    }
                }
            }
            ViewBag.PolicyNumber = new SelectList(PolyciNumberList, "PolicyNumber", "Description");
            return View(claim);
        }
        public ActionResult _PartialClaimList()
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                List<Vehicle> vehicles = GetVehiclesByCustomerID((int)Session["CusID"]);
                List<OrderPolicy> orderPolicies = db.OrderPolicies.ToList();
                List<Claim> claims = db.Claims.ToList();
                IList<Claim> claimList = new List<Claim>();
                foreach (Vehicle v in vehicles)
                {
                    foreach (OrderPolicy o in orderPolicies)
                    {
                        if (v.VehicleID == o.VehicleID)
                        {
                            foreach (Claim c in claims)
                            {
                                if (o.PolicyNumber == c.PolicyNumber)
                                {
                                    claimList.Add(c);
                                }
                            }
                        }
                    }
                }
                return PartialView("_PartialClaimList", claimList);
            }

        }
        public ActionResult _PartialAddClaim()
        {

            if (Session["CusID"] != null)
            {
                List<Vehicle> vehicles = GetVehiclesByCustomerID((int)Session["CusID"]);
                List<OrderPolicy> orderPolicies = db.OrderPolicies.ToList();
                List<InsuranceType> insuranceTypes = db.InsuranceTypes.ToList();
                ViewBag.Vehicle = vehicles;
                ViewBag.OrderPolicy = orderPolicies;
                ViewBag.InsuranceTypes = insuranceTypes;
                IList<PolicyNumberView> PolyciNumberList = new List<PolicyNumberView>();
                foreach (Vehicle v in ViewBag.Vehicle)
                {
                    foreach (OrderPolicy o in ViewBag.OrderPolicy)
                    {
                        if (v.VehicleID == o.VehicleID)
                        {
                            foreach (InsuranceType i in ViewBag.InsuranceTypes)
                            {
                                if (o.InsuranceTypeID == i.InsuranceTypeID)
                                {
                                    PolyciNumberList.Add(new PolicyNumberView(o.PolicyNumber, "Vehicle Number: " + v.VehicleNumber + " ( InsuranceType: " + i.InsuranceTypeName + " )"));
                                }
                            }
                        }
                    }
                }
                ViewBag.PolicyNumber = new SelectList(PolyciNumberList, "PolicyNumber", "Description");
                return PartialView("_PartialAddClaim");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PartialAddClaim([Bind(Include = "ClaimNumber,PolicyNumber,PlaceOfAccident,DateOfAccident,InsuredAmount,ClaimableAmount,Status")] Claim claim)
        {
            if (ModelState.IsValid)
            {
                claim.InsuredAmount = null;
                claim.Status = "Unconfirm";
                db.Claims.Add(claim);
                db.SaveChanges();
                return Json(new { Success = true });
            }

            List<Vehicle> vehicles = GetVehiclesByCustomerID((int)Session["CusID"]);
            List<OrderPolicy> orderPolicies = db.OrderPolicies.ToList();
            List<InsuranceType> insuranceTypes = db.InsuranceTypes.ToList();
            ViewBag.Vehicle = vehicles;
            ViewBag.OrderPolicy = orderPolicies;
            ViewBag.InsuranceTypes = insuranceTypes;
            IList<PolicyNumberView> PolyciNumberList = new List<PolicyNumberView>();
            foreach (Vehicle v in ViewBag.Vehicle)
            {
                foreach (OrderPolicy o in ViewBag.OrderPolicy)
                {
                    if (v.VehicleID == o.VehicleID)
                    {
                        foreach (InsuranceType i in ViewBag.InsuranceTypes)
                        {
                            if (o.InsuranceTypeID == i.InsuranceTypeID)
                            {
                                PolyciNumberList.Add(new PolicyNumberView(o.PolicyNumber, "Vehicle Number: " + v.VehicleNumber + " ( InsuranceType: " + i.InsuranceTypeName + " )"));
                            }
                        }
                    }
                }
            }
            ViewBag.PolicyNumber = new SelectList(PolyciNumberList, "PolicyNumber", "Description");
            return PartialView("_PartialAddClaim", claim);
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
