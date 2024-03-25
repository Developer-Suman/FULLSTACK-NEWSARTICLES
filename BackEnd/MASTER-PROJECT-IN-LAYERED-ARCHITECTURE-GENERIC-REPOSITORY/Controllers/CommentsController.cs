using AutoMapper;
using Master_BLL.DTOs.Authentication;
using Master_BLL.DTOs.Comment;
using Master_BLL.Services.Interface;
using Master_DAL.Models;
using MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Controllers
{

    [Authorize(Roles = "admin")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]"), EnableCors("AllowAllOrigins")]


    [ApiController]
    public class CommentsController : MasterProjectControllerBase
    {
        private readonly ICommentsRepository _commentsRepository;

        public CommentsController(ICommentsRepository commentsRepository, IMemoryCacheRepository memoryCacheRepository, UserManager<ApplicationUser> userManager, IMapper mapper) : base(userManager,mapper)
        {
            _commentsRepository = commentsRepository;
            
        }

        [HttpGet("{CommentsId}")]
        public async Task<IActionResult> GetCommentsById([FromRoute] Guid CommentsId, CancellationToken cancellationToken)
        {
            try
            {
                var commensts = await _commentsRepository.GetCommentsByCommentId(CommentsId, cancellationToken);
                if(!commensts.IsSuccess)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new {commensts.Errors});
                }

                return Ok(commensts.Data);

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveComments([FromBody] CommentsCreateDTOs commentsCreateDTOs)
        {
            try
            {
                GetCurrentUserFromDB();
                var userDetails = _currentUser;
                var comments = await _commentsRepository.SaveComments(commentsCreateDTOs, userDetails!.Id);

                #region switchStatement
                return comments switch
                {
                    { IsSuccess: true, Data: not null } => new JsonResult(comments.Data, new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull

                    }),
                    { IsSuccess: false, Errors: not null}=>BadRequest(comments.Errors),
                    { Data: null}=> BadRequest(comments.Errors),
                    _=> BadRequest("Invalid articles comments")
                };
                #endregion


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
