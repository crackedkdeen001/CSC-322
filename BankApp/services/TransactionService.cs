using BankApp.interfaces;
using BankApp.models;
using BankApp.exceptions;
using BankApp.repository;

namespace BankApp.services;

/// <summary>
/// The service layer of the transactions
/// </summary>
/// <param name="fileName"></param>
public class TransactionService(string fileName)
{
    private readonly IRepository<Transaction> _transactionRepo = new TransactionRepository(fileName);

    /// <summary>
    /// Creates a new transaction<br/>
    /// Precondition: None<br/>
    /// Postcondition: The number of transactions in the database increases by 1
    /// </summary>
    /// <param name="accountId">The account ID of the account initiating the transaction</param>
    /// <param name="amount">Money deposited or withdrawed from the account</param>
    /// <param name="description">A short note on the transaction</param>
    /// <param name="type">The type denoting if money was added (Credit) or removed (Debit) from the account</param>
    public void CreateTransaction(int accountId, decimal amount, string description, TransactionType type)
    {
        var transaction = new Transaction(accountId, amount, description, type);
        _transactionRepo.Add(transaction);
    }

    /// <summary>
    /// Returns a list of transactions that an account has performed<br/>
    /// Precondition: Account must exist<br/>
    /// Postcondition: None
    /// </summary>
    /// <param name="accountId">The ID of the account whose transactions we want to see</param>
    /// <returns>A list of the transactions the account has performed</returns>
    /// <exception cref="Acc"></exception>
    public List<Transaction> GetAllTransactionsByAccount(int accountId)
    {
    }
}