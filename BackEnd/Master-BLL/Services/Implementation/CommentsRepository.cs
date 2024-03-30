using AutoMapper;
using Master_BLL.DTOs.Comment;
using Master_BLL.Repository.Interface;
using Master_BLL.Services.Interface;
using Master_BLL.Static.Cache;
using Master_BLL.Validator;
using Master_DAL.Abstraction;
using Master_DAL.DbContext;
using Master_DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Implementation
{

    public class CommentsRepository : ICommentsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCacheRepository _memoryCacheRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CommentsRepository(ApplicationDbContext applicationDbContext, IMapper mapper, IMemoryCacheRepository memoryCacheRepository, IUnitOfWork unitOfWork)
        {
            _context = applicationDbContext;
            _mapper = mapper;
            _memoryCacheRepository = memoryCacheRepository;
            _unitOfWork = unitOfWork;
            
        }
        public async Task<Result<CommentsGetDTOs>> DeleteComments(Guid CommentsId)
        {
            try
            {
                await _memoryCacheRepository.RemoveAsync(CacheKeys.Comments);
                var comments = await _unitOfWork.Repository<Comments>().GetByIdAsync(CommentsId);
                if(comments is null)
                {
                    return Result<CommentsGetDTOs>.Failure("Comments not Found");      
                }

                _unitOfWork.Repository<Comments>().Delete(comments);
                await _unitOfWork.SaveChangesAsync();
                var getCommentsDTOs = _mapper.Map<CommentsGetDTOs>(comments);
                return Result<CommentsGetDTOs>.Success(getCommentsDTOs);

            }catch (Exception ex)
            {
                throw new Exception("An error occured while deleting comments");
            }
        }

        public async Task<Result<List<CommentsGetDTOs>>> GetAllComments(int page, int pazeSize, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = CacheKeys.Comments;
                var cacheData = await _memoryCacheRepository.GetCahceKey<List<CommentsGetDTOs>>(cacheKey);
                if(cacheData is not null && cacheData.Count > 0)
                {
                    return Result<List<CommentsGetDTOs>>.Success(cacheData);
                }
                List<Comments> comments = await _context.Comments.AsNoTracking()
                    .Skip((page - 1)* pazeSize).Take(pazeSize).ToListAsync(cancellationToken);

                List<CommentsGetDTOs> commentsGets = comments.Select(x=> _mapper.Map<CommentsGetDTOs>(x)).ToList();

                await _memoryCacheRepository.SetAsync(cacheKey, commentsGets, new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30)
                }, cancellationToken);

                return Result<List<CommentsGetDTOs>>.Success(commentsGets);

            }catch(Exception ex)
            {
                throw new Exception("An error occured while getting all comments");
            }
        }

        public async Task<Result<CommentsGetDTOs>> GetCommentsByCommentId(Guid commentId, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"GetCommentsByCommentId{commentId}";
                var cacheData = await _memoryCacheRepository.GetCahceKey<CommentsGetDTOs>(cacheKey);
                if(cacheData is not null)
                {
                    return Result<CommentsGetDTOs>.Success(cacheData);
                }

                var comments = await _context.Comments.SingleOrDefaultAsync(x=>x.CommentsId == commentId);
                if(comments is null)
                {
                    return Result<CommentsGetDTOs>.Failure("Comments is not Found");
                }
                var commentsgetDTO = _mapper.Map<CommentsGetDTOs>(comments);
                await _memoryCacheRepository.SetAsync(cacheKey, commentsgetDTO, new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30)
                }, cancellationToken);

                return Result<CommentsGetDTOs>.Success(commentsgetDTO);

            }catch(Exception ex) 
            {
                throw new Exception("An error occured while geeting comments");
            }
        }

        public async Task<Result<List<CommentsGetDTOs>>> GetCommentsByUserId(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"GetCommentsByUserId{userId}";
                var cacheData = await _memoryCacheRepository.GetCahceKey<List<CommentsGetDTOs>>(cacheKey);

                if(cacheData is not null )
                {
                    return Result<List<CommentsGetDTOs>>.Success(cacheData);
                }

                List<Comments> comments = await _context.Comments.Include(x=>x.Articles).ThenInclude(y=>y.ApplicationUser).Where(x=>x.Articles.ApplicationUserId == userId.ToString()).ToListAsync();
                List<CommentsGetDTOs> commentsDTOsByUserId = comments.Select(x=>_mapper.Map<CommentsGetDTOs>(x)).ToList();

                await _memoryCacheRepository.SetAsync(cacheKey, commentsDTOsByUserId, new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30)
                }, cancellationToken) ;
                return Result<List<CommentsGetDTOs>>.Success(commentsDTOsByUserId);

            }catch(Exception ex)
            {
                throw new Exception("An error occured while Getting commnets from UserId");
            }
        }

        
        public async Task<Result<CommentsGetDTOs>> SaveComments(CommentsCreateDTOs commentsCreateDTOs, Guid Id)
        {
            try
            {
                await _memoryCacheRepository.RemoveAsync(CacheKeys.Comments);

                var validationError = CommentsCreateValidator.Validate(commentsCreateDTOs);
                if(validationError.Any())
                {
                    return Result<CommentsGetDTOs>.Failure(validationError.ToArray());
                }

                var comments = _mapper.Map<Comments>(commentsCreateDTOs);
                comments.ApplicationUserId = Id.ToString();
                await _unitOfWork.Repository<Comments>().AddAsync(comments);
                await _unitOfWork.SaveChangesAsync();

                var commentsGetDTOs = _mapper.Map<CommentsGetDTOs>(comments);
                return Result<CommentsGetDTOs>.Success(commentsGetDTOs);

            }
            catch(Exception ex)
            {
                throw new Exception("An error occured while Saving Comments");
            }
        }

        public async Task<Result<CommentsGetDTOs>> UpdateComments(CommentsUpdateDTOs commentsUpdateDTOs)
        {
            try
            {
                await _memoryCacheRepository.RemoveAsync(CacheKeys.Comments);
                Comments CommentsToBeUpdated = await _unitOfWork.Repository<Comments>().GetByIdAsync(commentsUpdateDTOs.CommentsId);
                if(CommentsToBeUpdated is null)
                {
                    return Result<CommentsGetDTOs>.Failure("Comments not Found");
                }
                _mapper.Map(commentsUpdateDTOs, CommentsToBeUpdated);
                await _unitOfWork.SaveChangesAsync();
                var commentsGetDTOs = _mapper.Map<CommentsGetDTOs>(CommentsToBeUpdated);

                return Result<CommentsGetDTOs>.Success(commentsGetDTOs);

            }catch(Exception)
            {
                throw new Exception("An error occured while updating Coments");
            }
        }
    }
}
