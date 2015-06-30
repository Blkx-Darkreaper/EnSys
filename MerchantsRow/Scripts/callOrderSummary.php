<?php
require 'produceGoods.php';

function callOrderItems ($user, $LinkID, $orderId) {
	if(isset($_POST['submit'])) {
		switch ($_POST['submit']) {
			case 'Accept Bid':
				acceptBid($user, $LinkID, $orderId);
				return;
			/*case 'Modify Order':
				modifyOrder($user, $LinkID, $orderId);
				return;
			case 'Modify Items':
				modifyOrderItems($user, $LinkID, $orderId);
				return;
			case 'Finish Editing':
				modifyOrderFinished($user, $LinkID, $orderId);
				echo "Order has been modified.<br>";
				echo '<form method="post" action="orderSummary.php?orderId=' . $orderId . '">';
				echo '<input type="submit" name="submit" value="Ok">';
				echo '</form>';
				return;
			case 'Finish Editing Items':
				modifyOrderItemsFinished($user, $LinkID, $orderId);
				echo "Order items have been modified.<br>";
				echo '<form method="post" action="orderSummary.php?orderId=' . $orderId . '">';
				echo '<input type="submit" name="submit" value="Ok">';
				echo '</form>';
				return;*/
			case 'Cancel Order':
				cancelOrder($LinkID, $orderId);
				echo "Order has been cancelled.<br>";
				echo '<form method="post" action="orders.php">';
				echo '<input type="submit" name="submit" value="Ok">';
				echo '</form>';
				return;
		}
	}

	/*
	Select orderType, concat('$', CAST(orderValue AS CHAR)), creator, recipient, creationDate, expirationDate, username
	From orders o left join buildings b On o.creator=b.buildingId
	Where orderId=$orderId;
	*/
	$query = "Select orderType as 'Type', concat('$', CAST(orderValue AS CHAR)) as 'Value', creator as 'Source', recipient as 'Destination', creationDate as 'Created', expirationDate as 'Expires', username From orders o left join buildings b On o.creator=b.buildingId Where orderId=$orderId";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	// Get the order's information and add it to the array
	while ($row = mysql_fetch_assoc($result)) {
		//echo "Row:<br>";	//debug
		//print_r($row);	//debug
		//echo "<br><br>";	//debug
		$orderInfo = array('Type'=>$row['Type'], 'Value'=>$row['Value'], 'Source'=>$row['Source'], 'Destination'=>$row['Destination'], 'Created'=>$row['Created'], 'Expires'=>$row['Expires']);
		$creator = $row['username'];
	}
	
	echo "<h1>" . $orderInfo['Type'] . " Order</h1><br>";
	
	// Modify order form
	echo '<form method="post" action="orderSummary.php?orderId=' . $orderId . '">';
	
	// Make a table
	echo "<table><tr>";
	
	// Print the column labels
	echo "<td><b>Value</b></td>";
	echo "<td><b>Source</b></td>";
	echo "<td><b>Destination</b></td>";
	echo "<td><b>Created</b></td>";
	echo "<td><b>Expires</b></td>";
	echo "</tr>";
	
	// Print each row of values
	echo "<tr>";
	echo '<td>' . $orderInfo['Value'] . '</td>';
	echo '<td>' . $orderInfo['Source'] . '</td>';
	echo '<td>' . $orderInfo['Destination'] . '</td>';
	echo '<td>' . $orderInfo['Created'] . '</td>';
	echo '<td>' . $orderInfo['Expires'] . '</td>';
		
	echo "</tr>";
	
	// End the table
	echo "</table>";
	
	if($creator == ucfirst(strtolower($user))) {
		//echo '<input type="submit" name="submit" value="Modify Order">';
		echo '<input type="submit" name="submit" value="Cancel Order">';
	}	
	
	// End the form
	echo '</form>';

	/*
	Select item, itemQuality, itemQuantity
	From orderItems left join orders using(orderId)
	Where orderId = $orderId
	Order by item, itemQuality;
	*/
	$query = "Select item as Item, itemQuality as Quality, itemQuantity as Quantity From orderItems left join orders using(orderId) Where orderId = $orderId Order by item, itemQuality";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	
	// Get the order's items and add them to an array
	$allOrderItems = array();
	
	while ($row = mysql_fetch_assoc($result)) {
		$allOrderItems[] = $row;
	}
	//echo "allOrderItems:<br>";	//debug
	//print_r($allOrderItems);	//debug
	//echo "<br><br>";	//debug
	
	/*
	// Modify Items form
	echo '<form method="post" action="orderSummary.php?orderId=' . $orderId . '">';
	*/
	
	echo "<br><h3>Items:</h3>";
	// Make a table
	echo "<table><tr>";
	
	// Print the column labels
	foreach(array_keys($allOrderItems[0]) as $column) {
		echo "<td><b>$column</b></td>";
	}
	echo "</tr>";
	
	// Print each row of values
	foreach($allOrderItems as $orderItem) {
		echo "<tr>";
		foreach($orderItem as $value) {
			echo "<td>$value</td>";
		}
		echo "</tr>";
	}
	
	// End the table
	echo "</table>";
	
	/*
	if($creator == ucfirst(strtolower($user))) {
		echo '<input type="submit" name="submit" value="Modify Items">';
	}
	
	// End the form
	echo "</form>";
	*/
	
	if($creator != ucfirst(strtolower($user))) {
		if(! isset($_POST['submit'])) {
			$_POST['submit'] = '';
		}
		//echo "Submit:<br>"; //debug
		//echo $_POST['submit']; //debug
	
		switch($_POST['submit']) {
			case 'Make Bid':
				makeBid($user, $LinkID, $orderId);
				break;
			case 'Submit Bid':
				finalizeBid($user, $LinkID, $orderId);
			default:
				// Bid form
				echo '<form method="post" action="orderSummary.php?orderId=' . $orderId . '">';
				
				// Add submit button to make a bid
				echo '<br><input type="submit" name="submit" value="Make Bid">';
				
				echo '</form';
		}
	}
	
	$allBids = getBids($user, $LinkID, $orderId);
	echo "<br><h2>Bids:</h2>";
	
	if(empty($allBids)) {
		echo "There are no bids at this time.<br>";
		return;
	}
	
	// Make a form
	echo '<form method="post" action="orderSummary.php?orderId=' . $orderId . '">';
	
	// Make a table
	echo "<table><tr>";
	
	// print the column labels
	if($creator == ucfirst(strtolower($user))) {
		echo '<td></td>';
	}
	foreach(array_keys($allBids[0]) as $column) {
		echo "<td><b>$column</b></td>";
	}
	echo "</tr>";
	
	// print each row of values
	foreach($allBids as $bid) {
		if($creator == ucfirst(strtolower($user))) {
			echo '<tr><td><input type="radio" name="bid" value="' . $bid['ID'] . '"></td>';
		}
		foreach($bid as $value) {
			echo "<td>$value</td>";
		}
		echo "</tr>";
	}
	
	// End the table
	echo "</table>";
	
	if($creator == ucfirst(strtolower($user))) {
		echo '<td><input type="submit" name="submit" value="Accept Bid"></td>';
	}
	
	// End the form
	echo "</form>";
}

