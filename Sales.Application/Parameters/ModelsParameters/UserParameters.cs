namespace Sales.Application.Parameters.ModelsParameters;

public class UserParameters : QueryStringParameters
{
    public string? Status { get; set; }
    // completed
    // preparation
    
    public string? Affiliation { get; set; }
    
    public string? Cpf { get; set; } 
    
    public string? Name { get; set; }
    
    public decimal? Points { get; set; }
    public string? PointsCriteria  { get; set; }
    // greater
    // equal
    // less
    
    public string? Role { get; set; }
    // Admin
    // Employee
    // Customer
}