drop database if exists OrderbookDB;
create database OrderbookDB;
use OrderbookDB;

CREATE TABLE Client (
	ClientID int AUTO_INCREMENT, 
    FirstName VARCHAR(255) NOT NULL,    
    LastName VARCHAR(255) NOT NULL,
    Balance DECIMAL(65, 2) NOT NULL,
    PRIMARY KEY (ClientID)
);

CREATE TABLE Portfolio (
    ClientID INT,
    Ticker VARCHAR(4) NOT NULL,
    UnitCost DECIMAL(65, 2) NOT NULL,
    Amount int NOT NULL,
    PRIMARY KEY (ClientID, Ticker),
    FOREIGN KEY (ClientID)
        REFERENCES Client (ClientID)
);

CREATE TABLE TransactionHistory (
    TransactionID INT AUTO_INCREMENT,
    TransactionTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    BuyerID INT NOT NULL,
    SellerID INT NOT NULL,
    Ticker VARCHAR(4) NOT NULL,
    Amount INT NOT NULL,
    Price DECIMAL(65, 2) NOT NULL,
    PRIMARY KEY (TransactionID)
);