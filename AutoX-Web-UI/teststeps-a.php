<?php
	$tc_id = $_GET['tc-id'];
	$ts_id = $_GET['ts-id'];
	$new_step_num = get_new_teststep_num($ts_id, $tc_id);
	
	$role = get_user_info($_SESSION["username"], 7);
	$isStepAdd = (get_role_info($role, 23)) ? "" : "hidden";
	$isStepEdit = (get_role_info($role, 25)) ? "" : "hidden";
	$isStepDelete = (get_role_info($role, 26)) ? "" : "hidden";
	$editModal = "notModal";
	$editClass = "muted not-allowed";
	if (get_role_info($role, 25))
	{
		$editModal = "modal";
		$editClass = "primary";
	}
?>

<div class="card-body">
	<div class="card">
		
		<div class="btn-toolbar" role="toolbar">
			<div class="btn-group" role="group" aria-label="First group" style="border:0px">
				<button type="button" title="Registrere ny teststeg" class="btn btn-inverse-light icon-btn modalClass <?php echo $isStepAdd; ?>" style="padding:5px" data-toggle='modal' data-icon="new-box" data-step-number="<?php echo $new_step_num; ?>" data-target='#modalStepRegForm' id="mnuBtnNew"><span class="mdi mdi-new-box mdi-24px text-warning"></span> Ny registrering</button>
				<button type="button" title="Aktiver/Deaktiver teststeg" class="btn btn-inverse-light icon-btn modalClass <?php echo $isStepEdit; ?>" style="padding:5px" id="mnuBtnEnable" data-icon="crosshairs-gps" data-target='#modalStepActivate'><span class="mdi mdi-crosshairs-gps mdi-24px text-warning"></span> Aktiver/Deaktiver teststeg</button>
				<button type="button" title="Kopi valgt registrering" class="btn btn-inverse-light icon-btn modalClass <?php echo $isStepAdd; ?>" style="padding:5px" id="mnuBtnCopy" data-icon="content-copy"><span class="mdi mdi-content-copy mdi-24px text-warning"></span> Kopi</button>
				<button type="button" title="Flytt valgt registrering" class="btn btn-inverse-light icon-btn modalClass <?php echo $isStepEdit; ?>" style="padding:5px" id="mnuBtnMove" data-icon="folder-move"><span class="mdi mdi-folder-move mdi-24px text-warning"></span> Flytt</button>
				<button type="button" title="Aktiver/Deaktiver testcase" class="btn btn-inverse-light icon-btn modalClass <?php echo $isStepEdit; ?>" style="padding:5px" data-icon="link-variant-off" id="mnuBtnToggleActivate"><span class="mdi mdi-link-variant-off mdi-24px text-warning"></span> Aktiver/Deaktiver testcase</button>
				<button type="button" title="Slett valgt teststeg" class="btn btn-inverse-light icon-btn modalClass <?php echo $isStepDelete; ?>" style="padding:5px" data-icon="delete" id="mnuBtnDelete"><span class="mdi mdi-delete mdi-24px text-warning"></span> Slett</button>
				<button type="button" title="Vis nyeste innhold i aktiv side..." class="btn btn-inverse-light icon-btn" style="padding:5px" id="mnuBtnRefresh"><span class="mdi mdi-refresh mdi-24px text-warning"></span> Refresh</button>
			</div>
		</div>
		
		<div class="tc-info">
			<h6 class="card-title embossed">Testcase informasjon</h6>
			<div style="margin-left:10px">
				<?php
					$tc_title = get_testcase_info(null, 3, $tc_id, $ts_id);
					$changed_by = get_testcase_info(null, 6, $tc_id, $ts_id);
					$changed_by = get_user_info($changed_by, 1) . " " . get_user_info($changed_by, 2);

					$last_date_modified = get_testcase_info(null, 5, $tc_id, $ts_id);
					setlocale(LC_TIME, 'norwegian');
					$day = utf8_encode(ucfirst(strftime('%A',strtotime($last_date_modified))));
					$last_date_modified = $day . strftime(' %d. %B %Y kl. %H:%M',strtotime($last_date_modified));
					
					$last_commit_date = get_testcase_info(null, 14, $tc_id, $ts_id);
					$day = utf8_encode(ucfirst(strftime('%A',strtotime($last_commit_date))));
					$last_commit_date = (empty($last_commit_date)) ? "<i class='text-muted'>Ikke commit</i>" : $day . strftime(' %d. %B %Y kl. %H:%M',strtotime($last_commit_date));
					
					$is_active = (get_testcase_info(null, 7, $tc_id, $ts_id)) ? "Ja" : "<b style='color:#DF0101'>Nei</b>";
					$comments = get_testcase_info(null, 10, $tc_id, $ts_id);
					$testsuite_title = get_testsuite_info($ts_id, 2);
					$ticket = get_testcase_info(null, 11, $tc_id, $ts_id);
					if (empty($comments)) { $comments = "<i class='text-muted'>Ingen kommentar</i>"; }
					
					$testStatus = (get_testcase_info(null, 12, $tc_id, $ts_id)) ? "<b style='color:#5FB404'>OK</b>" : "<b style='color:#DF0101'>FEIL</b>";
					$testStatus = (get_testcase_info(null, 12, $tc_id, $ts_id) == null) ? "<i class='text-muted'>Ikke kjørt</i>" : $testStatus;
					$jira_ticket = (empty($ticket)) ? "<i class='text-muted'>Ingen ticket</i>" : "<a href='http://jiracon-appsrv:8080/browse/$ticket' target='_blank'>$ticket</a>";
					echo "
						<div style='width:100%'>
							<div style='width:11%;float:left'>
								<p class='embossed'>Testsuite:</p>
							</div>
							<div style='width:44%;float:left'>
								<p class='embossed text-primary'><b class='pointer' id='pTestsuite' data-tc-id='$tc_id'>TS-$ts_id: $testsuite_title</b></p>
							</div>
							<div style='width:13%;float:left'>
								<p class='embossed'>Sist endret av:</p>
							</div>
							<div style='width:32%;float:left'>
								<p class='embossed'><b>$changed_by</b></p>
							</div>
						</div>
						
						<div style='width:100%'>
							<div style='width:11%;float:left'>
								<p class='embossed'>Tittel:</p>
							</div>
							<div style='width:44%;float:left'>
								<p class='embossed tc-title'><b>TC-$tc_id: $tc_title</b></p>
							</div>
							<div style='width:13%;float:left'>
								<p class='embossed'>Dato sist endret:</p>
							</div>
							<div style='width:32%;float:left'>
								<p class='embossed'><b>$last_date_modified</b></p>
							</div>
						</div>
						
						<div style='width:100%'>
							<div style='width:11%;float:left'>
								<p class='embossed'>Aktiv:</p>
							</div>
							<div style='width:44%;float:left'>
								<p class='embossed tc-active'><b>$is_active</b></p>
							</div>
							<div style='width:13%;float:left'>
								<p class='embossed'>Kommentar:</p>
							</div>
							<div style='width:32%;float:left'>
								<p class='embossed tc-comments'><b>$comments</b></p>
							</div>
						</div>
						
						<div style='width:100%'>
							<div style='width:11%;float:left'>
								<p class='embossed'>Teststatus:</p>
							</div>
							<div style='width:44%;float:left'>
								<p class='embossed'><b>$testStatus</b></p>
							</div>
							<div style='width:13%;float:left'>
								<p class='embossed'>GitHub Commit:</p>
							</div>
							<div style='width:32%;float:left'>
								<p class='embossed'><b>$last_commit_date</b></p>
							</div>
						</div>
						
						<div style='width:100%'>
							<div style='width:11%;float:left'>
								<p class='embossed'>JIRA Ticket:</p>
							</div>
							<div style='width:44%;float:left'>
								<p class='embossed'><b>$jira_ticket</b></p>
							</div>
						</div>
					";
					/*
					echo "<div style='width:15%;float:left'>
							<p class='embossed'>Testsuite:</p></div>";
					echo "<div style='width:85%;float:right'>
							<p class='embossed text-primary'><b class='pointer' id='pTestsuite' data-tc-id='$tc_id'>TS-$ts_id: $testsuite_title</b></p></div>";
							
					echo "<div style='width:15%;float:left'>
							<p class='embossed'>Tittel:</p></div>";
					echo "<div style='width:85%;float:right'>
							<p class='embossed tc-title'><b>TC-$tc_id: $tc_title</b></p></div>";
					
					echo "<div style='width:15%;float:left'>
							<p class='embossed'>Sist endret av:</p></div>";
					echo "<div style='width:85%;float:right'>
							<p class='embossed'><b>$changed_by</b></p></div>";
							
					echo "<div style='width:15%;float:left'>
							<p class='embossed'>Dato sist endret:</p></div>";
					echo "<div style='width:85%;float:right'>
							<p class='embossed'><b>$last_date_modified</b></p></div>";
							
					echo "<div style='width:15%;float:left'>
							<p class='embossed'>Aktiv:</p></div>";
					echo "<div style='width:85%;float:right'>
							<p class='embossed tc-active'><b>$is_active</b></p></div>";
							
					echo "<div style='width:15%;float:left'>
							<p class='embossed'>Kommentar:</p></div>";
					echo "<div style='width:85%;float:right'>
							<p class='embossed tc-comments'><b>$comments</b></p></div>";
					
					echo "<div style='width:15%;float:left'>
							<p class='embossed'>JIRA Ticket:</p></div>";
					$jira_ticket = (empty($ticket)) ? "<i class='text-muted'>Ingen ticket</i>" : "<a href='http://jiracon-appsrv:8080/browse/$ticket' target='_blank'>$ticket</a>";
					echo "<div style='width:85%;float:right'>
							<p class='embossed'><b>$jira_ticket</b></p></div>";
					*/
				?>
			</div>
		</div>
		
		<table class="table table-bordered">
			<thead>
				<tr class="bg-color-light">
					<th width="2%"><div class='round'><input type='checkbox' id="chkAction"><label for='chkAction'></label></div></th>
					<th width="6%" class="center">Steg nr.</th>
					<th width="10%">Steg kategori</th>
					<th width="10%">Test kommando</th>
					<th width="32%">Parameter</th>
					<th width="36%" class="center">Beskrivelse</th>
					<th width="2%"><i class="mdi mdi-login-variant icon-sm"></i></th>
					<th width="2%"></th>
				</tr>
			</thead>
			<tbody>
				<?php
					$ts_title = get_testsuite_info($ts_id, 2);
					$query = "SELECT * FROM teststeps WHERE ts_id=$ts_id AND tc_id=$tc_id AND step_number >0 ORDER BY step_number ASC";
					$result = mysqli_query($systemx_connect,$query)or die(mysqli_error());
					
					while($row = mysqli_fetch_array($result, MYSQL_NUM))
					{
						$id = $row[0];
						$sub_category = get_command_info($row[5], 3);
						$auto_login = ($row[10]) ? "<span class='mdi mdi-crosshairs-gps text-error icon-xmd'></span>" : "";
						
						$arr_test_command = extract_test_data($row[7]);
						if ($row[4] == "Application") {$arr_test_command[1] = 0; }
						$parent_object = get_parent_info($arr_test_command[0], "PARENT_NAME");
						$child_object = get_child_info($arr_test_command[1], "CONTROL_NAME");
						$test_command = str_replace("[" . $arr_test_command[0] . "][" . $arr_test_command[1] . "]", "[$parent_object][$child_object]", $row[7]);
						$visible_data = $test_command;
						
						$disabled = ($row[9]) ? "text-disabled" : "";
						if ($row[4] == "Application")
						{
							$visible_data = str_replace($arr_test_command[0], "Eksternt Program", $row[7]);
							$test_command = $visible_data;
							if ($sub_category == "Application_Delay")
							{
								$visible_data = $row[7];
								$test_command = $visible_data;
							}
						}
						elseif ($row[4] == "Checkpoint")
						{
							if ($sub_category == "Object_Exist")
							{
								$is_visible = $arr_test_command[2];
								$visible_data = str_replace("[true]", "", $test_command);
								$visible_data = str_replace("[false]", "", $visible_data);
								if (count($arr_test_command) == 4)
								{
									$is_visible = $arr_test_command[3];
								}
								$visible = ($is_visible == "true") ? "True" : "False";
								$visible_data = "<b>Objektet/data som skal sjekkes:</b><br>$visible_data<br><br><b>Synlig:</b><br>$visible";
							}
							elseif ($sub_category == "Object_Text")
							{
								$visible_data = str_replace("[" . $arr_test_command[2] . "]", "", $visible_data);
								$visible_data = "<b>Objektet som skal sjekkes:</b><br>$visible_data<br><br><b>Parameter:</b><br>" . nl2br($arr_test_command[2]);
							}
						}
						elseif (($row[4] == "Keyboard") || ($row[4] == "Mouse"))
						{
							$split_test_data = explode(";", $row[7]);
							$visible_data = "";
							foreach ($split_test_data as &$data)
							{
								$arr_test_data = extract_test_data($data);
								$window_title = get_parent_info($arr_test_data[0], "PARENT_NAME");
								$alias = get_child_info($arr_test_data[1], "CONTROL_NAME");
								$extra = (isset($arr_test_data[2]) ? $arr_test_data[2] : "");
								$extra = (empty($extra) ? "" : "[$extra]");
								$visible_data = empty($visible_data) ? $visible_data = $visible_data : $visible_data = $visible_data . ";<br>";
								$visible_data = "$visible_data" . "[$window_title][$alias]$extra";
								
							}
							$test_command = $visible_data;
						}
						/*if ($row[4] == "Application")
						{
							$test_data = str_replace($arr_test_data[0], "Eksternt Program", $row[7]);
						}
						elseif ($row[4] == "Checkpoint")
						{	
							if (count($arr_test_data) == 4)
							{
								$visible_data = $visible_data . "[" . $arr_test_data[2] . "]";
							}
							if (($sub_category == "Press_Keys_Exist") || ($sub_category == "Program_Crash"))
							{
								$test_data = $visible_data;
							}
							elseif ($sub_category == "Object_Exist")
							{
								$visibility = "False";
								if ($arr_test_data[2] == "true")
								{
									$visibility = "True";
								}
								$test_data = "<b>Objektet/data som skal sjekkes:</b><br>$visible_data<br><br><b>Synlig:</b><br>$visibility";
								$visible_data = $visible_data . "[" . $arr_test_data[2] . "]";
							}
							elseif ($sub_category == "Object_Text")
							{
								$test_data = "<b>Objektet som skal sjekkes:</b><br>$visible_data<br><br><b>Parameter:</b><br>" . nl2br($row[8]);
							}
							elseif ($sub_category == "Click_Exist")
							{
								if (!empty($row[8]))
								{
									$row[8] = "[$row[8]]";
								}
								$test_data = "$visible_data$row[8]";
							}
						}
						elseif (($row[4] == "Keyboard") || ($row[4] == "Mouse"))
						{
							$split_test_data = explode(";", $row[7]);
							$test_data = "";
							foreach ($split_test_data as &$data)
							{
								$arr_test_data = extract_test_data($data);
								$window_title = get_object_info("object_id", $arr_test_data[0], 1);
								$alias = get_object_info("object_id", $arr_test_data[1], 4);
								$extra = (isset($arr_test_data[2]) ? $arr_test_data[2] : "");
								$extra = (empty($extra) ? "" : "[$extra]");
								$test_data = empty($test_data) ? $test_data = $test_data : $test_data = $test_data . ";";
								$test_data = "$test_data" . "[$window_title][$alias]$extra";
							}
						}*/
						echo "
							<tr class='$disabled test-step' data-step-number='$row[3]' title='Dobbeltklikk for å sette inn nytt trinn'>
								<td class='center'><div class='round' style='margin-left:10px'><input type='checkbox' id='chkRow$row[0]' value='$id' class='cbox-default'><label for='chkRow$row[0]'></label></div></td>
								<td class='center'>$row[3]</td>
								<td>$row[4]</td>
								<td>$sub_category</td>
								<td>$visible_data</td>
								<td class='va-top'>$row[6]</td>
								<td class='center'>$auto_login</td>
								<td class='center'><i data-id='$row[0]' data-step-num='$row[3]' data-category='$row[4]' data-sub-category='$row[5]' data-description='$row[6]' data-test-data='$test_command' data-expected-result='$row[8]' data-auto-login='$row[10]' class='mdi mdi-pencil-box icon-xmd text-$editClass edit-class modalClass' data-category='testcase' data-icon='pencil-box' data-toggle='$editModal' data-target='#modalStepRegForm' title='Redigere teststeg'></i></td>
							</tr>
						";
					}
				?>
			</tbody>
		</table>
	</div>
	<span style="font-size:13px">
	<?php
		$row_result = "<span class='mdi mdi-crosshairs-gps text-error icon-sm'></span> Deaktiver System X automatisk pålogging når feil oppstår";
		if (mysqli_num_rows($result) == 0)
		{
			//$row_result = "<span class='mdi mdi-alert text-error icon-md'></span> <b class='text-error'>Testcase ikke funnet eller inneholder ikke teststeg</b>";
		}
		echo $row_result;
	?>
	</span>
</div>
<?php
	include "modals.php";
?>