function getOrderInfo ($LinkID, $orderId) {
	/*
	Select orderType, orderValue, creator, recipient, creationDate, expirationDate
	From orders
	Where orderId=$orderId;
	*/
	$query = "Select orderType, orderValue, creator, recipient, expirationDate From orders Where orderId=$orderId";
	//echo"$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	$orderInfo = null;

	// Get all the order's info and add it to an array
	while ($row = mysql_fetch_assoc($result)) {
		//echo"Row:<br>";	//debug
		//print_r($row);	//debug
		//echo"<br><br>";	//debug
		$orderInfo = array('Type' => $row['orderType'], 'Value' => $row['orderValue'], 'Source' => $row['creator'], 'Destination' => $row['recipient'], 'Expires' => $row['expirationDate']);
	}
	
	return $orderInfo;
}

function getOrderItems ($LinkID, $orderId) {
	/*
	Select item, itemQuality, itemQuantity
	From orderItems
	Where orderId=$orderId;
	*/
	$query = "Select item, itemQuality, itemQuantity From orderItems Where orderId=$orderId";
	//echo"$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	$orderItems = null;

	// Get all the order's info and add it to an array
	while ($row = mysql_fetch_assoc($result)) {
		//echo"Row:<br>";	//debug
		//print_r($row);	//debug
		//echo"<br><br>";	//debug
		$orderItems[$row['item']] = array($row['itemQuality'] => $row['itemQuantity']);
	}
	
	return $orderItems;
}

