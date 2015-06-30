<?php
//include '../../includes/include.php';

function levelUp($LinkID, $hoursElapsed) {
	if($hoursElapsed < LEVELUP_INTERVAL) {
		return;
	}
	$_SESSION['lastLevelled'] = time();
	
	// Get all the currently busy workers
	/*
	Select bw.workerId, bw.workHours, ws.skillName, ws.skillLevel, ws.experience, ws.nextLevel
	From buildingWorkers bw left join buildings b using(buildingId) left join skills s on bw.currentTask = s.activeSkill left join workerSkills ws using(workerId, skillName)
	Where bw.currentTask not in('Nothing', '')
	Order by bw.workerId;
	*/
	$query = "Select bw.workerId, bw.workHours, ws.skillName, ws.skillLevel, ws.experience, ws.nextLevel From buildingWorkers bw left join buildings b using(buildingId) left join skills s on bw.currentTask = s.activeSkill left join workerSkills ws using(workerId, skillName) Where bw.currentTask not in('Nothing', '') Order by bw.workerId";
	//print "$query<br>";	//debug
	$result = mysql_query($query, $LinkID);
	$busyWorkers = array();
	
	while ($row = mysql_fetch_assoc($result)) {
		//print "Row:<br>";	//debug
		//print_r($row);	//debug
		//print "<br><br>";	//debug
		$busyWorkers[] = $row;
	}
	//print "Busy Workers:<br>"; //debug
	//print_r($busyWorkers); //debug
	//print "<br><br>";	//debug
	
	foreach($busyWorkers as $worker) {
		$workerId = $worker['workerId'];
		//print "workerId: $workerId<br>"; //debug
		$workHours = $worker['workHours'];
		//print "workHours: $workHours<br>"; //debug
		$skillName = $worker['skillName'];
		//print "skillName: $skillName<br>"; //debug
		$skillLevel = $worker['skillLevel'];
		//print "skillLevel: $skillLevel<br>"; //debug
		$experience = $worker['experience'];
		//print "experience: $experience<br>"; //debug
		$nextLevel = $worker['nextLevel'];
		//print "nextLevel: $nextLevel<br>"; //debug
		
		if($nextLevel == null) {
			continue;
		}
		
		$workHours += $hoursElapsed;
		$experience += $hoursElapsed;
		if(isset($nextLevel)) {
			if(($experience % $nextLevel) > 1) {
				// Level up
				$skillLevel++;
				$experience = ($experience % $nextLevel);
				$nextLevel = $skillLevel * 24;
			}
		}
		// Update all worker stats
		/*
		Update buildingWorkers
		Set workHours = $workHours
		Where workerId = $workerId;
		
		Update workerSkills
		Set skillLevel = $skillLevel, experience = $experience, nextLevel = $nextLevel
		Where workerId = $workerId
		And skillName = '$skillName';
		*/
		$query = "Update buildingWorkers Set workHours = $workHours Where workerId = $workerId";
		//print "$query<br>";	//debug
		$result = mysql_query($query, $LinkID);
		
		if(! isset($skillName)) {
			print "<br>";
			continue;
		}
		
		if($skillLevel == 10) {
			$query = "Update workerSkills Set skillLevel = $skillLevel, experience = 0, nextLevel = null Where workerId = $workerId And skillName = '$skillName'";
			//print "$query<br><br>";	//debug
			$result = mysql_query($query, $LinkID);
			continue;
		}
		
		$query = "Update workerSkills Set skillLevel = $skillLevel, experience = $experience, nextLevel = $nextLevel Where workerId = $workerId And skillName = '$skillName'";
		//print "$query<br><br>";	//debug
		$result = mysql_query($query, $LinkID);
	}
}