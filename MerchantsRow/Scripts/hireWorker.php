<?php
function hireWorker($LinkID) {
	$workerResult = mysql_query("select * from workerBids", $LinkID);
	while ($worker = mysql_fetch_row($workerResult)) {
		$timeResult = mysql_query("select time_to_sec(TIMEDIFF(now(), (select expirationDate from workerBids where workerId = $worker[0])))", $LinkID);
		$timeLeft = mysql_fetch_row($timeResult);
	
		if ($timeLeft[0] < 0) {
			continue;
		}
		if ($worker[1] == 0) {
			//remove worker
			continue;
		}
		//add the worker to the users building
		mysql_query("Insert Into buildingWorkers Set buildingId = $worker[3], workerId = $worker[0], currentTask = 'On Vacation'",$LinkID);
		//update the workers wage to the correct amount
		mysql_query("update workers set wage = $worker[1] where workerId = $worker[0]", $LinkID);
		//remove the worker from the workerBids table
		mysql_query("delete from workerBids where workerId = $worker[0]", $LinkID);
		echo mysql_error($LinkID);
	}
	return;
}
?>