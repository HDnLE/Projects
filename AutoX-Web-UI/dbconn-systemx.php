<?php
	if (session_status() == PHP_SESSION_NONE) 
	{
		session_start();
	}
	$database = (isset($_SESSION["database"]) ? $_SESSION["database"] : null);
	/*$database = "";
	switch ($version)
	{
		case "41":
			$database = "sxtest_41";
			break;
		case "51":
			$database = "sxtest_51";
			break;
		case "52":
			$database = "sxtest_52";
			break;
		case "99":
			$database = "sxtest_testdb";
			break;
	}*/
	
	$hostname = '192.168.10.61';	//The hostname or the IP address of the MySQL database
	$username = 'ta_admin'; 		//Username to get access to the MySQL database
	$password = 'dhocc648';			//Security password of the above username to get access to the MySQL database

	$systemx_connect = mysqli_connect($hostname, $username, $password,$database);
	
	if (mysqli_connect_errno())
	{
		echo "Failed to connect to MySQL: " . mysqli_connect_error();
	}
?> 