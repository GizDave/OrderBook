using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Orderbook.ServiceLayer;
using Orderbook.Entities;

namespace Orderbook.PresentationLayer
{
    class ApplicationConsole
    {
        private Trade trans;
        private ClientInformation profile;

        public ApplicationConsole(Trade trans, ClientInformation profile)
        {
            this.trans = trans;
            this.profile = profile;
        }

        public void Run()
        {
            Menu();

            while (true)
            {
                switch(GetMenuOption())
                {
                    case 1:
                        Trade(OrderType.Buy);
                        break;
                    case 2:
                        Trade(OrderType.Sell);
                        break;
                    case 3:
                        Portfolio();
                        break;
                    case 4:
                        TransactionHistory();
                        break;
                    case 5:
                        return;
                    default:
                        Console.WriteLine("Enter a valid number between 1 and 5, inclusively.");
                        break;
                }

                Console.WriteLine(" ");
            }
        }

        private void Menu()
        {
            Console.WriteLine(
@"************************************************************
* Menu Options:
* 1) Buy shares
* 2) Sell shares
* 3) Get your portfolio summary
* 4) Get all transaction history
* 5) Exit
*
* Choose the corresponding number for your desired action.
************************************************************"
            );
        }

        private int GetMenuOption()
        {
            int.TryParse(Console.ReadLine(), out int user);
            return user;
        }

        private void Trade(OrderType Type)
        {
            string Ticker = "";
            int Amount = 0;
            decimal Price = 0;

            if (GetTransactionDetail(ref Ticker, ref Amount, ref Price))
            {
                Order o = new Order();
                o.Ticker = Ticker;
                o.Amount = Amount;
                o.Price = Price;
                o.Type = Type;

                bool IsSubmitted = trans.AddOrder(o);

                if (IsSubmitted)
                {
                    Console.WriteLine($"Order submitted.");
                }
                else
                {
                    Console.WriteLine("Order submission failed.");
                }
            }
        }

        private bool GetTransactionDetail(ref string ticker, ref int amount, ref decimal price)
        {
            string input;

            Console.Write("Stock symbol: ");
            ticker = Console.ReadLine();

            if (string.IsNullOrEmpty(ticker))
            {
                Console.WriteLine("Symbol cannot be null or empty.");
                return false;
            }

            Console.Write("Number of shares: ");
            input = Console.ReadLine();

            if (int.TryParse(input, out amount))
            {
                if (amount <= 0)
                {
                    Console.WriteLine("Number of shares cannot be fewer than 1.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Enter a valid Number.");
                return false;
            }

            Console.Write("Price: ");
            input = Console.ReadLine();

            if (decimal.TryParse(input, out price))
            {
                if (price < Convert.ToDecimal(0.01))
                {
                    Console.WriteLine("Price cannot be less than 0.01.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Enter a valid price.");
                return false;
            }

            return true;
        }

        private void Portfolio()
        {
            Portfolio p = profile.GetPortfolio();

            Console.WriteLine("Portfolio:");
            foreach (KeyValuePair<string, int> kvp in p.Equities)
            {
                Console.WriteLine($"{kvp.Value} shares of {kvp.Key}");
            }
            Console.WriteLine("-END-");
        }

        private void TransactionHistory()
        {
            TransactionHistory th = profile.GetTransactionHistory();

            Console.WriteLine("TransactionHistory:");
            foreach (Transaction t in th.TransactionBook)
            {
                Console.WriteLine(t);
            }
            Console.WriteLine("-END-");
        }
    }
}