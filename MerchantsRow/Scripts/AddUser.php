<?php
require '../../includes/include.php';

require 'addWorker.php';

function addBaseGoods($buildingID, $LinkID) {
	$goodsResource = mysql_query("Select * from goods",$LinkID);
	while ($goodRow = mysql_fetch_row($goodsResource)) {
		mysql_query("Insert into buildingGoods set buildingId = $buildingID, goodName = '$goodRow[0]', goodQuality = 'good', goodQuantity = 0, reserved = 0, goodPrice = 0, forSale = 'n', goodSales = 0",$LinkID);
	}
}

$user = $_POST['username'];
$password = $_POST['password'];
$dob = $_POST['DoB'];
$email = $_POST['email'];

mysql_select_db($database, $LinkID);								//select the DB
$result = mysql_query("Select * From users",$LinkID);				//query the DB to retrieve all existing players
//echo mysql_error($LinkID);

$i = 0;
while ($row=mysql_fetch_row($result)) {								//add all existing users to an array
	$userList[$i] = $row[0];
	$i++;
}

if (in_array($user,$userList)) {									//check if the user exists already
	header('Location: http://deepblue.cs.camosun.bc.ca/~comp19908/?userExists=true');		//if so, reload the main page with an error message
	exit();
}

$usersQuery = 'Insert Into users Values (\''.$user.'\', \''.$password.'\', \''.$dob.'\', \''.$email.'\''.', 100, Now(), Now(), Now())';			//assemble the query for the users table
$addBuildingQuery = "Insert Into buildings set username = '".$user."', structureType = 'Shop', district = 'Central District'";		//assemble the query for the buildings table
$findBuildingQuery = "select buildingId from buildings where username = '$user'";

mysql_query($usersQuery,$LinkID);								//add the user to the users table
mysql_query($addBuildingQuery,$LinkID);							//add the users building to the buildings table
$buildingResult = mysql_query($findBuildingQuery,$LinkID);		//find the building ID of the users new building
$buildingInfo = mysql_fetch_row($buildingResult);
$buildingID = $buildingInfo[0];
addBaseGoods($buildingID, $LinkID);								//populate the goods table

for ($i = 0; $i < 3; $i++) {
	$workerID = addWorker($LinkID, false);
	addWorkerToBuilding($buildingID, $workerID, $LinkID);
}

echo mysql_error($LinkID);
mysql_close($LinkID);

header('Location: http://deepblue.cs.camosun.bc.ca/~comp19908/?success=true');	//reload the main page with a success message
exit();
?>