<?php
//include '../../includes/include.php';

function getCurrentInventory($LinkID, $buildingId, $allGoodsToCheck) {
	$query = "Select goodName, goodQuality, goodQuantity from buildingGoods Where buildingId = $buildingId";
	if($allGoodsToCheck != '') {
		$query = $query . " And goodName in($allGoodsToCheck)";
	}
	//echo "$query<br>";	//debug 
	$result = mysql_query($query, $LinkID);
	
	$totalGoods = array();
	while ($row = mysql_fetch_assoc($result)) {
		$totalGoods[$row['goodName']][$row['goodQuality']] = $row['goodQuantity'];
	}
	//echo "Total goods:<br>";	//debug
	//print_r($totalGoods);	//debug
	//echo "<br><br>";	//debug
	
	return $totalGoods;
}

function getBuildingStorageSpace($LinkID, $buildingId) {
	/*
	Select storageSpace
	From structures left join buildings using(structureType)
	Where buildingId=$buildingId;
	*/
	$query = "Select storageSpace From structures left join buildings using(structureType) Where buildingId=$buildingId";
	$result = mysql_query($query, $LinkID);
	while($row = mysql_fetch_assoc($result)) {
		$storageSpace = $row['storageSpace'];
	}
	//echo "Storage space: $storageSpace<br>"; //debug
}

function buildingAvailableSpace($LinkID, $buildingId) {
	/*
	Select (storageSpace - sum(goodQuantity)) as 'available storage'
	From structures left join buildings using(structureType) left join buildingGoods using(buildingId)
	Where buildingId=$buildingId;
	*/
	$query = "Select (storageSpace - sum(goodQuantity)) as 'available storage' From structures left join buildings using(structureType) left join buildingGoods using(buildingId) Where buildingId=$buildingId";
	$result = mysql_query($query, $LinkID);
	
	while($row = mysql_fetch_assoc($result)) {
		$availableStorageSpace = $row['available storage'];
	}
	//echo "Available storage space: $availableStorageSpace<br>"; //debug
	return $availableStorageSpace;
}

function getCraftingRecipes($LinkID, $goodsToCheck) {
	$allGoodNames = '';
	foreach($goodsToCheck as $goodName => $good) {
		if($allGoodNames != '') {
			$allGoodNames = $allGoodNames . ", '$goodName'";
		} else {
			$allGoodNames = $allGoodNames . "'$goodName'";
		}
	}
	
	/*
	Select product, outputQuantity, ingredient, inputQuantity, (outputQuantity - inputQuantity) as difference
	From methodProducts left join craftingRecipes using(method)
	Where product in ($allGoodNames);
	*/
	$query = "Select product, outputQuantity, ingredient, inputQuantity, (outputQuantity - inputQuantity) as difference From methodProducts left join craftingRecipes using(method) Where product in ($allGoodNames)";
	//echo "$query<br>";	//debug
	$result = mysql_query($query, $LinkID);
	
	$craftingRecipes = null;
	
	while($row = mysql_fetch_assoc($result)) {
		$craftingRecipes[$row['product']]['output'] = $row['outputQuantity'];
		$craftingRecipes[$row['product']]['ingredients'][$row['ingredient']] = $row['inputQuantity'];
		$craftingRecipes[$row['product']]['net'] = $row['difference'];
	}
	
	//echo "Crafted Goods:<br>"; //debug
	//print_r($craftingRecipes); //debug
	//echo "<br><br>"; //debug
	
	return $craftingRecipes;
}

