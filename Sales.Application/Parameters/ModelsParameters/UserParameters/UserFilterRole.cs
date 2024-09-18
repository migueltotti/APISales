namespace Sales.Application.Parameters.ModelsParameters.UserParameters;

public class UserFilterRole : QueryStringParameters
{
    public string? Role { get; set; }
    // Admin
    // Employee
    // Customer
}