<?php
	if (isset($_GET['id']))
	{
		$id = $_GET['id'];
		$modal_content1 = "hidden";
		$modal_content2 = "hidden";
		switch ($id)
		{
			case 1:
				$category = "testsuite";
				$modal_content1 = "";
				break;
			case 2:
				$category = "testcase";
				$modal_content2 = "";
				break;
		}
	}
?>
<html lang="en">
<head>
	<link rel="stylesheet" href="https://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">
</head>
<body>
<!-- ***************** DELETE ITEM **************** -->
<div class="modal fade" id="modalDelete" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle"><span class="mdi mdi-alert text-warning icon-md"></span> Bekreft</h5>
			</div>
			<div class="modal-body">
				<span class="text-13">Slette valgte registreringer kan ikke fortrykkes. Fortsette?</span>
			</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-danger" id="btnConfirmDelete">Bekreft</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
			</div>
		</div>
	</div>
</div>

<!-- ***************** ACTIVATE/DEACTIVATE TESTCASES **************** -->
<div class="modal fade" id="modalTcActivate" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">Aktiver/Deaktiver testcase</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmTcActivate">
				<div class="modal-body">
						<label class="text-13">Med utvalgte testcaser:</label>
						<div>
							<input type="radio" id="radio_enable" checked value=1 name="n_activate"> <label for="radio_enable" class="text-13"> Kjør skriptet</label>
						</div>
						<div>
							<input type="radio" id="radio_disable" value=0 name="n_activate"> <label for="radio_disable" class="text-13"> Ikke kjør skriptet</label>
						</div>
				</div>
				<div class="modal-footer hdr">
					<button type="button" class="btn btn-primary" id="btnConfirmActivate" data-form="#frmTcActivate" data-button-action="activate_testcase">Bekreft</button>
					<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
				</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** COPY ITEM -- TESTCASES **************** -->
<div class="modal fade" id="modalCopy" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">Kopiere <?php echo $category; ?></h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmCopy">
				<div class="modal-body <?php echo "$modal_content2"; ?>">
						<label for="testsuite" class="text-13">Velg en testsuite kopiere til</label>
						<div>
							<input type="hidden" id="actionCopy">
							<select name="n_ts_id" id="testsuite" class="form-control form-control-sm">
							<?php
								$query = "SELECT ts_id, title FROM testsuites ORDER BY title ASC";
								$result = mysqli_query($systemx_connect,$query)or die(mysqli_error());
								while($row = mysqli_fetch_array($result, MYSQL_NUM))
								{
									echo "<option value='$row[0]'>$row[1]</option>";
								}
							?>
							</select>
						</div>
				</div>
				<div class="text-13 modal-body <?php echo "$modal_content1"; ?>">
						<span>Kopi valgte testsuiter?</span>
						<div>
							<input type="checkbox" name="n_include" id="include_tc"> <label for="include_tc">Kopi også testcaser</label>
						</div>
				</div>
				<div class="modal-footer hdr">
					<button type="button" class="btn btn-primary" id="btnConfirmCopy" data-form="#frmCopy">Bekreft</button>
					<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
				</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** MOVE TESTCASES **************** -->
<div class="modal fade" id="modalMove" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">Flytte testcaser</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmMove">
				<div class="modal-body">
						<label for="testsuite" class="text-13">Velg en mål testsuite</label>
						<div>
							<select name="n_ts_id" id="cboTestsuites" class="form-control form-control-sm">
							<?php
								$query = "SELECT ts_id, title FROM testsuites WHERE ts_id != $id ORDER BY title ASC";
								$result = mysqli_query($systemx_connect,$query)or die(mysqli_error());
								while($row = mysqli_fetch_array($result, MYSQL_NUM))
								{
									echo "<option value='$row[0]'>$row[1]</option>";
								}
							?>
							</select>
						</div>
				</div>
				<div class="modal-footer hdr">
					<button type="button" class="btn btn-primary" id="btnConfirmMove" data-form="#frmMove" data-button-action="move_testcases">Bekreft</button>
					<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
				</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** LOADING GIF **************** -->
<div class="modal-loading" role="dialog"></div>

<!-- ***************** ERROR DIALOG **************** -->
<div class="modal fade" id="errModal" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">Informasjon</h5>
			</div>
			<div id="infoModal"></div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-secondary" data-dismiss="modal">Lukk</button>
			</div>
			
		</div>
	</div>
</div>

<!-- ***************** ADD/EDIT FORM -- TESTSUITES **************** -->
<div class="modal fade" id="modalTsRegForm" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">Endre testsuite</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmTsRegForm">
			<div class="modal-body">
				
					<div class="form-group">
						<label for="ts_id" class="col-sm-3 col-form-label">ID</label>
						<div class="col-sm-9">
							<input type="text" readonly class="form-control center" id="ts_id" style="width:80px" name="n_ts_id">
							<input type="hidden" id="actionx">
							<input type="hidden" name="n_rec_id" id="rec_id">
						</div>
					</div>
					<div class="form-group">
						<label for="title" class="col-sm-3 col-form-label">Tittel</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="title" style="width:445px" required name="n_ts_title">
						</div>
					</div>
					<div class="form-group">
						<label for="description" class="col-sm-3 col-form-label">Beskrivelse</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="description" style="width:445px" required name="n_ts_desc">
						</div>
					</div>
					<div class="form-group">
						<label for="description" class="col-sm-3 col-form-label">Testgruppe</label>
						<div class="col-sm-9">
							<select id="cboTestGroup" style="width:445px" class="form-control form-control-sm" required name="n_testgroup">
								<option selected disabled></option>
								<option>A</option>
								<option>B</option>
								<option>C</option>
								<option>D</option>
								<option>E</option>
								<option>X</option>
							</select>
						</div>
					</div>
				<div id="info"></div>
			</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-danger" id="btnDelete" data-button-action="ts_delete">Slett</button>
				<button type="button" class="btn btn-warning hidden" id="btnDeactivateTS">Deaktiver</button>
				<button type="button" class="btn btn-primary" id="btnSave" data-form="#frmTsRegForm">Lagre</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
			</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** ADD/EDIT FORM -- TESTCASES **************** -->
