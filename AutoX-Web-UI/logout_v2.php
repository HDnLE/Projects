<?php
	session_start();
	include "dbconn-users.php";
	$designer = $_SESSION["username"];
	$activity_log_date = date("Y-m-d H:m:s");
	mysqli_query($GLOBALS['users_connect'], "UPDATE designers SET ACCESS_POINT=REPLACE(ACCESS_POINT, 'Web|','') WHERE username='$designer' AND ACCESS_POINT LIKE '%Main%'");
	mysqli_query($GLOBALS['users_connect'], "UPDATE designers SET STATUS=0, ACCESS_POINT=NULL WHERE username='$designer' AND ACCESS_POINT='Web|'");
	$log = "Logged out from web UI due to inactivity";
	mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
	unset($_SESSION['username']);
	unset($_SESSION['database']);
	setcookie("user", "", time()-3600);
	setcookie("token", "", time()-3600);
?>
<!DOCTYPE html>
<html lang="en">

<head>
	<!-- Required meta tags -->
	<meta charset="utf-8">
	<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
	<title>AutoX Web UI | Logg ut</title>
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
	<script>
		//localStorage.setItem('name', "/autox-web-ui");
		sessionStorage.setItem('login',null);
		//setTimeout(function () {window.location.href = "login.php";}, 2000); 
	</script>
</head>

<body>
	<div class="container-scroller">
		<!-- partial:partials/_navbar.html -->
		<nav class="navbar col-lg-12 col-12 p-0 fixed-top d-flex flex-row">
			<div class="text-center navbar-brand-wrapper d-flex align-items-top justify-content-center">
				<a class="navbar-brand brand-logo" href="/autox-web-ui"><img src="images/hms-logo.png" alt="logo" style="height:85px;width:190px"/></a>
			</div>
			<div class="navbar-menu-wrapper d-flex align-items-center">
				<ul class="navbar-nav navbar-nav-left header-links d-none d-md-flex">
					<li class="nav-item special-font" style="width:600px">
						Auto<i style="color:#FBB917;font-weight:bold">X</i> Web UI									
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
			</nav>
			<!-- partial -->
			<div class="main-panel">
				<div class="content-wrapper">
					<div class="row">
						<div class="col-12 grid-margin">
							
							<div class="card-body">
								<div class="alert alert-danger center">
									<h4>Sessjonen din har utløpt. Vennligst <a href="login.php">logg inn</a> igjen...</h4>
								</div>
							</div>
							
						</div>
					</div>
				</div>
				<!-- content-wrapper ends -->
				<!-- partial:partials/_footer.html -->
				<footer class="footer">
					<div class="container-fluid clearfix">
						<span class="text-muted d-block text-center text-sm-left d-sm-inline-block">Hove Medical | Dyrmyrgata 35, 3611 Kongsberg</span>
						<span class="text-muted float-none float-sm-right d-block mt-1 mt-sm-0 text-center">Maintained & Developed By: <a href="mailto:rommel@hovemedical.no">Rommel</a></span>
					</div>
				</footer>
				<!-- partial -->
			</div>
			<!-- main-panel ends -->
		</div>
		<!-- page-body-wrapper ends -->
	</div>
	<!-- container-scroller -->

	<!-- plugins:js -->
	<script src="node_modules/jquery/dist/jquery.min.js"></script>
	<script src="node_modules/popper.js/dist/umd/popper.min.js"></script>
	<script src="node_modules/bootstrap/dist/js/bootstrap.min.js"></script>
	<!-- endinject -->
	<!-- Plugin js for this page-->
	<script src="node_modules/chart.js/dist/Chart.min.js"></script>
	<!-- End plugin js for this page-->
	<!-- inject:js -->
	<script src="js/off-canvas.js"></script>
	<script src="js/misc.js"></script>
	<!-- endinject -->
	<!-- Custom js for this page-->
	<script src="js/dashboard.js"></script>
	<script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyB5NXz9eVnyJOA81wimI8WYE08kW_JMe8g&callback=initMap" async defer></script>
	<script src="js/maps.js"></script>
	<!-- End custom js for this page-->
</body>

</html>