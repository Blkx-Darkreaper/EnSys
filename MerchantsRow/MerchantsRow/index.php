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
	<title>Main</title>
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

		<!-- Display title banner -->
		<div class="user-panel">
			<img src="../images/banner-grey2.gif"></img>

			<!-- Call log in information -->
			Logged in as 
			<?php
				echo $_SESSION['user'];
			?>
		</div>

		<!-- Call quick stats bar -->
		<div class="stats-bar";>
			<?php include "../scripts/callQuickStats.php"?>
		</div>

		<!-- Start Navigation bar -->
		<nav>
			<a href="index.php"><img src="../images/buttons/indexButton.jpg" class="border"></img></a><span class="mini-tab"></span>
			<a href="buildingStats.php"><img src="../images/buttons/buildingButton.jpg" class="border"></img></a><span class="mini-tab"></span>	
			<a href="workerStats.php"><img src="../images/buttons/workersButton.jpg" class="border"></img></a><span class="mini-tab"></span>	
			<a href="orders.php"><img src="../images/buttons/ordersButton.jpg" class="border"></img></a><span class="mini-tab"></span>	
			<a href="help.php"><img src="../images/buttons/helpButton.jpg" class="border"></img></a><span class="mini-tab"></span>	
			<a href="../scripts/LogOut.php"><img src="../images/buttons/logoutButton.jpg" class="border"></img></a><span class="mini-tab"></span>	
		</nav>
		<!-- End Navigation bar -->

		<!-- Call Inventory section -->
		<div class="inventory">
			<div class="inv">
				Inventory
				<?php include "../scripts/callGoods.php"?>
			</div>
		</div>
		<!-- End Inventory section -->

		<!-- Start center stage of page -->
		<div class="center-page">
			<h1>Welcome to Merchant's Row!</h1>
			<?php
				echo "<p>";
				echo $_SESSION['report'];
				echo "</p>";
				echo "<br>";
				include '../scripts/callScoreboard.php';
			?>
		</div>
		<!-- End center stage -->
	</div>
</body>
</html>