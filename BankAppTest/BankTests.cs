using BankApp;
using BankApp.exceptions;
using BankApp.models;

namespace BankAppTest;

/// <summary>
/// Behaviour-level tests for the <see cref="Bank"/> facade, covering the nine features, the invariants that
/// hold money and history together, and the boundaries around amounts and unknown accounts.
/// </summary>
/// <remarks>
/// Each test gets its own <see cref="Bank"/> backed by two unique JSON files, and <see cref="Dispose"/>
/// deletes them afterwards, so tests never see each other's data. The bank throws (Program is what catches),
/// so failures are asserted with <see cref="Assert.Throws{T}"/>.
/// </remarks>
public class BankTests : IDisposable
{
    private readonly string _accountsFile = $"acc_{Guid.NewGuid():N}.json";
    private readonly string _transactionsFile = $"tx_{Guid.NewGuid():N}.json";
    private readonly Bank _bank;

    public BankTests()
    {
        _bank = new Bank(_accountsFile, _transactionsFile);
    }

    // JsonFileHandler resolves the files into a shared "database" directory next to the test output; clean up
    // both so the folder does not fill with leftovers from every run.
    public void Dispose()
    {
        foreach (var name in new[] { _accountsFile, _transactionsFile })
        {
            var path = Path.Combine(DatabaseDirectory(), name);
            if (File.Exists(path)) File.Delete(path);
        }
    }

    // Mirror of JsonFileHandler's own path resolution so the test can find (and delete) the files it created.
    private static string DatabaseDirectory() =>
        Path.Combine(Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName, "database");

    // --- Creating accounts ---

    [Fact]
    public void CreatedAccountShowsUpInTheAccountList()
    {
        var account = _bank.CreateAccount("Ada");

        Assert.Contains(_bank.ListAccounts(), a => a.Id == account.Id && a.Username == "Ada");
    }

    [Fact]
    public void AccountsGetSequentialIdsStartingAtOne()
    {
        var first = _bank.CreateAccount("Ada");
        var second = _bank.CreateAccount("Grace");

        Assert.Equal(1, first.Id);
        Assert.Equal(2, second.Id);
    }

    [Fact]
    public void CreatingAnAccountWithABlankNameThrows()
    {
        Assert.Throws<ArgumentException>(() => _bank.CreateAccount("   "));
    }

    [Fact]
    public void CreatingAnAccountWithANegativeOpeningBalanceThrows()
    {
        Assert.Throws<InvalidAmountException>(() => _bank.CreateAccount("Ada", -5m));
    }

    [Fact]
    public void OpeningWithMoneyRecordsExactlyOneCreditOfThatAmount()
    {
        var account = _bank.CreateAccount("Ada", 100m);

        var history = _bank.GetAccountTransactions(account.Id);
        var opening = Assert.Single(history);
        Assert.Equal(TransactionType.Credit, opening.Type);
        Assert.Equal(100m, opening.Amount);
    }

    [Fact]
    public void OpeningWithZeroRecordsNoTransaction()
    {
        var account = _bank.CreateAccount("Ada");

        Assert.Empty(_bank.GetAccountTransactions(account.Id));
    }

    // --- Depositing and withdrawing ---

    [Fact]
    public void DepositRaisesTheBalanceAndRecordsACredit()
    {
        var account = _bank.CreateAccount("Ada");

        var (prev, next) = _bank.Deposit(account.Id, 40m);

        Assert.Equal(0m, prev);
        Assert.Equal(40m, next);
        Assert.Equal(40m, _bank.GetBalance(account.Id));
        Assert.Contains(_bank.GetAccountTransactions(account.Id),
            t => t.Type == TransactionType.Credit && t.Amount == 40m);
    }

    [Fact]
    public void WithdrawLowersTheBalanceAndRecordsADebit()
    {
        var account = _bank.CreateAccount("Ada", 100m);

        var (prev, next) = _bank.Withdraw(account.Id, 30m);

        Assert.Equal(100m, prev);
        Assert.Equal(70m, next);
        Assert.Equal(70m, _bank.GetBalance(account.Id));
        Assert.Contains(_bank.GetAccountTransactions(account.Id),
            t => t.Type == TransactionType.Debit && t.Amount == 30m);
    }

    // --- Invariants ---

    [Fact]
    public void ARejectedWithdrawalLeavesTheBalanceUntouchedAndRecordsNothing()
    {
        var account = _bank.CreateAccount("Ada", 50m);
        int historyBefore = _bank.GetAccountTransactions(account.Id).Count;

        Assert.Throws<InsufficientFundsException>(() => _bank.Withdraw(account.Id, 50.01m));

        Assert.Equal(50m, _bank.GetBalance(account.Id));
        Assert.Equal(historyBefore, _bank.GetAccountTransactions(account.Id).Count);
    }

    [Fact]
    public void EveryBalanceChangeLeavesAMatchingTransaction()
    {
        var account = _bank.CreateAccount("Ada");

        _bank.Deposit(account.Id, 100m);
        _bank.Withdraw(account.Id, 25m);

        var history = _bank.GetAccountTransactions(account.Id);
        Assert.Equal(2, history.Count);
        Assert.Equal(75m, _bank.GetBalance(account.Id));
    }

    // --- Boundaries ---

    [Fact]
    public void WithdrawingTheExactBalanceSucceedsAndEmptiesTheAccount()
    {
        var account = _bank.CreateAccount("Ada", 100m);

        var (_, next) = _bank.Withdraw(account.Id, 100m);

        Assert.Equal(0m, next);
    }

