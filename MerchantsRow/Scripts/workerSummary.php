<?php
$workerId = $_GET["worker"];

include '../../includes/include.php';

mysql_select_db($database, $LinkID);	

$workerQuery = "Select workerName, wage, morale from workers where workerId = $workerId";
$workerResult = mysql_query($workerQuery);

$skillsQuery = "Select skillName, skillLevel, experience, nextLevel from workerSkills where workerId = $workerId and skillName <> 'No action'";
$skillsResult = mysql_query($skillsQuery);

echo mysql_error($LinkID);
?>

<html>
<head>
<title>Worker Summary</title>
	<link href="../css/styles.css" rel="stylesheet" type="text/css" />
	<link rel="shortcut icon" href="../images/titleIcon.ico" type="image/png">
</head>
<style>
div.window{  background-image:url('../images/background.jpg');
			 background-color:#cccccc;
			 height:600px;
			 width:420px;
}
</style>
<body>
	<div class="window">
		<table>
			<tr><td width=120><b>Worker Name</b></td><td width=50><b>Wage</b></td><td width=50><b>Morale</b></td></tr>
			
			<?php
			$row = mysql_fetch_row($workerResult);
			$morale = (float) $row[2];
			if ($morale <= 0.5) {
				$morale = "Terrible";
			}
			elseif (($morale > 0.5) and ($morale <= 0.7)) {
				$morale = "Bad";
			}
			elseif (($morale > 0.7) and ($morale <= 0.9)) {
				$morale = "Mediocre";
			}
			elseif (($morale > 0.9) and ($morale <= 1.1)) {
				$morale = "Good";
			}
			elseif (($morale > 1.1) and ($morale <= 1.3)) {
				$morale = "Great";
			}
			else {
				$morale = "Excellent";
			}
			?>
			
			<tr><td><?php echo $row[0]; ?></td><td><?php echo $row[1]; ?></td><td><?php echo $morale; ?></td></tr>
		</table>
		<br>
		<b>Skills<b>
		<table>
			<tr><td width=150>Skill</td><td width=60>Skill Level</td><td width=60>Experience</td><td width=60>Next Level</td></tr>
		</table>
		<?php
		while($row = mysql_fetch_row($skillsResult)) {
			echo "<table border=3><tr><td width=150>$row[0]</td><td width=60>$row[1]</td><td width=60>$row[2]</td><td width=60>$row[3]</td></tr></table>";
		}
		?>
	</div>
</body>
</html>
