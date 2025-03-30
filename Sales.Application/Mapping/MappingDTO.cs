using AutoMapper;
using Sales.Application.DTOs.AffiliateDTO;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.DTOs.LineItemDTO;
using Sales.Application.DTOs.UserDTO;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.DTOs.ShoppingCartDTO;
using Sales.Application.DTOs.WorkDayDTO;
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
        
        CreateMap<LineItem, LineItemDTOOutput>().ReverseMap();
        
        CreateMap<User, UserDTOOutput>().ReverseMap();
        CreateMap<User, UserDTOInput>().ReverseMap();
        
        CreateMap<Affiliate, AffiliateDTOOutput>().ReverseMap();
        CreateMap<Affiliate, AffiliateDTOInput>().ReverseMap();
        
        CreateMap<WorkDay, WorkDayDTOOutput>().ReverseMap();
        CreateMap<WorkDay, WorkDayDTOInput>().ReverseMap();
        
        //CreateMap<ProductShoppingCart, ProductShoppingCartDTOOutput>().ReverseMap();
        //CreateMap<ShoppingCart, ShoppingCartDTOOutput>().ReverseMap();
    }
}