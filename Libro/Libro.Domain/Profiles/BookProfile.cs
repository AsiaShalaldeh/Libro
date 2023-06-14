using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Domain.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDto>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
            .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre.ToString()));

            CreateMap<RequestBookDto, Book>();
        }
    }
}
