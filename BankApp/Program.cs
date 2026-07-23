using BankApp;
using BankApp.models;

// A single shared bank instance; every menu operation runs off it.
var bank = new Bank("accounts.json", "transactions.json");

Console.WriteLine("=== Welcome to the Bank ===");

var running = true;
while (running)
{
    PrintMenu();
    var choice = Console.ReadLine();

    // Every operation goes through this one try/catch: when a service throws (unknown account, bad amount,
    // insufficient funds, unparseable input) the message is printed and the loop keeps running.
    try
    {
        switch (choice)
        {
            case "1":
                var name = ReadText("Account name: ");
                var opening = ReadDecimal("Opening balance: ");
                var created = bank.CreateAccount(name, opening);
                Console.WriteLine($"Created {created}");
                break;

            case "2":
                var balanceId = ReadInt("Account id: ");
                Console.WriteLine($"Balance: {bank.GetBalance(balanceId):N2}");
                break;

            case "3":
                PrintTransactions(bank.GetAllTransactions());
                break;

            case "4":
                PrintAccounts(bank.ListAccounts());
                break;

            case "5":
                var depositId = ReadInt("Account id: ");
                var depositAmount = ReadDecimal("Amount to deposit: ");
                var (depositPrev, depositNew) = bank.Deposit(depositId, depositAmount);
                Console.WriteLine($"Deposited. Balance: {depositPrev:N2} -> {depositNew:N2}");
                break;

            case "6":
                var withdrawId = ReadInt("Account id: ");
                var withdrawAmount = ReadDecimal("Amount to withdraw: ");
                var (withdrawPrev, withdrawNew) = bank.Withdraw(withdrawId, withdrawAmount);
                Console.WriteLine($"Withdrew. Balance: {withdrawPrev:N2} -> {withdrawNew:N2}");
                break;

            case "7":
                var deleteId = ReadInt("Account id: ");
                var deleted = bank.DeleteAccount(deleteId);
                Console.WriteLine($"Deleted {deleted}");
                break;

            case "8":
                var updateId = ReadInt("Account id: ");
                var newName = ReadText("New name: ");
                var updated = bank.UpdateAccountName(updateId, newName);
                Console.WriteLine($"Updated {updated}");
                break;

            case "9":
                var historyId = ReadInt("Account id: ");
                PrintTransactions(bank.GetAccountTransactions(historyId));
                break;

            case "0":
                running = false;
                break;

            default:
                Console.WriteLine("Unknown option. Please choose a number from the menu.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }

    Console.WriteLine();
}

Console.WriteLine("Goodbye!");
return;

void PrintMenu()
{
    Console.WriteLine("""
        1. Create an account
        2. Display an account's balance
        3. Show all transactions
        4. List all accounts
        5. Deposit money
        6. Withdraw money
        7. Delete an account
        8. Update an account's name
        9. Show an account's transactions
        0. Exit
        """);
    Console.Write("Choose an option: ");
}

void PrintAccounts(List<Account> accounts)
{
    if (accounts.Count == 0)
    {
        Console.WriteLine("No accounts yet.");
        return;
    }

    foreach (var account in accounts)
    {
        Console.WriteLine(account);
    }
}

void PrintTransactions(List<Transaction> transactions)
{
    if (transactions.Count == 0)
    {
        Console.WriteLine("No transactions yet.");
        return;
    }

    foreach (var transaction in transactions)
    {
        Console.WriteLine(transaction);
    }
}

// Reads a line and parses it as a whole number, turning bad input into a message the main loop can print.
int ReadInt(string prompt)
{
    Console.Write(prompt);
    if (!int.TryParse(Console.ReadLine(), out var value))
    {
        throw new FormatException("Please enter a whole number.");
    }

    return value;
}

// Reads a line and parses it as a money amount, turning bad input into a message the main loop can print.
decimal ReadDecimal(string prompt)
{
    Console.Write(prompt);
    if (!decimal.TryParse(Console.ReadLine(), out var value))
    {
        throw new FormatException("Please enter a valid amount.");
    }

    return value;
}

string ReadText(string prompt)
{
    Console.Write(prompt);
    return Console.ReadLine() ?? "";
}