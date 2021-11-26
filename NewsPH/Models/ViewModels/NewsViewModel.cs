using Microsoft.AspNetCore.Http;
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
        public IFormFile ImageFile { get; set; }
        [Required]
        [FileExtensions(Extensions = "jpg,jpeg,png")]
        public string ImageFileName 
        { 
            // Avoid null reference exception
            get 
            {
                if (ImageFile == null)
                {
                    return "";
                }
                return ImageFile.FileName;
            } 
        }
    }
}
