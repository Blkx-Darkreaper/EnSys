-- Run command: source public_html/scripts/orderBidsTable.sql
-- OrderBids(P bid Id, order Id, bid Value, bidder, recipient)

Drop Table IF EXISTS orderBids;
CREATE TABLE orderBids
(bidId integer NOT NULL Primary key unique AUTO_INCREMENT,
orderId integer NOT NULL,
Foreign key (orderId) references orders(orderId) on delete cascade,
bidValue INTEGER NOT NULL default 0,
bidder varchar(20) not null,
Foreign key (bidder) references users(username) on delete cascade,
recipient varchar(20) not null,
Foreign key (recipient) references buildings(buildingId) on delete cascade
);