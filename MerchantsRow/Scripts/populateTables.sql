-- Run command: source public_html/scripts/populateTables.sql

-- Populate the tables with test data

-- Users(P username, password, dob, email, logged, money)
Insert into users
(username, password, dob, email, money, loggedin, loggedout)
Values
('Mayor', 'rosebud', '1970-1-01', 'email@web.net', 0, Now(), Now());

-- Buildings(P building Id, username, structure Type, district)
Insert into buildings
(username, structureType, district)
Values
('Mayor', 'Warehouse', 'Central district');

Insert into buildings
(username, structureType, district)
Values
('Lady3lle', 'Warehouse', 'Central district');

Insert into buildings
(username, structureType, district)
Values
('Mayor', 'Shop', 'Central district');

-- BuildingGoods(P building Id, P good Name, P good Quality, good Quantity, reserved, good Price, for Sale, good Sales)
Insert into buildingGoods
(buildingId, goodName, goodQuality, goodQuantity, goodPrice, forSale, goodSales)
Values
(1, 'stone', 'good', 10, 15, 'n', 0);

Insert into buildingGoods
(buildingId, goodName, goodQuality, goodQuantity, goodPrice, forSale, goodSales)
Values
(1, 'wood', 'good', 10, 8, 'n', 0);

Insert into buildingGoods
(buildingId, goodName, goodQuality, goodQuantity, goodPrice, forSale, goodSales)
Values
(1, 'fish', 'good', 10, 12, 'n', 0);

Insert into buildingGoods
(buildingId, goodName, goodQuality, goodQuantity, goodPrice, forSale, goodSales)
Values
(1, 'venison', 'good', 10, 19, 'n', 0);

Insert into buildingGoods
(buildingId, goodName, goodQuality, goodQuantity, goodPrice, forSale, goodSales)
Values
(1, 'grain', 'good', 10, 8, 'n', 0);

Insert into buildingGoods
(buildingId, goodName, goodQuality, goodQuantity, goodPrice, forSale, goodSales)
Values
(1, 'water', 'good', 10, 0, 'n', 0);

Insert into buildingGoods
(buildingId, goodName, goodQuality, goodQuantity, goodPrice, forSale, goodSales)
Values
(1, 'iron ore', 'good', 10, 12, 'n', 0);

Insert into buildingGoods
(buildingId, goodName, goodQuality, goodQuantity, goodPrice, forSale, goodSales)
Values
(1, 'gold ore', 'good', 10, 14, 'n', 0);

Insert into buildingGoods
(buildingId, goodName, goodQuality, goodQuantity, goodPrice, forSale, goodSales)
Values
(1, 'coal', 'good', 10, 10, 'n', 0);

Insert into buildingGoods
(buildingId, goodName, goodQuality, goodQuantity, goodPrice, forSale, goodSales)
Values
(1, 'wool', 'good', 10, 6, 'n', 0);

Insert into buildingGoods
(buildingId, goodName, goodQuality, goodQuantity, goodPrice, forSale, goodSales)
Values
(1, 'grapes', 'good', 10, 10, 'n', 0);

