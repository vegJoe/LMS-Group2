using AutoMapper;
using LMS.API.Models.Dtos;
using LMS.API.Models.Entities;


namespace LMS.API.MappingProfile
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course))
                .ReverseMap();

            CreateMap<Course, CourseDto>()
           .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users))
           .ForMember(dest => dest.Module, opt => opt.MapFrom(src => src.Modules)).ReverseMap();

            CreateMap<Module, ModuleDto>()
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
                //.ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course))
                .ForMember(dest => dest.Activity, opt => opt.MapFrom(src => src.Activites))
                .ReverseMap();

            CreateMap<Activity, ActivityDto>()
                .ForMember(dest => dest.ModuleId, opt => opt.MapFrom(src => src.ModuleId))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ReverseMap();

            CreateMap<ActivityType, ActivityTypeDto>().ReverseMap();

            CreateMap<Module, CreateUpdateModuleDto>().ReverseMap();

            CreateMap<Activity, CreateUpdateActivityDto>().ReverseMap();
        }
    }
}
