<?php
	// Choose the DB and run a query.
	mysql_select_db($database, $LinkID);

	$user = $_SESSION['user'];
	$query = "SELECT CONCAT('Workers: ', COUNT(bw.workerId), '  Structures: ', COUNT(Distinct b.buildingId), '  Florins: $', u.money) AS ''
				FROM buildings b left join buildingWorkers bw using(buildingId) inner join users u using(username)
				WHERE username = '$user'
				Group by username";
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);

	//Fetch a row with column labels
	$row=mysql_fetch_assoc($result);
	if (! $row) {
		return;
	}
	
	// Print the column labels
	print "<table border=0><tr>";
	foreach (array_keys($row) as $column) {
		print "<td><b>$column</b></td>";
	}
	print "</tr>";
	// Print the values for the first row
	foreach ($row as $value) {
		print "<td><b>$value<b></td></tr>";
	}
	//end table
	print "</table>";
?>