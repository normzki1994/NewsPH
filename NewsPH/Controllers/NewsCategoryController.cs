using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsPH.Data;
using NewsPH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPH.Controllers
{
    public class NewsCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public NewsCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var newsCategories = _db.NewsCategories;
            return View(newsCategories);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(NewsCategory model)
        {
            if (ModelState.IsValid)
            {
                _db.NewsCategories.Add(model);
                _db.SaveChanges();
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = _db.NewsCategories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(NewsCategory newsCategory)
        {
            if (ModelState.IsValid)
            {
                _db.NewsCategories.Update(newsCategory);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(newsCategory);
        }
    }
}
