<!DOCTYPE html>
<html lang="en">

<head>
	<!-- Required meta tags -->
	<meta charset="utf-8">
	<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
	<title>AutoX Web UI | Logginn</title>
	<!-- plugins:css -->
	<link rel="stylesheet" href="node_modules/mdi/css/materialdesignicons.min.css">
	<link rel="stylesheet" href="node_modules/simple-line-icons/css/simple-line-icons.css">
	<!-- endinject -->
	<!-- plugin css for this page -->
	<!-- End plugin css for this page -->
	<!-- inject:css -->
	<link rel="stylesheet" href="css/style.css">
	<!-- endinject -->
	<link rel="shortcut icon" href="images/hovemedical.ico" />
	<style>
		select option {
			background-color:rgba(0, 0, 0, 0.6)
		}
	</style>
</head>

<body>
	<div class="container-scroller">
		<div class="container-fluid page-body-wrapper full-page-wrapper">
			<div class="content-wrapper d-flex align-items-center auth login-full-bg">
				<div class="row w-100">
					<div class="col-lg-4 mx-auto">
						<div class="auth-form-dark text-left p-5">
							<h2>Brukkerinnlogging</h2>
							<h4 class="font-weight-light">Logg inn på kontoen din</h4>
							<form class="pt-5" action="" method='POST' id="frm_login">
								<form>
									<div class="form-group">
										<label for="username"></label>
										<input type="text" class="form-control" id="username" placeholder="Brukernavn" name="n_username">
										<i class="mdi mdi-account"></i>
									</div>
									<div class="form-group">
										<label for="key_login"></label>
										<input type="password" class="form-control" id="key_login" placeholder="Passord" name="n_password">
										<i class="mdi mdi-eye"></i>
									</div>
									<div class="form-group">
										<label for="database">Velg database</label>
										<select class="form-control" name="n_database" id="database">
											<option selected disabled></option>
											<option value="41">System X 4.1</option>
											<option value="51">System X 5.1</option>
											<option value="52">System X 5.2</option>
											<option value="99">Test Database</option>
										</select>
									</div>
									<div class="mt-5">
										<a class="btn btn-block btn-warning btn-lg font-weight-medium" id="btn_login">Logg inn</a>
									</div>
									<div class="mt-3 text-center">
										<a href="register.php" class="auth-link text-white">Opprett brukerpålogging</a>
									</div>
								</form>                  
							</form>
							<div id="info"></div>
						</div>
					</div>
				</div>
			</div>
			<!-- content-wrapper ends -->
		</div>
		<!-- page-body-wrapper ends -->
	</div>
	<!-- container-scroller -->
	<!-- plugins:js -->
	<script src="node_modules/jquery/dist/jquery.min.js"></script>
	<script src="node_modules/popper.js/dist/umd/popper.min.js"></script>
	<script src="node_modules/bootstrap/dist/js/bootstrap.min.js"></script>
	
	<script src="js/jquery.ajax.js"></script>
	<script src="js/global.functions.js"></script>
	<script src="js/jquery.users.js" type="application/javascript"></script>
	<!-- endinject -->
	<!-- inject:js -->
	<script src="js/off-canvas.js"></script>
	<script src="js/misc.js"></script>
	<!-- endinject -->
</body>
</html>
