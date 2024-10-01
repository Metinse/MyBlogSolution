using MyBlog.Entities.Entities;

namespace MyBlog.DataAccess.Repositories
{
    public interface IGalleryRepository
    {
        List<Gallery> GetAll();
        Gallery GetById(int id);
        Gallery Add(Gallery gallery);
        Gallery Update(Gallery gallery);
        void Delete(int id);
        List<Gallery> Search(string title, string type, string description);
    }
}
