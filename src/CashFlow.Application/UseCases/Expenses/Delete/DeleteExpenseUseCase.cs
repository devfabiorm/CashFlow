using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception;

namespace CashFlow.Application.UseCases.Expenses.Delete;
public class DeleteExpenseUseCase : IDeleteExpenseUseCase
{
    private readonly IExpensesWriteOnlyRepository _repository;
    private readonly IExpensesReadOnlyRepository _readOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;

    public DeleteExpenseUseCase(
        IExpensesWriteOnlyRepository repository,
        IExpensesReadOnlyRepository readOnlyRepository,
        IUnitOfWork unitOfWork,
        ILoggedUser loggerUser)
    {
        _repository = repository;
        _readOnlyRepository = readOnlyRepository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggerUser;
    }

    public async Task Execute(long id)
    {
        var loggedUser = await _loggedUser.Get();

        var expense = await _readOnlyRepository.GetById(loggedUser, id);

        if (expense is null)
        {
            throw new DirectoryNotFoundException(ResourceErrorMessages.EXPENSE_NOT_FOUND);
        }
        
        await _repository.Delete(id);

        await _unitOfWork.Commit();
    }
}
