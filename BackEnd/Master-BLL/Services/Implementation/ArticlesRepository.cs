using AutoMapper;
using Master_BLL.DTOs.Articles;
using Master_BLL.DTOs.Comment;
using Master_BLL.DTOs.RegistrationDTOs;
using Master_BLL.Repository.Interface;
using Master_BLL.Services.Interface;
using Master_BLL.Static.Cache;
using Master_BLL.Validator;
using Master_DAL.Abstraction;
using Master_DAL.DbContext;
using Master_DAL.Exceptions;
using Master_DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Implementation
{
    public class ArticlesRepository : IArticlesRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCacheRepository _memoryCacheRepository;
        private readonly IUploadImageRepository _uploadImageRepository;
        private readonly IUnitOfWork uow;

        public ArticlesRepository(IUnitOfWork unitOfWork,IUploadImageRepository uploadImageRepository,ApplicationDbContext applicationDbContext, IMapper mapper, IMemoryCacheRepository memoryCacheRepository)
        {
            _context = applicationDbContext;
            _mapper = mapper;
            uow = unitOfWork;
            _uploadImageRepository = uploadImageRepository;
            _memoryCacheRepository = memoryCacheRepository;

        }
        public async Task<Result<ArticlesGetDTOs>> DeleteArticles(Guid ArticlesId)
        {
            try
            {
                await _memoryCacheRepository.RemoveAsync(CacheKeys.Articles);
                var articles = await uow.Repository<Articles>().GetByIdAsync(ArticlesId);
                if(articles is null)
                {
                    return Result<ArticlesGetDTOs>.Failure(new List<string> { "Not Found","The Articles cannot be deleted"});
                }
                
                uow.Repository<Articles>().Delete(articles);

                await uow.SaveChangesAsync();

                var getArticlesDTO = _mapper.Map<ArticlesGetDTOs>(articles);
                return Result<ArticlesGetDTOs>.Success(getArticlesDTO);

            }catch (Exception ex)
            {
                throw new Exception("An error occured while Deleting");
            }
            

        }

        public async Task<Result<List<ArticlesGetDTOs>>> GetAllArticles(int page, int pageSize, CancellationToken cancellationToken)
        {

            try
            {
                var cacheKey = CacheKeys.Articles;
                var cacheData = await _memoryCacheRepository.GetCahceKey<List<ArticlesGetDTOs>>(cacheKey);

                if (cacheData is not null && cacheData.Count > 0)
                {
                    return Result<List<ArticlesGetDTOs>>.Success(cacheData);
                }

                List<Articles> artices = await _context.Articles.AsNoTracking().OrderByDescending(x => x.CreatedAt)
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
            catch(ConflictException ex)
            {
                throw new ConflictException("An error occured while mapping reference of database") ;
            }
            catch(Exception ex)
            {
                throw;

            }
                       
        }




        public async Task<Result<ArticlesGetDTOs>> GetArticlesById(Guid Id, CancellationToken cancellationToken)
        {
         
                var cacheKeys = $"GetArticlesById{Id}";
                var data = "Suman Rai";
  
                var cacheData = await _memoryCacheRepository.GetCahceKey<ArticlesGetDTOs>(cacheKeys);
                if(cacheData is not null)
                {
                    return Result<ArticlesGetDTOs>.Success(cacheData);
                }

                var articles = await _context.Articles.SingleOrDefaultAsync(x=>x.ArticlesId == Id);
                if(articles is null)
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
           
                IQueryable<ArticlesWithCommentsDTOs> articleswithComments = _context.Articles
                .Include(x => x.Comments)
                .OrderBy(article => article.ArticlesId)  
                .AsSplitQuery()
                .Select(articles => new ArticlesWithCommentsDTOs
                {
                    ArticlesId = articles.ArticlesId,
                    ArticlesTitle = articles.ArticlesTitle,
                    ArticlesContent = articles.ArticlesContent,
                    Comments = articles.Comments.Select(a => _mapper.Map<CommentsGetDTOs>(a)).ToList(),
                })
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

                if(articleswithComments is null)
                {
                    return Result<IQueryable<ArticlesWithCommentsDTOs>>.Failure("Could not Found Articles with Comments");
                }

                return Result<IQueryable<ArticlesWithCommentsDTOs>>.Success(articleswithComments);

        
        }

        public async Task<Result<List<CommentsWithArticles>>> GetCommentsWithArticlesName(int page, int pageSize, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"GetCommentsWithArticlesName{page}{pageSize}";
                var cacheData = await _memoryCacheRepository.GetCahceKey<List<CommentsWithArticles>>(cacheKey);

                if (cacheData is not null)
                {
                    return Result<List<CommentsWithArticles>>.Success(cacheData);
                }

              

                List<CommentsWithArticles> commentsWithArticles = await _context.Articles.SelectMany(x => x.Comments
                .Select(x => new CommentsWithArticles()
                {
                    CommentsId = x.CommentsId,
                    CommentDescription = x.CommentDescription,
                    ArticleName = x.Articles.ArticlesTitle,

                })).AsNoTracking().Skip((1 - page) * pageSize).Take(pageSize).ToListAsync();

                if(commentsWithArticles is null)
                {
                    return Result<List<CommentsWithArticles>>.Failure("All the items were not found");
                };



                await _memoryCacheRepository.SetAsync(cacheKey, commentsWithArticles, new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30)

                }, cancellationToken);



                //List<CommentsWithArticles> commentsWithArticles = await _context.Articles.SelectMany(x => x.Comments
                //.Select(x => _mapper.Map<CommentsWithArticles>(x)));

                #region ImplementArticlesNameInAutomapper
                //List<CommentsWithArticles> commentsWithArticles = await _context.Articles.SelectMany(x => x.Comments
                //.Select(x => new CommentsWithArticles()
                //{
                //    CommentsId = x.CommentsId,
                //    CommentDescription = x.CommentDescription,
                //    ArticleName = x.Articles.ArticlesTitle,

                //})).AsNoTracking().Skip((1 - page) * pageSize).Take(pageSize).ToListAsync();
                #endregion


                return Result<List<CommentsWithArticles>>.Success(commentsWithArticles);

            }
            catch (Exception)
            {
                throw new Exception("An error occured while getting Comments from Articles");
            }
        }

        public async Task<Result<ArticlesGetDTOs>> SaveArticles(ArticlesCreateDTOs articlesCreateDTOs, Guid Id)
        {
            try
            {

                await _memoryCacheRepository.RemoveAsync(CacheKeys.Articles);


                var validationError = ArticleValidator.Validate(articlesCreateDTOs);
                if(validationError.Any()) 
                {
                    return Result<ArticlesGetDTOs>.Failure(validationError.ToArray());
                }



                List<string> images = await _uploadImageRepository.UploadMultipleImage(articlesCreateDTOs.filesList);
                if (images is null || !images.Any())
                {
                    return Result<ArticlesGetDTOs>.Failure( "Image upload Failed" );

                    #region OtherOption
                    //throw new ImageuploadExceptions("Image upload Failed");
                    //return Result<ArticlesGetDTOs>.Failure("Image upload Failed");
                    #endregion
                }


                var articles = _mapper.Map<Articles>(articlesCreateDTOs);

                if (images is null && images.Count() <=0 )
                {
                    return Result<ArticlesGetDTOs>.Failure("Images URLs are missing");
                    #region OtherOption
                    //return Result<ArticlesGetDTOs>.Failure(new List<string> { "Images URLs are missing" });
                    //throw new ImageUrlException("Images URLs are missing");
                    #endregion

                }
                if (articles is null)
                {                 
                    return Result<ArticlesGetDTOs>.Failure( "Mapping To articles Failed" );
                    #region Other Options
                    //return Result<ArticlesGetDTOs>.Failure("Mapping To articles Failed");
                    //Exception is more overhead
                    //throw new MappingException("Mapping To articles Failed");
                    #endregion

                }
                articles.ApplicationUserId = Id.ToString();
                await uow.Repository<Articles>().AddAsync(articles);
                await uow.SaveChangesAsync();

                #region UseSelectToSaveMultipleImage
                //List<string> imagess = await _uploadImageRepository.UploadMultipleImage(articlesCreateDTOs.filesList);
                //if (images is null || !images.Any())
                //{
                //    return Result<ArticlesGetDTOs>.Exception("Image upload Failed");
                //}


                //List<ArticlesImagesDTOs> articlesImages = articlesCreateDTOs.articlesImages.ToList();
                //IList<ArticlesImage> articlesImagesList = articlesImages.Select(article => new ArticlesImage
                //{
                //    ArticlesImagesUrl = images,
                //    ArticlesId = articles.ArticlesId

                //}).ToList();

                //var Articles = new Articles()
                //{

                //    ArticlesImages = articlesImagesList,

                //};

                //await uow.Repository<Articles>().AddAsync(Articles);
                #endregion


                var articlesImageList = new List<ArticlesImage>();
                foreach (var image in images)
                {
                    articlesImageList.Add(new ArticlesImage
                    {
                        ArticlesImagesUrl = image,
                        ArticlesId = articles.ArticlesId

                    });

                }
                //articles.ArticlesImages = articlesImageList;

                await uow.Repository<ArticlesImage>().AddRange(articlesImageList);



                await uow.SaveChangesAsync();



                var articlesGet = _mapper.Map<ArticlesGetDTOs>(articles);
                return Result<ArticlesGetDTOs>.Success(articlesGet);




            }
            catch (Exception ex)
            {
                throw new ConflictException("I got Exception like NotFoundException in Service");
                //return Result<ArticlesGetDTOs>.Failure("An unexpected error occured.");
                //throw new Exception("An error occured while adding multiple Images");
            }

        }

        public async Task<Result<ArticlesGetDTOs>> UpdateArticles(ArticlesUpdateDTOs articlesUpdateDTOs)
        {
            try
            {
                await _memoryCacheRepository.RemoveAsync(CacheKeys.Articles);
                Articles articlesTobeUpdated = await uow.Repository<Articles>().GetByIdAsync(articlesUpdateDTOs.ArticlesId);

                List<string> articlesImage = await _context.ArticlesImages.Where(articlesImage => articlesImage.ArticlesId == articlesUpdateDTOs.ArticlesId)
                    .Select(articlesImages => articlesImages.ArticlesImagesUrl).AsNoTracking().ToListAsync();
           
                if (articlesTobeUpdated is null)
                {
                    return Result<ArticlesGetDTOs>.Failure("Articles Not Found");
                }
                List<string> updatedImage = await _uploadImageRepository.UpdateMultipleImage(articlesUpdateDTOs.filesList, articlesImage);

                List<ArticlesImage> articlesImage1 = await _context.ArticlesImages.Where(x => x.ArticlesId == articlesTobeUpdated.ArticlesId).ToListAsync();
                uow.Repository<ArticlesImage>().DeleteRange(articlesImage1);
             
                foreach(var image in updatedImage)
                {
                    var articleImage = new ArticlesImage()
                    {
                        ArticlesId = articlesTobeUpdated.ArticlesId,
                        ArticlesImagesUrl = image
                    };

                    uow.Repository<ArticlesImage>().Update(articleImage);
                    
                }
              
                _mapper.Map(articlesTobeUpdated, articlesUpdateDTOs);
                await uow.SaveChangesAsync();
                var articlesGetDTOs = _mapper.Map<ArticlesGetDTOs>(articlesTobeUpdated);



                return Result<ArticlesGetDTOs>.Success(articlesGetDTOs);

            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while Updating Articles");
   
            }
        }
    }
}
