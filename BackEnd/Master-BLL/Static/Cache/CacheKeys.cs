using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Static.Cache
{
    public static class CacheKeys
    {
        public const string User = "UserCacheKey";
        public const string Articles = "ArticlesCacheKey";
        public const string ArticlesWithComment = "ArticlesWithCommentCacheKey";
        public const string Comments = "CommentsCacheKey";
    }
}
