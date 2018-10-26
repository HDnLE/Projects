<div class="card-body">
	<?php
		$ts_id = $_GET['ts-id'];
		$tc_id = $_GET['tc-id'];
		if (is_tcnum_exist($tc_id, $ts_id))
		{
			$teststepCount = get_number_of_teststeps($ts_id, $tc_id);
			if ($teststepCount == 0)
			{
				$new_step_num = get_new_teststep_num($ts_id, $tc_id);
				echo "
					<div class='alert alert-warning center'>
						ADVARSEL: Det er ingen teststeg  ennå for denne testcasen!
						<span class='modalClass pointer text-primary' data-step-number='$new_step_num' title='Registrere ny teststeg' data-icon='new-box' data-toggle='modal' data-target='#modalStepRegForm' id='mnuBtnNew'>Registrere ny nå...</span>
					</div>";
			}
			else include "teststeps.php";
		}
		else
		{
			echo "
				<div class='alert alert-danger center'>
					FEIL: Testcase eksisterer ikke!
				</div>";
		}
	?>
	
</div>

<?php
	include "modals.php";
?>