using System;
using System.Collections.Generic;

// ---- Core model ----
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// ---- Payment behavior via interface ----
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Mobile Money] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Crypto Wallet] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

// ---- Accounts ----
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
    }
}

public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance)
    {
    }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. Updated balance: {Balance:C}");
        }
    }
}

// ---- Application ----
public class FinanceApp
{
    private List<Transaction> _transactions = new List<Transaction>();

    public void Run()
    {
        var savings = new SavingsAccount("ACC-001", 1000m);

        var t1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 200m, "Utilities");
        var t3 = new Transaction(3, DateTime.Now, 75m, "Entertainment");

        ITransactionProcessor mobileMoney = new MobileMoneyProcessor();
        ITransactionProcessor bankTransfer = new BankTransferProcessor();
        ITransactionProcessor cryptoWallet = new CryptoWalletProcessor();

        mobileMoney.Process(t1);
        savings.ApplyTransaction(t1);

        bankTransfer.Process(t2);
        savings.ApplyTransaction(t2);

        cryptoWallet.Process(t3);
        savings.ApplyTransaction(t3);

        _transactions.Add(t1);
        _transactions.Add(t2);
        _transactions.Add(t3);
    }
}

class Program
{
    static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}
