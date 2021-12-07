using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPH.Models.ViewModels
{
    public class NewsCommentViewModel
    {
        public int NewsId { get; set; }
        public Comment Comment { get; set; }
    }
}
