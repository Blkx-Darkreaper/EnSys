<?php
function addBaseSkills($workerID, $LinkID) {
		//get the list of all the goods
	$skillsResource = mysql_query("Select * from skills",$LinkID);
	while ($skillName = mysql_fetch_row($skillsResource)) {
		if($skillName[0] == 'No action') {
			mysql_query("Insert into workerSkills set workerId = $workerID, skillName = '$skillName[0]', nextLevel=null",$LinkID);
			continue;
		}	
		mysql_query("Insert into workerSkills set workerId = $workerID, skillName = '$skillName[0]'",$LinkID);
	}
}

function addWorker ($LinkID, $forHire) {
		//generate a random name for the new worker
	$firstNameArray = explode("\n", file_get_contents("firstNames.txt"));
	$lastNameArray = explode("\n", file_get_contents("lastNames.txt"));
	
	$firstNamePos = rand(0,199);
	$lastNamePos = rand(0,99);
	
	$firstName = $firstNameArray[$firstNamePos];
	$lastName = $lastNameArray[$lastNamePos];
	
	$name = $firstName." ".$lastName;
	
		//add the worker to the workers table and then find their ID number
	$addWorkerQuery = "Insert Into workers set workerName = '$name', wage = 5, morale = 1.0";
	$findWorkerQuery = "Select * from workers Order By workerId Desc Limit 1";
	mysql_query($addWorkerQuery,$LinkID);
	$workerResult = mysql_query($findWorkerQuery,$LinkID);
	$workerInfo = mysql_fetch_row($workerResult);
	$workerID = $workerInfo[0];
		//add all the skills to the worker
	addBaseSkills($workerID, $LinkID);
	
		//if they are a free agent, add them to the bids table
	if ($forHire == true) {
		$bidQuery = "Insert into workerBids set workerId = $workerID, expirationDate = DATE_ADD(now(), INTERVAL 2 DAY)";
		mysql_query($bidQuery, $LinkID);
	}
	return $workerID;
}

function addWorkerToBuilding ($buildingID, $workerID, $LinkID) {
		//assign the new worker to the players building
	mysql_query("Insert Into buildingWorkers Set buildingId = $buildingID, workerId = $workerID, currentTask = 'On vacation'",$LinkID);
}
?>