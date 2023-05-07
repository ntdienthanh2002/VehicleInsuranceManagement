using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VehicleInsuranceManagement.Models;

namespace VehicleInsuranceManagement.Controllers
{
    public class VehiclesController : Controller
    {
        private VehicleInsuranceEntities db = new VehicleInsuranceEntities();

        // GET: Vehicles/Create
        public ActionResult Create()
        {

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username");
            ViewBag.VehicleTypeID = new SelectList(db.VehicleTypes, "VehicleTypeID", "VehicleTypeName");
            ViewBag.VehicleBrandID = new SelectList(db.VehicleBrands, "VehicleBrandID", "VehicleBrandName");
            ViewBag.VehicleModelID = new SelectList(db.VehicleModels, "VehicleModelID", "VehicleModelName");
            return View();
        }
        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VehicleID,VehicleTypeID,VehicleModelID,CustomerID,VehicleOwnerName,VehicleBodyNumber,VehicleEngineNumber,VehicleNumber,Image,SeatNumber,Status")] Vehicle vehicle, HttpPostedFileBase uploadFile)
        {
            if (ModelState.IsValid)
            {
                vehicle.Status = "Disable";
                //upload file
                if (uploadFile != null)
                {
                    string filename = "vehicle" + vehicle.VehicleTypeID + "" + vehicle.VehicleModelID + "" + vehicle.CustomerID + "" + vehicle.VehicleNumber + ".jpg";
                    string path = Path.Combine(Server.MapPath("~/img"), filename);
                    uploadFile.SaveAs(path);
                    //vehicle.Image = filename;
                }
                // save Vehicle
                db.Vehicles.Add(vehicle);
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", vehicle.CustomerID);
            ViewBag.VehicleTypeID = new SelectList(db.VehicleTypes, "VehicleTypeID", "VehicleTypeName", vehicle.VehicleTypeID);
            ViewBag.VehicleModelID = new SelectList(db.VehicleModels, "VehicleModelID", "VehicleModelName", vehicle.VehicleModelID);
            return View(vehicle);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public List<Vehicle> GetVehiclesByCustomerID(int customerID)
        {
            List<Vehicle> vehicles = (from v in db.Vehicles
                                      where v.CustomerID == customerID
                                      select v).ToList();
            return vehicles;
        }

        public ActionResult _PartialVehicleList()
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
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

                return PartialView("_PartialVehicleList", vehicleRecord);
            }
        }

        public ActionResult _PartialAddVehicle(int? brandID)
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username");
            ViewBag.VehicleTypeID = new SelectList(db.VehicleTypes, "VehicleTypeID", "VehicleTypeName");
            ViewBag.VehicleBrandID = new SelectList(db.VehicleBrands, "VehicleBrandID", "VehicleBrandName");
            ViewBag.SeatNumber = new SelectList(db.Coefficients, "SeatNumber", "SeatNumber");

            if (brandID != null)
            {
                List<VehicleModel> vehicleModels = (from v in db.VehicleModels
                                                    where v.VehicleBrandID == brandID
                                                    select v).ToList();
                ViewBag.VehicleModelID = new SelectList(vehicleModels, "VehicleModelID", "VehicleModelName");
            }
            else
            {
                List<VehicleModel> vehicleModels = (from v in db.VehicleModels
                                                    where v.VehicleBrandID == 0
                                                    select v).ToList(); ;
                ViewBag.VehicleModelID = new SelectList(vehicleModels);
            }

            return PartialView("_PartialAddVehicle");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PartialAddVehicle([Bind(Include = "VehicleID,VehicleTypeID,VehicleModelID,CustomerID,VehicleOwnerName,VehicleBodyNumber,VehicleEngineNumber,VehicleNumber,Image,SeatNumber,Status")] Vehicle vehicle, HttpPostedFileBase uploadFile, int? brandID)
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            Byte[] bytes = null;

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", vehicle.CustomerID);
            ViewBag.VehicleTypeID = new SelectList(db.VehicleTypes, "VehicleTypeID", "VehicleTypeName", vehicle.VehicleTypeID);
            ViewBag.VehicleBrandID = new SelectList(db.VehicleBrands, "VehicleBrandID", "VehicleBrandName", brandID);
            ViewBag.SeatNumber = new SelectList(db.Coefficients, "SeatNumber", "SeatNumber", vehicle.SeatNumber);

