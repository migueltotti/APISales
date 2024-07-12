using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using ApiSales.Models.Enums;

namespace ApiSales.Models;

public class Employee
{
    
    [Key]
    public int EmployeeId { get; set; }
    public string? Name { get; set; }
    public string? Cpf { get; set; }
    public DateTime DateBirth { get; set; }
    public Permission Permission { get; set; }

    // Employee 1 : n Order
    public ICollection<Order>? Orders = [];
}