<html>
	<head>
		<title>System XAuto-Test: Testcase DeActivator</title>
		<link rel="shortcut icon" href="images/hovemedical.ico" />
		<script src="https://code.jquery.com/jquery-3.2.1.min.js"></script>
		<script src="js/jquery.ajax.js"></script>
		<link rel="stylesheet" href="css/bootstrap.min.css">
		<link href='https://fonts.googleapis.com/css?family=Syncopate' rel='stylesheet'>
		<link rel="stylesheet" href="css/tc-manager.css">
		<script>
			jQuery(function($)
			{
				$(document).ready(function()
				{
					$("input[type='radio']").prop("checked",false);
					$("input[type='text'], input[type='submit']").prop("disabled",true);
					$("input[type='submit']").addClass("disabled");
					$(document).on( "change", ":radio", function(event)
					{
						$(".span2").prop("disabled", false);
						$(".span1").prop("disabled", false);
						var labelId = "#radio" + $(this).val();
						$("label").removeClass("bold");
						$(labelId).addClass("bold");
						$(".span1").select();
						$("input[type='submit']").removeClass("disabled");
						if ($(this).val() <=1)
						{
							$(".span2").prop("disabled", true);
						}
					});
					
					$(document).on( "click", "[type='submit']", function(event)
					{
						serialize_form = $("#frmTestcase").serialize();
						$(".results").hide().html("<p id='infoMsg'>Please wait...</p>").fadeToggle("slow", function() {
							ajax_manager(serialize_form);
						});
						
					});	
					
					$(document).on( "submit", "form", function(event)
					{
						return false;
					});	
				});
			});
		</script>
	</head>
	<body>
		
		<?php
			//Display all errors
			session_start();
			include "users-functions.php";
			error_reporting( E_ALL );
			ini_set('display_errors', 1);
			
			//session_start();
			//$array = array(range(109,111), 113, 150, 156, 195, 218, 220, 221, range(223,227), 256, 257, 261, 268, 277, 280, 316, 317, 322, range(327,329), 335, 384, 408, 451, 460, 491, 590, 617, 631, 661, 673, 690, 692, 725, 743, 759, range(817,822), 824, 840, 846, 896);
			$designer = $_SESSION["username"];
			$disabled = "";
			$warning_msg = "";
			if (!is_admin($designer))
			{
				$disabled = "disabled";
				$warning_msg = "<p class='error' style='display:inline'>Sorry, this page is for administrator only!</p>";
			}
		?>
		<form method="POST" id="frmTestcase" class="form-wrapper">
		<div class="wrapper"><h4><i class="glyphicon glyphicon-wrench"></i> TESTCASE MANAGER</h4>
			<div class="inner-wrapper">
				<div class="content" style="boder:1px solid #000" >
					<div class="formcontrol">
						<select name='nDB' <?php echo $disabled; ?>>
							<option selected disabled>Select database version</option>
							<option value="sxtest_41">System X 4.1</option>
							<option value="sxtest_51">System X 5.1</option>
							<option value="sxtest_52">System X 5.2</option>
						</select>
					</div>
					<div class="formcontrol">
						<input type="radio" name="nOption" value=0 <?php echo $disabled; ?>>
						<label id="radio0">All testcases in a given testsuite will be deactivated </label>
					</div>
					<div class="formcontrol">
						<input type="radio" name="nOption" value=1 <?php echo $disabled; ?>> 
						<label id="radio1">All testcases in a given testsuite will be activated</label>
					</div>
					<div class="formcontrol">
						<input type="radio" name="nOption" value=2 <?php echo $disabled; ?>> 
						<label id="radio2">Entered testcases will be activated</label>
					</div>
					<div class="formcontrol">
						<input type="radio" name="nOption" value=3 <?php echo $disabled; ?>> 
						<label id="radio3">Entered testcases will be deactivated</label>
					</div>
				</div>
				<div class="content">
					<div class="formcontrol">
						<label>Enter Testsuite ID</label><br>
						<input type="text" class="span1" name="nTS">
					</div>
					<div class="formcontrol">
						<label>Enter Testcase ID</label><br>
						<input type="text" class="span2" name="nTC">
					</div>
					<div class="formcontrol">
						<input type="submit" class="span1 bold" value="Submit">
					</div>
				</div>
			</div>
			<div class="results">
				&nbsp;<?php echo $warning_msg; ?>
			</div>
		</div>
		</form>
		<div class="footer">Testcase Manager (<?php echo date("Y"); ?>) | Developed and maintained by: <a href="mailto:rommel@systemx.no">Rommel A. Lamanilao</a></div>
	</body>

</html>