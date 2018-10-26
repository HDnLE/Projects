<!DOCTYPE html>
<html lang="en">
<head>
	<meta charset="utf-8">
	<meta name="viewport" content="width=device-width, initial-scale=1">
	<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
	<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
	<style>
		.nav-tabs>li.active>a,.nav-tabs>li.active>a:focus,.nav-tabs>li.active>a:hover {
			color:#555;
			cursor:pointer;
			background-color:#fff;
			border:1px solid #ddd;
			border-bottom-color:transparent;
			padding:10px;
			text-decoration: none;
		}
		.nav-tabs {
			border-bottom:1px solid #ddd
		}
		.nav-tabs>li {
			float:left;
			margin-bottom:-1px;
			padding:10px
		}
		.nav-tabs>li>a {
			margin-right:2px;
			line-height:1.42857143;
			border:1px solid transparent;
			border-radius:4px 4px 0 0;
			padding:10px;
			background-color:#A9E2F3;
		}
		.nav-tabs>li>a:hover {
			border-color:#eee #eee #ddd;
			text-decoration: none;
			padding:10px;
		}
		.menu-tab-container {
			padding:20px;
		}
		.tab-container {
			margin-top:10px;
			border:1px solid #DDD;
			box-shadow: 3px 5px 5px #ccc;;
			border-radius:4px 4px 0 0;
			padding:10px;
		}
	</style>
</head>
<body>
	<h4 class="headliner" id="category" data-category='tasks'>Testcase Manager</h4>
	<div class="card-body">
		<div class="card">
			<div class="menu-tab-container">
				<ul class="nav nav-tabs">
					<li class="active"><a data-toggle="tab-menu" href="#tab1">Change Order of Testcases</a></li>
					<li><a data-toggle="tab-menu" href="#tab2">Activate/Deactivate Testcases</a></li>
				</ul>
				<div class="tabs">
					<div id="tab1" class="tab-pane in active tab-container">
						<form class="forms-sample" action="" method='POST' id="frmTab1">
							<div class="form-group row">
								<label for="tsID" class="col-sm-1 col-form-label">*TS-ID</label>
								<div class="col-sm-9">
									<input type="text" required class="form-control text-14" id="tsID" style="width:80px" name="nTSID">
								</div>
							</div>
							<div class="form-group row">
								<label for="tcIDStartOld" class="col-sm-1 col-form-label">*Starting TC-ID (Old)</label>
								<div class="col-sm-9">
									<input type="text" required class="form-control text-14" id="tcIDStartOld" style="width:80px" name="nStartTCIDOld">
								</div>
							</div>
							<div class="form-group row">
								<label for="tcIDEndOld" class="col-sm-1 col-form-label">*Ending TC-ID (Old)</label>
								<div class="col-sm-9">
									<input type="text" required class="form-control text-14" id="tcIDEndOld" style="width:80px" name="nEndTCIDOld">
								</div>
							</div>
							<div class="form-group row">
								<label for="tcIDStartNew" class="col-sm-1 col-form-label">*Starting TC-ID (New)</label>
								<div class="col-sm-9">
									<input type="text" required class="form-control text-14" id="tcIDStartNew" style="width:80px" name="nStartTCIDNew">
								</div>
							</div>
							<div class="form-group row" style="margin-left:1px">
								<button type="button" class="btn btn-primary" id="btnTab1">Submit</button>
								<button type="reset" class="btn " style="margin-left:10px">Reset</button>
							</div>
						</form>
						
					</div>
					<div id="tab2" class="tab-pane  in hidden tab-container">
						<form class="forms-sample" action="" method='POST' id="frmTab2">
							<div class="inner-wrapper">
								<div class="content" style="border-right:1px solid #DDD">
									<div class="formcontrol">
										<input type="radio" name="nOption" id="radio0" value="1" data-action="Deactivate">
										<label for="radio0" class="pointer">All testcases in a given testsuite will be deactivated </label>
									</div>
									<div class="formcontrol">
										<input type="radio" name="nOption" id="radio1" value="2" data-action="Activate"> 
										<label for="radio1" class="pointer">All testcases in a given testsuite will be activated</label>
									</div>
									<div class="formcontrol">
										<input type="radio" name="nOption" id="radio2" value="3" data-action="Activate"> 
										<label for="radio2" class="pointer">Entered testcases will be activated</label>
									</div>
									<div class="formcontrol">
										<input type="radio" name="nOption" id="radio3" value="4" data-action="Deactivate"> 
										<label for="radio3" class="pointer">Entered testcases will be deactivated</label>
									</div>
								</div>
								<div class="content" style="padding-left:50px">
									<div class="formcontrol">
										<label>*Enter Testsuite ID</label><br>
										<input type="text" class="form-control center text-14 input-80" required name="nTS" id="txtTSID">
									</div>
									<div class="formcontrol">
										<label id="lblTC">Enter Testcase ID</label><br>
										<input type="text" class="form-control text-14" disabled name="nTC" required id="txtTCID">
									</div>
									<div class="formcontrol">
										<br>
										<button type="button" class="btn btn-primary" id="btnTab2" disabled>Activate/Deactivate</button>
									</div>
								</div>
							</div>
						</form>
					</div>
					<div id="info"></div>
				</div>
			</div>
		</div>
	</div>
</body>
</html>

