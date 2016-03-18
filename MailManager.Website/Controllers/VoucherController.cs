using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MailManager.Models;
using AutoMapper;
using System.Data.Entity.Validation;
using MailManager.DAL.MailContext;
using MailManager.Core;

namespace MailManager.Controllers
{
    [CustomAuthorize]
    public class VoucherController : Controller
    {
        private MailManagerDb db = new MailManagerDb();

        // GET: /Voucher/
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.VoucheringStlye = db.Company_Info.Select(c => c.VoucherIncrementType).FirstOrDefault();
            ViewBag.VoucherConfigurationId = db.VoucherConfigurations.Select(v => v.Id).FirstOrDefault();

            var voucherNumberingList = db.VoucherNumberings.Include(v => v.Category).OrderByDescending(v => v.ID).ToList();
            var voucher = voucherNumberingList.Count();
            var category = db.Categories.Count();
            if (voucher == category)
            {
                ViewBag.showCreate = "false";
            }

            List<VoucherNumberingModel> voucherNumberingModelList = new List<VoucherNumberingModel>();
            Mapper.CreateMap<VoucherNumbering, VoucherNumberingModel>();

            if (voucherNumberingList.Any())
            {
                foreach (var v in voucherNumberingList)
                {
                    VoucherNumberingModel voucherNumberingModel = Mapper.Map<VoucherNumbering, VoucherNumberingModel>(v);
                    voucherNumberingModelList.Add(voucherNumberingModel);
                }
            }
            return View(voucherNumberingModelList);
        }


