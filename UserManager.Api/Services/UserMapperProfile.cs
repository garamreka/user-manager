using AutoMapper;
using UserManager.Api.DTOs;
using UserManager.Api.Models;

namespace UserManager.Api.Services
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
            CreateMap<UserDto, User>();

            CreateMap<User, NewUserDto>();
            CreateMap<NewUserDto, User>();

            CreateMap<Address, AddressDto>();
            CreateMap<AddressDto, Address>();

            CreateMap<Company, CompanyDto>();
            CreateMap<CompanyDto, Company>();

            CreateMap<Geo, GeoDto>();
            CreateMap<GeoDto, Geo>();
        }
    }
}
