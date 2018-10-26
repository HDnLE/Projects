<?php
	include "dbconn-users.php";
	global $users_connect;
	//To get information from the logged in user
	function get_user_info($username, $col)
	{
		$result = "";
		$sql = mysqli_query($GLOBALS['users_connect'], "SELECT * FROM designers WHERE USERNAME='$username'");
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$col];
			}
		}
		return $result;
		mysqli_close($GLOBALS['users_connect']);
	}
	
	//Validate if the given username has admin rights
	function is_admin($username)
	{
		/* $result = true;
		if (get_user_info($username, 8) == 0)
		{$result = false;}
		return $result;	 */
		return get_user_info($username, 8) ;	
	}
	
	//Validate if the given username has rights to manage the tests
	function is_manage_allowed($username)
	{
		$result = true;
		if (get_user_info($username, 9) != 1)
		{$result = false;}
		return $result;	
	}
	
	//To get all the number of users
	function get_user_count()
	{
		$sql = mysqli_query($GLOBALS['users_connect'], "SELECT * FROM designers WHERE MASTER_ADMIN=0");
		return mysqli_num_rows($sql);
	}
	
	//To get all the number of users with admin rights
	function get_admin_count()
	{
		$sql = mysqli_query($GLOBALS['users_connect'], "SELECT * FROM designers WHERE MASTER_ADMIN=0 AND MANAGE_RIGHTS=1");
		return mysqli_num_rows($sql);
	}
	
	//To get all the number of online users
	function get_online_user_count()
	{
		$sql = mysqli_query($GLOBALS['users_connect'], "SELECT * FROM designers WHERE MASTER_ADMIN=0 AND STATUS=1 AND ACTIVE=1");
		return mysqli_num_rows($sql);
	}
	
	//To get the list of online users
	function get_online_users()
	{
		$sql = mysqli_query($GLOBALS['users_connect'], "SELECT * FROM designers WHERE MASTER_ADMIN=0 AND STATUS=1 AND ACTIVE=1 ORDER BY FNAME ASC LIMIT 5");
		$result = array();
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				array_push($result, "$row[1] $row[2]");
			}
		}
		return $result;
	}
	
	function is_username_active($username)
	{
		return get_user_info($username, 12);
	}
?>