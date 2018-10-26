<?php 
	$role = get_user_info($_SESSION["username"], 7);
	if (!get_role_info($role, 11))
	{
		echo "<div class='text-error'><img src='images/error.png' style='float:left'><div style='display:inline;padding-left:10px;float:left'><h2>Beklager! Du har ikke tilgang til denne siden...</h2>Ta kontakt med administrator</div></div>"; 
	}
	else
	{
		include "3-a.php";
	}
?>