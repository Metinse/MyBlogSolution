using Microsoft.AspNetCore.Mvc;
using MyBlog.DataAccess.Repositories;
using MyBlog.Entities;
using MyBlog.Entities.DTOs;
using AutoMapper;

namespace MyBlog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ArticleController> _logger;

        public ArticleController(IArticleRepository articleRepository, IMapper mapper, ILogger<ArticleController> logger)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
            _logger = logger;
        }
        // GET: api/articles
        [HttpGet]
        public IActionResult GetAllArticles()
        {
            _logger.LogInformation("ArticleController.GetAll method called");
            var articles = _articleRepository.GetAllArticles();
            var articleDtos = _mapper.Map<IEnumerable<ArticleDTO>>(articles);
            return Ok(articleDtos);
        }

        // GET: api/articles/5
        [HttpGet("{id}")]
        public IActionResult GetArticleById(int id)
        {
            var article = _articleRepository.GetArticleById(id);
            if (article == null)
            {
                return NotFound();
            }

            var articleDto = _mapper.Map<ArticleDTO>(article);
            return Ok(articleDto);
        }

        // POST: api/articles
        [HttpPost]
        public IActionResult AddArticle([FromBody] ArticleDTO articleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = _mapper.Map<Article>(articleDto);
            _articleRepository.AddArticle(article);
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

            var article = _articleRepository.GetArticleById(id);
            if (article == null)
            {
                return NotFound();
            }

            _mapper.Map(articleDto, article);
            _articleRepository.UpdateArticle(article);
            return Ok();
        }

        // DELETE: api/articles/5
        [HttpDelete("{id}")]
        public IActionResult DeleteArticle(int id)
        {
            var article = _articleRepository.GetArticleById(id);
            if (article == null)
            {
                return NotFound();
            }

            _articleRepository.DeleteArticle(id);
            return Ok();
        }

        // GET: api/articles/search
        [HttpGet("search")]
        public IActionResult SearchArticles(string title, string titleSummary, int? categoryId)
        {
            var articles = _articleRepository.SearchArticles(title, titleSummary, categoryId);
            var articleDtos = _mapper.Map<IEnumerable<ArticleDTO>>(articles);
            return Ok(articleDtos);
        }
    }
}

