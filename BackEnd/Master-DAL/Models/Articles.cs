using Master_DAL.Premetives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Models
{
    public sealed class Articles 
    {

        public Guid ArticlesId { get; set; }
        public string ArticlesTitle { get; set; }
        public string ArticlesContent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
        public ICollection<Comments> Comments { get; set; }
        public ICollection<ArticlesImage> ArticlesImages { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        #region DomainDriven
        //public Articles(Guid id, string articleTitle, string articleContent, DateTime createdAt) : base(id)
        //{
        //    ArticleTitle = articleTitle;
        //    ArticleContent = articleContent;
        //    CreatedAt = createdAt;
        //    Comments = new List<Comments>();

        //}
        //public string ArticleTitle { get; set; }
        //public string ArticleContent { get; set; }
        //public DateTime CreatedAt { get; set; }

        //public ICollection<Comments> Comments { get; set; }
        #endregion

    }
}
