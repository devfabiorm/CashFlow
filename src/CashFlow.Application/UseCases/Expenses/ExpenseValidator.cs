using CashFlow.Communication.Requests;
using CashFlow.Exception;
using FluentValidation;

namespace CashFlow.Application.UseCases.Expenses;
public class ExpenseValidator : AbstractValidator<RequestExpenseJson>
{
    public ExpenseValidator()
    {
        RuleFor(expense => expense.Title).NotEmpty().WithMessage(ResourceErrorMessages.REQUIRED_TITLE);
        RuleFor(expense => expense.Amount).GreaterThan(0).WithMessage(ResourceErrorMessages.AMOUN_MUST_BE_GREATER_THAN_ZERO);
        RuleFor(expense => expense.Date).LessThanOrEqualTo(DateTime.UtcNow).WithMessage(ResourceErrorMessages.AMOUNT_CANNOT_BE_FOR_THE_FUTURE);
        RuleFor(expense => expense.PaymentType).IsInEnum().WithMessage(ResourceErrorMessages.PAYMENT_TYPE_INVALID);
        RuleFor(expense => expense.Tags).ForEach(rules => 
        { 
            rules.IsInEnum().WithMessage(ResourceErrorMessages.TAG_TYPE_NOT_SUPPORTED);
        });
    }
}
