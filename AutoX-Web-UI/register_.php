<!DOCTYPE html>
<html lang="en">

<head>
  <!-- Required meta tags -->
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
  <title>AutoX Web UI | Registrering av ny bruker</title>
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
</head>

<body>
	<div class="container-scroller">
		<div class="container-fluid page-body-wrapper full-page-wrapper">
			<div class="content-wrapper d-flex align-items-center auth login-full-bg">
				<div class="row w-100">
					<div class="col-lg-4 mx-auto">
						<div class="auth-form-light text-left p-5">
							<h2>Brukerregistrering</h2>
							<h4 class="font-weight-light">Registrer ny bruker</h4>
							<form class="pt-4" action="" method='POST' id="frm_create_user">
								<form>
									<div class="form-group">
										<label for="etternavn">Etternavn</label>
										<input type="text" class="form-control" id="etternavn" placeholder="Etternavn" name="n_last" required>
										<i class="mdi mdi-account"></i>
									</div>	
									<div class="form-group">
										<label for="fornavn">Fornavn</label>
										<input type="text" class="form-control" id="fornavn" placeholder="Fornavn" name="n_first" required>
										<i class="mdi mdi-account"></i>
									</div>
									<div class="form-group">
										<label for="brukernavn">Brukernavn</label>
										<input type="text" class="form-control" id="brukernavn" placeholder="Brukernavn" name="n_username" required>
										<i class="mdi mdi-account"></i>
									</div>
									<div class="form-group">
										<label for="exampleInputPassword1">Password</label>
										<input type="password" class="form-control" id="exampleInputPassword1" placeholder="Password" name="n_password" required>
										<i class="mdi mdi-lock-outline"></i>
									</div>
									<div class="form-group">
										<label for="exampleInputPassword2">Password</label>
										<input type="password" class="form-control" id="exampleInputPassword2" placeholder="Bekfrete passord" name="n_confirm_password" required>
										<i class="mdi mdi-lock-outline"></i>
									</div>
									<div class="form-group">
										<label for="role">Velg rolle</label>
										<select class="form-control" name="n_role" id="role" required>
											<option selected disabled></option>
											<option>Prosjektleder</option>
											<option>Testutvikler</option>
											<option>Testleder</option>
											<option>Tester</option>
											<option>Systemutvikler</option>
										</select>
									</div>
									<div class="mt-5">
										<a class="btn btn-block btn-primary btn-lg font-weight-medium" id="btn_create_user">Registrer</a>
									</div>
									<div class="mt-2 text-center">
										<a href="login.php" class="auth-link text-black">Har du allerede en konto? <span class="font-weight-medium">Logg inn</span></a>
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
	<!-- endinject -->
	<!-- inject:js -->
	<script src="js/off-canvas.js"></script>
	<script src="js/misc.js"></script>
	<!-- endinject -->
	<script src="js/jquery.ajax.js"></script>
	<script src="js/global.functions.js"></script>
	<script src="js/jquery.users.js" type="application/javascript"></script>
</body>

</html>
