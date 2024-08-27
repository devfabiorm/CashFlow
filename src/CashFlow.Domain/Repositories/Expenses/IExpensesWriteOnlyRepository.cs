using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Expenses;
public interface IExpensesWriteOnlyRepository
{
    Task Add(Expense expense);
    /// <summary>
    /// Method to delete an expense
    /// </summary>
    /// <param name="id">Expense identifier</param>
    /// <returns>This function returns TRUE if the deletion was successful otherwise FALSE</returns>
    Task Delete(long id);
}
