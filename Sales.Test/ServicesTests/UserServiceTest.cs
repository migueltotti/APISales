using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.UserDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Application.Services;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using X.PagedList;
using X.PagedList.Extensions;

namespace Sales.Test.ServicesTests;

public class UserServiceTest
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _mockUof;
    private readonly IValidator<UserDTOInput> _mockValidator;
    private readonly IMapper _mockMapper;
    private readonly IUserFilterFactory _mockUserFilterFactory;
    private readonly Fixture _fixture;

    public UserServiceTest()
    {
        _mockUof = Substitute.For<IUnitOfWork>();
        _mockValidator = Substitute.For<IValidator<UserDTOInput>>();
        _mockMapper = Substitute.For<IMapper>();
        _mockUserFilterFactory = Substitute.For<IUserFilterFactory>();
        _fixture = new Fixture();

        _userService = new UserService(
            _mockUof,
            _mockValidator,
            _mockMapper,
            _mockUserFilterFactory
            );
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnAllUsers()
    {
        // Arrange
        var users = _fixture.CreateMany<User>(3);
        var usersDto = _fixture.CreateMany<UserDTOOutput>(3);
        
        _mockUof.UserRepository.GetAllAsync().Returns(users);
        _mockMapper.Map<IEnumerable<UserDTOOutput>>(Arg.Any<IEnumerable<User>>()).Returns(usersDto);
        
        // Act
        var result = await _userService.GetAllUsers();
        
        // Assert
        result.Should().BeEquivalentTo(usersDto);
        
        await _mockUof.UserRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<UserDTOOutput>>(Arg.Any<IEnumerable<User>>());
    }
    
    [Fact]
    public async Task GetAllUsersPaged_ShouldReturnAllUsersPaged()
    {
        // Arrange
        var parameters = new QueryStringParameters();
        var users = _fixture.CreateMany<User>(3);
        var usersDto = _fixture.CreateMany<UserDTOOutput>(3);
        
        _mockUof.UserRepository.GetAllAsync().Returns(users);
        _mockMapper.Map<IEnumerable<UserDTOOutput>>(Arg.Any<IEnumerable<User>>()).Returns(usersDto);
        
        // Act
        var result = await _userService.GetAllUsers();
        
        // Assert
        result.Should().BeEquivalentTo(usersDto.ToPagedList(parameters.PageNumber, parameters.PageSize));
        
        await _mockUof.UserRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<UserDTOOutput>>(Arg.Any<IEnumerable<User>>());
    }
    
    [Fact]
    public async Task GetUsersWithFilter_ShouldReturnUsersThatMatchesSomeFilter()
    {
        // Arrange
        var filter = "filter";
        var parameters = new UserParameters();
        var users = _fixture.CreateMany<User>(3);
        var usersDto = _fixture.CreateMany<UserDTOOutput>(3);
        
        _mockUof.UserRepository.GetAllAsync().Returns(users);
        _mockMapper.Map<IEnumerable<UserDTOOutput>>(Arg.Any<IEnumerable<User>>()).Returns(usersDto);
        _mockUserFilterFactory.GetStrategy(Arg.Any<string>())
            .ApplyFilter(Arg.Any<IEnumerable<UserDTOOutput>>(), Arg.Any<UserParameters>())
            .Returns(usersDto);
        
        // Act
        var result = await _userService.GetUsersWithFilter(filter, parameters);
        
        // Assert
        result.Should().BeEquivalentTo(usersDto.ToPagedList(parameters.PageNumber, parameters.PageSize));
        
        await _mockUof.UserRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<UserDTOOutput>>(Arg.Any<IEnumerable<User>>());
        _mockUserFilterFactory.Received(1).GetStrategy(Arg.Any<string>())
            .ApplyFilter(Arg.Any<IEnumerable<UserDTOOutput>>(), Arg.Any<UserParameters>());
    }
    
    [Fact]
    public async Task GetUsersByPoint_ShouldReturnUsersThatMatchesPointsAndGreaterCriteriaPoints()
    {
        // Arrange
        var parameters = new UserParameters()
        {
            Points = 10,
            PointsCriteria = "greater"
        };
        var users = _fixture.CreateMany<User>(3);
        var usersDto = _fixture.CreateMany<UserDTOOutput>(3);
        
        _mockUof.UserRepository.GetAllAsync().Returns(users);
        _mockMapper.Map<IEnumerable<UserDTOOutput>>(Arg.Any<IEnumerable<User>>()).Returns(usersDto);
        
        // Act
        var result = await _userService.GetUsersByPoints(parameters);
        
        // Assert
        result.Should().BeEquivalentTo(usersDto.ToPagedList(parameters.PageNumber, parameters.PageSize));
        
        await _mockUof.UserRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<UserDTOOutput>>(Arg.Any<IEnumerable<User>>());
    }
    
    [Fact]
    public async Task GetUsersByPoint_ShouldReturnUsersThatMatchesPointsAndEqualCriteriaPoints()
    {
        // Arrange
        var parameters = new UserParameters()
        {
            Points = 10,
            PointsCriteria = "equal"
        };
        var users = _fixture.CreateMany<User>(3);
        var usersDto = _fixture.CreateMany<UserDTOOutput>(3);
        
        _mockUof.UserRepository.GetAllAsync().Returns(users);
        _mockMapper.Map<IEnumerable<UserDTOOutput>>(Arg.Any<IEnumerable<User>>()).Returns(usersDto);
        
        // Act
        var result = await _userService.GetUsersByPoints(parameters);
        
        // Assert
        result.Should().BeOfType<PagedList<UserDTOOutput>>();
        
        await _mockUof.UserRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<UserDTOOutput>>(Arg.Any<IEnumerable<User>>());
    }
    
    [Fact]
    public async Task GetUsersByPoint_ShouldReturnUsersThatMatchesPointsAndLessCriteriaPoints()
    {
        // Arrange
        var parameters = new UserParameters()
        {
            Points = 10,
            PointsCriteria = "less"
        };
        var users = _fixture.CreateMany<User>(3);
        var usersDto = _fixture.CreateMany<UserDTOOutput>(3);
        
        _mockUof.UserRepository.GetAllAsync().Returns(users);
        _mockMapper.Map<IEnumerable<UserDTOOutput>>(Arg.Any<IEnumerable<User>>()).Returns(usersDto);
        
        // Act
        var result = await _userService.GetUsersByPoints(parameters);
        
        // Assert
        result.Should().BeOfType<PagedList<UserDTOOutput>>();
        
        await _mockUof.UserRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<UserDTOOutput>>(Arg.Any<IEnumerable<User>>());
    }
    
    [Fact]
    public async Task GetUsersByAffiliation_ShouldReturnUsersThatMatchesSomeAffiliationName()
    {
        // Arrange
        var parameters = new UserParameters()
        {
            Affiliation = "Affiliation"
        };
        var users = _fixture.CreateMany<User>(3);
        var usersDto = _fixture.CreateMany<UserDTOOutput>(3);
        var affiliates = _fixture.Create<Affiliate>();
        
        _mockUof.UserRepository.GetAllAsync().Returns(users);
        _mockMapper.Map<IEnumerable<UserDTOOutput>>(Arg.Any<IEnumerable<User>>()).Returns(usersDto);
        _mockUof.AffiliateRepository.GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>())
            .Returns(affiliates);
        
        // Act
        var result = await _userService.GetUsersByAffiliation(parameters);
        
        // Assert
        result.Should().BeOfType<PagedList<UserDTOOutput>>();
        
        await _mockUof.UserRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<UserDTOOutput>>(Arg.Any<IEnumerable<User>>());
        await _mockUof.AffiliateRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>());
    }
    
    [Fact]
    public async Task GetUsersByOrdersNotCompleted_ShouldReturnUsersThatOrdersHaveNotBeenCompleted()
    {
        // Arrange
        var parameters = new UserParameters();
        var users = _fixture.CreateMany<User>(3);
        var usersDto = _fixture.CreateMany<UserDTOOutput>(3);
        var orders = _fixture.CreateMany<Order>(3);
        
        _mockUof.OrderRepository.GetAllAsync().Returns(orders);
        _mockUof.UserRepository.GetAllAsync().Returns(users);
        _mockMapper.Map<IEnumerable<UserDTOOutput>>(Arg.Any<IEnumerable<User>>()).Returns(usersDto);
        
        // Act
        var result = await _userService.GetUsersByOrdersNotCompleted(parameters);
        
        // Assert
        result.Should().BeOfType<PagedList<UserDTOOutput>>();
        
        await _mockUof.OrderRepository.Received(1).GetAllAsync();
        await _mockUof.UserRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<UserDTOOutput>>(Arg.Any<IEnumerable<User>>());
    }
    
    [Fact]
    public async Task GetUsersById_ShouldReturnUserThatMatchesId()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        var user = _fixture.Create<User>();
        var userDto = _fixture.Create<UserDTOOutput>();
        
        _mockUof.UserRepository.GetByIdAsync(Arg.Any<int>()).Returns(user);
        _mockMapper.Map<UserDTOOutput>(Arg.Any<User>()).Returns(userDto);
        
        // Act
        var result = await _userService.GetUserById(userId);
        
        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(userDto);
        
        await _mockUof.UserRepository.Received(1).GetByIdAsync(Arg.Any<int>());
        _mockMapper.Received(1).Map<UserDTOOutput>(Arg.Any<User>());
    }
    
    [Fact]
    public async Task GetUsersById_ShouldReturnNotFoundWithNotFoundResponse_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        User user = null;
        
        _mockUof.UserRepository.GetByIdAsync(Arg.Any<int>()).Returns(user);
        
        // Act
        var result = await _userService.GetUserById(userId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(UserErrors.NotFound);
        
        await _mockUof.UserRepository.Received(1).GetByIdAsync(Arg.Any<int>());
    }
    
    [Fact]
    public async Task GetUsersBy_ShouldReturnUserThatMatchesSomeExpression()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var userDto = _fixture.Create<UserDTOOutput>();
        
        _mockUof.UserRepository.GetAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(user);
        _mockMapper.Map<UserDTOOutput>(Arg.Any<User>()).Returns(userDto);
        
        // Act
        var result = await _userService.GetUserBy(u => u.UserId == 1);
        
        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(userDto);
        
        await _mockUof.UserRepository.Received(1).GetAsync(Arg.Any<Expression<Func<User, bool>>>());
        _mockMapper.Received(1).Map<UserDTOOutput>(Arg.Any<User>());
    }
    
    [Fact]
    public async Task GetUsersBy_ShouldReturnNotFoundWithNotFoundResponse_WhenUserDoesNotExist()
    {
        // Arrange
        User user = null;
        
        _mockUof.UserRepository.GetAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(user);
        
        // Act
        var result = await _userService.GetUserBy(u => u.UserId == 1);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(UserErrors.NotFound);
        
        await _mockUof.UserRepository.Received(1).GetAsync(Arg.Any<Expression<Func<User, bool>>>());
    }
    
    [Fact]
    public async Task CreateUser_ShouldReturnCreatedUser()
    {
        // Arrange
        var userInput = _fixture.Create<UserDTOInput>();
        var validation = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
        };
        User userExists = null;
        var user = _fixture.Create<User>();
        var userDto = _fixture.Create<UserDTOOutput>();
        
        _mockUof.UserRepository.GetAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(userExists);
        _mockValidator.ValidateAsync(Arg.Any<UserDTOInput>()).Returns(validation);
        _mockMapper.Map<User>(Arg.Any<UserDTOInput>()).Returns(user);
        _mockUof.UserRepository.Create(Arg.Any<User>()).Returns(user);
        _mockMapper.Map<UserDTOOutput>(Arg.Any<User>()).Returns(userDto);
        
        // Act
        var result = await _userService.CreateUser(userInput);
        
        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(userDto);
        
        await _mockUof.UserRepository.Received(1).GetAsync(Arg.Any<Expression<Func<User, bool>>>());
        await _mockValidator.Received(1).ValidateAsync(Arg.Any<UserDTOInput>());
        _mockMapper.Received(1).Map<User>(Arg.Any<UserDTOInput>());
        _mockUof.UserRepository.Received(1).Create(Arg.Any<User>());
        _mockMapper.Received(1).Map<UserDTOOutput>(Arg.Any<User>());
    }
    
    [Fact]
    public async Task CreateUser_ShouldReturnBadRequestWithUserExistsResponse_WhenUserExist()
    {
        // Arrange
        var userInput = _fixture.Create<UserDTOInput>();
        var validation = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
        };
        var userExists = _fixture.Create<User>();
        var userExistsDto = _fixture.Create<UserDTOOutput>();
        
        _mockValidator.ValidateAsync(Arg.Any<UserDTOInput>()).Returns(validation);
        _mockMapper.Map<UserDTOOutput>(Arg.Any<User>()).Returns(userExistsDto);
        _mockUof.UserRepository.GetAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(userExists);
        
        // Act
        var result = await _userService.CreateUser(userInput);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(UserErrors.UserExists);
        
        await _mockUof.UserRepository.Received(1).GetAsync(Arg.Any<Expression<Func<User, bool>>>());
        await _mockValidator.Received(1).ValidateAsync(Arg.Any<UserDTOInput>());
    }
    
    [Fact]
    public async Task CreateUser_ShouldReturnBadRequestWithIncorrectFormatDataResponse_WhenUserInputIsInvalid()
    {
        // Arrange
        var userInput = _fixture.Create<UserDTOInput>();
        var validation = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
            {
                new ValidationFailure("IncorrectFormatData", "IncorrectFormatData")
            }
        };
        
        _mockValidator.ValidateAsync(Arg.Any<UserDTOInput>()).Returns(validation);
        
        // Act
        var result = await _userService.CreateUser(userInput);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(UserErrors.IncorrectFormatData);
        
        await _mockValidator.Received(1).ValidateAsync(Arg.Any<UserDTOInput>());
    }
    
    [Fact]
    public async Task CreateUser_ShouldReturnBadRequestWithDataIsNullResponse_WhenUserInputIsNull()
    {
        // Arrange
        UserDTOInput userInput = null;
        
        // Act
        var result = await _userService.CreateUser(userInput);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(UserErrors.DataIsNull);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnUpdatedUser()
    {
        // Arrange
        var userInput = _fixture.Create<UserDTOInput>();
        var userId = userInput.UserId;
        var user = _fixture.Create<User>();
        var validation = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
        };
        var userDtoUpdated = _fixture.Create<UserDTOOutput>();
        
        _mockUof.UserRepository.GetAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(user);
        _mockValidator.ValidateAsync(Arg.Any<UserDTOInput>()).Returns(validation);
        _mockMapper.Map<User>(Arg.Any<UserDTOInput>()).Returns(user);
        _mockUof.UserRepository.Update(Arg.Any<User>()).Returns(user);
        _mockMapper.Map<UserDTOOutput>(Arg.Any<User>()).Returns(userDtoUpdated);
        
        // Act
        var result = await _userService.UpdateUser(userInput, userId);
        
        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(userDtoUpdated);

        await _mockUof.UserRepository.Received(1).GetAsync(Arg.Any<Expression<Func<User, bool>>>());
        await _mockValidator.Received(1).ValidateAsync(Arg.Any<UserDTOInput>());
        _mockMapper.Received(1).Map<User>(Arg.Any<UserDTOInput>());
        _mockUof.UserRepository.Received(1).Update(Arg.Any<User>());
        _mockMapper.Received(1).Map<UserDTOOutput>(Arg.Any<User>());
    }
    
    [Fact]
    public async Task UpdateUser_ShouldReturnBadRequestWithIncorrectFormatDataResponse_WhenUserInputIsInvalid()
    {
        // Arrange
        var userInput = _fixture.Create<UserDTOInput>();
        var userId = userInput.UserId;
        var user = _fixture.Create<User>();
        var validation = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
            {
                new ValidationFailure("IncorrectFormatData", "IncorrectFormatData")
            }
        };
        
        _mockUof.UserRepository.GetAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(user);
        _mockValidator.ValidateAsync(Arg.Any<UserDTOInput>()).Returns(validation);
        
        // Act
        var result = await _userService.UpdateUser(userInput, userId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(UserErrors.IncorrectFormatData);
        result.validationFailures.Should().NotBeEmpty();

        await _mockUof.UserRepository.Received(1).GetAsync(Arg.Any<Expression<Func<User, bool>>>());
        await _mockValidator.Received(1).ValidateAsync(Arg.Any<UserDTOInput>());
    }
    
    [Fact]
    public async Task UpdateUser_ShouldReturnNotFoundWithNotFoundResponse_WhenUserDoesNotExist()
    {
        // Arrange
        var userInput = _fixture.Create<UserDTOInput>();
        var userId = userInput.UserId;
        User user = null;
        
        _mockUof.UserRepository.GetAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(user);
        
        // Act
        var result = await _userService.UpdateUser(userInput, userId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(UserErrors.NotFound);

        await _mockUof.UserRepository.Received(1).GetAsync(Arg.Any<Expression<Func<User, bool>>>());
    }
    
    [Fact]
    public async Task UpdateUser_ShouldReturnBadRequestWithIdMismatchResponse_WhenUserAndUserIdDoesNotMatch()
    {
        // Arrange
        var userInput = _fixture.Create<UserDTOInput>();
        var userId = _fixture.Create<int>();
        
        // Act
        var result = await _userService.UpdateUser(userInput, userId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(UserErrors.IdMismatch);
    }
    
    [Fact]
    public async Task UpdateUser_ShouldReturnBadRequestWithDataIsNullResponse_WhenUserInputIsNull()
    {
        // Arrange
        UserDTOInput userInput = null;
        var userId = _fixture.Create<int>();
        
        // Act
        var result = await _userService.UpdateUser(userInput, userId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(UserErrors.DataIsNull);
    }
    
    [Fact]
    public async Task DeleteUser_ShouldReturnDeletedUser()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        var user = _fixture.Create<User>();
        var userDto = _fixture.Create<UserDTOOutput>();
        
        _mockUof.UserRepository.GetAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(user);
        _mockUof.UserRepository.Delete(Arg.Any<User>()).Returns(user);
        _mockMapper.Map<UserDTOOutput>(Arg.Any<User>()).Returns(userDto);
        
        // Act
        var result = await _userService.DeleteUser(userId);
        
        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(userDto);
        
        await _mockUof.UserRepository.Received(1).GetAsync(Arg.Any<Expression<Func<User, bool>>>());
        _mockUof.UserRepository.Received(1).Delete(Arg.Any<User>());
        _mockMapper.Received(1).Map<UserDTOOutput>(Arg.Any<User>());
    }
    
    [Fact]
    public async Task DeleteUser_ShouldReturnNotFoundWithNotFoundResponse_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        User user = null;
        
        _mockUof.UserRepository.GetAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(user);
        
        // Act
        var result = await _userService.DeleteUser(userId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(UserErrors.NotFound);
        
        await _mockUof.UserRepository.Received(1).GetAsync(Arg.Any<Expression<Func<User, bool>>>());
    }
}