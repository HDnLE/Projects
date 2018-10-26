<?php
	$page = "all.php";
	$header_title = "Testcaser (Alle)";
	$data_category = "testcase_all";
	$show_ts_combo = true;
	$show_tc_navigator = false;
	$float = ";float:left";
	$ts_id = "";
	if (isset($_GET['ts-id']))
	{
		$page = "single.php";
		$data_category = "testcase_single";
		$ts_id = $_GET['ts-id'];
		$ts_title = (empty(get_testsuite_info($ts_id, 2))) ? "<i style='color:Firebrick'>TS-$ts_id: Testsuite eksisterer ikke!</i>" : "TS-$ts_id: " . get_testsuite_info($ts_id, 2);
		$header_title = "Testcaser ($ts_title)";
		if (isset($_GET['tc-id']))
		{
			$tc_id = $_GET['tc-id'];
			$page = "teststeps.php";
			$teststep_count = get_number_of_teststeps($ts_id, $tc_id);
			if ($teststep_count == 0)
			{
				$page = "teststeps_2.php";
			}
			$data_category = "teststeps";
			//$tc_title = (empty(get_testcase_info(null, 3, $tc_id, $ts_id))) ? "<b style='color:#DF0101'>???</b>" : get_testcase_info(null, 3, $tc_id, $ts_id);
			$tc_title = (is_tcnum_exist($tc_id, $ts_id)) ? "TC-$tc_id: " . get_testcase_info(null, 3, $tc_id, $ts_id) : "<i style='color:Firebrick'>TC-$tc_id: Testcase eksisterer ikke!</i>";
			$header_title = "Teststeg ($tc_title)";
			$show_ts_combo = false;
			$show_tc_navigator = true;
			//$float = ";float:left";
		}
	}
	echo "<div><h4 class='headliner' id='category' data-category='$data_category'>$header_title </h4></div>";
	/*echo "<div style='float:left'>
				<input type='text' placeholder='Søk testcaser...' id='txtSearch'>
				<div id='searchResult'></div>
				</div>
			";*/
	if ($show_ts_combo)
	{
		$checked1 = "checked";
		$checked2 = "";
		$hidden1 = "";
		$hidden2 = "hidden";
		
		if (isset($_GET['view-by']))
		{
			if ($_GET['view-by'] == "id")
			{
				$checked2 = "checked";
				$checked1 = "";
				$hidden1 = "hidden";
				$hidden2 = "";
			}
		}
		echo "<div style='float:left;margin-left:20px'><a href='?id=1' class='text-13'><span class='mdi mdi-backburger icon-xmd'></span> Tilbake til hovedliste</a></div>";
		echo "<div style='float:right'>
				<label style='margin:5px'>Vis testsuite etter: </label>
				<input type='radio' name='rbViewType' id='rbName' value='name' $checked1>
				<label class='pointer text-primary' for='rbName'>navn </label>
				<input type='radio' name='rbViewType' id='rbID' value='id' $checked2>
				<label class='pointer text-primary' for='rbID'> ID </label>
				<input type='text' value='" . $ts_id . "' class='$hidden2 ts-input center pointer' style='width:50px;margin-left:5px;font-size:16px'>
				<select style='margin-left:5px' id='cboTestsuiteList' class='ts-list pointer $hidden1' title='Vis andre testsuite'>
				
			";
		foreach (get_all_test_suites() as &$testsuite) 
		{
			$selected = (split(":", $testsuite)[0] == $ts_id) ? "selected" : "";
			echo "<option $selected title='TS-" . split(":", $testsuite)[0] . ": " . split(":", $testsuite)[1] . "' value=" . split(":", $testsuite)[0] . ">" . split(":", $testsuite)[1] . "</option>";
		}echo "
				</select>
				</div><br>
			";
	}
	if ($show_tc_navigator)
	{
		$disabled_prev = ($tc_id == 100) ? "text-muted" : "";
		$last_testcase = get_latest_testcase($ts_id);
		$last_testsuite = get_latest_testsuite();
		$disabled_next = ($tc_id == $last_testcase) ? "text-muted" : "";
		echo "<div style='width:25%;float:right;text-align:right;padding-right:20px'>
				<button type='button' title='Vis en testcase backover...' class='btn btn-inverse-light icon-btn $disabled_prev tc-navigator' data-direction='previous' style='padding:2px'><span class='mdi mdi-skip-previous mdi-24px'></span></button>
				TS-<input type='text' class='btn btn-inverse-light center' style='width:35px;height:30px;padding:0px;margin-left:-3px' value='$ts_id' id='txtTsNum' data-last-testsuite='$last_testsuite'>
				TC-<input type='text' class='btn btn-inverse-light center' style='width:35px;height:30px;padding:0px;margin-left:-3px' value='$tc_id' id='txtTcNum' data-last-testcase='$last_testcase'>
				<button type='button' title='Vis en testcase fremover...' class='btn btn-inverse-light icon-btn tc-navigator $disabled_next' data-direction='next' style='padding:2px;margin-left:-3px'><span class='mdi mdi-skip-next mdi-24px'></span></button>
			</div><br>";
	}
	include $page;
?>
