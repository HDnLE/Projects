<?php
	ob_start();
	session_start();
	
	error_reporting( E_ALL );
	ini_set('display_errors', 1);
	
	include "functions.php";
	include "users-functions.php";
	include "dbconn-systemx.php";
	include "dbconn-users.php";
	
	//Go to login page if user attempts to go to the main page without user authentication
	if ((empty($_SESSION["username"])) || !get_user_info($_SESSION["username"], 12))
	{
		header("location: login.php");
		exit;
	}
	/*setcookie("user", "", time()-3600);
	setcookie("token", "", time()-3600);*/
?>
<!DOCTYPE html>
<html lang="en">

<head>
	<!-- Required meta tags -->
	<meta charset="utf-8">
	<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
	<meta http-equiv="Content-Type" content="text/html; charset=Windows-1252">
	<title>AutoX Web UI</title>
	<!-- plugins:css -->
	<link rel="stylesheet" href="node_modules/mdi/css/materialdesignicons.min.css">
	<link rel="stylesheet" href="node_modules/simple-line-icons/css/simple-line-icons.css">
	<link href='https://fonts.googleapis.com/css?family=Audiowide' rel='stylesheet'>
	
	<!-- endinject -->
	<!-- plugin css for this page -->
	<!-- End plugin css for this page -->
	<!-- inject:css -->
	<link rel="stylesheet" href="css/style.css">
	<!-- endinject -->
	<link rel="shortcut icon" href="images/hovemedical.ico" />
	<style>
		.resultContainer {
			border:1px solid #DDD;
			background:#FFF;
			width:30%;
			border-top-right-radius:5px;
			border-bottom-right-radius:5px;
			border-bottom-left-radius:5px;
			position:relative;
			-webkit-box-shadow: 2px 3px 5px -1px rgba(0,0,0,0.75);
			-moz-box-shadow: 2px 3px 5px -1px rgba(0,0,0,0.75);
			box-shadow: 2px 3px 5px -1px rgba(0,0,0,0.75);
			font-size:14px;
			position:absolute;
			z-index:1;
			top:50px;
			left:10;
			color:#111;
		}
		#txtSearch {
			border-radius:4px;
			border:0;
			padding:5px;
			width:300px;
			font-size:14px;
			background: rgba(255, 255, 255, 0.2);
			color:#FFF;
			background-image: url('images/search.png');
			background-repeat: no-repeat;
			background-position: right;
			background-size: 18px 18px;
			background-origin: content-box;
		}
		.rowResult, .rowObjResult {
			padding:5px 5px 0px 5px;
		}
		.rowResult:not(:last-child), .rowObjResult:not(:last-child) {
			border-bottom:1px solid #EEE;
		}
		.rowResult:hover, .rowObjResult:hover {
			background: linear-gradient(to right, #01A9DB , #E0F2F7);
			cursor:pointer;
			color:#FFF;
		}
		.rowResult:hover > p, .rowObjResult:hover > p{
			color:#111;
		}
		#txtSearch::placeholder { /* Chrome, Firefox, Opera, Safari 10.1+ */
			color: #EEE;
			opacity: 1; /* Firefox */
		}
		#txtSearch:-ms-input-placeholder {
			color: #EEE;
		}
		#txtSearch::-ms-input-placeholder {
			color: #EEE;
		}
	</style>
</head>

