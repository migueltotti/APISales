namespace Sales.Application.DTOs.UserDTO;

public record ChangePasswordDTO(
    int userId,
    string oldPassword,
    string newPassword
);