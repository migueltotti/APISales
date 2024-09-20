using Sales.Application.DTOs.UserDTO;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Domain.Models.Enums;
using X.PagedList;

namespace Sales.Application.Strategy.FilterImplementation.UserStrategy;

public class UserRoleFilter : IUserFilterStrategy   
{
    public IEnumerable<UserDTOOutput> ApplyFilter(IEnumerable<UserDTOOutput> users, UserParameters parameters)  
    {
        if (parameters.Role is not null)
        {
            users = parameters.Role.ToLower() switch
            {
                "admin" => users.Where(u => u.Role == Role.Admin).OrderBy(u => u.Name),
                "employee" => users.Where(u => u.Role == Role.Employee).OrderBy(u => u.Name),
                "customer" => users.Where(u => u.Role == Role.Customer).OrderBy(u => u.Name),
                _ => users.OrderBy(u => u.Name)
            };
        }
        
        return users;
    }

    public string GetFilter()
    {
        return "role";
    }
}