<body>
	<?php
		$user_login = get_user_info($_SESSION["username"], 1) . " " . get_user_info($_SESSION["username"], 2);
		$role = get_role_name(get_user_info($_SESSION["username"], 7));
		$role_id = get_user_info($_SESSION["username"], 7);
		$avatar = get_user_info($_SESSION["username"], 10);
		$curr_database = $_SESSION["database"];
	?>
	<div class="container-scroller">
		<!-- partial:partials/_navbar.html -->
		<nav class="navbar col-lg-12 col-12 p-0 fixed-top d-flex flex-row">
			<div class="text-center navbar-brand-wrapper d-flex align-items-top justify-content-center">
				<a class="navbar-brand brand-logo" href="/autox-web-ui"><img src="images/hms-logo.png" alt="logo" style="height:85px;width:190px"/></a>
			</div>
			<div class="navbar-menu-wrapper d-flex align-items-center">
				<ul class="navbar-nav navbar-nav-left header-links d-none d-md-flex">
					<li class="nav-item special-font embossed">
						Auto<i style="color:#FBB917;font-weight:bold">X</i> Web UI
					</li>
				</ul>
				<ul class="navbar-nav navbar-nav-right">
					<li class="nav-item">
						<input type='text' placeholder='Søk testcaser...' id='txtSearch'>
						<div id='searchResult'></div>	
					</li>
					<li class="nav-item text-14" id="liDateTime" style="font-size:14px">
						<?php 
							setlocale(LC_TIME, 'norwegian');
							$day = ucfirst(strftime('%A',time()));
							echo $day . strftime(' %d. %B %Y kl. %H:%M',time());
						?>
					</li>
					<li class="nav-item d-none d-lg-block dropdown">
						<a class="nav-link dropdown-toggle" href="#" data-toggle="dropdown" id="dbDropdown">
							<b class="pointer text-13 modalClass" data-toggle="modalx" data-target="#modalChangeDatabase" data-icon="database" title="Bytt database"><i class="mdi mdi-database"></i><?php echo get_database_name(); ?></b>	
						</a>
						<div class="dropdown-menu dropdown-menu-right navbar-dropdown preview-list" aria-labelledby="dbDropdown">
							<?php
								$link = mysqli_connect('192.168.10.61', 'ta_admin', 'dhocc648');
								$res = mysqli_query($link, "SHOW DATABASES WHERE `Database` LIKE '%sxtest_%' AND `Database` NOT LIKE '%testdb%'") or die(mysqli_error());

								while ($row = mysqli_fetch_array($res, MYSQL_NUM)) 
								{
									$dbValue = $row[0];
									$dbList = str_replace("sxtest_", "", $dbValue);
									$disabled = ($dbValue == $curr_database) ? "selected disabled" : "";
									$dbItem = substr($dbList, 0, 1) . "." . substr($dbList, 1, 1);
									echo "
										<a class='dropdown-item preview-item dbClass pointer' data-dbname='$dbValue'>
											<div class='preview-item-content flex-grow'>
												<h6 class='preview-subject ellipsis font-weight-medium pointer'>System X $dbItem</h6>
											</div>
										</a>
									";
								}
							?>
						</div>
					</li>
					<li class="nav-item dropdown">
						<?php
							$data_toggle = "";
							$notification_counter = "";
							if ((get_open_tasks() > 0) && (get_role_info($role_id, 19)))
							{
								$data_toggle = "dropdown";
								$notification_counter = "<span class='count'>" . get_open_tasks() . "</span>";
							}
						?>
						<a class="nav-link count-indicator dropdown-toggle" id="notificationDropdown" href="#" data-toggle="<?php echo $data_toggle; ?>">
						<i class="mdi mdi-bell-ring"></i>
						<?php
							echo $notification_counter;
						?>
							
						</a>
						<div class="dropdown-menu dropdown-menu-right navbar-dropdown preview-list" aria-labelledby="notificationDropdown">
						  <a class="dropdown-item">
							<p class="mb-0 font-weight-normal float-left">Du har <?php echo get_open_tasks(); ?> åpne oppgaver
							</p>
							<span class="badge badge-pill badge-warning float-right pointer" id="spViewTask">Vis alle</span>
						  </a>
						  <div class="dropdown-divider"></div>
							<?php
								$user = $_SESSION["username"];
								$sql = mysqli_query($GLOBALS['users_connect'], "SELECT * FROM tasks WHERE STATUS = 'IN PROGRESS' AND USER='$user' LIMIT 5");
								while($row = mysqli_fetch_array($sql, MYSQL_NUM))
								{
									$lc_status = strtolower($row[3]);
									$status_class = str_replace(" ", "-", $lc_status);
									echo "<a class='dropdown-item preview-item'>";
									echo "<div class='preview-thumbnail'>
									  <div class='preview-icon $status_class'></div>
									</div>";
									echo "<div class='preview-item-content' title='$row[2]'>";
									$task_length = strlen($row[2]);
									$task = ($task_length > 30) ? substr($row[2],0,30) . "..." : $row[2];
									echo "<h6 class='preview-subject font-weight-medium'>$task</h6>";
									echo "<p class='font-weight-light small-text'>
										$row[3]
									</p>";
									echo "</div>";
									echo "</a>";
								}
							?>
						</div>
					</li>
					<li class="nav-item d-none d-lg-block dropdown">
						<a class="nav-link dropdown-toggle" href="#" data-toggle="dropdown" id="messageDropdown">
							<img class="img-xs rounded-circle" src="<?php echo $avatar; ?>" alt="">
						</a>
						<div class="dropdown-menu dropdown-menu-right navbar-dropdown preview-list" aria-labelledby="messageDropdown">
							<a class="dropdown-item preview-item account">
								<div class="preview-item-content flex-grow">
									<h6 class="preview-subject ellipsis font-weight-medium pointer modalClass" data-toggle="modal" data-icon="account" data-target="#modalUserProfile" title="Min Profil">Profil</h6>
								</div>
							</a>
							<a class="dropdown-item preview-item account">
								<div class="preview-item-content flex-grow">
									<h6 class="preview-subject ellipsis font-weight-medium pointer modalClass" data-toggle="modal" data-icon="logout" data-target="#modalConfirmLogout" title="Logg ut" id="aLogout">Logg ut</h6>
								</div>
							</a>
						</div>
					</li>
				</ul>
				<button class="navbar-toggler navbar-toggler-right d-lg-none align-self-center" type="button" data-toggle="offcanvas">
					<span class="icon-menu"></span>
				</button>
			</div>
		</nav>
		<!-- partial -->
		<div class="container-fluid page-body-wrapper">
			<!-- partial:partials/_sidebar.html -->
			<nav class="sidebar sidebar-offcanvas" id="sidebar">
				<ul class="nav">
					<li class="nav-item nav-profile">
						<div class="nav-link">
							<div class="profile-image"> <img src="<?php echo $avatar; ?>" alt="image" data-toggle="modal" data-target="#modalProfilePhoto" class="pointer modalClass" data-icon="face-profile" title="Profil bilde" /> <span class="online-status online"></span> </div>
							<div class="profile-name">
								<p class="name"><?php echo $user_login; ?></p>
								<p class="designation"><?php echo $role; ?></p>
								<div class="badge badge-teal mx-auto mt-3">Online</div>
							</div>
						</div>
					</li>
					<li class="nav-item"><a class="nav-link" href="/autox-web-ui"><img class="menu-icon" src="images/menu_icons/home.png" alt="menu icon"><span class="menu-title">Hjem</span></a></li>
					<li class="nav-item">
						<a class="nav-link" data-toggle="collapse" href="#general-manage" aria-expanded="false" aria-controls="general-manage"> <img class="menu-icon" src="images/menu_icons/tests.png" alt="menu icon"> <span class="menu-title">Testskripter</span><i class="menu-arrow"></i></a>
						<div class="collapse" id="general-manage">
							<ul class="nav flex-column sub-menu">
								<li class="nav-item menu-item"> <a class="nav-link" href="?id=1"> Testsuiter</a></li>
								<li class="nav-item menu-item"> <a class="nav-link" href="?id=2">Testcaser</a></li>
								<li class="nav-item menu-item"> <a class="nav-link" href="?id=3">Test kommandoer</a></li>
								<li class="nav-item menu-item"> <a class="nav-link" href="?id=4">System X objekter</a></li>
								<li class="nav-item menu-item"> <a class="nav-link" href="?id=19">CI-Teststatus</a></li>
							</ul>
						</div>
					</li>
					<li class="nav-item">
						<a class="nav-link" data-toggle="collapse" href="#general-users" aria-expanded="false" aria-controls="general-users"> <img class="menu-icon" src="images/menu_icons/users.png" alt="menu icon"> <span class="menu-title">Brukere</span><i class="menu-arrow"></i></a>
						<div class="collapse" id="general-users">
							<ul class="nav flex-column sub-menu">
								<li class="nav-item menu-item"> <a class="nav-link" href="?id=6">Oppgaver</a></li>
								<li class="nav-item menu-item"> <a class="nav-link" href="?id=7">Brukerliste</a></li>
							</ul>
						</div>
					</li>
					<li class="nav-item">
						<a class="nav-link" data-toggle="collapse" href="#general-help" aria-expanded="false" aria-controls="general-help"> <img class="menu-icon" src="images/menu_icons/help.png" alt="menu icon"> <span class="menu-title">Hjelp</span><i class="menu-arrow"></i></a>
						<div class="collapse" id="general-help">
							<ul class="nav flex-column sub-menu">
								<li class="nav-item menu-item"> <a class="nav-link" href="?id=13">Om dette</a></li>
								<li class="nav-item menu-item"> <a class="nav-link" href="?id=14">Web UI</a></li>
								<li class="nav-item menu-item"> <a class="nav-link" href="?id=15">System XAuto-Test</a></li>
								<li class="nav-item menu-item"> <a class="nav-link" href="?id=16">Scripter på XStudio</a></li>
								<li class="nav-item menu-item"> <a class="nav-link" href="?id=5">Endringer/Oppdateringer</a></li>
							</ul>
						</div>
					</li>
					<?php
						if (get_role_info($role_id, 22))
						{
							echo "
								<li class='nav-item'>
									<a class='nav-link' data-toggle='collapse' href='#general-tools' aria-expanded='false' aria-controls='general-users'> <img class='menu-icon' src='images/menu_icons/manage.png' alt='menu icon'> <span class='menu-title'>Admin verktøy</span><i class='menu-arrow'></i></a>
									<div class='collapse' id='general-tools'>
										<ul class='nav flex-column sub-menu'>";
										if (is_admin($_SESSION["username"]))
										{
											echo "
												<li class='nav-item menu-item'> <a class='nav-link' href='?id=9'>Activity Logs</a></li>
												<li class='nav-item menu-item'> <a class='nav-link' href='?id=8'>User Administration</a></li>
											";
										}
										echo "
											<li class='nav-item menu-item'> <a class='nav-link' href='?id=10'>Testcase Manager</a></li>
										</ul>
									</div>
								</li>
							";
						}
					?>
					<li class="nav-item">
						<a class="nav-link" data-toggle="collapse" href="#external-links" aria-expanded="false" aria-controls="external-links"> <img class="menu-icon" src="images/menu_icons/external-links.png" alt="menu icon"> <span class="menu-title">Eksterne linker</span><i class="menu-arrow"></i></a>
						<div class="collapse" id="external-links">
							<ul class="nav flex-column sub-menu">
								<li class="nav-item menu-item"> <a class="nav-link" href="http://jiracon-appsrv:8090/confluence/"  target="_blank">Confluence</a></li>
								<li class="nav-item menu-item"> <a class="nav-link" href="http://jiracon-appsrv:8080/secure/MyJiraHome.jspa"  target="_blank">Jira</a></li>
								<li class="nav-item menu-item"> <a class="nav-link" href="http://cistaging:8080/view/Ranorex/" target="_blank">Jenkins</a></li>
								<li class="nav-item menu-item"> <a class="nav-link" href="http://testrail.hovemedsys.local/index.php?/dashboard" target="_blank">TestRail</a></li>
							</ul>
						</div>
					</li>
					<li class="nav-item"><a class="nav-link pointer"><img class="menu-icon" src="images/menu_icons/logout.png" alt="menu icon"><span class="menu-title modalClass" title="Logg ut" data-icon="logout" data-toggle="modal" data-target="#modalConfirmLogout" id="aLogout">Logg ut</span></a></li>
				</ul>
				<div class="online-users text-13">
					<div class="badge badge-teal mx-auto mt-3">Hvem er online?</div>
				</div>
				<div id="maincontainer">
					<?php
						$user_limit = 12;
						$query = "SELECT * FROM designers WHERE ACTIVE=1 AND STATUS=1 ORDER BY LNAME ASC LIMIT $user_limit";
						$result = mysqli_query($users_connect,$query)or die(mysqli_error());
						$sql =  mysqli_query($users_connect,"SELECT * FROM designers WHERE ACTIVE=1 AND STATUS=1")or die(mysqli_error());
						while($row = mysqli_fetch_array($result, MYSQL_NUM))
						{
							$you = ($_SESSION["username"] == $row[3]) ? " (Du)" : "";
							$access_point = $row[13];
							$access_point_split = explode("|",$access_point);
							$access_point = (count($access_point_split) == 2) ? $access_point_split[0] : $access_point_split[0] . " og " . $access_point_split[1];
							echo "
								<img class='img-xs' src='$row[10]' title='$row[1] $row[2]$you\nLogg inn fra: $access_point'>
							";
						}
					?>
				</div>
				
				<div class="text-13" style="clear:left;margin-top:10px;float:right;padding-right:5px">
					<?php
						if (mysqli_num_rows($sql) > 0)
						{
							if (mysqli_num_rows($sql) > $user_limit)
							{
								echo "<a href='?id=7&show=online'>Vis alle</a>";
							}
						}
						else
						{
							echo "<p class='text-danger text-13'>Ingen er online...</p>";
						}
					?>
				</div>
				
			</nav>
			<!-- partial -->
			<div class="main-panel">
				<div class="content-wrapper">
					<div class="row">
						<div class="col-12 grid-margin">
							<?php
								$limit = get_user_info($_SESSION["username"], 11);
								$rec_count = get_record_count("activity_logs", "users_connect");
								if (($rec_count >= $limit) && (is_admin($_SESSION["username"])))
								{
									echo "<div class='alert alert-danger text-13'><b class='blink'>Alert:</b> You have now reached (<b>$rec_count</b> log entries) the threshold limit of $limit. <a class='bold pointer text-danger' data-toggle='modal' data-target='#modalCleanLogs'>Clean logs</a> now! Or <a class='bold pointer text-danger' data-toggle='modal' data-target='#modalLogLimit'>update threshold limit</a>...</div>";
								}
								if (getTestStatusCount("TC", 0) > 0)
								{
									echo "<div class='alert alert-danger text-13'>
											<span style='float:right;position:relative;top:-15px;left:18px' id='spnAlert' class='mdi mdi-close icon-sm pointer' title='Skjul denne meldingen...'></span>
											<b class='blink'>CI Test Alert:</b> En testfeil har oppstått. Vennligst gå til Jenkins <a href='http://cistaging:8080/view/Ranorex/' target='_blank'>logg</a> for mer informasjon... På tiden er det <b class='text-14'>" . getTestStatusCount("TC", 0) . "</b> testcaser som er feil.
										</div>";
								}
								$page = "dashboard.php";
								if (isset($_GET['id']))
								{
									$id = $_GET['id'];
									$page = $id . ".php";
								}
								if (!file_exists($page))
								{
									echo "<div class='text-error'><img src='images/error.png'><h2 style='margin-top:5px;display:inline'>Ooops! Vi finner ikke denne siden!</h2></div>";
								}
								else
								{
									include $page;
								}
							?>
						</div>
					</div>
				</div>
				<!-- content-wrapper ends -->
				<!-- partial:partials/_footer.html -->
				<footer class="footer">
					<div class="container-fluid clearfix">
						<span class="d-block text-center text-sm-left d-sm-inline-block">Hove Medical | Dyrmyrgata 35, 3611 Kongsberg</span>
						<span class="float-none float-sm-right d-block mt-1 mt-sm-0 text-center">Auto X Web UI (Database Version: <b><?php echo get_database_name(); ?></b> | Build Date: <b><?php echo get_last_modified_date(); ?></b> | Maintained & Developed By: <a href="mailto:rommel@hovemedical.no">Rommel</a>)</span>
					</div>
				</footer>
				<!-- partial -->
			</div>
			<!-- main-panel ends -->
		</div>
		<!-- page-body-wrapper ends -->
	</div>
	
	<!-- ***************** USER PROFILE **************** -->
