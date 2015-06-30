<?php
include 'addWorker.php';
include '../../includes/include.php';

$lifetime = LOGIN_TIMEOUT;
session_start();
setcookie(session_name(),session_id(),time()+$lifetime);

mysql_select_db($database, $LinkID);

$worker = addWorker($LinkID, true);

print $worker;
?>