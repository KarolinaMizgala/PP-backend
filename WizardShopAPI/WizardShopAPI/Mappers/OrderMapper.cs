using AutoMapper;
using WizardShopAPI.DTOs;
using WizardShopAPI.Models;

namespace WizardShopAPI.Mappers
{
    public class OrderMapper : Profile
    {
        public OrderMapper()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(m=>m.Id, c=>c.MapFrom(s=>s.OrderId))
                .ForMember(m => m.FirstName, c => c.MapFrom(s => s.OrderDetails.FirstName))
                .ForMember(m => m.LastName, c => c.MapFrom(s => s.OrderDetails.LastName))
                .ForMember(m => m.PhoneNumber, c => c.MapFrom(s => s.OrderDetails.PhoneNumber))
                .ForMember(m => m.Email, c => c.MapFrom(s => s.OrderDetails.Email))
                .ForMember(m => m.Comment, c => c.MapFrom(s => s.OrderDetails.Comment));

           CreateMap<Address, AddressDto>();
            CreateMap<Payment, PaymentDto>();
            CreateMap<PaymentDto, Payment>();
            CreateMap<OrderDetailsDto, OrderDetails>()
                .ForMember(r => r.Address,
                c => c.MapFrom(dto => new Address()
                { City = dto.City, ZipCode = dto.ZipCode, Street = dto.Street, HouseNumber = dto.HouseNumber, ApartmentNumber = dto.ApartmentNumber }));
            ;
            CreateMap<OrderDetailsDto, OrderDto>();
            CreateMap<OrderDto, OrderDetailsDto>();
            CreateMap<OrderDto, Order>()
                .ForMember(r => r.OrderDetails,
                c => c.MapFrom(dto => new OrderDetails()
                { FirstName = dto.FirstName, LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber, 
                Email = dto.Email,
                Comment = dto.Comment}));
            CreateMap<CartItem, OrderItem>();
        }
    }
}
