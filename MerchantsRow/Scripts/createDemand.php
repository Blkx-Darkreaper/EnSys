<?php

function createDemand($LinkID) {
	/*
	Select count(username) as 'All users', (Select count(username) From users Where loggedIn > loggedOut) as 'Current players', sum(money) as 'Wealth'
	From users;
	*/
	$query = "Select count(username) as 'Total users', (Select count(username) From users Where loggedIn > loggedOut) as 'Current players', sum(money) as 'Wealth' From users";
	//echo "$query<br>";	//debug
	$result = mysql_query($query , $LinkID);
	echo mysql_error($LinkID);
	
	while($row = mysql_fetch_assoc($result)) {
		$userData[] = $row;
	}
	
	//echo "User data:<br>"; //debug
	//print_r($userData); //debug
	//echo "<br><br>"; //debug
	
	$totalUsers = $userData[0]['Total users'];
	$currentPlayers = $userData[0]['Current players'];
	$wealth = $userData[0]['Wealth'];
	
	$maxDemands = intval(ceil($totalUsers/5));
	//echo "Max demands: $maxDemands<br>"; //debug
	
	/*
	Select count(orderId)
	From orders
	Where orderType='Demand';
	*/
	$query = "Select count(orderId) as 'current Demands' From orders Where orderType='Demand'";
	//echo "$query<br>";	//debug
	$result = mysql_query($query , $LinkID);
	echo mysql_error($LinkID);
	
	while($row = mysql_fetch_assoc($result)) {
		$currentDemands = $row['current Demands'];
	}
	
	$demandProb = ($maxDemands - $currentDemands)/(10 + $currentPlayers * 2.5);
	$demand = rand(0, $maxDemands);
	
	if($demand > $demandProb) {
		return;
	}
	
	$difficulty = intval(log10($wealth / $totalUsers));
	
	// Find the most uncommon goods amongst all users
	/*
	Select goodName
	From (Select goodName, sum(goodQuantity) as goodTotal
		From buildingGoods
		Group by goodName
		Order by goodTotal) as totals
	Where goodTotal = (Select min(goodQuantity) 
		From buildingGoods)
	Order by goodName;
	*/
	$query = "Select goodName From (Select goodName, sum(goodQuantity) as goodTotal From buildingGoods Group by goodName Order by goodTotal) as totals Where goodTotal = (Select min(goodQuantity) From buildingGoods) Order by goodName";
	$result = mysql_query($query , $LinkID);
	echo mysql_error($LinkID);
	
	$goodsToPick = null;
	while($row = mysql_fetch_assoc($result)) {
		$goodsToPick[] = $row['goodName'];
	}
	
	//echo "Goods to pick:<br>"; //debug
	//print_r($goodsToPick); //debug
	//echo "<br><br>"; //debug
	
	//echo "Demands: $numberOfDemands<br>"; //debug
	
	// Pick a good at random and create a demand for it
	$pickedGoodIndex = array_rand($goodsToPick);
	$pickedGood = $goodsToPick[$pickedGoodIndex];
	
	//echo "Picked good: $pickedGood<br>"; //debug
	
	$goodQuantity = $difficulty * 5;
	$value = $difficulty * 10;
	$expiration = date('Y-m-d', time() + ((14/$difficulty) * 24 * 60 * 60));
	
	$orderItems[$pickedGood]['good'] = $goodQuantity;
	
	createOrder ($LinkID, 'Demand', $value, 0, '', $expiration, $orderItems);
}

function createOrder ($LinkID, $type, $value, $source, $destination, $expiration, $orderItems) {
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
	
	//echo "Order Items:<br>"; //debug
	//print_r($orderItems); //debug
	//echo "<br><br>"; //debug
	
	foreach($orderItems As $goodName => $item) {
		foreach($item as $goodQuality => $goodQuantity) {
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
		}
	}
}
?>