<div class="modal fade" id="modalTcRegForm" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle"></h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmTcRegForm">
				<div class="modal-body">
					<div class="form-group">
						<label for="title" class="col-sm-3 col-form-label">* Tittel</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="title" style="width:445px" required name="n_tc_title">
							<input type="hidden" id="tc_id" name="n_tc_id">
							<input type="hidden" id="actionx">
							<input type="hidden" name="n_rec_id" id="rec_id">
						</div>
					</div>
					<div class="form-group">
						<label for="testsuite" class="col-sm-3 col-form-label">Testsuite</label>
						<div class="col-sm-9">
							<select name="n_ts_id" id="testsuite" style="width:445px" class="form-control form-control-sm">
							<?php
								$query = "SELECT ts_id, title FROM testsuites ORDER BY title ASC";
								$result = mysqli_query($systemx_connect,$query)or die(mysqli_error());
								while($row = mysqli_fetch_array($result, MYSQL_NUM))
								{
									echo "<option value='$row[0]'>$row[1]</option>";
								}
							?>
							</select>
						</div>
					</div>
					<div class="form-group">
						<label for="comments" class="col-sm-3 col-form-label">Kommentar</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="comments" style="width:445px" name="n_comments">
						</div>
					</div>
					<div class="form-group">
						<label for="txtJira" class="col-sm-3 col-form-label">JIRA Ticket</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="txtJira" style="width:85px" name="n_Jira" placeholder="SX-####">
						</div>
					</div>
					<div id="infoMsg" class="text-warning hidden">Denne testcase er deaktivert</div>
					<div id="info"></div>
				</div>
				<div class="modal-footer hdr">
					<button type="button" class="btn btn-danger" id="btnDelete" data-button-action="tc_delete">Slett</button>
					<button type="button" class="btn btn-warning" id="btnActivate">Aktivere</button>
					<button type="button" class="btn btn-primary" id="btnSave" data-form="#frmTcRegForm">Lagre</button>
					<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
				</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** ADD/EDIT FORM -- TESTSTEPS **************** -->
<?php
	if (isset($_GET['tc-id']))
	{
		$tc_id = $_GET['tc-id'];
		$ts_id = $_GET['ts-id'];
		$tc_title = get_testcase_info(null, 3, $tc_id, $ts_id);
		$new_step_num = get_new_teststep_num($ts_id, $tc_id);
	}
?>
<div class="modal fade" id="modalStepRegForm" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">Endre test steg</h5>
			</div>
			<span class="modal-sub-header"><?php echo "TC-$tc_id: $tc_title"; ?></span>
			<form class="forms-sample" action="" method='POST' id="frmStegRegForm">
			<div class="modal-body">
				
					<div class="form-group">
						<label for="stepNum" class="col-sm-3 col-form-label">Steg nr.</label>
						<div class="col-sm-9">
							<input type="text" readonly class="form-control center" id="stepNum" style="width:80px" name="n_step_num" value="<?php echo $new_step_num; ?>">
						</div>
					</div>
					<div class="form-group">
						<label for="title" class="col-sm-3 col-form-label">*Kategori</label>
						<div class="col-sm-9">
							<select name="n_category" id="cboCategory" style="width:445px" class="form-control form-control-sm" required>
								<option selected disabled></option>
								<?php
									$query = "SELECT DISTINCT command_category FROM test_commands ORDER BY command_category ASC";
									$result = mysqli_query($systemx_connect,$query)or die(mysqli_error());
									while($row = mysqli_fetch_array($result, MYSQL_NUM))
									{
										echo "<option>$row[0]</option>";
									}
								?>
							</select>
						</div>
					</div>
					<div class="form-group">
						<label for="cboSubCategory" class="col-sm-4 col-form-label">*Test kommando</label>
						<div class="col-sm-9" id="divSubCategory">
							<select name="n_sub_category" id="cboSubCategory" style="width:445px" class="form-control form-control-sm" disabled>
								<option></option>
							</select>
						</div>
					</div>
					<div class="form-group">
						<label for="txtCommand" class="col-sm-4 col-form-label">*Parameter</label>
						<div class="col-sm-12">
							<textarea class="form-control" id="txtCommand" style="width:445px" required name="n_test_command"></textarea>
							<div class="text-13 float-right pointer text-primary" id="divTCGenerator">Test Command Generator <i class='mdi mdi-open-in-new icon-sm'></i></div>
						</div>
					</div>
					<div class="form-group">
						<label for="txtDescription" class="col-sm-3 col-form-label">Beskrivelse</label>
						<div class="col-sm-9">
							<textarea class="form-control" id="txtDescription" style="width:445px" name="n_description"></textarea>
						</div>
					</div>
					<div class="form-group">
						<div class="col-sm-12">
							<div class='round'><input type="checkbox" id="chkAutoLogin" readonly style="line-height:1.6em" name="n_auto_login"><label for='chkAutoLogin'></label></div><label for="chkAutoLogin" style="line-height:1.6em;position:relative;top:-25px;left:30px" class="pointer">Deaktiver System X automatisk pålogging når feil oppstår</label>
						</div>
					</div>
					<div id="info"></div>
				</div>
				<div class="modal-footer hdr">
					<button type="button" class="btn btn-danger" id="btnDelete" data-button-action="step_delete">Slett</button>
					<button type="button" class="btn btn-primary" id="btnSave" data-form="#frmStegRegForm">Lagre</button>
					<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
				</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** TEST COMMAND GENERATOR **************** -->
