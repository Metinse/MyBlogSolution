using Dapper;
using MyBlog.Entities;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MyBlog.DataAccess.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IDbConnection _dbConnection;

        public ArticleRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // CRUD ve Search metotlarını burada uyguluyoruz.
        public IEnumerable<Article> GetAllArticles()
        {
            return _dbConnection.Query<Article>("SELECT * FROM [Article]").ToList();
        }

        public Article GetArticleById(int id)
        {
            return _dbConnection.Query<Article>("SELECT * FROM [Article] WHERE Id = @Id", new { Id = id }).FirstOrDefault();
        }

        public void AddArticle(Article article)
        {
            var sql = @"INSERT INTO [Article] 
                        (Title, TitleSummary, MetaDescription, Content, CategoryID, Author, SmallimageUrl, MiddleimageUrl, BigimageUrl, SeoLink, Tag, Related, Showcase, AddDate, UpdateDate, Status)
                        VALUES
                        (@Title, @TitleSummary, @MetaDescription, @Content, @CategoryID, @Author, @SmallimageUrl, @MiddleimageUrl, @BigimageUrl, @SeoLink, @Tag, @Related, @Showcase, @AddDate, @UpdateDate, @Status)";
            _dbConnection.Execute(sql, article);
        }

        public void UpdateArticle(Article article)
        {
            var sql = @"UPDATE [Article] SET 
                        Title = @Title,
                        TitleSummary = @TitleSummary,
                        MetaDescription = @MetaDescription,
                        Content = @Content,
                        CategoryID = @CategoryID,
                        Author = @Author,
                        SmallimageUrl = @SmallimageUrl,
                        MiddleimageUrl = @MiddleimageUrl,
                        BigimageUrl = @BigimageUrl,
                        SeoLink = @SeoLink,
                        Tag = @Tag,
                        Related = @Related,
                        Showcase = @Showcase,
                        AddDate = @AddDate,
                        UpdateDate = @UpdateDate,
                        Status = @Status
                        WHERE Id = @Id";
            _dbConnection.Execute(sql, article);
        }

        public void DeleteArticle(int id)
        {
            var sql = "DELETE FROM [Article] WHERE Id = @Id";
            _dbConnection.Execute(sql, new { Id = id });
        }

        public IEnumerable<Article> SearchArticles(string title, string titleSummary, int? categoryId)
        {
            var sql = @"SELECT * FROM [Article] WHERE 
                        (@Title IS NULL OR Title LIKE '%' + @Title + '%') AND
                        (@TitleSummary IS NULL OR TitleSummary LIKE '%' + @TitleSummary + '%') AND
                        (@CategoryID IS NULL OR CategoryID = @CategoryID)";
            return _dbConnection.Query<Article>(sql, new { Title = title, TitleSummary = titleSummary, CategoryID = categoryId }).ToList();
        }
    }
}