function modifyOrder ($user, $LinkID, $orderId) {
	$user = ucfirst(strtolower($user));
	
	// Make the Modify Order form
	echo '<form method="post" action="orderSummary.php?orderId=' . $orderId . '">';
	
	$orderInfo = getOrderInfo($LinkID, $orderId);

	$type = $orderInfo['Type'];
	$value = $orderInfo['Value'];
	$source = $orderInfo['Source'];
	$destination = $orderInfo['Destination'];
	$expires = $orderInfo['Expires'];
	
	// Change the source building
	/*
	Select username, buildingId, buildingName, structureType, district
	From buildings
	Order by username, district, structureType;
	*/
	$query = "Select username, buildingId, buildingName, structureType, district From buildings Order by username, district, structureType";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	$allBuildings = null;

	// Get all the buildings and add them to an array
	while ($row = mysql_fetch_assoc($result)) {
		//echo"Row:<br>";	//debug
		//print_r($row);	//debug
		//echo"<br><br>";	//debug
		$allBuildings[] = $row;
	}
	
	echo'<b>Change the building</b><br><select name="source">';
	foreach ($allBuildings as $building) {
		if(ucfirst(strtolower($building['username'])) != $user) {
			continue;
		}
		$buildingId = $building['buildingId'];
		//echo"ID: $buildingId<br>"; //debug
		$buildingName = $building['buildingName'];
		//echo"Name: $buildingName<br>"; //debug
		$structureType = $building['structureType'];
		//echo"Type: $structureType<br>"; //debug
		$district = $building['district'];
		//echo"District: $district<br>"; //debug
		echo '<option value="' . $buildingId . '"';
		if($buildingId == $source) {
			echo 'selected';
		}
		echo '>';
		if(! $buildingName) {
			echo "$structureType $buildingId in the $district";
		}
		else{
			echo"$buildingName";
		}
		
		echo "</option>";
	}
	echo"</select><br>";
	
	// Set the order type
	echo'<b>Type:</b><br><select name="type">';
	$allTypes = array('Transfer', 'Buy', 'Sell');
	foreach($allTypes as $aType) {
		echo '<option value="' . $aType . '"';
		if($aType == $type) {
			echo 'selected';
		}
		echo '>';
		echo $aType;
		echo '</option>';
	}
	echo '</select><br>';
	
	// Set a value
	echo'<b>Value:</b><br><input type="number" name="value" value="' . $value . '" min="0" max="9999"><br>';

	// Choose a destination
	echo'<b>Destination:</b><br><select name="destination">';
	echo'<option value="">None</option>';
	foreach ($allBuildings as $building) {
		$username = ucfirst(strtolower($building['username']));
		//echo"Username: $username<br>"; //debug
		$buildingId = $building['buildingId'];
		//echo"ID: $buildingId<br>"; //debug
		$buildingName = $building['buildingName'];
		//echo"Name: $buildingName<br>"; //debug
		$structureType = $building['structureType'];
		//echo"Type: $structureType<br>"; //debug
		$district = $building['district'];
		//echo"District: $district<br>"; //debug
		
		echo '<option value="' . $buildingId . '"';
		if($buildingId == $destination) {
			echo 'selected';
		}
		echo '>';
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
	echo"</select><br>";
	
	// Set an expiration date
	echo'<b>Choose an expiration date:</b><br><input type="date" name="expiration" value=' . $expires . '><br>';
	
	// Add submit button to create the order
	echo'<br><input type="submit" name="submit" value="Finish Editing">';

	// End the form
	echo"</form>";
}

function modifyOrderItems ($user, $LinkID, $orderId) {
	$user = ucfirst(strtolower($user));
	
	$orderInfo = getOrderInfo($LinkID, $orderId);
	
	$type = $orderInfo['Type'];
	$value = $orderInfo['Value'];
	$source = $orderInfo['Source'];
	$destination = $orderInfo['Destination'];
	$expires = $orderInfo['Expires'];
	
	$orderItems = getOrderItems($LinkID, $orderId);
	//echo "Order Items:<br>";	//debug
	//print_r($orderItems);	//debug
	//echo "<br><br>";	//debug
	
	// Transferring or Selling
	if($type != 'Buy') {
		// Get the source's inventory
		/*
		Select goodName, goodQuality, goodQuantity, reserved
		From buildingGoods
		Where buildingId = 1
		Order by goodName, goodQuality;
		*/
		$query = "Select goodName, goodQuality, goodQuantity, reserved From buildingGoods Where buildingId = $source Order by goodName, goodQuality";
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
			$allGoods[] = $row;
		}
	// Buying
	} else {
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
			if($row['goodQuantity'] == 0){
				continue;
			}
			$allGoods[] = $row;
		}
	}
	
	if(! $allGoods) {
		echo "You have no goods.";
		return;
	}

	//echo "allGoods:<br>";	//debug
	//print_r($allGoods);	//debug
	//echo "<br><br>";	//debug
	
	// Make the Modify Order form
	echo '<form method="post" action="orderSummary.php?orderId=' . $orderId . '">';
	$orderInfo = getOrderInfo($LinkID, $orderId);
	
	// Build a table of all the goods
	echo "<table>";
	
	// Print the column names
	echo "<tr><td><b>Good</b></td>
			<td><b>Quality</b></td>
			<td><b>Quantity</b></td></tr>";
	
	foreach($allGoods as $good) {
		$goodName = $good['goodName'];
		$goodQuality = $good['goodQuality'];
		if($goodQuantity = $good['goodQuantity']) {
			$_SESSION['order'][$goodName] = $goodQuantity;
		}
		if($goodReserved = $good['reserved']) {
			if(! $_SESSION['order'][$goodName]['reserved']) {
				$_SESSION['order'][$goodName]['reserved'] = 0;
			}
			//echo $_SESSION['order'][$goodName]['reserved']; //debug
			//echo "<br>"; //debug
			//echo intval($goodReserved); //debug
			$_SESSION['order'][$goodName]['reserved'] = $_SESSION['order'][$goodName]['reserved'] + intval($goodReserved);
		}
		
		// Print each row
		echo "<tr>";
		echo '<td><input type="checkbox" name="' . "$goodName,$goodQuality" . '" value="';
		echo '">' . $goodName . '</td>';
		echo "<td>$goodQuality</td>";
		echo '<td><input type="number" name="' . "$goodName,$goodQuality" . '" value="';
		if(! isset($orderItems[$goodName][$goodQuality])) {
			$orderItems[$goodName][$goodQuality] = 0;
		}
		if($orderItems[$goodName][$goodQuality] != 0) {
			echo $orderItems[$goodName][$goodQuality];
		} else {
			echo '0';
		}
		echo '" min="0"';
		if(isset($goodQuantity)) {
			echo ' max="' . $goodQuantity . '">';
		}else {
			echo ' max="99">';
		}
		echo "</td></tr>";
	}
	
	// End the table
	echo "</table>";
	
	// Add submit button to finalize the order
	echo '<br><input type="submit" name="submit" value="Finish Editing Items">';

	// End the form
	echo "</form>";
}

