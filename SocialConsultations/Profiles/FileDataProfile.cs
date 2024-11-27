using AutoMapper;
using SocialConsultations.Models;

namespace SocialConsultations.Profiles
{
    public class FileDataProfile : Profile
    {
        public FileDataProfile() 
        {
            CreateMap<Entities.FileData, Models.FileDataDto>();
            CreateMap<Models.FileDataForCreationDto, Entities.FileData>();
            CreateMap<FileDataDto, Entities.FileData>();
            CreateMap<Entities.FileData, FileDataForCreationDto>();
        }
    }
}
