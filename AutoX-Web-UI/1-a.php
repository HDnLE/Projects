<h4 class="headliner" id="category" data-category='testsuite'>Testsuiter</h4>
<div class="card-body">
	<div class="card">
		<div class="btn-toolbar" role="toolbar">
			<?php
				$role = get_user_info($_SESSION["username"], 7);
				$isTSAdd = (get_role_info($role, 2)) ? "" : "hidden";
				$isTSDelete = (get_role_info($role, 5)) ? "" : "hidden";
				$isTSEdit = (get_role_info($role, 4)) ? "" : "hidden";
				$editModal = "notModal";
				$editClass = "muted not-allowed";
				if (get_role_info($role, 4))
				{
					$editModal = "modal";
					$editClass = "primary";
				}
			?>
			<div class="btn-group" role="group" aria-label="First group" style="border:0px">
				<button type="button" title="Registrere ny testsuite" class="btn btn-inverse-light icon-btn modalClass <?php echo $isTSAdd; ?>" data-icon="new-box" style="padding:5px" data-toggle='modal' data-target='#modalTsRegForm' id="mnuBtnNew"><span class="mdi mdi-new-box mdi-24px text-warning"></span> Ny registrering</button>
				<button type="button" title="Slett valgt registrering" class="btn btn-inverse-light icon-btn modalClass <?php echo $isTSDelete; ?>" data-icon="delete" style="padding:5px" id="mnuBtnDelete"><span class="mdi mdi-delete mdi-24px text-warning"></span> Slett</button>
				<button type="button" title="Kopi valgt registrering" class="btn btn-inverse-light icon-btn modalClass <?php echo $isTSAdd; ?>" style="padding:5px" data-icon="content-copy" id="mnuBtnCopy"><span class="mdi mdi-content-copy mdi-24px text-warning"></span> Kopi</button>
				<button type="button" title="Aktiver/Deaktiver testsuiter" class="btn btn-inverse-light icon-btn modalClass <?php echo $isTSEdit; ?>" style="padding:5px" data-icon="crosshairs-gps" id="mnuBtnEnable" data-target='#modalTSActivate'><span class="mdi mdi-crosshairs-gps mdi-24px text-warning"></span> Aktiver/Deaktiver</button>
				<button type="button" title="Vis nyeste innhold i aktiv side..." class="btn btn-inverse-light icon-btn" style="padding:5px" id="mnuBtnRefresh"><span class="mdi mdi-refresh mdi-24px text-warning"></span> Refresh</button>
			</div>
		</div>
		
		<table class="table table-bordered" id="testsuite">
			<thead>
				<tr class="bg-color-light">
					<th width="2%" class="v-align"><div class="round"><input type="checkbox" class="pointer" id="chkAction"> <label for="chkAction"></label></div></th>
					<th width="5%" class="center">Testgruppe</th>
					<th width="5%" class="center">TS-ID</th>
					<th width="23%">Testsuite tittel</th>
					<th width="10%" class="center">Siste commitdato</th>
					<th width="15%">Oppdatert av</th>
					<?php
						$active = "true";
						$title = "Vise kun aktive testcaser";
						$chkbox_checked = "mdi-crosshairs mdi-dark mdi-inactive";
						if (isset($_GET['show-active-only']))
						{
							$active = "true";
							if ($_GET['show-active-only'] == "true")
							{	
								$active = "false";
								$chkbox_checked = "mdi-crosshairs-gps";
								$title = "Vise alle testcaser";
							}
						}
					?>
					<th width="10%" class="center">
						Antall testcaser<br>
						Alle/Kun aktiv<br>
						<span data-active='<?php echo $active; ?>' class='mdi text-success <?php echo $chkbox_checked; ?> icon-xmd pointer' title='<?php echo $title; ?>'></span>
					</th>
					<th width="10%" class="center">Siste kjøredato</th>
					<th width="10%" class="center">Kjører fra</th>
					<th width="8%" class="center">Siste teststatus</th>
					<th width="2%"></th>
				</tr>
			</thead>
			<tbody>
				<?php
					$page = 0;
					$default_limit = 25;
					$limit = $default_limit;
					$curr_page = 1;
					
					
					if (isset($_GET['page'])) 
					{
						$curr_page = $_GET['page'];
						$limit = $_GET['limit']; 
						$page = (($curr_page - 1) * $limit);
					}
					$curr_page++;
					$limit = ($limit == "Alle" ? $limit = 999999 : $limit);
					$query = "SELECT * FROM testsuites ORDER BY ts_id ASC LIMIT $page, $limit";
					$result = mysqli_query($systemx_connect,$query)or die(mysqli_error());
					$testsuite_count = get_testsuite_count();
					if ($testsuite_count == 0) {$curr_page = 0;}
					$total_testcase = 0;
					
					while($row = mysqli_fetch_array($result, MYSQL_NUM))
					{
						$fname = ucwords(strtolower(get_user_info($row[4], 1)));
						$user_info = $fname{0} . ". " . ucwords(strtolower(get_user_info($row[4], 2)));
						$id = $row[0];
						$testcases = get_number_of_testcases($row[1], $active);
						$commitDate = (empty($row[8])) ? "" : date("d.m.Y H:i", strtotime($row[8]));
						$buildDate = (empty($row[13])) ? "" : date("d.m.Y \k\l. H:i", strtotime($row[13]));
						$creationDate = (empty($row[15])) ? "" : date("d.m.Y \k\l. H:i", strtotime($row[15]));
						
						$run_test_from = (empty($row[9])) ? "" : "<a href='http://cistaging:8080/computer/$row[9]/' target='_blank'>$row[9]</a>";
						$run_status = "<img src='images/jenkins/aborted.png' title='Not Executed'>";
						$run_date = "";
						$disabled = ($row[11]) ? "" : "text-disabled";
						$commitMsg = (empty($row[10])) ? "" : "(Commit beskjed: $row[10])";
						
						if (!$row[11])
						{
							$row[6] = null;
							$row[9] = "";
						}
						if ($row[6] != null)
						{
							$run_date = date("d.m.Y H:i", strtotime($row[7]));
							//$run_status = (!$row[6]) ? "<span class='mdi mdi-thumb-down icon-sm text-danger'></span>":"<span class='mdi mdi-thumb-up icon-sm text-success'></span>";
							$run_status = (!$row[6]) ? "<img src='images/jenkins/failure.png' title='Failed'>":"<img src='images/jenkins/success.png' title='Success'>";
							$testStatus = (!$row[6]) ? "Failed" : "Success";
							$testStatus = "\nSiste kjørestatus: $testStatus (Testen kjøres fra $row[9] $run_date)";
						}
						$run_test_from = (empty($row[9])) ? "" : "<a href='http://cistaging:8080/computer/$row[9]/' target='_blank'>$row[9]</a>";
						$tsDescription = str_replace("<b>", "", $row[3]);
						$tsDescription = str_replace("</b>", "", $tsDescription);
						$lastRun = (empty($run_test_from)) ? "" : "Siste kjøre $run_date fra $row[9] maskin";
						$comments = "\nBeskrivelse: $tsDescription";
						$lastUpdate = "Oppdatert av $user_info $creationDate";
						$builInfo = (empty($row[14])) ? "" : "\nByggdato: $buildDate (av " . get_user_info($row[14], 1) . " " . get_user_info($row[14], 2) . ")";
						$commitInfo = (empty($row[8])) ? "": "\nCommitdato: $commitDate $commitMsg";
						
						$tooltip = $lastUpdate . $comments . $builInfo . $commitInfo . $testStatus;
						echo "
							<tr class='$disabled' title='$tooltip'>
								<td class='center'><div class='round' style='margin-left:10px'><input type='checkbox' data-ts-id='$row[1]' value='$row[0]' id='chkRow$row[0]' data-evalz='$testcases' class='cbox-default'> <label for='chkRow$row[0]'></label></div></td>
								<td class='center'>$row[12]</td>
								<td class='center'><a href='?id=2&ts-id=$row[1]'>$row[1]</a></td>
								<td><a href='?id=2&ts-id=$row[1]'>$row[2]</a></td>
								<td class='center'>$commitDate</td>
								<td>$user_info</td>
								<td class='center'>$testcases</td>
								<td class='center'>$run_date</td>
								<td class='center'>$run_test_from</td>
								<td class='center'>$run_status</td>
								<td class='center' title='Redigere testsuite'><i data-id='$row[0]' data-active='$row[11]' data-group='$row[12]' data-ts-id='$row[1]' data-title='$row[2]' data-description='$row[3]' class='mdi mdi-pencil-box icon-xmd text-$editClass pointer edit-class modalClass' data-category='testsuite' data-icon='pencil-box' data-toggle='$editModal' data-target='#modalTsRegForm'></i></td>
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
			$total_page = ceil($testsuite_count/$limit);
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
				if ($_GET['page'] < 2)
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
</div>

<?php
	include "modals.php";
?>