using Dapper;
using MyBlog.Entities.Entities;
using System.Data;

namespace MyBlog.DataAccess.Repositories
{
    public class GalleryRepository : IGalleryRepository
    {
        private readonly IDbConnection _dbConnection;

        public GalleryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public List<Gallery> GetAll()
        {
            var sql = "SELECT * From Galleries";
            return _dbConnection.Query<Gallery>(sql).ToList();
        }

        public Gallery GetById(int id)
        {
            var sql = "SELECT * From Galleries WHERE Id = @Id";
            return _dbConnection.QueryFirstOrDefault<Gallery>(sql, new { Id = id });
        }

        public Gallery Add(Gallery gallery)
        {
            var sql = @"INSERT INTO Galleries (Title, ArticleID, FileUrl, Type, Description, AddDate, UpdateDate, Status)
                        VALUES (@Title, @ArticleID, @FileUrl, @Type, @Description, @AddDate, @UpdateDate, @Status);
                        SELECT CAST(SCOPE_IDENTITY() as int);";
            var id = _dbConnection.Query<int>(sql, gallery).Single();
            gallery.Id = id;
            return gallery;
        }

        public Gallery Update(Gallery gallery)
        {
            var sql = @"UPDATE Galleries 
                        SET Title = @Title, ArticleID = @ArticleID, FileUrl = @FileUrl, Type = @Type, 
                            Description = @Description, AddDate = @AddDate, UpdateDate = @UpdateDate, Status = @Status 
                        WHERE Id = @Id";
            _dbConnection.Execute(sql, gallery);
            return gallery;
        }

        public void Delete(int id)
        {
            var sql = "DELETE From Galleries WHERE Id = @Id";
            _dbConnection.Execute(sql, new { Id = id });
        }

        public List<Gallery> Search(string title, string type, string description)
        {
            var sql = @"SELECT * From Galleries 
                        WHERE (@Title IS NULL OR Title LIKE '%' + @Title + '%') 
                        AND (@Type IS NULL OR Type LIKE '%' + @Type + '%') 
                        AND (@Description IS NULL OR Description LIKE '%' + @Description + '%')";
            return _dbConnection.Query<Gallery>(sql, new { Title = title, Type = type, Description = description }).ToList();
        }
    }
}
