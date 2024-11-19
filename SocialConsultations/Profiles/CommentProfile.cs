using AutoMapper;

namespace SocialConsultations.Profiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Entities.Comment, Models.CommentDto>();
            CreateMap<Models.CommentDto, Entities.Comment>();
            CreateMap<Models.CommentForCreationDto, Entities.Comment>();
            CreateMap<Models.CommentForUpdateDto, Entities.Comment>();
            CreateMap<Entities.Comment, Models.CommentFullDto>();
        }
    }
}
