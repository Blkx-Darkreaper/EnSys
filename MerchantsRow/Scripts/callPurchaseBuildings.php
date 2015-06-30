<?php
if (isset($_SESSION['buildingPurchased'])) {
		echo $_SESSION['buildingPurchased'];
		$_SESSION['buildingPurchased'] = null;
	}
	
//run a query to get all of the structure information 
$buildingResult = mysql_query("Select * from structures", $LinkID);

//start the table and print the column titles
echo "<table class='center-cell' id='cellspacing'><tr><td><b>Structure Type</b></td><td><b>Structure Size</b></td><td><b>Max Workers</b></td><td><b>Storage Space</b></td><td><b>Cost</b></td></tr>";

//print each of the structure types and their info
while ($buildingInfo = mysql_fetch_row($buildingResult)) {
	echo "<tr>";
	foreach ($buildingInfo as $value) {
		echo "<td>$value</td>";
	}
	echo "<td><form method='post' action='../scripts/purchaseBuilding.php'><input type='hidden' value='$buildingInfo[0]' name='type'><button type='submit' id='purchasebuilding'>Purchase</buton></form></td>";
	echo "</tr>";
}
echo "</table>";

?>