Insert into buildingGoods
(buildingId, goodName, goodQuality, goodQuantity, goodPrice, forSale, goodSales)
Values
(1, 'milk', 'good', 10, 12, 'n', 0);

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(1, 'lumber', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(1, 'charcoal', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(1, 'sausage', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(1, 'flour', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(1, 'iron', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(1, 'gold', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(1, 'fabric', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(1, 'wine', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(1, 'beer', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(1, 'cheese', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(1, 'carts', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(1, 'bread', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(1, 'tools', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(1, 'jewellery', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(1, 'clothes', 'good');

-- Workers(P workerId, workerName, wage, morale)
Insert into workers
(workerName)
Values
('Adam');

Insert into workers
(workerName)
Values
('Billy');

Insert into workers
(workerName)
Values
('Cathy');

Insert into workers
(workerName)
Values
('Dominic');

Insert into workers
(workerName)
Values
('Erin');

Insert into workers
(workerName, wage)
Values
('Billy-Joel', 5);

Insert into workers
(workerName, wage)
Values
('Bob Thornton', 2);

Insert into workers
(workerName, wage)
Values
('Billie Armstrong', 1);

Insert into workers
(workerName, wage)
Values
('Dan Reynolds', 1);

Insert into workers
(workerName, wage)
Values
('Will Beckett', 1);

-- BuildingWorkers(P building Id, P worker Id, current Task, workHours)
Insert into buildingWorkers
(buildingId, workerId, currentTask, workHours)
Values
(1, 1, 'Nothing',0.0);

Insert into buildingWorkers
(buildingId, workerId, currentTask, workHours)
Values
(1, 2, 'Nothing',0.0);

Insert into buildingWorkers
(buildingId, workerId, currentTask, workHours)
Values
(1, 3, 'Nothing',0.0);

Insert into buildingWorkers
(buildingId, workerId, currentTask, workHours)
Values
(1, 4, 'Nothing',0.0);

Insert into buildingWorkers
(buildingId, workerId, currentTask, workHours)
Values
(1, 5, 'Nothing',0.0);

Insert into buildingWorkers
(buildingId, workerId, currentTask, workHours)
Values
(2, 6, 'Cutting wood', 100);

Insert into buildingWorkers
(buildingId, workerId, currentTask, workHours)
Values
(2, 7, 'Picking grapes', 20);

Insert into buildingWorkers
(buildingId, workerId, currentTask, workHours)
Values
(2, 8, 'Nothing', 10);

Insert into buildingWorkers
(buildingId, workerId, currentTask, workHours)
Values
(2, 9, 'Nothing', 4);

Insert into buildingWorkers
(buildingId, workerId, currentTask, workHours)
Values
(2, 10, 'Nothing', 2);

-- WorkerSkills(P worker Id, P skill Name, skill Level, experience, next Level)
Insert into workerSkills
(workerId, skillName)
Values
(1, 'No action');

Insert into workerSkills
(workerId, skillName)
Values
(1, 'Quarrying');

Insert into workerSkills
(workerId, skillName)
Values
(1, 'Woodcutting');

Insert into workerSkills
(workerId, skillName)
Values
(1, 'Fishing');

Insert into workerSkills
(workerId, skillName)
Values
(1, 'Hunting');

Insert into workerSkills
(workerId, skillName)
Values
(1, 'Harvesting');

Insert into workerSkills
(workerId, skillName)
Values
(1, 'Water collecting');

Insert into workerSkills
(workerId, skillName)
Values
(1, 'Iron mining');

Insert into workerSkills
(workerId, skillName)
Values
(1, 'Gold mining');

Insert into workerSkills
(workerId, skillName)
Values
(1, 'Coal mining');

Insert into workerSkills
(workerId, skillName)
Values
(1, 'Shearing');

Insert into workerSkills
(workerId, skillName)
Values
(1, 'Grape picking');

Insert into workerSkills
(workerId, skillName)
Values
(1, 'Milking');

Insert into workerSkills
(workerId, skillName)
Values
(2, 'No action');

Insert into workerSkills
(workerId, skillName)
Values
(2, 'Quarrying');

Insert into workerSkills
(workerId, skillName)
Values
(2, 'Woodcutting');

Insert into workerSkills
(workerId, skillName)
Values
(2, 'Fishing');

Insert into workerSkills
(workerId, skillName)
Values
(2, 'Hunting');

Insert into workerSkills
(workerId, skillName)
Values
(2, 'Harvesting');

Insert into workerSkills
(workerId, skillName)
Values
(2, 'Water collecting');

Insert into workerSkills
(workerId, skillName)
Values
(2, 'Iron mining');

Insert into workerSkills
(workerId, skillName)
Values
(2, 'Gold mining');

Insert into workerSkills
(workerId, skillName)
Values
(2, 'Coal mining');

Insert into workerSkills
(workerId, skillName)
Values
(2, 'Shearing');

Insert into workerSkills
(workerId, skillName)
Values
(2, 'Grape picking');

Insert into workerSkills
(workerId, skillName)
Values
(2, 'Milking');

Insert into workerSkills
(workerId, skillName)
Values
(3, 'No action');

Insert into workerSkills
(workerId, skillName)
Values
(3, 'Quarrying');

Insert into workerSkills
(workerId, skillName)
Values
(3, 'Woodcutting');

Insert into workerSkills
(workerId, skillName)
Values
(3, 'Fishing');

Insert into workerSkills
(workerId, skillName)
Values
(3, 'Hunting');

Insert into workerSkills
(workerId, skillName)
Values
(3, 'Harvesting');

Insert into workerSkills
(workerId, skillName)
Values
(3, 'Water collecting');

Insert into workerSkills
(workerId, skillName)
Values
(3, 'Iron mining');

Insert into workerSkills
(workerId, skillName)
Values
(3, 'Gold mining');

Insert into workerSkills
(workerId, skillName)
Values
(3, 'Coal mining');

Insert into workerSkills
(workerId, skillName)
Values
(3, 'Shearing');

Insert into workerSkills
(workerId, skillName)
Values
(3, 'Grape picking');

Insert into workerSkills
(workerId, skillName)
Values
(3, 'Milking');

Insert into workerSkills
(workerId, skillName)
Values
(4, 'No action');

Insert into workerSkills
(workerId, skillName)
Values
(4, 'Quarrying');

Insert into workerSkills
(workerId, skillName)
Values
(4, 'Woodcutting');

Insert into workerSkills
(workerId, skillName)
Values
(4, 'Fishing');

Insert into workerSkills
(workerId, skillName)
Values
(4, 'Hunting');

Insert into workerSkills
(workerId, skillName)
Values
(4, 'Harvesting');

Insert into workerSkills
(workerId, skillName)
Values
(4, 'Water collecting');

Insert into workerSkills
(workerId, skillName)
Values
(4, 'Iron mining');

Insert into workerSkills
(workerId, skillName)
Values
(4, 'Gold mining');

Insert into workerSkills
(workerId, skillName)
Values
(4, 'Coal mining');

Insert into workerSkills
(workerId, skillName)
Values
(4, 'Shearing');

Insert into workerSkills
(workerId, skillName)
Values
(4, 'Grape picking');

Insert into workerSkills
(workerId, skillName)
Values
(4, 'Milking');

Insert into workerSkills
(workerId, skillName)
Values
(5, 'No action');

Insert into workerSkills
(workerId, skillName)
Values
(5, 'Quarrying');

Insert into workerSkills
(workerId, skillName)
Values
(5, 'Woodcutting');

Insert into workerSkills
(workerId, skillName)
Values
(5, 'Fishing');

Insert into workerSkills
(workerId, skillName)
Values
(5, 'Hunting');

Insert into workerSkills
(workerId, skillName)
Values
(5, 'Harvesting');

Insert into workerSkills
(workerId, skillName)
Values
(5, 'Water collecting');

Insert into workerSkills
(workerId, skillName)
Values
(5, 'Iron mining');

Insert into workerSkills
(workerId, skillName)
Values
(5, 'Gold mining');

Insert into workerSkills
(workerId, skillName)
Values
(5, 'Coal mining');

Insert into workerSkills
(workerId, skillName)
Values
(5, 'Shearing');

Insert into workerSkills
(workerId, skillName)
Values
(5, 'Grape picking');

Insert into workerSkills
(workerId, skillName)
Values
(5, 'Milking');

Insert into workerSkills
(workerId, skillName)
Values
(6, 'No action');

Insert into workerSkills
(workerId, skillName)
Values
(6, 'Quarrying');

Insert into workerSkills
(workerId, skillName, skillLevel)
Values
(6, 'Woodcutting', 10);

Insert into workerSkills
(workerId, skillName)
Values
(6, 'Fishing');

Insert into workerSkills
(workerId, skillName)
Values
(6, 'Hunting');

Insert into workerSkills
(workerId, skillName)
Values
(6, 'Harvesting');

Insert into workerSkills
(workerId, skillName)
Values
(6, 'Water collecting');

Insert into workerSkills
(workerId, skillName)
Values
(6, 'Iron mining');

Insert into workerSkills
(workerId, skillName)
Values
(6, 'Gold mining');

Insert into workerSkills
(workerId, skillName)
Values
(6, 'Coal mining');

Insert into workerSkills
(workerId, skillName)
Values
(6, 'Shearing');

Insert into workerSkills
(workerId, skillName, skillLevel)
Values
(6, 'Grape picking', 5);

Insert into workerSkills
(workerId, skillName, skillLevel)
Values
(6, 'Milking', 2);

Insert into workerSkills
(workerId, skillName)
Values
(7, 'No action');

Insert into workerSkills
(workerId, skillName)
Values
(7, 'Quarrying');

Insert into workerSkills
(workerId, skillName)
Values
(7, 'Woodcutting');

Insert into workerSkills
(workerId, skillName)
Values
(7, 'Fishing');

Insert into workerSkills
(workerId, skillName)
Values
(7, 'Hunting');

Insert into workerSkills
(workerId, skillName)
Values
(7, 'Harvesting');

Insert into workerSkills
(workerId, skillName)
Values
(7, 'Water collecting');

Insert into workerSkills
(workerId, skillName)
Values
(7, 'Iron mining');

Insert into workerSkills
(workerId, skillName)
Values
(7, 'Gold mining');

Insert into workerSkills
(workerId, skillName)
Values
(7, 'Coal mining');

Insert into workerSkills
(workerId, skillName)
Values
(7, 'Shearing');

Insert into workerSkills
(workerId, skillName)
Values
(7, 'Grape picking');

Insert into workerSkills
(workerId, skillName)
Values
(7, 'Milking');

Insert into workerSkills
(workerId, skillName)
Values
(8, 'No action');

Insert into workerSkills
(workerId, skillName)
Values
(8, 'Quarrying');

Insert into workerSkills
(workerId, skillName)
Values
(8, 'Woodcutting');

Insert into workerSkills
(workerId, skillName)
Values
(8, 'Fishing');

Insert into workerSkills
(workerId, skillName)
Values
(8, 'Hunting');

Insert into workerSkills
(workerId, skillName)
Values
(8, 'Harvesting');

Insert into workerSkills
(workerId, skillName)
Values
(8, 'Water collecting');

Insert into workerSkills
(workerId, skillName)
Values
(8, 'Iron mining');

Insert into workerSkills
(workerId, skillName)
Values
(8, 'Gold mining');

Insert into workerSkills
(workerId, skillName)
Values
(8, 'Coal mining');

Insert into workerSkills
(workerId, skillName)
Values
(8, 'Shearing');

Insert into workerSkills
(workerId, skillName)
Values
(8, 'Grape picking');

Insert into workerSkills
(workerId, skillName)
Values
(8, 'Milking');

Insert into workerSkills
(workerId, skillName)
Values
(9, 'No action');

Insert into workerSkills
(workerId, skillName)
Values
(9, 'Quarrying');

Insert into workerSkills
(workerId, skillName)
Values
(9, 'Woodcutting');

Insert into workerSkills
(workerId, skillName)
Values
(9, 'Fishing');

Insert into workerSkills
(workerId, skillName)
Values
(9, 'Hunting');

Insert into workerSkills
(workerId, skillName)
Values
(9, 'Harvesting');

Insert into workerSkills
(workerId, skillName)
Values
(9, 'Water collecting');

Insert into workerSkills
(workerId, skillName)
Values
(9, 'Iron mining');

Insert into workerSkills
(workerId, skillName)
Values
(9, 'Gold mining');

Insert into workerSkills
(workerId, skillName)
Values
(9, 'Coal mining');

Insert into workerSkills
(workerId, skillName)
Values
(9, 'Shearing');

Insert into workerSkills
(workerId, skillName)
Values
(9, 'Grape picking');

Insert into workerSkills
(workerId, skillName)
Values
(9, 'Milking');

Insert into workerSkills
(workerId, skillName)
Values
(10, 'No action');

Insert into workerSkills
(workerId, skillName)
Values
(10, 'Quarrying');

Insert into workerSkills
(workerId, skillName)
Values
(10, 'Woodcutting');

Insert into workerSkills
(workerId, skillName)
Values
(10, 'Fishing');

Insert into workerSkills
(workerId, skillName)
Values
(10, 'Hunting');

Insert into workerSkills
(workerId, skillName)
Values
(10, 'Harvesting');

Insert into workerSkills
(workerId, skillName)
Values
(10, 'Water collecting');

Insert into workerSkills
(workerId, skillName)
Values
(10, 'Iron mining');

Insert into workerSkills
(workerId, skillName)
Values
(10, 'Gold mining');

Insert into workerSkills
(workerId, skillName)
Values
(10, 'Coal mining');

Insert into workerSkills
(workerId, skillName)
Values
(10, 'Shearing');

Insert into workerSkills
(workerId, skillName)
Values
(10, 'Grape picking');

Insert into workerSkills
(workerId, skillName)
Values
(10, 'Milking');

-- BuildingGoods(P building Id, P good Name, P good Quality, good Quantity, reserved, good Price, for Sale, good Sales)
Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'stone', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'wood', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'fish', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'venison', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'grain', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'water', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'iron ore', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'gold ore', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'coal', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'wool', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'grapes', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'milk', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'lumber', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'charcoal', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'sausage', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'flour', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'iron', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'gold', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'fabric', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'wine', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'beer', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'cheese', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'carts', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'bread', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'tools', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'jewellery', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(2, 'clothes', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'stone', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'wood', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'fish', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'venison', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'grain', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'water', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'iron ore', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'gold ore', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'coal', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'wool', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'grapes', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'milk', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'lumber', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'charcoal', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'sausage', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'flour', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'iron', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'gold', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'fabric', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'wine', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'beer', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'cheese', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'carts', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'bread', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'tools', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'jewellery', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(3, 'clothes', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'stone', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'wood', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'fish', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'venison', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'grain', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'water', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'iron ore', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'gold ore', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'coal', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'wool', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'grapes', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'milk', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'lumber', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'charcoal', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'sausage', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'flour', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'iron', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'gold', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'fabric', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'wine', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'beer', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'cheese', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'carts', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'bread', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'tools', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'jewellery', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(4, 'clothes', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'stone', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'wood', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'fish', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'venison', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'grain', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'water', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'iron ore', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'gold ore', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'coal', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'wool', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'grapes', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'milk', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'lumber', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'charcoal', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'sausage', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'flour', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'iron', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'gold', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'fabric', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'wine', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'beer', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'cheese', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'carts', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'bread', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'tools', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'jewellery', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(5, 'clothes', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'stone', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'wood', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'fish', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'venison', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'grain', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'water', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'iron ore', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'gold ore', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'coal', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'wool', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'grapes', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'milk', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'lumber', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'charcoal', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'sausage', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'flour', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'iron', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'gold', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'fabric', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'wine', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'beer', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'cheese', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'carts', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'bread', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'tools', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'jewellery', 'good');

Insert into buildingGoods
(buildingId, goodName, goodQuality)
Values
(6, 'clothes', 'good');