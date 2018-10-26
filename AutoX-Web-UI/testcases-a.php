<?php
	$active = (!isset($_GET['show-active-only'])) ? "false" : $_GET['show-active-only'];
	$btnText = "Vis kun aktive testcaser";
	$btnIcon = "-gps";
	$queryActive = "";
	if ($active == "true")
	{
		$btnText = "Vis alle testcaser";
		$btnIcon = "";
		$queryActive = "AND ACTIVE=1";
	}
	$role = get_user_info($_SESSION["username"], 7);
	$isTCAdd = (get_role_info($role, 6)) ? "" : "hidden";
	$isTCEdit = (get_role_info($role, 8)) ? "" : "hidden";
	$isTCDelete = (get_role_info($role, 9)) ? "" : "hidden";
	$editModal = "notModal";
	$editClass = "muted not-allowed";
	if (get_role_info($role, 8))
	{
		$editModal = "modal";
		$editClass = "primary pointer";
	}
?>
<div class="card">
		<div class="btn-toolbar" role="toolbar">
			<div class="btn-group" role="group" aria-label="First group" style="border:0px">
				<button type="button" title="Registrere ny testcase" class="btn btn-inverse-light icon-btn modalClass <?php echo $isTCAdd; ?>" style="padding:5px" data-icon="new-box" data-toggle='modal' data-target='#modalTcRegForm' id="mnuBtnNew"><span class="mdi mdi-new-box mdi-24px text-warning"></span> Ny registrering</button>
				<button type="button" title="Aktiver/Deaktiver testcaser" class="btn btn-inverse-light icon-btn modalClass <?php echo $isTCEdit; ?>" style="padding:5px" data-icon="crosshairs-gps" id="mnuBtnEnable" data-target='#modalTcActivate'><span class="mdi mdi-crosshairs-gps mdi-24px text-warning"></span> Aktiver/Deaktiver</button>
				<button type="button" title="Slett valgt registrering" class="btn btn-inverse-light icon-btn modalClass <?php echo $isTCDelete; ?>" style="padding:5px" id="mnuBtnDelete" data-icon="delete"><span class="mdi mdi-delete mdi-24px text-warning"></span> Slett</button>
				<button type="button" title="Kopi valgt registrering" class="btn btn-inverse-light icon-btn modalClass <?php echo $isTCAdd; ?>" style="padding:5px" data-icon="content-copy" id="mnuBtnCopy"><span class="mdi mdi-content-copy mdi-24px text-warning"></span> Kopi</button>
				<button type="button" title="Flytt valgt registrering" class="btn btn-inverse-light icon-btn modalClass <?php echo $isTCEdit; ?>" style="padding:5px" data-icon="folder-move" id="mnuBtnMove"><span class="mdi mdi-folder-move mdi-24px text-warning"></span> Flytt</button>
				<button type="button" title="Vis nyeste innhold i aktiv side" class="btn btn-inverse-light icon-btn" style="padding:5px" id="mnuBtnRefresh"><span class="mdi mdi-refresh mdi-24px text-warning"></span> Refresh</button>
				<button type="button" title="<?php echo $btnText; ?>" data-active="<?php echo $active; ?>" class="btn btn-inverse-light icon-btn" style="padding:5px" id="mnuShowActiveOnly"><span class="mdi mdi-crosshairs<?php echo $btnIcon; ?> mdi-24px text-warning"></span> <?php echo $btnText; ?></button>
				
			</div>
		</div>
		
		<table class="table table-bordered">
			<thead>
				<tr class="bg-color-light">
					<th width="2%" class='center'><div class='round'><input type='checkbox' id="chkAction"><label for='chkAction'></label></div></th>
					<th width="5%" class="center">TC-ID</th>
					<th width="39%">Testcase tittel</th>
					<th width="10%" class="center">Siste commitdato</th>
					<th width="7%" class="center">JIRA Ticket</th>
					<th width="20%">Oppdatert av</th>
					<th width="7%" class="center">Teststatus</th>
					<th width="8%" class="center">Siste kjøredato</th>
					<th width="2%"></th>
				</tr>
			</thead>
			<tbody>
				<?php
					$page = 0;
					$default_limit = 25;
					$limit = $default_limit;
					$curr_page = 1;
					
					$ts_title = get_testsuite_info($ts_id, 2);
					if (isset($_GET['page'])) 
					{
						$curr_page = $_GET['page'];
						$limit = $_GET['limit']; 
						$page = (($curr_page - 1) * $limit);
					}
					$curr_page++;
					$limit = ($limit == "Alle" ? $limit = 999999 : $limit);
					$query = "SELECT * FROM testcases WHERE TS_ID=$ts_id $queryActive ORDER BY TC_ID ASC LIMIT $page, $limit";
					$result = mysqli_query($systemx_connect,$query)or die(mysqli_error());
					
					while($row = mysqli_fetch_array($result, MYSQL_NUM))
					{
						$fname = ucwords(strtolower(get_user_info($row[4], 1)));
						$lname = ucwords(strtolower(get_user_info($row[4], 2)));
						$user_info = substr($fname,0,1) . ". " . ucwords(strtolower(get_user_info($row[4], 2)));
						$id = $row[0];
						$last_modified_date = date("d.m.Y ", strtotime($row[5])) . "kl. " . date("H:i", strtotime($row[5]));
						$build_date = (empty($row[18])) ? "" : "\nBuilddato: " . date("d.m.Y ", strtotime($row[18])) . "kl. " . date("H:i", strtotime($row[18])) . " med Ranorex $row[17]";
						$builder = (empty($row[19])) ? "" : "\nBygget av: " . get_user_info($row[19], 1) . " " . get_user_info($row[19], 2);
						$testsuite_title = get_testsuite_info($row[2], 2);
						$active = "<span class='mdi mdi-crosshairs-gps icon-sm'></span>";
						$disabled = "";
						//$run_status = "";
						$run_status = "<img src='images/jenkins/aborted.png' title='Not Executed'>";
						$comments = (empty($row[10])) ? "" : "Kommentar: $row[10]";
						$run_date = "";
						$commit_date = (empty($row[14])) ? "" : date("Y-m-d H:i", strtotime($row[14]));
						$run_test_from = "";
						if (!get_testsuite_info($ts_id, 11))
						{
							$row[7] = 0;
							$row[10] = "Testsuite er deaktivert";
						}
						if (!$row[7])
						{
							$active = "<span class='mdi mdi-crosshairs icon-sm text-muted'></span>";
							$disabled = "text-disabled";
							$comments = (empty($row[10])) ? "Ingen kommentar" : $row[10];
							$comments = "\nIkke aktiv: " . $comments;
						}
						$testStatus = "";
						if ($row[12] != null)
						{
							$run_date = date("Y-m-d H:i", strtotime($row[13]));
							//$run_status = (!$row[12]) ? "<span class='mdi mdi-thumb-down icon-sm text-danger'></span>":"<span class='mdi mdi-thumb-up icon-sm text-success'></span>";
							$run_status = (!$row[12]) ? "<img src='images/jenkins/failure.png' title='Failed'>":"<img src='images/jenkins/success.png' title='Success'>";
							$run_test_from = "Testen kjøres fra $row[15]";
							$testStatus = (!$row[12]) ? "Failed" : "Success";
							$testStatus = "\nSiste kjørestatus: $testStatus (Testen kjøres fra $row[15] $run_date)";							
						}
						$commitMsg = (empty($row[16])) ? "" : "\nCommit beskjed: $row[16]";
						$lastModified = "\nOppdatert av $fname $lname $last_modified_date";
						
						$tooltip = $lastModified . $comments . $build_date . $builder . $commitMsg . $testStatus;
						
						echo "
							<tr class='$disabled' title='$tooltip'>
								<td class='center'><div class='round' style='margin-left:10px'><input type='checkbox' id='chkRow$row[0]' value='$row[0]' class='cbox-default'><label for='chkRow$row[0]'></label></div></td>
								<td class='center'><a href='?id=2&ts-id=$row[2]&tc-id=$row[1]' >$row[1]</a></td>
								<td><a href='?id=2&ts-id=$row[2]&tc-id=$row[1]'>$row[3]</a></td>
								<td class='center'>$commit_date</td>
								<td class='center'><a href='http://jiracon-appsrv:8080/browse/$row[11]' target='_blank'>$row[11]</a></td>
								<td>$user_info</td>
								<td class='center'>$run_status</td>
								<td class='center'>$run_date</td>
								<td class='center' title='Redigere testcase'><i data-id='$row[0]' data-tc-id='$row[1]' data-active='$row[7]' data-ts-id='$row[2]' data-title='$row[3]' data-comments='$row[10]' data-ticket='$row[11]' class='mdi mdi-pencil-box text-$editClass pointer icon-xmd edit-class modalClass' data-icon='pencil-box' data-category='testcase' data-toggle='$editModal' data-target='#modalTcRegForm' title='Redigere testcase'></i></td>
							</tr>
						";
					}
				?>
			</tbody>
		</table>
		
	</div>
	<br>
		<?php
			$first_button = "disabled";
			$next_button = "";
			$prev_button = "disabled";
			$last_button = "";
			$page_select = "";
			$total_page = ceil($testcase_count/$limit);
			if (isset($_GET['limit']))
			{
				if ($_GET['limit'] == "Alle")
				{
					$first_button = "disabled";
					$next_button = "disabled";
					$prev_button = "disabled";
					$last_button = "disabled";
					$page_select = "disabled";
				}
			}
			if (isset($_GET['page']))
			{
				$first_button = "";
				$prev_button = "";
				if ($_GET['page'] <= 1)
				{
					$first_button = "disabled";
					$prev_button = "disabled";
				}
				if ($_GET['page'] == $total_page)
				{
					$next_button = "disabled";
					$last_button = "disabled";
				}
			}
			if ($total_page == 1)
			{
				$next_button = "disabled";
				$last_button = "disabled";
			}
		?>
		<div class="btn-group right" role="group">
			<button type="button" title="Først..." <?php echo $first_button; ?> class="btn btn-primary icon-btn btn-pagination" data-page="1"><i class="mdi mdi-skip-backward px-25"></i></button>
			<button type="button" title="Tidligere..." <?php echo $prev_button; ?> class="btn btn-primary icon-btn btn-pagination" data-page="<?php echo $curr_page-2; ?>"><i class="mdi mdi-skip-previous px-25"></i></button>
			<div class="btn-primary" style="padding:10px 5px">Side</div>
			<select id="page" <?php echo $page_select; ?>>
				<?php
					for ($i=1; $i<=$total_page; $i++)
					{
						$selected = "";
						if (isset($_GET['page']))
						{
							$selected = ($_GET['page'] == $i) ? "selected" : "";
						}
						echo "<option value='$i' $selected>$i</option>";
					}
				?>
			</select>
			<button type="button" title="Neste..." <?php echo $next_button; ?> class="btn btn-primary icon-btn btn-pagination" data-page="<?php echo $curr_page; ?>"><i class="mdi mdi-skip-next px-25"></i></button>
			<button type="button" title="Siste..." <?php echo $last_button; ?> class="btn btn-primary icon-btn btn-pagination" data-page="<?php echo $total_page; ?>"><i class="mdi mdi-skip-forward px-25"></i></button>
			<div class="btn-primary" style="padding:10px 5px">Vis</div>
			<select id="limit">
				<?php
					$arr_limits = array(25, 50, 100, "Alle");
					foreach ($arr_limits as $opt_limit)
					{
						$selected2 = "";
						if ($opt_limit == $_GET['limit']) {$selected2 = "selected";}
						echo "<option $selected2>$opt_limit</option>";
					}
				?>
			</select>
			<div class="btn-primary" style="padding:10px 5px">per side</div>
		</div>