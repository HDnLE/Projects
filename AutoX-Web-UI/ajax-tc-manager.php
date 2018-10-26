<?php
	error_reporting( E_ALL );
	ini_set('display_errors', 1);
	$database = (isset($_POST['nDB'])) ? $_POST['nDB'] : null;
	$option = $_POST['nOption'];
	$ts_id = $_POST['nTS'];
	$tc_id = (isset($_POST['nTC'])) ? $_POST['nTC'] : null;
	$response = false;
	$_SESSION['database2'] = (isset($_POST['nDB'])) ? $_POST['nDB'] : null;
	//include "functions-tool.php";
	if ($database == null)
	{
		echo "ERROR: No database has been selected!";
	}
	else
	{
		if (empty($ts_id))
		{
			echo "ERROR: No testsuite has been entered!";
		}
		else
		{
			if (($ts_id > 9999) || ($ts_id < 1000))
			{
				echo "ERROR: Testsuite ID must be from 1000 - 9999";
			}
			else
			{
				if (!is_testsuite_exist($ts_id))
				{
					echo "ERROR: Unable to find testsuite!";
				}
				else
				{
					if (($option >= 2) && (empty($tc_id)))
					{
						echo "ERROR: No testcase has been entered!";
					}
					elseif (($option >= 2) && (!empty($tc_id)))
					{
						if (($tc_id > 999) || ($tc_id < 100))
						{
							echo "ERROR: Testcase ID must be from 100 - 999";
						}
						else
						{
							if (!is_entry_valid($tc_id))
							{
								echo "ERROR: Invalid data entered on Testcase ID field!";
							}
							else
							{
								$arr_list = get_valid_testcases($tc_id);
				
								foreach ($arr_list as $list)
								{
									$response = true;
									if (!is_tcnum_exist($list, $ts_id))
									{
										echo "ERROR: Some testcases entered are not found!";
										$response = false;
										break;
									}
								}
								
							}
						}
					}
					else
					{
						$response = true;
					}
				}
			}
		}
	}
	if ($response)
	{
		$conWhereClause = "";
		$valActive = (($option == 0) || ($option == 2)) ? 0:1;
		$total_testcases = get_number_of_testcases($ts_id);
		mysqli_query($systemx_connect, "UPDATE testcases SET include_ms=$valActive WHERE ts_id=$ts_id") or die(mysqli_error());
		if ($option >= 2)
		{
			$conWhereClause = " AND (";
			$i = 1;
			$arr_list = get_valid_testcases($tc_id);
			$total_testcases = count($arr_list);
			foreach ($arr_list as $testcase)
			{
				$conWhereClause = ($i == 1) ? "$conWhereClause tc_id=$testcase" : "$conWhereClause OR tc_id=$testcase";
				$i++;
			}
			$valActive = ($option == 2) ? 1 : 0;
			$conWhereClause = $conWhereClause . ")";
			$query = "UPDATE testcases SET include_ms=$valActive WHERE ts_id=$ts_id $conWhereClause";
			mysqli_query($systemx_connect, $query) or die(mysqli_error());
		}
		$msg = ($valActive == 1) ? "Activating" : "Deactivating";
		echo "$msg $total_testcases testcases from TS-ID: $ts_id... Success";
	}
?>