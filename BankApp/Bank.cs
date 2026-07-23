using BankApp.models;
using BankApp.services;

namespace BankApp;

/// <summary>
/// The face of the bank and the only type the program talks to. It drives the account and transaction
/// services together so that money never moves without the history recording it: every deposit, withdrawal
/// and opening balance leaves a matching transaction behind.
/// </summary>
/// <remarks>
/// The bank does not catch anything. When a rule is broken the underlying service throws, and the caller
/// (the menu in Program.cs) is the one place that turns those exceptions into messages.
/// </remarks>
/// <param name="accountsFile">The name of the JSON file the accounts are stored in</param>
/// <param name="transactionsFile">The name of the JSON file the transactions are stored in</param>
public class Bank(string accountsFile, string transactionsFile)
{
    private readonly AccountService _accounts = new(accountsFile);
    private readonly TransactionService _transactions = new(transactionsFile);

    /// <summary>
    /// requires: none<br/>
    /// modifies: the account and transaction databases<br/>
    /// effects: opens a new account and returns it; if the opening balance is positive, records it as the
    ///          account's first credit; throws ArgumentException if the username is blank, or
    ///          InvalidAmountException if the opening balance is negative
    /// </summary>
    /// <param name="username">The name of the account owner</param>
    /// <param name="openingBalance">The amount the account starts with</param>
    /// <returns>The newly opened account</returns>
    public Account CreateAccount(string username, decimal openingBalance = 0m)
    {
        var account = _accounts.CreateAccount(username, openingBalance);

        if (openingBalance > 0m)
        {
            _transactions.CreateTransaction(account.Id, openingBalance, TransactionType.Credit, "Opening balance");
        }

        return account;
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: nothing<br/>
    /// effects: returns the balance of the account with the given id; throws AccountNotFoundException if no
    ///          account has that id
    /// </summary>
    /// <param name="id">The id of the account</param>
    /// <returns>The account's current balance</returns>
    public decimal GetBalance(int id) => _accounts.GetAccount(id).Balance;

    /// <summary>
    /// requires: none<br/>
    /// modifies: nothing<br/>
    /// effects: returns the account with the given id; throws AccountNotFoundException if no account has that id
    /// </summary>
    /// <param name="id">The id of the account</param>
    /// <returns>The account with the given id</returns>
    public Account GetAccount(int id) => _accounts.GetAccount(id);

    /// <summary>
    /// requires: none<br/>
    /// modifies: nothing<br/>
    /// effects: returns every account in the bank, or an empty list if there are none
    /// </summary>
    /// <returns>All accounts</returns>
    public List<Account> ListAccounts() => _accounts.ListAccounts();

    /// <summary>
    /// requires: none<br/>
    /// modifies: the balance of the account with the given id, and the transaction database<br/>
    /// effects: pays amount into the account, records a matching credit and returns the balance before and
    ///          after; throws AccountNotFoundException if no account has that id, or InvalidAmountException if
    ///          amount is not positive. Nothing is recorded if the deposit is rejected
    /// </summary>
    /// <param name="id">The id of the account</param>
    /// <param name="amount">How much to pay in</param>
    /// <param name="description">A note saying why the money came in</param>
    /// <returns>The balance before and after the deposit</returns>
    public (decimal prevBalance, decimal newBalance) Deposit(int id, decimal amount, string description = "Deposit")
    {
        var balances = _accounts.Deposit(id, amount);
        _transactions.CreateTransaction(id, amount, TransactionType.Credit, description);

        return balances;
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: the balance of the account with the given id, and the transaction database<br/>
    /// effects: takes amount out of the account, records a matching debit and returns the balance before and
    ///          after; throws AccountNotFoundException if no account has that id, InvalidAmountException if
    ///          amount is not positive, or InsufficientFundsException if there is not enough money. Nothing is
    ///          recorded if the withdrawal is rejected
    /// </summary>
    /// <param name="id">The id of the account</param>
    /// <param name="amount">How much to take out</param>
    /// <param name="description">A note saying why the money went out</param>
    /// <returns>The balance before and after the withdrawal</returns>
    public (decimal prevBalance, decimal newBalance) Withdraw(int id, decimal amount, string description = "Withdrawal")
    {
        // The service throws before touching the balance when the amount is bad or the funds are short, so
        // reaching the line below means the money really moved and the debit belongs in the history.
        var balances = _accounts.Withdraw(id, amount);
        _transactions.CreateTransaction(id, amount, TransactionType.Debit, description);

        return balances;
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: the account with the given id<br/>
    /// effects: renames the account's owner and returns the updated account; throws ArgumentException if the
    ///          new name is blank, or AccountNotFoundException if no account has that id
    /// </summary>
    /// <param name="id">The id of the account</param>
    /// <param name="newName">The new username</param>
    /// <returns>The updated account</returns>
    public Account UpdateAccountName(int id, string newName)
    {
        _accounts.UpdateAccountUsername(id, newName);

        return _accounts.GetAccount(id);
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: the account and transaction databases<br/>
    /// effects: closes the account and deletes its transactions with it, then returns the closed account;
    ///          throws AccountNotFoundException if no account has that id
    /// </summary>
    /// <param name="id">The id of the account</param>
    /// <returns>The deleted account</returns>
    public Account DeleteAccount(int id)
    {
        var deleted = _accounts.DeleteAccount(id);
        _transactions.DeleteTransactions(id);

        return deleted;
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: nothing<br/>
    /// effects: returns the account's transactions oldest first; throws AccountNotFoundException if no account
    ///          has that id
    /// </summary>
    /// <param name="id">The id of the account</param>
    /// <returns>That account's transactions, oldest first</returns>
    public List<Transaction> GetAccountTransactions(int id)
    {
        // Asking about a stranger's history is a mistake worth reporting, not an empty list, so make sure the
        // account exists before reaching for its transactions.
        _accounts.GetAccount(id);

        return _transactions.GetAllTransactionsByAccount(id);
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: nothing<br/>
    /// effects: returns every transaction in the bank, or an empty list if there are none
    /// </summary>
    /// <returns>Every transaction in the bank</returns>
    public List<Transaction> GetAllTransactions() => _transactions.GetAllTransactions();
}
