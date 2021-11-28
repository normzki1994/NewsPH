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
    public class NewsViewModel
    {
        public News News { get; set; }
        [Required]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]
        public IFormFile ImageFile { get; set; }
        //[Required]
        //[FileExtensions(Extensions = "jpg,jpeg,png")]
        //public string ImageFileName 
        //{ 
        //    // Avoid null reference exception
        //    get 
        //    {
        //        if (ImageFile == null)
        //        {
        //            return "";
        //        }
        //        return ImageFile.FileName;
        //    } 
        //}
        public IEnumerable<SelectListItem> NewsCategories { get; set; }
    }
}
