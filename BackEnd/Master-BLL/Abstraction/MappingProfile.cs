using AutoMapper;
using Master_BLL.DTOs.Articles;
using Master_BLL.DTOs.Authentication;
using Master_BLL.DTOs.Comment;
using Master_BLL.DTOs.Likes;
using Master_BLL.DTOs.Pagination;
using Master_BLL.DTOs.Permission;
using Master_BLL.DTOs.RegistrationDTOs;
using Master_DAL.Models;
using Microsoft.AspNetCore.Identity;

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



            CreateMap<UserPermission, AssignPermissionGetDTOs>().ReverseMap();


            CreateMap<CommentsGetDTOs, Comments>().ReverseMap();
            CreateMap<CommentsCreateDTOs, Comments>().ReverseMap();
            CreateMap<CommentsWithArticlesDTOs, Comments>().ReverseMap()
                .ForMember(dest => dest.ArticlesId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.ArticlesName, opt => opt.MapFrom(src => src.Articles.Title));



            CreateMap<ControllerActionUserGetDTOs, ControllerAction>().ReverseMap();



            #region Likes and LikesArticlesGetDTOs
            CreateMap<Likes, LikesArticlesGetDTOs>().ReverseMap();
            #endregion




            #region Users Mapping and RoleMapping
            CreateMap<PagedResult<ApplicationUser>, PagedResult<UserDTOs>>().ReverseMap();
            CreateMap<PagedResult<IdentityRole>, PagedResult<RoleDTOs>>().ReverseMap();
            #endregion

            #region Articles Pagination
            CreateMap<PagedResult<Articles>, PagedResult<ArticlesWithCommentsDTOs>>().ReverseMap();
            CreateMap<PagedResult<Articles>, PagedResult<CommentsGetDTOs>>().ReverseMap();
            CreateMap<PagedResult<Articles>, PagedResult<ArticlesGetDTOs>>().ReverseMap();
            #endregion






        }
    }
}
