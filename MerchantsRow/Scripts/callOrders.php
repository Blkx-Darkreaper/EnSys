<?php
require 'produceGoods.php';

function callAllOrders ($user, $LinkID) {
	/*
	Select orderId, orderType, concat('$', CAST(orderValue AS CHAR)), creator, creationDate, expirationDate
	From orders
	Order by creator, expirationDate, creationDate, orderId;
	*/
	$query = "Select orderId as 'Order No.', orderType as 'Type', concat('$', CAST(orderValue AS CHAR)) as 'Value', creator as 'Source', creationDate as 'Created', expirationDate as 'Expires' From orders Order by creationDate, expirationDate, orderType, orderId";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	if(! $result) {
		echo "<p>There are no pending orders at this time.</p>";
		return;
	}
	
	$allOrders = null;

	// Get all the orders and add them to an array
	while ($row = mysql_fetch_assoc($result)) {
		//echo "Row:<br>";	//debug
		//print_r($row);	//debug
		//echo "<br><br>";	//debug
		$allOrders[] = $row;
	}
	
	if(! $allOrders) {
		echo "<p>There are no pending orders at this time.</p>";
		return;
	}

	//echo "allOrders:<br>";	//debug
	//print_r($allOrders);	//debug
	//echo "<br><br>";	//debug
	
	// Make a table
	echo "<table><tr>";
	
	// Print the column labels
	foreach(array_keys($allOrders[0]) as $column) {
		echo "<td><b>$column</b></td>";
	}
	echo "</tr>";
	
	// Print each row of values
	foreach($allOrders as $order) {
		echo "<tr>";
		foreach($order as $column => $value) {
			if($column == 'Order No.') {
				echo '<td><a href ="orderSummary.php?orderId=' . $value . '">' . $value . '</a></td>';
				continue;
			}
			echo "<td>$value</td>";
		}
		echo "</tr>";
	}
	
	// End the table
	echo "</table>";
}

function makeOrder ($user, $LinkID) {
	$user = ucfirst(strtolower($user));

	/*
	Select buildingId, buildingName, structureType, district
	From buildings
	Where username=$user;
	*/
	$query = "Select buildingId, buildingName, structureType, district From buildings Where username='$user'";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	if(! $result) {
		echo "You do not have any buildings.";
		return;
	}
	
	$allBuildings = null;

	// Get all the buildings and add them to an array
	while ($row = mysql_fetch_assoc($result)) {
		//echo "Row:<br>";	//debug
		//print_r($row);	//debug
		//echo "<br><br>";	//debug
		$allBuildings[] = $row;
	}
	
	if(! $allBuildings) {
		echo "You do not have any buildings.";
		return;
	}

	//echo "allBuildings:<br>";	//debug
	//print_r($allBuildings);	//debug
	//echo "<br><br>";	//debug
	
	// Make the form
	echo "<form method=\"post\" action=\"orders.php\">";
	
	//echo "$user<br>"; //debug
	
	// Choose a source building
	echo '<b>Choose a building</b><br><select name="source">';
	foreach ($allBuildings as $building) {
		$buildingId = $building['buildingId'];
		//echo "ID: $buildingId<br>"; //debug
		$buildingName = $building['buildingName'];
		//echo "Name: $buildingName<br>"; //debug
		$structureType = $building['structureType'];
		//echo "Type: $structureType<br>"; //debug
		$district = $building['district'];
		//echo "District: $district<br>"; //debug
		if(! $buildingName) {
			echo '<option value="' . $buildingId . '">' . "$structureType $buildingId in the $district</option>";
			continue;
		}
		echo '<option value="' . $buildingId . '">' . $buildingName . '</option>';
	}
	echo "</select><br>";
	
	// Set the order type
	echo '<b>Type:</b><br><select name="type">
			<option value="Transfer">Transfer</option>
			<option value="Buy">Buy</option>
			<option value="Sell">Sell</option>
			<option value="Dump">Sell at Cost</option>
			</select><br>';
	
	// Set an expiration date
	echo '<b>Choose an expiration date:</b><br><input type="date" name="expiration" value=' . date('Y-m-d', time()+(7 * 24 * 60 * 60)) . '><br>';
	
	// Add submit button to create the order
	echo '<br><input type="submit" name="submit" value="Create Order">';

	// End the form
	echo "</form>";
}

