using BankApp.exceptions;
using BankApp.models;
using BankApp.repository;

namespace BankApp.services;

/// <summary>
/// The service layer of the account containing the business logic of the account. It sits between the
/// bank and the account repository, enforcing the rules an account must obey before anything is persisted.
/// </summary>
/// <param name="filename">The name of the JSON file the accounts are stored in</param>
public class AccountService(string filename)
{
    private readonly AccountRepository _accRepository = new(filename);

    /// <summary>
    /// requires: none<br/>
    /// modifies: the account database<br/>
    /// effects: adds a new account with the given name and opening balance and returns it; throws
    ///          ArgumentException if the username is blank, or InvalidAmountException if the opening
    ///          balance is negative
    /// </summary>
    /// <param name="userName">The name of the owner of the account</param>
    /// <param name="openingBalance">The amount of money to open the account with</param>
    /// <returns>The newly created account</returns>
    /// <exception cref="ArgumentException">thrown if the username is blank</exception>
    /// <exception cref="InvalidAmountException">thrown if the opening balance is negative</exception>
    public Account CreateAccount(string userName, decimal openingBalance)
    {
        if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Username cannot be empty");
        if (openingBalance < 0m) throw new InvalidAmountException("Opening balance cannot be negative");

        var newAccount = new Account(userName, openingBalance);
        _accRepository.Add(newAccount);

        return newAccount;
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: the account with id accountId<br/>
    /// effects: changes that account's username to newUserName; throws ArgumentException if newUserName
    ///          is blank, or AccountNotFoundException if no account has that id
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="newUserName">The new username to change to</param>
    /// <exception cref="ArgumentException">thrown if the new username is blank</exception>
    /// <exception cref="AccountNotFoundException">thrown if the account is not found</exception>
    public void UpdateAccountUsername(int accountId, string newUserName)
    {
        if (string.IsNullOrWhiteSpace(newUserName)) throw new ArgumentException("Username must not be empty");

        var account = _getOrThrow(accountId);

        account.Username = newUserName;
        _accRepository.Update(account);
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: the balance of the account with id accountId<br/>
    /// effects: decreases that account's balance by amount and returns the balance before and after;
    ///          throws AccountNotFoundException if no account has that id, InvalidAmountException if
    ///          amount is not positive, or InsufficientFundsException if amount is greater than the balance
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="amount">Amount to be withdrawn</param>
    /// <returns>The previous and new balance</returns>
    /// <exception cref="AccountNotFoundException">thrown if the account was not found.</exception>
    /// <exception cref="InvalidAmountException">thrown if the amount is not greater than 0.</exception>
    /// <exception cref="InsufficientFundsException">thrown if the amount is greater than the balance.</exception>
    public (decimal prevBalance, decimal newBalance) Withdraw(int accountId, decimal amount)
    {
        var account = _getOrThrow(accountId);

        decimal prevBalance = account.Balance;
        account.Withdraw(amount);
        _accRepository.Update(account);

        return (prevBalance, account.Balance);
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: the balance of the account with id accountId<br/>
    /// effects: increases that account's balance by amount and returns the balance before and after;
    ///          throws AccountNotFoundException if no account has that id, or InvalidAmountException if
    ///          amount is not positive
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="amount">Amount to be deposited</param>
    /// <returns>The previous and new balance</returns>
    /// <exception cref="AccountNotFoundException">thrown if the account was not found</exception>
    /// <exception cref="InvalidAmountException">thrown if the amount is not greater than 0</exception>
    public (decimal prevBalance, decimal newBalance) Deposit(int accountId, decimal amount)
    {
        var account = _getOrThrow(accountId);

        decimal prevBalance = account.Balance;
        account.Deposit(amount);
        _accRepository.Update(account);

        return (prevBalance, account.Balance);
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: nothing<br/>
    /// effects: returns the account with id accountId; throws AccountNotFoundException if no account has that id
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <returns>The account with the given id</returns>
    /// <exception cref="AccountNotFoundException">thrown if the account can't be found</exception>
    public Account GetAccount(int accountId)
    {
        return _getOrThrow(accountId);
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: nothing<br/>
    /// effects: returns every account, or an empty list if the bank has none
    /// </summary>
    /// <returns>A list of all accounts currently opened</returns>
    public List<Account> ListAccounts()
    {
        return _accRepository.GetAll();
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: the account database<br/>
    /// effects: removes the account with id accountId and returns it; throws AccountNotFoundException if
    ///          no account has that id
    /// </summary>
    /// <param name="accountId">The ID of the account to be deleted</param>
    /// <returns>The deleted account</returns>
    /// <exception cref="AccountNotFoundException">thrown if the account is not found</exception>
    public Account DeleteAccount(int accountId)
    {
        var removed = _accRepository.Delete(accountId);

        if (removed is null) throw new AccountNotFoundException();
        return removed;
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: the account database<br/>
    /// effects: removes every account, leaving the database empty
    /// </summary>
    public void Clear()
    {
        _accRepository.Clear();
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: nothing<br/>
    /// effects: returns the account with id accountId; throws AccountNotFoundException if no account has that id
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <returns>The account with the given id</returns>
    /// <exception cref="AccountNotFoundException">thrown if the account is not found</exception>
    private Account _getOrThrow(int accountId)
    {
        var account = _accRepository.GetById(accountId);
        if (account is null) throw new AccountNotFoundException();

        return account;
    }
}