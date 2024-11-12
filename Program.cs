using System;
using System.Collections.Generic;
using System.Linq;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public List<Account> Accounts { get; set; } = new List<Account>();

    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }
}

public class Account
{
    public int AccountNumber { get; set; }
    public string AccountHolderName { get; set; }
    public string AccountType { get; set; }
    public double InitialDeposit { get; set; }
    public double Balance { get; set; }
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();

    public Account(int accountNumber, string accountHolderName, string accountType, double initialDeposit)
    {
        AccountNumber = accountNumber;
        AccountHolderName = accountHolderName;
        AccountType = accountType;
        InitialDeposit = initialDeposit;
        Balance = initialDeposit;
    }

    public void Deposit(double amount)
    {
        Balance += amount;
        Transactions.Add(new Transaction("Deposit", amount));
        Console.WriteLine($"Deposit of ${amount} successful.");
    }

    public bool Withdraw(double amount)
    {
        if (amount > Balance)
        {
            Console.WriteLine("Insufficient funds.");
            return false;
        }
        Balance -= amount;
        Transactions.Add(new Transaction("Withdrawal", amount));
        Console.WriteLine($"Withdrawal of ${amount} successful.");
        return true;
    }

    public void GenerateStatement()
    {
        Console.WriteLine($"\n--- Statement for Account: {AccountNumber} ---");
        Console.WriteLine($"Account Holder: {AccountHolderName}");
        Console.WriteLine($"Account Type: {AccountType}");
        Console.WriteLine($"Initial Deposit: ${InitialDeposit}");
        Console.WriteLine("Transaction History:");
        foreach (var transaction in Transactions)
        {
            Console.WriteLine($"{transaction.Date} - {transaction.Type}: ${transaction.Amount}");
        }
        Console.WriteLine($"Current Balance: ${Balance}");
        Console.WriteLine("-------------------------------\n");
    }
}

public class Transaction
{
    public string Type { get; set; }
    public double Amount { get; set; }
    public DateTime Date { get; set; }

    public Transaction(string type, double amount)
    {
        Type = type;
        Amount = amount;
        Date = DateTime.Now;
    }
}

public class BankingApp
{
    private List<User> users = new List<User>();
    private User loggedInUser = null;
    private const double InterestRate = 0.02; 

    public void Register(string username, string password)
    {
        if (users.Any(u => u.Username == username))
        {
            Console.WriteLine("Username already exists.");
            return;
        }
        users.Add(new User(username, password));
        Console.WriteLine("Registration successful.");
    }

    public bool Login(string username, string password)
    {
        loggedInUser = users.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (loggedInUser == null)
        {
            Console.WriteLine("Invalid login credentials.");
            return false;
        }
        Console.WriteLine("Login successful.");
        return true;
    }

    public void OpenAccount(string accountHolderName, string accountType, double initialDeposit)
    {
        if (loggedInUser == null)
        {
            Console.WriteLine("Please login to open an account.");
            return;
        }

        if (accountHolderName != loggedInUser.Username)
        {
            Console.WriteLine("Account holder name does not match the logged-in user.");
            return;
        }

        int accountNumber = new Random().Next(100000, 999999);
        var newAccount = new Account(accountNumber, accountHolderName, accountType, initialDeposit);
        loggedInUser.Accounts.Add(newAccount);
        Console.WriteLine($"Account created successfully. Account Number: {accountNumber}");
    }

    public void Deposit(int accountNumber, double amount)
    {
        var account = GetAccount(accountNumber);
        if (account != null)
        {
            account.Deposit(amount);
        }
    }

    public void Withdraw(int accountNumber, double amount)
    {
        var account = GetAccount(accountNumber);
        if (account != null)
        {
            account.Withdraw(amount);
        }
    }

    public void ViewStatement(int accountNumber)
    {
        var account = GetAccount(accountNumber);
        if (account != null)
        {
            account.GenerateStatement();
        }
    }