function addItemsToOrder ($user, $LinkID) {
	$type = $_POST['type'];
	$source = $_POST['source'];
	if($type == 'Transfer') {
		/*
		Select buildingId, buildingName, structureType, district
		From buildings
		Where username='$user'
		And buildingId not in ($source);
		*/
		$query = "Select buildingId, buildingName, structureType, district From buildings Where username='$user' And buildingId not in ($source)";
		//echo "$query<br>";	//debug
		$result = mysql_query($query ,$LinkID);
		
		// Get all the buildings and add them to an array
		$otherBuildings = 0;
		while ($row = mysql_fetch_assoc($result)) {
			//echo "Row:<br>";	//debug
			//print_r($row);	//debug
			//echo "<br><br>";	//debug
			$allBuildings[] = $row;
			if(isset($row['buildingId'])) {
				$otherBuildings++;
			}
		}
		
		if($otherBuildings == 0) {
			echo '<form method="post" action="orders.php">You have no other buildings<br><input type="submit" value="Ok"></form>';
			return;
		}
	}

	// Make the form
	echo '<form method="post" action="orders.php">';
	
	echo '<input type="hidden" name="source" value="' . $source . '">';

	echo '<input type="hidden" name="type" value="' . $type . '">';
	$creation = gmdate("Y-m-d", time());
	$expiration = $_POST['expiration'];
	echo '<input type="hidden" name="expiration" value="' . $expiration . '">';

	
	echo "<h3>$type Order</h3>";
	echo "<table><tr>";
/* 	if($type != 'Transfer') {
		echo "<td><b>Value:</b>$$value</td>";
	} */
	if(isset($_POST['buildingName'])) {
		$buildingName = $_POST['buildingName'];
		echo "<td><b>Source:</b>$buildingName</td>";
	} else {
		echo "<td><b>Source:</b>$source</td>";
	}
	if($type == 'Transfer') {
		// Choose a destination
		echo '<b>Destination:</b><br><select name="destination">';
		echo '<option value="">None</option>';
		foreach ($allBuildings as $building) {
			$buildingId = $building['buildingId'];
			//echo "ID: $buildingId<br>"; //debug
			$buildingName = $building['buildingName'];
			//echo "Name: $buildingName<br>"; //debug
			$structureType = $building['structureType'];
			//echo "Type: $structureType<br>"; //debug
			$district = $building['district'];
			//echo "District: $district<br>"; //debug
			echo '<option value="' . $buildingId . '">';
			if($username != $user) {
				echo "$username's ";
			}
			if(! $buildingName) {
				echo "$structureType $buildingId in the $district";
			} else {
				echo "$buildingName";
			}
			echo '</option>';
		}
		echo "</select><br>";
	}
	echo "</tr></table><table><tr>";
	echo "<td><b>Created:</b>$creation</td>";
	echo "<td><b>Expires:</b>$expiration</td>";
	echo "</tr></table>";
	
	// Transferring or Selling
	if($type != 'Buy') {
		// Get the source's inventory
		/*
		Select goodName, goodQuality, goodQuantity
		From buildingGoods
		Where buildingId = 1
		Order by goodName, goodQuality;
		*/
		$query = "Select goodName, goodQuality, goodQuantity From buildingGoods Where buildingId = $source Order by goodName, goodQuality";
		//echo "$query<br>";	//debug
		$result = mysql_query($query ,$LinkID);
		echo mysql_error($LinkID);
		
		if(! $result) {
			echo "You have no goods.";
			return;
		}
		
		$allGoods = null;

		// Get all the goods and add them to an array
		while ($row = mysql_fetch_assoc($result)) {
			//echo "Row:<br>";	//debug
			//print_r($row);	//debug
			//echo "<br><br>";	//debug
			if($row['goodQuantity'] == 0){
				continue;
			}
			$allGoods[$row['goodName']][$row['goodQuality']] = $row['goodQuantity'];
		}
	// Buying
	} else {
		// Set a value
		/*
		Select money
		From users
		Where username='$user';
		*/
		$query = "Select money From users Where username='$user'";
		//echo "$query<br>";	//debug
		$result = mysql_query($query ,$LinkID);
		echo mysql_error($LinkID);
		while($row = mysql_fetch_assoc($result)) {
			$money = $row['money'];
		}
		
		echo '<b>Value:</b><br><input type="number" name="value" value="0" min="0" max="' . $money . '"><br>';
		
		// Get a list of all goods
		/*
		Select goodName, goodQuality
		From goods
		Order by goodName, goodQuality;
		*/
		$query = "Select goodName, goodQuality From goods Order by goodName, goodQuality";
		//echo "$query<br>";	//debug
		$result = mysql_query($query ,$LinkID);
		echo mysql_error($LinkID);
		
		if(! $result) {
			echo "There are no goods.";
			return;
		}
		
		$allGoods = null;

		// Get all the goods and add them to an array
		while ($row = mysql_fetch_assoc($result)) {
			//echo "Row:<br>";	//debug
			//print_r($row);	//debug
			//echo "<br><br>";	//debug
			$allGoods[$row['goodName']][$row['goodQuality']] = 99;
		}
	}
	
	if(! $allGoods) {
		echo "You have no goods.";
		return;
	}

	//echo "allGoods:<br>";	//debug
	//print_r($allGoods);	//debug
	//echo "<br><br>";	//debug
	
	// Build a table of all the goods
	echo "<table>";
	
	// Print the column names
	echo "<tr><td><b>Good</b></td>
					<td><b>Quality</b></td>
					<td><b>Quantity</b></td></tr>";
	
	foreach($allGoods as $goodName => $good) {
		foreach($good as $goodQuality => $goodQuantity) {
/* 			$_SESSION['order'][$goodName] = $goodQuantity;
			if($goodReserved = $good[$goodQuality]['reserved']) {
				if(! $_SESSION['order'][$goodName][$goodQuality]['reserved']) {
					$_SESSION['order'][$goodName][$goodQuality]['reserved'] = 0;
				}
				//echo $_SESSION['order'][$goodName]['reserved']; //debug
				//echo "<br>"; //debug
				//echo intval($goodReserved); //debug
				$_SESSION['order'][$goodName][$goodQuality]['reserved'] = $_SESSION['order'][$goodName][$goodQuality]['reserved'] + intval($goodReserved);
			} */
			
			// Print each row
			echo "<tr>";
			echo '<td>' . $goodName . "</td>";
			echo "<td>$goodQuality</td>";
			echo '<td><input type="number" name="goods' . "[$goodName][$goodQuality]" . '" value="0" min="0"';
			echo ' max="' . $goodQuantity . '">';
			echo "</td></tr>";
		}
	}
	
	// End the table
	echo "</table>";
	
	// Add submit button to finalize the order
	echo '<br><input type="submit" name="submit" value="Finalize Order">';
	
	// Add submit button to cancel the order
	echo '<input type="submit" name="submit" value="Cancel Order">';

	// End the form
	echo "</form>";
}

