-- Run command: source public_html/scripts/structuresTable.sql
-- Structures(P structure Type, size, max Workers, storageSpace)

Drop Table IF EXISTS structures;
CREATE TABLE structures
(structureType varchar(20) NOT NULL PRIMARY KEY,
Foreign key (structureType) references buildings(structureType),
structureSize INTEGER NOT NULL,
maxWorkers integer not null,
storageSpace integer not null,
structureCost integer);

Insert into structures
(structureType, structureSize, maxWorkers, storageSpace, structureCost)
Values
('Shop', 1, 5, 50, 200);

Insert into structures
(structureType, structureSize, maxWorkers, storageSpace, structureCost)
Values
('Store', 2, 10, 100, 400);

Insert into structures
(structureType, structureSize, maxWorkers, storageSpace, structureCost)
Values
('Warehouse', 5, 10, 500, 2000);