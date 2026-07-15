namespace BankApp.Models;


// Todo: Finish bank app by implementing Account, Transaction, FileHandler
public class Account(string username, string balance)
{
    public int AccountId { get; private set; }
    public string Username { get; private set; }
    public int Balance { get; private set; }
}