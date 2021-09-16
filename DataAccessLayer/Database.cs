using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using Orderbook.Entities;

namespace Orderbook.DataAccessLayer
{
    class Database
    {
        private string _dbConnStr;

        public Database(string url)
        {
            _dbConnStr = url;
        }

        internal bool HasClient(int clientID)
        {
            using (MySqlConnection connection = new MySqlConnection(_dbConnStr))
            {
                connection.Open();

                var sql = $"select count(ClientID) from Client where ClientID = {clientID}";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                var result = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();

                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        internal bool HasEnoughBalance(int clientID, decimal requirement)
        {
            using (MySqlConnection connection = new MySqlConnection(_dbConnStr))
            {
                connection.Open();

                var sql = $"select case when Balance >= {requirement} then true else false end from Client where ClientID = {clientID}";
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                bool result = false;
                if (cmd != null)
                {
                    result = Convert.ToBoolean(cmd.ExecuteScalar());
                }

                connection.Close();

                return result;
            }
        }

        internal bool HasEnoughShares(int clientID, string ticker, int requirement)
        {
            using (MySqlConnection connection = new MySqlConnection(_dbConnStr))
            {
                connection.Open();

                var sql = $"select case when Amount >= {requirement} then true else false end from Portfolio where ClientID = {clientID} and Ticker = '{ticker}'";
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                bool result = false;
                if (cmd != null)
                {
                    result = Convert.ToBoolean(cmd.ExecuteScalar());
                }

                connection.Close();

                return result;
            }
        }
        
        internal void ChangeBalance(int clientID, decimal change)
        {
            using (MySqlConnection connection = new MySqlConnection(_dbConnStr))
            {
                connection.Open();

                var sql = $"update Client set Balance = Balance + {change} where ClientID = {clientID}";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        internal void TransferBalance(int senderID, int receiverID, decimal balance)
        {
            ChangeBalance(senderID, balance*-1);
            ChangeBalance(receiverID, balance);
        }

        internal void TransferShares(int buyerID, int sellerID, string ticker, int amount, decimal price)
        {
            using (MySqlConnection connection = new MySqlConnection(_dbConnStr))
            {
                connection.Open();

                var sql = $"insert into Portfolio (ClientID, Ticker, UnitCost, Amount) values ({buyerID}, '{ticker}', {price}, {amount}) on duplicate key update UnitCost = (((UnitCost * Amount) + (({amount} * {price})) / (Amount + {amount}))), Amount = Amount + {amount};" +
                          $"update Portfolio set Amount = Amount - {amount} where ClientID = {sellerID} and Ticker = '{ticker}';" +
                          $"delete from Portfolio where ClientID = {sellerID} and Ticker = '{ticker}' and Amount = 0;" +
                          $"insert into TransactionHistory(BuyerID, SellerID, Ticker, Amount, Price) values ({buyerID}, {sellerID}, '{ticker}', {amount}, {price})";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        internal Portfolio GetPortfolio(int clientID)
        {
            using (MySqlConnection connection = new MySqlConnection(_dbConnStr))
            {
                connection.Open();

                Portfolio p = new Portfolio();

                var sql1 = $"select concat(FirstName, ' ', LastName) from Client where ClientID = {clientID}";
                MySqlCommand cmd1 = new MySqlCommand(sql1, connection);
                object result = cmd1.ExecuteScalar();

                if (result != null)
                {
                    p.ClientName = Convert.ToString(result);
                }
                else
                {
                    return null;
                }

                var sql2 = $"select Ticker, Amount from Portfolio where ClientID = {clientID}";
                MySqlCommand cmd2 = new MySqlCommand(sql2, connection);
                MySqlDataReader rdr = cmd2.ExecuteReader();

                while (rdr.Read())
                {
                    p.Equities.Add(Convert.ToString(rdr[0]), Convert.ToInt32(rdr[1]));
                }

                rdr.Close();
                connection.Close();

                return p;
            }
        }

        internal TransactionHistory GetTransactionHistory()
        {
            using (MySqlConnection connection = new MySqlConnection(_dbConnStr))
            {
                connection.Open();

                TransactionHistory th = new TransactionHistory();

                var sql = $"select TransactionTime, concat(c1.FirstName, ' ', c1.LastName), concat(c2.FirstName, ' ', c2.LastName), Ticker, Amount, Price from TransactionHistory th inner join Client c1 on BuyerID = c1.ClientID inner join Client c2 on SellerID = c2.ClientID";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Transaction t = new Transaction();
                    t.TransactionTime = Convert.ToDateTime(rdr[0]);
                    t.BuyerName = Convert.ToString(rdr[1]);
                    t.SellerName = Convert.ToString(rdr[2]);
                    t.Ticker = Convert.ToString(rdr[3]);
                    t.Amount = Convert.ToInt32(rdr[4]);
                    t.Price = Convert.ToDecimal(rdr[5]);
                    th.Add(t);
                }

                rdr.Close();
                connection.Close();

                return th;
            }
        }
    }
}
