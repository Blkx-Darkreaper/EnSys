-- Run command: source public_html/scripts/buildingGoodsTable.sql
-- BuildingGoods(P building Id, P good Name, P good Quality, good Quantity, reserved, good Price, for Sale, good Sales)

Drop Table IF EXISTS buildingGoods;
CREATE TABLE buildingGoods
(buildingId integer NOT NULL,
goodName varchar(20) NOT NULL,
goodQuality varchar(12) NOT NULL default 'good',
Primary key (buildingId, goodName, goodQuality),
Foreign key (buildingId) references buildings(buildingId) On Delete Cascade,
goodQuantity INTEGER NOT NULL default 0,
reserved integer not null default 0,
goodPrice integer NOT NULL default 0,
forSale char(1) not null default 'n',
goodSales integer NOT NULL default 0);