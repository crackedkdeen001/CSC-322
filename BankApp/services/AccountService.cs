using BankApp.exceptions;
using BankApp.interfaces;
using BankApp.Models;
using BankApp.repository;

namespace BankApp.services;

public class AccountService(string filename)
{
    private IRepository<Account> _accRepository = new AccountRepository(filename);
    /// <summary>
    /// Creates an account in the database with the name and opening balance specified.<br/>
    /// The opening balance by default is 0.<br/>
    /// PreCondition: A non-empty username and an opening balance >= 0 must be supplied<br/>
    /// PostCondition: A new account with said username and opening balance is added to the database.<br/>
    /// </summary>
    /// <param name="userName">The name of the owner of the account</param>
    /// <param name="openingBalance">The amount of money to open the account with. By default, it is 0.</param>
    /// <exception cref="ArgumentException">If any of the arguments are invalid</exception>
    public void CreateAccount(string userName, decimal openingBalance = 0m)
    {
        if (userName.IsWhiteSpace()) throw new ArgumentException("Username cannot be empty");
        if (openingBalance < 0m) throw new InvalidAmountException("Opening balance cannot be negative");
        
        var newAccount = new Account(userName, openingBalance);
        _accRepository.Add(newAccount);
    }

    /// <summary>
    /// Updates the username of an account<br/>
    /// PreCondition: Account must exist for update to take place and the new username must be non-empty<br/>
    /// PostCondition: Account username has been changed if the account exists.<br/>
    /// An error is thrown if it doesn't exist.
    /// </summary>
    /// <param name="accountId">The ID of the account.</param>
    /// <param name="newUserName">The new username to change to.</param>
    /// <exception cref="AccountNotFoundException">Thrown if the account is not found.</exception>
    /// <exception cref="ArgumentException">If the new username is empty</exception>
    /// <returns>The updated account if it exists.</returns>
    public void UpdateAccountUsername(int accountId, string newUserName)
    {
        if (newUserName.IsWhiteSpace()) throw new ArgumentException("Username must not be empty");
         
        var account = _accRepository.GetById(accountId);
        if (account is null) throw new AccountNotFoundException();
        
        account.Username = newUserName;
        _accRepository.Update(account);
    }

    /// <summary>
    /// Withdraws a specified amount of money from an account with enough cash.<br/>
    /// PreCondition: The specified account must exist and the amount to be withdrawn <= account balance.<br/>
    ///               The amount must also be greater than 0<br/>
    /// PostCondition: The new balance of the account is the previous balance - amount withdrawn.
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="amount">Amount to be withdrawn</param>
    /// <exception cref="InvalidAmountException">thrown if the amount is less than 0</exception>
    /// <exception cref="InsufficientFundsException">thrown if the amount is greater than account balance</exception>
    /// <exception cref="AccountNotFoundException">thrown if the account was not found</exception>
    /// <returns>The previous and new balance</returns>
    public (decimal prevBalance, decimal newBalance) Withdraw(int accountId, decimal amount)
    {
        var account = _accRepository.GetById(accountId);
        if (account is null) throw new AccountNotFoundException();
        
        if (amount <= 0m) throw new InvalidAmountException("Amount must be greater than 0.");
        if (account.Balance < amount) throw new InsufficientFundsException("Insufficient funds");

        decimal prevBalance = account.Balance;
        account.Balance -= amount;
        _accRepository.Update(account);

        return (prevBalance, account.Balance);
    }


    /// <summary>
    /// Returns the current balance of an account. Throws an exception if the account does not exist<br/>
    /// PostCondition: The account must exist in the database
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns>The current balance of the account</returns>
    public decimal GetBalance(int accountId)
    {
        var account = _accRepository.GetById(accountId);
        if (account is null) throw new AccountNotFoundException();

        return account.Balance;
    }

    /// <summary>
    /// Gets the details of an account<br/>
    /// Precondition: The account must exist<br/>
    /// Postcondition: Nothing
    /// </summary>
    /// <exception cref="AccountNotFoundException">thrown if the account can't be found</exception>
    /// <param name="accountId"></param>
    public void  GetAccountDetails(int accountId)
    {
        var account = _accRepository.GetById(accountId);
        if (account is null) throw new AccountNotFoundException();

        Console.WriteLine(account);
    }

    /// <summary>
    /// Deposits a specified amount of money into an account.<br/>
    /// PreCondition: The specified account must exist and the amount to be deposited must also be greater than 0<br/>
    /// PostCondition: The new balance of the account is the previous balance + amount deposited.
    ///</summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="amount">Amount to be deposited</param>
    /// <exception cref="InvalidAmountException">thrown if the amount is less than 0</exception>
    /// <exception cref="AccountNotFoundException">thrown if the account was not found</exception>
    /// <returns>The previous balance before money had been deposited and the new balance after money has been deposited</returns>
    public (decimal prevBalance, decimal newBalance) Deposit(int accountId, decimal amount)
    {
        var account = _accRepository.GetById(accountId);
        if (account is null) throw new AccountNotFoundException();
        if (amount <= 0m) throw new InvalidAmountException("Amount must be greater than 0.");

        decimal prevBalance = account.Balance;
        account.Balance += amount;
        _accRepository.Update(account);
        
        return (prevBalance, account.Balance);
    }
    
}

