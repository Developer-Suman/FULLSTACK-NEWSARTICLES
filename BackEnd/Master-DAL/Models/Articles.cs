using Master_DAL.Premetives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Models
{
    //[DebuggerDisplay("ArticlesId = {ArticlesId}, ArticlesTitle = {ArticlesTitle,nq}")]
    public sealed class Articles 
    {

        public Guid ArticlesId { get; set; }
        public string ArticlesTitle { get; set; }
        public string ArticlesContent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
        public ICollection<Comments> Comments { get; set; }
        public ICollection<ArticlesImage> ArticlesImages { get; set; }

      

        public string? ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }



    }
}
