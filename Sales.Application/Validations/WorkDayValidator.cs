using FluentValidation;
using Sales.Application.DTOs.WorkDayDTO;

namespace Sales.Application.Validations;

public class WorkDayValidator : AbstractValidator<WorkDayDTOInput>
{
    public WorkDayValidator()
    {
        RuleFor(wk => wk.EmployeeId)
            .NotEmpty()
            .NotNull()
            .GreaterThan(0);
    }
}