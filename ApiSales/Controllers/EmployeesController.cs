using ApiSales.Models;
using ApiSales.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiSales.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController(IUnitOfWork _uof) : ControllerBase
{

    [HttpGet("getEmployees")]
    public async Task<ActionResult<Employee>> Get()
    {
        var employees = await _uof.EmployeeRepository.GetAllAsync();

        return Ok(employees);
    }

    [HttpGet("{id:int:min(1)}", Name = "GetEmployee")]
    public async Task<ActionResult<Employee>> GetEmployee(int id)
    {
        var employee = await _uof.EmployeeRepository.GetAsync(e => e.EmployeeId == id);

        if (employee is null)
        {
            return NotFound($"Employee with id = {id} not found");
        }

        return Ok(employee);
    }

    [HttpGet("Orders/{id:int:min(1)}")]
    public async Task<ActionResult<Employee>> GetEmployeeOrders(int id)
    {
        var employeeOrder = await _uof.EmployeeRepository.GetEmployeeOrders(id);

        if (employeeOrder is null)
        {
            return NotFound($"Employee with id = {id} not found");
        }

        return Ok(employeeOrder);
    }

    [HttpGet("Orders")]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesOrders()
    {
        var employeesOrders = await _uof.EmployeeRepository.GetEmployeesOrders();

        return Ok(employeesOrders);
    }

    [HttpPost]
    public async Task<ActionResult<Employee>> Post(Employee employee)
    {
        if (employee is null)
        {
            return BadRequest("Employee is null!!");
        }

        var employeeCreated = _uof.EmployeeRepository.Create(employee);
        await _uof.CommitChanges();

        return new CreatedAtRouteResult("GetEmployee", 
            new { id = employeeCreated.EmployeeId },
            employeeCreated);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<Employee>> Put(Employee employee, int id)
    {
        if (employee is null)
        {
            return BadRequest("Incorrect Data");
        }

        if (employee.EmployeeId != id)
        {
            return BadRequest("Id does not match Employee Id");
        }

        var employeeUpdate = _uof.EmployeeRepository.Update(employee);
        await _uof.CommitChanges();

        return Ok(employeeUpdate);
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<Employee>> Delete(int id)
    {
        var employee = await _uof.EmployeeRepository.GetAsync(e => e.EmployeeId == id);

        if (employee is null)
        {
            return NotFound($"Employee with id = {id} Not Found!");
        }

        var employeeDeleted = _uof.EmployeeRepository.Delete(employee);
        await _uof.CommitChanges();

        return Ok(employeeDeleted);
    }
}