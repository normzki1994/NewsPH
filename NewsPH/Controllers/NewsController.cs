using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsPH.Data;
using NewsPH.Models;
using NewsPH.Models.ViewModels;
using NewsPH.Utility;
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
            List<NewsViewModel> newsViewmodel = new List<NewsViewModel>();
            IEnumerable<News> news = _db.News.OrderByDescending(n => n.Date);
            foreach (var obj in news)
            {
                string connectionString = "Server=.;Database=NewsPH;Trusted_Connection=True;MultipleActiveResultSets=True";

                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                using (ApplicationDbContext dbContext = new ApplicationDbContext(optionsBuilder.Options))
                {
                    obj.NewsCategory = dbContext.NewsCategories.FirstOrDefault(u => u.Id == obj.NewsCategoryId);
                    newsViewmodel.Add(new NewsViewModel()
                    {
                        News = obj,
                        Likes = (from l in dbContext.Likes
                                 where l.NewsId == obj.Id
                                 select l).Count(),
                        Comments = (from c in dbContext.NewsComments
                                    where c.NewsId == obj.Id
                                    select c).Count()
                    });
                }
            }

            return View(newsViewmodel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            NewsViewModel newsViewModel = new NewsViewModel()
            {
                NewsCategories = _db.NewsCategories.Select(category => new SelectListItem
                {
                    Text = category.Name,
                    Value = category.Id.ToString()
                })
            };
            return View(newsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(NewsViewModel model)
        {
            if (model.ImageFile == null)
            {
                return View(model);
            }

            string[] validContentType = { "image/png", "image/jpg", "image/jpeg" };

            if(validContentType.Contains(model.ImageFile.ContentType) == false)
            {
                ModelState.AddModelError(string.Empty, "Invalid file type");
                return View(model);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                model.News.Image = UploadFile(model.ImageFile);
                model.News.UserId = userId;

                _db.News.Add(model.News);
                _db.SaveChanges();
                return RedirectToAction("Index");
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

            UpdateNewsViewModel newsViewModel = new UpdateNewsViewModel()
            {
                NewsCategories = _db.NewsCategories.Select(category => new SelectListItem
                {
                    Text = category.Name,
                    Value = category.Id.ToString()
                })
            };

            newsViewModel.News = _db.News.Find(id);

            if (newsViewModel.News == null)
            {
                return NotFound();
            }

            return View(newsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(UpdateNewsViewModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            News newsindb = new News();
            string connectionString = "Server=.;Database=NewsPH;Trusted_Connection=True;MultipleActiveResultSets=True";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using (ApplicationDbContext dbContext = new ApplicationDbContext(optionsBuilder.Options))
            {
                newsindb = dbContext.News.Find(model.News.Id);
                if (newsindb == null)
                {
                    return NotFound();
                }
            }

            if (ModelState.IsValid)
            {
                if (model.ImageFile == null)
                {
                    model.News.Image = newsindb.Image;
                }
                else
                {
                    model.News.Image = UploadFile(model.ImageFile);
                }
                model.News.UserId = userId;
                _db.News.Update(model.News);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var news = _db.News.Find(id);

            if (news == null)
            {
                return NotFound();
            }

            _db.News.Remove(news);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            News news = _db.News.Find(id);

            if (news == null)
            {
                return NotFound();
            }

            news.NewsCategory = _db.NewsCategories.Find(news.NewsCategoryId);

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isLiked = (from l in _db.Likes
                          where l.NewsId == news.Id && l.UserId == userId
                          select l).Count();

            var newscomments = (from newscomment in _db.NewsComments
                            where newscomment.NewsId == news.Id
                            select newscomment);

            List<Comment> comments = new List<Comment>();

            foreach (var newscomment in newscomments)
            {
                string connectionString = "Server=.;Database=NewsPH;Trusted_Connection=True;MultipleActiveResultSets=True";

                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                using (ApplicationDbContext dbContext = new ApplicationDbContext(optionsBuilder.Options))
                {
                    var comment = dbContext.Comments.Find(newscomment.CommentId);
                    if (comment != null)
                    {
                        var user = dbContext.Users.Find(comment.UserId);
                        comment.ApplicationUser = user;
                        comments.Add(comment);
                    }
                }
            }

            NewsDetailViewModel model = new NewsDetailViewModel()
            {
                News = news,
                IsLiked = isLiked == 1 ? true : false,
                Comments = comments.OrderByDescending(c => c.Date).ToList()
            };

            return View(model);
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
