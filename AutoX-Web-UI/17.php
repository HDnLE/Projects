<h4 class="headliner" id="category" data-category='messages'>Meldinger</h4>
<?php
	$page = "inbox.php";
	if (isset($_GET['message']))
	{
		$page = "message.php";
	}
	include $page;
	include "modals.php";
?>