    [Fact]
    public void WithdrawingAPennyMoreThanTheBalanceThrows()
    {
        var account = _bank.CreateAccount("Ada", 100m);

        Assert.Throws<InsufficientFundsException>(() => _bank.Withdraw(account.Id, 100.01m));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DepositingANonPositiveAmountThrows(decimal amount)
    {
        var account = _bank.CreateAccount("Ada");

        Assert.Throws<InvalidAmountException>(() => _bank.Deposit(account.Id, amount));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void WithdrawingANonPositiveAmountThrows(decimal amount)
    {
        var account = _bank.CreateAccount("Ada", 100m);

        Assert.Throws<InvalidAmountException>(() => _bank.Withdraw(account.Id, amount));
    }

    [Fact]
    public void OperatingOnAnUnknownAccountThrows()
    {
        Assert.Throws<AccountNotFoundException>(() => _bank.GetBalance(999));
        Assert.Throws<AccountNotFoundException>(() => _bank.Deposit(999, 10m));
        Assert.Throws<AccountNotFoundException>(() => _bank.Withdraw(999, 10m));
        Assert.Throws<AccountNotFoundException>(() => _bank.DeleteAccount(999));
        Assert.Throws<AccountNotFoundException>(() => _bank.GetAccountTransactions(999));
    }

    [Fact]
    public void AFreshBankReturnsEmptyListsNotNull()
    {
        Assert.Empty(_bank.ListAccounts());
        Assert.Empty(_bank.GetAllTransactions());
    }

    [Fact]
    public void TinyDecimalDepositsSumExactly()
    {
        var account = _bank.CreateAccount("Ada");

        _bank.Deposit(account.Id, 0.1m);
        _bank.Deposit(account.Id, 0.2m);

        Assert.Equal(0.3m, _bank.GetBalance(account.Id));
    }

    // --- Updating ---

    [Fact]
    public void UpdatingTheNameKeepsTheIdAndBalance()
    {
        var account = _bank.CreateAccount("Ada", 100m);

        var updated = _bank.UpdateAccountName(account.Id, "Ada Lovelace");

        Assert.Equal(account.Id, updated.Id);
        Assert.Equal("Ada Lovelace", updated.Username);
        Assert.Equal(100m, updated.Balance);
    }

    [Fact]
    public void UpdatingWithABlankNameThrows()
    {
        var account = _bank.CreateAccount("Ada");

        Assert.Throws<ArgumentException>(() => _bank.UpdateAccountName(account.Id, " "));
    }

    // --- Deleting ---

    [Fact]
    public void DeletingRemovesTheAccountAndItsTransactions()
    {
        var account = _bank.CreateAccount("Ada", 100m);
        _bank.Deposit(account.Id, 20m);

        _bank.DeleteAccount(account.Id);

        Assert.DoesNotContain(_bank.ListAccounts(), a => a.Id == account.Id);
        Assert.DoesNotContain(_bank.GetAllTransactions(), t => t.AccountId == account.Id);
    }

    [Fact]
    public void DeletingOneAccountLeavesAnotherAccountsTransactionsIntact()
    {
        var keep = _bank.CreateAccount("Ada", 100m);
        var drop = _bank.CreateAccount("Grace", 100m);
        _bank.Deposit(keep.Id, 10m);
        _bank.Deposit(drop.Id, 10m);

        _bank.DeleteAccount(drop.Id);

        Assert.All(_bank.GetAllTransactions(), t => Assert.Equal(keep.Id, t.AccountId));
        Assert.NotEmpty(_bank.GetAccountTransactions(keep.Id));
    }

    [Fact]
    public void NewAccountNeverCollidesWithALivingAccountAfterADelete()
    {
        // checks that each ID in the bank is unique
        var first = _bank.CreateAccount("Ada");
        var second = _bank.CreateAccount("Grace");
        var third = _bank.CreateAccount("Katherine");
        _bank.DeleteAccount(second.Id);

        var fourth = _bank.CreateAccount("Dorothy");

        var liveIds = _bank.ListAccounts().Select(a => a.Id).ToList();
        Assert.Equal(liveIds.Count, liveIds.Distinct().Count());
        Assert.DoesNotContain(fourth.Id, new[] { first.Id, third.Id });
    }

    // --- History across the whole bank ---

    [Fact]
    public void AccountHistoryReturnsOnlyThatAccountsTransactionsOldestFirst()
    {
        var ada = _bank.CreateAccount("Ada");
        var grace = _bank.CreateAccount("Grace");
        _bank.Deposit(ada.Id, 10m);
        _bank.Deposit(grace.Id, 99m);
        _bank.Withdraw(ada.Id, 5m);

        var history = _bank.GetAccountTransactions(ada.Id);

        Assert.All(history, t => Assert.Equal(ada.Id, t.AccountId));
        Assert.Equal(history.OrderBy(t => t.Time), history);
    }

    [Fact]
    public void WholeBankHistorySpansEveryAccount()
    {
        var ada = _bank.CreateAccount("Ada", 10m);
        var grace = _bank.CreateAccount("Grace", 20m);

        var all = _bank.GetAllTransactions();

        Assert.Contains(all, t => t.AccountId == ada.Id);
        Assert.Contains(all, t => t.AccountId == grace.Id);
    }
}
