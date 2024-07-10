using Master_BLL.DTOs.Articles;
using Master_BLL.DTOs.Comment;
using Master_BLL.DTOs.Likes;
using Master_DAL.Abstraction;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Interface
{
    public interface IArticlesRepository
    {
     
        Task<Result<ArticlesGetDTOs>> Save(ArticlesCreateDTOs articlesCreateDTOs, string Id);
        Task<Result<ArticlesGetDTOs>> GetById(string id, CancellationToken cancellationToken);
        Task<Result<ArticlesGetDTOs>> Update(ArticlesUpdateDTOs articlesUpdateDTOs);
        Task<Result<ArticlesGetDTOs>> Delete(string ArticlesId);
        Task<Result<List<ArticlesGetDTOs>>> GetAll(int page, int pageSize, CancellationToken cancellationToken);
        Result<IQueryable<ArticlesWithCommentsDTOs>> GetArticlesWithComments(int page, int pageSize, CancellationToken cancellationToken);
        Task<Result<List<CommentsWithArticles>>> GetCommentsWithArticlesName(int page, int pageSize, CancellationToken cancellationToken);
        Task<Result<List<CommentsWithArticles>>> GetCommentsWithArticlesId(int ArticlesId);
        Task<Result<List<CommentsWithArticles>>> GetMoreReviews(string ArticlesId, int skip, int take, CancellationToken cancellationToken);
        Task<Result<List<CommentsWithArticles>>>GetHighRatedReview(string ArticlesId);
        Task<Result<LikesArticlesGetDTOs>> LikeArticles(string ArticlesId, string userId);

    }
}
