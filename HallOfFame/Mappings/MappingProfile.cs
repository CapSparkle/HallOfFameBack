using AutoMapper;
using HallOfFame.Entities;
using HallOfFame.Models;

namespace HallOfFame.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Person, PersonEntity>()
               .ForMember(dest => dest.PersonSkills, opt => opt.MapFrom(src =>
                   src.Skills.Select(s => new PersonSkill { Skill = new SkillEntity { Name = s.Name, Level = s.Level } })));

            CreateMap<PersonEntity, Person>()
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src =>
                    src.PersonSkills.Select(ps => ps.Skill).ToList()));

            CreateMap<Skill, SkillEntity>().ReverseMap();
        }
    }
}
