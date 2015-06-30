-- Run command: source public_html/scripts/buildingReportsTable.sql
-- BuildingReports(P building Id, P businessDay, daily Profits, daily Costs)

Drop Table IF EXISTS buildingReports;
CREATE TABLE buildingReports
(buildingId integer not null,
businessDay date not null,
Primary Key (buildingId, businessDay),
dailyProfits decimal(10, 2) not null default 0,
dailyCosts decimal(10, 2) not null default 0);