<div class="modal fade" id="modalUserProfile" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle"></h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmUserProfile">
			<div class="modal-body">
				<div class="form-group row">
					<label for="txtRegDate" class="col-sm-3 col-form-label">Profil bilde</label>
					<div class="col-sm-9">
						<img class="img-xs pointer modalClass" src="<?php echo $avatar; ?>" data-icon="face-profile" title="Profil bilde" data-toggle="modal" data-target="#modalProfilePhoto" id="imgAvatar">
						<label style="margin-top:15px;font-size:12px">(Klikk bilde for å endre)</label>
					</div>
				</div>
				<div class="form-group row">
					<label for="txtFirstName" class="col-sm-3 col-form-label">*Fornavn</label>
					<div class="col-sm-9">
						<input type="text" class="form-control" id="txtFirstName" name="nFirstName" required value="<?php echo get_user_info($_SESSION["username"], 1); ?>">
					</div>
				</div>
				<div class="form-group row">
					<label for="txtLastName" class="col-sm-3 col-form-label">*Etternavn</label>
					<div class="col-sm-9">
						<input type="text" class="form-control" id="txtLastName" name="nLastName" required value="<?php echo get_user_info($_SESSION["username"], 2); ?>">
					</div>
				</div>
				<div class="form-group row">
					<label for="txtUsername" class="col-sm-3 col-form-label">Brukernavn</label>
					<div class="col-sm-9">
						<input type="text" class="form-control" id="txtUsername" name="nUserName" readonly value="<?php echo $_SESSION["username"]; ?>">
					</div>
				</div>
				<div class="form-group row">
					<label for="txtRegDate" class="col-sm-3 col-form-label">Registert dato</label>
					<div class="col-sm-9">
						<input type="text" class="form-control" id="txtRegDate" readonly value="<?php echo get_user_info($_SESSION["username"], 4); ?>">
					</div>
				</div>
				<div class="form-group row">
					<label class="col-sm-3 col-form-label">Rolle</label>
					<div class="col-sm-9">
						<input type="text" class="form-control" name="nRole" readonly value="<?php echo get_role_name(get_user_info($_SESSION["username"], 7)); ?>">
					</div>
				</div>
				<div class="form-group row">
					<label for="txtNewPassword" class="col-sm-3 col-form-label">Ny passord</label>
					<div class="col-sm-9">
						<input type="password" class="form-control" id="txtNewPassword">
					</div>
				</div>
				<div class="form-group row">
					<label for="txtPassword" class="col-sm-3 col-form-label">Bekreft passord</label>
					<div class="col-sm-9">
						<input type="password" class="form-control" id="txtPassword" name="nPassword">
					</div>
				</div>
				<div id="info"></div>
			</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-primary" id="btnUpdateProfile" data-form="#frmUserProfile">Oppdater</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
			</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** UPLOAD PROFILE PHOTO **************** -->
