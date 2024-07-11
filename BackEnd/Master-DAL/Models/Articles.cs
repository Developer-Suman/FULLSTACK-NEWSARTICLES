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
    public class Articles : Entity
    {

        public Articles() : base(null)
        {
            
        }

        public Articles(
            string id,
            string title,
            string content,
            bool isActive
            ): base(id)
        {
            Title = title;
            Content = content;
            IsActive = isActive;
            ArticlesImages = new List<ArticlesImage>();

        }

        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
        public string? UserId { get; set; }


        //NavigaionProperty
        public ICollection<ArticlesImage> ArticlesImages { get; set; }
        public ApplicationUser ApplicationUsers { get; set; }
        public virtual ICollection<Comments> Comments { get; set; } = new List<Comments>();
        //This is due to the Polymorphic Association(ie without Pk in Like table it take relationship with Articles)
        public ICollection<Likes> Likes { get; set; }



    }
}
