using AutoMapper;

namespace SocialConsultations.Profiles
{
    public class FileDataProfile : Profile
    {
        public FileDataProfile() 
        {
            CreateMap<Entities.FileData, Models.FileDataDto>();
            CreateMap<Models.FileDataForCreationDto, Entities.FileData>();
        }
    }
}
