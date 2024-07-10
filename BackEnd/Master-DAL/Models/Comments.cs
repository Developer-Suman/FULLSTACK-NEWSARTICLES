using Master_DAL.Premetives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Models
{
    public sealed class Comments : Entity
    {

        public Comments() : base(null) { }

        public Comments(
            string id,
            string content,
            string articlesId
            ): base(id)
        {
            Content = content;
            ArticlesId = articlesId;
        }
        public string Content { get; set; }
        public string ArticlesId { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? UserId { get; set; }


        //Navigation Property
        public Articles Articles { get; set; }
        public ApplicationUser ApplicationUsers { get; set; }
        //This is due to the Polymorphic Association(ie without Pk in Like table it take relationship with Comments)
        public ICollection<Likes> Likes { get; set; }


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
