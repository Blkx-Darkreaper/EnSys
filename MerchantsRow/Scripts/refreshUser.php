<?php
//include '../../includes/include.php';
require 'produceGoods.php';
require 'levelUp.php';
require 'payWorkers.php';
require 'callOrderSummary.php';
require 'callAllOrdersInfo.php';
require 'createDemand.php';

function checkOrderExpiry ($LinkID, $currentTime) {
	$allOrders = callAllOrdersInfo ($LinkID);
	//echo "All Orders:<br>";	//debug
	//print_r($allOrders);	//debug
	//echo "<br><br>";	//debug
	
	if(! isset($allOrders)) {
		return;
	}
	
	foreach($allOrders As $order) {
		$expires = strtotime($order['Expires']);
		$type = $order['Type'];
		//echo "Expires: $expires<br>"; //debug
		//echo "Current Time: $currentTime<br>"; //debug
		
		if($expires <= $currentTime) {
			$orderId = $order['Order No.'];
			cancelOrder($LinkID, $orderId);
		}
	}
}

function refreshUser ($user, $LinkID) {
	// Create random demands
	createDemand($LinkID);

	$currentTime = time();
	checkOrderExpiry ($LinkID, $currentTime);
	
	// Hours between updates
	$hoursElapsedSinceLastUpdated = floor(($currentTime - $_SESSION['lastUpdated'])/3600);
	//echo "Hours elapsed: $hoursElapsedSinceLastUpdated<br>";	//debug
	
	//Get hours since last pay day
	$hoursElapsed = floor(($currentTime - $_SESSION['lastPaid']) / 3600);
	//echo "hours elapsed: $hoursElapsed<br>"; //debug
	
	//echo "Time elapsed(hour): $hoursElapsed<br>";	//debug
	//echo "Hours elapsed: $hoursElapsed<br>";	//debug
	//echo mysql_error($LinkID);
	
	$report = produceGoods($user, $LinkID, $hoursElapsedSinceLastUpdated);
	$report = $report."<br>";
	$report = $report.payWorkers($user, $LinkID, $hoursElapsed);
	
	return $report;
}
?>