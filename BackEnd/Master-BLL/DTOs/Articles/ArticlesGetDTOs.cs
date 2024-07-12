using Master_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Articles
{
    public record ArticlesGetDTOs(
        string Id,
        string Title,
        string Content,
        DateTime PublishedDate,
        bool IsActive,
        string userId,
        List<string> articlesImages
        );
}
