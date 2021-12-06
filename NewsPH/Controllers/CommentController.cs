﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CommentController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NewsDetailViewModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            Comment comment = new Comment();
            comment = model.Comment;

            comment.UserId = userId;
            comment.Date = DateTime.Now;

            //if (ModelState.IsValid)
            //{
                _db.Comments.Add(comment);
                _db.SaveChanges();

                int commentId = comment.Id;

                NewsComment newsComment = new NewsComment()
                {
                    CommentId = commentId,
                    NewsId = model.News.Id
                };

                _db.NewsComments.Add(newsComment);
                _db.SaveChanges();
            //}

            return RedirectToAction("Details", "News", new { id = model.News.Id });
        }
    }
}
