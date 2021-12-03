using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsPH.Data;
using NewsPH.Models;
using NewsPH.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NewsPH.Controllers
{
    public class LikeController : Controller
    {
        private readonly ApplicationDbContext _db;
        UserManager<ApplicationUser> _userManager;

        public LikeController(ApplicationDbContext db, UserManager<ApplicationUser> usermanager)
        {
            _db = db;
            _userManager = usermanager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Like(NewsDetailViewModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var isLiked = (from l in _db.Likes
                           where l.NewsId == model.News.Id && l.UserId == userId
                           select l).Count();

            if (isLiked == 1)
            {
                // unlike
                var LikedId = (from l in _db.Likes
                               where l.NewsId == model.News.Id && l.UserId == userId
                               select l.Id).First();

                Likes like = _db.Likes.Find(LikedId);
                _db.Likes.Remove(like);
                _db.SaveChanges();
                return RedirectToAction("Details", "News", new { id = model.News.Id });
            }
            else
            {
                // like
                _db.Likes.Add(new Likes() { NewsId = model.News.Id, UserId = userId });
                _db.SaveChanges();
                return RedirectToAction("Details", "News", new { id = model.News.Id });
            }

            return View();
        }
    }
}
