<?php
include '../../includes/include.php';
require 'produceGoods.php';
require 'levelUp.php';
require 'payWorkers.php';

$lifetime = LOGIN_TIMEOUT;
session_start();
setcookie(session_name(),session_id(),time()+$lifetime);

function updateUser ($user, $LinkID) {	
	// Get the time in seconds rounded down since the user last logged out
	$productionQuery = "SELECT time_to_sec(TIMEDIFF(now(), (select loggedOut from users Where username = '$user')))";
	//print "$productionQuery<br>";	//debug
	$result = mysql_query($productionQuery, $LinkID);
	if (! $result) {
		return;
	}
	// Convert to minutes
	$minsElapsed = mysql_fetch_row($result);
	$minsElapsed = floor($minsElapsed[0]/60); //minutes
	$hoursElapsedSinceProduceGoods = $minsElapsed/60;
	//echo "Time elapsed(min): $minsElapsed<br>";	//debug
	
	$hoursElapsedSinceLevelUp = $minsElapsed/60;
	
	// Get the time in seconds rounded down since the user last paid their workers
	$payQuery = "SELECT time_to_sec(TIMEDIFF(now(), (select lastPaid from users Where username = '$user')))";
	//print "$productionQuery<br>";	//debug
	$result = mysql_query($payQuery, $LinkID);
	// Convert to hours
	$hoursElapsed = mysql_fetch_row($result);
	$hoursElapsed = floor($hoursElapsed[0]/3600);
	//echo "Time elapsed(hour): $hoursElapsed<br>";	//debug
	
	$report = produceGoods($user, $LinkID, $hoursElapsedSinceProduceGoods);
	$report = $report."<br>";
	$report = $report.payWorkers($user, $LinkID, $hoursElapsed);
	
	return $report;
}

$user = $_POST['username'];
$user = strtoupper($user);
$password = $_POST['password'];

mysql_select_db($database, $LinkID);																//select the DB
$result = mysql_query("Select * From users where upper(username) = '".$user."'", $LinkID);			//query the DB to retrieve the users info
echo mysql_error($LinkID);

if ($result == false) {
	header('Location: http://deepblue.cs.camosun.bc.ca/~comp19908/?incorrectLogin=true');		//if they do not match, reload the main page with an error message
	exit();
}
$row = mysql_fetch_row($result);

$timeIn = strtotime($row[5]);
$timeOut = strtotime($row[6]);

if (($password != $row[1])) {																	//compare the registered username and password to those entered
	header('Location: http://deepblue.cs.camosun.bc.ca/~comp19908/?incorrectLogin=true');		//if they do not match, reload the main page with an error message
	exit();
}
elseif (($timeIn > $timeOut) and ((time() - $timeIn) < LOGIN_TIMEOUT)) {									//check if the user is already logged in.
	header('Location: http://deepblue.cs.camosun.bc.ca/~comp19908/?alreadyLogged=true');		//if they are, reload the main page with an error message.
	exit();
}
else {																							
	$_SESSION['user'] = $user;																	//track the current user
	$_SESSION['lastUpdated'] = time();
	$_SESSION['lastLevelled'] = time();
	$_SESSION['lastPaid'] = time();
	
	$report = updateUser($user, $LinkID);
	$_SESSION['report'] = $report;

	$result = mysql_query("Update users set loggedIn=NOW() where upper(username) = '".$user."'", $LinkID);
	header('Location: http://deepblue.cs.camosun.bc.ca/~comp19908/MerchantsRow');	//Log the user in and send them to the main game page
	exit();
}
?>