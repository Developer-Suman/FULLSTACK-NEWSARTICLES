using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Models
{
    public sealed class ArticlesImage
    {
        
        public string Id { get; set; }

        public string? ArticlesImagesUrl { get; set; }

        public string ArticlesId { get; set; }
        public Articles? Articles { get; set; }
        
    }
}
