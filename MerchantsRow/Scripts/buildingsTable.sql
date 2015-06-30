-- Run command: source public_html/scripts/buildingsTable.sql
-- Buildings(P building Id, username, structure Type, district, buildingName)

Drop Table IF EXISTS buildings;
CREATE TABLE buildings
(buildingId integer NOT NULL PRIMARY KEY unique AUTO_INCREMENT,
foreign key (buildingId) references buildingReports(buildingId)  On Delete Cascade,
username varchar(20) not null,
structureType varchar(20) NOT NULL,
district varchar(30) NOT NULL,
buildingName varchar(40) default null unique);