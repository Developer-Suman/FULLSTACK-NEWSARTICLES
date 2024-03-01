﻿using Master_BLL.DTOs.Articles;
using Master_BLL.Services.Interface;
using Master_DAL.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;

namespace MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Controllers
{
    [Route("api/[controller]"), EnableCors("AllowAllOrigins")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticlesRepository _articlesRepository;


        public ArticlesController(IArticlesRepository articlesRepository, IMemoryCacheRepository memoryCacheRepository)
        {
            _articlesRepository = articlesRepository;
        }

        [HttpGet("{ArticlesId}")]
        public async Task<IActionResult> GetArticlesById(Guid ArticlesId)
        {
            try
            {
                var articles = await _articlesRepository.GetArticlesById(ArticlesId);


                if (articles.IsException)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { articles.Errors });

                }
                if (!articles.IsSuccess)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { articles.Errors });
                }
                else
                {
                    return Ok(articles.Data);

                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }

        }

        [HttpGet("all-articles")]
        public async Task<IActionResult> GetAllArticles(int page, int pageSize)
        {


            var articles = await _articlesRepository.GetAllArticles(page, pageSize);
            if (articles.Data is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { articles.Errors });
            }
            return Ok(articles.Data);
        }

        [HttpGet("articles-with-comments")]
        public IActionResult GetArticlesWithComments(int page, int pageSize)
        {
            var articlesWithComments = _articlesRepository.GetArticlesWithComments(page, pageSize);
            return Ok(articlesWithComments.Data);
        }

        [HttpGet("all-comments-from-articles")]
        public async Task<IActionResult> GetAllCommentsFromArticles(int page, int pageSize)
        {
            var commentsfromarticles = await _articlesRepository.GetCommentsWithArticlesName(page, pageSize);
            return Ok(commentsfromarticles.Data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveArticles([FromForm] ArticlesCreateDTOs articlesCreateDTOs)
        {
            if(articlesCreateDTOs.filesList.Count() < 0 && articlesCreateDTOs.filesList is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "No Files are Added");

            }

            var articles = await _articlesRepository.SaveArticles(articlesCreateDTOs);

            if(articles.IsException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { articles.Errors });
            }

            return Ok(articles.Data);
        }


        [HttpPatch]
        public async Task<IActionResult> UpdateArticles([FromForm] ArticlesUpdateDTOs articlesUpdateDTOs)
        {

            var articles = await _articlesRepository.UpdateArticles(articlesUpdateDTOs);
            if (articles.IsException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { articles.Errors });
            }
            if (!articles.IsSuccess)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { articles.Errors });
            }

            return Ok(articles.Data);
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
                if(articles.IsException)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { articles.Errors });
                }

                return Ok(articles.Data);
            }catch(Exception)
            {
                throw new Exception("An error occured while Deeting");
            }
        }

    }
}