<div class="modal fade" id="modalProfilePhoto" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">Endre profil bilde</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmProfilePhoto" enctype="multipart/form-data">
			<div class="modal-body">
				<div class="form-group row">
					<label for="txtPhoto" class="col-sm-6 col-form-label">*Last opp nytt bilde</label>
					<div class="col-sm-9">
						<input type="file" class="form-control" id="txtPhoto" name="nPhoto">
					</div>
				</div>
				<div class="form-group row" style="border:1px solid #EEE;padding:15px">
					<?php
						$avatar = array("Agent Coulson", "Black Widow", "Captain America", "Giant Man", "Iron Man", "Hawkeye", "Hulk", "Loki", "Nick Fury", "Thor", "War Machine");
						for ($x=0; $x<=10; $x++)
						{
							echo "<img class='img-xs rounded-circle avatar pointer' src='images/avatar/avatar$x.png' title='$avatar[$x]' style='margin-right:5px'>";
						}
					?>
				</div>
				<div id="infoUpload"></div>
			</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-primary" id="btnUpload">Bekreft</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
			</div>
			</form>
		</div>
	</div>
</div>

<div class="modal-loading" role="dialog"></div>

<!-- ***************** CONFIRM LOGOUT DIALOG **************** -->
<div class="modal fade" id="modalConfirmLogout" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">Bekreft logg ut</h5>
			</div>
			<div class="text-danger text-13" style="padding:10px">Er du sikker på at du vil logge ut?</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-primary" id="btnLogout">Bekreft</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
			</div>
			
		</div>
	</div>
