<?php

include '../../includes/include.php';

function updateUser ($user) {
	$loggedOut = mysql_query("Select loggedOut From users where upper(username) = \' $user \'", $LinkID);
	$currentTime = mysql_query("SELECT CURTIME()", $LinkID);
	$timeElapsed = $currentTime - $loggedOut;
	print $timeElapsed;
	$goodsProducted = $timeElapsed;
	/*	
	Select b.buildingId, s.product
	From buildingWorkers bw, buildings b, skills s
	Where bw.buildingId = b.buildingId
	And bw.currentTask = s.activeSkill
	And b.username = 'Lady3lle'
	And bw.currentTask not in('Nothing', '');
	*/
	$workDone = mysql_query("Select b.buildingId, s.product From buildingWorkers bw, buildings b, skills s where bw.buildingId = b.buildingId And bw.currentTask = s.activeSkill And bw.currentTask not in('Nothing', '') And b.username = \' $user \'", $LinkID);
	/*
	foreach($workDone as $building => $good) {
		$totalGoods = mysql_query("Select goodName, goodQuantity from buildingGoods Where buildingId = \'$building\'", $LinkID);
		$goodName = $totalGoods[0];
		$goodQuantity = $totalGoods[1];
		if ($goodName == $good) {
			$update = mysql_query("Update buildingGoods Set goodQuantity = ($goodQuantity + $goodsProduced) Where goodName = $goodName");
		}
	}
	*/
}

mysql_select_db($database, $LinkID);
echo mysql_error($LinkID);

updateUser('Mayor');