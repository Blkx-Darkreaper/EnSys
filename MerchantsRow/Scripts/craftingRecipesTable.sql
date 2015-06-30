-- Run command: source public_html/scripts/craftingRecipesTable.sql
-- CraftingRecipes(P method, P ingredient, inputQuantity)

Drop Table IF EXISTS craftingRecipes;
CREATE TABLE craftingRecipes
(method varchar(20) NOT NULL,
ingredient varchar(20) NOT NULL,
Primary key (method, ingredient),
Foreign key (ingredient) references goods(goodName),
inputQuantity integer not null);

-- MethodProducts(P method, product, output Quantity)

Drop Table methodProducts;
CREATE TABLE methodProducts
(method varchar(20) NOT NULL Primary key,
Foreign key (method) references craftingRecipes(method),
product varchar(20) NOT NULL,
Foreign key (product) references goods(goodName),
outputQuantity integer not null);

-- Adding all crafting recipes

-- Lumber
Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Hewing', 'wood', 1);

Insert into methodProducts
(method, product, outputQuantity)
Values
('Hewing', 'lumber', 1);

-- Charcoal
Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Charring', 'wood', 1);

Insert into methodProducts
(method, product, outputQuantity)
Values
('Charring', 'coal', 1);

-- Sausage
Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Sausage making', 'venison', 1);

Insert into methodProducts
(method, product, outputQuantity)
Values
('Sausage making', 'sausage', 1);

-- Flour
Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Milling', 'grain', 2);

Insert into methodProducts
(method, product, outputQuantity)
Values
('Milling', 'flour', 1);

-- Iron
Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Iron smelting', 'iron ore', 4);

Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Iron smelting', 'coal', 2);

Insert into methodProducts
(method, product, outputQuantity)
Values
('Iron smelting', 'iron', 1);

-- Gold
Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Gold smelting', 'gold ore', 4);

Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Gold smelting', 'coal', 2);

Insert into methodProducts
(method, product, outputQuantity)
Values
('Gold smelting', 'gold', 1);

-- Fabric
Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Weaving', 'wool', 4);

Insert into methodProducts
(method, product, outputQuantity)
Values
('Weaving', 'fabric', 1);

-- Wine
Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Winemaking', 'grapes', 4);

Insert into methodProducts
(method, product, outputQuantity)
Values
('Winemaking', 'wine', 1);

-- Beer
Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Brewing', 'grain', 1);

Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Brewing', 'water', 2);

Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Brewing', 'wood', 1);

Insert into methodProducts
(method, product, outputQuantity)
Values
('Brewing', 'beer', 1);

-- Cheese
Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Cheesemaking', 'milk', 2);

Insert into methodProducts
(method, product, outputQuantity)
Values
('Cheesemaking', 'cheese', 1);

-- Carts
Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Carpentry', 'lumber', 10);

Insert into methodProducts
(method, product, outputQuantity)
Values
('Carpentry', 'carts', 1);

-- Bread
Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Baking', 'flour', 1);

Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Baking', 'water', 1);

Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Baking', 'milk', 1);

Insert into methodProducts
(method, product, outputQuantity)
Values
('Baking', 'bread', 1);

-- Tools
Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Toolmaking', 'iron', 1);

Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Toolmaking', 'coal', 2);

Insert into methodProducts
(method, product, outputQuantity)
Values
('Toolmaking', 'tools', 1);

-- Jewellery
Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Goldsmithing', 'gold', 1);

Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Goldsmithing', 'coal', 2);

Insert into methodProducts
(method, product, outputQuantity)
Values
('Goldsmithing', 'jewellery', 1);

-- Clothes
Insert into craftingRecipes
(method, ingredient, inputQuantity)
Values
('Tailoring', 'fabric', 2);

Insert into methodProducts
(method, product, outputQuantity)
Values
('Tailoring', 'clothes', 1);