function modifyOrderFinished ($user, $LinkID, $orderId) {
	$source = $_POST['source'];
	$type = $_POST['type'];
	$value = $_POST['value'];
	$destination = $_POST['destination'];
	$expiration = $_POST['expiration'];
	
	/*
	Update orders
	Set orderType='$type', orderValue=$value, creator=$source, recipient=$destination, expirationDate=$expiration
	Where orderId=$orderId;
	*/
	$query = "Update orders Set orderType='$type', orderValue=$value, creator=$source, ";
	if($destination != '') {
		$query = $query . "recipient=$destination, ";
	}
	$query = $query . "expirationDate=$expiration";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
}

function modifyOrderItemsFinished ($user, $LinkID, $orderId) {
	// Get posted values and add to an array
	$orderItems = null;
	foreach($_POST as $key => $value) {
		if($value == '0' or $value == '') {
			continue;
		}
		if($key == 'submit') {
			continue;
		}
		$goodNameAndQuality = explode(',', $key);
		$goodName = $goodNameAndQuality[0];
		$goodQuality = $goodNameAndQuality[1];
		$goodQuantity = $value;
		$orderItems[] = array('goodName' => $goodName, 'goodQuality' => $goodQuality, 'goodQuantity' => $goodQuantity);
	}
	
	//print "Order items:<br>"; //debug
	//print_r($orderItems); //debug
	//print "<br>"; //debug
	
	if(empty($orderItems)) {
		echo '<form method="post" action="orders.php">No items were included in the order<br><input type="submit" value="Cancel"></form>';
		return;
	}
	
	/*
	Delete
	From orderItems
	Where orderId=$orderId;
	*/
	$query = "Delete From orderItems Where orderId=$orderId";
	//print "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
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
		//print "$query<br>";	//debug
		$result = mysql_query($query ,$LinkID);
		echo mysql_error($LinkID);
		
		if($type == 'Buy') {
			continue;
		}
		
		//print "Current quantity: " . $_SESSION['order'][$goodName] . "<br>"; //debug
		$newQuantity = $_SESSION['order'][$goodName] - $goodQuantity;
		//print "Current reserve: " . $_SESSION['order'][$goodName]['reserved'] . "<br>"; //debug
		$newReserved = $_SESSION['order'][$goodName]['reserved'] + $goodQuantity;
		
		// If Selling or Transferring move goods to reserve
		/*
		Update buildingGoods
		Set goodQuantity=$currentQuantity-$goodQuantity, reserved=$goodQuantity
		Where goodName='$goodName'
		And buildingId=$source;
		*/
		$query = "Update buildingGoods Set goodQuantity=$newQuantity, reserved=$newReserved Where goodName='$goodName' And buildingId=$source";
		//print "$query<br>";	//debug
		$result = mysql_query($query ,$LinkID);
		echo mysql_error($LinkID);
	}
}

