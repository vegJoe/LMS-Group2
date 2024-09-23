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
                .ReverseMap();

            CreateMap<Course, CourseDto>().ReverseMap();

            CreateMap<Module, ModuleDto>().ReverseMap();

            CreateMap<Activity, ActivityDto>().ReverseMap();

            CreateMap<ActivityType, ActivityTypeDto>().ReverseMap();

            CreateMap<Module, CreateUpdateModuleDto>().ReverseMap();

            CreateMap<Activity, CreateUpdateActivityDto>().ReverseMap();
        }
    }
}
