using AutoMapper;
using MyBlog.Entities;
using MyBlog.Entities.DTOs;


namespace MyBlog.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Entity => DTO dönüşümü
            //CreateMap<User, UserDto>();       

            // Login için DTO dönüşümü
            CreateMap<LoginDto, User>();

            CreateMap<Article, ArticleDTO>().ReverseMap();
        }
    }
}

