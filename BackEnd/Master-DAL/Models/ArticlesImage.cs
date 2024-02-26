using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Models
{
    public sealed class ArticlesImage
    {
        
        public Guid ArticlesImageId { get; set; }

        public string? ArticlesImagesUrl { get; set; }

        public Guid ArticlesId { get; set; }
        public Articles? Articles { get; set; }
        
    }
}
