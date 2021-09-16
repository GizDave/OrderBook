using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Orderbook.PresentationLayer;
using Orderbook.ServiceLayer;
using Orderbook.DataAccessLayer;
using Orderbook.Test;

namespace Orderbook
{
    class Program
    {
        static void Main(string[] args)
        {
            string connStr = "server=localhost;user=root;database=OrderbookDB;port=3306;password=00000000";            
            
            // Clients 1 and 2 for testing transactions and Client 5 for exception handling.
            int[] clientPool = { 1, 2, 5 };

            var conn = new Database(connStr);
            var book = new OrderBook(conn);

            foreach (int clientID in clientPool)
            {
                if (conn.HasClient(clientID))
                {
                    Trade trans = new Trade(clientID, book);
                    ClientInformation profile = new ClientInformation(clientID, conn);

                    var console = new ApplicationConsole(trans, profile);
                    console.Run();

                    /*
                    var tbook = new TOrderBook(book);
                    tbook.Test(50);
                    */
                }
                else
                {
                    Console.WriteLine($"User {clientID} does not exist.");
                }
            }
        }
    }
}
