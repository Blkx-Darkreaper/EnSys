<?php

function callAllOrdersInfo ($LinkID) {
	/*
	Select orderId, orderType, concat('$', CAST(orderValue AS CHAR)), creator, recipient, creationDate, expirationDate
	From orders
	Order by creator, expirationDate, creationDate, orderId;
	*/
	$query = "Select orderId as 'Order No.', orderType as 'Type', concat('$', CAST(orderValue AS CHAR)) as 'Value', creator as 'Source', recipient as 'Destination', creationDate as 'Created', expirationDate as 'Expires' From orders Order by creator, expirationDate, creationDate, orderId";
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
	
	return $allOrders;
}

?>