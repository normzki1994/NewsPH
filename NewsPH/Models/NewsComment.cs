using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPH.Models
{
    public class NewsComment
    {
        [Key]
        public int Id { get; set; }
        public int NewsId { get; set; }
        [ForeignKey("NewsId")]
        public virtual News News { get; set; }
        public int CommentId { get; set; }
        [ForeignKey("CommentId")]
        public virtual Comment Comment { get; set; }
    }
}
