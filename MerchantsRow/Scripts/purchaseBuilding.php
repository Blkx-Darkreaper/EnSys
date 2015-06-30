<?php
include '../../includes/include.php';
session_start();
mysql_select_db($database, $LinkID);

//retrieve the form data
$buildingType = $_POST['type'];
$user = $_SESSION['user'];

$userMoney = mysql_query("select money from users where username = '$user'", $LinkID);
$userMoney = mysql_fetch_row($userMoney);

$buildingInfo = mysql_query("select * from structures where structureType = '$buildingType'", $LinkID);
$buildingInfo = mysql_fetch_row($buildingInfo);

$costInfo = mysql_query("select structureCost from structures where structureType = '$buildingType'", $LinkID);
$costInfo = mysql_fetch_row($costInfo);

//check if the user has enough money
if ($userMoney[0] < $buildingInfo[4]) {
	$_SESSION['buildingPurchased'] = "You do not have enough money to purchase that building.";

	header('Location: http://deepblue.cs.camosun.bc.ca/~comp19908/MerchantsRow/buildingStats.php');
	exit();
}

//Add the building to the users account
mysql_query("insert into buildings set username = '$user', structureType = '$buildingType', district = 'Central District'", $LinkID);

mysql_query("update users set money = money - $costInfo[0] where username = '$user'", $LinkID);

$_SESSION['buildingPurchased'] = "You have purchased a new $buildingType!";

header('Location: http://deepblue.cs.camosun.bc.ca/~comp19908/MerchantsRow/buildingStats.php');
exit();

?>