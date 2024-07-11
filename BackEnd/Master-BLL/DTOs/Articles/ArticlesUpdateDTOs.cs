using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Articles
{
    public record ArticlesUpdateDTOs(
        string Id,
        string Title,
        string Content,
        bool IsActive,
        List<IFormFile>? filesList
        );
}
