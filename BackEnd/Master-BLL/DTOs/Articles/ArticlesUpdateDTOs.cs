using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Articles
{
    public class ArticlesUpdateDTOs
    {
        public Guid ArticlesId { get; set; }
        public string? ArticlesTitle { get; set; }
        public string? ArticlesContent { get; set; }

        public List<IFormFile>? filesList { get; set; }

        public bool IsActive { get; set; }
    }
}
