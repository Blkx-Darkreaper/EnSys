<?php
session_start();

include '../../includes/include.php';

mysql_select_db($database, $LinkID);		
$result = mysql_query("Update users set loggedOut=NOW() where upper(username) = '".$_SESSION['user']."'", $LinkID);	//remove the users logged status from the DB

$_SESSION['user'] = null;												//remove the session data for the user

header('Location: http://deepblue.cs.camosun.bc.ca/~comp19908/');		//if they do not match, reload the main page with an error message
exit();
?>