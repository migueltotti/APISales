using AutoMapper;
using FluentValidation;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.DTOs.WorkDayDTO;
using Sales.Application.Interfaces;
using Sales.Application.ResultPattern;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;

namespace Sales.Application.Services;

public class WorkDayService : IWorkDayService
{
    private readonly IUnitOfWork _uof;
    private readonly IValidator<WorkDayDTOInput> _validator;
    private readonly IMapper _mapper;

    public WorkDayService(IUnitOfWork uof, IValidator<WorkDayDTOInput> validator, IMapper mapper)
    {
        _uof = uof;
        _validator = validator;
        _mapper = mapper;
    }


    public async Task<Result<WorkDayDTOOutput>> GetWorkDayByIdAsync(int workDayId)
    {
        var workDay =  await _uof.WorkDayRepository.GetByIdAsync(workDayId);
        
        if(workDay is null)
            return Result<WorkDayDTOOutput>.Failure(WorkDayErrors.NotFound);
        
        var workDayDTO = _mapper.Map<WorkDayDTOOutput>(workDay);
        
        return Result<WorkDayDTOOutput>.Success(workDayDTO);
    }

    public async Task<Result<WorkDayDTOOutput>> GetWorkDayByDateAsync(DateTime date)
    {
        var workDay =  await _uof.WorkDayRepository.GetAsync(wd => wd.StartDayTime.Date.Equals(date.Date));
        
        if(workDay is null)
            return Result<WorkDayDTOOutput>.Failure(WorkDayErrors.NotFound);
        
        var workDayDTO = _mapper.Map<WorkDayDTOOutput>(workDay);
        
        return Result<WorkDayDTOOutput>.Success(workDayDTO);
    }

    public async Task<Result<WorkDayDTOOutput>> StartWorkDay(WorkDayDTOInput workDayInput, int employeeId)
    {
        var validation = await _validator.ValidateAsync(workDayInput);

        if (!validation.IsValid)
            Result<WorkDayDTOOutput>.Failure(WorkDayErrors.IncorrectFormatData);

        var workDayRoutineExist = await GetWorkDayByDateAsync(DateTime.Now);

        if (workDayRoutineExist.isSuccess)
            return Result<WorkDayDTOOutput>.Failure(WorkDayErrors.RoutineExists);

        var employee = await _uof.UserRepository.GetByIdAsync(workDayInput.EmployeeId);
        
        if(employee is null)
            return Result<WorkDayDTOOutput>.Failure(UserErrors.NotFound);

        var workDay = new WorkDay(
            employeeId: workDayInput.EmployeeId,
            employeeName: employee.Name,
            startDayTime: DateTime.Now,
            numberOfOrders: 0,
            numberOfCanceledOrders: 0
        );

        _uof.WorkDayRepository.Create(workDay);
        await _uof.CommitChanges();
        
        var workDayDTO = _mapper.Map<WorkDayDTOOutput>(workDay);
        
        return Result<WorkDayDTOOutput>.Success(workDayDTO);
    }

    public async Task<Result<WorkDayDTOOutput>> FinishWorkDay(int workDayId, int employeeId)
    {
        var result = await GetWorkDayByIdAsync(workDayId);

        if (!result.isSuccess)
            return result;

        var workDay = _mapper.Map<WorkDay>(result.value);

        if (!workDay.EmployeeId.Equals(employeeId))
            return Result<WorkDayDTOOutput>.Failure(WorkDayErrors.EmployeeIdMismatch);

        workDay.FinishWorkDay(employeeId);

        _uof.WorkDayRepository.Update(workDay);
        await _uof.CommitChanges();
        
        var workDayDTO = _mapper.Map<WorkDayDTOOutput>(workDay);
        
        return Result<WorkDayDTOOutput>.Success(workDayDTO);
    }

    public async Task<Result<WorkDayDTOOutput>> RegisterOrderToWorkDay()
    {
        var workDay =  await _uof.WorkDayRepository.GetAsync(wd => wd.StartDayTime.Date.Equals(DateTime.Now.Date));
        
        if(workDay is null)
            return Result<WorkDayDTOOutput>.Failure(WorkDayErrors.NotFound);
        
        workDay.IncreaseNumberOfOrders();
        
        _uof.WorkDayRepository.Update(workDay);
        
        var workDayDTO = _mapper.Map<WorkDayDTOOutput>(workDay);
        
        return Result<WorkDayDTOOutput>.Success(workDayDTO);
    }

    public async Task<Result<WorkDayDTOOutput>> CancelOrderToWorkDay()
    {
        var workDay =  await _uof.WorkDayRepository.GetAsync(wd => wd.StartDayTime.Date.Equals(DateTime.Now.Date));
        
        if(workDay is null)
            return Result<WorkDayDTOOutput>.Failure(WorkDayErrors.NotFound);
        
        workDay.DecreaseNumberOfOrders();
        workDay.IncreaseNumberOfCanceledOrders();
        
        _uof.WorkDayRepository.Update(workDay);
        
        var workDayDTO = _mapper.Map<WorkDayDTOOutput>(workDay);
        
        return Result<WorkDayDTOOutput>.Success(workDayDTO);
    }
}