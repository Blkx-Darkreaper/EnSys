-- Run command: source public_html/scripts/setupDatabase.sql

use c199grp08;

-- Create Orders table and insert game data
source public_html/scripts/ordersTable.sql

-- Create Order items table and insert game data
source public_html/scripts/orderItemsTable.sql

-- Create Order bids table
source public_html/scripts/orderBidsTable.sql

-- Create Skills table and insert game data
source public_html/scripts/skillsTable.sql

-- Create Goods table and insert game data
source public_html/scripts/goodsTable.sql

-- Create Crafting Recipes table and insert game data
source public_html/scripts/craftingRecipesTable.sql

-- Create Building Reports table and insert game data
source public_html/scripts/buildingReportsTable.sql

-- Create Buildings table and insert game data
source public_html/scripts/buildingsTable.sql

-- Create Users table and insert game data
source public_html/scripts/usersTable.sql

-- Create Structures table and insert game data
source public_html/scripts/structuresTable.sql

-- Create Districts table and insert game data
source public_html/scripts/districtsTable.sql

-- Create Building Goods table and insert game data
source public_html/scripts/buildingGoodsTable.sql

-- Create Building Workers table and insert game data
source public_html/scripts/buildingWorkersTable.sql

-- Create Workers table and insert game data
source public_html/scripts/workersTable.sql

-- Create Worker bids table and insert game data
source public_html/scripts/workerBidsTable.sql

-- Create Worker Skills table and insert game data
source public_html/scripts/workerSkillsTable.sql

-- Populate the tables with test data
-- source public_html/scripts/populateTables.sql