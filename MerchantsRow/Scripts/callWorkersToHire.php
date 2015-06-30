<?php
function CallWorkersToHire($LinkID) {
	if (isset($_SESSION['bidPlaced'])) {
		echo $_SESSION['bidPlaced'];
		$_SESSION['bidPlaced'] = null;
	}

	$workerQuery = "Select * from workers where workerId not in (Select workerId from buildingWorkers)";
	$workerResult = mysql_query($workerQuery, $LinkID);
	
	if ($workerResult == false) {
		echo "There are currently no workers for hire";
		return;
	}
	echo mysql_error($LinkID);	//debug
	
	echo "<table>";
	echo "<tr><td><b>Worker Name</b></td><td><b>Bidding Ends</b></td><td><b>Offered Wage</b></td><td><b>Building to Use</b></td></tr>";
	
	$allBuildings = null;
	$allBuildings = Array();
	$buildingResult = mysql_query("select * from buildings where username = '$_SESSION[user]'");
	while ($row = mysql_fetch_row($buildingResult)) {
		$allBuildings[] = $row;
	}
	
	while ($row = mysql_fetch_assoc($workerResult)) {
		$bidQuery = "Select expirationDate from workerBids where workerId = $row[workerId]";
		$bidResult = mysql_query($bidQuery, $LinkID);
		$expiry = mysql_fetch_row($bidResult);
		
		echo "<form method='post' action='../scripts/placeWorkerBid.php'>";
		echo "<tr>
		<td>$row[workerName]</td>
		<td>$expiry[0]</td>
		<td width=5><input type='number' required name='bid' min=1></td>
		<td><select name='hiree' required>";
		
		foreach ($allBuildings as $building) {
			echo "<option value=$building[0]>$building[2] $building[0] in the $building[3]</option>";
		}
		
		echo "</select></td>
		<td><button type='submit' id='submitbid'>Submit Bid</button></td>
		<td><input type='hidden' value=$row[workerId] name='workerID'></td>
		</tr></form>";
	}
	echo "</table>";
	echo mysql_error($LinkID);	//debug
}
?>