function sellItemsAtCost ($user, $LinkID) {
	$source = $_POST['source'];
	$type = $_POST['type'];
		
	// Get the source's inventory
	/*
	Select goodName, goodQuality, goodQuantity, goodPrice
	From buildingGoods left join goods using(goodName)
	Where buildingId = 1
	Order by goodName, goodQuality;
	*/
	$query = "Select bg.goodName, bg.goodQuality, bg.goodQuantity, g.goodPrice From buildingGoods bg left join goods g using(goodName) Where buildingId = $source Order by bg.goodName, bg.goodQuality";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	if(! $result) {
		echo '<form method="post" action="orders.php">You have no goods to sell<br><input type="submit" value="Ok"></form>';
		return;
	}
	
	$allGoods = null;
	$totalGoods = 0;

	// Get all the goods and add them to an array
	while ($row = mysql_fetch_assoc($result)) {
		$goodQuantity = $row['goodQuantity'];
		//echo "Row:<br>";	//debug
		//print_r($row);	//debug
		//echo "<br><br>";	//debug
		if($goodQuantity == 0) {
			continue;
		}
		$allGoods[$row['goodName']][$row['goodQuality']][$row['goodPrice']] = $goodQuantity;
		$totalGoods += $goodQuantity;
	}
	
	if($totalGoods <= 0) {
		echo '<form method="post" action="orders.php">You have no goods to sell<br><input type="submit" value="Ok"></form>';
		return;
	}
	
	// Make the form
	echo '<form method="post" action="orders.php">';
	echo '<input type="hidden" name="source" value="' . $source . '">';
	echo '<input type="hidden" name="type" value="' . $type . '">';
	echo "<h3>Sell Goods at Cost</h3>";
	echo "<table><tr>";
	echo "<td><b>Source:</b>$source</td>";
	echo "</tr></table><table><tr>";
	
	// Build a table of all the goods
	echo "<table>";
	
	// Print the column names
	echo "<tr><td><b>Good</b></td>
					<td><b>Quality</b></td>
					<td><b>Price</b></td>
					<td><b>Quantity</b></td>
					</tr>";
	
	foreach($allGoods as $goodName => $good) {
		foreach($good as $goodQuality => $goodInfo) {
			foreach($goodInfo as $goodPrice => $goodQuantity) {
				// Print each row
				echo "<tr>";
				echo '<td>' . $goodName . "</td>";
				echo "<td>$goodQuality</td>";
				echo "<td>$$goodPrice</td>";
				echo '<td><input type="number" name="goods' . "[$goodName][$goodQuality][$goodPrice]" . '" value="0" min="0"';
				echo ' max="' . $goodQuantity . '">';
				echo "</td></tr>";
			}
		}
	}
	
	// End the table
	echo "</table>";
	
	// Add submit button to finalize the deal
	echo '<br><input type="submit" name="submit" value="Finalize Deal">';
	
	// Add submit button to cancel the deal
	echo '<input type="submit" name="submit" value="Cancel Deal">';

	// End the form
	echo "</form>";
}

