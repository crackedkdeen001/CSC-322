namespace BankApp.exceptions;

public class AccountNotFoundException: Exception
{
    public AccountNotFoundException(){}
    public AccountNotFoundException(string message= "Account not found") : base(message){}
    public AccountNotFoundException(string message, Exception exception) : base(message, exception){}
}