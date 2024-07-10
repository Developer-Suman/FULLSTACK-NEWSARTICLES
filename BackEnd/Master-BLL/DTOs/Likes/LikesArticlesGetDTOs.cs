using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Likes
{
    public record LikesArticlesGetDTOs(
        string articlesId,
        string userId,
        bool isLike
        );
  
}
