<?php
	$page = "parents.php";
	$header_title = "Parent objekter";
	$data_category = "parent_objects";
	$showBackToMainLink = false;
	$show_tc_navigator = false;
	$float = ";float:left";
	$backToMainLink = "";
	if (isset($_GET['parent-id']))
	{
		$page = "children.php";
		$data_category = "child_objects";
		$parent_id = $_GET['parent-id'];
		$parent_name = get_parent_info($parent_id, "PARENT_NAME");
		$header_title = "Child objekter ($parent_name)";
		$showBackToMainLink = true;
		/*if (isset($_GET['tc-id']))
		{
			$tc_id = $_GET['tc-id'];
			$page = "teststeps.php";
			$data_category = "teststeps";
			$tc_title = (empty(get_testcase_info(null, 3, $tc_id, $ts_id))) ? "<b style='color:#DF0101'>???</b>" : get_testcase_info(null, 3, $tc_id, $ts_id);
			$header_title = "Test steg (TC-$tc_id: $tc_title)";
			$show_parent_combo = false;
			$show_tc_navigator = true;
			//$float = ";float:left";
		}*/
	}
	echo "
		<div style='width:100%;float:left'>
			<h4 class='headliner' id='category' data-category='$data_category'>
				$header_title 
				<div style='float:right;margin-top:-3px'>
					<input type='text' id='txtSearchObject' placeholder='Søk objekter' class='input-search'>
					<div id='searchObjResult' style='width:300px'></div>
				</div>
			</h4>
		</div>
		";
	if ($showBackToMainLink)
	{
		$backToMainLink = "<a href='?id=4'><span class='mdi mdi-backburger icon-xmd'></span> Tilbake til parent objekter</a>";
	}
	echo "<br><br>$backToMainLink<div style='width:25%;float:right'>
			<select class='object-list pointer' >
		";
	foreach (get_all_parent_objects() as &$object) 
	{
		$selected = (split(":", $object)[0] == $parent_id) ? "selected" : "";
		echo "<option $selected value=" . split(":", $object)[0] . ">" . split(":", $object)[1] . "</option>";
	}echo "
			</select>
			</div><br>
		";
	if ($show_tc_navigator)
	{
		$disabled_prev = ($tc_id == 100) ? "text-muted" : "";
		$last_testcase = get_latest_testcase($ts_id);
		$last_testsuite = get_latest_testsuite();
		$disabled_next = ($tc_id == $last_testcase) ? "text-muted" : "";
		echo "<div style='width:50%;float:right;text-align:right;padding-right:20px'>
				<button type='button' title='Vis en testcase backover...' class='btn btn-inverse-light icon-btn $disabled_prev tc-navigator' data-direction='previous' style='padding:2px'><span class='mdi mdi-skip-previous mdi-24px'></span></button>
				TS-<input type='text' class='btn btn-inverse-light center' style='width:35px;height:30px;padding:0px;margin-left:-3px' value='$ts_id' id='txtTsNum' data-last-testsuite='$last_testsuite'>
				TC-<input type='text' class='btn btn-inverse-light center' style='width:35px;height:30px;padding:0px;margin-left:-3px' value='$tc_id' id='txtTcNum' data-last-testcase='$last_testcase'>
				<button type='button' title='Vis en testcase fremover...' class='btn btn-inverse-light icon-btn tc-navigator $disabled_next' data-direction='next' style='padding:2px;margin-left:-3px'><span class='mdi mdi-skip-next mdi-24px'></span></button>
			</div><br>";
	}
	include $page;
?>
