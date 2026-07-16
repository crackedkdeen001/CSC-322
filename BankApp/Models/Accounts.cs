using BankApp.interfaces;

namespace BankApp.Models;


// Todo: Finish bank app by implementing Account, Transaction, FileHandler

/// <summary>
/// An object modelling the account class
/// </summary>
/// <param name="username"></param>
/// <param name="balance"></param>
public class Account(string username, string balance): IHasID
{
    /// <summary>
    /// The unique identifier of the account
    /// </summary>
    public int Id { get;  set; }
    
    /// <summary>
    /// The username of the owner of the account
    /// </summary>
    public string Username { get; private set; }
    
    /// <summary>
    /// The amount of money an account currently has
    /// </summary>
    public int Balance { get; private set; }
}