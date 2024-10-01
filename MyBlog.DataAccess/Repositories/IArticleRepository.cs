using MyBlog.Entities;
using System.Collections.Generic;

namespace MyBlog.DataAccess.Repositories
{
    public interface IArticleRepository
    {
        IEnumerable<Article> GetAllArticles();
        Article GetArticleById(int id);
        void AddArticle(Article article);
        void UpdateArticle(Article article);
        void DeleteArticle(int id);
        IEnumerable<Article> SearchArticles(string title, string titleSummary, int? categoryId);
    }
}