</div>

<!-- ***************** CHANGE DATABASE DIALOG **************** -->
<div class="modal fade" id="modalChangeDatabase" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle"></h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmChangeDatabase">
				<div class="modal-body">
					<div class="form-group">
						<label for="database">Velg database</label>
						<select class="form-control" name="n_database" id="database">
							<?php
								/*$databases = array(array("41", "System X 4.1"), array("51", "System X 5.1"), array("52", "System X 5.2"), array("99", "Test Database"));
								for ($i=0; $i<=3; $i++)
								{
									$disabled = ($databases[$i][0] == $curr_database) ? "selected disabled" : "";
									echo "<option value='" . $databases[$i][0] . "' $disabled>" . $databases[$i][1] . "</option>";
								}*/
								$link = mysqli_connect('192.168.10.61', 'ta_admin', 'dhocc648');
								$res = mysqli_query($link, "SHOW DATABASES WHERE `Database` LIKE '%sxtest_%' AND `Database` NOT LIKE '%testdb%'") or die(mysqli_error());

								while ($row = mysqli_fetch_array($res, MYSQL_NUM)) 
								{
									$dbValue = $row[0];
									$dbList = str_replace("sxtest_", "", $dbValue);
									$disabled = ($dbValue == $curr_database) ? "selected disabled" : "";
									$dbItem = substr($dbList, 0, 1) . "." . substr($dbList, 1, 1);
									echo "<option value='$dbValue' $disabled>System X $dbItem</option>"; //echo $row['Database'] . "\n";
								}
							?>
						</select>
					</div>
				</div>
				<div class="modal-footer hdr">
					<button type="button" class="btn btn-primary" id="btnChangeDB">Bekreft</button>
					<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
				</div>
				
			</form>
		</div>
	</div>