            if (brandID != null)
            {
                List<VehicleModel> vehicleModels = (from v in db.VehicleModels
                                                    where v.VehicleBrandID == brandID
                                                    select v).ToList();
                ViewBag.VehicleModelID = new SelectList(vehicleModels, "VehicleModelID", "VehicleModelName");
            }
            else
            {
                List<VehicleModel> vehicleModels = (from v in db.VehicleModels
                                                    where v.VehicleBrandID == 0
                                                    select v).ToList(); ;
                ViewBag.VehicleModelID = new SelectList(vehicleModels);
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_PartialAddVehicle", vehicle);
            }

            if (uploadFile.FileName != null)
            {
                Stream fs = uploadFile.InputStream;
                BinaryReader br = new BinaryReader(fs);
                bytes = br.ReadBytes((Int32)fs.Length);
                vehicle.Image = bytes;
            }
            vehicle.CustomerID = (int)Session["CusID"];
            vehicle.Status = "Disable";
            db.Vehicles.Add(vehicle);
            db.SaveChanges();
            return Json(new { Success = true });
        }

        public ActionResult _PartialEditVehicle(int? id, int? brandID)
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle vehicle = db.Vehicles.Find(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", vehicle.CustomerID);
            ViewBag.VehicleTypeID = new SelectList(db.VehicleTypes, "VehicleTypeID", "VehicleTypeName", vehicle.VehicleTypeID);

            List<VehicleModel> vehicleModels;
            if (brandID != null)
            {
                vehicleModels = (from v in db.VehicleModels
                                 where v.VehicleBrandID == brandID
                                 select v).ToList();
                ViewBag.VehicleModelID = new SelectList(vehicleModels, "VehicleModelID", "VehicleModelName");
            }
            else
            {
                vehicleModels = (from v in db.VehicleModels
                                 where v.VehicleBrandID == vehicle.VehicleModel.VehicleBrandID
                                 select v).ToList();
            }
            ViewBag.VehicleModelID = new SelectList(vehicleModels, "VehicleModelID", "VehicleModelName", vehicle.VehicleModelID);
            ViewBag.VehicleBrandID = new SelectList(db.VehicleBrands, "VehicleBrandID", "VehicleBrandName", vehicle.VehicleModel.VehicleBrandID);
            ViewBag.SeatNumber = new SelectList(db.Coefficients, "SeatNumber", "SeatNumber", vehicle.SeatNumber);

            return PartialView("_PartialEditVehicle", vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PartialEditVehicle([Bind(Include = "VehicleID,VehicleTypeID,VehicleModelID,CustomerID,VehicleOwnerName,VehicleBodyNumber,VehicleEngineNumber,VehicleNumber,Image,SeatNumber,Status")] Vehicle vehicle, HttpPostedFileBase uploadFile, int? brandID)
        {
            if (Session["CusID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            Byte[] bytes = null;

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", vehicle.CustomerID);
            ViewBag.VehicleTypeID = new SelectList(db.VehicleTypes, "VehicleTypeID", "VehicleTypeName", vehicle.VehicleTypeID);
            ViewBag.VehicleBrandID = new SelectList(db.VehicleBrands, "VehicleBrandID", "VehicleBrandName", brandID);
            ViewBag.SeatNumber = new SelectList(db.Coefficients, "SeatNumber", "SeatNumber", vehicle.SeatNumber);
            List<VehicleModel> vehicleModels;
            if (brandID != null)
            {
                vehicleModels = (from v in db.VehicleModels
                                 where v.VehicleBrandID == brandID
                                 select v).ToList();
                ViewBag.VehicleModelID = new SelectList(vehicleModels, "VehicleModelID", "VehicleModelName");
            }
            else
            {
                vehicleModels = (from v in db.VehicleModels
                                 where v.VehicleBrandID == vehicle.VehicleModel.VehicleBrandID
                                 select v).ToList();
            }
            ViewBag.VehicleModelID = new SelectList(vehicleModels, "VehicleModelID", "VehicleModelName", vehicle.VehicleModelID);

            if (!ModelState.IsValid)
            {
                return PartialView("_PartialEditVehicle", vehicle);
            }

            var oldVehicle = db.Vehicles.AsNoTracking().Where(v => v.VehicleID == vehicle.VehicleID).FirstOrDefault();

            if (uploadFile != null)
            {
                Stream fs = uploadFile.InputStream;
                BinaryReader br = new BinaryReader(fs);
                bytes = br.ReadBytes((Int32)fs.Length);
                vehicle.Image = bytes;
            }
            else
            {
                vehicle.Image = oldVehicle.Image;
            }

            vehicle.CustomerID = (int)Session["CusID"];
            vehicle.Status = "Disable";
            db.Entry(vehicle).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Success = true });
        }
    }
}
