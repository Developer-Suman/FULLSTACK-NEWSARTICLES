using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Comment
{
    public class CommentsCreateDTOs
    {
        public string CommentDescription { get; set; }
      
        public Guid ArticlesId { get; set; }
    }
}