function getBids($user, $LinkID, $orderId) {
	/*
	Select bidId, orderId, concat('$', CAST(bidValue AS CHAR)), bidder, recipient
	From orderBids
	Where orderId=$orderId;
	*/
	$query = "Select bidId as ID, orderId as 'Order ID', concat('$', CAST(bidValue AS CHAR)) as 'Bid', bidder as 'Bidder', recipient as 'Destination' From orderBids Where orderId=$orderId";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	if(! $result) {
		return null;
	}
	
	$allBids = null;
	
	// Get the order's information and add it to the array
	while ($row = mysql_fetch_assoc($result)) {
		$allBids[] = $row;
	}
	
	return $allBids;
}

function makeBid ($user, $LinkID, $orderId) {
	$user = ucfirst(strtolower($user));

	echo "<h2>Make Bid on Order $orderId</h2>";
	// Make the form
	echo '<form method="post" action="orderSummary.php?orderId=' . $orderId . '">';
	
	// Set a bid value
	/*
	Select money
	From users
	Where username='Darkreaper';
	*/
	$query = "Select money from users Where username='$user'";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	$row = mysql_fetch_row($result);
	$money = $row[0];
	//echo "Money: $money<br>"; //debug
	
	echo '<b>Value:</b><br><input type="number" name="value" value="0" min="0" max="' . $money . '"><br>';
	
	// Choose a destination
	/*
	Select buildingId, buildingName, structureType, district
	From buildings
	Where username='Darkreaper';
	*/
	$query = "Select buildingId, buildingName, structureType, district From buildings Where username='$user'";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	$allBuildings = null;

	// Get all the orders and add them to an array
	while ($row = mysql_fetch_assoc($result)) {
		//echo "Row:<br>";	//debug
		//print_r($row);	//debug
		//echo "<br><br>";	//debug
		$allBuildings[] = $row;
	}
	
	echo '<b>Destination:</b><br><select name="destination">';
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
			echo "<option value=\"$buildingId\">$structureType $buildingId in the $district</option>";
			continue;
		}
		echo "<option value=\"$buildingId\">$buildingName</option>";
	}
	echo "</select><br>";
	
	// Submit button to finalize bid
	echo '<br><input type="submit" name="submit" value="Submit Bid">';

	// End the form
	echo '</form>';
}

