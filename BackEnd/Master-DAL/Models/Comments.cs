using Master_DAL.Premetives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Models
{
    public sealed class Comments 
    {
        public Guid CommentsId { get; set; }
        public string CommentDescription { get; set; }
        public Guid ArticlesId { get; set; }
        public Articles Articles { get; set; }

        #region DomainDrivenapproach
        //public Comments(Guid id, string commentDescription,string articlesId) : base(id)
        //{
        //    CommentDesription= commentDescription;
        //    ArticlesId= articlesId;

        //}
        //public string CommentDesription { get; set; }
        //public string ArticlesId { get; set; }
        //public Articles Articles { get; set; }
        #endregion

    }
}
