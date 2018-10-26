<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="utf-8">
<meta http-equiv="X-UA-Compatible" content="IE=edge">
<meta name="viewport" content="width=device-width, initial-scale=1">
<link rel="shortcut icon" href="images/hovemedical.ico" />
<title>AutoX Web UI | Logginn</title>
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
<style type="text/css">
	.login-form {
		position: fixed;
		top: 50%;
		left: 50%;
		transform: translate(-50%, -50%);
		width:345px;
		
		
	}
	body {
		background:#666;
		background-image: url("images/systemx.png");
		background-repeat: no-repeat;
		background-position: center;
		background-attachment: fixed;
	}
    .login-form form {
    	margin-bottom: 15px;
        background: #f7f7f7;
        box-shadow: 0px 2px 2px rgba(0, 0, 0, 0.3);
        padding: 30px;
		border:1px solid #EEE;
		border-radius:5px;
    }
    .login-form h2 {
        margin: 0 0 15px;
    }
    .form-control, .btn {
        min-height: 38px;
        border-radius: 2px;
    }
    .btn {        
        font-size: 15px;
        font-weight: bold;
		border-radius: 2px !important;
    }
	input {
		font-size:18px !important;
		color:Firebrick !important
	}
</style>
</head>
<body>
<div class="login-form">
    <form action="" method='POST' id="frm_login">
        <h2 class="text-center">Logg inn</h2>       
        <div class="form-group">
            <input type="text" class="form-control" placeholder="Brukernavn" required="required" name="n_username">
        </div>
        <div class="form-group">
            <input type="password" class="form-control" placeholder="Passord" required="required" name="n_password">
        </div>
		<div class="form-group">
			<label for="database" style="font-size:15px">Velg database</label>
			<select class="form-control" name="n_database" id="database">
				<option selected disabled></option>
				<?php		
					$link = mysqli_connect('192.168.10.61', 'ta_admin', 'dhocc648');
					$res = mysqli_query($link, "SHOW DATABASES WHERE `Database` LIKE '%sxtest_%' AND `Database` NOT LIKE '%testdb%'") or die(mysqli_error());

					while ($row = mysqli_fetch_array($res, MYSQL_NUM)) 
					{
						$dbValue = $row[0];
						$dbList = str_replace("sxtest_", "", $dbValue);
						$dbItem = substr($dbList, 0, 1) . "." . substr($dbList, 1, 1);
						echo "<option value='$dbValue'>System X $dbItem</option>";
					}
				?>
			</select>
		</div>
		<div class="form-group">
            <button type="button" class="btn btn-primary btn-block" id="btn_login">Logg inn</button>
        </div>    
		
    </form>
	<div id="info"></div>
</div>

<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script> 
<script src="js/jquery.ajax.js"></script>
	<script src="js/global.functions.js"></script>
	<script src="js/jquery.users.js" type="application/javascript"></script>
</body>
</html>                                		                            