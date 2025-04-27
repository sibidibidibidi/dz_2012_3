using System;

public class CreditCard
{
    public string cardnum { get; private set; }
    public string owner { get; private set; }
    public DateTime expirydate { get; private set; }
    public string pin { get; private set; }
    public decimal creditlimit { get; private set; }
    public decimal balance { get; private set; }

    public event Action<decimal> balanceup;
    public event Action<decimal> spendmoney;
    public event Action creditstart;
    public event Action limitup;
    public event Action<string> pinchanged;

    public CreditCard(string cardnum, string owner, DateTime expirydate, string pin, decimal creditlimit)
    {
        this.cardnum = cardnum;
        this.owner = owner;
        this.expirydate = expirydate;
        this.pin = pin;
        this.creditlimit = creditlimit;
        this.balance = 0;
    }

    public void topup(decimal amount)
    {
        if (amount > 0)
        {
            balance += amount;
            balanceup?.Invoke(amount);
        }
    }

    public void spend(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Сума має бути додатньою");
            return;
        }

        if (balance >= amount)
        {
            balance -= amount;
            spendmoney?.Invoke(amount);
        }
        else if (balance + creditlimit >= amount)
        {
            creditstart?.Invoke();
            balance -= amount;
            spendmoney?.Invoke(amount);

            if (Math.Abs(balance) >= creditlimit)
            {
                limitup?.Invoke();
            }
        }
        else
        {
            Console.WriteLine("Недостатньо коштів");
        }
    }

    public void changepin(string newpin)
    {
        if (!string.IsNullOrWhiteSpace(newpin) && newpin != pin)
        {
            pin = newpin;
            pinchanged?.Invoke(newpin);
        }
    }
}
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Створення кредитної картки");
        Console.Write("Введіть номер картки: ");
        string cardnum = Console.ReadLine();
        Console.Write("Введіть ПІБ власника: ");
        string owner = Console.ReadLine();
        Console.Write("Введіть термін дії (рік-місяць-день): ");
        DateTime expirydate = DateTime.Parse(Console.ReadLine());
        Console.Write("Введіть початковий PIN: ");
        string pin = Console.ReadLine();
        Console.Write("Введіть кредитний ліміт: ");
        decimal creditlimit = decimal.Parse(Console.ReadLine());
        CreditCard card = new CreditCard(cardnum, owner, expirydate, pin, creditlimit);

        card.balanceup += amount => Console.WriteLine($"[Подія] Поповнення на: {amount} грн");
        card.spendmoney += amount => Console.WriteLine($"[Подія] Витрачено: {amount} грн");
        card.creditstart += () => Console.WriteLine("[Подія] Почато використання кредитних коштів");
        card.limitup += () => Console.WriteLine("[Подія] Досягнуто кредитного ліміту!");
        card.pinchanged += newpin => Console.WriteLine($"[Подія] PIN-код змінено на: {newpin}");

        Console.WriteLine("Операції з рахунком ");
        Console.Write("Поповнити рахунок на: ");
        decimal topupamount = decimal.Parse(Console.ReadLine());
        card.topup(topupamount);
        Console.Write("Списати з рахунку: ");
        decimal spendamount = decimal.Parse(Console.ReadLine());
        card.spend(spendamount);
        Console.Write("Бажаєте змінити PIN? (так/ні): ");
        string pinanswer = Console.ReadLine().ToLower();
        if (pinanswer == "так")
        {
            Console.Write("Введіть новий PIN: ");
            string newpin = Console.ReadLine();
            card.changepin(newpin);
        }

        Console.WriteLine("Готово");
        Console.ReadLine();
    }
}

