using BankApp.exceptions;
using BankApp.interfaces;
using BankApp.models;
using BankApp.repository;

namespace BankApp.services;

/// <summary>
/// The service layer of the account containing the business logic of the account
/// </summary>
/// <param name="filename"></param>
public class AccountService(string filename)
{
    private readonly AccountRepository _accRepository = new AccountRepository(filename);
    /// <summary>
    /// Creates an account in the database with the name and opening balance specified.<br/>
    /// </summary>
    /// <param name="userName">The name of the owner of the account</param>
    /// <param name="openingBalance">The amount of money to open the account with. By default, it is 0.</param>
    /// <returns>The account ID of the create account</returns>
    /// <exception cref="ArgumentException">thrown if the username is empty or the opening balance is less than 0</exception>
    public int CreateAccount(string userName, decimal openingBalance = 0m)
    {
        if (userName.IsWhiteSpace()) throw new ArgumentException("Username cannot be empty");
        if (openingBalance < 0m) throw new InvalidAmountException("Opening balance cannot be negative");
        
        var newAccount = new Account(userName, openingBalance);
        _accRepository.Add(newAccount);

        return newAccount.Id;
    }

    /// <summary>
    /// Updates the username of an account<br/>
    /// </summary>
    /// <param name="accountId">The ID of the account.</param>
    /// <param name="newUserName">The new username to change to.</param>
    /// <returns>The updated account if it exists.</returns>
    /// <exception cref="AccountNotFoundException">Thrown if the account is not found.</exception>
    /// <exception cref="ArgumentException">If the new username is empty</exception>
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
    /// Returns the current balance of an account<br/>
    /// </summary>
    /// <param name="accountId">The ID of the specified account </param>
    /// <returns>The current balance of the account</returns>
    /// <exception cref="AccountNotFoundException">thrown if the account doesn't exist</exception>
    public decimal GetBalance(int accountId)
    {
        var account = _accRepository.GetById(accountId);
        if (account is null) throw new AccountNotFoundException();

        return account.Balance;
    }

    /// <summary>
    /// Returns the username of an account
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <returns>The username of the account holder</returns>
    /// <exception cref="AccountNotFoundException">thrown if the account cannot be found</exception>
    public string GetAccountUserName(int accountId)
    {
        var account = _accRepository.GetById(accountId);
        if (account is null) throw new AccountNotFoundException();

        return account.Username;
    }

    /// <summary>
    /// Gets the details of an account<br/>
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <exception cref="AccountNotFoundException">thrown if the account can't be found</exception>
    public void  GetAccountDetails(int accountId)
    {
        var account = _accRepository.GetById(accountId);
        if (account is null) throw new AccountNotFoundException();

        Console.WriteLine(account);
    }

    /// <summary>
    /// Deposits a specified amount of money into an account.<br/>
    ///</summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="amount">Amount to be deposited</param>
    /// <returns>The previous balance before money had been deposited and the new balance after money has been deposited</returns>
    /// <exception cref="InvalidAmountException">thrown if the amount is less than 0</exception>
    /// <exception cref="AccountNotFoundException">thrown if the account was not found</exception>
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

    /// <summary>
    /// List all accounts currently opened<br/>
    /// PreCondition: None
    /// PostCondtion: None
    /// </summary>
    public List<Account> ListAccounts()
    {
        return _accRepository.GetAll();
    }

    /// <summary>
    /// Deletes an account from the database. Throws an exception if the account doesn't exist.<br/>
    /// Precondition: None<br/>
    /// Postcondition: Number of accounts reduces by 1 if the account exists.
    /// </summary>
    /// <param name="accountId">The ID of the account to be deleted</param>
    /// <exception cref="AccountNotFoundException">Thrown if the account is not found</exception>
    /// <returns>The deleted account</returns>
    public Account DeleteAccount(int accountId)
    {
        var removed = _accRepository.Delete(accountId);

        if (removed is null) throw new AccountNotFoundException();
        return removed;
    }

    /// <summary>
    /// Removes all accounts for the database<br/>
    /// Precondition: None<br/>
    /// Postcondition: Number of accounts in the database is equal to 0
    /// </summary>
    public void Clear()
    {
        _accRepository.Clear();
    }
}

