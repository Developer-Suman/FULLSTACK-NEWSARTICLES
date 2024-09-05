using AutoMapper;
using Master_BLL.DTOs.Articles;
using Master_BLL.DTOs.Comment;
using Master_BLL.DTOs.Likes;
using Master_BLL.Services.Interface;
using Master_DAL.Exceptions;
using Master_DAL.Models;
using MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Configs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;

namespace MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Controllers
{
    [Authorize]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]"), EnableCors("AllowAllOrigins")]
    [ApiController]
    public class ArticlesController : MasterProjectControllerBase
    {
        private readonly IArticlesRepository _articlesRepository;
        private readonly ILogger<IArticlesRepository> _logger;


        public ArticlesController(IArticlesRepository articlesRepository, ILogger<IArticlesRepository> logger, IMemoryCacheRepository memoryCacheRepository, UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager) : base(mapper,userManager, roleManager)
        {
            _articlesRepository = articlesRepository;
            _logger = logger;
        }
        [Authorize(Policy = "CanCreateArticles")]
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromForm] ArticlesCreateDTOs articlesCreateDTOs, List<IFormFile> imagefiles)
        {
            await GetCurrentUserFromDB();
            var saveArticlesResult = await _articlesRepository.Save(articlesCreateDTOs, imagefiles, _currentUser!.Id);
            #region switch
            return saveArticlesResult switch
            {
                { IsSuccess: true, Data: not null } => CreatedAtAction(nameof(Save), saveArticlesResult.Data),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(saveArticlesResult.Errors),
                _ => BadRequest("Invalid Some Fields")
            };
            #endregion
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete([FromRoute] string Id, CancellationToken cancellationToken)
        {
            var deleteArticlesResult = await _articlesRepository.Delete(Id);

            #region switch
            return deleteArticlesResult switch
            {
                { IsSuccess: true, Data: not null } => NoContent(),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(deleteArticlesResult.Errors),
                _ => BadRequest("Invalid some Fields")
            };
            #endregion
        }

        [HttpPatch("{Id}")]
        public async Task<IActionResult> Update([FromRoute] string Id, [FromBody] ArticlesUpdateDTOs articlesUpdateDTOs, List<IFormFile> imagefiles)
        {
            var updateArticlesResult = await _articlesRepository.Update(Id, articlesUpdateDTOs, imagefiles);

            #region switch
            return updateArticlesResult switch
            {
                { IsSuccess: true, Data: not null } => new JsonResult(updateArticlesResult.Data, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                }),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(updateArticlesResult.Errors),
                _ => BadRequest("Invalid Id")
            };
            #endregion
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex, [FromQuery] int PageSize, CancellationToken cancellationToken)
        {
            var getAllArticlesData = await _articlesRepository.GetAll(pageIndex, PageSize, cancellationToken);


            #region switch
            return getAllArticlesData switch
            {
                { IsSuccess: true, Data: not null } => new JsonResult(getAllArticlesData.Data, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                }),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(getAllArticlesData.Errors),
                _ => BadRequest("Invalid Data")
            };
            #endregion

        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] string Id, CancellationToken cancellationToken)
        {
            var getByIdResultData = await _articlesRepository.GetById(Id, cancellationToken);

            #region switch
            return getByIdResultData switch
            {
                { IsSuccess: true, Data: not null } => new JsonResult(getByIdResultData.Data, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                }),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(getByIdResultData.Errors),
                _ => BadRequest("Invalid Data")
            };
            #endregion
        }

        [HttpGet("{id}/details-with-comments")]
        public async Task<IActionResult> GetArticlesWithComments([FromQuery] int pageIndex, [FromQuery] int PageSize, CancellationToken cancellationToken)
        {
            var getArticlesWithCommentstData = _articlesRepository.GetArticlesWithComments(pageIndex, PageSize, cancellationToken);

            #region switch
            return getArticlesWithCommentstData switch
            {
                { IsSuccess: true, Data: not null } => new JsonResult(getArticlesWithCommentstData.Data, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                }),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(getArticlesWithCommentstData.Errors),
                _ => BadRequest("Invalid Data")
            };
            #endregion
        }


        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetArticlesDetails([FromRoute] string Id, CancellationToken cancellationToken)
        {
            var getArticlesWithCommentstData = await _articlesRepository.GetArticlesDetails(Id, cancellationToken);

            #region switch
            return getArticlesWithCommentstData switch
            {
                { IsSuccess: true, Data: not null } => new JsonResult(getArticlesWithCommentstData.Data, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                }),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(getArticlesWithCommentstData.Errors),
                _ => BadRequest("Invalid Data")
            };
            #endregion
        }


        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetMoreComments([FromRoute] string Id, int page, int pageSize, CancellationToken cancellationToken)
        {
            var getmoreCommentstData = await _articlesRepository.GetMoreComments(Id, page, pageSize,cancellationToken);

            #region switch
            return getmoreCommentstData switch
            {
                { IsSuccess: true, Data: not null } => new JsonResult(getmoreCommentstData.Data, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                }),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(getmoreCommentstData.Errors),
                _ => BadRequest("Invalid Data")
            };
            #endregion
        }


        [HttpPost("like")]
        public async Task<IActionResult> LikeArticles([FromBody] LikesArticlesCreateDTOs likesArticlesCreateDTOs, CancellationToken cancellationToken)
        {
            var getLikeArticles = await _articlesRepository.LikeArticles(likesArticlesCreateDTOs);

            #region switch
            return getLikeArticles switch
            {
                { IsSuccess: true, Data: not null } => new JsonResult(getLikeArticles.Data, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                }),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(getLikeArticles.Errors),
                _ => BadRequest("Invalid Data")
            };
            #endregion
        }




    }
}
