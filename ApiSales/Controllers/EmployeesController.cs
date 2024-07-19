using ApiSales.DTOs.EmployeeDTO;
using ApiSales.Models;
using ApiSales.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiSales.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController(IUnitOfWork _uof, IMapper mapper) : ControllerBase
{

    [HttpGet("getEmployees")]
    public async Task<ActionResult<IEnumerable<EmployeeDTOOutput>>> Get()
    {
        var employees = await _uof.EmployeeRepository.GetAllAsync();

        var employeesDto = mapper.Map<IEnumerable<EmployeeDTOOutput>>(employees);
        
        return Ok(employeesDto);
    }

    [HttpGet("{id:int:min(1)}", Name = "GetEmployee")]
    public async Task<ActionResult<EmployeeDTOOutput>> GetEmployee(int id)
    {
        var employee = await _uof.EmployeeRepository.GetAsync(e => e.EmployeeId == id);

        if (employee is null)
        {
            return NotFound($"Employee with id = {id} not found");
        }
        
        var employeeDto = mapper.Map<EmployeeDTOOutput>(employee);

        return Ok(employeeDto);
    }

    [HttpGet("Orders/{id:int:min(1)}")]
    public async Task<ActionResult<EmployeeDTOOutput>> GetEmployeeOrders(int id)
    {
        var employeeOrder = await _uof.EmployeeRepository.GetEmployeeOrders(id);

        if (employeeOrder is null)
        {
            return NotFound($"Employee with id = {id} not found");
        }

        var employeeDtoOrder = mapper.Map<EmployeeDTOOutput>(employeeOrder);

        return Ok(employeeDtoOrder);
    }

    [HttpGet("Orders")]
    public async Task<ActionResult<IEnumerable<EmployeeDTOOutput>>> GetEmployeesOrders()
    {
        var employeesOrders = await _uof.EmployeeRepository.GetEmployeesOrders();

        var employeesDtoOrders = mapper.Map<IEnumerable<EmployeeDTOOutput>>(employeesOrders);
        
        return Ok(employeesDtoOrders);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDTOOutput>> Post(EmployeeDTOInput employeeDtoInput)
    {
        if (employeeDtoInput is null)
        {
            return BadRequest("Employee is null!!");
        }

        var employee = mapper.Map<Employee>(employeeDtoInput);

        var employeeCreated = _uof.EmployeeRepository.Create(employee);
        await _uof.CommitChanges();

        var employeeDtoCreated = mapper.Map<EmployeeDTOOutput>(employeeCreated);

        return new CreatedAtRouteResult("GetEmployee", 
            new { id = employeeDtoCreated.EmployeeId },
            employeeDtoCreated);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<EmployeeDTOOutput>> Put(EmployeeDTOInput employeeDtoInput, int id)
    {
        if (employeeDtoInput is null)
        {
            return BadRequest("Incorrect Data");
        }

        if (employeeDtoInput.EmployeeId != id)
        {
            return BadRequest("Id does not match Employee Id");
        }

        var employee = mapper.Map<Employee>(employeeDtoInput);

        var employeeUpdate = _uof.EmployeeRepository.Update(employee);
        await _uof.CommitChanges();

        var employeeDtoUpdated = mapper.Map<EmployeeDTOOutput>(employeeUpdate);

        return Ok(employeeDtoUpdated);
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<EmployeeDTOOutput>> Delete(int id)
    {
        var employee = await _uof.EmployeeRepository.GetAsync(e => e.EmployeeId == id);

        if (employee is null)
        {
            return NotFound($"Employee with id = {id} Not Found!");
        }

        var employeeDeleted = _uof.EmployeeRepository.Delete(employee);
        await _uof.CommitChanges();

        var employeeDtoDeleted = mapper.Map<EmployeeDTOOutput>(employeeDeleted);

        return Ok(employeeDtoDeleted);
    }
}