</div>

<!-- ***************** CLEAN ACTIVITY LOGS DIALOG **************** -->
<div class="modal fade" id="modalCleanLogs" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle"></h5>
			</div>
			<div class="text-error text-13" style="padding:10px">This will delete all activity logs. Continue?</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-primary" id="btnCleanLogs">Confirm</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
			</div>
			
		</div>
	</div>
</div>

<!-- ***************** SESSION EXPIRE DIALOG **************** -->
<div class="modal fade" id="modalSessionExpire" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document">
		<div class="modal-content" >
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">Session Expiration Warning!</h5>
			</div>
			<div class="text-error text-13" id="session" style="padding:5px"></div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-primary" id="btnResetTimer">Stay Signed In</button>
				<button type="button" class="btn btn-secondary" id="btnSignOut">Sign Me Out</button>
			</div>
			
		</div>
	</div>
</div>


<!-- ***************** SESSION TIMEOUT DIALOG **************** -->
<div class="modal fade" id="modalSessionTimeout" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">Test</h5>
			</div>
			<div style="padding:10px" id="liSessionTimer">test</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
			</div>
			
		</div>
	</div>
</div>

<div class="modal fade" id="modalSessionTimeout2" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="top:40%">
		<div class="modal-content center embossed" id="divTimeout">
		</div>
	</div>
