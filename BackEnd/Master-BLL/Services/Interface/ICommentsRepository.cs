using Master_BLL.DTOs.Comment;
using Master_DAL.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Interface
{
    public interface ICommentsRepository
    {
        Task<Result<CommentsGetDTOs>> SaveComments(CommentsCreateDTOs commentsCreateDTOs, Guid Id);
        Task<Result<List<CommentsGetDTOs>>> GetCommentsByUserId(Guid userId, CancellationToken cancellationToken);
        Task<Result<CommentsGetDTOs>> GetCommentsByCommentId(Guid commentId, CancellationToken cancellationToken);
        Task<Result<List<CommentsGetDTOs>>> GetAllComments(int page, int pazeSize, CancellationToken cancellationToken);
        Task<Result<CommentsGetDTOs>> UpdateComments(CommentsUpdateDTOs commentsUpdateDTOs);
        Task<Result<CommentsGetDTOs>> DeleteComments(Guid CommentsId);
    }
}
