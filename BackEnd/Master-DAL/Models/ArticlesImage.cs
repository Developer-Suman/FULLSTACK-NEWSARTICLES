using Master_DAL.Premetives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Models
{
    public sealed class ArticlesImage : Entity
    {
        public ArticlesImage(): base(null) { }

        public ArticlesImage(
            string id,
            string imageUrl,
            string articlesId
            ): base(id)
        {
            ImagesUrl = imageUrl;
            ArticlesId = articlesId;
            
        }

        public string? ImagesUrl { get; set; }

        public string ArticlesId { get; set; }
        public Articles? Articles { get; set; }
        
    }
}
