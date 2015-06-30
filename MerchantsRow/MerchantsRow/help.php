<?php
$lifetime=7200;
session_start();
setcookie(session_name(),session_id(),time()+$lifetime);

include '../../includes/include.php';

require '../scripts/refreshUser.php';

if (isset($_SESSION['user']) == False) {
	header('Location: http://deepblue.cs.camosun.bc.ca/~comp19908');
}
?>

<html>
<head>
	<title>Tutorial</title>
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
			<h1>Tutorial</h1>
			<h2>Getting Started</h2>
			<p>When you create an account, you are issued three workers and one building, but zero goods.  Your objective is to
			give your workers tasks to start collecting ingredients. You may purchase new workers and buildings through selling these 
			goods for gold or create manufactured items to upgrade your buildings or to sell at a higher price.
			<p>

			<h2>Inventory</h2>
			<p><!--This will hold all of your goods, each item is displayed on the left side of the screen.  The categories
			for the goods are as follows: <br>-->
				The goods are separately by the quality of each type: Poor, Good, Excellent, Perfect.
				<br>
				<b>Base goods:</b>
				<br>
					Stone, Wood, Fish, Venison, Grain, Water, Iron Ore, Gold Ore, Coal, Wool, Grapes, Milk.
				<br>
				<b>Processed goods:</b>
				<br>
					Lumber(Wood), Sausage(Venison), Flour(Grain), Iron(Iron Ore, Coal), Gold(Gold Ore, Coal), Fabric(Wool), Wine(Grapes), Beer(Grain, Water, Wood), Cheese(Milk).
				<br>
				<b>Manufactured goods:</b>
				<br>
					Carts(Lumber), Bread(Flour, Water, Milk), Tools(Iron, Coal), Jewellery(Gold, Coal), Clothes(Fabric).
			</p>

			<h2>Workers</h2>
			<p>
			To issue a task, go to the "Workers" tab and select the drop-box for the specific worker and select a task. When completed, click the "Change Tasks" button below the workers.
			<br>
			To obtain new workers, you must first bid an offered wage against other players bids. Each player may only purchase one new worker each week.
			Then select which building the worker will be assigned to and click the "Submit Bid" button.
			<br>
			As your workers continue to have tasks, they will start to be skilled in their chosen profession, allowing them to level up and produce better quality goods per cycle.
			</p>

			<h2>Buildings</h2>
			<p>
			To purchase new buildings, go to the "Building" tab and select which building you would like the purchase.  You must have enough gold and ingredients in order to make the valid purchase.
			<br>
			To nickname your buildings, select the buildings you wish to give the nickname, type in the new name below and click the "Change Building Name" button.
			</p>

			<h2>Orders</h2>
			<p>
			Creating a "Buy" order will create a list of goods that a player may sell you for a price that you set.
			<br>
			Creating a "Sell" order will allow other players to bid for the list of your goods on the order.
			<br>
			To sell an order at a fixed buyout price, use the "Sell at Cost" option.
			<br>
			Use the "Transfer" option to transfer goods  from one building to another
			</p>
		</div>
	</div>
</body>
</html>