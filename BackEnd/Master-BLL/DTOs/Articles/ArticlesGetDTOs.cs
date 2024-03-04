using Master_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Articles
{
    public class ArticlesGetDTOs
    {
        public Guid ArticlesId { get; set; }
        public string ArticlesTitle { get; set; }
        public string ArticlesContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public string ApplicationUserId { get; set; }


    }
}
