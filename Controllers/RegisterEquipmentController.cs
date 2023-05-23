using SNRWMSPortal.DataAccess;
using SNRWMSPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SNRWMSPortal.Controllers
{
    public class RegisterEquipmentController : Controller
    {
        SQLQueryEquipment sqlQueryequip = new SQLQueryEquipment();
        // GET: Home
        public ActionResult Index()
        {
            ModelState.Clear();
            RegisterEquipment model = new RegisterEquipment();
            model.Equipments = sqlQueryequip.GetEquipment();

            var equip = sqlQueryequip.GetEquipmentType();
            var equiplist = new List<SelectListItem>();

            foreach (var i in equip)
            {
                equiplist.Add(new SelectListItem() { Text = i.Description, Value = i.Code.ToString() });
            }
            ViewBag.EquipmentType = equiplist;

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Available", Value = "Available" });
            items.Add(new SelectListItem { Text = "Not Available", Value = "Not Available" });
            ViewBag.StatusType = items;
            return View(model);
        }



        [HttpPost]
        public ActionResult InsertEquipment(RegisterEquipment _registerequipment)
        {
            try
            {
                int result = sqlQueryequip.InsertEquipment(_registerequipment);
                if (result == 1)
                {
                    ViewBag.Message = "Equipment Added Successfully";
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }

        public JsonResult EditEquipment(int? id)
        {

            var equipment = sqlQueryequip.GetEquipment().Find(x => x.EquipmentId.Equals(id));
            return Json(equipment, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateEquipment(RegisterEquipment objModel)
        {
            try
            {

                int result = sqlQueryequip.UpdateEquipment(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Equipment Updated Successfully";
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }



        public ActionResult Details(int? id)
        {
            var _people = sqlQueryequip.GetEquipment().Find(x => x.EquipmentId.Equals(id));
            return Json(_people, JsonRequestBehavior.AllowGet);
        }

    }
}