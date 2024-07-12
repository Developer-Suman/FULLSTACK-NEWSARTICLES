using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Comment
{
    public record CommentsWithArticlesDTOs(
         string ArticlesName,
        string ArticlesId,
        string Comments
        );
}