function finalizeBid ($user, $LinkID, $orderId) {
	$bidValue = $_POST['value'];
	$destination = $_POST['destination'];

	$orderInfo = getOrderInfo($LinkID, $orderId);
	$type = $orderInfo['Type'];
	$value = $orderInfo['Value'];
	if(($type == 'Buy') or ($type == 'Demand')) {
		$orderItems = getOrderItems($LinkID, $orderId);
		
		//echo "Order Items:<br>"; //debug
		//print_r($orderItems); //debug
		//echo "<br><br>"; //debug
		
		// Make a list of all the order Items
		$allGoods = '';
		foreach($orderItems as $itemName => $item) {
			if($allGoods != '') {
				$allGoods = $allGoods . ", '$itemName'";
			} else {
				$allGoods = $allGoods . "'$itemName'";
			}
		}
		
		//echo "All goods: $allGoods<br>"; //debug
		
		// Get bidder's current inventory
		$inventory = getCurrentInventory($LinkID, $destination, $allGoods);
		
		//echo "Inventory:<br>"; //debug
		//print_r($inventory); //debug
		//echo "<br><br>"; //debug
		
		// Check that bidder has all order items in their inventory
		foreach($orderItems as $itemName => $item) {
			foreach($item as $itemQuality => $itemQuantity) {
				if($inventory[$itemName][$itemQuality] < $itemQuantity) {
					echo "You do not have enough $itemQuality $itemName to bid on this order.<br>";
					return;
				}
			}
		}
	}
	
	if($type == 'Demand') {
		demandIsMet($LinkID, $orderId, $user, $destination, $value);
		return;
	}
	
	// Insert the bid into the database
	/*
	Insert into orderBids
	(orderId, bidValue, bidder, recipient)
	Values
	($orderId, $bidValue, $user, $destination);
	*/
	$query = "Insert into orderBids (orderId, bidValue, bidder, recipient) Values ($orderId, $bidValue, '$user', $destination)";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
}

function demandIsMet ($LinkID, $orderId, $bidder, $destination, $amount) {
	$orderItems = getOrderItems ($LinkID, $orderId);
	
	$goodsToRemove = null;
	foreach($orderItems as $itemName => $item) {
		foreach($item as $itemQuality => $itemQuantity) {
			$goodsToRemove[$itemName][$itemQuality] = (-1) * $itemQuantity;
		}
	}
	
	updateInventory($LinkID, $destination, null, $goodsToRemove, false);
	
	// Transfer payment
	changeUserMoney ($bidder, $LinkID, $amount);
	
	// Remove the order
	removeOrder($LinkID, $orderId);
	
	echo "Demand has been met.<br>";
	echo '<form method="post" action="orders.php">';
	echo '<input type="submit" name="submit" value="Ok">';
	echo '</form>';
}

function changeUsersMoney ($user, $LinkID, $amount) {
	if($amount <= 0) {
		return;
	}

	$query = "Update users Set money=money+$amount Where username='$user'";
	//echo "$query<br>";	//debug
	$results = mysql_query($query, $LinkID);
}

