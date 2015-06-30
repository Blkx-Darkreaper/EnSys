<?php
function payWorkers($user, $LinkID, $hoursElapsed) {
	if ($hoursElapsed < 24) {
		$report = "No workers needed to be paid.";
		return $report;
		}
	$report = "";
	
	//retrieve all of a users workers
	$workerQuery = "select w.workerId, w.workerName, w.wage, w.morale, bw.currentTask from workers w inner join buildingWorkers bw on w.workerId = bw.workerId inner join buildings b on bw.buildingId = b.buildingId where b.username = '$user'";
	$workerResult = mysql_query($workerQuery);
	
	$moneyQuery = "select money from users where username = '$user'";
	
	$payAmounts = null;
	$workerIDs = null;
	
	$payAmounts = array();
	$workerIDs = array();
	
	//for each worker, calculate how much they should be paid
	while($row = mysql_fetch_assoc($workerResult)) {
		if ($row['currentTask'] == "On vacation") {
			$report = $report."$row[workerName] is on vacation and did not need to be paid.\n";
			continue;
		}
		$worker = $row['workerName'];
		$payRate = $row['wage'];
		$amountToPay = $payRate * ($hoursElapsed / 24);
		$payAmounts[] = $amountToPay;
		$workerIDs[] = $row['workerId'];
		$workerNames[] = $row['workerName'];
		
		print $worker;	//debug
		print $payRate;	//debug
	}
	
	print_r($workerIDs);	//debug
	print_r($payAmounts);	//debug
	
	$i = 0;
	
	foreach($payAmounts as $amount) {
		$moneyResult = mysql_query($moneyQuery);
		$currentMoney = mysql_fetch_row($moneyResult);
		$amount = floor($amount);
		
		if ($currentMoney[0] - $amount < 0) {
			$report = $report."You were unable to pay $workerNames[$i] their wage of $amount. Their morale has dropped.\n";
			reduceMorale($workerIDs[$i], $LinkID);
			$i++;
			continue;
		}
		
		$payQuery = "update users set money = money - $amount where username = '$user'";
		mysql_query($payQuery);
		$report = $report."$workerNames[$i] was paid $amount gold from your coffers.\n<br>";
		$i++;
	}
	
	mysql_query("update users set lastPaid = Now() where username = '$user'",$LinkID);
	$_SESSION['lastPaid'] = time();
	return $report;
}

function reduceMorale($workerId, $LinkID) {
	$moraleQuery = "update workers set morale = morale - 0.1 where workerId = $workerId";
	mysql_query($moraleQuery);
	return;
}
?>