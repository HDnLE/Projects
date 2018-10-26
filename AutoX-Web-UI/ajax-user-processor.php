<?php
	session_start();
	$action = $_POST["action"];
	include "users-functions.php";
	require_once "dbconn-users.php";
	include "functions.php";
	date_default_timezone_set('Europe/Oslo');
	$activity_log_date = date("Y-m-d H:m:s");
	switch($action)
	{
		//******************* USERS *******************//
		//Save new user to database
		case "create_user":
			$last = ucfirst(strtolower($_POST["n_last"]));
			$first = ucfirst(strtolower($_POST["n_first"]));
			$username = $_POST["n_username"];
			$password = md5($_POST["n_password"]);
			$date = date("Y-m-d");
			$role = $_POST["n_role"];
			$rights = 0;
			$avatar = "images/avatar/default-user.png";
			if ($role == "Test Developer")
			{
				$rights = 1;
			}
			
			$sql = "SELECT * FROM designers WHERE username='$username'";
			$info = "<b>Feil:</b> Brukernavn er allerede i bruk";
			
			if (!is_username_exist($username))
			{
				$sql = "INSERT INTO designers (lname, fname, username, password, date_reg, manage_rights, role, avatar) VALUES ('$last', '$first', '$username', '$password', '$date', $rights, '$role', '$avatar')";
				$info = "<b>Vellykket:</b> Ny bruker har blitt lagt til. Viderekobler til påloggingssiden...";
			}
			
			
			break;
		//Log user in to main web ui
		case "login":
			$username = $_POST["n_username"];
			$password = md5($_POST["n_password"]);
			$database = $_POST["n_database"];
			$sql = "SELECT * FROM designers WHERE USERNAME='$username' AND PASSWORD='$password'";
			$result = mysqli_query($users_connect,$sql)or die(mysqli_error($users_connect));
			$info = "Åpner hovedside. Vennligst vent...";
			if (mysqli_num_rows($result) == 0)	//If user authentication fails
			{
				$info = "<b>Feil:</b> Ugyldig brukernavn eller passord";
			}
			else
			{
				if (!is_username_active($username))
				{
					$info = "<b>Feil:</b> Denne brukeren er deaktivert! Ta kontakt med administrator.";
					$log = "A deactivated user has attempted to log in";
					mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$username')") or die(mysqli_error());
				}
				else
				{
					//Set login status to online when user authentication is successful
					mysqli_query($users_connect, "UPDATE designers SET STATUS=1, ACCESS_POINT=CONCAT(ACCESS_POINT, 'Web|') WHERE username='$username' AND ACCESS_POINT NOT LIKE '%Web%'");
					mysqli_query($users_connect, "UPDATE designers SET STATUS=1, ACCESS_POINT='Web|' WHERE username='$username' AND ACCESS_POINT IS NULL");
					
					//Set session name based on the successful logged username
					$_SESSION["username"] = $username;
					
					//Set database session name
					$_SESSION["database"] = $database;
					
					//Log out when no activity within expire time
					/* $_SESSION['last_activity'] = time();
					$_SESSION['expire_time'] = 60; */
					$_SESSION['timestamp']=time();
					$log = "Logged in to web UI";
					mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$username')") or die(mysqli_error());
				}
			}
			
			break;
		//Save changes made on the user profile
		case "update_profile":
			$username = $_SESSION["username"];
			$first = $_POST["nFirstName"];
			$last = $_POST["nLastName"];
			$current = $_POST["nPassword"];
			$role = $_POST["nRole"];
			//$avatar = $_POST["nAvatar"];
			$set_password = "";
			
			$info = "<b>Vellykket:</b> Profilen har blitt oppdatert";
			
			//Save the changed password
			if (!empty($current))
			{
				$current = md5($current);
				$set_password = ", PASSWORD='$current'";
				$info = "Profilen har blitt oppdatert. Logger ut nå...<script>setTimeout(function () {window.location.href = 'logout.php';}, 2000);</script>";
			}
			mysqli_query($users_connect, "UPDATE designers SET FNAME='$first', LNAME='$last' $set_password WHERE USERNAME='$username'") or die(mysqli_error());
			$log = "Updated user profile";
			$sql = "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$username')";
			
			break;
		//Set or unset a user with admin rights
		case "change_role":
			$username = $_POST["id"];
			$rights = $_POST["tm_rights"];
			$sql = "UPDATE designers SET manage_rights='$rights' WHERE username='$username'";
			$info = "Endre TM-rettigheter til utvalgte brukere...";
			break;
		//Delete a user
		case "delete_user":
			$username = $_POST["id"];
			$sql = "DELETE FROM designers WHERE username='$username'";
			
			//If the user has existing testsuites/testcases, disallow the request
			if (is_user_owner($username))
			{
				$sql = "";
			}
			$info = "Sletter utvalgte elementer...";
			break;
		//Reset a user's password to default
		case "reset_password":
			$username = $_POST["id"];
			$password = md5("ranorex123");	//Default password is "ranorex123"
			$sql = "UPDATE designers SET password='$password' WHERE username='$username'";
			$info = "Tilbakestille passordet til utvalgte brukere...";
			break;
			
		case "logout":
			$username = $_POST["username"];
			mysqli_query($users_connect, "UPDATE designers SET status=0 WHERE username='$username'");
			$log = "Logged out from web UI";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$username')") or die(mysqli_error());
			break;
	}
	
	//Notify the user the response of the request
	echo $info;
	
	//Execute the MySQL query
	mysqli_query($users_connect,$sql) or die(mysqli_error($users_connect));
	mysqli_close($users_connect);
?>