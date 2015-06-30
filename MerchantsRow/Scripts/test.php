<?php
include '../../includes/include.php';
require 'produceGoods.php';
//require 'refreshUser.php';
//require 'callOrderSummary.php';

session_start();
$_SESSION['lastUpdated'] = time()-60;

// Choose the DB and run a query.
mysql_select_db($database, $LinkID);

$user = 'Darkreaper';
$buildingId = 1;
$hoursElapsed = 9.5;

$report = produceGoods ($user, $LinkID, $hoursElapsed);
echo $report;

//refreshUser($user, $LinkID);

// $inventory = getCurrentInventory($LinkID, $buildingId, '');
// $inventory['wood']['good'] = 10;
// $inventory['venison']['good'] = 10;
// $inventory['grapes']['good'] = 10;
// $inventory['stone']['good'] = 10;
// $inventory['iron ore']['good'] = 10;
// $inventory['gold ore']['good'] = 0;

//$goodsToChange = array('stone' => array('good' => 4), 'lumber' => array('good' => 4), 'sausage' => array('good' => 4), 'wine' => array('good' => 4));
//$goodsToChange = array('wood' => array('good' => 0), 'venison' => array('good' => 0), 'grapes' => array('good' => 0),'stone' => array('good' => 4), 'lumber' => array('good' => 4), 'sausage' => array('good' => 4), 'wine' => array('good' => 4));
//$goodsToChange = array('stone' => array('good' => 4), 'lumber' => array('good' => 4));

// $goodsThatChanged = updateInventory($LinkID, $buildingId, $inventory, $goodsToChange, true);
// echo "Goods that changed:<br>"; //debug
// print_r($goodsThatChanged); //debug
// echo "<br><br>"; //debug

//print "$report<br>";
//cancelOrder($LinkID, $orderId);
//acceptBid($user, $LinkID, $orderId);
//makeBid($user, $LinkID, $orderId);
//finalizeBid($user, $LinkID, $orderId);
?>