<div class="modal fade" id="modalCommandGenerator" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title">Test Command Generator</h5>
			</div>
			<div class="modal-body div-gen" id="divApplication">
				<div class="form-group">
					<label for="txtProgram" class="col-sm-4 col-form-label">*Program og full sti</label>
					<div class="col-sm-12">
						<input type="text" class="form-control" id="txtProgram" style="width:445px" required placeholder="f.eks. C:\HMSCLIENT\Systemx.exe">
					</div>
				</div>
			</div>
			<div class="modal-body div-gen" id="divAppObject">
				<div class="form-group">
					<label for="cboWindowTitle" class="col-sm-4 col-form-label">*Vindutittel</label>
					<div class="col-sm-12">
						<select name="n_category" id="cboWindowTitle" style="width:445px" class="form-control form-control-sm" required>
							<option selected disabled></option>
							<?php
								$query = "SELECT PARENT_ID, PARENT_NAME FROM OBJECTS_PARENTS ORDER BY PARENT_NAME ASC";
								$result = mysqli_query($systemx_connect,$query)or die(mysqli_error());
								while($row = mysqli_fetch_array($result, MYSQL_NUM))
								{
									echo "<option value='$row[0]'>$row[1]</option>";
								}
							?>
						</select>
					</div>
				</div>
				<div class="form-group">
					<label for="cboObjects" class="col-sm-4 col-form-label">*Objekt/Element</label>
					<div class="col-sm-9" id="divObjects">
						<select id="cboObjects" style="width:445px" class="form-control form-control-sm" disabled>
							<option></option>
						</select>
					</div>
				</div>
				<div class="form-group div-gen" id="divBoolean">
					<label for="rdYes" class="col-sm-4 col-form-label">Objekt eksisterer</label>
					<div class="col-sm-9">
						<input type="radio" id="rdYes" name="n_object_exist" value="true" checked readonly /> <label for="rdYes">Sjekk om det valgte objektet eksisterer</label><br>
						<input type="radio" id="rdNo" name="n_object_exist" value="false" readonly /> <label for="rdNo">Sjekk om det valgte objektet IKKE eksisterer</label>
					</div>
				</div>
				<div id="more-options-2" class="hidden div-gen">
					<div class="form-group">
						<div class="col-sm-12">
							<div class='round'><input type="checkbox" id="chkAdvance2" readonly><label for='chkAction'></label></div><label class="pointer" for="chkAdvance2" style="line-height:1.6em;position:relative;left:30px;top:-25px">Vis mer opsjoner</label>
						</div>
					</div>
					<div class="hidden" id="divObjExistOptions">
						<div class="form-group object-text-filter">
							<label for="cboFilter2" class="col-sm-4 col-form-label">Objektfilter</label>
							<div class="col-sm-9" id="divFilter">
								<select id="cboFilter2" style="width:445px" class="form-control form-control-sm">
									<option></option>
									<option title="Teksten fra RawText objekt" value="text-">text</option>
									<option title="Dagens dato" value="date-today">date-today</option>
								</select>
							</div>
						</div>
						<div class="form-group rawtext">
							<label for="txtVisibleText2" class="col-sm-10 col-form-label">Teksten fra RawText objekt</label>
							<div class="col-sm-12">
								<input type="text" class="form-control" id="txtVisibleText2" style="width:445px">
							</div>
						</div>
					</div>
				</div>
				<div id="divObjText" class="div-gen">
					<div class="form-group">
						<label for="cboConditions" class="col-sm-4 col-form-label">Betingelse</label>
						<div class="col-sm-9" id="divObjects">
							<select id="cboConditions" style="width:445px" class="form-control form-control-sm">
								<option></option>
								<option>contains</option>
								<option>match</option>
								<option>notcontain</option>
							</select>
						</div>
					</div>
					<div class="form-group">
						<label for="txtObjText" class="col-sm-10 col-form-label">*Tekst som vil bli sjekket</label>
						<div class="col-sm-12">
							<input type="text" class="form-control" id="txtObjText" style="width:445px" required placeholder="f.eks. match-Dette er eksempel tekst">
						</div>
					</div>
				</div>
				<div id="divKeys" class="div-gen">
					<div class="form-group">
						<label for="cboSpecialKeys" class="col-sm-4 col-form-label">Taster</label>
						<div class="col-sm-9" id="divObjects">
							<select id="cboSpecialKeys" style="width:445px" class="form-control form-control-sm">
								<option></option>
								<option value="{ENTER}">ENTER</option>
								<option value="{ESCAPE}">ESCAPE</option>
								<option value="ALT+">ALT</option>
								<option value="{Alt DOWN}{Oemtilde}{Alt UP}">ALT+Ø</option>
								<option value="{Alt DOWN}{Oem6}{Alt UP}">ALT+Å</option>
								<option value="{Alt DOWN}{Oem7}{Alt UP}">ALT+Æ</option>
								<option value="CTRL+">CTRL</option>
								<option value="{Control DOWN}{Oemtilde}{Control UP}">CTRL+Ø</option>
								<option value="{Control DOWN}{Oem6}{Control UP}">CTRL+Å</option>
								<option value="{Control DOWN}{Oem7}{Control UP}">CTRL+Æ</option>
								<option value="{TAB}">TAB</option>
								<option value="{UP}">Pil opp</option>
								<option value="{DOWN}">Pil ned</option>
								<option value="{RIGHT}">Pil høyre</option>
								<option value="{LEFT}">Pil venstre</option>
								<option value="{PAGEDOWN}">Page Down</option>
								<option value="{PAGEUP}">Page Up</option>
							</select>
						</div>
					</div>
					<div class="form-group">
						<label for="txtKeys" class="col-sm-10 col-form-label">*Tekst som skal sendes til det valgte vinduet</label>
						<div class="col-sm-12">
							<input type="text" class="form-control" id="txtKeys" style="width:445px" required placeholder="f.eks. CTRL+R">
						</div>
					</div>
				</div>
				<div id="divObjClick" class="div-gen">
					<div class="form-group">
						<div class="col-sm-12">
							<div class='round'><input type="checkbox" id="chkAdvance" readonly><label for='chkAction'></label></div><label class="pointer" for="chkAdvance" style="line-height:1.6em;position:relative;top:-25px;left:30px">Vis mer opsjoner</label>
						</div>
					</div>
					<div id="more-options" class="hidden">
						<div class="form-group">
							<div class="col-sm-12">
								<input type="radio" id="rbText" name="n_specific" value="object-text" readonly style="line-height:1.6em"> <label for="rbText" style="line-height:1.6em">Spesifikk tekst i objektet</label><br>
								<input type="radio" id="rbLocation" name="n_specific" value="location" readonly style="line-height:1.6em"> <label for="rbLocation" style="line-height:1.6em">Spesifikk plassering i objektet</label>
							</div>
						</div>
						<div class="form-group hidden object-text">
							<label for="cboFilter" class="col-sm-4 col-form-label">Objektfilter</label>
							<div class="col-sm-9" id="divFilter">
								<select id="cboFilter" style="width:445px" class="form-control form-control-sm">
									<option></option>
									<option title="Format: button-<buttonText>">button</option>
									<option title="Format: text-<RawText>">text</option>
									<option title="Format: contains-<textFromObject>">hastext</option>
									<option title="Format: imageId-<imageId> (Bruk Ranorex Spy)">imageId</option>
									<option title="Format: indexId-<indexId> (Bruk Ranorex Spy)">indexId</option>
									<option title="Format: item-<textFromTreeItem>">item</option>
									<option title="Format: chkbox-<checkboxName>">chkbox</option>
									<option title="Format: rbbutton-<radioButtonText>">rbbutton</option>
								</select>
							</div>
						</div>
						<div class="form-group hidden object-text">
							<label for="txtVisibleText" class="col-sm-10 col-form-label">Teksten fra RawText objekt</label>
							<div class="col-sm-12">
								<input type="text" class="form-control" id="txtVisibleText" style="width:445px" required>
							</div>
						</div>
						<div class="form-group hidden location">
							<label for="txtLocation" class="col-sm-10 col-form-label">Spesifikk plassering der du vil at objektet skal klikkes</label>
							<div class="col-sm-12">
								<input type="text" class="form-control" id="txtLocation" style="width:445px" placeholder="Format: <x>,<y> (f. eks. 100,250)" required>
							</div>
						</div>
					</div>
				</div>
				<div class="form-group div-gen" id="divLocation">
					<label for="txtXY" class="col-sm-10 col-form-label">*Spesifikk plassering der du vil at objektet skal flyttes</label>
					<div class="col-sm-12">
						<input type="text" class="form-control" id="txtXY" style="width:445px" placeholder="Format: <x>,<y> (f. eks. 100,250)" required>
					</div>
				</div>
			</div>
			<div id="divGenerator"></div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-primary" id="btnEnter">ENTER</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
			</div>
			
		</div>
	</div>
