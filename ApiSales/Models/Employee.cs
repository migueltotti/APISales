using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using ApiSales.Models.Enums;
using ApiSales.Validations;

namespace ApiSales.Models;

public class Employee
{
    public Employee()
    {
        Orders = new Collection<Order>();
    }
    
    [Key]
    public int EmployeeId { get; set; }
    
    [Required(ErrorMessage = "Employee name is required!")]
    [StringLength(50, ErrorMessage = "Name must be less than {1} and more than {3}",MinimumLength = 5)]
    [FirstLetterUpperCase]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "Employee CPF must be informed!")]
    [StringLength(15, ErrorMessage = "Incorrect format")]
    [RegularExpression("^[0-9]{3}\\.?[0-9]{3}\\.?[0-9]{3}\\-?[0-9]{2}$")]
    public string? Cpf { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime DateBirth { get; set; }
    
    public Permission Permission { get; set; }

    // Employee 1 : n Order
    public ICollection<Order>? Orders { get; set; }
}