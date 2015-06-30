<?php
//include '../../includes/include.php';

function callWorkers ($user, $LinkID) {
	/*
	Select w.workerId, w.workerName, bw.currentTask, s.skillName, w.wage, w.morale, bw.workHours
	From workers w left join buildingWorkers bw using (workerId) left join buildings b using(buildingId), skills s
	Where b.username = 'Darkreaper'
	And s.activeSkill = bw.currentTask
	Order by w.workerName, w.workerId;
	*/
	$query = "Select w.workerId, w.workerName as 'Name', bw.currentTask as 'Current Task', s.skillName as 'Skill', w.wage as 'Wage', w.morale as 'Morale', bw.workHours as 'Work Hours' From workers w left join buildingWorkers bw using (workerId) left join buildings b using(buildingId), skills s Where b.username = '$user' And s.activeSkill = bw.currentTask Order by w.workerName, w.workerId";
	//print "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);
	echo mysql_error($LinkID);

	$allWorkerIds = "";
	$row = mysql_fetch_assoc($result);
	//print "Row:<br>";	//debug
	//print_r($row);	//debug
	//print "<br><br>";	//debug
	$allWorkers[] = $row;
	$allWorkerIds = $allWorkerIds . $row['workerId'];

	//Get all the workers' stats and add them to an array and make a list of all the workerIds
	while ($row = mysql_fetch_assoc($result)) {
		//print "Row:<br>";	//debug
		//print_r($row);	//debug
		//print "<br><br>";	//debug
		$allWorkers[] = $row;
		$allWorkerIds = $allWorkerIds . ", " . $row['workerId'];
	}
	
	if(! $allWorkers) {
		print "<p>You currently have no workers.</p>";
		return;
	}

	//print "allWorkers:<br>";	//debug
	//print_r($allWorkers);	//debug
	//print "<br><br>";	//debug

	//print "allWorkerIds: $allWorkerIds<br><br>"; //debug

	/*
	Select workerId, skillName, skillLevel
	From workerSkills
	Where workerId in (6, 7, 8)
	Order by workerId, skillLevel desc, skillName;
	*/
	$query = "Select workerId, skillName, skillLevel From workerSkills Where workerId in ($allWorkerIds) Order by skillLevel desc, skillName";
	//print "$query<br>";	//debug
	$result = mysql_query($query ,$LinkID);

	// Get each worker's skills and add them to an array with the workerId as the key
	$allWorkerTasks = array();
	
	while ($row = mysql_fetch_assoc($result)) {
		$allWorkerTasks[$row['workerId']][$row['skillName']] = array('task' => $row['skillName'], 'skillLevel' => $row['skillLevel']);
	}

	// Build and populate the table of all workers with their stats
	// Print the column labels
	print "<p><form method=\"post\" action=\"workerStats.php\"><table><tr>";
	print "<td><b>Name</b></td>";
	print "<td><b>Currently</b></td>";
	print "<td><b>Skill level</b></td>";
	print "<td><b>Rate</b></td>";
	print "<td><b>Wage</b></td>";
	print "<td><b>Morale</b></td>";
	print "<td><b>Assign Task</b></td>";
	print "</tr>";

	// Print the rows with values
	foreach ($allWorkers as $row) {
		$worker = $row['workerId'];
		$name = $row['Name'];
		$currentTask = $row['Current Task'];
		$skill = $row['Skill'];
		$wage = $row['Wage'];
		$morale = $row['Morale'];
		$workHours = $row['Work Hours'];
		$skillLevel = $allWorkerTasks[$worker][$skill]['skillLevel'];
		$productionRate = 0;
		
		// Calculate the Production rate for the current task
		if($skill != 'No action') {
			//print "Skill level: $skillLevel<br>"; //debug
			$productionRate = round(6/UPDATE_INTERVAL * BASE_PRODUCTION * (.9 + .1*($skillLevel + $morale)), 1);
			//print "Production: $productionRate<br>"; //debug
		}

		print "<tr>";
		// Worker Name
		print "<td><a href=\"../scripts/workerSummary.php?worker=$worker\" target=\"popup\" onclick=\"window.open('../scripts/workerSummary.php?worker=$worker', 'name','width=420,height=600')\">$name </a></td>";
		// Current Task
		print "<td>$currentTask</td>";
		// Skill level
		print "<td>$skillLevel</td>";
		// Production rate
		print "<td>$productionRate/6hrs</td>";
		// Wage
		print "<td>$wage</td>";
		// Morale
		$morale = (float) $morale;
		if ($morale <= 0.5) {
			print "<td>Terrible</td>";
		}
		elseif (($morale > 0.5) and ($morale <= 0.7)) {
			print "<td>Bad</td>";
		}
		elseif (($morale > 0.7) and ($morale <= 0.9)) {
			print "<td>Mediocre</td>";
		}
		elseif (($morale > 0.9) and ($morale <= 1.1)) {
			print "<td>Good</td>";
		}
		elseif (($morale > 1.1) and ($morale <= 1.3)) {
			print "<td>Great</td>";
		}
		else {
			print "<td>Excellent</td>";
		}
		// Assignments
		print "<td><select name = \"assignments[]\">";
		foreach (array_keys($allWorkerTasks[$worker]) as $task) {
			$output = "$worker,$skill,$task";
			if($task != $skill) {
				print "<option value=\"$output\">$task</option>";
			} else {
				print "<option value=\"$output\" selected>$task</option>";
			}
		}
		print "</select></td>";
		print "</tr>";
	}
	// End table
	print "</table><br>";

	// Add submit button to change tasks
	print "<input type=\"submit\" value=\"Change Tasks\">";

	// End the form
	print "</form></p>";
}

function changeTasks ($user, $LinkID) {
	// Get all submitted assignments
	$assignments = $_POST["assignments"];
	
	if(! $assignments) {
		callWorkers($user, $LinkID);
	}
	
	$newAssignments = array();
	foreach($assignments as $entry) {
		//print "$entry<br>";	//debug
		$workerTasks = explode(",", $entry);
		$workerId = $workerTasks[0];
		$currentTask = $workerTasks[1];
		$newTask = $workerTasks[2];
		//print "Worker ID: $workerId<br>"; //debug
		//print "Current task: $currentTask<br>"; //debug
		//print "New task: $newTask<br><br>"; //debug
		
		// Only add those assignments that have changed from before
		if($newTask != $currentTask) {
			$newAssignments[$workerId] = array('workerId' => $workerId, 'New task' => $newTask);
		}
	}
	//print "New Assignments:<br>";	//debug
	//print_r($newAssignments);	//debug
	//print "<br><br>";	//debug
	
	// Update the current task of those workers with new assignments
	foreach($newAssignments as $entry) {
		$workerId = $entry['workerId'];
		$newTask = $entry['New task'];
		//print "Worker ID: $workerId<br>"; //debug
		//print "New task: $newTask<br><br>"; //debug
		/*
		Update buildingWorkers
		Set currentTask = (Select activeSkill From skills Where skillName = '$newTask')
		Where workerId = $workerId;
		*/
		$query = "Update buildingWorkers Set currentTask = (Select activeSkill from skills Where skillName = '$newTask') Where workerId = $workerId";
		//print "$query<br>";	//debug
		$result = mysql_query($query ,$LinkID);
		echo mysql_error($LinkID);
	}
	callWorkers($user, $LinkID);
}

$user = $_SESSION['user'];

// Choose the DB and run a query.
mysql_select_db($database, $LinkID);

if(isset($_POST['assignments'])) {
    changeTasks($user, $LinkID);
}else {
	callWorkers($user, $LinkID);
}
?>