using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Domain.Profiles
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<Author, AuthorDto>();
            CreateMap<AuthorDto, Author>();
        }
    }
}
