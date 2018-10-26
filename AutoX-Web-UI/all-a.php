<?php
	$role = get_user_info($_SESSION["username"], 7);
	$isTCAdd = (get_role_info($role, 6)) ? "" : "hidden";
	$isTCEdit = (get_role_info($role, 8)) ? "" : "hidden";
	$isTCDelete = (get_role_info($role, 9)) ? "" : "hidden";
	$editModal = "notModal";
	$editClass = "muted not-allowed";
	if (get_role_info($role, 8))
	{
		$editModal = "modal";
		$editClass = "primary";
	}
?>
<div class="card-body">
	<div class="card">
		<div class="btn-toolbar" role="toolbar">
			<div class="btn-group" role="group" aria-label="First group" style="border:0px">
				<button type="button" title="Registrere ny testcase" class="btn btn-inverse-light icon-btn modalClass <?php echo $isTCAdd; ?>" style="padding:5px" data-toggle='modal' data-icon="new-box" data-target='#modalTcRegForm' id="mnuBtnNew"><span class="mdi mdi-new-box mdi-24px text-warning"></span> Ny registrering</button>
				<button type="button" title="Aktiver/Deaktiver testcaser" class="btn btn-inverse-light icon-btn modalClass <?php echo $isTCEdit; ?>" style="padding:5px" id="mnuBtnEnable" data-icon="crosshairs-gps" data-target='#modalTcActivate'><span class="mdi mdi-crosshairs-gps mdi-24px text-warning"></span> Aktiver/Deaktiver</button>
				<button type="button" title="Slett valgt registrering" class="btn btn-inverse-light icon-btn modalClass <?php echo $isTCDelete; ?>" style="padding:5px" data-icon="delete" id="mnuBtnDelete"><span class="mdi mdi-delete mdi-24px text-warning"></span> Slett</button>
				<button type="button" title="Kopi valgt registrering" class="btn btn-inverse-light icon-btn modalClass <?php echo $isTCAdd; ?>" style="padding:5px" data-icon="content-copy" id="mnuBtnCopy"><span class="mdi mdi-content-copy mdi-24px text-warning"></span> Kopi</button>
				<button type="button" title="Vis nyeste innhold i aktiv side..." class="btn btn-inverse-light icon-btn" style="padding:5px" id="mnuBtnRefresh"><span class="mdi mdi-refresh mdi-24px text-warning"></span> Refresh</button>
			</div>
		</div>
		
		<table class="table table-bordered">
			<thead>
				<tr class="bg-color-light">
					<th width="2%" class="center"><div class='round'><input type='checkbox' id="chkAction"><label for='chkAction'></label></div></th>
					<th width="5%" class="center">TC-ID</th>
					<th width="37%">Testcase tittel</th>
					<th width="24%">Testsuite</th>
					<th width="13%">Sist endret av</th>
					<th width="10%" class="center">Siste commitdato</th>
					<th width="7%" class="center">Teststatus</th>
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
					$query = "SELECT * FROM testcases ORDER BY ts_id, tc_id ASC LIMIT $page, $limit";
					$result = mysqli_query($systemx_connect,$query)or die(mysqli_error());
					$testcase_count = get_testcase_count();
					while($row = mysqli_fetch_array($result, MYSQL_NUM))
					{
						$fname = ucwords(strtolower(get_user_info($row[4], 1)));
						$user_info = $fname{0} . ". " . ucwords(strtolower(get_user_info($row[4], 2)));
						$id = $row[0];
						
						$testsuite_title = get_testsuite_info($row[2], 2);
						$active = "<span class='mdi mdi-crosshairs-gps icon-sm'>";
						$comments = ($row[7]) ? "" : "Ikke aktiv: " . $row[10];
						$disabled = "";
						$commit_date = (empty($row[14])) ? "" : date("Y-m-d H:i", strtotime($row[14]));
						$run_status = "<img src='images/jenkins/aborted.png' title='Not Executed'>";
						$run_test_from = "";
						if (!$row[7])
						{
							$active = "<span class='mdi mdi-crosshairs icon-sm'>";
							$disabled = "text-disabled";
						}
						if ($row[12] != null)
						{
							$run_date = date("Y-m-d H:i", strtotime($row[13]));
							$run_status = (!$row[12]) ? "<img src='images/jenkins/failure.png' title='Failed'>":"<img src='images/jenkins/success.png' title='Success'>";
							$run_test_from = "Testen kjøres fra $row[15] på $run_date";	
						}
						echo "
							<tr class='$disabled' title='$comments'>
								<td class='center'><div class='round' style='margin-left:10px'><input type='checkbox' value='$row[0]' id='chkRow$row[0]' class='cbox-default'><label for='chkRow$row[0]'></label></div></td>
								<td class='center'><a href='?id=2&ts-id=$row[2]&tc-id=$row[1]' >$row[1]</a></td>
								<td><a href='?id=2&ts-id=$row[2]&tc-id=$row[1]'>$row[3]</a></td>
								<td><a href='?id=2&ts-id=$row[2]'>$testsuite_title</a></td>
								<td>$user_info</td>
								<td class='center'>$commit_date</td>
								<td class='center' title='$run_test_from'>$run_status</td>
								<td class='center'><i data-id='$row[0]' data-tc-id='$row[1]' data-active='$row[7]' data-ts-id='$row[2]' data-title='$row[3]' data-comments='$row[10]' class='mdi mdi-pencil-box icon-xmd text-$editClass edit-class modalClass' data-category='testcase' data-icon='pencil-box' data-toggle='$editModal' data-target='#modalTcRegForm' title='Redigere testcase'></i></td>
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
</div>

<?php
	include "modals.php";
?>