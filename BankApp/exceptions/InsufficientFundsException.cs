namespace BankApp.exceptions;

public class InsufficientFundsException: Exception
{
    public InsufficientFundsException(){}
    public InsufficientFundsException(string message) : base(message){}
    public InsufficientFundsException(string message, Exception exception) : base(message, exception){}
}