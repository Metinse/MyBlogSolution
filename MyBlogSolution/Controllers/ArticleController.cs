using Microsoft.AspNetCore.Mvc;
using MyBlog.Business.Services;
using MyBlog.Entities.DTOs;
using Microsoft.Extensions.Logging;

namespace MyBlog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly ILogger<ArticleController> _logger;

        public ArticleController(IArticleService articleService, ILogger<ArticleController> logger)
        {
            _articleService = articleService;
            _logger = logger;
        }

        // GET: api/articles
        [HttpGet]
        public IActionResult GetAllArticles()
        {
            _logger.LogInformation("ArticleController.GetAll method called");
            var articles = _articleService.GetAll();
            return Ok(articles);
        }

        // GET: api/articles/5
        [HttpGet("{id}")]
        public IActionResult GetArticleById(int id)
        {
            var article = _articleService.GetById(id);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

        // POST: api/articles
        [HttpPost]
        public IActionResult AddArticle([FromBody] ArticleDTO articleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _articleService.Add(articleDto);
            return Ok();
        }

        // PUT: api/articles/5
        [HttpPut("{id}")]
        public IActionResult UpdateArticle(int id, [FromBody] ArticleDTO articleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = _articleService.GetById(id);
            if (article == null)
            {
                return NotFound();
            }

            _articleService.Update(articleDto);
            return Ok();
        }

        // DELETE: api/articles/5
        [HttpDelete("{id}")]
        public IActionResult DeleteArticle(int id)
        {
            var article = _articleService.GetById(id);
            if (article == null)
            {
                return NotFound();
            }

            _articleService.Delete(id);
            return Ok();
        }

        // GET: api/articles/search
        [HttpGet("search")]
        public IActionResult SearchArticles(string title, string titleSummary, int? categoryId)
        {
            var articles = _articleService.Search(title, titleSummary, categoryId);
            return Ok(articles);
        }
    }
}