</div>

<!-- ***************** ACTIVATE/DEACTIVATE TESTSTEPS **************** -->
<div class="modal fade" id="modalStepActivate" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">Aktiver/Deaktiver teststeg</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmStepActivate">
				<div class="modal-body">
						<label class="text-13">Med utvalgte teststeg:</label>
						<div>
							<input type="radio" id="rbEnable" checked value=0 name="n_activate"> <label for="rbEnable" class="text-13"> Utfør dette steget</label>
						</div>
						<div>
							<input type="radio" id="rbDisable" value=1 name="n_activate"> <label for="rbDisable" class="text-13"> IKKE utfør dette steget</label>
						</div>
				</div>
				<div class="modal-footer hdr">
					<button type="button" class="btn btn-primary" id="btnConfirmActivate" data-form="#frmStepActivate" data-button-action="activate_teststep">Bekreft</button>
					<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
				</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** COPY/MOVE ITEM -- TESTSTEPS **************** -->
<div class="modal fade" id="modalStepCopyMove" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">Kopiere teststeg</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmStepCopy">
				<div class="modal-body">
						<label class="text-13">Med utvalgte teststeg:</label>
						<div>
							<input type="radio" id="rbBeforeStep" value=0 name="n_copy_step"> <label for="rbBeforeStep" class="text-13" id="lblCustom"> %label% før steg nr. </label>
							<span style="float:right;margin-right:110px"><input type="text" class="form-control center" id="txtStepNum" style="width:40px;height:25px;padding:1px" disabled></span>
						</div>
						<div>
							<input type="radio" id="rbToTop" value=1 name="n_copy_step"> <label for="rbToTop" class="text-13" id="lblTop"> %label% til toppen</label>
						</div>
						<div>
							<input type="radio" id="rbToBottom" value=2 name="n_copy_step" checked> <label for="rbToBottom" class="text-13" id="lblBottom"> %label% til bunnen</label>
						</div>
				</div>
				<div class="modal-footer hdr">
					<button type="button" class="btn btn-primary" id="btnConfirmCopy" data-form="#frmStepCopy" data-last-step="<?php echo $new_step_num-1; ?>">Bekreft</button>
					<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
				</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** ADD/EDIT FORM -- PARENT OBJECTS **************** -->
<div class="modal fade" id="modalObjectRegForm" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">%ModalTitle%</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmObjectRegForm">
			<div class="modal-body">
				
					<div class="form-group">
						<label for="txtParentID" class="col-sm-3 col-form-label">Parent ID</label>
						<div class="col-sm-9">
							<input type="text" readonly class="form-control center" id="txtParentID" style="width:80px" name="nParentId">
						</div>
					</div>
					<div class="form-group">
						<label for="txtWinTitle" class="col-sm-10 col-form-label">*Objektnavn/Vindutittel</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="txtWinTitle" style="width:445px" required name="nWinTitle">
						</div>
					</div>
					<div class="form-group">
						<label for="txtRxPath" class="col-sm-3 col-form-label">*Ranorex sti</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="txtRxPath" style="width:445px" required name="nRxPath" placeholder="Format: /form[@title~'<windowTitle>' and @processname='Systemx']">
						</div>
					</div>
					<div class="form-group">
						<label for="txtControlName" class="col-sm-3 col-form-label">*Kontrol navn</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="txtControlName" style="width:445px" required name="nControlName" value="frm">
						</div>
					</div>
					<div class="form-group">
						<label for="txtDescription" class="col-sm-3 col-form-label">Beskrivelse</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="txtDescription" style="width:445px" name="nDescription">
						</div>
					</div>
				
				<div id="info"></div>
			</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-danger" id="btnDelete" data-button-action="parent_objects_delete">Slett</button>
				<button type="button" class="btn btn-primary" id="btnSave" data-form="#frmObjectRegForm">Lagre</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
			</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** ADD/EDIT FORM -- CHILD OBJECTS **************** -->
<?php
	if (isset($_GET['parent-id']))
	{
		$parent_id = $_GET['parent-id'];
		$parent_name = get_parent_info($parent_id, "PARENT_NAME");
	}
