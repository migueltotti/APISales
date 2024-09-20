using Sales.Application.DTOs.UserDTO;
using Sales.Application.Parameters.ModelsParameters;

namespace Sales.Application.Strategy.FilterImplementation.UserStrategy;

public class UserNameFilter : IUserFilterStrategy
{
    public IEnumerable<UserDTOOutput> ApplyFilter(IEnumerable<UserDTOOutput> users, UserParameters parameters)
    {
        if (parameters.Name is not null)
        {
            users = users.Where(u => u.Name.Contains(parameters.Name,
                    StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(u => u.Name);
        }
        
        return users;
    }
    public string GetFilter()
    {
        return "name";
    }
}