-- Run command: source public_html/scripts/workerBidsTable.sql
-- WorkerBids(P worker Id, bid Value, bidder, hiree, expirationDate)

Drop Table IF EXISTS workerBids;
CREATE TABLE workerBids
(workerId integer NOT NULL Primary key unique,
Foreign key (workerId) references workers(workerId) on delete cascade,
bidValue INTEGER NOT NULL default 0,
bidder varchar(20) not null,
Foreign key (bidder) references users(username) on delete cascade,
hiree integer not null,
Foreign key (hiree) references buildings(buildingId) on delete cascade,
expirationDate datetime not null
);