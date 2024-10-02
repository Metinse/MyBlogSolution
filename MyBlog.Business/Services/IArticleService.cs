using MyBlog.Entities.DTOs;

namespace MyBlog.Business.Services
{
    public interface IArticleService
    {
        List<ArticleDTO> GetAll();
        ArticleDTO GetById(int id);
        void Add(ArticleDTO articleDto);
        ArticleDTO Update(ArticleDTO articleDto);
        void Delete(int id);
    }
}