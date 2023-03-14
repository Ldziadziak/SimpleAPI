using AutoMapper;
using SimpleAPI.DTO;
using SimpleAPI.Models;

namespace SimpleAPI.Utils;
public class CustomerMapper : Profile
{
    public CustomerMapper()
    {
        CreateMap<CustomerDto, CustomerModel>().ReverseMap();
        CreateMap<Entities.Customer, CustomerModel>().ReverseMap();
        CreateMap<CustomerDto, Entities.Customer>().ReverseMap();
    }
}
