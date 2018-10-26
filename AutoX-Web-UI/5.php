<?php 
	if (!is_admin($_SESSION["username"]))
	{
		echo "<div class='text-error'><img src='images/error.png' style='float:left'><div style='display:inline;padding-left:10px;float:left'><h2>Beklager! Du har ikke tilgang til denne siden...</h2>Ta kontakt med administrator</div></div>"; 
	}
	else
	{
		include "5-a.php";
	}
?>