-- Run command: source public_html/scripts/usersTable.sql
-- Users(P username, password, dob, email, logged, money, lastPaid)

Drop Table IF EXISTS users;
CREATE TABLE users
(username varchar(20) NOT NULL primary key,
Foreign key (username) references buildings(username),
password varchar(20) NOT NULL,
dob DATE NOT NULL,
email varchar(30) NOT NULL,
money integer not null default 100,
loggedIn datetime,
loggedOut datetime,
lastPaid datetime);