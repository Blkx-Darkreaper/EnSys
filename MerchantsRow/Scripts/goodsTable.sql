-- Run command: source public_html/scripts/goodsTable.sql
-- goods(P good Name)

Drop Table IF EXISTS goods;
CREATE TABLE goods
(goodName varchar(20) NOT NULL Primary Key,
goodQuality varchar(12) not null,
goodPrice integer not null default 0);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('stone', 'good', 6);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('wood', 'good', 4);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('fish', 'good', 6);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('venison', 'good', 8);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('grain', 'good', 5);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('water', 'good', 1);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('iron ore', 'good', 10);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('gold ore', 'good', 12);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('coal', 'good', 8);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('wool', 'good', 3);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('grapes', 'good', 4);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('milk', 'good', 5);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('lumber', 'good', 6);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('sausage', 'good', 12);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('flour', 'good', 12);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('iron', 'good', 60);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('gold', 'good', 70);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('fabric', 'good', 15);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('wine', 'good', 20);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('beer', 'good', 15);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('cheese', 'good', 12);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('carts', 'good', 50);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('bread', 'good', 25);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('tools', 'good', 80);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('jewellery', 'good', 90);

Insert into goods
(goodName, goodQuality, goodPrice)
Values
('clothes', 'good', 40);