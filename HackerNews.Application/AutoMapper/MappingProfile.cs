using AutoMapper;
using HackerNews.Infrastructure.Dtos;
using HackerNews.Infrastructure.ExternalDtos;

namespace HackerNews.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<ExternalStoryDto, StoryDto>()
             .ForMember(d => d.Title, s => s.MapFrom(x => x.Title))
             .ForMember(d => d.Url, s => s.MapFrom(x => x.Url));
        }

      
    }
}
