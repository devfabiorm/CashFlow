using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Expenses.Register;
public class RegisterExpanseUseCase : IRegisterExpanseUseCase
{
    private readonly IExpensesRepository _expensesRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterExpanseUseCase(IExpensesRepository expensesRepository, IUnitOfWork unitOfWork)
    {
        _expensesRepository = expensesRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisteredExpenseJson> Execute(RequestRegisterExpenseJson request)
    {
        Validate(request);

        var entity = new Expense
        {
            Amount = request.Amount,
            Date = request.Date,
            Description = request.Description,
            PaymentType = (Domain.Enums.PaymentType)request.PaymentType,
            Title = request.Title,
        };

        await _expensesRepository.Add(entity);

        await _unitOfWork.Commit();

        return new ResponseRegisteredExpenseJson();
    }

    private void Validate(RequestRegisterExpenseJson request)
    {
        var validator = new RegisterExpenseValidator();

        var result = validator.Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(f => f.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
