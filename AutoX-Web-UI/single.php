<div class="card-body">
	<?php
		$ts_id = $_GET['ts-id'];
		if (is_testsuite_exist($ts_id))
		{
			$testcase_count = get_testcase_count_per_testsuite($ts_id);
			if ($testcase_count == 0)
			{
				echo "
					<div class='alert alert-warning center'>
						ADVARSEL: Det er ingen testcaser ennå for denne testsuiten!
						<span class='modalClass pointer text-primary' data-icon='new-box' data-toggle='modal' data-target='#modalTcRegForm' title='Registrere ny testcase' id='mnuBtnNew'>Registrere ny nå...</span>
					</div>";
			}
			else include "testcases.php";
		}
		else
		{
			echo "
				<div class='alert alert-danger center'>
					FEIL: Testsuite eksisterer ikke!
				</div>";
		}
	?>
	
</div>

<?php
	include "modals.php";
?>