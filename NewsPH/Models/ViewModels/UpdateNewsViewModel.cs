using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewsPH.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPH.Models.ViewModels
{
    public class UpdateNewsViewModel
    {
        public News News { get; set; }
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]
        public IFormFile ImageFile { get; set; }
        public IEnumerable<SelectListItem> NewsCategories { get; set; }
    }
}
