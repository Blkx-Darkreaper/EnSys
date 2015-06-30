-- Run command: source public_html/scripts/workersTable.sql
-- Workers(P worker Id, worker Name, wage, morale)

Drop table IF EXISTS workers;
CREATE TABLE workers
(workerId integer NOT NULL PRIMARY KEY unique AUTO_INCREMENT,
Foreign key (workerId) references buildingWorkers(workerId),
workerName varchar(30) NOT NULL,
wage integer NOT NULL default 0,
morale decimal(3,1) not null default 1);