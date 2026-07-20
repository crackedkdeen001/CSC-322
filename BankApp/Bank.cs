/*using BankApp.Models;
using BankApp.repository;
using BankApp.services;

namespace BankApp;

/// <summary>
/// The face of the bank. This is the only class the user of the program talks to - it takes the two
/// services and drives them together so that money never moves without the history saying so
/// </summary>
/// <remarks>
/// Nothing here throws. When an operation cannot be carried out the bank writes a message explaining
/// why and hands back null (or false), so the caller never has to catch anything. The services below
/// still raise the rules as exceptions - the bank is the one place that turns those into messages.
/// </remarks>
public class Bank
{
    private readonly AccountService _accountService;
    private readonly TransactionService _transactionService;
    private readonly TextWriter _output;

    /// <summary>
    /// Creates a bank whose accounts and transactions live in JSON files
    /// </summary>
    /// <param name="accountsFile">The path of the file holding the accounts</param>
    /// <param name="transactionsFile">The path of the file holding the transactions</param>
    /// <param name="output">Where messages are written. Defaults to the console</param>
    public Bank(string accountsFile, string transactionsFile, TextWriter? output = null)
        : this(
            new AccountService(new AccountRepository(accountsFile)),
            new TransactionService(new TransactionRepository(transactionsFile)),
            output)
    {
    }

    /// <summary>
    /// Creates a bank on top of the given services. Used by the tests to run the bank without touching disk
    /// </summary>
    /// <param name="accountService">The service holding the account rules</param>
    /// <param name="transactionService">The service holding the transaction rules</param>
    /// <param name="output">Where messages are written. Defaults to the console</param>
    public Bank(AccountService accountService, TransactionService transactionService, TextWriter? output = null)
    {
        _accountService = accountService;
        _transactionService = transactionService;
        _output = output ?? Console.Out;
    }

    /// <summary>
    /// Opens a new account. If it is opened with money in it, that is recorded as its first credit<br/>
    /// PreCondition: None<br/>
    /// PostCondition: The account exists and has an id, and its history holds an opening credit if it
    /// was opened with money. If the username was blank or the opening balance negative, no account is
    /// created and a message says why
    /// </summary>
    /// <param name="username">The name of the account owner</param>
    /// <param name="openingBalance">The amount the account starts with</param>
    /// <returns>The new account, or null if it could not be created</returns>
    public Account? CreateAccount(string username, decimal openingBalance = 0m)
    {
        try
        {
            Account account = _accountService.CreateAccount(username, openingBalance);

            if (openingBalance > 0m)
            {
                _transactionService.Record(account.Id, openingBalance, TransactionType.Credit, "Opening balance");
            }

            return account;
        }
        catch (Exception ex)
        {
            return Report<Account>(ex);
        }
    }

    /// <summary>
    /// Lists every account in the bank<br/>
    /// PreCondition: None<br/>
    /// PostCondition: Returns all accounts, or an empty list if the bank has none
    /// </summary>
    /// <returns>All accounts, or an empty list if the bank has none</returns>
    public List<Account> GetAllAccounts() => _accountService.GetAllAccounts();

    /// <summary>
    /// Gets one account<br/>
    /// PreCondition: None<br/>
    /// PostCondition: Returns the account, or writes a message and returns null if no account has that id
    /// </summary>
    /// <param name="id">The id of the account</param>
    /// <returns>The account, or null if no account has that id</returns>
    public Account? GetAccount(int id)
    {
        try
        {
            return _accountService.GetAccount(id);
        }
        catch (Exception ex)
        {
            return Report<Account>(ex);
        }
    }

    /// <summary>
    /// Shows an account's balance<br/>
    /// PreCondition: None<br/>
    /// PostCondition: Returns the balance, or writes a message and returns null if no account has that id
    /// </summary>
    /// <param name="id">The id of the account</param>
    /// <returns>The account's current balance, or null if no account has that id</returns>
    public decimal? GetBalance(int id)
    {
        try
        {
            return _accountService.GetBalance(id);
        }
        catch (Exception ex)
        {
            _output.WriteLine(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Pays money into an account and records the credit<br/>
    /// PreCondition: None<br/>
    /// PostCondition: The balance has grown by the amount and the history holds a matching credit. If
    /// the account does not exist or the amount is not positive, nothing changes and a message says why
    /// </summary>
    /// <param name="id">The id of the account</param>
    /// <param name="amount">How much to pay in</param>
    /// <param name="description">A note saying why the money came in</param>
    /// <returns>True if the money was paid in, false if it was not</returns>
    public bool Deposit(int id, decimal amount, string description = "Deposit")
    {
        try
        {
            _accountService.Deposit(id, amount);
            _transactionService.Record(id, amount, TransactionType.Credit, description);

            return true;
        }
        catch (Exception ex)
        {
            _output.WriteLine(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Takes money out of an account and records the debit<br/>
    /// PreCondition: None<br/>
    /// PostCondition: The balance has shrunk by the amount and the history holds a matching debit. If
    /// the account does not exist, the amount is not positive, or there is not enough money, neither the
    /// balance nor the history changes and a message says why
    /// </summary>
    /// <param name="id">The id of the account</param>
    /// <param name="amount">How much to take out</param>
    /// <param name="description">A note saying why the money went out</param>
    /// <returns>True if the money was taken out, false if it was not</returns>
    public bool Withdraw(int id, decimal amount, string description = "Withdrawal")
    {
        try
        {
            // Withdraw raises before touching the balance when there isn't enough money, so no
            // transaction is recorded for a withdrawal that never happened.
            _accountService.Withdraw(id, amount);
            _transactionService.Record(id, amount, TransactionType.Debit, description);

            return true;
        }
        catch (Exception ex)
        {
            _output.WriteLine(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Renames an account's owner<br/>
    /// PreCondition: None<br/>
    /// PostCondition: The account's username is the new one, and its balance, id and history are
    /// untouched. If the account does not exist or the new username is blank, nothing changes and a
    /// message says why
    /// </summary>
    /// <param name="id">The id of the account</param>
    /// <param name="newUsername">The new username</param>
    /// <returns>The updated account, or null if it could not be updated</returns>
    public Account? UpdateAccountName(int id, string newUsername)
    {
        try
        {
            return _accountService.UpdateUsername(id, newUsername);
        }
        catch (Exception ex)
        {
            return Report<Account>(ex);
        }
    }

    /// <summary>
    /// Closes an account and wipes its history with it, so nothing is left pointing at an account that
    /// no longer exists<br/>
    /// PreCondition: None<br/>
    /// PostCondition: Neither the account nor any of its transactions are in the database, and other
    /// accounts are untouched. If the account does not exist, nothing changes and a message says why
    /// </summary>
    /// <param name="id">The id of the account</param>
    /// <returns>The deleted account, or null if no account has that id</returns>
    public Account? DeleteAccount(int id)
    {
        try
        {
            Account deleted = _accountService.DeleteAccount(id);
            _transactionService.DeleteForAccount(id);

            return deleted;
        }
        catch (Exception ex)
        {
            return Report<Account>(ex);
        }
    }

    /// <summary>
    /// Shows one account's history, oldest first<br/>
    /// PreCondition: None<br/>
    /// PostCondition: Returns that account's transactions, or writes a message and returns null if no
    /// account has that id
    /// </summary>
    /// <param name="id">The id of the account</param>
    /// <returns>That account's transactions oldest first, or null if no account has that id</returns>
    public List<Transaction>? GetTransactionHistory(int id)
    {
        try
        {
            // Asking about a stranger's history is a mistake worth reporting, not an empty list.
            _accountService.GetAccount(id);

            return _transactionService.GetForAccount(id);
        }
        catch (Exception ex)
        {
            return Report<List<Transaction>>(ex);
        }
    }

    /// <summary>
    /// Shows the whole bank's history, oldest first<br/>
    /// PreCondition: None<br/>
    /// PostCondition: Returns every transaction, or an empty list if there are none
    /// </summary>
    /// <returns>Every transaction, oldest first</returns>
    public List<Transaction> GetAllTransactions() => _transactionService.GetAll();

    /// <summary>
    /// Writes out why an operation could not be carried out<br/>
    /// PreCondition: None<br/>
    /// PostCondition: The reason has been written to the output and null is returned
    /// </summary>
    /// <param name="ex">The rule that was broken</param>
    /// <returns>Null, so callers can return this straight back</returns>
    private T? Report<T>(Exception ex) where T : class
    {
        _output.WriteLine(ex.Message);
        return null;
    }
}*/
