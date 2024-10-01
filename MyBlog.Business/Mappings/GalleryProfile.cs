using AutoMapper;
using MyBlog.Entities.DTOs;
using MyBlog.Entities.Entities;

namespace MyBlog.Business.Mappings
{
    public class GalleryProfile : Profile
    {
        public GalleryProfile()
        {
            CreateMap<Gallery, GalleryDTO>().ReverseMap();
        }
    }
}
