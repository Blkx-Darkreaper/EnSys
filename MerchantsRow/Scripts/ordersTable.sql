-- Run command: source public_html/scripts/ordersTable.sql
-- Orders(P order Id, order Type, order Value, recipient, expiration Date)

Drop Table IF EXISTS orders;
CREATE TABLE orders
(orderId integer NOT NULL Primary key unique AUTO_INCREMENT,
orderType varchar(30) NOT NULL default 'Transfer', 
orderValue INTEGER NOT NULL default 0,
creator varchar(20) not null,
Foreign key (creator) references buildings(buildingId) on delete cascade,
recipient varchar(20),
Foreign key (recipient) references buildings(buildingId) on delete cascade,
creationDate Date not null,
expirationDate DATE NOT NULL);