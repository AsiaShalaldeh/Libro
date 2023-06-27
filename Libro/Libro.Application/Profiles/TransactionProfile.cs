using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Models;

namespace Libro.Application.Profiles
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Reservation, ReservationDto>()
            .ForMember(dest => dest.ReservationDate, opt => opt.MapFrom(src => src.ReservationDate.ToString("yyyy-MM-dd")));

            CreateMap<Checkout, TransactionResponseModel>();
        }
    }
}
