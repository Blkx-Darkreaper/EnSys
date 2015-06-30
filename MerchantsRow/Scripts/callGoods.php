 <?php
	//Include the file to connect to the database

	// Choose the DB and run a query.
	mysql_select_db($database, $LinkID);
   	/*
	Select bg.goodName, bg.goodQuantity
	From buildingGoods bg left join buildings b using(buildingId)
	Where b.username = 'Lady3lle'
	AND bg.goodName IN ('stone', 'wood', 'fish', 'venison', 'grain', 'water', 'iron ore', 'gold ore', 'coal', 'wool', 'grapes', 'milk')
	GROUP BY bg.goodName;
	*/
	$user = $_SESSION['user'];
	$query = "SELECT bg.goodName As 'Item', bg.goodQuantity As 'Quantity' 
		  FROM buildingGoods bg left join  buildings b using (buildingId) 
		  WHERE b.username = '$user' 
		  Order BY bg.goodName";
	$result = mysql_query($query,$LinkID);
	echo mysql_error($LinkID);

	// Fetch a row with the column labels
	$row=mysql_fetch_assoc($result);
	if (! $row) {
		return;
	}
	  
   // Print the column labels
	print "<table border=.5 align=center width=198 pixels><tr>";
	foreach (array_keys($row) as $column) {
		print "<td><b>$column</b></td>";
	}
	print "</tr>";
/*     // Print the values for the first row
	foreach ($row as $value) {
		print "<td>$value</td>";
	}
	print "</tr><tr>"; */
    // Print the rest of the rows
	while ($row=mysql_fetch_row($result)) {
		print "<tr>";
		foreach ($row as $value) {
			print "<td>$value</td>";
		}
		print "</tr>";
	}
	//end table
	print "</table>";
?>
