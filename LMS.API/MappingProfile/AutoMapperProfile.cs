using AutoMapper;
using LMS.API.Models.Dtos;
using LMS.API.Models.Entities;


namespace LMS.API.MappingProfile
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        { 
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId)).ReverseMap();

            CreateMap<Course, CourseDto>().ReverseMap();

            CreateMap<Module,  ModuleDto>()
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId)).ReverseMap();

            CreateMap<Activity, ActivityDto>()
                .ForMember(dest => dest.ModuleId, opt => opt.MapFrom(src => src.ModuleId)).ReverseMap();

            CreateMap<ActivityType, ActivityTypeDto>().ReverseMap();
        }
    }
}
