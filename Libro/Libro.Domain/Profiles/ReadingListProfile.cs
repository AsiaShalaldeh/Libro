using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Domain.Profiles
{
    public class ReadingListProfile : Profile
    {
        public ReadingListProfile()
        {
            CreateMap<ReadingListDto, ReadingList>();
            CreateMap<ReadingList, ReadingListDto>();
        }
    }
}
