<?php
	error_reporting( E_ALL );
	ini_set('display_errors', 1);
	include "dbconn-systemx.php";
	//global $systemx_connect;
	
	function test()
	{
		return $GLOBALS['hostname'];
	}
	//To get a new testsuite ID
	function get_new_testsuite_id()
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT ts_id FROM testsuites ORDER BY ts_id DESC LIMIT 1");
		if (mysqli_num_rows($sql) == 0)
		{
			$result = 999;
		}
		while($row = mysqli_fetch_array($sql))
		{
			$result = $row[0];
		}
		return $result + 1;
	}
	
	//To get a new testcase ID
	function get_new_testcase_num($ts_id)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT tc_id FROM testcases WHERE ts_id=$ts_id ORDER BY tc_id DESC LIMIT 1");
		$result = 100;
		if (mysqli_num_rows($sql) > 0)
		{
			$arrayResult = mysqli_fetch_array($sql);
			$result = $arrayResult['tc_id'] + 1;
		}
		return $result;
	}
	
	//To get a new test step number
	function get_new_teststep_num($ts_id, $tc_num)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT step_number FROM teststeps WHERE ts_id=$ts_id AND tc_id=$tc_num ORDER BY step_number DESC LIMIT 1") or die(mysqli_error());
		$result = 0;
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[0];
			}
		}
		return $result + 1;
	}
	
	function get_new_command_id($category)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT COMMAND_ID FROM test_commands WHERE COMMAND_CATEGORY='$category' ORDER BY COMMAND_ID DESC LIMIT 1");
		if (mysqli_num_rows($sql) == 0)
		{
			$result = 999;
		}
		while($row = mysqli_fetch_array($sql))
		{
			$result = $row[0];
		}
		return $result + 1;
	}
	
	
	//To get information from a given testsuite
	function get_testsuite_info($ts_id, $col)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM testsuites WHERE ts_id='$ts_id'");
		$result = "";
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$col];
			}
		}
		return $result;
	}
	
	function get_testsuite_info_by_id($id, $col)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM testsuites WHERE id='$id'");
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$col];
			}
		}
		return $result;
	}
	
	//To get information from a given testcase
	function get_testcase_info($id=null, $col, $tc_num, $ts_id)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM testcases WHERE id='$id'") or die(mysqli_error());
		$result = "";
		if ($id == null)
		{
			$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM testcases WHERE tc_id=$tc_num AND ts_id=$ts_id") or die(mysqli_error());
		}
		
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$col];
			}
		}
		return $result;
	}
	
	function get_testcase_info_by_id($id, $col)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM testcases WHERE id='$id'") or die(mysqli_error());
		$result = "";
		
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$col];
			}
		}
		return $result;
	}
	
	//To get information from a given test step
	function get_teststep_info($id, $col)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM teststeps WHERE id='$id'");
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$col];
			}
		}
		return $result;
	}
	
	//To get the number of testcases from a given testsuite
	function get_number_of_testcases($ts_id, $only_active=false)
	{
		$condition = ($only_active) ? "AND ACTIVE=1" : "";
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM testcases WHERE ts_id='$ts_id' $condition ORDER BY tc_id ASC");
		return $result = mysqli_num_rows($sql);
	}
	
	function get_number_of_active_testcases($ts_id)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM testcases WHERE ts_id='$ts_id' and active=1 ORDER BY tc_id ASC");
		return $result = mysqli_num_rows($sql);
	}
	
	//Validate if the given testcase ID exist
	function is_tcnum_exist($tc_num, $ts_id)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM testcases WHERE ts_id='$ts_id' AND tc_id='$tc_num'");
		$result = mysqli_num_rows($sql);
		/* $ret_val = true;
		if ($result == 0)
		{
			$ret_val = false;
		} */
		return ($result > 0);
	}
	
	//Re-organize the testcase IDs
	function update_testcase_number()
	{
		$ts_ids = mysqli_query($GLOBALS['systemx_connect'], "SELECT SQL_NO_CACHE testcases.ts_id FROM testcases ORDER BY testcases.ts_id ASC");
		while($row = mysqli_fetch_array($ts_ids))
		{
			update_testcase_number_per_suite($row[0]);
		}
	}
	
	//Re-organize the testcase IDs from a given testsuite
	function update_testcase_number_per_suite($ts_id)
	{
		//Default starting testcase ID
		$tc_num = 100;
		if ($ts_id != "")
		{
			$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT SQL_NO_CACHE * FROM testcases WHERE ts_id='$ts_id' ORDER BY tc_id ASC");
			while($row = mysqli_fetch_array($sql))
			{
				mysqli_query($GLOBALS['systemx_connect'], "UPDATE testcases SET tc_id=$tc_num WHERE ts_id='$ts_id' AND tc_id=$row[1]") or die(mysqli_error());
				$tc_num++;
			}
		}
	}
	
	//Re-organize the testsuite IDs
	function update_testsuite_number()
	{
		//Default starting testsuite ID
		$ts_num = 1000;
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM testsuites ORDER BY ts_id ASC");
		while($row = mysqli_fetch_array($sql))
		{
			mysqli_query($GLOBALS['systemx_connect'], "UPDATE testsuites SET ts_id=$ts_num WHERE ts_id=$row[1]") or die(mysqli_error());
			$ts_num++;
		}
	}
	
	//Re-organize the test step numbers from a given testsuite ID and testcase ID
	function update_teststep_number($ts_id, $tc_num)
	{
		$step_number = 1;
		if ($ts_id != "")
		{
			$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM teststeps WHERE ts_id='$ts_id' AND tc_id=$tc_num AND step_number>0 ORDER BY step_number ASC");
			while($row = mysqli_fetch_array($sql))
			{
				mysqli_query($GLOBALS['systemx_connect'], "UPDATE teststeps SET step_number=$step_number WHERE ts_id=$ts_id AND id=$row[0] AND tc_id=$tc_num") or die(mysqli_error());
				$step_number++;
			}
		}
	}
	
	//To get the total number of test steps from a given testsuite ID and testcase ID
	function get_number_of_teststeps($ts_id, $tc_num)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM teststeps WHERE ts_id='$ts_id' AND tc_id='$tc_num' AND step_number>0");
		return $result = mysqli_num_rows($sql);
	}
	
	//To get information from a given command
	function get_command_info($id, $col)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM test_commands WHERE COMMAND_ID='$id'");
		$result = "";
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$col];
			}
		}
		return $result;
	}
	
	function get_test_command_info($cmdID, $col)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM test_commands WHERE COMMAND_ID='$cmdID'");
		$result = "";
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$col];
			}
		}
		return $result;
	}
	
	//To get all the testsuites
	function get_all_test_suites()
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT ts_id, title FROM testsuites ORDER BY title ASC");
		$result = array();
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				array_push($result, "$row[0]:$row[1]");
			}
		}
		return $result;
	}
	
	function get_all_parent_objects()
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT PARENT_ID, PARENT_NAME FROM OBJECTS_PARENTS ORDER BY PARENT_NAME ASC");
		$result = array();
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				array_push($result, "$row[0]:$row[1]");
			}
		}
		return $result;
	}
	
	
	//Validate if the given username has existing testsuites/testcases
	function is_user_owner($username)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM testcases WHERE designer='$username' OR modifier='$username'");
		$result = false;
		if (mysqli_num_rows($sql) > 0)
		{
			$result = true;
		}
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM testsuites WHERE designer='$username'") or die(mysqli_error());
		if (mysqli_num_rows($sql) > 0)
		{
			$result = true;
		}
		
		return $result;
	}
	

	
	//Validate if the given command is already in the database
	function check_duplicate_command($step_category, $command, $rec_id)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT command FROM test_commands WHERE command_category='$step_category' AND command='$command' AND id <> $rec_id");
		if (empty($rec_id))
		{
			$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT command FROM test_commands WHERE command_category='$step_category' AND command='$command'");
		}
		$result = false;
		if (mysqli_num_rows($sql) > 0)
		{
			$result = true;
		}
		return $result;
	}
	
	//To get all the testcases from all testsuites
	function get_testcase_count($write_status=false)
	{
		$condition = "";
		if ($write_status)
		{
			$condition = "WHERE WRITE_STATUS=1";
		}
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM testcases $condition");
		return mysqli_num_rows($sql);
	}
	
	function get_active_testcase_count()
	{
		//$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM testcases WHERE ACTIVE=1");
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT COUNT(*) AS COUNT FROM testcases INNER JOIN testsuites ON testcases.TS_ID = testsuites.TS_ID WHERE testcases.ACTIVE=1 AND testsuites.ACTIVE=1");
		$data=mysqli_fetch_assoc($sql);
		//return mysqli_num_rows($sql);
		return $data['COUNT'];
	}
	
	//To get all the testcases from a given testsuite
	function get_testcase_count_per_testsuite($ts_id, $write_status=false)
	{
		$condition = "";
		if ($write_status)
		{
			$condition = "AND write_status=1";
		}
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM testcases WHERE TS_ID='$ts_id' $condition");
		return mysqli_num_rows($sql);
	}
	
	//To get all the testsuites
	function get_testsuite_count($write_status=false)
	{
		$condition = "";
		
		if ($write_status)
		{
			$condition = "WHERE write_status=1";
		}
		$sql = mysqli_query($GLOBALS['systemx_connect'],"SELECT * FROM testsuites $condition") or die (mysqli_error($GLOBALS['systemx_connect']));
		return mysqli_num_rows($sql);
		
		
	}
	
	function get_record_count($tableName, $dbName='systemx_connect')
	{
		$sql = mysqli_query($GLOBALS[$dbName], "SELECT * FROM $tableName");
		return mysqli_num_rows($sql);
	}
	
	//To get the number of known commands
	function get_command_count()
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM test_commands");
		return mysqli_num_rows($sql);
	}
	
	//To get System X program information
	function get_program_info($id, $info)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM main_programs WHERE program_id=$id");
		$result = "";
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$info];
			}
		}
		$result = ($id == "9999999") ? $result = "Eksternt Program" : $result = $result;
		return $result;
	}
	
	function query_program_by_name($type, $name, $info)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM " . $type . "_programs WHERE prog_name='$name'") or die (mysqli_error());
		$result = "";
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$info];
			}
		}
		$result = ($type == "main" && $name == "Eksternt Program") ? $result = "9999999" : $result = $result;
		$result = empty($result) ? $result = "$name" : $result = $result;
		return $result;
	}
	
	function query_object_by_name($progName, $objectName, $info)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM OBJECTS_CHILDREN WHERE CONTROL_NAME = '$objectName' AND CHILD_NAME = '$progName'") or die (mysqli_error());
		$result = "";
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$info];
			}
		}
		return $result;
	}
	
	function query_parent_object_by_name($parentName, $info)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM OBJECTS_PARENTS WHERE PARENT_NAME = '$parentName'") or die (mysqli_error());
		$result = "";
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$info];
			}
		}
		return $result;
	}
	
	function query_child_object_by_name($parentId, $controlName, $info)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM OBJECTS_CHILDREN WHERE PARENT_ID = '$parentId' AND CONTROL_NAME='$controlName'") or die (mysqli_error());
		$result = "";
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$info];
			}
		}
		return $result;
	}
	
	
	
	//To get the number of System X programs
	function get_program_count()
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM main_programs");
		return mysqli_num_rows($sql);
	}
	
	//To get the number of System X sub-programs
	function get_subprogram_count($prog_id)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM sub_programs WHERE main_prog_id=$prog_id");
		return mysqli_num_rows($sql);
	}
	
	//To get System X sub-program information
	function get_subprogram_info($id, $info)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM sub_programs WHERE sub_prog_id='$id'");
		$result = "";
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$info];
			}
		}
		$result = (empty($result)) ? $result = $id : $result = $result;
		return $result;
	}
	
	function get_all_programs_with_objects()
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT DISTINCT CHILD_NAME FROM OBJECTS_CHILDREN ORDER BY CHILD_NAME ASC");
		$result = array();
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				//$program_name = get_program_info($row[0], 1);
				array_push($result, "$row[0]");
			}
		}
		return $result;
	}
	
	function get_all_main_programs()
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT program_id, prog_name FROM main_programs ORDER BY prog_name ASC");
		$result = array();
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				array_push($result, "$row[0]:$row[1]");
			}
		}
		return $result;
	}
	
	function get_all_sub_programs($program_id)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT prog_name FROM sub_programs WHERE main_prog_id=$program_id ORDER BY prog_name ASC");
		$result = array();
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				array_push($result, "$row[0]");
			}
		}
		return $result;
	}
	
	function check_duplicate_entry($tbl_name, $column_name, $entry, $extra_condition)
	{
		$result = false;
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM $tbl_name WHERE $column_name='$entry' $extra_condition");
		if (mysqli_num_rows($sql) > 0)
		{
			$result = true;
		}
		return $result;
	}
	
	/* function get_object_info($id, $info)
	{
		$sql = mysqli_query("SELECT * FROM objects WHERE object_id='$id'");
		$result = "";
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$info];
			}
		}
		$result = empty($result) ? $result = $id : $result = $result;
		return $result;
	} */
	
	function get_object_info($src_col, $src_str, $info, $extra_condition="")
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM OBJECTS_CHILDREN WHERE $src_col='$src_str' $extra_condition");
		$result = "";
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$info];
			}
		}
		$result = empty($result) ? $result = $src_str : $result = $result;
		return $result;
	}
	
	function get_object_info_by_name($name, $info)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM OBJECTS_CHILDREN WHERE CONTROL_NAME='$name'");
		$result = "";
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$info];
			}
		}
		$result = empty($result) ? $result = $name : $result = $result;
		return trim($result);
	}
	
	function update_object_on_teststeps($old_object_name, $new_object_name)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT id, test_data FROM teststeps WHERE test_data LIKE '%[$old_object_name]%'");
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$new_test_data = str_replace($old_object_name, $new_object_name, $row[1]);
				mysqli_query($GLOBALS['systemx_connect'], "UPDATE teststeps SET test_data='$new_test_data' WHERE id=$row[0]");
			}
		}
	}
	
	function extract_test_data($test_data)
	{
		preg_match_all("/\[([^\]]*)\]/", $test_data, $matches);
		return $matches[1];
	}
	
	function get_database_name()
	{
		$database = $_SESSION["database"];
		$dbList = str_replace("sxtest_", "", $database);
		$dbItem = substr($dbList, 0, 1) . "." . substr($dbList, 1, 1);
		return "v$dbItem";
	}
	
	function is_testsuite_exist($ts_id)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT ts_id FROM testsuites WHERE ts_id='$ts_id'");
		return (mysqli_num_rows($sql) > 0);
	}
	
	function is_entry_valid($entry)
	{
		$arr_entry = explode(",", $entry);
		$valid = true;
		foreach ($arr_entry as $tc)
		{
			$arr_entry2 = explode("-", $tc);
			if (strpos($tc, '-') !== false) 
			{
				/*if (($arr_entry2[0] > 999) || ($arr_entry2[0] < 100))
				{
					$valid = false;
					break;
				}
				if (($arr_entry2[1] > 999) || ($arr_entry2[1] < 100))
				{
					$valid = false;
					break;
				}*/
				if ($arr_entry2[0] > $arr_entry2[1])
				{
					$valid = false;
					break;
				}
			}
			
			else
			{
				foreach ($arr_entry2 as $tc2)
				{
					if (empty($tc2))
					{
						$valid = false;
						break;
					}
					//echo $tc2 . "<br>";
				}
			}
			if (empty($tc))
			{
				$valid = false;
			}
			if (!$valid) break;
		}
		return $valid;
	}
	
	function get_valid_testcases($testcases)
	{
		$arr_entry = explode(",", $testcases);
		$testcase_list = [];
		foreach ($arr_entry as $tc)
		{
			$arr_entry2 = explode("-", $tc);
			if (strpos($tc, '-') !== false)
			{
				for ($i = $arr_entry2[0]; $i<=$arr_entry2[1]; $i++)
				{
					array_push($testcase_list,$i);
				}
			}
			else 
			{
				foreach ($arr_entry2 as $tc2)
				{
					array_push($testcase_list,$tc2);
				}
			}
		}
		return $testcase_list;
	}
	
	function get_active($page_id)
	{
		$response = "";
		$id = (!isset($_GET['id'])) ? 0 : $_GET['id'];
		$arr_pages = explode(",", $page_id);
		foreach ($arr_pages as $page)
		{
			if ($page == $id) 
			{
				$response = "current";
				break;
			}
		}
		return $response;
	}
	
	function get_latest_testcase($ts_id)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT tc_id FROM testcases WHERE ts_id=$ts_id ORDER BY tc_id DESC LIMIT 1");
		$row = mysqli_fetch_array($sql);
		return $row['tc_id'];
	}
	
	function get_latest_testsuite()
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT ts_id FROM testsuites ORDER BY ts_id DESC LIMIT 1");
		$row = mysqli_fetch_array($sql);
		return $row['ts_id'];
	}
	
	//To get a new parent ID
	function get_new_parent_id()
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT PARENT_ID FROM OBJECTS_PARENTS ORDER BY PARENT_ID DESC LIMIT 1");
		if (mysqli_num_rows($sql) == 0)
		{
			$result = 999;
		}
		while($row = mysqli_fetch_array($sql))
		{
			$result = $row[0];
		}
		return $result + 1;
	}
	
	function get_parent_info($parent_id, $col_name)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT $col_name FROM OBJECTS_PARENTS WHERE PARENT_ID='$parent_id'");
		$row = mysqli_fetch_array($sql);
		return $row[$col_name];
	}
	
	function get_child_info($child_id, $col_name)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT $col_name FROM OBJECTS_CHILDREN WHERE CHILD_ID=$child_id ");
		if (mysqli_num_rows($sql) == 0)
		{
			return "";
		}
		else
		{
			$row = mysqli_fetch_array($sql);
			return $row[$col_name];
		}
	}
	
	function get_number_of_child_objects($parent_id)
	{
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM OBJECTS_CHILDREN WHERE PARENT_ID='$parent_id'");
		return $result = mysqli_num_rows($sql);
	}
	
	function rand_string( $length ) 
	{
       $chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz@#$&*";  
       $size = strlen( $chars );
	   $string = "";
       for( $i = 0; $i < $length; $i++ ) 
	   {
			$str= $chars[ rand( 0, $size - 1 ) ];
			$string = $string . $str;
              

       }
		return $string;
	}
	
	function ExportFile($records) 
	{
		 $heading = false;
		 if(!empty($records))
		   foreach($records as $row) {
		 if(!$heading) {
		   // display field/column names as a first row
		   echo implode("\t", array_keys($row)) . "\n";
		   $heading = true;
		 }
		 echo implode("\t", array_values($row)) . "\n";
		   }
		 exit;
	}
	
	function get_last_modified_date()
	{
		$mostRecentFilePath = "";
		$mostRecentFileMTime = 0;

		$iterator = new RecursiveDirectoryIterator(".");
		foreach ($iterator as $fileinfo) {
			if ($fileinfo->isFile()) {
				if ($fileinfo->getMTime() > $mostRecentFileMTime) {
					$mostRecentFileMTime = $fileinfo->getMTime();
					$mostRecentFilePath = $fileinfo->getFilename();
				}
			}
		}
		return date("d M. Y H:i:s T", filemtime($mostRecentFilePath));
	}
	
	function get_open_tasks()
	{
		$user = $_SESSION["username"];
		$sql = mysqli_query($GLOBALS['users_connect'], "SELECT * FROM tasks WHERE STATUS = 'IN PROGRESS' AND USER='$user'");
		return mysqli_num_rows($sql);
	}
	
	function rb_checked($rb, $option)
	{
		$rb_value = (isset($_GET['show'])) ? $_GET['show'] : "all";
		if ($option==1) { return ($rb == $rb_value) ? "checked" : ""; }
		if ($option==2) { return ($rb == $rb_value) ? "bold" : ""; }
	}
	
	function getTestStatusCount($source, $status)
	{
		$source = ($source == "TS") ? "testsuites" : "testcases";
		$sql = mysqli_query($GLOBALS['systemx_connect'], "SELECT * FROM $source WHERE RUN_STATUS=$status");
		$result = mysqli_num_rows($sql);
		return $result;
	}
	
	function getPendingJobInfo($sxVersion, $set)
	{
		$url = "http://rommel.lamanilao:e8998fdfaef8197b7c8f1892692bebce@cistaging:8080/view/Ranorex/job/System%20X%20$sxVersion%20Ranorex%20Test%20$set/api/json";
		$ch = curl_init();
		curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
		curl_setopt($ch, CURLOPT_URL, $url);
		curl_setopt($ch,CURLOPT_USERAGENT,'Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.13) Gecko/20080311 Firefox/2.0.0.13');
		
		$result = curl_exec($ch);
		if(curl_errno($ch))
		{
			echo 'Error: ' . curl_error($ch);
			curl_close($ch);
		}
		else
		{
			curl_close($ch);
			if (strpos($result, 'ERROR') !== false)
			{
				$arrResult = explode("<pre>",$result);
				$arrResult = explode("</pre>",$arrResult[1]);
				$response = trim($arrResult[0]);
			}
			else
			{
				if (strpos($result, 'why') !== false)
				{
					$obj = json_decode($result);
					$response = $obj->nextBuildNumber . ":" . $obj->queueItem->why;
				}
				else
				{
					$response = "";
				}
			}
			return $response;
		}
	}
	
	function getJenkinsAPI($sxVersion, $set, $data)
	{
		$url = "http://rommel.lamanilao:e8998fdfaef8197b7c8f1892692bebce@cistaging:8080/view/Ranorex/job/System%20X%20$sxVersion%20Ranorex%20Test%20$set/lastBuild/api/json?tree=$data";
		//$url = "http://cistaging:8080/view/Ranorex/job/System%20X%20$sxVersion%20Ranorex%20Test%20$set/lastBuild/api/json?tree=$data";
		$ch = curl_init();
		curl_setopt($ch, CURLOPT_HEADER, 0);
		curl_setopt($ch,CURLINFO_HEADER_OUT,true);
		//curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
		curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
		curl_setopt($ch, CURLOPT_URL, $url);
		curl_setopt($ch,CURLOPT_USERAGENT,'Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.13) Gecko/20080311 Firefox/2.0.0.13');
		
		$result = curl_exec($ch);
		if(curl_errno($ch))
		{
			echo 'Error: ' . curl_error($ch);
			curl_close($ch);
		}
		else
		{
			curl_close($ch);
			if (strpos($result, 'ERROR') !== false)
			{
				$arrResult = explode("<pre>",$result);
				$arrResult = explode("</pre>",$arrResult[1]);
				$response = trim($arrResult[0]);
			}
			else
			{
				$obj = json_decode($result);
				$response = $obj->$data;
			}
			return $response;
		}
	}
	
	function checkJenkinsAPIResponse($username="", $token="")
	{
		$ch = curl_init();
		
		if ((empty($username)) && (empty($token)))
		{
			$token = "e8998fdfaef8197b7c8f1892692bebce";
			$username = "rommel.lamanilao";
		}
		$url = "http://$username:$token@cistaging:8080/view/Ranorex/api/json";
		curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
		curl_setopt($ch, CURLOPT_URL, $url);
		curl_setopt($ch,CURLOPT_USERAGENT,'Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.13) Gecko/20080311 Firefox/2.0.0.13');
		curl_setopt($ch,CURLINFO_HEADER_OUT,true);
		$result = curl_exec($ch);
		if(curl_errno($ch))
		{			
			return "<b class='blink'>ERROR:</b> " . curl_error($ch);
			curl_close($ch);
		}
		else
		{
			curl_close($ch);
			if (strpos($result, 'ERROR') !== false)
			{
				$arrResult = explode("<pre>",$result);
				$arrResult = explode("</pre>",$arrResult[1]);
				$response = "<b class='blink'>FEIL:</b> " . str_replace(": rommel.lamanilao", "",(trim($arrResult[0])));
			}
			else
			{
				$response = "SUCCESS";
			}
			return $response;
		}
	}
	
	function getJenkinsAPI_($sxVersion, $set, $data)
	{
		$context = stream_context_create(array('http' => array('header'=>'Connection: close\r\n')));
		$url = "http://rommel.lamanilao:e8998fdfaef8197b7c8f1892692bebce@cistaging:8080/view/Ranorex/job/System%20X%20$sxVersion%20Ranorex%20Test%20$set/lastBuild/api/json?tree=$data";
		$contents = file_get_contents($url,false,$context);
		$json = json_decode($contents, true);
		return $json[$data];
	}
	
	function getPreviousBuildStatus($sxVersion, $set, $buildID)
	{
		$url = "http://rommel.lamanilao:e8998fdfaef8197b7c8f1892692bebce@cistaging:8080/view/Ranorex/job/System%20X%20$sxVersion%20Ranorex%20Test%20$set/$buildID/api/json";
		$contents = file_get_contents($url);
		$json = json_decode($contents, true);
		return $json['result'];
	}
	
	function startBuild($sxVersion, $set, $username = "", $token = "")
	{
		if ((empty($username)) && (empty($token)))
		{
			$token = "e8998fdfaef8197b7c8f1892692bebce";
			$username = "rommel.lamanilao";
		}
		$ch = curl_init();
		curl_setopt($ch, CURLOPT_URL, "http://$username:$token@cistaging:8080/view/Ranorex/job/System%20X%20$sxVersion%20Ranorex%20Test%20$set/build");
		curl_setopt($ch, CURLOPT_HEADER, 0);
		curl_setopt($ch, CURLOPT_POST, 1);
		curl_setopt($ch,CURLINFO_HEADER_OUT,true);
		curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
		$result = curl_exec($ch);
		if(curl_errno($ch))
		{
			return "<b class='blink'>FEIL:</b> " . curl_error($ch);
		}
		else
		{
			curl_close($ch);
			if (strpos($result, 'ERROR') !== false)
			{
				$arrResult = explode("<pre>",$result);
				$arrResult = explode("</pre>",$arrResult[1]);
				$response = "<b class='blink'>FEIL:</b> " . (trim($arrResult[0]));
			}
			else
			{
				$response = "SUCCESS";
			}
			return $response;
		}
		curl_close($ch);
	}
	
	function stopBuild($sxVersion, $set, $id, $username = "", $token = "")
	{
		if ((empty($username)) && (empty($token)))
		{
			$token = "e8998fdfaef8197b7c8f1892692bebce";
			$username = "rommel.lamanilao";
		}
		$ch = curl_init();
		curl_setopt($ch, CURLOPT_URL, "http://$username:$token@cistaging:8080/view/Ranorex/job/System%20X%20$sxVersion%20Ranorex%20Test%20$set/$id/stop");
		curl_setopt($ch, CURLOPT_HEADER, 0);
		curl_setopt($ch, CURLOPT_POST, 1);
		curl_setopt($ch,CURLINFO_HEADER_OUT,true);
		curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
		$result = curl_exec($ch);
		if(curl_errno($ch))
		{
			return 'Feil: ' . curl_error($ch);
		}
		else
		{
			curl_close($ch);
			if (strpos($result, 'ERROR') !== false)
			{
				$arrResult = explode("<pre>",$result);
				$arrResult = explode("</pre>",$arrResult[1]);
				$response = "<b class='blink'>FEIL: </b> " . (trim($arrResult[0]));
			}
			else
			{
				$response = "SUCCESS";
			}
			return $response;
		}
		curl_close($ch);
	}
	
	function getProgress($sxVersion, $set, $id)
	{
		$ch = curl_init();
		curl_setopt($ch, CURLOPT_URL, "http://rommel.lamanilao:e8998fdfaef8197b7c8f1892692bebce@cistaging:8080/view/Ranorex/job/System%20X%20$sxVersion%20Ranorex%20Test%20$set/$id/api/json?tree=executor[progress]");
		curl_setopt($ch, CURLOPT_HEADER, 0);
		curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
		curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
		$result = curl_exec($ch);
		
		if(curl_errno($ch))
		{
			return 'NULL';
			curl_close($ch);
		}
		else
		{
			curl_close($ch);
			if (strpos($result, 'null') !== false)
			{
				return '100';
			}
			else
			{
				$obj = json_decode($result);
				return $obj->executor->progress;		
			}
		}
		
		/*curl_close($ch);
		$obj = json_decode($result);
		
		return $obj->executor->progress;*/
	}
	
	function formatMilliseconds($milliseconds) 
	{
		$seconds = floor($milliseconds / 1000);
		$minutes = floor($seconds / 60);
		$hours = floor($minutes / 60);
		//$milliseconds = $milliseconds % 1000;
		$seconds = $seconds % 60;
		$minutes = $minutes % 60;
		
		if ($milliseconds >= 60000)
		{
			$format = '%u timer og %02u minutter';
			$time = sprintf($format, $hours, $minutes, $seconds);
		}
		else
		{
			$format = '%02u sekunder';
			$time = sprintf($format, $seconds);
		}
		return $time;
	}
	
	//To get all the testsuites
	function getTestsuitesByGroup($SXVersion, $group)
	{
		$database = "sxtest_" . str_replace(".", "", $SXVersion);
		$hostname = '192.168.10.61';
		$username = 'ta_admin';
		$password = 'dhocc648';
		$sx_connect = mysqli_connect($hostname, $username, $password, $database);
		
		$sql = mysqli_query($sx_connect, "SELECT TS_ID FROM testsuites WHERE TEST_GROUP='$group' ORDER BY TS_ID ASC");
		$result = array();
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				array_push($result, $row[0]);
			}
		}
		return $result;
	}
	
	//Validate if the given username is in the database
	function is_username_exist($username)
	{
		$sql = mysqli_query($GLOBALS['users_connect'], "SELECT * FROM designers WHERE USERNAME='$username'");
		return (mysqli_num_rows($sql) > 0);
	}
	
	function get_role_name($role_id)
	{
		$sql = mysqli_query($GLOBALS['users_connect'], "SELECT ROLE FROM roles WHERE ROLE_ID='$role_id'") or die(mysqli_error());
		$result = "";
		
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[0];
			}
		}
		return $result;
	}
	function get_role_info($role_id, $info)
	{
		$sql = mysqli_query($GLOBALS['users_connect'], "SELECT * FROM roles WHERE ROLE_ID='$role_id'");
		$result = "";
		if (mysqli_num_rows($sql) > 0)
		{
			while($row = mysqli_fetch_array($sql))
			{
				$result = $row[$info];
			}
		}
		return $result;
	}
	
?>