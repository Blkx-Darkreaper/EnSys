<?php
	// Choose the DB and run a query.
	mysql_select_db($database, $LinkID);


/* bw.workerId), '  Structures: ', COUNT(Distinct b.buildingId), '  Gold: ', u.money) AS ''
				FROM buildings b left join buildingWorkers bw using(buildingId) inner join users u using(username)
*/
	$user = $_SESSION['user'];
	$query = "SELECT b.structureType As 'Building', 
					b.district As 'Location', COUNT(bw.workerId) As 'Workers',
					b.buildingName As 'Building Name'
				FROM buildings b left join buildingWorkers bw using(buildingId)
				WHERE username = '$user'
				GROUP BY buildingId";
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);
	
	//Print the table headers
	echo "<table class='building-report' border=0><tr><td><b>Type</b></td><td><b>District</b></td><td><b>Workers</b></td><td><b>Name</b></td></tr>";
	
	//Print each row of data
	while ($buildingInfo = mysql_fetch_row($result)) {
		echo "<tr><td>$buildingInfo[0]</td><td>$buildingInfo[1]</td><td>$buildingInfo[2]</td><td>$buildingInfo[3]</td></tr>";
	}
	
	//end table
	print "</table>";
	if(! isset($_POST['submit'])){
		$_POST['submit'] = '';
	}
	
	switch($_POST['submit']){
		case 'Change Building Name':
			setBuildingName($user, $LinkID);
			break;
		default:
			echo "<div><h1>Change Building Names</h1>";
			changeBuildingName($user, $LinkID);
			echo "</div>";
	}
	
	function changeBuildingName($user, $LinkID){
	
		$user = ucfirst(strtolower($user));
		//Query DB for user's buildings
		$query = "
		Select username, buildingId, buildingName, structureType, district 
		From buildings 
		Order by username, district, structureType";
		
		$result = mysql_query($query ,$LinkID);
		echo mysql_error($LinkID);
		
		if(! $result) {
			echo "You do not have any buildings.";
			return;
		}
		
		$allBuildings = null;

		// Get all the buildings and add them to an array
		while ($row = mysql_fetch_assoc($result)) {
			$allBuildings[] = $row;
		}
		
		if(! $allBuildings) {
			echo "You do not have any buildings.";
			return;
		}
		// Make the form
		echo "<form method=\"post\" action=\"buildingStats.php\">";
		
		// Choose a building to change the name of
		echo '<b>Select the building you would like to change the name of</b><br>
			<select name="buildingId">';
		foreach ($allBuildings as $building) {
			if(ucfirst(strtolower($building['username'])) != $user) {
				continue;
			}
			$buildingId = $building['buildingId'];
			$buildingName = $building['buildingName'];
			$structureType = $building['structureType'];
			$district = $building['district'];
			if(! $buildingName) {
				echo "<option value=\"$buildingId\">$structureType $buildingId in the $district</option>";
				continue;
			}
			echo "<option value=\"$buildingId\">$buildingName</option>";
		}
		echo "</select><br>";
		
		// Enter new building name
		echo '<b>New Name*:</b><br><input type="text" name="nickName" size=40 maxlength=40 value="Enter New Name"><br>';
		echo '*Make sure to use no special characters and to keep the length within 40 characters.<br>';
		// Add submit button to change the building name
		echo '<br><input type="submit" name="submit" value="Change Building Name">';

		// End the form
		echo "</form>";

	}
	function setBuildingName($user, $LinkID){
		
		$buildingId = $_POST['buildingId'];
		//$buildingName = $_POST['buildingName'];
		$nickName = $_POST['nickName'];
		$nickName = strip_tags($nickName);
		#Parse the data and replace any mistakes, or dangerous words
		$verifiedNickName = preg_replace("/(\")|(\')|(<)|(>)|(;)/i", "", $nickName);
		//echo $verifiedNickName;
		$setBuildingName = "update buildings set buildingName = '$verifiedNickName' where username = '$user' and buildingId = $buildingId";
		$newBuildingName = mysql_query($setBuildingName, $LinkID);
		echo mysql_error($LinkID);
		
		echo '<form method="post" action="buildingStats.php"><b>Your building\'s new name is ' . $verifiedNickName . '</b><br><input type="submit" value="ok"></form>';
		//echo $setBuildingName;
	}
?>