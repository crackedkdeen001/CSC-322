namespace BankApp.exceptions;

/// <summary>
/// An exception class thrown when the account balance isn't sufficient for withdrawal.
/// </summary>
public class InsufficientFundsException: Exception
{
    public InsufficientFundsException(){}
    public InsufficientFundsException(string message) : base(message){}
    public InsufficientFundsException(string message, Exception exception) : base(message, exception){}
}