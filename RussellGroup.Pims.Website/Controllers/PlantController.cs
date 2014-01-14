﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RussellGroup.Pims.DataAccess.Models;

namespace RussellGroup.Pims.Website.Controllers
{
    public class PlantController : Controller
    {
        private PimsContext db = new PimsContext();

        // GET: /Plant/
        public ActionResult Index()
        {
            return View();
        }

        // this method has been adapted from the code described here:
        // http://www.codeproject.com/KB/aspnet/JQuery-DataTables-MVC.aspx
        public JsonResult GetDataTableResult(JqueryDataTableParameterModel model)
        {
            IEnumerable<Plant> entries = db.Plants;
            var sortColumnIndex = int.Parse(Request["iSortCol_0"]);

            // ordering
            Func<Plant, string> ordering = (c =>
                sortColumnIndex == 1 ? c.XPlantId :
                    sortColumnIndex == 2 ? c.XPlantNewId :
                        sortColumnIndex == 3 ? c.Description :
                            sortColumnIndex == 4 ? c.Category.Name : c.IsDisused.ToString());

            // sorting
            IEnumerable<Plant> ordered = Request["sSortDir_0"] == "asc" ?
                entries.OrderBy(ordering) :
                entries.OrderByDescending(ordering);

            // filter for sSearch
            string hint = Request["sSearch"].ToUpperInvariant();
            IEnumerable<Plant> searched;

            if (string.IsNullOrEmpty(hint))
            {
                searched = ordered;
            }
            else
            {
                // don't include in the search the id as it is hidden from the display
                searched = ordered.Where(f =>
                    (f.XPlantId != null && f.XPlantId.ToUpperInvariant().Contains(hint)) ||
                    (f.XPlantNewId != null && f.XPlantNewId.ToUpperInvariant().Contains(hint)) ||
                    (f.Description != null && f.Description.ToUpperInvariant().Contains(hint)) ||
                    (f.Category != null && f.Category.Name.ToUpperInvariant().Contains(hint))
                );
            }

            // filter for the display
            var filtered = searched
                .Skip(searched.Count() > model.iDisplayLength ? model.iDisplayStart : 0)
                .Take(searched.Count() > model.iDisplayLength ? model.iDisplayLength : searched.Count());

            // get the display values
            var displayData = filtered
                .Select(c => new string[]
                {
                    c.PlantId.ToString(),
                    c.XPlantId,
                    c.XPlantNewId,
                    c.Description,
                    c.Category != null ? c.Category.Name : string.Empty,
                    c.IsDisused.ToYesNo(),
                    c.IsHired.ToYesNo(),
                    this.CrudLinks(new { id = c.PlantId })
                });

            var result = new
            {
                sEcho = model.sEcho,
                iTotalRecords = db.Plants.Count(),
                iTotalDisplayRecords = searched.Count(),
                aaData = displayData
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: /Plant/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Plant plant = await db.Plants.FindAsync(id);
            if (plant == null)
            {
                return HttpNotFound();
            }
            return View(plant);
        }

        // GET: /Plant/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Plant/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PlantId,CategoryId,XPlantId,XPlantNewId,Description,WhenPurchased,WhenDisused,Rate,Cost,Serial,FixedAssetCode,IsElectrical,IsTool")] Plant plant)
        {
            if (ModelState.IsValid)
            {
                db.Plants.Add(plant);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(plant);
        }

        // GET: /Plant/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Plant plant = await db.Plants.FindAsync(id);
            if (plant == null)
            {
                return HttpNotFound();
            }
            return View(plant);
        }

        // POST: /Plant/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PlantId,CategoryId,XPlantId,XPlantNewId,Description,WhenPurchased,WhenDisused,Rate,Cost,Serial,FixedAssetCode,IsElectrical,IsTool")] Plant plant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(plant).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(plant);
        }

        // GET: /Plant/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Plant plant = await db.Plants.FindAsync(id);
            if (plant == null)
            {
                return HttpNotFound();
            }
            return View(plant);
        }

        // POST: /Plant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Plant plant = await db.Plants.FindAsync(id);
            db.Plants.Remove(plant);
            await db.SaveChangesAsync();
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

        private new ActionResult View()
        {
            return View(null);
        }

        private ActionResult View(Plant plant)
        {
            var categories = db.Categories.OrderBy(f => f.Name);
            var category = plant != null ? plant.CategoryId : 0;

            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name", category);

            return base.View(plant);
        }
    }
}