?>
<div class="modal fade" id="modalChildObjectRegForm" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">%ModalTitle%</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmChildObjectRegForm">
			<div class="modal-body">
				
					<div class="form-group">
						<label for="txtParentObject" class="col-sm-3 col-form-label">Parent objekt</label>
						<div class="col-sm-9">
							<input type="text" readonly class="form-control" id="txtParentObject" style="width:445px" name="nParentName" value="<?php echo $parent_name; ?>">
						</div>
					</div>
					<div class="form-group">
						<label for="cboChildType" class="col-sm-3 col-form-label">*Objekttype</label>
						<div class="col-sm-9">
							<select class="form-control" id="cboChildType" style="width:445px" required name="nChildType">
								<option disabled selected></selected>
								<option value="btn">Button</option>
								<option value="cell">Cell</option>
								<option value="chk">CheckBox</option>
								<option value="cbo">ComboBox</option>
								<option value="con">Container</option>
								<option value="el">Element</option>
								<option value="lst">List</option>
								<option value="li">ListItem</option>
								<option value="rb">RadioButton</option>
								<option value="ri">RawImage</option>
								<option value="rt">RawText</option>
								<option value="rtb">RawTextBlock</option>
								<option value="tab">TabPage</option>
								<option value="txt">Text</option>
								<option value="tb">Toolbar</option>
								<option value="ti">TreeItem</option>
							</select>
						</div>
					</div>
					<div class="form-group">
						<label for="txtControlName" class="col-sm-10 col-form-label">*Kontrolnavn</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="txtControlName" style="width:445px" required name="nControlName">
						</div>
					</div>
					<div class="form-group">
						<label for="txtRxPath" class="col-sm-3 col-form-label">*Ranorex sti</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="txtRxPath" style="width:445px" required name="nRxPath" placeholder="Format: /form[@title~'<windowTitle>' and @processname='Systemx']">
						</div>
					</div>
					<div class="form-group">
						<label for="txtDescription" class="col-sm-3 col-form-label">Beskrivelse</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="txtDescription" style="width:445px" name="nDescription">
						</div>
					</div>
				
				<div id="info"></div>
			</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-danger" id="btnDelete" data-button-action="child_objects_delete">Slett</button>
				<button type="button" class="btn btn-primary" id="btnSave" data-form="#frmChildObjectRegForm">Lagre</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
			</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** ADD/EDIT FORM -- TEST COMMANDS **************** -->
<div class="modal fade" id="modalTestCommandsRegForm" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">%ModalTitle%</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmTestCommandsRegForm">
			<div class="modal-body">
				
					<div class="form-group">
						<label for="txtCmdID" class="col-sm-9 col-form-label">Kommando ID</label>
						<div class="col-sm-9">
							<input type="text" readonly class="form-control center" id="txtCmdID" style="width:80px" name="nCmdID" value="Auto">
						</div>
					</div>
					<div class="form-group">
						<label for="cboCmdCategory" class="col-sm-3 col-form-label">*Kategori</label>
						<div class="col-sm-9">
							<select class="form-control" id="cboCmdCategory" style="width:445px" required name="nCmdCategory">
								<option disabled selected></selected>
								<option>Application</option>
								<option>Checkpoint</option>
								<option>Keyboard</option>
								<option>Mouse</option>
							</select>
						</div>
					</div>
					<div class="form-group">
						<label for="txtCommand" class="col-sm-10 col-form-label">*Kommando</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="txtCommand" style="width:445px" required name="nCommand">
						</div>
					</div>
					<div class="form-group">
						<label for="txtFormat" class="col-sm-3 col-form-label">*Format</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="txtFormat" style="width:445px" required name="nFormat">
						</div>
					</div>
					<div class="form-group">
						<label for="txtDescription" class="col-sm-3 col-form-label">Beskrivelse</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="txtDescription" style="width:445px" name="nDescription">
						</div>
					</div>
				
				<div id="info"></div>
			</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-warning" id="btnActivate">Aktivere</button>
				<button type="button" class="btn btn-danger" id="btnDelete" data-button-action="test_commands_delete">Slett</button>
				<button type="button" class="btn btn-primary" id="btnSave" data-form="#frmTestCommandsRegForm">Lagre</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
			</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** ADMIN ERROR DIALOG **************** -->
<div class="modal fade" id="adminErrModal" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle"><span class="mdi mdi-alert text-warning icon-md"></span> Informasjon</h5>
			</div>
			<div class="text-error text-13" style="padding:10px">Kun administrator kan administrere denne siden!</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-secondary" data-dismiss="modal">Lukk</button>
			</div>
			
		</div>
	</div>
</div>

<!-- ***************** ADD/EDIT FORM -- UPDATES/CHANGES **************** -->
<div class="modal fade" id="modalUpdatesRegForm" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">%ModalTitle%</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmUpdatesRegForm">
			<div class="modal-body">
				
					<div class="form-group">
						<label for="txtLogDate" class="col-sm-9 col-form-label">Dato</label>
						<div class="col-sm-9">
							<input type="text" readonly class="form-control center" id="txtLogDate" style="width:100px" name="nLogDate" value="<?php echo date("Y-m-d"); ?>">
						</div>
					</div>
					<div class="form-group">
						<label for="txtUpdates" class="col-sm-3 col-form-label">*Endringer/Oppdateringer</label>
						<div class="col-sm-9">
							<textarea class="form-control" id="txtUpdates" style="width:445px" rows="5" required name="nUpdates"></textarea>
						</div>
					</div>
				
				<div id="info"></div>
			</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-danger" id="btnDelete" data-button-action="update_delete">Slett</button>
				<button type="button" class="btn btn-primary" id="btnSave" data-form="#frmUpdatesRegForm">Lagre</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
			</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** ADD/EDIT FORM -- TASKS **************** -->
<div class="modal fade" id="modalTaskRegForm" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">%ModalTitle%</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmTaskRegForm">
			<div class="modal-body">
				<div class="form-group">
					<label for="txtTaskDate" class="col-sm-9 col-form-label">Dato</label>
					<div class="col-sm-9">
						<input type="text" readonly class="form-control center" id="txtTaskDate" style="width:100px" name="nTaskDate" value="<?php echo date("Y-m-d"); ?>">
					</div>
				</div>
				<div class="form-group">
					<label for="cboTaskArea" class="col-sm-3 col-form-label">*Oppgaveområde</label>
					<div class="col-sm-9">
						<select class="form-control" id="cboTaskArea" style="width:445px" required name="nTaskArea">
							<option disabled selected></selected>
							<option>SX-4.1</option>
							<option>SX-5.1</option>
							<option>SX-5.2</option>
							<option>Web UI</option>
							<option>Main Tool</option>
						</select>
					</div>
				</div>
				<div class="form-group">
					<label for="txtTask" class="col-sm-10 col-form-label">*Oppgave</label>
					<div class="col-sm-9">
						<input type="text" class="form-control" id="txtTask" style="width:445px" required name="nTask">
					</div>
				</div>
				<div class="form-group">
					<label for="cboStatus" class="col-sm-3 col-form-label">*Status</label>
					<div class="col-sm-9">
						<select class="form-control" id="cboStatus" style="width:445px" required name="nStatus">
							<option disabled selected></selected>
							<option>TO DO</option>
							<option>IN PROGRESS</option>
							<option>RE-OPENED</option>
							<option>ON HOLD</option>
							<option>CLOSED</option>
							<option>CANCELLED</option>
						</select>
					</div>
				</div>
				<div id="info"></div>
			</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-danger" id="btnDelete" data-button-action="task_delete">Slett</button>
				<button type="button" class="btn btn-primary" id="btnSave" data-form="#frmTaskRegForm">Lagre</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
			</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** RESET PASSWORD DIALOG **************** -->