function finalizeDeal ($user, $LinkID) {
	//echo "Goods:<br>"; //debug
	//print_r($_POST['goods']); //debug
	//echo "<br>"; //debug
	
	// Get posted values and add to an array
	$orderItems = null;
	$goodPrices = null;
	$totalQuantity = 0;
	foreach($_POST['goods'] as $goodName => $good) {
		foreach($good as $goodQuality => $goodInfo) {
			foreach($goodInfo as $goodPrice => $goodQuantity) {
				if($goodQuantity == '0' or $goodQuantity == '') {
					continue;
				}
				$orderItems[$goodName][$goodQuality] = (-1 * $goodQuantity);
				$goodPrices[$goodName][$goodQuality] = $goodPrice;
				$totalQuantity += $goodQuantity;
			}
		}
	}
	
	//echo "Order items:<br>"; //debug
	//print_r($orderItems); //debug
	//echo "<br>"; //debug
	
	if($totalQuantity == 0) {
		echo '<form method="post" action="orders.php">No items were added to the order<br><input type="submit" value="Ok"></form>';
		return;
	}
	
	$source = $_POST['source'];
	
	$goodsSold = updateInventory($LinkID, $source, null, $orderItems, false);
	//echo "Goods sold:<br>"; //debug
	//print_r($goodsSold); //debug
	//echo "<br>"; //debug

	$value = 0;
	echo '<form method="post" action="orders.php">';
	echo '<b>Goods sold:</b><br>';
	foreach ($goodsSold as $goodName => $good) {
		foreach($good as $goodQuality => $goodQuantity) {
			$goodQuantity = (-1 * $goodQuantity);
			if($goodQuantity == 0) {
				continue;
			}
			$goodPrice = $goodPrices[$goodName][$goodQuality];
			//echo "Price: $goodPrice<br>"; //debug
			$profit = $goodQuantity * $goodPrice;
			$value += $profit;
			echo "$goodQuantity $goodQuality $goodName for $$profit<br>";
		}
	}
	changeUserMoney($user, $LinkID, $value);
	echo "<br><b>Total profit: $$value</b><br>";
	echo '<input type="submit" value="Ok"></form>';
}

function changeUserMoney ($user, $LinkID, $amount) {
	if($amount <= 0) {
		return;
	}

	$query = "Update users Set money=money+$amount Where username='$user'";
	//echo "$query<br>";	//debug
	$results = mysql_query($query, $LinkID);
}

