<?php
$lifetime=7200;
session_start();
setcookie(session_name(),session_id(),time()+$lifetime);

include '../../includes/include.php';
require '../scripts/refreshUser.php';
include '../scripts/hireWorker.php';

mysql_select_db($database, $LinkID);	
refreshUser($_SESSION['user'], $LinkID);
hireWorker($LinkID);

if (isset($_SESSION['user']) == False) {
	header('Location: http://deepblue.cs.camosun.bc.ca/~comp19908');
}
?>

<html>
<head>
	<title>Orders</title>
	<link href="../css/styles.css" rel="stylesheet" type="text/css" />
	<link rel="shortcut icon" href="../images/titleIcon.ico" type="image/png">
</head>
<style>
div.page-container {  background-image:url('../images/background.jpg');
			 background-color:#cccccc;
}
</style>
<body>
	<div class="page-container" id="home">
		<div class="user-panel">
			<img src="../images/banner-grey2.gif"></img>
			Logged in as 
			<?php
				echo $_SESSION['user'];
			?>
		</div>
		<div class="stats-bar";>
			<?php include "../scripts/callQuickStats.php"?>
		</div>
		<nav>
			<a href="index.php"><img src="../images/buttons/indexButton.jpg" class="border"></img></a><span class="mini-tab"></span>
			<a href="buildingStats.php"><img src="../images/buttons/buildingButton.jpg" class="border"></img></a><span class="mini-tab"></span>	
			<a href="workerStats.php"><img src="../images/buttons/workersButton.jpg" class="border"></img></a><span class="mini-tab"></span>
			<a href="orders.php"><img src="../images/buttons/ordersButton.jpg" class="border"></img></a><span class="mini-tab"></span>	
			<a href="help.php"><img src="../images/buttons/helpButton.jpg" class="border"></img></a><span class="mini-tab"></span>	
			<a href="../scripts/LogOut.php"><img src="../images/buttons/logoutButton.jpg" class="border"></img></a><span class="mini-tab"></span>	
		</nav>
		<div class="inventory">
			<div class="inv">
				Inventory
				<?php include "../scripts/callGoods.php"?>
			</div>
		</div>
		<div class="center-page">
			<?php include "../scripts/callOrders.php"?>
		</div>
	</div>
</body>
</html>