-- Run command: source public_html/scripts/orderItemsTable.sql
-- OrderItems(P order Id, P item, P item Quality, item Quantity)

Drop Table IF EXISTS orderItems;
CREATE TABLE orderItems
(orderId integer not null,
item varchar(20) not null,
itemQuality varchar(20) not null,
Primary key (orderId, item, itemQuality),
Foreign key (orderId) references orders(orderId) on delete cascade,
Foreign key (item) references goods(goodName),
itemQuantity integer not null);