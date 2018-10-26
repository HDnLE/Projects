<?php 
	if (!is_admin($_SESSION["username"]))
	{
		echo "<div class='text-error'><img src='images/error.png' style='float:left'><div style='display:inline;padding-left:10px;float:left'><h2>Beklager! Denne siden er kun for administrator.</h2></div></div>"; 
	}
	else
	{
		include "activity-logs.php";
	}
?>