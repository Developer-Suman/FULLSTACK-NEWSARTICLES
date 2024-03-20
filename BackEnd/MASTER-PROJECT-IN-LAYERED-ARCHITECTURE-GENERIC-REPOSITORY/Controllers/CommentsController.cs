using Master_BLL.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsRepository _commentsRepository;

        public CommentsController(ICommentsRepository commentsRepository)
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
    }
}
