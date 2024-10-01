namespace MyBlog.Entities.DTOs
{
    public class ArticleDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TitleSummary { get; set; }
        public string MetaDescription { get; set; }
        public string Content { get; set; }
        public int CategoryID { get; set; }
        public string Author { get; set; }
        public string SmallimageUrl { get; set; }
        public string MiddleimageUrl { get; set; }
        public string BigimageUrl { get; set; }
        public string SeoLink { get; set; }
        public string Tag { get; set; }
        public string Related { get; set; }
        public bool? Showcase { get; set; }
    }
}
