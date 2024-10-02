using AutoMapper;
using MyBlog.DataAccess.Repositories;
using MyBlog.Entities;
using MyBlog.Entities.DTOs;

namespace MyBlog.Business.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IMapper _mapper;

        public ArticleService(IArticleRepository articleRepository, IMapper mapper)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
        }

        public List<ArticleDTO> GetAll()
        {
            var articles = _articleRepository.GetAllArticles();
            return _mapper.Map<List<ArticleDTO>>(articles);
        }

        public ArticleDTO GetById(int id)
        {
            var article = _articleRepository.GetArticleById(id);
            return _mapper.Map<ArticleDTO>(article);
        }

        public void Add(ArticleDTO articleDto)
        {
            var article = _mapper.Map<Article>(articleDto);
            _articleRepository.AddArticle(article);
        }

        public ArticleDTO Update(ArticleDTO articleDto)
        {
            var article = _mapper.Map<Article>(articleDto);
            _articleRepository.UpdateArticle(article);
            return _mapper.Map<ArticleDTO>(article);
        }

        public void Delete(int id)
        {
            _articleRepository.DeleteArticle(id);
        }

        public List<ArticleDTO> Search(string? title, string? titleSummary, int? categoryId)
        {
            var articles = _articleRepository.SearchArticles(title, titleSummary, categoryId);
            return _mapper.Map<List<ArticleDTO>>(articles);
        }
    }
}
