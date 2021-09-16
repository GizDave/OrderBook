use OrderbookDB;

insert into Client (FirstName, LastName, Balance)
values 
("Vendor", "1", 1000000),
("James", "Smith", 1000),
("Maria", "Smith", 1000),
("Mary", "Smith", 2000);

insert into Portfolio (ClientID, Ticker, UnitCost, Amount)
values
(1, "AMZN", 2, 1000),
(1, "AAPL", 4, 1000),
(1, "FB", 6, 1000),
(1, "GOOG", 8, 1000),
(2, "AMZN", 2, 20),
(2, "AAPL", 4, 40),
(3, "AMZN", 6, 60);

select * from TransactionHistory;
select * from Portfolio;
select * from Client;