function updateInventory($LinkID, $buildingId, $currentGoods, $goodsToChange, $craftingGoods) {
	//echo "Flag: $craftingGoods<br>"; //debug

	//echo "Current goods:<br>";	//debug
	//print_r($currentGoods);	//debug
	//echo "<br><br>";	//debug
	
	//echo "Goods to Change:<br>";	//debug
	//print_r($goodsToChange);	//debug
	//echo "<br><br>";	//debug
	
	// Get the building's available storage capacity
	$availableStorageSpace = buildingAvailableSpace ($LinkID, $buildingId);
	//$availableStorageSpace = 0; //debug
	//echo "Current space available: $availableStorageSpace<br><br>"; //debug
	
	// If crafting get all the crafting recipes
	if($craftingGoods == true) {
		//echo "Crafting<br>"; //debug
		$craftingRecipes = getCraftingRecipes($LinkID, $goodsToChange);
	}
	
	$actualGoodsToChange = null;
	foreach($goodsToChange as $goodName => $good) {
		foreach($good as $goodQuality => $goodQuantity) {
			// Check if there is enough storage available
			if($availableStorageSpace > $goodQuantity) {
				$actualGoodQuantity = $goodQuantity;
			} else {
				$actualGoodQuantity = $availableStorageSpace;
			}
			
			//echo "Actual Quantity: $actualGoodQuantity<br>"; //debug
		
			// Crafting
			if($craftingGoods == true) {
				// Determine if good is a crafted good
				if(isset($craftingRecipes[$goodName])) {
					
					// Check there are sufficient ingredients
					$enoughIngredientsForProduct = $actualGoodQuantity;
					foreach($craftingRecipes[$goodName]['ingredients'] as $ingredientName => $ingredientCost) {
						// Check inventory for ingredient
						$ingredientQuantity = $currentGoods[$ingredientName][$goodQuality];
						$ingredientNeeded = floor($ingredientQuantity/$ingredientCost);
						
						if($ingredientNeeded < $enoughIngredientsForProduct) {
							$enoughIngredientsForProduct = $ingredientNeeded;
						}
					}
					
					// Adjust ingredient quantities used
					foreach(array_keys($craftingRecipes[$goodName]['ingredients']) as $ingredientName) {
						if(! isset($actualGoodsToChange[$ingredientName][$goodQuality])) {
							$actualGoodsToChange[$ingredientName][$goodQuality] = 0;
						}
					
						$actualGoodsToChange[$ingredientName][$goodQuality] -= $enoughIngredientsForProduct;
					}
					
					// Set actual product output based on ingredients available
					$actualGoodQuantity = $enoughIngredientsForProduct;
				}					
			}

			$actualGoodsToChange[$goodName][$goodQuality] = $actualGoodQuantity;
			
			if($availableStorageSpace == 0) {
				break 2;
			}
		}
	}
	
	//echo "Actual goods to change:<br>"; //debug
	//print_r($actualGoodsToChange); //debug
	//echo "<br><br>"; //debug
	
	$totalGoodsProduced = 0;
	foreach($actualGoodsToChange as $goodName => $good) {
		foreach($good as $goodQuality => $goodQuantity) {
			$totalGoodsProduced += $goodQuantity;
		}
	}
	
	if($totalGoodsProduced == 0) {
		//echo "No goods to change<br>"; //debug
		return null;
	}
	
	// Change the building's inventory quantities
	foreach($actualGoodsToChange As $itemName => $item) {
		foreach($item As $itemQuality => $itemQuantity) {
			$query = "Update buildingGoods Set goodQuantity = goodQuantity + $itemQuantity Where goodName = '$itemName' And goodQuality = '$itemQuality' And buildingId = $buildingId";
			//echo "$query<br>";	//debug
			$result = mysql_query($query, $LinkID);
		}
	}
	
	return $actualGoodsToChange;
}

function workerVacation ($LinkID, $workerId, $hoursElapsed) {
	// Get worker's morale
	/*
	Select morale
	From workers
	Where workerId=$workerId;
	*/
	$query = "Select morale From workers Where workerId=$workerId";
	$result = mysql_query($query, $LinkID);
	
	while($row = mysql_fetch_assoc($result)) {
		$currentMorale = $row['morale'];
	}
	
	if($currentMorale == 2.0) {
		return;
	}

	$daysElapsed = $hoursElapsed/24;

	$moraleBoost = round($daysElapsed * 0.1, 1);
	
	$newMorale = $currentMorale + $moraleBoost;
	
	if($newMorale > 2.0) {
		$newMorale = 2.0;
	}
	
	// Set the worker's morale to the new value
	/*
	Update workers Set morale = morale + $moraleBoost
	Where workerId = $workerId;
	*/
	$query = "Update workers Set morale = $newMorale Where workerId = $workerId";
	//echo "$query<br>";	//debug
	$result = mysql_query($query, $LinkID);
}

