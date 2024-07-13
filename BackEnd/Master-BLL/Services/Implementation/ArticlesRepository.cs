using AutoMapper;
using Master_BLL.DTOs.Articles;
using Master_BLL.DTOs.Comment;
using Master_BLL.DTOs.Likes;
using Master_BLL.DTOs.RegistrationDTOs;
using Master_BLL.Repository.Interface;
using Master_BLL.Services.Interface;
using Master_BLL.Static.Cache;
using Master_BLL.Validator;
using Master_DAL.Abstraction;
using Master_DAL.DbContext;
using Master_DAL.Exceptions;
using Master_DAL.Models;
using Master_DAL.Models.Enum.Likes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Master_BLL.Services.Implementation
{
    public class ArticlesRepository : IArticlesRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCacheRepository _memoryCacheRepository;
        private readonly IUploadImageRepository _uploadImageRepository;
        private readonly IUnitOfWork _unitofwork;

        public ArticlesRepository(IUnitOfWork unitOfWork, IUploadImageRepository uploadImageRepository, ApplicationDbContext applicationDbContext, IMapper mapper, IMemoryCacheRepository memoryCacheRepository)
        {
            _context = applicationDbContext;
            _mapper = mapper;
            _unitofwork = unitOfWork;
            _uploadImageRepository = uploadImageRepository;
            _memoryCacheRepository = memoryCacheRepository;

        }
        public async Task<Result<ArticlesGetDTOs>> Delete(string Id )
        {
            try
            {
                await _memoryCacheRepository.RemoveAsync(CacheKeys.Articles);
                var articles = await _unitofwork.Repository<Articles>().GetByIdAsync(Id);
                if (articles is null)
                {
                    return Result<ArticlesGetDTOs>.Failure("Not Found", "The Articles cannot be deleted");
                }

                _unitofwork.Repository<Articles>().Delete(articles);

                await _unitofwork.SaveChangesAsync();

                var getArticlesDTO = _mapper.Map<ArticlesGetDTOs>(articles);
                return Result<ArticlesGetDTOs>.Success(getArticlesDTO);

            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while Deleting");
            }


        }
            
        public async Task<Result<List<ArticlesGetDTOs>>> GetAll(int page, int pageSize, CancellationToken cancellationToken)
        {

            try
            {
                var cacheKey = CacheKeys.Articles;
                var cacheData = await _memoryCacheRepository.GetCahceKey<List<ArticlesGetDTOs>>(cacheKey);

                if (cacheData is not null && cacheData.Count > 0)
                {
                    return Result<List<ArticlesGetDTOs>>.Success(cacheData);
                }

                List<Articles> artices = await _context.Articles.AsNoTracking().OrderByDescending(x => x.PublishedDate)
                    .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
                if (artices.Count < 0)
                {
                    return Result<List<ArticlesGetDTOs>>.Failure("Not Found");
                }

                //throw new MappingException("An error occurred while mapping");
                List<ArticlesGetDTOs> articlesGetDTOs = artices.Select(x => _mapper.Map<ArticlesGetDTOs>(x)).ToList();

                if (articlesGetDTOs is null)
                {
                    return Result<List<ArticlesGetDTOs>>.Failure("Not Found");
                }

                await _memoryCacheRepository.SetAsync(cacheKey, articlesGetDTOs, new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30)
                }, cancellationToken);

                return Result<List<ArticlesGetDTOs>>.Success(articlesGetDTOs);

            }
            catch (ConflictException ex)
            {
                throw new ConflictException("An error occured while mapping reference of database");
            }
            catch (Exception ex)
            {
                throw;

            }

        }

        public async Task<Result<ArticlesGetDTOs>> GetById(string Id, CancellationToken cancellationToken)
        {

            var cacheKeys = $"GetById{Id}";



            var cacheData = await _memoryCacheRepository.GetCahceKey<ArticlesGetDTOs>(cacheKeys);
            if (cacheData is not null)
            {
                return Result<ArticlesGetDTOs>.Success(cacheData);
            }

            var articles = await _context.Articles.SingleOrDefaultAsync(x => x.Id == Id);
            if (articles is null)
            {
                return Result<ArticlesGetDTOs>.Failure("Articles not found");
            }
            var articlesDTO = _mapper.Map<ArticlesGetDTOs>(articles);
            await _memoryCacheRepository.SetAsync(cacheKeys, articlesDTO, new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30)
            }, cancellationToken);

            return Result<ArticlesGetDTOs>.Success(articlesDTO);

        }

        public Result<IQueryable<ArticlesWithCommentsDTOs>> GetArticlesWithComments(int page, int pageSize, CancellationToken cancellationToken)
        {

            var articlesQuery = _context.Articles
                .Include(articles => articles.Comments)
                .OrderBy(article => article.Id)
                .AsSingleQuery()
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var articlesWithCommentsDTOs = articlesQuery
                .Select(articles => new ArticlesWithCommentsDTOs(
                articles.Id,
                articles.Title,
                articles.Content,
                articles.PublishedDate,
                true,
                articles.Comments.Select(a => _mapper.Map<CommentsGetDTOs>(a)).ToList()
                )).ToList();

            #region Less Optimized
            //IQueryable<ArticlesWithCommentsDTOs> articleswithComments = _context.Articles
            //.Include(x => x.Comments)
            //.OrderBy(article => article.Id)
            //.AsSplitQuery()
            //.Select(articles => new ArticlesWithCommentsDTOs(
            //    articles.Id,
            //    articles.Title,
            //    articles.Content,
            //    articles.PublishedDate,
            //    true,
            //    articles.Comments.Select(a => _mapper.Map<CommentsGetDTOs>(a)).ToList()
            //    ))
            //.AsNoTracking()
            //.Skip((page - 1) * pageSize)
            //.Take(pageSize);
            #endregion
            if (!articlesWithCommentsDTOs.Any())
            {
                return Result<IQueryable<ArticlesWithCommentsDTOs>>.Failure("Could not Found Articles with Comments");
            }

            return Result<IQueryable<ArticlesWithCommentsDTOs>>.Success(articlesWithCommentsDTOs.AsQueryable());


        }


        public async Task<Result<List<CommentsWithArticlesDTOs>>> GetArticlesDetails(string ArticlesId,CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"GetArticlesDetails{ArticlesId}";
                var cacheData = await _memoryCacheRepository.GetCahceKey<List<CommentsWithArticlesDTOs>>(cacheKey);

                if (cacheData is not null)
                {
                    return Result<List<CommentsWithArticlesDTOs>>.Success(cacheData);
                }



                List<CommentsWithArticlesDTOs> commentsWithArticles = await _context.Articles
                .SelectMany(a => a.Comments
                .Where(x=>x.ArticlesId == ArticlesId)
                .Select(x => new CommentsWithArticlesDTOs(
                    x.Content,
                    x.Id,
                    a.Content
                    )
                )).AsNoTracking().Skip((1 - 1) * 1).Take(1).ToListAsync();

                if (commentsWithArticles is null)
                {
                    return Result<List<CommentsWithArticlesDTOs>>.Failure("All the items were not found");
                };



                await _memoryCacheRepository.SetAsync(cacheKey, commentsWithArticles, new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30)

                }, cancellationToken);



                return Result<List<CommentsWithArticlesDTOs>>.Success(commentsWithArticles);

            }
            catch (Exception)
            {
                throw new Exception("An error occured while getting Comments from Articles");
            }
        }


        public Task<Result<List<CommentsWithArticlesDTOs>>> GetHighRatedReview(string ArticlesId)
        {
            throw new NotImplementedException();
            //var articles = _context.Articles.Find(ArticlesId);
            //if (articles != null)
            //{
            //    _context.Entry(articles).Collection(p => p.ArticlesImages).Query()
            //        .Where(r => r.Rating > 4) // Filter to only high-rated reviews
            //        .Load();
            //}
        }


        public async Task<Result<List<CommentsGetDTOs>>> GetMoreComments(string ArticlesId, int skip, int take, CancellationToken cancellationToken)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var articles = _context.Articles
                                     .Include(p => p.Comments) 
                                     .FirstOrDefault(p => p.Id == ArticlesId);

                    if (articles is null)
                    {
                        return Result<List<CommentsGetDTOs>>.Failure("NotFound", "Articles are not Found");
                    }

                    // Explicitly load more Comments
                    var moreComments = _context.Comments
                                             .Where(r => r.ArticlesId == ArticlesId)
                                             .OrderByDescending(r => r.Id)  
                                             .Skip(skip)
                                             .Take(take)
                                             .ToList();

                    // Add the additional Comments to the existing collection
                    articles.Comments = articles.Comments.Concat(moreComments).ToList();

                    var reviewModels = articles.Comments.Select(r => new CommentsGetDTOs
                    (
                        r.Id,
                        r.Content,
                        r.ArticlesId
                        )).ToList();

                    return Result<List<CommentsGetDTOs>>.Success(reviewModels);

                }
                catch (Exception ex)
                {
                    throw new Exception("An error occured while getting Review");
                }
               
            }
        }

        public async Task<Result<LikesArticlesGetDTOs>> LikeArticles(LikesArticlesCreateDTOs likesArticlesCreateDTOs)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                
                try
                {
                    await _memoryCacheRepository.RemoveAsync(CacheKeys.Likes);
                    string newId = Guid.NewGuid().ToString();
                    var likeArticles = new Likes(
                        newId,
                        likesArticlesCreateDTOs.userId,
                        likesArticlesCreateDTOs.articlesId,
                        LikeableType.Article
                        );
                    await _unitofwork.Repository<Likes>().AddAsync(likeArticles);
                    await _unitofwork.SaveChangesAsync();

                    scope.Complete();
                    return Result<LikesArticlesGetDTOs>.Success(_mapper.Map<LikesArticlesGetDTOs>(likeArticles));
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception("An error occured while Like Articles");
                }
            }
          
        }

        public async Task<Result<ArticlesGetDTOs>> Save(ArticlesCreateDTOs articlesCreateDTOs, List<IFormFile> filesList, string userId)
        {
            using(var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await _memoryCacheRepository.RemoveAsync(CacheKeys.Articles);


                    var validationError = ArticleValidator.Validate(articlesCreateDTOs);
                    if (validationError.Any())
                    {
                        return Result<ArticlesGetDTOs>.Failure(validationError.ToArray());
                    }


                    string newId = Guid.NewGuid().ToString();
                    var articlesData = new Articles(
                        newId,
                        articlesCreateDTOs.ArticlesTitle,
                        articlesCreateDTOs.ArticlesContent,
                        true
                        );

                    articlesData.UserId = userId;

                    if (articlesData is null)
                    {
                        return Result<ArticlesGetDTOs>.Failure("NotFound", "Error while mapping");
                    }

                    await _unitofwork.Repository<Articles>().AddAsync(articlesData);

                    var tasks = filesList.Select(async item =>
                    {
                        string imageUrl = await _uploadImageRepository.UploadImage(item);
                        return new ArticlesImage(
                            Guid.NewGuid().ToString(),
                            imageUrl,
                            newId
                            );

                    }).ToList();

                    //Await the completion of all tasks
                    var imagesPath = (await Task.WhenAll(tasks)).ToList();
                    await _unitofwork.Repository<ArticlesImage>().AddRange(imagesPath);
                    await _unitofwork.SaveChangesAsync();

                    var resultDTOs = new ArticlesGetDTOs(
                        articlesData.Id,
                        articlesData.Title,
                        articlesData.Content,
                        articlesData.PublishedDate,
                        articlesData.IsActive ?? true,
                        articlesData.UserId,
                        imagesPath.Select(image=>image.ImagesUrl).ToList());

                    scope.Complete();

                    return Result<ArticlesGetDTOs>.Success(_mapper.Map<ArticlesGetDTOs>(resultDTOs));

                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new ConflictException("An Exception occured while Adding Articles");
                }
            }


        }

        public async Task<Result<ArticlesGetDTOs>> Update(string ArticlesId, ArticlesUpdateDTOs articlesUpdateDTOs, List<IFormFile> multipleFiles)
        {
            using(var scope= new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await _memoryCacheRepository.RemoveAsync(CacheKeys.Articles);
                    if (string.IsNullOrEmpty(ArticlesId))
                    {
                        return Result<ArticlesGetDTOs>.Failure("Please provide a valid ArticlesId");
                    }

                    var articlesToBeUpdated = await _unitofwork.Repository<Articles>().GetByIdAsync(ArticlesId);
                    if (articlesToBeUpdated is null)
                    {
                        return Result<ArticlesGetDTOs>.Failure("NotFound", "ArticlesData are not Found");
                    }

                    List<string> articlesImages = _context.ArticlesImages.Where(p => p.ArticlesId == articlesToBeUpdated.Id).AsQueryable().Select(x => x.ImagesUrl).ToList();

                    List<string> updateImageUrl = await _uploadImageRepository.UpdateMultipleImage(multipleFiles, articlesImages);

                    List<ArticlesImage> articlesImages1 = await _context.ArticlesImages.Where(x => x.ArticlesId == articlesToBeUpdated.Id).ToListAsync();

                    _unitofwork.Repository<ArticlesImage>().DeleteRange(articlesImages1);


                    List<ArticlesImage> updatedImages = new List<ArticlesImage>();

                    var task = multipleFiles.Select(async item =>
                    {
                        string imageURL = await _uploadImageRepository.UploadImage(item);
                        return new ArticlesImage(
                            Guid.NewGuid().ToString(),
                            imageURL,
                            articlesToBeUpdated.Id);
                    });

                    var results = await Task.WhenAll(task);
                    updatedImages.AddRange(results);

                    articlesToBeUpdated.ArticlesImages = updatedImages;

                    _mapper.Map(articlesUpdateDTOs, articlesToBeUpdated);
                    await _unitofwork.SaveChangesAsync();

                    var resultDTOs = new ArticlesGetDTOs(
                        articlesToBeUpdated.Id,
                        articlesToBeUpdated.Title,
                        articlesToBeUpdated.Content,
                        articlesToBeUpdated.PublishedDate,
                        articlesToBeUpdated.IsActive ?? true,
                        articlesToBeUpdated.UserId,
                        updatedImages.Select(image => image.ImagesUrl).ToList()
                        );

                    scope.Complete();

                    return Result<ArticlesGetDTOs>.Success(_mapper.Map<ArticlesGetDTOs>(resultDTOs));

                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception("An exception occured while updating Articles Data");
                }

            }
        }

  
    }
}