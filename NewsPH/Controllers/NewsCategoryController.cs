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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsCategory model)
        {
            if (ModelState.IsValid)
            {
                _db.NewsCategories.Add(model);
                _db.SaveChanges();
            }

            return View(model);
        }
    }
}