function finalizeOrder ($user, $LinkID) {
	// Get posted values and add to an array
	$orderItems = null;
	$totalQuantity = 0;
	foreach($_POST['goods'] as $goodName => $good) {
		foreach($good as $goodQuality => $goodQuantity) {
			if($goodQuantity == '0' or $goodQuantity == '') {
				continue;
			}
			$orderItems[] = array('goodName' => $goodName, 'goodQuality' => $goodQuality, 'goodQuantity' => $goodQuantity);
			$totalQuantity += $goodQuantity;
		}
	}
	
	//echo "Order items:<br>"; //debug
	//print_r($orderItems); //debug
	//echo "<br>"; //debug
	
	if($totalQuantity == 0) {
		echo '<form method="post" action="orders.php">No items were added to the order<br><input type="submit" value="Ok"></form>';
		return;
	}

	$type = $_POST['type'];
	$value = $_POST['value'];
	$source = $_POST['source'];
	if(isset($_POST['destination'])) {
		$destination = $_POST['destination'];
	} else {
		$destination = '';
	}
	$expiration = $_POST['expiration'];
	
	if($type == 'Transfer') {
		$value = 0;
		$expiration = date('Y-m-d', time());
	}
	/*
	Insert into orders
	(orderType, orderValue, creator, recipient, creationDate, expirationDate)
	Values
	($type, $value, $source, $destination, CURDATE(), $expiration);
	*/
	if($destination == '') {
		$query = "Insert into orders (orderType, orderValue, creator, creationDate, expirationDate) Values ('$type', $value, $source, CURDATE(), '$expiration')";
	} else {
		$query = "Insert into orders (orderType, orderValue, creator, recipient, creationDate, expirationDate) Values ('$type', $value, $source, $destination, CURDATE(), '$expiration')";
	}
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	if(! $result) {
		echo "Failed to finalize order.";
		return;
	}
	
	/*
	Select last_insert_id()
	From Orders;
	*/
	$query = "Select last_insert_id() From orders";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	$row = mysql_fetch_assoc($result);
	$orderId = $row['last_insert_id()'];
	
	//echo "Order Id:<br>"; //debug
	//print_r($row); //debug
	//echo "<br>"; //debug
	
	foreach($orderItems As $item) {
		$goodName = $item['goodName'];
		$goodQuality = $item['goodQuality'];
		$goodQuantity = $item['goodQuantity'];
		// Add values to the orderItems
		/*
		Insert into orderItems
		(orderId, item, itemQuality, itemQuantity)
		Values
		($orderId, $goodName, $goodQuality, $goodQuantity);
		*/
		$query = "Insert into orderItems (orderId, item, itemQuality, itemQuantity) Values ($orderId, '$goodName', '$goodQuality', $goodQuantity)";
		//echo "$query<br>";	//debug
		$result = mysql_query($query ,$LinkID);
		echo mysql_error($LinkID);
		
		if($type == 'Buy') {
			continue;
		}
		
		// If Selling or Transferring move goods to reserve
		/*
		Update buildingGoods
		Set goodQuantity=$currentQuantity-$goodQuantity, reserved=$goodQuantity
		Where goodName='$goodName'
		And buildingId=$source;
		*/
		$query = "Update buildingGoods Set goodQuantity=(goodQuantity-$goodQuantity), reserved=(reserved+$goodQuantity) Where goodName='$goodName' And buildingId=$source";
		//echo "$query<br>";	//debug
		$result = mysql_query($query ,$LinkID);
		echo mysql_error($LinkID);
	}
	
	echo '<form method="post" action="orders.php"><b>Order completed</b><br><input type="submit" value="Ok"></form>';
}

$user = $_SESSION['user'];

// Choose the DB and run a query.
mysql_select_db($database, $LinkID);
//echo "Post data: "; //debug
//print_r($_POST); //debug
//echo "<br><br>"; //debug

If(! isset($_POST['submit'])) {
	$_POST['submit'] = '';
}

switch($_POST['submit']) {
	case 'Finalize Order':
		finalizeOrder($user, $LinkID);
		break;
	case 'Finalize Deal':
		finalizeDeal($user, $LinkID);
		break;
	case 'Create Order':
		if($_POST['type'] == 'Dump') {
			sellItemsAtCost ($user, $LinkID);
			break;
		}
		addItemsToOrder($user, $LinkID);
		break;
	default:
		$_SESSION['order'] = null;
		echo "<div><h1>Pending Orders</h1>";
		callAllOrders($user, $LinkID);
		echo "</div>";

		echo "<div><h1>Make an Order</h1>";
		makeOrder($user, $LinkID);
		echo "</div>";
}
?>