namespace Sales.Application.DTOs.CategoryDTO;    

public record CategoryDTOInput(
    int CategoryId,
    string Name,
    string ImageUrl
);