    public void CheckBalance(int accountNumber)
    {
        var account = GetAccount(accountNumber);
        if (account != null)
        {
            Console.WriteLine($"Account Type: {account.AccountType}");
            Console.WriteLine($"Account Balance: ${account.Balance}");
        }
    }

    public void ApplyMonthlyInterest()
    {
        if (loggedInUser == null)
        {
            Console.WriteLine("Please log in to apply interest.");
            return;
        }

        bool interestApplied = false;
        foreach (var account in loggedInUser.Accounts)
        {
            double interest = account.Balance * InterestRate;
            account.Deposit(interest);
            Console.WriteLine($"Interest of ${interest} added to account {account.AccountNumber}");
            interestApplied = true;
        }

        if (!interestApplied)
        {
            Console.WriteLine("No accounts available for interest application.");
        }
    }

    private Account GetAccount(int accountNumber)
    {
        var account = loggedInUser?.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        if (account == null)
        {
            Console.WriteLine("Account not found.");
        }
        return account;
    }
}

public class Program
{
    static void Main(string[] args)
    {
        var app = new BankingApp();

        while (true)
        {
            Console.WriteLine("\n--- Banking Application ---");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Open Account");
            Console.WriteLine("4. Deposit");
            Console.WriteLine("5. Withdraw");
            Console.WriteLine("6. View Statement");
            Console.WriteLine("7. Apply Interest");
            Console.WriteLine("8. Check Balance");
            Console.WriteLine("9. Exit");

            Console.Write("Choose an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter username: ");
                    var username = Console.ReadLine();
                    Console.Write("Enter password: ");
                    var password = Console.ReadLine();
                    app.Register(username, password);
                    break;

                case "2":
                    Console.Write("Enter username: ");
                    username = Console.ReadLine();
                    Console.Write("Enter password: ");
                    password = Console.ReadLine();
                    app.Login(username, password);
                    break;

                case "3":
                    Console.Write("Enter account holder name: ");
                    var holderName = Console.ReadLine();
                    Console.Write("Enter account type (Savings/Checking): ");
                    var type = Console.ReadLine();
                    Console.Write("Enter initial deposit: ");
                    if (double.TryParse(Console.ReadLine(), out var initialDeposit))
                    {
                        app.OpenAccount(holderName, type, initialDeposit);
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount entered.");
                    }
                    break;

                case "4":
                    Console.Write("Enter account number: ");
                    if (int.TryParse(Console.ReadLine(), out var accountNumberDeposit))
                    {
                        Console.Write("Enter amount to deposit: ");
                        if (double.TryParse(Console.ReadLine(), out var depositAmount))
                        {
                            app.Deposit(accountNumberDeposit, depositAmount);
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount entered.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid account number.");
                    }
                    break;

                case "5":
                    Console.Write("Enter account number: ");
                    if (int.TryParse(Console.ReadLine(), out var accountNumberWithdraw))
                    {
                        Console.Write("Enter amount to withdraw: ");
                        if (double.TryParse(Console.ReadLine(), out var withdrawAmount))
                        {
                            app.Withdraw(accountNumberWithdraw, withdrawAmount);
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount entered.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid account number.");
                    }
                    break;

                case "6":
                    Console.Write("Enter account number: ");
                    if (int.TryParse(Console.ReadLine(), out var accountNumberStatement))
                    {
                        app.ViewStatement(accountNumberStatement);
                    }
                    else
                    {
                        Console.WriteLine("Invalid account number.");
                    }
                    break;

                case "7":
                    app.ApplyMonthlyInterest();
                    break;

                case "8":
                    Console.Write("Enter account number: ");
                    if (int.TryParse(Console.ReadLine(), out var accountNumberBalance))
                    {
                        app.CheckBalance(accountNumberBalance);
                    }
                    else
                    {
                        Console.WriteLine("Invalid account number.");
                    }
                    break;

                case "9":
                    Console.WriteLine("Exiting application.");
                    return;

                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}
