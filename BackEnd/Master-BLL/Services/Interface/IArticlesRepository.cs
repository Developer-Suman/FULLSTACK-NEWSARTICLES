using Master_BLL.DTOs.Articles;
using Master_BLL.DTOs.Comment;
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
     
        Task<Result<ArticlesGetDTOs>> SaveArticles(ArticlesCreateDTOs articlesCreateDTOs);
        Task<Result<ArticlesGetDTOs>> GetArticlesById(Guid id);
        Task<Result<ArticlesGetDTOs>> UpdateArticles(ArticlesUpdateDTOs articlesUpdateDTOs);
        Task<Result<ArticlesGetDTOs>> DeleteArticles(Guid ArticlesId);
        Task<Result<List<ArticlesGetDTOs>>> GetAllArticles(int page, int pageSize);
        Result<IQueryable<ArticlesWithCommentsDTOs>> GetArticlesWithComments(int page, int pageSize);
        Task<Result<List<CommentsWithArticles>>> GetCommentsWithArticlesName(int page, int pageSize);

    }
}
