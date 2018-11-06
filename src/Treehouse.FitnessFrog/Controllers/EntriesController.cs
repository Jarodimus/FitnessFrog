using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            var entry = new Entry()
            {
                Date = DateTime.Today
            };
            SetupActivitiesSelectListItems();
            return View(entry);
        }

        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            //Global error message denoted by blank string
            //ModelState.AddModelError("", "This is a global message.");

            //Below is not needed because Html helper methods were used in the Add Razor page
            //ViewBag.date = ModelState["Date"].Value.AttemptedValue;
            //ViewBag.activityID = ModelState["ActivityId"].Value.AttemptedValue;
            //ViewBag.duration = ModelState["Duration"].Value.AttemptedValue;
            //ViewBag.intensity = ModelState["Intensity"].Value.AttemptedValue;
            //ViewBag.exclude = ModelState["Exclude"].Value.AttemptedValue;
            //ViewBag.notes= ModelState["Notes"].Value.AttemptedValue;

            //If there aren't any "Duration" field validation errors
            //then make sure that the duration is greater than 0
            ValidateEntry(entry);

            //Check if model is valid (no errors)
            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);

                return RedirectToAction("Index");
            }
            SetupActivitiesSelectListItems();
            return View(entry);
        }



        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //TODO Get requested entry from repository
            Entry entry = _entriesRepository.GetEntry((int) id);

            //TODO return status not found if entry wasn't found
            if(entry == null)
            {
                return HttpNotFound();
            }
            //TODO pass entry to view

            SetupActivitiesSelectListItems();
            return View(entry);
        }

        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            ValidateEntry(entry);

            //TODO Validate the entry
            if (ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);
                return RedirectToAction("Index");
            }

            //TODO Populate the activities select list items ViewBag property
            SetupActivitiesSelectListItems();
            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Retrieve the entry by the id
            Entry entry = _entriesRepository.GetEntry((int) id);

            if (entry == null)
            {
                return HttpNotFound();
            }

            SetupActivitiesSelectListItems();
            return View(entry);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _entriesRepository.DeleteEntry(id);
            return RedirectToAction("Index");
        }

        private void ValidateEntry(Entry entry)
        {
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "The Duration field value must be greater than '0'.");
            }
        }

        private void SetupActivitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(Data.Data.Activities, "Id", "Name");
        }
    }
}