<div class="modal fade" id="modalResetPassword" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle"><span class="mdi mdi-lock-reset text-warning icon-md"></span> Reset Password</h5>
			</div>
			<div class="text-error text-13" style="padding:10px"></div>
			<input type="text" id="txtNewPassword" style="opacity:0">
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-primary" id="btnReset">Confirm</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Cancel</label>
			</div>
			
		</div>
	</div>
</div>

<!-- ***************** CLEAN ACTIVITY LOGS DIALOG **************** -->
<div class="modal fade" id="modalCleanLogs" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle"><span class="mdi mdi-delete-empty text-warning icon-md"></span> Confirm</h5>
			</div>
			<div class="text-error text-13" style="padding:10px">This will delete all activity logs. Continue?</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-primary" id="btnCleanLogs">Confirm</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Cancel</label>
			</div>
			
		</div>
	</div>
</div>

<!-- ***************** ENABLE/DISABLE LIMIT **************** -->
<?php
	$limit = get_user_info($_SESSION["username"], 11);
	$option1 = "checked";
	$option2 = "";
	$div_class = "text-muted";
	$input_disabled = "disabled";
	$limit_value = "";
	if ($limit > 0)
	{
		$option2 = "checked";
		$option1 = "";
		$div_class = "";
		$input_disabled = "";
		$limit_value = $limit;
	}
?>
<div class="modal fade" id="modalLogLimit" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">Threshold Limit</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmLogLimit">
				<div class="modal-body">
						<div>
							<input type="radio" id="rdDisableLimit" <?php echo $option1; ?> value=0 name="nLimit"> <label for="rdDisableLimit" class="text-13"> Disable Limit</label>
						</div>
						<div>
							<input type="radio" id="rdEnableLimit" value=1 <?php echo $option2; ?> name="nLimit"> <label for="rdEnableLimit" class="text-13"> Enable Limit</label>
						</div>
						<div class="form-group row <?php echo $div_class; ?>" id="divLimit">
							<label for="txtLimit" class="col-sm-6 col-form-label">*Threshold Limit</label>
							<div class="col-sm-3">
								<input type="text" <?php echo $input_disabled; ?> value="<?php echo $limit_value; ?>" required class="form-control center" id="txtLimit" name="nThresholdLimit">
							</div>
						</div>
				</div>
				<div id="info"></div>
				<div class="modal-footer hdr">
					<button type="button" class="btn btn-primary" id="btnConfirmLimit">Confirm</button>
					<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
				</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** SEND MESSAGE FORM **************** -->
<div class="modal fade" id="modalMessageForm" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">Send ny melding</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmMessageForm">
			<div class="modal-body">
				<div class="form-group row">
					<label for="cboReceiver" class="col-sm-3 col-form-label">*Mottaker</label>
					<div class="col-sm-9">
						<select class="form-control" id="cboReceiver" required name="nReceiver">
							<option disabled selected></selected>
							<?php
								$user = $_SESSION["username"];
								$sql = mysqli_query($GLOBALS['users_connect'], "SELECT * FROM designers WHERE USERNAME <> '$user' AND TEST_USER=0 ORDER BY FNAME ASC");
								while($row = mysqli_fetch_array($sql, MYSQL_NUM))
								{
									echo "<option value='$row[3]'>$row[1] $row[2]</option>";
								}
							?>
						</select>
					</div>
				</div>
				<div class="form-group row">
					<label for="txtSubject" class="col-sm-3 col-form-label">*Emne</label>
					<div class="col-sm-9">
						<input type="text" class="form-control" id="txtSubject" required name="nSubject">
					</div>
				</div>
				<div class="form-group">
					<label for="txtMessage" class="col-sm-3 col-form-label">*Meldinger</label>
					<div class="col-sm-9">
						<textarea class="form-control" id="txtMessage" style="width:460px" rows="10" required name="nMessage"></textarea>
					</div>
				</div>
				<div id="info"></div>
			</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-primary" id="btnSend">Send</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
			</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** ACTIVATE/DEACTIVATE USER **************** -->
<div class="modal fade" id="modalActivateUser" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">Activate/Deactivate User</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmActivateToggleUser">
				<div class="modal-body">
						<div>
							<input type="radio" id="rbActivate" checked value=1 name="nToggleActivate"> <label for="rbActivate" class="text-13"> Activate selected user(s)</label>
						</div>
						<div>
							<input type="radio" id="rbDeActivate" value=0 name="nToggleActivate"> <label for="rbDeActivate" class="text-13"> De-activate selected user(s)</label>
						</div>
				</div>
				<div id="info"></div>
				<div class="modal-footer hdr">
					<button type="button" class="btn btn-primary" id="btnConfirmActivateUser">Confirm</button>
					<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Cancel</label>
				</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** ACTIVATE/DEACTIVATE TESTCASE (FROM TESTSTEPS PAGE) **************** -->
<div class="modal fade" id="modalToggleTCActivate" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle"></h5>
			</div>
			<div class="modal-body">
				<div class="form-group">
					<label for="txtComments" class="col-sm-3 col-form-label">Kommentar</label>
					<div class="col-sm-9">
						<input type="text" class="form-control" id="txtComments" style="width:445px" name="n_comments">
					</div>
				</div>
				<div id="info"></div>
			</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-primary" id="btnToggleActivate">Aktivere</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
			</div>
		</div>
	</div>
