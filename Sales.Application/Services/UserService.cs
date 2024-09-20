using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using Sales.Application.DTOs.UserDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.Parameters.ModelsParameters.UserParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Domain.Models.Enums;
using X.PagedList;
using X.PagedList.Extensions;

namespace Sales.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uof;
    private readonly IValidator<UserDTOInput> _validator;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork uof, IValidator<UserDTOInput> validator, IMapper mapper)
    {
        _uof = uof;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDTOOutput>> GetAllUsers()
    {
        var users = await _uof.UserRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<UserDTOOutput>>(users);
    }

    public async Task<IPagedList<UserDTOOutput>> GetAllUsers(QueryStringParameters parameters)
    {
        var users = (await GetAllUsers()).OrderBy(u => u.Name);
        
        return users.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<IPagedList<UserDTOOutput>> GetUsersByRole(UserParameters parameters)
    {
        var users = await GetAllUsers();

        /*if (parameters.Role.ToLower() is not null)
        {
            users = parameters.Role switch
            {
                "admin" => users.Where(u => u.Role == Role.Admin).OrderBy(u => u.Name),
                "employee" => users.Where(u => u.Role == Role.Employee).OrderBy(u => u.Name),
                "customer" => users.Where(u => u.Role == Role.Customer).OrderBy(u => u.Name),
                _ => users.OrderBy(u => u.Name)
            };
        }*/
        
        return users.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<IPagedList<UserDTOOutput>> GetUsersByName(UserParameters parameters)
    {
        var users = await GetAllUsers();

        /*if (parameters.Name is not null)
        {
            users = users.Where(u => u.Name.Contains(parameters.Name,
                StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(u => u.Name);
        }*/
        
        return users.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<Result<UserDTOOutput>> GetUsersByCpf(UserParameters parameters)
    {
        var user = await _uof.UserRepository.GetAsync(u => u.Cpf == parameters.Cpf);

        if (user is null)
        {
            return Result<UserDTOOutput>.Failure(UserErrors.CpfNotFound);
        }
        
        return Result<UserDTOOutput>.Success(_mapper.Map<UserDTOOutput>(user));
    }

    public async Task<IPagedList<UserDTOOutput>> GetUsersByPoints(UserParameters parameters)
    {
        var users = await GetAllUsers();

        if (parameters is { Points: not null, PointsCriteria: not null })
        {
            users = parameters.PointsCriteria.ToLower() switch
            {
                //"greater" => users.Where(u => u.Points > parameters.Points).OrderBy(u => u.Name),
                //"equal" => users.Where(u => u.Points == parameters.Points).OrderBy(u => u.Name),
                //"less" => users.Where(u => u.Points < parameters.Points).OrderBy(u => u.Name),
                _ => users.OrderBy(u => u.Name)
            };
        }
        
        return users.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<IPagedList<UserDTOOutput>> GetUsersByAffiliation(UserParameters parameters)
    {
        var users = await GetAllUsers();

        if (parameters.Affiliation is not null)
        {
            //users = users.Where(u => u.Affiliation == parameters.Affiliation).OrderBy(u => u.Name);
        }
        
        return users.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<IPagedList<UserDTOOutput>> GetUsersByOrdersNotCompleted(UserParameters parameters)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<UserDTOOutput>> GetUserBy(Expression<Func<User, bool>> expression)
    {
        var user = await _uof.UserRepository.GetAsync(expression);

        if (user is null)
        {
            return Result<UserDTOOutput>.Failure(UserErrors.NotFound);
        }
        
        var userDto = _mapper.Map<UserDTOOutput>(user);

        return Result<UserDTOOutput>.Success(userDto);
    }

    public async Task<Result<UserDTOOutput>> CreateUser(UserDTOInput userDtoInput)
    {
        if (userDtoInput is null)
        {
            return Result<UserDTOOutput>.Failure(UserErrors.DataIsNull);
        }
        
        var validation = await _validator.ValidateAsync(userDtoInput);

        if (!validation.IsValid)
        {
            return Result<UserDTOOutput>.Failure(UserErrors.IncorrectFormatData, validation.Errors);
        }

        var user = _mapper.Map<User>(userDtoInput);

        var userCreated = _uof.UserRepository.Create(user);
        await _uof.CommitChanges();

        var userDtoCreated = _mapper.Map<UserDTOOutput>(userCreated);

        return Result<UserDTOOutput>.Success(userDtoCreated);
    }

    public async Task<Result<UserDTOOutput>> UpdateUser(UserDTOInput userDtoInput, int id)
    {
        if (userDtoInput is null)
        {
            return Result<UserDTOOutput>.Failure(UserErrors.DataIsNull);
        }

        if (userDtoInput.UserId != id)
        {
            return Result<UserDTOOutput>.Failure(UserErrors.IdMismatch);
        }
        
        var user = await _uof.UserRepository.GetAsync(x => x.UserId == id);

        if (user is null)
        {
            return Result<UserDTOOutput>.Failure(UserErrors.NotFound);
        }
        
        var validation = await _validator.ValidateAsync(userDtoInput);

        if (!validation.IsValid)
        {
            return Result<UserDTOOutput>.Failure(UserErrors.IncorrectFormatData, validation.Errors);
        }
        
        var userForUpdate = _mapper.Map<User>(userDtoInput);

        var userUpdate = _uof.UserRepository.Update(userForUpdate);
        await _uof.CommitChanges();

        var userDtoUpdated = _mapper.Map<UserDTOOutput>(userUpdate);
        
        return Result<UserDTOOutput>.Success(userDtoUpdated);
    }

    public async Task<Result<UserDTOOutput>> DeleteUser(int? id)
    {
        var user = await _uof.UserRepository.GetAsync(e => e.UserId == id);

        if (user is null)
        {
            return Result<UserDTOOutput>.Failure(UserErrors.NotFound);
        }

        var userDeleted = _uof.UserRepository.Delete(user);
        await _uof.CommitChanges();

        var userDtoDeleted = _mapper.Map<UserDTOOutput>(userDeleted);

        return Result<UserDTOOutput>.Success(userDtoDeleted);
    }
}