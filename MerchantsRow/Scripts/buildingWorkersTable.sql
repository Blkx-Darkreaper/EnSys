-- Run command: source public_html/scripts/buildingWorkersTable.sql
-- BuildingWorkers(P building Id, P worker Id, current Task, workHours)

Drop Table IF EXISTS buildingWorkers;
CREATE TABLE buildingWorkers
(buildingId integer not null,
workerId integer not null unique,
Primary key (buildingId, workerId),
Foreign key (buildingId) references buildings(buildingId)  On Delete Cascade,
currentTask varchar(40),
Foreign key (currentTask) references skills(activeSkill),
workHours decimal(10,2) NOT NULL default 0);