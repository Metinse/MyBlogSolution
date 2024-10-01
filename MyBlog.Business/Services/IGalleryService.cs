using MyBlog.Entities.DTOs;

namespace MyBlog.Business.Services
{
    public interface IGalleryService
    {
        List<GalleryDTO> GetAll();
        GalleryDTO GetById(int id);
        GalleryDTO Add(GalleryDTO galleryDto);
        GalleryDTO Update(GalleryDTO galleryDto);
        void Delete(int id);
        List<GalleryDTO> Search(string title, string type, string description);
    }
}
