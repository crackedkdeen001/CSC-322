using BankApp.services;

var accountService = new AccountService("idk.json");
List<string> names = ["kamal", "tayo", "grand", "joseph", "mark"];


for (int i = 0; i < 5; i++)
{
    int randomIndex = Random.Shared.Next(names.Count);
    accountService.CreateAccount(names[randomIndex], 20m);
}
accountService.ListAccounts();
accountService.Clear();
accountService.ListAccounts();
