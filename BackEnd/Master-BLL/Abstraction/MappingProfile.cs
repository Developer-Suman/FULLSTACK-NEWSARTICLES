using AutoMapper;
using Master_BLL.DTOs.Articles;
using Master_BLL.DTOs.Authentication;
using Master_BLL.DTOs.Comment;
using Master_BLL.DTOs.RegistrationDTOs;
using Master_DAL.Models;

namespace MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Configs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegistrationCreateDTOs, ApplicationUser>().ReverseMap();
            CreateMap<UserDTOs, ApplicationUser>().ReverseMap();
            CreateMap<ArticlesGetDTOs, Articles>().ReverseMap();
            CreateMap<Articles, ArticlesCreateDTOs>().ReverseMap();
            CreateMap<Articles, ArticlesUpdateDTOs>().ReverseMap();


            CreateMap<CommentsGetDTOs, Comments>().ReverseMap();
            CreateMap<CommentsWithArticles, Comments>().ReverseMap()
                .ForMember(dest => dest.CommentsId, opt => opt.MapFrom(src => src.CommentsId))
                .ForMember(dest => dest.CommentDescription, opt => opt.MapFrom(src => src.CommentDescription))
                .ForMember(dest => dest.ArticleName, opt => opt.MapFrom(src => src.Articles.ArticlesTitle));
              

            
        }
    }
}
