using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Domain.Profiles
{
    public class ReviewProfile: Profile
    {
        public ReviewProfile()
        {
            CreateMap<ReviewDto, Review>();
        }
    }
}