        // GET: /Voucher/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VoucherNumbering vouchernumbering = db.VoucherNumberings.Find(id);
            if (vouchernumbering == null)
            {
                return HttpNotFound();
            }
            return View(vouchernumbering);
        }
         [CustomAuthorize(Roles = "Admin,User")]
        
        // GET: /Voucher/Create
        public ActionResult Create(long? id)
        {
            var model = new VoucherNumberingModel();

            TempData.Remove("message");

            List<Category> categoryList = new List<Category>();
            var voucherCategoryId = db.VoucherNumberings.Select(v => v.CategoryId).ToList();
            var categoryId = db.Categories.Select(ca => ca.CategoryID).ToList();

            foreach(var categ in categoryId)
            {
                int count = 0;
                foreach (var voucher in voucherCategoryId)
                {
                    if(categ == voucher)
                    {
                        count++;
                    }
                  
                }
                if (count == 0)
                {
                    var category = db.Categories.Where(c => c.CategoryID == categ).FirstOrDefault();
                    categoryList.Add(category);
                }
                }
        
           //voucher config
           int? voucherType = db.Company_Info.ToList().FirstOrDefault().VoucherIncrementType;
           ViewBag.VoucherType = voucherType;
           if (voucherType == 1)
           {
               var voucherConfig = db.VoucherConfigurations.ToList().FirstOrDefault();
               ViewBag.VoucherType = voucherType;
               ViewBag.StartNo = voucherConfig.StartNo;
               ViewBag.EndNo = voucherConfig.EndNo;
               ViewBag.BodyLength = voucherConfig.BodyLength;
           }
           ViewBag.CategoryList = new SelectList(categoryList, "CategoryID", "Name");

           if (id != null)
           {
               ViewBag.CategoryId = id;
           }
       return View();
      
     }

        //for angualrjs category list
        [HttpGet]
        public JsonResult getCategory()
        {
            var resultData = db.Categories.ToList();

            return Json(new { json = resultData }, JsonRequestBehavior.AllowGet);
        }

        // POST: /Voucher/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(VoucherNumberingModel voucherModel)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<VoucherNumberingModel, VoucherNumbering>();
                var voucher = Mapper.Map<VoucherNumberingModel, VoucherNumbering>(voucherModel);
                voucher.CreatedBy = User.Identity.Name;
                voucher.CreateDate = DateTime.Now;


                if(voucher.Prefix == null && voucher.Suffix == null)
                {
                    voucher.TotalLength = voucher.BodyLength;
                }
                else if (voucher.Prefix == null)
                {
                    voucher.TotalLength = (voucher.Suffix+"-").Count()+ (voucher.BodyLength);
                }
                else if(voucher.Suffix == null)
                {
                    voucher.TotalLength = (voucher.Prefix + "-").Count() + (voucher.BodyLength);
                }
                else
                {
                    voucher.TotalLength = (voucher.Prefix + "-").Count() + (voucher.Suffix + "-").Count()  +(voucher.BodyLength);
                }

               
                voucher.CurrentNo = 0;
                try
                {
                    db.VoucherNumberings.Add(voucher);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    throw ex;
                }
                TempData["message"] = "Document design added successfully";
               
                return RedirectToAction("Index");
            }

            ViewBag.CategoryList = new SelectList(db.Categories, "CategoryID", "Name");
            return View(voucherModel);
        }
        [CustomAuthorize(Roles = "Admin,User")]
        // GET: /Voucher/Edit/5
        public ActionResult Edit(long? id)
        {
            TempData.Remove("message");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VoucherNumbering vouchernumbering = db.VoucherNumberings.Find(id);
            if (vouchernumbering == null)
            {
                return HttpNotFound();
            }
            Mapper.CreateMap<VoucherNumbering, VoucherNumberingModel>();
            var voucherModel = Mapper.Map<VoucherNumbering, VoucherNumberingModel>(vouchernumbering);

            //voucher config
            int? voucherType = db.Company_Info.ToList().FirstOrDefault().VoucherIncrementType;
            ViewBag.VoucherType = voucherType;
            if (voucherType == 1)
            {
                var voucherConfig = db.VoucherConfigurations.ToList().FirstOrDefault();
                ViewBag.VoucherType = voucherType;
                ViewBag.StartNo = voucherConfig.StartNo;
                ViewBag.EndNo = voucherConfig.EndNo;
                ViewBag.BodyLength = voucherConfig.BodyLength;
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryID", "Name", voucherModel.CategoryId);

            return View(voucherModel);
        }

        // POST: /Voucher/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VoucherNumberingModel voucherModel)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<VoucherNumberingModel, VoucherNumbering>();
                var voucher = Mapper.Map<VoucherNumberingModel, VoucherNumbering>(voucherModel);
                voucher.LastUpdateDate = DateTime.Now;
                voucher.LastUpdatedBy = User.Identity.Name;
                db.Entry(voucher).State = EntityState.Modified;


                db.SaveChanges();
                TempData["message"] = "Document design updated successfully";
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryID", "Name", voucherModel.CategoryId);
            return View(voucherModel);
        }
         [CustomAuthorize(Roles = "Admin")]
        // GET: /Voucher/Delete/5
        public ActionResult Delete(long? id)
        {
            TempData.Remove("message");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VoucherNumbering vouchernumbering = db.VoucherNumberings.Find(id);
            if (vouchernumbering == null)
            {
                return HttpNotFound();
            }
            return View(vouchernumbering);
        }

        // POST: /Voucher/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            VoucherNumbering vouchernumbering = db.VoucherNumberings.Find(id);
            db.VoucherNumberings.Remove(vouchernumbering);
            db.SaveChanges();

            var status = "Document design deleted successfully";
            return Json(status);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Get:Voucher/VoucherConfiguration
        public ActionResult VoucherConfiguration(long? id)
        {
            if(id != null)
            {
                var voucherConfig = db.VoucherConfigurations.Find(id);
                Mapper.CreateMap<VoucherConfiguration,VoucherConfigurationModel >();
                var model = Mapper.Map<VoucherConfiguration, VoucherConfigurationModel>(voucherConfig);
                return View(model);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VoucherConfiguration(long? id,VoucherConfigurationModel model)
        {
            if (ModelState.IsValid)
            {

            if(id == null)
            {

                Mapper.CreateMap<VoucherConfigurationModel, VoucherConfiguration>();
                var voucherConfig = Mapper.Map<VoucherConfigurationModel, VoucherConfiguration>(model);
                voucherConfig.CurrentNo = 0;
                db.VoucherConfigurations.Add(voucherConfig);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
                else
            {
                Mapper.CreateMap<VoucherConfigurationModel, VoucherConfiguration>();
                var voucherConfig = Mapper.Map<VoucherConfigurationModel, VoucherConfiguration>(model);
                db.Entry(voucherConfig).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
           
          
            }
            return View(model);

           
        }

        public JsonResult GetVoucherType() 
        {
            var resultData = db.Company_Info.ToList().FirstOrDefault();
            return Json(new { result = resultData.VoucherIncrementType }, JsonRequestBehavior.AllowGet);
        }
    }
}
