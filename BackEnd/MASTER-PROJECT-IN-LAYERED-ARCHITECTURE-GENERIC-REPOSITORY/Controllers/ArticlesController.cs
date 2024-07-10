using AutoMapper;
using Master_BLL.DTOs.Articles;
using Master_BLL.Services.Interface;
using Master_DAL.Exceptions;
using Master_DAL.Models;
using MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Configs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Controllers
{
    //[Authorize(Roles = "admin")]
    //[Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]"), EnableCors("AllowAllOrigins")]
    [ApiController]
    public class ArticlesController : MasterProjectControllerBase
    {
        private readonly IArticlesRepository _articlesRepository;
        private readonly ILogger<IArticlesRepository> _logger;


        public ArticlesController(IArticlesRepository articlesRepository, ILogger<IArticlesRepository> logger, IMemoryCacheRepository memoryCacheRepository, UserManager<ApplicationUser> userManager, IMapper mapper) : base(userManager, mapper)
        {
            _articlesRepository = articlesRepository;
            _logger = logger;
        }

        [HttpGet("{ArticlesId}")]
        public async Task<IActionResult> GetArticlesById([FromRoute] Guid ArticlesId, CancellationToken cancellationToken)
        {
            try
            {
                var articles = await _articlesRepository.GetArticlesById(ArticlesId, cancellationToken);
                #region SwitchStatemet
                return articles switch
                {
                    { IsSuccess: true, Data: not null } => new JsonResult(articles.Data, new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull

                    }),
                    { IsSuccess: false, Errors: not null}=>NotFound(articles.Errors),
                    { Data: not null}=>BadRequest(articles.Errors),
                    _ => BadRequest("Invalid articles object")
                };
                #endregion

                #region IfStstement
                //if (!articles.IsSuccess)
                //{
                //    return StatusCode(StatusCodes.Status404NotFound, new { articles.Errors });
                //}

                //return Ok(articles.Data);
                #endregion
            }
            catch(NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        [HttpGet]
        [Route("all-articles")]
        public async Task<IActionResult> GetAllArticles([FromQuery] int page, int pageSize, CancellationToken cancellationToken)
        {
            try
            {

                if (cancellationToken.IsCancellationRequested)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Operation Cancelled");
                }
                
                var articles = await _articlesRepository.GetAllArticles(page, pageSize, cancellationToken);

                #region switchStarement
                return articles switch
                {
                    { IsSuccess: true, Data: not null } => new JsonResult(articles.Data, new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                    }),
                    { IsSuccess: false, Errors: not null}=> HandleFailureResult(articles.Errors),
                    _ => BadRequest("Invalid articles object")
                };

                #endregion

                #region IfStatement
                //if (articles.Data is null)
                //{
                //    return StatusCode(StatusCodes.Status400BadRequest, new { articles.Errors });
                //}

                //return new JsonResult(articles.Data, new JsonSerializerOptions
                //{
                //    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull

                //});

                #endregion

            }
            catch(ConflictException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;

            }
           
        }

      

        [HttpGet("articles-with-comments")]
        public IActionResult GetArticlesWithComments([FromBody] int page, int pageSize, CancellationToken cancellationToken)
        {
            try
            {
                var articlesWithComments = _articlesRepository.GetArticlesWithComments(page, pageSize, cancellationToken);
                #region SwitchStatement
                return articlesWithComments switch
                {
                    { IsSuccess: true, Data: not null } => new JsonResult(articlesWithComments.Data, new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                    }),
                    { IsSuccess: false, Errors: not null}=> BadRequest(articlesWithComments.Errors),
                    { Data: null} => BadRequest(articlesWithComments.Errors),
                    _ => BadRequest(" Invalid articlesWithComments objects")
                };
                #endregion


                #region IfStatement
                //if (articlesWithComments.Data is null)
                //{
                //    return StatusCode(StatusCodes.Status400BadRequest, new { articlesWithComments.Errors });
                //}

                //return new JsonResult(articlesWithComments.Data, new JsonSerializerOptions
                //{
                //    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                //});
                #endregion

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

                Log.Information("CommentsFromArticles => {@commentsfromarticles}", commentsfromarticles);
                _logger.LogInformation("Log Testing");

                #region SwitchStatement
                return commentsfromarticles switch
                {
                    { IsSuccess: true, Data: not null } => new JsonResult(commentsfromarticles.Data, new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull

                    }),
                    { IsSuccess: false, Errors: not null } => BadRequest(commentsfromarticles.Errors),
                    { Data: null } => BadRequest(commentsfromarticles.Errors),
                    _ => BadRequest("Invalid commentsfromarticles objects")
                } ;
                #endregion

                #region IfStatement
                //if (commentsfromarticles.Data is null)
                //{
                //    return StatusCode(StatusCodes.Status400BadRequest, new { commentsfromarticles.Errors });
                //}

                //return new JsonResult(commentsfromarticles.Data, new JsonSerializerOptions
                //{
                //    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull

                //});
                #endregion
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpPost]
        //[ProducesResponseType(typeof(ArticlesGetDTOs), StatusCodes.Status201Created )]
        //[ProducesResponseType(typeof(string[]), StatusCodes.Status400BadRequest)]
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

                Log.Information("Articles =>{@articles}", articles);
                #region SwitchStatement
                return articles switch
                {
                    { IsSuccess: true, Data: not null } => new JsonResult(articles.Data, new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull

                    })
                    { StatusCode = StatusCodes.Status201Created },
                    { IsSuccess: false, Errors: not null } => BadRequest(articles.Errors),
                    { Data: null } => BadRequest(articles.Errors),
                    _ => BadRequest("Invalid articles object") //Default case if none of the above case match
                };

                //#endregion

                #region IfStarement
                //if(articles.IsSuccess)
                //{
                //    return Ok(articles.Data);
                //}
                //else
                //{
                //    return BadRequest(articles.Errors);
                //}
                //if (articles.Data is null)
                //{
                //    return BadRequest(articles.Errors);
                //}
                #endregion
            }catch(ConflictException ex) 
            { 
                return Conflict();
            }


            catch (Exception ex)
            {
                throw new NotImplementedException("An error Occured");
            }
        }


        [HttpPatch]
        public async Task<IActionResult> UpdateArticles([FromForm] ArticlesUpdateDTOs articlesUpdateDTOs)
        {
            try
            {
                var articles = await _articlesRepository.UpdateArticles(articlesUpdateDTOs);


                #region SwitchStatement

                return articles switch
                {
                    { IsSuccess: true, Data: not null } => new JsonResult(articles.Data, new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                    }),
                    { IsSuccess: false, Errors: not null } => BadRequest(articles?.Errors),
                    { Data: null } => BadRequest(articles.Errors),
                    _ => BadRequest("Invalid articles update")
                };

                #endregion

                #region IfStatement

                //if (articles.IsSuccess)
                //{
                //    return Ok(articles.Data);

                //}
                //else
                //{
                //    return StatusCode(StatusCodes.Status404NotFound, new { articles.Errors });
                //}

                #endregion

            }
            catch (Exception ex)
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

                #region SwitchStatement
                return articles switch
                {
                    { IsSuccess: true, Data: not null}=> new  JsonResult(articles.Data, new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                    }),
                    { IsSuccess: false, Errors: not null}=> HandleFailureResult(articles.Errors),
                    { Data: null } => BadRequest(articles?.Errors),
                    _ => BadRequest("Invalid articles Delete")
                };
                #endregion

                #region IFStatement
                //if (articles.IsSuccess)
                //{
                //    return Ok(articles.Data);

                //}
                //else
                //{
                //    return StatusCode(StatusCodes.Status404NotFound, new { articles.Errors });

                //}
                #endregion


            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

    }
}
#endregion