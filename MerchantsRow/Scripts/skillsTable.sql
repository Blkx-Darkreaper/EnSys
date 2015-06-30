-- Run command: source public_html/scripts/skillsTable.sql
-- skills(P skill Name, activeSkill)

Drop Table IF EXISTS skills;
CREATE TABLE skills
(skillName varchar(30) NOT NULL Primary key,
activeSkill varchar(30) NOT NULL,
product varchar(20) default null,
Foreign key (product) references goods(goodName));

Insert into skills
(skillName, activeSkill, product)
Values
('No action', 'On vacation', 'Nothing');

Insert into skills
(skillName, activeSkill, product)
Values
('Quarrying', 'Quarrying for stone', 'stone');

Insert into skills
(skillName, activeSkill, product)
Values
('Woodcutting', 'Cutting wood', 'wood');

Insert into skills
(skillName, activeSkill, product)
Values
('Fishing', 'Fishing', 'fish');

Insert into skills
(skillName, activeSkill, product)
Values
('Hunting', 'Hunting deer', 'venison');

Insert into skills
(skillName, activeSkill, product)
Values
('Harvesting', 'Harvesting grain', 'grain');

Insert into skills
(skillName, activeSkill, product)
Values
('Water collecting', 'Collecting water', 'water');

Insert into skills
(skillName, activeSkill, product)
Values
('Iron mining', 'Mining for iron', 'iron ore');

Insert into skills
(skillName, activeSkill, product)
Values
('Gold mining', 'Mining for gold', 'gold ore');

Insert into skills
(skillName, activeSkill, product)
Values
('Coal mining', 'Mining for coal', 'coal');

Insert into skills
(skillName, activeSkill, product)
Values
('Shearing', 'Shearing sheep', 'wool');

Insert into skills
(skillName, activeSkill, product)
Values
('Grape picking', 'Picking grapes', 'grapes');

Insert into skills
(skillName, activeSkill, product)
Values
('Milking', 'Milking cows', 'milk');

Insert into skills
(skillName, activeSkill, product)
Values
('Hewing', 'Hewing wood', 'lumber');

Insert into skills
(skillName, activeSkill, product)
Values
('Charring', 'Charring wood', 'coal');

Insert into skills
(skillName, activeSkill, product)
Values
('Sausage making', 'Curing sausage', 'sausage');

Insert into skills
(skillName, activeSkill, product)
Values
('Milling', 'Milling grain', 'flour');

Insert into skills
(skillName, activeSkill, product)
Values
('Iron smelting', 'Smelting iron', 'iron');

Insert into skills
(skillName, activeSkill, product)
Values
('Gold smelting', 'Smelting gold', 'gold');

Insert into skills
(skillName, activeSkill, product)
Values
('Weaving', 'Weaving fabric', 'fabric');

Insert into skills
(skillName, activeSkill, product)
Values
('Winemaking', 'Making wine', 'wine');

Insert into skills
(skillName, activeSkill, product)
Values
('Brewing', 'Brewing beer', 'beer');

Insert into skills
(skillName, activeSkill, product)
Values
('Cheesemaking', 'Making cheese', 'cheese');

Insert into skills
(skillName, activeSkill, product)
Values
('Carpentry', 'Building carts', 'carts');

Insert into skills
(skillName, activeSkill, product)
Values
('Baking', 'Baking bread', 'bread');

Insert into skills
(skillName, activeSkill, product)
Values
('Toolmaking', 'Making tools', 'tools');

Insert into skills
(skillName, activeSkill, product)
Values
('Goldsmithing', 'Crafting jewellery', 'jewellery');

Insert into skills
(skillName, activeSkill, product)
Values
('Tailoring', 'Making clothes', 'clothes');