using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Application.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDto>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
            .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre.ToString()))
            .ForMember(dest => dest.PublicationDate, opt =>
            {
                opt.Condition(src => src.PublicationDate.HasValue);
                opt.MapFrom(src => src.PublicationDate.Value.ToString("yyyy-MM-dd"));
            });


            CreateMap<BookRequest, Book>();
        }
    }
}
