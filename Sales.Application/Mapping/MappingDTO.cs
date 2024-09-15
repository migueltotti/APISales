using AutoMapper;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.DTOs.UserDTO;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Domain.Models;

namespace Sales.Application.Mapping;

public class MappingDTO : Profile
{
    public MappingDTO()
    {
        CreateMap<Category, CategoryDTOOutput>().ReverseMap();
        CreateMap<Category, CategoryDTOInput>().ReverseMap();
        
        CreateMap<Product, ProductDTOOutput>().ReverseMap();
        CreateMap<Product, ProductDTOInput>().ReverseMap();
        
        CreateMap<Order, OrderDTOOutput>().ReverseMap();
        CreateMap<Order, OrderDTOInput>().ReverseMap();
        CreateMap<Order, OrderProductDTO>().ReverseMap();
        
        CreateMap<User, UserDTOOutput>().ReverseMap();
        CreateMap<User, UserDTOInput>().ReverseMap();
    }
}