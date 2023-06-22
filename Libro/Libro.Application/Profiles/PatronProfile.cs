using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Application.Profiles
{
    public class PatronProfile : Profile
    {
        public PatronProfile()
        {
            CreateMap<PatronDto, Patron>();
        }
    }
}
