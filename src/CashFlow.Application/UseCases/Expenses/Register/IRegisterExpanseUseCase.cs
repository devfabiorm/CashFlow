using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.Expenses.Register;
public interface IRegisterExpanseUseCase
{
    Task<ResponseRegisteredExpenseJson> Execute(RequestRegisterExpenseJson request);
}
