using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Application.Profiles
{
    public class ReviewProfile: Profile
    {
        public ReviewProfile()
        {
            CreateMap<ReviewDto, Review>();

            CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title));
            //.ForMember(dest => dest.PatronName, opt => opt.MapFrom(src => src.Patron.Name));
        }
    }
}
