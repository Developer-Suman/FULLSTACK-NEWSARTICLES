using AutoMapper;
using Master_BLL.DTOs.Articles;
using Master_BLL.DTOs.Comment;
using Master_BLL.DTOs.RegistrationDTOs;
using Master_BLL.Repository.Interface;
using Master_BLL.Services.Interface;
using Master_BLL.Static.Cache;
using Master_DAL.Abstraction;
using Master_DAL.DbContext;
using Master_DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
                var articles = await uow.Repository<Articles>().GetByIdAsync(ArticlesId);
                if(articles is null)
                {
                    return Result<ArticlesGetDTOs>.Failure("Articles Not Found");
                }
                uow.Repository<Articles>().Delete(articles);
                await uow.SaveChangesAsync();

                var getArticlesDTO = _mapper.Map<ArticlesGetDTOs>(articles);
                return Result<ArticlesGetDTOs>.Success(getArticlesDTO);

            }catch (Exception ex)
            {
                return Result<ArticlesGetDTOs>.Exception("An error occured while Deleting");
            }
            

        }

        public async Task<Result<List<ArticlesGetDTOs>>> GetAllArticles(int page, int pageSize)
        {
            try
            {
                var cacheKey = CacheKeys.Articles;
                var cacheData = await _memoryCacheRepository.GetCahceKey<List<ArticlesGetDTOs>>(cacheKey);
                if(cacheData is not null && cacheData.Count > 0)
                {
                    return Result<List<ArticlesGetDTOs>>.Success(cacheData);
                }
                List<Articles> artices = await _context.Articles.AsNoTracking().OrderByDescending(x => x.CreatedAt)
                    .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

              

                List<ArticlesGetDTOs> articlesGetDTOs = artices.Select(x=> _mapper.Map<ArticlesGetDTOs>(x)).ToList();

                await _memoryCacheRepository.SetAsync(cacheKey, articlesGetDTOs, new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30)
                });

                return Result<List<ArticlesGetDTOs>>.Success(articlesGetDTOs);

            }catch(Exception ex)
            {
                throw new Exception("An error occured while getting All articles");
            }
        }

        public async Task<Result<ArticlesGetDTOs>> GetArticlesById(Guid Id)
        {
            try
            {
                var cacheKeys = $"GetArticlesById{Id}";
                var cacheData = await _memoryCacheRepository.GetCahceKey<ArticlesGetDTOs>(cacheKeys);
                if(cacheData is not null)
                {
                    return Result<ArticlesGetDTOs>.Success(cacheData);
                }

                var articles = await _context.Articles.SingleOrDefaultAsync(x=>x.ArticlesId == Id);
                if(articles is null)
                {
                    return Result<ArticlesGetDTOs>.Failure("Not Found");

                }
                var testForException = _mapper.Map<ArticlesUpdateDTOs>(articles);

                var articlesDTO = _mapper.Map<ArticlesGetDTOs>(articles);
                await _memoryCacheRepository.SetAsync(cacheKeys, articlesDTO, new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30)
                });
                
                return Result<ArticlesGetDTOs>.Success(articlesDTO);




            }catch(Exception ex)
            {
                //return Result<ArticlesGetDTOs>.Exception(ex.ToString());
                throw new Exception(ex.Message);
               
              
            }
        }

        public Result<IQueryable<ArticlesWithCommentsDTOs>> GetArticlesWithComments(int page, int pageSize)
        {
            try
            {
                IQueryable <ArticlesWithCommentsDTOs> articleswithComments = _context.Articles
                    .Include(x => x.Comments)
                    .Select(articles => new ArticlesWithCommentsDTOs
                    {
                        ArticlesId = articles.ArticlesId,
                        ArticlesTitle = articles.ArticlesTitle,
                        ArticlesContent = articles.ArticlesContent,
                        Comments = articles.Comments.Select(a=>_mapper.Map<CommentsGetDTOs>(a)).ToList(),
                   

                    }).AsNoTracking().Skip((page-1)*pageSize).Take(pageSize).AsQueryable();



                return Result<IQueryable<ArticlesWithCommentsDTOs>>.Success(articleswithComments);

            }
            catch(Exception ex)
            {
                throw new Exception("An error occured while getting AllArticleswithComments");
            }
        }

        public async Task<Result<List<CommentsWithArticles>>> GetCommentsWithArticlesName(int page, int pageSize)
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



                await _memoryCacheRepository.SetAsync(cacheKey, commentsWithArticles, new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30)

                });



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

        public async Task<Result<ArticlesGetDTOs>> SaveMultipleImages(ArticlesCreateDTOs articlesCreateDTOs)
        {
            try
            {
                await _memoryCacheRepository.RemoveAsync(CacheKeys.Articles);

                var images = await _uploadImageRepository.UploadImage(articlesCreateDTOs.Files);
                if (images is null || !images.Any())
                {
                    return Result<ArticlesGetDTOs>.Exception("Image Upload Failed");
                }

                List<string> imagess = await _uploadImageRepository.UploadMultipleImage(articlesCreateDTOs.filesList);
                if (images is null || !images.Any())
                {
                    return Result<ArticlesGetDTOs>.Exception("Image upload Failed");
                }



                



                var articles = _mapper.Map<Articles>(articlesCreateDTOs);
                articles.ImageUrl = images;
                await uow.Repository<Articles>().AddAsync(articles);
                await uow.SaveChangesAsync();


                List<ArticlesImagesDTOs> articlesImages = articlesCreateDTOs.articlesImages.ToList();
                IList<ArticlesImage> articlesImagesList = articlesImages.Select(article => new ArticlesImage
                {
                    ArticlesImagesUrl = images,
                    ArticlesId = articles.ArticlesId



                }).ToList();


                //var articlesImageList = new List<ArticlesImage>();
                //foreach (var image in imagess)
                //{
                //    articlesImageList.Add(new ArticlesImage
                //    {
                //        ArticlesImagesUrl = images,
                //        ArticlesId = articles.ArticlesId

                //    });

                //}
                //articles.ArticlesImages = articlesImageList;

                //await uow.Repository<ArticlesImage>().AddRange(articlesImageList);
                var Articles = new Articles()
                {

                    ArticlesImages = articlesImagesList,

                };

                await uow.Repository<Articles>().AddAsync(Articles);
                await uow.SaveChangesAsync();



                var articlesGet = _mapper.Map<ArticlesGetDTOs>(articles);
                return Result<ArticlesGetDTOs>.Success(articlesGet);

                
                

            }catch(Exception ex)
            {
                return Result<ArticlesGetDTOs>.Exception("An error occured while adding multiple Images");
            }

        }

        public async Task<Result<ArticlesGetDTOs>> SaveArticles(ArticlesCreateDTOs articlesCreateDTOs)
        {
            try
            {
                await _memoryCacheRepository.RemoveAsync(CacheKeys.Articles);
                var images = await _uploadImageRepository.UploadImage(articlesCreateDTOs.Files);
                if(images is null || !images.Any())
                {
                    return Result<ArticlesGetDTOs>.Exception("Image Upload Failed");
                }

                var articles = _mapper.Map<Articles>(articlesCreateDTOs);
                articles.ImageUrl = images;

                if(string.IsNullOrEmpty(articles.ImageUrl))
                {
                    return Result<ArticlesGetDTOs>.Exception("Images URLs are missing");

                }

                if(articles is null)
                {
                    return Result<ArticlesGetDTOs>.Exception("Mapping to articles Failed");
                }
                await uow.Repository<Articles>().AddAsync(articles);
                await uow.SaveChangesAsync();
                var articlesGet = _mapper.Map<ArticlesGetDTOs>(articles);

                return Result<ArticlesGetDTOs>.Success(articlesGet);
                

            }catch(Exception ex)
            {
                return Result<ArticlesGetDTOs>.Exception("An error occur while Adding Articles");
            }
        }

        public async Task<Result<ArticlesGetDTOs>> UpdateArticles(ArticlesUpdateDTOs articlesUpdateDTOs)
        {
            try
            {
                await _memoryCacheRepository.RemoveAsync(CacheKeys.Articles);
                Articles articlesTobeUpdated = await uow.Repository<Articles>().GetByIdAsync(articlesUpdateDTOs.ArticlesId);

                if(articlesTobeUpdated is null)
                {
                    return Result<ArticlesGetDTOs>.Failure("Articles Not Found");
                }
                var updatedImage = await _uploadImageRepository.UpdateImage(articlesUpdateDTOs.Files, articlesTobeUpdated.ImageUrl);

                _mapper.Map(articlesUpdateDTOs, articlesTobeUpdated);

                articlesTobeUpdated.ImageUrl = updatedImage;
                await uow.SaveChangesAsync();
                var articlesGetDTOs = _mapper.Map<ArticlesGetDTOs>(articlesTobeUpdated);

                return Result<ArticlesGetDTOs>.Success(articlesGetDTOs);

            }
            catch(Exception ex)
            {
                return Result<ArticlesGetDTOs>.Exception("An error occured while Updating Articles");
            }
        }
    }
}
