namespace CashFlow.Exception.ExceptionsBase;
public class ErrorOnValidationException : CashFlowException
{
    public List<string> ErrorMessages { get; set; }

    public ErrorOnValidationException(List<string> errorMessages) : base(string.Empty)
    {
        ErrorMessages = errorMessages;
    }
}
