using ApiSales.DTOs.CategoryDTO;
using ApiSales.DTOs.EmployeeDTO;
using ApiSales.DTOs.OrderDTO;
using ApiSales.DTOs.ProductDTO;
using ApiSales.Models;
using AutoMapper;

namespace ApiSales.DTOs;

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