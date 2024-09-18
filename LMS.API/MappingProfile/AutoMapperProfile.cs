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
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.Course, opt => opt.Ignore()).ReverseMap();

            CreateMap<Course, CourseDto>()
           .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users))
           .ForMember(dest => dest.Module, opt => opt.MapFrom(src => src.Modules));

            CreateMap<Module,  ModuleDto>()
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId)).ReverseMap();

            CreateMap<Activity, ActivityDto>()
                .ForMember(dest => dest.ModuleId, opt => opt.MapFrom(src => src.ModuleId)).ReverseMap();

            CreateMap<ActivityType, ActivityTypeDto>().ReverseMap();
        }
    }
}