</div>

<div class="modal fade" id="modalWait" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="top:50%">
		<div class="modal-content center wait">
			Vennligst vent...
		</div>
	</div>
</div>

	<!-- container-scroller -->

	<!-- plugins:js -->
	
	
	<script src="node_modules/jquery/dist/jquery.min.js"></script>
	<script src="https://code.jquery.com/jquery-1.12.4.js"></script>
	<script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>
	<script src="node_modules/popper.js/dist/umd/popper.min.js"></script>
	<script src="node_modules/bootstrap/dist/js/bootstrap.min.js"></script>
	<!-- endinject -->
	<!-- Plugin js for this page-->
	<script src="node_modules/chart.js/dist/Chart.min.js"></script>
	<!-- End plugin js for this page-->
	<!-- inject:js -->
	<script src="js/off-canvas.js"></script>

	<!-- endinject -->
	<!-- Custom js for this page-->
	
	<script src="js/dashboard.js"></script>
	<script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyB5NXz9eVnyJOA81wimI8WYE08kW_JMe8g&callback=initMap" async defer></script>
	<script src="js/maps.js"></script>
	
	<script src="js/jquery.systemx.js"></script>
	
	<script src="js/global.functions.js"></script>
	<script src="js/jquery.users.js"></script>
	<script src="js/jquery.ajax.js"></script>
	<!-- End custom js for this page-->
	
  
  <script>
  $( function() {
    $( "#txtLogDate, #txtTaskDate" ).datepicker({ firstDay: 1, dateFormat: 'yy-mm-dd' });
  } );
  </script>
  
</body>

</html>
<?php
    ob_end_flush(); // Flush the output from the buffer
?>