function produceGoods ($user, $LinkID, $hoursElapsed) {
	$report = "Since your last visit";
	if($hoursElapsed < UPDATE_INTERVAL) {
		$report = $report . " your workers have not produced any goods.";
		return $report;
	}
	$_SESSION['lastUpdated'] = time();
	// Get all the user's worker's activity
	/*
	Select b.buildingId, s.product, ws.skillLevel, bw.workerId, w.morale
	From buildingWorkers bw left join buildings b using(buildingId) left join skills s on bw.currentTask = s.activeSkill left join workerSkills ws using(workerId, skillName) left join workers w using(workerId)
	Where b.username = 'Darkreaper'
	And bw.currentTask not in('');
	*/

	$query = "Select b.buildingId, b.buildingName, s.product, ws.skillLevel, bw.workerId, w.morale From buildingWorkers bw left join buildings b using(buildingId) left join skills s on bw.currentTask = s.activeSkill left join workerSkills ws using(workerId, skillName) left join workers w using(workerId) Where bw.currentTask not in('') And b.username = '$user'";
	//echo "$query<br>";	//debug
	$result = mysql_query($query, $LinkID);
	$workDone = array();
	
	if (! $result) {
		$report = $report . " your workers have not produced any goods.";
		return $report;
	}
	
	// Sort all the produced goods by building and type
	while ($row = mysql_fetch_assoc($result)) {
		//echo "Row:<br>";	//debug
		//print_r($row);	//debug
		//echo "<br><br>";	//debug
		if (! array_key_exists($row['buildingId'], $workDone)) {
			$workDone[$row['buildingId']] = array();
			$i = 0;
		}
		$workDone[$row['buildingId']][$i] = array('buildingName' => $row['buildingName'], 'product' => $row['product'], 'skillLevel' => $row['skillLevel'], 'workerId' => $row['workerId'], 'morale' => $row['morale']);
		$i++;
	}
	//echo "Work done:<br>";	//debug
	//print_r ($workDone);	//debug
	//echo "<br><br>";	//debug
	
	// Determine how much of each good has been produced in the elapsed time
	$report = $report . "...<br>";
	foreach($workDone as $building => $inventory) {
		$goodsProduced = array();
		$allProducedGoodsNames = "";
		$buildingId = $building;
		$totalProduced = 0;
		$buildingName = $inventory['buildingName'];
		
		echo "Work done:<br>"; //debug
		print_r($inventory);	//debug
		echo "<br><br>";	//debug
		
		if($buildingName == '') {
			$report = $report . "Building $buildingId produced:<br>";
		} else {
			$report = $report . "$buildingName produced:<br>";
		}
		//echo $report; //debug
		
		// Total up all the goods of each type produced in this building
		foreach($inventory as $entry) {
			$product = $entry['product'];
			echo "Product: $product<br>";	//debug
			
			if($product == 'Nothing') {
				$workerId = $entry['workerId'];
				workerVacation($LinkID, $workerId, $hoursElapsed);
				continue;
			}
			
			$skillLevel = $entry['skillLevel'];
			$morale = $entry['morale'];
			$productQuality = 'good';
			
			$produced = intval($hoursElapsed/UPDATE_INTERVAL * BASE_PRODUCTION * (.9 + .1*($skillLevel + $morale)));
			echo "Production: $produced<br>"; //debug
			
			/*
			if($allProducedGoodsNames != '') {
				$allProducedGoodsNames = $allProducedGoodsNames . ", '$product'";
			} else {
				$allProducedGoodsNames = $allProducedGoodsNames . "'$product'";
			}
			//echo "All goods' names: $allProducedGoodsNames<br>";	//debug
			*/
			
			if(! isset($goodsProduced[$product][$productQuality])) {
				$goodsProduced[$product][$productQuality] = 0;
			}
			$goodsProduced[$product][$productQuality] += $produced;
			$totalProduced += $produced;
		}
		
		if ($totalProduced == 0) {
			$report = $report . " your workers have not produced any goods.";
			continue;
		}
		
		echo "Goods Produced:<br>";	//debug
		print_r($goodsProduced);	//debug
		echo "<br><br>";	//debug
		
		//echo "All goods: $allProducedGoodsNames<br><br>";	//debug
		/*
		Select bg.goodName
		From buildingGoods bg left join buildings b using(buildingId)
		Where bg.goodQuantity > 0
		And username = '$user';
		*/
		/*
		$query = "Select goodName From buildingGoods left join buildings using(buildingId) Where goodQuantity > 0 And username = '$user'";
		$result = mysql_query($query, $LinkID);
		
		$allGoods = '';
		while($row = mysql_fetch_assoc($result)) {
			$goodName = $row['goodName'];
			if($allGoods != '') {
				$allGoods = $allGoods . ", '$goodName'";
			} else {
				$allGoods = $allGoods . "'$goodName'";
			}
		}
		
		// Add List of good names in inventory to those produced
		$allGoodsToCheck = $allGoods . ", " . $allProducedGoodsNames;
		*/
		
		// Determine quantities already existing in building's stores
		$totalGoods = getCurrentInventory($LinkID, $buildingId, '');
		
		// Add newly produced goods to those already in stores if space is available
		$goodsActuallyProduced = updateInventory($LinkID, $buildingId, $totalGoods, $goodsProduced, true);
		echo "Goods Actually Produced:<br>";	//debug
		print_r($goodsActuallyProduced);	//debug
		echo "<br><br>";	//debug
		
		// Update worker skill levels and experience
		levelUp($LinkID, $hoursElapsed);
		
		// Update the building report
		if(isset($goodsActuallyProduced)) {
			foreach($goodsActuallyProduced as $productName => $product) {
				foreach($product as $productQuality => $totalProduced) {
					if($totalProduced == 0) {
						continue;
					}
				
					$report = $report . "$totalProduced $productQuality $productName<br>";
				}
			}
		}
		$report = $report . "<br>";
		
		$buildingSpaceAvailable = buildingAvailableSpace($LinkID, $buildingId);
		if($buildingSpaceAvailable <= 0) {
			$report = $report . "There is no more storage space available.<br>";
		}
	}
	
	return $report;
}
?>