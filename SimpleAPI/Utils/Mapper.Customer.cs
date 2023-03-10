using AutoMapper;
using SimpleAPI.DTO;
using SimpleAPI.Models;

namespace SimpleAPI.Utils;
public class CustomerMapper : Profile
{
    public CustomerMapper()
    {
        CreateMap<CustomerDto, Customer>().ReverseMap();
       // CreateMap<Entities.Customer, Customer>().ReverseMap();
      //  CreateMap<CustomerDto, Customer>().ReverseMap();
    }
}
