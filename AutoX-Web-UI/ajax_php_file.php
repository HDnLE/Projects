<?php
	ini_set('display_errors',1);
	error_reporting(E_ALL);
	session_start();
	require_once "dbconn-users.php";
	$activity_log_date = date("Y-m-d H:m:s");
	if(isset($_FILES["nPhoto"]["type"]))
	{
		$validextensions = array("jpeg", "jpg", "png");
		$temporary = explode(".", $_FILES["nPhoto"]["name"]);
		$file_extension = end($temporary);
		$error_code = $_FILES["nPhoto"]["error"];
		if ($error_code == 0)
		{
			if ((($_FILES["nPhoto"]["type"] == "image/png") || ($_FILES["nPhoto"]["type"] == "image/jpg") || ($_FILES["nPhoto"]["type"] == "image/jpeg")) && ($_FILES["nPhoto"]["size"] < 200000)//Approx. 100kb files can be uploaded.
			&& in_array($file_extension, $validextensions)) 
			{
				if ($_FILES["nPhoto"]["error"] > 0)
				{
					echo "Return Code: " . $_FILES["nPhoto"]["error"] . "<br/><br/>";
				}
				else
				{
					$username = $_SESSION["username"];
					$sourcePath = $_FILES['nPhoto']['tmp_name']; // Storing source path of the file in a variable
					$filenameitems = explode(".", $_FILES["nPhoto"]["name"]);
					$filename = $username . "." . $filenameitems[count($filenameitems) - 1];
					$targetPath = "images/avatar/".$filename; // Target path where file is to be stored
					if (move_uploaded_file($sourcePath,$targetPath))
					{
						echo "<b>SUCCESS:</b> Image uploaded successfully";
						mysqli_query($users_connect,"UPDATE designers SET AVATAR='$targetPath' WHERE username='$username'")or die(mysqli_error($users_connect));
						$log = "Changed profile photo";
						mysqli_query($users_connect, "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$username')") or die(mysqli_error());
					}
					else
					{
						echo "<b>ERROR:</b> Error uploading";
					}
				}
			}
			else
			{
				echo "<b>ERROR:</b> Unsupported size or type. Size limit : 200kb | Type: JPG, PNG, JPEG";
			}
		}
		else
		{
			echo "<b>ERROR:</b> No image selected";
		}
	}
?>