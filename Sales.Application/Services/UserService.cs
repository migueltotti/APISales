using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Sales.Application.DTOs.UserDTO;
using Sales.Application.Interfaces;
using Sales.Application.Mapping.Extentions;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
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
    private readonly IValidator<UserUpdateDTO> _updateValidator;
    private readonly IMapper _mapper;
    private readonly IUserFilterFactory _userFilterFactory;
    private readonly ICacheService _cacheService;
    private readonly IEncryptService _encryptService;

    public UserService(IUnitOfWork uof, IValidator<UserDTOInput> validator, IMapper mapper, IUserFilterFactory userFilterFactory, ICacheService cacheService, IValidator<UserUpdateDTO> updateValidator, IEncryptService encryptService)
    {
        _uof = uof;
        _validator = validator;
        _mapper = mapper;
        _userFilterFactory = userFilterFactory;
        _cacheService = cacheService;
        _updateValidator = updateValidator;
        _encryptService = encryptService;
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

    public async Task<IPagedList<UserDTOOutput>> GetUsersWithFilter(string filter, UserParameters parameters)
    {
        var users = await GetAllUsers();

        users = _userFilterFactory.GetStrategy(filter).ApplyFilter(users, parameters);
        
        return users.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<IPagedList<UserDTOOutput>> GetUsersByPoints(UserParameters parameters)
    {
        var users = await GetAllUsers();

        if (parameters is { Points: not null, PointsCriteria: not null })
        {
            users = parameters.PointsCriteria.ToLower() switch
            {
                "greater" => users.Where(u => u.Points > parameters.Points).OrderBy(u => u.Name),
                "equal" => users.Where(u => u.Points == parameters.Points).OrderBy(u => u.Name),
                "less" => users.Where(u => u.Points < parameters.Points).OrderBy(u => u.Name),
                _ => users.OrderBy(u => u.Name)
            };
        }
        
        return users.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<IPagedList<UserDTOOutput>> GetUsersByAffiliation(UserParameters parameters)
    {
        var users = await GetAllUsers();
        var affiliate = await _uof.AffiliateRepository
                            .GetAsync(a => a.Name.Contains(parameters.Affiliation));
                                        

        if (parameters.Affiliation is not null)
        {
            users = users.Where(u => u.AffiliateId == affiliate.AffiliateId).OrderBy(u => u.Name);
        }
        
        return users.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<IPagedList<UserDTOOutput>> GetUsersByOrdersNotCompleted(UserParameters parameters)
    {
        var orders = await _uof.OrderRepository.GetAllAsync();
        
        HashSet<int> usersIds = new();
        
        orders.Where(o => o.OrderStatus is Status.Preparing or Status.Finished)
            .ToList()
            .ForEach(o => usersIds.Add(o.OrderId));
        
        var users = await GetAllUsers();
        users = users.Where(u => usersIds.Contains(u.UserId));
        
        return users.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }
    
    public async Task<Result<UserDTOOutput>> GetUserById(int id)
    {
        var user = await _uof.UserRepository.GetByIdAsync(id);

        if (user is null)
        {
            return Result<UserDTOOutput>.Failure(UserErrors.NotFound);
        }
        
        var userDto = _mapper.Map<UserDTOOutput>(user);

        return Result<UserDTOOutput>.Success(userDto);
    }
    
    public async Task<Result<UserDTOOutput>> GetUserByEmail(string email)
    {
        var user = await _uof.UserRepository.GetByEmailAsync(email);

        if (user is null)
        {
            return Result<UserDTOOutput>.Failure(UserErrors.NotFound);
        }
        
        var userDto = _mapper.Map<UserDTOOutput>(user);

        return Result<UserDTOOutput>.Success(userDto);
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

        var userInput = userDtoInput with { Password = _encryptService.Decrypt(userDtoInput.Password) };
        
        var validation = await _validator.ValidateAsync(userInput);

        if (!validation.IsValid)
        {
            return Result<UserDTOOutput>.Failure(UserErrors.IncorrectFormatData, validation.Errors);
        }
        
        var userExists = await GetUserBy(u => u.Email == userInput.Email);

        if (userExists.value is not null)
        {
            return Result<UserDTOOutput>.Failure(UserErrors.UserExists);
        }

        var passwordEncrypted = _encryptService.Encrypt(userInput.Password);

        var user = new User(
            userInput.UserId,
            userInput.Name,
            userInput.Email,
            passwordEncrypted,
            userInput.Cpf,
            userInput.Points,
            userInput.DateBirth,
            userInput.Role,
            userInput.AffiliateId
        );

        var userCreated = _uof.UserRepository.Create(user);
        await _uof.CommitChanges();

        var userDtoCreated = _mapper.Map<UserDTOOutput>(userCreated);

        return Result<UserDTOOutput>.Success(userDtoCreated);
    }

    public async Task<Result<(UserDTOOutput, Dictionary<string, string>)>> UpdateUser(UserUpdateDTO userDtoInput, int id)
    {
        if (userDtoInput is null)
        {
            return Result<(UserDTOOutput, Dictionary<string, string>)>.Failure(UserErrors.DataIsNull);
        }

        if (userDtoInput.UserId != id)
        {
            return Result<(UserDTOOutput, Dictionary<string, string>)>.Failure(UserErrors.IdMismatch);
        }
        
        var user = await _uof.UserRepository.GetAsync(x => x.UserId == id);

        if (user is null)
        {
            return Result<(UserDTOOutput, Dictionary<string, string>)>.Failure(UserErrors.NotFound);
        }
        
        var validation = await _updateValidator.ValidateAsync(userDtoInput);

        if (!validation.IsValid)
        {
            return Result<(UserDTOOutput, Dictionary<string, string>)>.Failure(UserErrors.IncorrectFormatData, validation.Errors);
        }

        var updatedFields = IdentifyUpdatedFields(userDtoInput, user);
        
        //var userForUpdate = _mapper.Map<User>(userDtoInput);
        var userForUpdate = new User(
            user.UserId,
            userDtoInput.Name,
            userDtoInput.Email,
            user.Password,
            userDtoInput.Cpf,
            user.Points,
            userDtoInput.DateBirth,
            userDtoInput.Role,
            userDtoInput.AffiliateId
        );

        var userUpdate = _uof.UserRepository.Update(userForUpdate);
        
        // remove old data from cache
        await _cacheService.RemoveAsync($"user-{id}");
        
        await _uof.CommitChanges();

        var userDtoUpdated = _mapper.Map<UserDTOOutput>(userUpdate);
        
        return Result<(UserDTOOutput, Dictionary<string, string>)>.Success((userDtoUpdated, updatedFields));
    }

    // oldPassword and newPassword come encrypted to the method 
    public async Task<Result<UserDTOOutput>> UpdateUserPassword(int userId, string oldPassword, string newPassword)
    { 
        var user = await _uof.UserRepository.GetByIdAsync(userId);
        
        if(user is null)
            return Result<UserDTOOutput>.Failure(UserErrors.NotFound);
        
        // Compare decrypted passwords
        var newPasswordDecrypted = _encryptService.Decrypt(newPassword);
        var oldPasswordDecrypted = _encryptService.Decrypt(oldPassword);
        var userPasswordDecrypted = _encryptService.Decrypt(user.Password!);
        
        if(!userPasswordDecrypted.Equals(oldPasswordDecrypted))
            return Result<UserDTOOutput>.Failure(UserErrors.PasswordMismatch);
        
        if (userPasswordDecrypted.Equals(newPasswordDecrypted) || oldPasswordDecrypted.Equals(newPasswordDecrypted))
            return Result<UserDTOOutput>.Failure(UserErrors.PasswordsEqualError);
        
        // change the old password to the new already encrypted password
        user.UpdatePassword(newPassword);
        
        _uof.UserRepository.Update(user);
        await _uof.CommitChanges();
        
        return Result<UserDTOOutput>.Success(_mapper.Map<UserDTOOutput>(user));
    }

    public Task<Result<UserDTOOutput>> UpdateUserRole(int userId, string role)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<UserDTOOutput>> DeleteUser(int? id)
    {
        var user = await _uof.UserRepository.GetAsync(e => e.UserId == id);

        if (user is null)
        {
            return Result<UserDTOOutput>.Failure(UserErrors.NotFound);
        }

        var userDeleted = _uof.UserRepository.Delete(user);
        
        // remove old data from cache
        await _cacheService.RemoveAsync($"user-{id}");
        
        await _uof.CommitChanges();

        var userDtoDeleted = _mapper.Map<UserDTOOutput>(userDeleted);

        return Result<UserDTOOutput>.Success(userDtoDeleted);
    }

    private Dictionary<string, string> IdentifyUpdatedFields(UserUpdateDTO userDtoInput, User user)
    {
        var updatedFields = new Dictionary<string, string>();
        
        if(userDtoInput.Name != user.Name)
            updatedFields.Add("Name", user.Name!);
        
        if(userDtoInput.Email != user.Email)
            updatedFields.Add("Email", user.Email!);
        
        if(userDtoInput.Role != user.Role)
            updatedFields.Add("Role", user.Role.ToString());

        return updatedFields;
    }
}