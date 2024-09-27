using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.DTOs.UserDTO;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using X.PagedList;

namespace Sales.Application.Strategy;

public interface IUserFilterStrategy
{
    IEnumerable<UserDTOOutput> ApplyFilter(IEnumerable<UserDTOOutput> users, UserParameters parameters);    
    string GetFilter();
}