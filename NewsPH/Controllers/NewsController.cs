using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsPH.Data;
using NewsPH.Models;
using NewsPH.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NewsPH.Controllers
{
    public class NewsController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ApplicationDbContext _db;
        UserManager<ApplicationUser> _userManager;

        public NewsController(ApplicationDbContext db, UserManager<ApplicationUser> usermanager, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _userManager = usermanager;
            webHostEnvironment = hostEnvironment;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsViewModel model)
        {
            if (model.ImageFile == null)
            {
                return View();
            }

            if (ModelState.IsValid)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                News news = new News()
                {
                    Title = model.Title,
                    Content = model.Content,
                    Date = model.Date,
                    Image = UploadFile(model),
                    UserId = userId
                };

                _db.News.Add(news);
                _db.SaveChanges();
            }

            return View(model);
        }

        private string UploadFile(NewsViewModel model)
        {
            string uniqueFileName = null;

            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "news-image");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
