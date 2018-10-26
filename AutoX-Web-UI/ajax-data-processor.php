<?php
	session_start();
	$action = $_POST["action"];
	include "functions.php";
	date_default_timezone_set('Europe/Oslo');
	include "dbconn-systemx.php";
	include "dbconn-users.php";
	$info = "";
	$sql = "";
	$activity_log_date = date("Y-m-d H:m:s");
	$date = date("Y-m-d H:i:s");
	$designer = $_SESSION["username"];
	switch($action)
	{
		//******************* TESTSUITES *******************//
		//Save changes made on the testsuite
		//Save a new testsuite
		case "reg_testsuite":
			$title = $_POST["n_ts_title"];
			$desc = $_POST["n_ts_desc"];
			$testGroup = $_POST["n_testgroup"];
			$id = get_new_testsuite_id();
			$date = date("Y-m-d H:i:s");
			$sql = "INSERT INTO testsuites (TS_ID, TITLE, DESCRIPTION, DESIGNER, TEST_GROUP, DATE_LAST_MODIFIED) VALUES ('$id', '$title', '$desc', '$designer', '$testGroup', '$date')";
			$info = "<b>Vellykket:</b> Lagrer ny testsuite...";
			$log = "Created a new testsuite (TS-$id)";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "edit_testsuite":
			$title = $_POST["n_ts_title"];
			$desc = $_POST["n_ts_desc"];
			$id =  $_POST["n_ts_id"];
			$testGroup = $_POST["n_testgroup"];
			$status = 0;
			$date = date("Y-m-d H:i:s");
			//if ($write_status == "on") {$status = 1;}
			$sql =  "UPDATE testsuites SET TITLE='$title', DESIGNER='$designer', DESCRIPTION='$desc', TEST_GROUP='$testGroup', DATE_LAST_MODIFIED='$date' WHERE TS_ID=$id";
			$info = "<b>Vellykket:</b> Lagrer endringer gjort på testsuite...";
			
			$log = "Modified a testsuite (TS-$id)";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "ts_delete":
			$id =  $_POST["id"];
			$sql =  "DELETE FROM testsuites WHERE ID=$id";
			$info = "<b>Vellykket:</b> Sletter utvalgte elementer...";
			$ts_id = get_testsuite_info_by_id($id,1);
			mysqli_query($systemx_connect, "DELETE FROM testcases WHERE TS_ID=$id") or die(mysqli_error());
			mysqli_query($systemx_connect, "DELETE FROM teststeps WHERE TS_ID=$id") or die(mysqli_error());
			$log = "Deleted a testsuite (TS-$id)";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "copy_testsuite":
			$idx =  $_POST["id"];
			$id = get_testsuite_info_by_id($idx, 1);
			$ts_id = get_new_testsuite_id();
			$include_testcase = $_POST["include-tc"];
			//If user wants to copy a testsuite and its testcases
			if ($include_testcase == "on")
			{
				mysqli_query($systemx_connect, "INSERT INTO testcases (TC_ID, TS_ID, TC_TITLE, DESIGNER, DATE_LAST_MODIFIED, MODIFIER, ACTIVE) SELECT TC_ID, $ts_id, TC_TITLE, '$designer', DATE_LAST_MODIFIED, MODIFIER, ACTIVE FROM testcases WHERE ts_id = $id");
				mysqli_query($systemx_connect, "INSERT INTO teststeps (TC_ID, TS_ID, STEP_NUMBER, STEP_CATEGORY, EXE_STEP, DESCRIPTION, TEST_DATA, EXPECTED_RESULT) SELECT TC_ID, $ts_id, STEP_NUMBER, STEP_CATEGORY, EXE_STEP, DESCRIPTION, TEST_DATA, EXPECTED_RESULT FROM teststeps WHERE ts_id = $id");
			}
			$sql =  "INSERT INTO testsuites (TS_ID, TITLE, DESCRIPTION, DESIGNER, TEST_GROUP) SELECT $ts_id, CONCAT(TITLE, ' - Kopi'), DESCRIPTION, '$designer' FROM testsuites WHERE TS_ID = $id";
			$info = "<b>Vellykket:</b> Kopier valgte testsuiter...";
			$log = "Copied a testsuite (TS-$id)";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "reg_testcase":
			$ts_id = $_POST["n_ts_id"];
			$tc_num = get_new_testcase_num($ts_id);
			$tc_title = $_POST["n_tc_title"];
			$date = date("Y-m-d H:i:s");
			$comments = $_POST["n_comments"];
			$ticket = $_POST["n_Jira"];
			//If the user wants to include/add the testcase to the masterscript list 
			
			mysqli_query($systemx_connect, "INSERT INTO teststeps (TC_ID, TS_ID, STEP_NUMBER, STEP_CATEGORY, EXE_STEP) VALUES ('$tc_num', '$ts_id', 0, 'Title', '$tc_title')");
			$sql =  "UPDATE testsuites SET TITLE='$title', DESIGNER='$designer', DESCRIPTION='$desc', TEST_GROUP='$testGroup', DATE_LAST_MODIFIED='$date' WHERE TS_ID=$id";
			mysqli_query($systemx_connect, "UPDATE testsuites SET DATE_LAST_MODIFIED='$date' WHERE TS_ID='$ts_id'");
			$info = "<b>Vellykket:</b> Lagrer ny testcase...";
			
			$log = "Created a new testcase (TC-$tc_num) on TS-$ts_id";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "edit_testcase":
			$id =  $_POST["n_rec_id"];
			$ts_id = $_POST["n_ts_id"];
			$tc_title = $_POST["n_tc_title"];
			$date = date("Y-m-d H:i:s");
			$comments = $_POST["n_comments"];
			$ts_id_orig = get_testcase_info_by_id($id, 2);
			$tc_id = $_POST["n_tc_id"];
			$ticket = $_POST["n_Jira"];
			if ($ts_id_orig != $ts_id)
			{
				//If the testcase ID has already exist on the updated testsuite, get a new testcase ID 
				$tc_id = get_new_testcase_num($ts_id);
			}
			$sql =  "UPDATE testcases SET COMMENTS='$comments', TS_ID='$ts_id', MODIFIER='$designer', DATE_LAST_MODIFIED='$date', TC_TITLE='$tc_title', TC_ID=$tc_id, JIRA_TICKET='$ticket' WHERE ID=$id";
			mysqli_query($systemx_connect, "UPDATE teststeps SET EXE_STEP='$tc_title' WHERE TS_ID=$ts_id AND TC_ID=$tc_id AND STEP_CATEGORY='Title'");
			mysqli_query($systemx_connect, "UPDATE testsuites SET DATE_LAST_MODIFIED='$date' WHERE TS_ID='$ts_id'");
			$info = "<b>Vellykket:</b> Lagrer endringer gjort på testcase...";
			$log = "Modified a testcase (TC-$tc_id) on TS-$ts_id";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "move_testcases":
			$id =  $_POST["id"];
			$new_ts_id = $_POST["n_ts_id"];
			$comments = $_POST["n_comments"];
			$new_tc_id = get_new_testcase_num($new_ts_id);
			$ts_id = get_testcase_info_by_id($id, 2);
			$tc_id = get_testcase_info_by_id($id, 1);
			$date = date("Y-m-d H:i:s");
			$sql =  "UPDATE testcases SET TS_ID='$new_ts_id', MODIFIER='$designer', DATE_LAST_MODIFIED='$activity_log_date', TC_ID=$new_tc_id WHERE ID=$id";
			mysqli_query($systemx_connect, "UPDATE teststeps SET TS_ID='$new_ts_id', TC_ID=$new_tc_id WHERE TS_ID=$ts_id AND TC_ID=$tc_id");
			mysqli_query($systemx_connect, "UPDATE testsuites SET DATE_LAST_MODIFIED='$date' WHERE TS_ID='$ts_id'");
			$info = "<b>Vellykket:</b> Lagrer endringer gjort på testcase...";
			$log = "Moved a testcase (TC-$tc_id) from TS-$ts_id to TS-$new_ts_id";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "activate_testcase":
			$id =  $_POST["id"];
			$activate =  $_POST["n_activate"];
			$activate_text = ($activate == 1) ? "Activated" : "Deactivated";
			$activate_msg = ($activate == 1) ? "aktivert" : "deaktivert";
			$comments = $_POST["comments"];
			$ticket = $_POST["n_Jira"];
			$date = date("Y-m-d H:i:s");
			$sql =  "UPDATE testcases SET ACTIVE=$activate, COMMENTS='$comments', JIRA_TICKET='$ticket', RUN_STATUS=NULL WHERE ID=$id";
			$info = "<b>Vellykket:</b> Testcase er nå $activate_msg...";
			$tc_id = get_testcase_info_by_id($id, 1);
			$ts_id = get_testcase_info_by_id($id, 2);
			$log = "$activate_text a testcase (TC-$tc_id) on TS-$ts_id";
			mysqli_query($systemx_connect, "UPDATE testsuites SET DATE_LAST_MODIFIED='$date' WHERE TS_ID='$ts_id'");
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "toggle_activate_testcase":
			$ts_id = $_POST["ts-id"];
			$tc_id = $_POST["tc-id"];
			$comments = $_POST["comments"];
			$activate =  $_POST["activate"];
			$date = date("Y-m-d H:i:s");
			$sql =  "UPDATE testcases SET ACTIVE=$activate, COMMENTS='$comments', RUN_STATUS=NULL WHERE TS_ID=$ts_id AND TC_ID=$tc_id";
			mysqli_query($systemx_connect, "UPDATE testsuites SET DATE_LAST_MODIFIED='$date' WHERE TS_ID='$ts_id'");
			break;
		case "tc_delete":
			$id =  $_POST["id"];
			$sql =  "DELETE FROM testcases WHERE id=$id";
			$info = "<b>Vellykket:</b> Sletter valgte elementer...";
			$ts_id = get_testcase_info_by_id($id, 2);
			$tc_id = get_testcase_info_by_id($id, 1);
			mysqli_query($systemx_connect, "DELETE FROM teststeps WHERE TS_ID=$ts_id AND TC_ID=$tc_id") or die(mysqli_error());
			$log = "Deleted a testcase (TC-$tc_id) on TS-$ts_id";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			mysqli_query($systemx_connect, "UPDATE testsuites SET DATE_LAST_MODIFIED='$date' WHERE TS_ID='$ts_id'");
			break;
		case "copy_testcase":
			$target_testsuite_id = $_POST["n_ts_id"];
			//update_testcase_number($target_testsuite_id);
			$new_tc_id = get_new_testcase_num($target_testsuite_id);
			$id = $_POST["id"];
			$ts_id = get_testcase_info_by_id($id, 2);
			$tc_id = get_testcase_info_by_id($id, 1);
			mysqli_query($systemx_connect, "INSERT INTO teststeps (TC_ID, TS_ID, STEP_NUMBER, STEP_CATEGORY, EXE_STEP, DESCRIPTION, TEST_DATA, EXPECTED_RESULT) SELECT $new_tc_id, $target_testsuite_id, STEP_NUMBER, STEP_CATEGORY, EXE_STEP, DESCRIPTION, TEST_DATA, EXPECTED_RESULT FROM teststeps WHERE TS_ID = $ts_id AND TC_ID=$tc_id") or die(mysqli_error());
			$title = get_testcase_info(null, 3, $tc_id, $ts_id);
			mysqli_query($systemx_connect, "UPDATE teststeps SET EXE_STEP='$title' WHERE TS_ID=$target_testsuite_id AND TC_ID=$new_tc_id AND STEP_CATEGORY='Title'");
			$sql =  "INSERT INTO testcases (TC_ID, TS_ID, TC_TITLE, DESIGNER, DATE_LAST_MODIFIED, MODIFIER, ACTIVE, JIRA_TICKET) SELECT $new_tc_id, $target_testsuite_id, CONCAT(TC_TITLE, ' - Kopi'), '$designer', '$activity_log_date', '$designer', ACTIVE, JIRA_TICKET FROM testcases WHERE ID = $id";
			$info = "<b>Vellykket:</b> Kopier valgte testcaser...";
			$log = "Copied a testcase (TC-$tc_id) on TS-$ts_id";
			mysqli_query($systemx_connect, "UPDATE testsuites SET DATE_LAST_MODIFIED='$date' WHERE TS_ID='$ts_id'");
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "get_subcategory":
			$category = $_POST["category"];
			$sub_category = (isset($_POST["sub-category"])) ? $_POST["sub-category"] : null;
			$sql = "SELECT COMMAND_ID, TEST_COMMAND FROM test_commands WHERE COMMAND_CATEGORY = '$category' AND ACTIVE = 1 ORDER BY TEST_COMMAND ASC";
			$result = mysqli_query($systemx_connect,$sql)or die(mysqli_error());
			echo "<select name='n_sub_category' id='cboSubCategory' style='width:445px' class='form-control form-control-sm' required>";
			echo "<option selected disabled></option>";
			while($row = mysqli_fetch_array($result, MYSQL_NUM))
			{
				$selected = ($row[0] == $sub_category) ? "selected": "";
				echo "<option value='$row[0]' $selected>$row[1]</option>";
			}
			echo "</select>";
			break;
		case "get_objects":
			$parent_id = $_POST["parent-id"];
			$sql = "SELECT CHILD_ID, CONTROL_NAME FROM OBJECTS_CHILDREN WHERE PARENT_ID = '$parent_id' ORDER BY CONTROL_NAME ASC";
			$result = mysqli_query($systemx_connect,$sql)or die(mysqli_error());
			echo "<select name='n_objects' id='cboObjects' style='width:445px' class='form-control form-control-sm' required>";
			echo "<option selected disabled></option>";
			while($row = mysqli_fetch_array($result, MYSQL_NUM))
			{
				echo "<option value='$row[0]'>$row[1]</option>";
			}
			echo "</select>";
			break;
		case "reg_teststeps":
			$step_action = $_POST["add-action"];
			$ts_id = $_POST["ts-id"];
			$tc_id = $_POST["tc-id"];
			//$step_num = get_new_teststep_num($ts_id, $tc_id);
			$step_num = $_POST["n_step_num"];
			$category = trim($_POST["n_category"]);
			$desc = $_POST["n_description"];
			$sub_category = $_POST["n_sub_category"];
			$command = get_command_info($sub_category, 3);
			$auto_login = (isset($_POST["n_auto_login"])) ? 1:0;
			$expected_result = null;
			$test_data = (isset($_POST["n_test_command"]) ? trim(mysqli_escape_string($systemx_connect, $_POST["n_test_command"])) : null);
			$arr_test_data = extract_test_data($test_data);
			if ($category == "Checkpoint")
			{
				//$object_id = query_object_by_name($arr_test_data[0], $arr_test_data[1], 6);
				$parent_id = query_parent_object_by_name($arr_test_data[0], 'PARENT_ID');
				$child_id = query_child_object_by_name($parent_id, $arr_test_data[1], "CHILD_ID");
				$test_data = str_replace($arr_test_data[0], $parent_id, $test_data);
				$test_data = str_replace($arr_test_data[1], $child_id, $test_data);
			}
			
			if ($category == "Application")
			{
				$test_data = str_replace($arr_test_data[0], "9999999", $test_data);
				if ($command == "Application_Delay")
				{
					$test_data = $_POST["n_test_command"];
				}
			}
			
			if (($category == "Keyboard") || ($category == "Mouse"))
			{
				$split_test_data = explode(";", $test_data);
				foreach ($split_test_data as &$data)
				{
					$arr_test_data = extract_test_data($data);
					$windowTitle = (isset($arr_test_data[0]) ? $arr_test_data[0] : null);
					$alias = (isset($arr_test_data[1]) ? $arr_test_data[1] : null);
					$test_data = (isset($arr_test_data[0]) ? str_replace($arr_test_data[0], query_parent_object_by_name($arr_test_data[0], "PARENT_ID"), $test_data) : null);
					$test_data = (isset($arr_test_data[1]) ? str_replace($arr_test_data[1], query_child_object_by_name(query_parent_object_by_name($arr_test_data[0], "PARENT_ID"), $arr_test_data[1], "CHILD_ID"), $test_data) : null);
					/*$test_data = (isset($arr_test_data[0]) ? str_replace($arr_test_data[0], get_object_info("CHILD_NAME", $arr_test_data[0], 6, "AND CONTROL_NAME='$alias'"), $test_data) : null);
					$test_data = (isset($arr_test_data[1]) ? str_replace($arr_test_data[1], get_object_info("CONTROL_NAME", $arr_test_data[1], 6, "AND CHILD_NAME='$windowTitle'"), $test_data) : null);*/
				}
			}
			
			//If user inserts a new test step
			if ($step_action == "insertNew")
			{
				$step_num = $_POST["n_step_num"];
				mysqli_query($systemx_connect, "UPDATE teststeps SET STEP_NUMBER=STEP_NUMBER+1 WHERE TC_ID=$tc_id AND STEP_NUMBER>=$step_num AND TS_ID=$ts_id") or die(mysqli_error());
			}
			mysqli_query($systemx_connect, "INSERT INTO teststeps (TC_ID, TS_ID, STEP_NUMBER, STEP_CATEGORY, EXE_STEP, DESCRIPTION, TEST_DATA, EXPECTED_RESULT, AUTO_LOGIN) VALUES ('$tc_id', '$ts_id', '$step_num', '$category', '$sub_category', '$desc', '$test_data', '$expected_result','$auto_login')") or die(mysqli_error());
			$sql = "UPDATE testcases SET DATE_LAST_MODIFIED='$activity_log_date', MODIFIER='$designer' WHERE TC_ID=$tc_id AND TS_ID=$ts_id";
			update_teststep_number($ts_id, $tc_id);
			$log = "Added a new teststep on TC-$tc_id from TS-$ts_id";
			mysqli_query($systemx_connect, "UPDATE testsuites SET DATE_LAST_MODIFIED='$date' WHERE TS_ID='$ts_id'");
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "edit_teststeps":
			$ts_id = $_POST["ts-id"];
			$tc_id = $_POST["tc-id"];
			$step_num = $_POST["n_step_num"];
			$category = trim($_POST["n_category"]);
			$desc = $_POST["n_description"];
			$sub_category = $_POST["n_sub_category"];
			$command = get_command_info($sub_category, 3);
			$auto_login = ($_POST["n_auto_login"] == "on") ? 1:0;
			$expected_result = null;
			$test_data = (isset($_POST["n_test_command"]) ? trim(mysqli_escape_string($systemx_connect, $_POST["n_test_command"])) : null);
			$arr_test_data = extract_test_data($test_data);
			if ($category == "Checkpoint")
			{
				$parent_id = query_parent_object_by_name($arr_test_data[0], 'PARENT_ID');
				$child_id = query_child_object_by_name($parent_id, $arr_test_data[1], "CHILD_ID");
				$test_data = str_replace($arr_test_data[0], $parent_id, $test_data);
				$test_data = str_replace($arr_test_data[1], $child_id, $test_data);
			}
			
			if ($category == "Application")
			{
				$arr_test_data = extract_test_data($test_data);
				$test_data = str_replace($arr_test_data[0], "9999999", $test_data);
				if ($command == "Application_Delay")
				{
					$test_data = $_POST["n_test_command"];
				}
			}
			
			if (($category == "Keyboard") || ($category == "Mouse"))
			{
				$split_test_data = explode(";", $test_data);
				foreach ($split_test_data as &$data)
				{
					$arr_test_data = extract_test_data($data);
					$windowTitle = (isset($arr_test_data[0]) ? $arr_test_data[0] : null);
					$alias = (isset($arr_test_data[1]) ? $arr_test_data[1] : null);
					$test_data = (isset($arr_test_data[0]) ? str_replace($arr_test_data[0], query_parent_object_by_name($arr_test_data[0], "PARENT_ID"), $test_data) : null);
					$test_data = (isset($arr_test_data[1]) ? str_replace($arr_test_data[1], query_child_object_by_name(query_parent_object_by_name($arr_test_data[0], "PARENT_ID"), $arr_test_data[1], "CHILD_ID"), $test_data) : null);
				}
			}
			
			$sql =  "UPDATE teststeps SET DESCRIPTION='$desc', STEP_CATEGORY='$category', EXE_STEP='$sub_category', TEST_DATA='$test_data', EXPECTED_RESULT='$expected_result', AUTO_LOGIN='$auto_login' WHERE ts_id=$ts_id AND tc_id=$tc_id AND step_number=$step_num";
			
			mysqli_query($systemx_connect, "UPDATE testcases SET DATE_LAST_MODIFIED='$activity_log_date', MODIFIER='$designer' WHERE TC_ID=$tc_id AND ts_id=$ts_id") or die(mysqli_error());
			$log = "Modified a teststep on TC-$tc_id from TS-$ts_id";
			mysqli_query($systemx_connect, "UPDATE testsuites SET DATE_LAST_MODIFIED='$date' WHERE TS_ID='$ts_id'");
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "step_delete":
			$id =  $_POST["id"];
			$sql =  "SELECT * FROM teststeps WHERE id=$id";
			$ts_id = get_teststep_info($id, 2);
			$tc_id = get_teststep_info($id, 1);
			mysqli_query($systemx_connect, "UPDATE testcases SET DATE_LAST_MODIFIED='$activity_log_date', MODIFIER='$designer' WHERE TC_ID=$tc_id AND TS_ID=$ts_id") or die(mysqli_error());
			mysqli_query($systemx_connect, "DELETE FROM teststeps WHERE id=$id") or die(mysqli_error());
			update_teststep_number($ts_id, $tc_id);
			$log = "Deleted a teststep on TC-$tc_id from TS-$ts_id";
			mysqli_query($systemx_connect, "UPDATE testsuites SET DATE_LAST_MODIFIED='$date' WHERE TS_ID='$ts_id'");
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "activate_teststep":
			$id =  $_POST["id"];
			$activate =  $_POST["n_activate"];
			$sql =  "UPDATE teststeps SET disable=$activate WHERE id=$id";
			$ts_id = get_teststep_info($id, 2);
			$tc_id = get_teststep_info($id, 1);
			mysqli_query($systemx_connect, "UPDATE testcases SET DATE_LAST_MODIFIED='$activity_log_date', MODIFIER='$designer' WHERE TC_ID=$tc_id AND TS_ID=$ts_id") or die(mysqli_error());
			$log = "Activated a teststep on TC-$tc_id from TS-$ts_id";
			mysqli_query($systemx_connect, "UPDATE testsuites SET DATE_LAST_MODIFIED='$date' WHERE TS_ID='$ts_id'");
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "copy_teststep":
			$id =  $_POST["id"];
			$ts_id = $_POST["ts-id"];
			$tc_id = $_POST["tc-id"];
			$copy_option = $_POST["n_copy_step"];
			$step_number = $_POST["step-location"];
			if ($copy_option == 2) $step_number = get_new_teststep_num($ts_id, $tc_id);
			$q_condition = "step_number>=$step_number";
			mysqli_query($systemx_connect, "UPDATE teststeps SET STEP_NUMBER=STEP_NUMBER+1000 WHERE TC_ID=$tc_id AND $q_condition AND TS_ID=$ts_id") or die(mysqli_error());
			mysqli_query($systemx_connect, "INSERT INTO teststeps (TC_ID, TS_ID, STEP_NUMBER, STEP_CATEGORY, EXE_STEP, DESCRIPTION, TEST_DATA, EXPECTED_RESULT, DISABLE, AUTO_LOGIN) SELECT TC_ID, TS_ID, $step_number, step_category, exe_step, description, test_data, expected_result, disable, auto_login FROM teststeps WHERE id = $id");
			update_teststep_number($ts_id, $tc_id);
			$sql = "UPDATE testcases SET DATE_LAST_MODIFIED='$activity_log_date', MODIFIER='$designer' WHERE TC_ID=$tc_id AND TS_ID=$ts_id";
			$log = "Copied a teststep (step #$step_number) on TC-$tc_id from TS-$ts_id";
			mysqli_query($systemx_connect, "UPDATE testsuites SET DATE_LAST_MODIFIED='$date' WHERE TS_ID='$ts_id'");
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "move_step":
			$id = $_POST["id"];
			$step_number = $_POST["step-location"];
			$ts_id = $_POST["ts-id"];
			$tc_id = $_POST["tc-id"];
			$sql = "UPDATE testcases SET DATE_LAST_MODIFIED='$activity_log_date', MODIFIER='$designer' WHERE TC_ID=$tc_id AND TS_ID=$ts_id";
			mysqli_query($systemx_connect, "UPDATE teststeps SET step_number=step_number+1000 WHERE TC_ID=$tc_id AND step_number>=$step_number AND TS_ID=$ts_id") or die(mysqli_error());
			mysqli_query($systemx_connect, "UPDATE teststeps SET step_number=$step_number WHERE id=$id") or die(mysqli_error());
			update_teststep_number($ts_id, $tc_id);
			$log = "Moved a teststep (step #$step_number) on TC-$tc_id from TS-$ts_id";
			mysqli_query($systemx_connect, "UPDATE testsuites SET DATE_LAST_MODIFIED='$date' WHERE TS_ID='$ts_id'");
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "get_latest_testcase":
			$ts_id = $_POST["ts-id"];
			echo trim(get_latest_testcase($ts_id));
			$sql = "SELECT TC_ID FROM testcases WHERE TS_ID=$ts_id ORDER BY TC_ID DESC LIMIT 1";
			break;
		case "reg_parent_objects":
			$parent_id = get_new_parent_id();
			$parent_name = $_POST["nWinTitle"];
			$rx_path = mysqli_escape_string($systemx_connect, $_POST["nRxPath"]);
			$control_name = $_POST["nControlName"];
			$description = mysqli_escape_string($systemx_connect, $_POST["nDescription"]);
			$child_id = time();
			$child_type = "Form";
			$sql = "INSERT INTO OBJECTS_PARENTS (PARENT_ID, PARENT_NAME, RX_PATH, CONTROL_NAME, DESCRIPTION) VALUES ('$parent_id', '$parent_name', '$rx_path', '$control_name', '$description')";
			mysqli_query($systemx_connect, "INSERT INTO OBJECTS_CHILDREN (PARENT_ID, CHILD_ID, CHILD_TYPE, RX_PATH, CONTROL_NAME, DESCRIPTION, windowTitle) VALUES ('$parent_id', '$child_id', '$child_type', '$rx_path', '$control_name', '$description', '$parent_name')") or die(mysqli_error());
			$log = "Created a parent object ($parent_name)";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "check_object_exist":
			$parent_name = $_POST["name"];
			$sql = "SELECT PARENT_NAME FROM OBJECTS_PARENTS WHERE PARENT_NAME = '$parent_name'";
			$result = mysqli_query($systemx_connect,$sql)or die(mysqli_error());
			$info = "Objektnavn OK";
			if (mysqli_num_rows($result) > 0)
			{
				$info = "<b>Feil:</b> Dupliserende navn finnes";
			}
			break;
		case "edit_parent_objects":
			$parent_id = $_POST["nParentId"];
			$parent_name = $_POST["nWinTitle"];
			$rx_path = mysqli_escape_string($systemx_connect, $_POST["nRxPath"]);
			$control_name = $_POST["nControlName"];
			$description = mysqli_escape_string($systemx_connect, $_POST["nDescription"]);
			$sql = "UPDATE OBJECTS_PARENTS SET PARENT_NAME='$parent_name', RX_PATH='$rx_path', CONTROL_NAME='$control_name', DESCRIPTION='$description' WHERE PARENT_ID=$parent_id";
			mysqli_query($systemx_connect, "UPDATE OBJECTS_CHILDREN SET CONTROL_NAME='$control_name', RX_PATH='$rx_path', DESCRIPTION='$description' WHERE PARENT_ID=$parent_id AND CHILD_TYPE='Form'") or die(mysqli_error());
			$log = "Modified a parent object ($parent_name)";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "parent_objects_delete":
			$parent_id =  $_POST["id"];
			$sql = "DELETE FROM OBJECTS_PARENTS WHERE PARENT_ID=$parent_id";
			mysqli_query($systemx_connect, "DELETE FROM OBJECTS_CHILDREN WHERE PARENT_ID=$parent_id") or die(mysqli_error());
			$log = "Deleted a parent object";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "check_child_object_exist":
			$control_name = $_POST["name"];
			$parent_id =  $_POST["parent-id"];
			$sql = "SELECT CONTROL_NAME FROM OBJECTS_CHILDREN WHERE CONTROL_NAME = '$control_name' AND PARENT_ID=$parent_id";
			$result = mysqli_query($systemx_connect,$sql)or die(mysqli_error());
			$info = "Objektnavn OK";
			if (mysqli_num_rows($result) > 0)
			{
				$info = "<b>Feil:</b> Dupliserende kontrolnavn finnes";
			}
			break;
		case "reg_child_objects":
			$parent_id =  $_POST["parent-id"];
			$rx_path = mysqli_escape_string($systemx_connect, $_POST["nRxPath"]);
			$control_name = $_POST["nControlName"];
			$description = mysqli_escape_string($systemx_connect, $_POST["nDescription"]);
			$child_id = time();
			$child_type = $_POST["child-type"];
			$sql = "INSERT INTO OBJECTS_CHILDREN (PARENT_ID, CHILD_ID, CHILD_TYPE, RX_PATH, CONTROL_NAME, DESCRIPTION) VALUES ('$parent_id', '$child_id', '$child_type', '$rx_path', '$control_name', '$description')";
			$log = "Created a child object ($control_name)";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "edit_child_objects":
			$child_id =  $_POST["child-id"];
			$rx_path = mysqli_escape_string($systemx_connect, $_POST["nRxPath"]);
			$control_name = $_POST["nControlName"];
			$description = mysqli_escape_string($systemx_connect, $_POST["nDescription"]);
			$child_type = $_POST["child-type"];
			$sql = "UPDATE OBJECTS_CHILDREN SET CHILD_TYPE='$child_type', RX_PATH='$rx_path', CONTROL_NAME='$control_name', DESCRIPTION='$description' WHERE CHILD_ID=$child_id";
			$log = "Modified a child object ($control_name)";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "child_objects_delete":
			$id =  $_POST["id"];
			$sql = "DELETE FROM OBJECTS_CHILDREN WHERE ID=$id";
			$log = "Deleted a child object";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "reg_test_commands":
			$category = $_POST["nCmdCategory"];
			$command_id = get_new_command_id($category);
			$command = $_POST["nCommand"];
			$format = $_POST["nFormat"];
			$description = mysqli_escape_string($systemx_connect, $_POST["nDescription"]);
			$reserved = 1;
			$active = 1;
			$sql = "INSERT INTO test_commands (COMMAND_ID, COMMAND_CATEGORY, TEST_COMMAND, DESCRIPTION, FORMAT, RESERVED, ACTIVE) VALUES ('$command_id', '$category', '$command', '$description', '$format', '$reserved', '$active')";
			
			break;
		case "edit_test_commands":
			$category = $_POST["nCmdCategory"];
			$command_id = $_POST["nCmdID"];
			$command = $_POST["nCommand"];
			$format = $_POST["nFormat"];
			$description = mysqli_escape_string($systemx_connect, $_POST["nDescription"]);
			$reserved = 1;
			$active = 1;
			$sql = "UPDATE test_commands SET COMMAND_CATEGORY='$category', TEST_COMMAND='$command', DESCRIPTION='$description', FORMAT='$format' WHERE COMMAND_ID='$command_id'";
			
			break;
		case "activate_command":
			$command_id = $_POST["id"];
			$active = $_POST["n_activate"];
			$sql = "UPDATE test_commands SET ACTIVE='$active' WHERE COMMAND_ID='$command_id'";
			
			break;
		case "test_commands_delete":
			$id = $_POST["id"];
			$sql = "DELETE FROM test_commands WHERE ID='$id'";
			break;
		case "reg_updates":
			$log_date = $_POST["nLogDate"];
			$update = $_POST["nUpdates"];
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO update_log (LOG_DATE, LOG_TEXT) VALUES ('$log_date', \"$update\")") or die(mysqli_error());
			
			break;
		case "edit_updates":
			$id =  $_POST["id"];
			$log_date = $_POST["nLogDate"];
			$update = $_POST["nUpdates"];
			mysqli_query($GLOBALS['users_connect'], "UPDATE update_log SET LOG_DATE='$log_date', LOG_TEXT=\"$update\" WHERE ID='$id'") or die(mysqli_error());
			break;
		case "update_delete":
			$id =  $_POST["id"];
			mysqli_query($GLOBALS['users_connect'], "DELETE FROM update_log WHERE ID='$id'") or die(mysqli_error());
			break;
		case "reg_tasks":
			$task_date = $_POST["nTaskDate"];
			$task_area = $_POST["nTaskArea"];
			$task = $_POST["nTask"];
			$status = $_POST["nStatus"];
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO tasks (TASK_DATE, TASK, STATUS, USER, TASK_AREA) VALUES ('$task_date', \"$task\", '$status', '$designer', '$task_area')") or die(mysqli_error());
			$log = "Created a task";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "edit_tasks":
			$id =  $_POST["id"];
			$task_date = $_POST["nTaskDate"];
			$task_area = $_POST["nTaskArea"];
			$task = $_POST["nTask"];
			$status = $_POST["nStatus"];
			mysqli_query($GLOBALS['users_connect'], "UPDATE tasks SET TASK_DATE='$task_date', TASK=\"$task\", STATUS='$status', TASK_AREA='$task_area' WHERE ID='$id'") or die(mysqli_error());
			$log = "Modified a task";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "task_delete":
			$id =  $_POST["id"];
			mysqli_query($GLOBALS['users_connect'], "DELETE FROM tasks WHERE ID='$id'") or die(mysqli_error());
			$log = "Deleted a task";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			break;
		case "create_password":
		case "create_temp_password":
			$len_password =  $_POST["length"];
			$info = rand_string($len_password);
			break;
		case "reset_password":
			$password =  md5($_POST["password"]);
			$username =  $_POST["username"];
			mysqli_query($GLOBALS['users_connect'], "UPDATE designers SET PASSWORD='$password' WHERE USERNAME='$username'") or die(mysqli_error());
			break;
		case "update_profile_photo":
			$photo =  $_POST["photo"];
			$log = "Changed profile photo";
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO activity_logs (LOG_DATE, LOG, USERNAME) VALUES ('$activity_log_date', '$log', '$designer')") or die(mysqli_error());
			mysqli_query($GLOBALS['users_connect'],"UPDATE designers SET AVATAR='$photo' WHERE username='$designer'")or die(mysqli_error($users_connect));
			break;
		case "clean_logs":
			mysqli_query($GLOBALS['users_connect'],"TRUNCATE TABLE activity_logs")or die(mysqli_error($users_connect));
			break;
		case "activate_limit":
			$threshold_limit = 0;                                  
			$limit =  $_POST["nLimit"];
			if ($limit == 1)
			{
				$threshold_limit =  $_POST["nThresholdLimit"];
			}
			mysqli_query($GLOBALS['users_connect'],"UPDATE designers SET LOG_LIMIT='$threshold_limit' WHERE USERNAME='$designer'")or die(mysqli_error($users_connect));
			break;
		case "change_database":
			$database =  $_POST["n_database"];
			unset($_SESSION['database']);
			$_SESSION["database"] = $database;
			break;
		case "send_message":
			$receiver =  $_POST["nReceiver"];
			$subject =  $_POST["nSubject"];
			$message =  mysqli_escape_string($GLOBALS['users_connect'], $_POST["nMessage"]);
			$message_id = md5(sprintf("%06d", mt_rand(1, 999999)));
			$date_send = $activity_log_date;
			mysqli_query($GLOBALS['users_connect'], "INSERT INTO messages (MESSAGE_ID, DATE_SEND, SENDER, RECEIVER, SUBJECT, MESSAGE) VALUES ('$message_id', '$date_send', '$designer', '$receiver', \"$subject\", \"$message\")") or die(mysqli_error());
			break;
		case "activate_user":
			$username =  $_POST["username"];
			$activate =  $_POST["nToggleActivate"];
			mysqli_query($GLOBALS['users_connect'],"UPDATE designers SET ACTIVE='$activate' WHERE USERNAME='$username'")or die(mysqli_error($users_connect));
			break;
		case "get_test_command_info":
			$command_id =  $_POST["command-id"];
			echo get_test_command_info($command_id, 5);
			break;
		case "search_testcase":
			$keyword = $_POST["keyword"];
			$query = "SELECT * FROM testcases WHERE TC_ID LIKE '%$keyword%' OR TC_TITLE LIKE '%$keyword%' OR TS_ID LIKE '%$keyword%' OR COMMENTS LIKE '%$keyword%' OR JIRA_TICKET LIKE '%$keyword%'";
			//$query = "SELECT * FROM testcases INNER JOIN teststeps ON testcases.TC_ID = teststeps.TC_ID WHERE teststeps.DESCRIPTION LIKE '%$keyword%'";
			$result = mysqli_query($systemx_connect,$query)or die(mysqli_error());
			echo "<div style='text-align:right;padding:5px;color:#FFF;background:#01A9DB'>Ditt søk returnerte <b>" . mysqli_num_rows($result) . "</b> resultater</div>";
			while($row = mysqli_fetch_array($result, MYSQL_NUM))
			{
				$jira_ticket = (empty($row[11])) ? "(Ingen)" : $row[11];
				$comments = (empty($row[10])) ? "Ingen kommentar..." : $row[10];
				$tc_title = (strlen($row[3]) > 40) ? substr($row[3], 0, 40) . "..." : $row[3];
				echo "<div class='rowResult' data-ts-id='$row[2]' data-tc-id='$row[1]' title='$row[3]'>
						TS-$row[2] | TC-$row[1]: " . $tc_title . "<p style='float:right'>Jira Ticket: $jira_ticket</p><p style='font-style:italic'>$comments</p>
					</div>";
			}
			if (mysqli_num_rows($result) == 0)
			{
				echo "<i style='color:firebrick;font-weight:bold'>Ingen treff...</i>";
			}
			break;
		case "search_object":
			$keyword = $_POST["keyword"];
			//$query = "SELECT * FROM objects_parents INNER JOIN objects_children ON objects_parents.PARENT_ID = objects_children.PARENT_ID WHERE objects_parents.PARENT_NAME LIKE '%$keyword%' OR objects_parents.RX_PATH LIKE '$%keyword%' OR objects_parents.CONTROL_NAME LIKE '%$keyword%' OR objects_parents.DESCRIPTION LIKE '%$keyword%' OR objects_children.RX_PATH LIKE '$%keyword%' OR objects_children.CONTROL_NAME LIKE '%$keyword%' OR objects_children.DESCRIPTION LIKE '%$keyword%'";
			$query = "SELECT * FROM objects_children WHERE RX_PATH LIKE '%$keyword%' OR windowTitle LIKE '%$keyword%'";
			$result = mysqli_query($systemx_connect,$query)or die(mysqli_error());
			echo "<div style='text-align:right;padding:5px;color:#FFF;background:#01A9DB'>Ditt søk returnerte <b>" . mysqli_num_rows($result) . "</b> resultater</div>";
			while($row = mysqli_fetch_array($result, MYSQL_NUM))
			{
				$parentName = get_parent_info($row[1], "PARENT_NAME");
				echo "<div class='rowObjResult' title=\"$row[5]\" data-parent-id='$row[1]'>
						$parentName<p style='font-style:italic'>$row[6]</p>
					</div>";
			}
			if (mysqli_num_rows($result) == 0)
			{
				echo "<i style='color:firebrick;font-weight:bold'>Ingen treff...</i>";
			}
			break;
		case "build_jenkins":
			$version = $_POST["sx-version"];
			$set = $_POST["set"];
			$response = startBuild($version, $set, $_COOKIE["user"], $_COOKIE["token"]);
			if (strpos($response, 'FEIL') !== false) 
			{
				$info = $response;
			}
			break;
		case "stop_build_jenkins":
			$version = $_POST["sx-version"];
			$set = $_POST["set"];
			$id = $_POST["id"];
			$response = stopBuild($version, $set, $id);
			if (strpos($response, 'FEIL') !== false) 
			{
				$info = $response;
			}
			break;
		case "get_progress":
			$version = $_POST["sx-version"];
			$set = $_POST["set"];
			$buildID = $_POST["id"];
			$divID = $_POST["div-id"];
			$progress = getProgress($version, $set, $buildID);
			$info = "$progress:$divID";
			break;
		case "activate_testsuite":
			$tsID = $_POST["ts-id"];
			$active = $_POST["n_activate"];
			$comments = ($active) ? "For å teste basisfunksjoner knyttet til " . get_testsuite_info($tsID, 2) . " program" : $_POST["comments"];
			$sql = "UPDATE testsuites SET ACTIVE='$active', DESCRIPTION='$comments' WHERE TS_ID='$tsID'";
			break;
		case "login_jenkins":
			$user = $_POST['user'];
			$token = $_POST['token'];
			$buildAction = $_POST['build-action'];
			$version = $_POST["sx-version"];
			$set = $_POST["set"];
			$buildID = $_POST['build-id'];
			$response = checkJenkinsAPIResponse();
			if ($response == "SUCCESS")
			{
				setcookie("user", $user, time()+3600);
				setcookie("token", $token, time()+3600);
				if ($buildAction == "run") 
				{ 
					echo startBuild($version, $set); 
				}
				else 
				{ 
					echo stopBuild($version, $set, $buildID, $user, $token); 
				}
			}
			else
			{
				echo $response;
			}
			break;
		case "change_tc_order":
			$tsID = $_POST["nTSID"];
			$oldTcIDStart = $_POST["nStartTCIDOld"];
			$oldTcIDEnd = $_POST["nEndTCIDOld"];
			$startTcIDNew = $_POST["nStartTCIDNew"];
			if (!is_testsuite_exist($tsID))
			{
				echo "ERROR: TS-ID ($tsID) does not exist!";
			}
			else
			{
				if ($oldTcIDStart > $oldTcIDEnd)
				{
					echo "ERROR: Starting TC-ID must be less than the Ending TC-ID!";
				}
				else
				{
					for ($x = $oldTcIDStart; $x<=$oldTcIDEnd; $x++)
					{
						mysqli_query($systemx_connect, "UPDATE TESTCASES SET TC_ID=$startTcIDNew WHERE TS_ID=$tsID AND TC_ID=$x") or die(mysqli_error());
						mysqli_query($systemx_connect, "UPDATE teststeps SET TC_ID=$startTcIDNew WHERE TS_ID=$tsID AND TC_ID=$x") or die(mysqli_error());
						$startTcIDNew++;
					}
					echo "SUCCESS: Testcase order has been changed!";
				}
			}
			break;
		case "activate_testcases":
			$tsID = $_POST["nTS"];
			$tcID = $_POST["nTC"];
			$option = $_POST['nOption'];
			$active = ($option >=2 && $option <=3) ? 1:0;
			if (($tsID > 9999) || ($tsID < 1000))
			{
				echo "ERROR: Invalid TS-ID format!";
			}
			else
			{
				if (!is_testsuite_exist($tsID))
				{
					echo "ERROR: TS-ID ($tsID $active) does not exist!";
				}
				else
				{
					if ($option <=2)
					{
						$sql =  "UPDATE testcases SET ACTIVE=$active, RUN_STATUS=NULL WHERE TS_ID=$tsID";
						$info = "<b>Result:</b> SUCCESS";
					}
					else
					{
						if (!is_entry_valid($tcID))
						{
							echo "ERROR: Invalid data entered on Testcase ID field!";
						}
						else
						{
							$arr_list = get_valid_testcases($tcID);
							$response = true;
							foreach ($arr_list as $list)
							{
								if (($list > 999) || ($list < 100))
								{
									echo "ERROR: Testcase ID must be from 100 - 999!";
									$response = false;
									break;
								}
								//echo "$list<br>";
								if (!is_tcnum_exist($list, $tsID))
								{
									echo "ERROR: One or more testcases entered are not found!";
									$response = false;
									break;
								}
							}
							if ($response)
							{
								$conWhereClause = " AND (";
								$i = 1;
								foreach ($arr_list as $testcase)
								{
									$conWhereClause = ($i == 1) ? "$conWhereClause TC_ID=$testcase" : "$conWhereClause OR TC_ID=$testcase";
									$i++;
								}
								$conWhereClause = $conWhereClause . ")";
								$sql = "UPDATE testcases SET ACTIVE=$active WHERE TS_ID=$tsID $conWhereClause";
								$info = "<b>Result:</b> SUCCESS";
							}
						}
					}
				}
			}
			break;
		case "add_role":
			$role = $_POST["nRoleName"];
			$sql_ = "INSERT INTO roles (ROLE) VALUES ('$role')";
			mysqli_query($GLOBALS['users_connect'], $sql_) or die(mysqli_error());
			$info = "<b>SUCCESS: New role has been successfully added...</b>";
			break;
		case "edit_role":
			$role = $_POST["nRoleName"];
			$id = $_POST["id"];
			//TESTSUITES
			$nTSAdd = (isset($_POST["nTSAdd"])) ? 1 : 0;
			$nTSView = (isset($_POST["nTSView"])) ? 1 : 0;
			$nTSEdit = (isset($_POST["nTSEdit"])) ? 1 : 0;
			$nTSDelete = (isset($_POST["nTSDelete"])) ? 1 : 0;
			
			//TESTCASES
			$nTCAdd = (isset($_POST["nTCAdd"])) ? 1 : 0;
			$nTCView = (isset($_POST["nTCView"])) ? 1 : 0;
			$nTCEdit = (isset($_POST["nTCEdit"])) ? 1 : 0;
			$nTCDelete = (isset($_POST["nTCDelete"])) ? 1 : 0;
			
			//TEST STEPS
			$nStepAdd = (isset($_POST["nStepAdd"])) ? 1 : 0;
			$nStepView = (isset($_POST["nStepView"])) ? 1 : 0;
			$nStepEdit = (isset($_POST["nStepEdit"])) ? 1 : 0;
			$nStepDelete = (isset($_POST["nStepDelete"])) ? 1 : 0;
			
			//TEST COMMANDS
			$nCmdAdd = (isset($_POST["nCmdAdd"])) ? 1 : 0;
			$nCmdView = (isset($_POST["nCmdView"])) ? 1 : 0;
			$nCmdEdit = (isset($_POST["nCmdEdit"])) ? 1 : 0;
			$nCmdDelete = (isset($_POST["nCmdDelete"])) ? 1 : 0;
			
			//TEST OBJECTS
			$nObjAdd = (isset($_POST["nObjAdd"])) ? 1 : 0;
			$nObjView = (isset($_POST["nObjView"])) ? 1 : 0;
			$nObjEdit = (isset($_POST["nObjEdit"])) ? 1 : 0;
			$nObjDelete = (isset($_POST["nObjDelete"])) ? 1 : 0;
			
			//TASKS
			$nTaskAdd = (isset($_POST["nTaskAdd"])) ? 1 : 0;
			$nTaskView = (isset($_POST["nTaskView"])) ? 1 : 0;
			$nTaskEdit = (isset($_POST["nTaskEdit"])) ? 1 : 0;
			$nTaskDelete = (isset($_POST["nTaskDelete"])) ? 1 : 0;
			
			//TESTCASE MANAGER
			$nTCManager = (isset($_POST["nTCManager"])) ? 1 : 0;
			
			$sql_ = "UPDATE roles SET 
						ROLE='$role', 
						TS_ADD=$nTSAdd, 
						TS_VIEW=$nTSView, 
						TS_EDIT=$nTSEdit, 
						TS_DELETE=$nTSDelete, 
						TC_ADD=$nTCAdd, 
						TC_VIEW=$nTCView, 
						TC_EDIT=$nTCEdit, 
						TC_DELETE=$nTCDelete, 
						STEP_ADD=$nStepAdd, 
						STEP_VIEW=$nStepView, 
						STEP_EDIT=$nStepEdit, 
						STEP_DELETE=$nStepDelete, 
						CMD_ADD=$nCmdAdd, 
						CMD_VIEW=$nCmdView, 
						CMD_EDIT=$nCmdEdit, 
						CMD_DELETE=$nCmdDelete, 
						OBJ_ADD=$nObjAdd, 
						OBJ_VIEW=$nObjView, 
						OBJ_EDIT=$nObjEdit, 
						OBJ_DELETE=$nObjDelete,
						TASK_ADD=$nTaskAdd, 
						TASK_VIEW=$nTaskView, 
						TASK_EDIT=$nTaskEdit, 
						TASK_DELETE=$nTaskDelete, 
						TC_MANAGER=$nTCManager 
					WHERE ROLE_ID=$id";
			mysqli_query($GLOBALS['users_connect'], $sql_) or die(mysqli_error());
			$info = "<b>SUCCESS: Role has been successfully modified...</b>";
			break;
		case "delete_role":
			$id = $_POST["id"];
			$sql_ =  "DELETE FROM roles WHERE ROLE_ID=$id";
			mysqli_query($GLOBALS['users_connect'], $sql_) or die(mysqli_error());
			$info = "<b>SUCCESS: Role has been successfully deleted...</b>";
			break;
		case "add_user":
			$last = ucfirst(strtolower($_POST["nLastName"]));
			$first = ucfirst(strtolower($_POST["nFirstName"]));
			$username = $_POST["nUserName"];
			$password = md5($_POST["nPassword"]);
			$date = date("Y-m-d");
			$role = $_POST["nRole"];
			$rights = 0;
			$avatar = "images/avatar/default-user.png";
			
			if (!is_username_exist($username))
			{
				$sql_ = "INSERT INTO designers (LNAME, FNAME, USERNAME, PASSWORD, DATE_REG, ROLE_ID, AVATAR, ACTIVE) VALUES ('$last', '$first', '$username', '$password', '$date', '$role', '$avatar', 1)";
				mysqli_query($GLOBALS['users_connect'], $sql_) or die(mysqli_error());
				$info = "<b>SUCCESS:</b> New user has been successfully added...";
			}
			else
			{
				$info = "<b>ERROR:</b> Username is already in use!";
			}
			break;
		case "edit_user":
			$username = $_POST["username"];
			$role = $_POST["nRole"];
			$password = md5(trim($_POST["nPassword"]));
			if (empty(trim($_POST["nPassword"])))
			{
				$sql_ =  "UPDATE designers SET ROLE_ID=$role WHERE USERNAME='$username'";
			}
			else
			{
				$sql_ =  "UPDATE designers SET ROLE_ID=$role, PASSWORD='$password' WHERE USERNAME='$username'";
			}
			mysqli_query($GLOBALS['users_connect'], $sql_) or die($sql_);
			$info = "<b>SUCCESS: User has been successfully modified...</b>";
			break;
	}
	
	//Notify the user the response of the request
	echo $info;
	
	//Execute the MySQL query
	if (!empty($sql))
	{
		mysqli_query($systemx_connect, $sql) or die(mysqli_error());
	}
	
?>