function acceptBid ($user, $LinkID, $orderId)  {
	if(! isset($_POST['bid'])) {
		return;
	}
	
	$chosenBid = $_POST['bid'];
	
	if($chosenBid == '') {
		return;
	}
	/*
	Select bidValue, bidder, recipient
	From orderBids
	Where bidId=$chosenBid;
	*/
	$query = "Select bidValue, bidder, recipient From orderBids Where bidId=$chosenBid";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	// Get the bid's information and add it to the array
	while ($row = mysql_fetch_assoc($result)) {
		//echo "Row:<br>";	//debug
		//print_r($row);	//debug
		//echo "<br><br>";	//debug
		$bidInfo = array('bidValue'=>$row['bidValue'], 'bidder'=>$row['bidder'], 'recipient'=>$row['recipient']);
	}
	
	$value = $bidInfo['bidValue'];
	$winningBidder = $bidInfo['bidder'];
	$recipient = $bidInfo['recipient'];
	
	/*
	Select orderType, creator, recipient
	From orders
	Where orderId=$orderId;
	*/
	$query = "Select orderType as 'Type', creator as 'Source', recipient as 'Destination' From orders Where orderId=$orderId";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	$orderInfo = null;
	// Get the order's information and add it to the array
	while ($row = mysql_fetch_assoc($result)) {
		//echo "Row:<br>";	//debug
		//print_r($row);	//debug
		//echo "<br><br>";	//debug
		$orderInfo = array('Type'=>$row['Type'], 'Source'=>$row['Source'], 'Destination'=>$row['Destination']);
	}
	//echo "Order Info:<br>";	//debug
	//print_r($orderInfo);	//debug
	//echo "<br><br>";	//debug
	
	$type = $orderInfo['Type'];
	//echo "Type: $type<br>"; //debug
	
	switch ($type) {
		case 'Buy':
			$source = $recipient;
			$destination = $orderInfo['Source'];
			$buyer = $user;
			$seller = $winningBidder;
			break;
		case 'Sell':
			$source = $orderInfo['Source'];
			$destination = $recipient;
			$buyer = $winningBidder;
			$seller = $user;
			break;
		case 'Transfer':
			$source = $orderInfo['Source'];
			$destination = $row['Destination'];
			break;
	}
	/*
	Select item, itemQuality, itemQuantity
	From orderItems
	Where orderId=$orderId;
	*/
	$query = "Select item, itemQuality, itemQuantity From orderItems Where orderId=$orderId";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	$goodsToAdd = null;
	$goodsToRemove = null;
	$allGoods = '';
		
	while ($row = mysql_fetch_assoc($result)) {
		$goodsToAdd[$row['item']][$row['itemQuality']] = $row['itemQuantity'];
		$goodsToRemove[$row['item']][$row['itemQuality']] = $row['itemQuantity'] * (-1);
		$itemName = $row['item'];
		if($allGoods != '') {
			$allGoods = $allGoods . ", '$itemName'";
		} else {
			$allGoods = $allGoods . "'$itemName'";
		}
	}
	//echo "Goods to add:<br>";	//debug
	//print_r($goodsToAdd);	//debug
	//echo "<br><br>";	//debug
	
	//echo "Goods to remove:<br>";	//debug
	//print_r($goodsToRemove);	//debug
	//echo "<br><br>";	//debug
	
	// Determine quantities already existing in destination's inventory
	// allGoods is a string of all goods to be updated separated by commas
	$totalGoodsAtDestination = getCurrentInventory($LinkID, $destination, $allGoods);
	
	// Add to destination's inventory
	//echo "Update destination:<br>"; //debug
	updateInventory($LinkID, $destination, $totalGoodsAtDestination, $goodsToAdd, false);
	//echo "<br>"; //debug
	
	// Determine quantities already existing in source's reserves
	// allGoods is a string of all goods to be updated separated by commas
	$totalGoodsAtSource = getCurrentReserve($LinkID, $source, $allGoods);
	
	// Remove from source's reserve quantities
	//echo "Update source:<br>"; //debug
	updateReserve($LinkID, $source, $totalGoodsAtSource, $goodsToRemove);
	//echo "<br>"; //debug
	
	// Transfer payment
	if($type != 'Transfer') {
		transferPayment($LinkID, $buyer, $seller, $value);
	}
	
	// Remove the order
	removeOrder($LinkID, $orderId);
	
	echo "Order has been filled.<br>";
	echo '<form method="post" action="orders.php">';
	echo '<input type="submit" name="submit" value="Ok">';
	echo '</form>';
}

function cancelOrder($LinkID, $orderId) {
	$orderInfo = getOrderInfo($LinkID, $orderId);
	$orderType = $orderInfo['Type'];
	
	if($orderType == 'Buy') {
		removeOrder($LinkID, $orderId);
		return;
	}

	$orderItems = getOrderItems($LinkID, $orderId);
	
	//echo "Order Items:<br>";	//debug
	//print_r($orderItems);	//debug
	//echo "<br><br>";	//debug
	
	$allGoods = '';
	foreach($orderItems as $itemName => $item) {
		if($allGoods != '') {
			$allGoods = $allGoods . ", '$itemName'";
		} else {
			$allGoods = $allGoods . "'$itemName'";
		}
		foreach($item as $itemQuality => $itemQuantity) {
			$orderItemsToRemove[$itemName][$itemQuality] = $itemQuantity * (-1);
		}
	}
	
	//echo "All goods: $allGoods<br>"; //debug
	
	// Get the building Id
	/*
	Select creator
	From orders
	Where orderId=$orderId;
	*/
	$query = "Select creator From orders Where orderId=$orderId";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	$row = mysql_fetch_assoc($result);
	$buildingId = $row['creator'];
	
	//echo "Building Id: $buildingId<br>";
	
	// Get the current Inventory
	$inventory = getCurrentInventory($LinkID, $buildingId, $allGoods);
	
	//echo "Inventory:<br>";	//debug
	//print_r($inventory);	//debug
	//echo "<br><br>";	//debug
	
	// Get the current reserve
	$reserve = getCurrentReserve($LinkID, $buildingId, $allGoods);
	
	//echo "Reserve:<br>";	//debug
	//print_r($reserve);	//debug
	//echo "<br><br>";	//debug
	
	updateInventory($LinkID, $buildingId, $inventory, $orderItems, false);
	updateReserve($LinkID, $buildingId, $reserve, $orderItemsToRemove);
	
	removeOrder($LinkID, $orderId);
}

