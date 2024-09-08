using AutoMapper;
using Sales.API.DTOs.CategoryDTO;
using Sales.API.DTOs.EmployeeDTO;
using Sales.API.DTOs.OrderDTO;
using Sales.API.DTOs.ProductDTO;
using Sales.API.Models;

namespace Sales.API.DTOs;

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
        
        CreateMap<Employee, EmployeeDTOOutput>().ReverseMap();
        CreateMap<Employee, EmployeeDTOInput>().ReverseMap();
    }
}