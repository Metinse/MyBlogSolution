using AutoMapper;
using MyBlog.DataAccess.Repositories;
using MyBlog.Entities.DTOs;
using MyBlog.Entities.Entities;

namespace MyBlog.Business.Services
{
    public class GalleryService : IGalleryService
    {
        private readonly IGalleryRepository _galleryRepository;
        private readonly IMapper _mapper;

        public GalleryService(IGalleryRepository galleryRepository, IMapper mapper)
        {
            _galleryRepository = galleryRepository;
            _mapper = mapper;
        }

        public List<GalleryDTO> GetAll()
        {
            var galleries = _galleryRepository.GetAll();
            return _mapper.Map<List<GalleryDTO>>(galleries);
        }

        public GalleryDTO GetById(int id)
        {
            var gallery = _galleryRepository.GetById(id);
            return _mapper.Map<GalleryDTO>(gallery);
        }

        public GalleryDTO Add(GalleryDTO galleryDto)
        {
            var gallery = _mapper.Map<Gallery>(galleryDto);
            gallery = _galleryRepository.Add(gallery);
            return _mapper.Map<GalleryDTO>(gallery);
        }

        public GalleryDTO Update(GalleryDTO galleryDto)
        {
            var gallery = _mapper.Map<Gallery>(galleryDto);
            gallery = _galleryRepository.Update(gallery);
            return _mapper.Map<GalleryDTO>(gallery);
        }

        public void Delete(int id)
        {
            _galleryRepository.Delete(id);
        }

        public List<GalleryDTO> Search(string? title, string? type, string? description)
        {
            var galleries = _galleryRepository.Search(title, type, description);
            return _mapper.Map<List<GalleryDTO>>(galleries);
        }
    }
}