function removeOrder($LinkID, $orderId) {
	/*
	Delete
	From orders
	Where orderId=$orderId;
	*/
	$query = "Delete From orders Where orderId=$orderId";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	/*
	Delete
	From orderItems
	Where orderId=$orderId;
	*/
	$query = "Delete From orderItems Where orderId=$orderId";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	/*
	Delete
	From orderBids
	Where orderId=$orderId;
	*/
	$query = "Delete From orderBids Where orderId=$orderId";
	//echo "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
}

function getCurrentReserve($LinkID, $buildingId, $allGoods) {
	$query = "Select goodName, goodQuality, reserved from buildingGoods Where buildingId = '$buildingId' And goodName in($allGoods)";
	//echo "$query<br>";	//debug
	$result = mysql_query($query, $LinkID);
	
	$totalGoods = array();
	while ($row = mysql_fetch_assoc($result)) {
		$totalGoods[$row['goodName']][$row['goodQuality']] = $row['reserved'];
	}
	//echo "Total goods:<br>";	//debug
	//print_r($totalGoods);	//debug
	//echo "<br><br>";	//debug
	return $totalGoods;
}

function updateReserve($LinkID, $buildingId, $currentGoods, $goodsToChange) {
	//echo "Current goods:<br>";	//debug
	//print_r($currentGoods);	//debug
	//echo "<br><br>";	//debug	
	
	//echo "Goods to remove:<br>";	//debug
	//print_r($goodsToChange);	//debug
	//echo "<br><br>";	//debug
	
	if (! empty($goodsToChange)) {
		foreach($goodsToChange As $productName => $product) {
			foreach($product As $productQuality => $productQuantity) {
				 $currentGoods[$productName][$productQuality] += $productQuantity;
			}
		}
	}
	//echo "New goods quantity:<br>";	//debug
	//print_r($currentGoods);	//debug
	//echo "<br><br>";	//debug
	
	// Change the reserved amount in the building inventory
	foreach($currentGoods As $productName => $product) {
		foreach($product As $productQuality => $productQuantity) {
			$query = "Update buildingGoods Set reserved = $productQuantity Where goodName = '$productName' And goodQuality = '$productQuality' And buildingId = $buildingId";
			//echo "$query<br>";	//debug
			$result = mysql_query($query, $LinkID);
		}
	}
}

function transferPayment($LinkID, $payer, $payee, $amount) {
	$query = "Select money from users Where username='$payer'";
	//echo "$query<br>";	//debug
	$result = mysql_query($query, $LinkID);
	
	while ($row = mysql_fetch_assoc($result)) {
		$currentMoneyPayer = $row['money'];
	}
	
	$query = "Select money from users Where username='$payee'";
	//echo "$query<br>";	//debug
	$result = mysql_query($query, $LinkID);
	
	while ($row = mysql_fetch_assoc($result)) {
		$currentMoneyPayee = $row['money'];
	}
	
	$newMoneyPayer = $currentMoneyPayer - $amount;
		
	$query = "Update users Set money=$newMoneyPayer Where username='$payer'";
	//echo "$query<br>";	//debug
	$results = mysql_query($query, $LinkID);
	
	$newMoneyPayee = $currentMoneyPayee + $amount;
	
	$query = "Update users Set money=$newMoneyPayee Where username='$payee'";
	//echo "$query<br>";	//debug
	$results = mysql_query($query, $LinkID);
}
?>