</div>

<!-- ***************** ACTIVATE/DEACTIVATE TESTSUITES **************** -->
<div class="modal fade" id="modalTSActivate" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">Aktiver/Deaktiver testsuiter</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmTSActivate">
				<div class="modal-body">
						<label class="text-13">Med utvalgte testsuiter:</label>
						<div>
							<input type="radio" id="rbEnableTS" checked value=1 name="n_activate"> <label for="rbEnableTS" class="text-13"> Aktivere</label>
						</div>
						<div>
							<input type="radio" id="rbDisableTS" value=0 name="n_activate"> <label for="rbDisableTS" class="text-13"> Deaktivere</label>
						</div>
				</div>
				<div class="modal-footer hdr">
					<button type="button" class="btn btn-primary" id="btnConfirmActivateTS" data-form="#frmTSActivate" data-button-action="activate_testsuite">Bekreft</button>
					<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
				</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** ADD NEW USER **************** -->
<div class="modal fade" id="modalAddUserForm" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">%ModalTitle%</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmAddUserForm">
			<div class="modal-body">
					<div class="form-group user-form">
						<label for="txtFirstName" class="col-sm-3 col-form-label">*First Name</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="txtFirstName" style="width:445px" required name="nFirstName">
						</div>
					</div>
					<div class="form-group user-form">
						<label for="txtLastName" class="col-sm-10 col-form-label">*Last Name</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="txtLastName" style="width:445px" required name="nLastName">
						</div>
					</div>
					<div class="form-group user-form">
						<label for="txtUserName" class="col-sm-3 col-form-label">*Username</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="txtUserName" style="width:445px" readonly name="nUserName">
						</div>
					</div>
					<div class="form-group user-form user-password">
						<label for="txtPassword" class="col-sm-3 col-form-label">*Password</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="txtPassword" style="width:445px" readonly name="nPassword" value="<?php echo rand_string(10);?>">
						</div>
					</div>
					<div class="form-group">
						<label for="cboRole" class="col-sm-3 col-form-label">*Role</label>
						<div class="col-sm-9">
							<select name="nRole" id="cboRole" class="form-control form-control-sm" required style="width:445px">
								<option selected disabled></option>
								<?php
									$query = "SELECT ROLE_ID, ROLE FROM roles ORDER BY ROLE ASC";
									$result = mysqli_query($GLOBALS['users_connect'], $query) or die(mysqli_error());
									while($row = mysqli_fetch_array($result, MYSQL_NUM))
									{
										echo "<option value='$row[0]'>$row[1]</option>";
									}
								?>
							</select>
						</div>
					</div>
				
				<div id="info"></div>
			</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-danger hidden" id="btnToggleActivateUser">Default</button>
				<button type="button" class="btn btn-warning hidden" id="btnResetPassword">Reset Password</button>
				<button type="button" class="btn btn-primary" id="btnSaveUser">Save</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Cancel</label>
			</div>
			</form>
		</div>
	</div>
</div>

