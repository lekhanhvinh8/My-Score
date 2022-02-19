using AutoMapper;
using MyScore.Core.Domain;
using MyScore.Core.Dtos;

namespace MyScore.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Mapper.CreateMap<Course, CourseDto>()
                .ForMember(cd => cd.UserName, opt => opt.MapFrom(c => c.User.UserName));
            Mapper.CreateMap<CourseDto, Course>();
        }
    }
}