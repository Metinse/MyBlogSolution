namespace MyBlog.Entities.Entities
{
    public class Gallery : BaseEntity
    {
        public string Title { get; set; }
        public int? ArticleID { get; set; }
        public string FileUrl { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
}
