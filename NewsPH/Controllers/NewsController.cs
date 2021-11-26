using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

        public IActionResult Index()
        {
            IEnumerable<News> news = _db.News;
            return View(news);
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
                //News news = new News()
                //{
                //    Title = model.Title,
                //    Content = model.Content,
                //    Date = model.Date,
                //    Image = UploadFile(model),
                //    UserId = userId
                //};
                model.News.Image = UploadFile(model.ImageFile);
                model.News.UserId = userId;

                _db.News.Add(model.News);
                _db.SaveChanges();
            }

            return View(model);
        }

        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            NewsViewModel newsViewModel = new NewsViewModel()
            {
                News = new News()
            };

            newsViewModel.News = _db.News.Find(id);

            if (newsViewModel.News == null)
            {
                return NotFound();
            }

            return View(newsViewModel);
        }

        private string UploadFile(IFormFile file)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "news-image");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