<!-- ***************** ADD NEW ROLE **************** -->
<div class="modal fade" id="modalAddRoleForm" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">%ModalTitle%</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmAddRoleForm">
			<div class="modal-body ">
					<div class="form-group">
						<label for="txtRoleName" class="col-sm-10 col-form-label bold">*Role Name</label>
						<div class="col-sm-9">
							<input type="text" class="form-control" id="txtRoleName" style="width:445px" required name="nRoleName" placeholder="ie. Tester, Test Developer, Software Developer">
						</div>
					</div>
					<div class="form-group">
						<label class="col-sm-9 col-form-label bold"></label>
						<div class="round form-checkbox" title="Check to select all"><input type="checkbox" class="pointer" id="chkSelectAll"> <label for="chkSelectAll"></label></div>
						<label for="chkSelectAll" class="form-label-check pointer" title="Check to select all">Select All</label>
					</div>
					<div class="form-group form-div-checkbox">
						<label class="col-sm-9 col-form-label bold">Testsuite</label>
						<br>
						<div class="round form-checkbox" title="This role can add a testsuite"><input type="checkbox" class="pointer" id="chkTSAdd" name="nTSAdd"> <label for="chkTSAdd"></label></div>
						<label for="chkTSAdd" class="form-label-check pointer" title="This role can add a testsuite">Add</label>
						<div class="round form-checkbox" title="This role can view a testsuite"><input type="checkbox" class="pointer" id="chkTSView" name="nTSView"> <label for="chkTSView"></label></div>
						<label for="chkTSView" class="form-label-check pointer" title="This role can view a testsuite">View</label>
						<div class="round form-checkbox" title="This role can edit a testsuite"><input type="checkbox" class="pointer" id="chkTSEdit" name="nTSEdit"> <label for="chkTSEdit"></label></div>
						<label for="chkTSEdit" class="form-label-check pointer" title="This role can edit a testsuite">Edit</label>
						<div class="round form-checkbox" title="This role can delete a testsuite"><input type="checkbox" class="pointer" id="chkTSDelete" name="nTSDelete"> <label for="chkTSDelete"></label></div>
						<label for="chkTSDelete" class="form-label-check pointer" title="This role can delete a testsuite">Delete</label>
					</div>
					<div class="form-group form-div-checkbox">
						<label class="col-sm-9 col-form-label bold" >Testcase</label>
						<br>
						<div class="round form-checkbox" title="This role can add a testcase"><input type="checkbox" class="pointer" id="chkTCAdd" name="nTCAdd"> <label for="chkTCAdd"></label></div>
						<label for="chkTCAdd" class="form-label-check pointer" title="This role can add a testcase">Add</label>
						<div class="round form-checkbox" title="This role can view a testcase"><input type="checkbox" class="pointer" id="chkTCView" name="nTCView"> <label for="chkTCView"></label></div>
						<label for="chkTCView" class="form-label-check pointer" title="This role can view a testcase">View</label>
						<div class="round form-checkbox" title="This role can edit a testcase"><input type="checkbox" class="pointer" id="chkTCEdit" name="nTCEdit"> <label for="chkTCEdit"></label></div>
						<label for="chkTCEdit" class="form-label-check pointer" title="This role can edit a testcase">Edit</label>
						<div class="round form-checkbox" title="This role can delete a testcase"><input type="checkbox" class="pointer" id="chkTCDelete" name="nTCDelete"> <label for="chkTCDelete"></label></div>
						<label for="chkTCDelete" class="form-label-check pointer" title="This role can delete a testcase">Delete</label>
					</div>
					<div class="form-group form-div-checkbox">
						<label class="col-sm-9 col-form-label bold" >Test Step</label>
						<br>
						<div class="round form-checkbox" title="This role can add a test step"><input type="checkbox" class="pointer" id="chkStepAdd" name="nStepAdd"> <label for="chkStepAdd"></label></div>
						<label for="chkStepAdd" class="form-label-check pointer" title="This role can add a test step">Add</label>
						<div class="round form-checkbox" title="This role can view a test step"><input type="checkbox" class="pointer" id="chkStepView" name="nStepView"> <label for="chkStepView"></label></div>
						<label for="chkStepView" class="form-label-check pointer" title="This role can view a test step">View</label>
						<div class="round form-checkbox" title="This role can edit a test step"><input type="checkbox" class="pointer" id="chkStepEdit" name="nStepEdit"> <label for="chkStepEdit"></label></div>
						<label for="chkStepEdit" class="form-label-check pointer" title="This role can edit a test step">Edit</label>
						<div class="round form-checkbox" title="This role can delete a test step"><input type="checkbox" class="pointer" id="chkStepDelete" name="nStepDelete"> <label for="chkStepDelete"></label></div>
						<label for="chkStepDelete" class="form-label-check pointer" title="This role can delete a test step">Delete</label>
					</div>
					<div class="form-group form-div-checkbox">
						<label class="col-sm-9 col-form-label bold" >Test Command</label>
						<br>
						<div class="round form-checkbox" title="This role can add a test command"><input type="checkbox" class="pointer" id="chkCmdAdd" name="nCmdAdd"> <label for="chkCmdAdd"></label></div>
						<label for="chkCmdAdd" class="form-label-check pointer" title="This role can add a test command">Add</label>
						<div class="round form-checkbox" title="This role can view a test command"><input type="checkbox" class="pointer" id="chkCmdView" name="nCmdView"> <label for="chkCmdView"></label></div>
						<label for="chkCmdView" class="form-label-check pointer" title="This role can view a test command">View</label>
						<div class="round form-checkbox" title="This role can edit a test command"><input type="checkbox" class="pointer" id="chkCmdEdit" name="nCmdEdit"> <label for="chkCmdEdit"></label></div>
						<label for="chkCmdEdit" class="form-label-check pointer" title="This role can edit a test command">Edit</label>
						<div class="round form-checkbox" title="This role can delete a test command"><input type="checkbox" class="pointer" id="chkCmdDelete" name="nCmdDelete"> <label for="chkCmdDelete"></label></div>
						<label for="chkCmdDelete" class="form-label-check pointer" title="This role can delete a test command">Delete</label>
					</div>
					<div class="form-group form-div-checkbox">
						<label class="col-sm-9 col-form-label bold" >Test Object</label>
						<br>
						<div class="round form-checkbox" title="This role can add a test object"><input type="checkbox" class="pointer" id="chkObjAdd" name="nObjAdd"> <label for="chkObjAdd"></label></div>
						<label for="chkObjAdd" class="form-label-check pointer" title="This role can add a test object">Add</label>
						<div class="round form-checkbox" title="This role can view a test object"><input type="checkbox" class="pointer" id="chkObjView" name="nObjView"> <label for="chkObjView"></label></div>
						<label for="chkObjView" class="form-label-check pointer" title="This role can view a test object">View</label>
						<div class="round form-checkbox" title="This role can edit a test object"><input type="checkbox" class="pointer" id="chkObjEdit" name="nObjEdit"> <label for="chkObjEdit"></label></div>
						<label for="chkObjEdit" class="form-label-check pointer" title="This role can edit a test object">Edit</label>
						<div class="round form-checkbox" title="This role can delete a test object"><input type="checkbox" class="pointer" id="chkObjDelete" name="nObjDelete"> <label for="chkObjDelete"></label></div>
						<label for="chkObjDelete" class="form-label-check pointer" title="This role can delete a test object">Delete</label>
					</div>
					<div class="form-group form-div-checkbox">
						<label class="col-sm-9 col-form-label bold" >Task</label>
						<br>
						<div class="round form-checkbox" title="This role can add a task"><input type="checkbox" class="pointer" id="chkTaskAdd" name="nTaskAdd"> <label for="chkTaskAdd"></label></div>
						<label for="chkTaskAdd" class="form-label-check pointer" title="This role can add a task">Add</label>
						<div class="round form-checkbox" title="This role can view a task"><input type="checkbox" class="pointer" id="chkTaskView" name="nTaskView"> <label for="chkTaskView"></label></div>
						<label for="chkTaskView" class="form-label-check pointer" title="This role can view a task">View</label>
						<div class="round form-checkbox" title="This role can edit a task"><input type="checkbox" class="pointer" id="chkTaskEdit" name="nTaskEdit"> <label for="chkTaskEdit"></label></div>
						<label for="chkTaskEdit" class="form-label-check pointer" title="This role can edit a task">Edit</label>
						<div class="round form-checkbox" title="This role can delete a task"><input type="checkbox" class="pointer" id="chkTaskDelete" name="nTaskDelete"> <label for="chkTaskDelete"></label></div>
						<label for="chkTaskDelete" class="form-label-check pointer" title="This role can delete a task">Delete</label>
					</div>
					<div class="form-group form-div-checkbox">
						<label class="col-sm-9 col-form-label bold" >Testcase Manager</label>
						<br>
						<div class="round form-checkbox" title="This role has access to Testcase Manager"><input type="checkbox" class="pointer" id="chkTCManager" name="nTCManager"> <label for="chkTCManager"></label></div>
						<label for="chkTCManager" class="form-label-check pointer" title="This role has access to Testcase Manager">Enable access to Testcase Manager</label>
					</div>
				<div id="info"></div>
			</div>
			<div class="modal-footer hdr">
				<button type="button" class="btn btn-danger" id="btnDeleteRole">Delete</button>
				<button type="button" class="btn btn-primary" id="btnSaveRole">Save</button>
				<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Cancel</label>
			</div>
			</form>
		</div>
	</div>
</div>

</body>
</html>