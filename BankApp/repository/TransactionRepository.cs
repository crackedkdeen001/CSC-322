using BankApp.models;

namespace BankApp.repository;

/// <summary>
/// A repository for transactions, adding lookups and deletes scoped to a single account on top of the shared
/// storage logic in <see cref="JsonRepository{T}"/>.
/// </summary>
/// <param name="fileName">The name of the JSON file the transactions are stored in</param>
public class TransactionRepository(string fileName) : JsonRepository<Transaction>(fileName)
{
    /// <summary>
    /// requires: none<br/>
    /// modifies: nothing<br/>
    /// effects: returns the account's transactions oldest first, or an empty list if it has none
    /// </summary>
    /// <param name="accountId">The id of the account whose transactions are wanted</param>
    /// <returns>That account's transactions, oldest first</returns>
    public List<Transaction> GetByAccountId(int accountId) =>
        GetAll()
            .Where(transaction => transaction.AccountId == accountId)
            .OrderBy(transaction => transaction.Time)
            .ToList();

    /// <summary>
    /// requires: none<br/>
    /// modifies: the file<br/>
    /// effects: removes every transaction belonging to the account and returns how many were removed; leaves
    ///          the file unchanged if the account had none
    /// </summary>
    /// <param name="accountId">The id of the account whose transactions should go</param>
    /// <returns>The number of transactions removed</returns>
    public int DeleteByAccountId(int accountId)
    {
        var transactions = GetAll();
        int removed = transactions.RemoveAll(transaction => transaction.AccountId == accountId);

        if (removed > 0)
        {
            Save(transactions);
        }

        return removed;
    }
}