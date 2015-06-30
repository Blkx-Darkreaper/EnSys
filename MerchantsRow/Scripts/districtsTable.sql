-- Run command: source public_html/scripts/districtsTable.sql
-- districts(P district, districtSize)

Drop Table IF EXISTS districts;
CREATE TABLE districts
(district varchar(30) NOT NULL Primary key,
Foreign key (district) references buildings(district),
districtSize INTEGER NOT NULL default 20);

-- Districts(P district, districtSize)
Insert into districts
(district, districtSize)
Values
('Central district', 50);