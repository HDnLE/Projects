<?php
	$hostname = '192.168.10.61';	//The hostname or the IP address of the MySQL database
	$dbname   = 'systemxtest_main'; 		//Name of the MySQL database
	$username = 'ta_admin'; 		//Username to get access to the MySQL database
	$password = 'dhocc648';			//Security password of the above username to get access to the MySQL database

	$users_connect = mysqli_connect($hostname, $username, $password,$dbname);
	
	if (mysqli_connect_errno())
	{
		echo "Failed to connect to MySQL: " . mysqli_connect_error();
	}
?> 