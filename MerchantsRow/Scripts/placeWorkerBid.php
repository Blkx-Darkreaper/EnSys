<?php
include '../../includes/include.php';
mysql_select_db($database, $LinkID);

$lifetime = LOGIN_TIMEOUT;
session_start();
setcookie(session_name(),session_id(),time()+$lifetime);

//get the POST data from the worker form
$userBid = $_POST['bid'];
$workerID = $_POST['workerID'];
$user = $_SESSION['user'];
$hiree = $_POST['hiree'];

//Get the workers info
$bidInfo = mysql_query("select * from workerBids where workerId = $workerID", $LinkID);
$bidInfo = mysql_fetch_row($bidInfo);

//check if the user has room in their buildings. If not, were done
$capacity = mysql_query("select st.maxWorkers from buildings b inner join structures st using (structureType) where b.buildingId = $hiree", $LinkID);
$capacity = mysql_fetch_row($capacity);

$currentStaff = mysql_query("select count(workerId) from buildingWorkers where buildingId = $hiree", $LinkID);
$currentStaff = mysql_fetch_row($currentStaff);

if ($currentStaff[0] >= $capacity[0]) {
	$_SESSION['bidPlaced'] = "You do not have enough room in your building.";
	header('Location: http://deepblue.cs.camosun.bc.ca/~comp19908/MerchantsRow/workerStats.php');
	exit();
}

//check if the users bid is higher than the current bid. If not, were done
if ($bidInfo[1] > $userBid) {
	$_SESSION['bidPlaced'] = "You have placed a bid of $userBid gold on a new worker.";
	header('Location: http://deepblue.cs.camosun.bc.ca/~comp19908/MerchantsRow/workerStats.php');
	exit();
}


//set the new attributes for the worker
mysql_query("update workerBids set bidValue = $userBid, bidder = '$user', hiree = $hiree where workerId = $workerID",$LinkID);

$_SESSION['bidPlaced'] = "You have placed a bid of $userBid gold on a new worker.";
header('Location: http://deepblue.cs.camosun.bc.ca/~comp19908/MerchantsRow/workerStats.php');
exit();
?>
