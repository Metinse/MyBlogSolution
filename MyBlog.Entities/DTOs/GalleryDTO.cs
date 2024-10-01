namespace MyBlog.Entities.DTOs
{
    public class GalleryDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? ArticleID { get; set; }
        public string FileUrl { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool Status { get; set; }
    }
}
