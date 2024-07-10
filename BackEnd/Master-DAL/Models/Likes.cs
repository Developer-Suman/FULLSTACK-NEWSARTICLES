using Master_DAL.Premetives;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Models
{
    public sealed class Likes : Entity
    {
        public Likes() : base(null) { }

        public Likes(
            string id,
            string userId,
            string likeableId,
            LikeableType likeableType

            ) : base(id)
        {
            UserId = userId;
            LikelableId = likeableId;
            LikeableType = likeableType;
        }
        public string LikelableId { get; set; }
        public  LikeableType { get; set; }
        public DateTime LikedDate{get;set;} = DateTime.Now;
        public string UserId { get; set; }


        //NavigationProperty
        public ApplicationUser User { get; set; }


    }
}
