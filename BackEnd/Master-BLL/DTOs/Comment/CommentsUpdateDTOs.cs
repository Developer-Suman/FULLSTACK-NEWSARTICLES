using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Comment
{
    public class CommentsUpdateDTOs
    {
        public Guid CommentsId { get; set; }
        public string CommentDescription { get; set; }
        public Guid ArticlesId { get; set; }
    }
}
