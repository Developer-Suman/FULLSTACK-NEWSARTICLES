using AutoMapper;
using Master_BLL.DTOs.Articles;
using Master_BLL.Services.Interface;
using Master_DAL.Models;
using MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;

namespace MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Controllers
{
    [Authorize(Roles = "admin")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]"), EnableCors("AllowAllOrigins")]
    [ApiController]
    public class ArticlesController : MasterProjectControllerBase
    {
        private readonly IArticlesRepository _articlesRepository;


        public ArticlesController(IArticlesRepository articlesRepository, IMemoryCacheRepository memoryCacheRepository, UserManager<ApplicationUser> userManager, IMapper mapper) : base(userManager, mapper)
        {
            _articlesRepository = articlesRepository;
        }

        [HttpGet("{ArticlesId}")]
        public async Task<IActionResult> GetArticlesById([FromRoute] Guid ArticlesId, CancellationToken cancellationToken)
        {
            try
            {
                var articles = await _articlesRepository.GetArticlesById(ArticlesId, cancellationToken);

                if (!articles.IsSuccess)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { articles.Errors });
                }

                return Ok(articles.Data);
         

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }

        [HttpGet]
        [Route("all-articles")]
        public async Task<IActionResult> GetAllArticles([FromQuery] int page, int pageSize, CancellationToken cancellationToken)
        {
            try
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Operation Cancelled");
                }
                
                var articles = await _articlesRepository.GetAllArticles(page, pageSize, cancellationToken);
                if (articles.Data is null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { articles.Errors });
                }
                return Ok(articles.Data);

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);  

            }

           
        }

        [HttpGet("articles-with-comments")]
        public IActionResult GetArticlesWithComments([FromBody] int page, int pageSize, CancellationToken cancellationToken)
        {
            try
            {
                var articlesWithComments = _articlesRepository.GetArticlesWithComments(page, pageSize, cancellationToken);
                return Ok(articlesWithComments.Data);

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpGet("all-comments-from-articles")]
        public async Task<IActionResult> GetAllCommentsFromArticles(int page, int pageSize, CancellationToken cancellationToken)
        {
            try
            {
                var commentsfromarticles = await _articlesRepository.GetCommentsWithArticlesName(page, pageSize, cancellationToken);
                return Ok(commentsfromarticles.Data);

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveArticles([FromForm] ArticlesCreateDTOs articlesCreateDTOs)
        {
            try
            {
                if (articlesCreateDTOs.filesList.Count() < 0 && articlesCreateDTOs.filesList is null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "No Files are Added");

                }
                GetCurrentUserFromDB();
                var userDetails = _currentUser;
                var articles = await _articlesRepository.SaveArticles(articlesCreateDTOs, _currentUser!.Id);
                if(articles.IsSuccess)
                {
                    return Ok(articles.Data);
                }
                else
                {
                    return BadRequest(articles.Errors);
                }



               
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPatch]
        public async Task<IActionResult> UpdateArticles([FromForm] ArticlesUpdateDTOs articlesUpdateDTOs)
        {
            try
            {
                var articles = await _articlesRepository.UpdateArticles(articlesUpdateDTOs);

                if (!articles.IsSuccess)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { articles.Errors });
                }

                return Ok(articles.Data);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticles(Guid id)
        {
            try
            {
                var articles = await _articlesRepository.DeleteArticles(id);

                if(!articles.IsSuccess)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new {articles.Errors});
                }
              

                return Ok(articles.Data);
            }catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

    }
}
