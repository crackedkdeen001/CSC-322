using BankApp.models;

namespace BankApp.repository;

/// <summary>
/// A repository used for interacting with the database of the accounts
/// </summary>
public class AccountRepository(string fileName) : JsonRepository<Account>(fileName)
{
}
