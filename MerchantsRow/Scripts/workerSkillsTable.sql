-- Run command: source public_html/scripts/workerSkillsTable.sql
-- WorkerSkills(P worker Id, P skill Name, skill Level)

Drop Table IF EXISTS workerSkills;
CREATE TABLE workerSkills
(workerId integer NOT NULL,
skillName varchar(30) NOT NULL,
PRIMARY KEY (workerId, skillName),
Foreign key (workerId) references workers(workerId),
Foreign key (skillName) references skills(skillName),
skillLevel INTEGER NOT NULL default 1,
experience decimal(10,2) default 0,
nextLevel decimal(10,2) default 24);