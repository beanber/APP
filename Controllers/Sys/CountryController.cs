using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APP.Controllers.Sys
{
    public class CountryController : Controller
    {
        

        // GET: Country
        public ActionResult Index()
        {
            var context = new Models.Sys.ModelSystem();
            var list = (from c in context.sysCountries
                        where c.IsDeleted == false
                        orderby c.Order
                        select c).ToList();
            ViewData["ListCountry"] = list;
            return View(ViewData["ListCountry"]);
        }

        // Create
        public ActionResult Create(HttpPostedFileBase file, string Code, string Name, string ZipCode, string Order, string Description)
        {
            if (ModelState.IsValid) //checking model is valid or not  
            {
                if (Name != null && Name.Trim() != "")
                {
                    var context = new Models.Sys.ModelSystem();
                    var obj = new Models.Sys.sysCountry();
                    if (file != null && file.ContentLength > 0)
                    {
                        obj.Avata = Beanber.Utility.DTWebSettings.IOHelper.FileUploadAndResizeFromStream("Uploads/Avata", 300, file.InputStream);
                    }
                    else
                    {
                        obj.Avata = "Images/noimage.jpg";
                    }
                    obj.Code = Code;
                    obj.Description = Description;
                    obj.IsDeleted = false;
                    obj.Name = Name;
                    if (Order.Trim() != "")
                    {
                        obj.Order = Convert.ToInt32(Order);
                    }
                    else
                    {
                        obj.Order = 1;
                    }
                    obj.ZipCode = ZipCode;
                    context.sysCountries.Add(obj);
                    if (context.SaveChanges() > 0)
                    {
                        // Insert succsessful.
                        ViewBag.Message = "Create new successful!";
                    }
                    ModelState.Clear(); //clearing model  
                }else
                {
                    ViewBag.Message = "Name is not null. Please try again!";
                }
            }
            else
            {
                ViewBag.Message = "Create new fail!";
                ModelState.AddModelError("", "Error in saving data");
            }
            return View();
        }

        // Delete
        public ActionResult Delete(Int32 id)
        {
            if (User.Identity.Name.Trim() != "")
            {
                var context = new Models.Sys.ModelSystem();
                var obj = context.sysCountries.Find(id);
                obj.IsDeleted = true;
                if (context.SaveChanges() > 0)
                {
                    // Deleted successful
                    return RedirectToAction("", "Country");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
            
        }

        public ActionResult GetListDataResult()
        {
            var context = new Models.Sys.ModelSystem();
            var list = (from c in context.sysCountries
                        where c.IsDeleted == false
                        orderby c.Order
                        select c).ToList();
            ViewData["ListCountry"] = list;
            return View(ViewData["ListCountry"]);
        }

        public ActionResult GetDataResultJson()
        {
            if (User.Identity.Name.Trim() != "")
            {
                var context = new Models.Sys.ModelSystem();
                var list = (from c in context.sysCountries
                            where c.IsDeleted == false
                            orderby c.Order
                            select new
                            {
                                Avata = c.Avata,
                                Code = c.Code,
                                Description = c.Description,
                                ID = c.ID,
                                IsDeleted = c.IsDeleted,
                                Name = c.Name,
                                Order = c.Order,
                                ZipCode = c.ZipCode
                            }).ToList();
                return Json(new { data = list }, JsonRequestBehavior.AllowGet);
            }else
            {
                return Json(new { data = "null" }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}