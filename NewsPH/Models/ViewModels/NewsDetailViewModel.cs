using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPH.Models.ViewModels
{
    public class NewsDetailViewModel
    {
        public News News { get; set; }
        public bool IsLiked { get; set; }
        public Comment Comment { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
    }
}
