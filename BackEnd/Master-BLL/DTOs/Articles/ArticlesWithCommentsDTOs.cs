
using Master_BLL.DTOs.Comment;
using Master_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Articles
{
    public record ArticlesWithCommentsDTOs(
        string Id,
        string Title,
        string Content,
        DateTime PublishedDate,
        bool IsActive,
        List<CommentsGetDTOs> Comments
        );
}
