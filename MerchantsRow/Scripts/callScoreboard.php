<?php
$scores = mysql_query("select username, money from users order by money desc Limit 10");

echo "<h2>Player Rankings - Top 10</h2>";
echo "<table width=400>";
echo "<tr><td width=300><b>Player Name</b></td><td><b>Gold</b></td></tr>";
echo "<table border=2 width=400>";

while ($playerScore = mysql_fetch_row($scores)) {
	echo "<tr><td width=300>$playerScore[0]</td><td>$playerScore[1]</td></tr>";
}

echo "</table>";
?>