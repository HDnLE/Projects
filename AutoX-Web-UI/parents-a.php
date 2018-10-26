<?php
	$role = get_user_info($_SESSION["username"], 7);
	$isObjAdd = (get_role_info($role, 14)) ? "" : "hidden";
	$isObjDelete = (get_role_info($role, 17)) ? "" : "hidden";
	$editModal = "notModal";
	$editClass = "muted not-allowed";
	if (get_role_info($role, 16))
	{
		$editModal = "modal";
		$editClass = "primary";
	}
?>
<div class="card-body">
	<div class="card">
		<div class="btn-toolbar" role="toolbar">
			<div class="btn-group" role="group" aria-label="First group" style="border:0px">
				<button type="button" title="Registrere nytt objekt" class="btn btn-inverse-light icon-btn modalClass <?php echo $isObjAdd; ?>" style="padding:5px" data-icon="new-box" data-toggle='modal' data-target='#modalObjectRegForm' id="mnuBtnNew"><span class="mdi mdi-new-box mdi-24px text-warning"></span> Ny registrering</button>
				<button type="button" title="Slett valgt registrering" class="btn btn-inverse-light icon-btn modalClass <?php echo $isObjDelete; ?>" style="padding:5px" data-icon="delete" id="mnuBtnDelete"><span class="mdi mdi-delete mdi-24px text-warning"></span> Slett</button>
				<button type="button" title="Vis nyeste innhold i aktiv side..." class="btn btn-inverse-light icon-btn" style="padding:5px" id="mnuBtnRefresh"><span class="mdi mdi-refresh mdi-24px text-warning"></span> Refresh</button>
			</div>
		</div>
		
		<table class="table table-bordered" id="testsuite">
			<thead>
				<tr class="bg-color-light">
					<th width="2%"><div class="round"><input type="checkbox" class="pointer" id="chkAction"> <label for="chkAction"></label></div></th>
					<th width="10%">Objektnavn</th>
					<th width="49%">Ranorex sti</th>
					<th width="7%" class="center">Antall child<br>objekter</th>
					<th width="20%">Beskrivelse</th>
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
					$query = "SELECT * FROM OBJECTS_PARENTS ORDER BY PARENT_NAME ASC LIMIT $page, $limit";
					$result = mysqli_query($systemx_connect,$query)or die(mysqli_error());
					$object_count = get_record_count("OBJECTS_PARENTS");
					if ($object_count == 0) {$curr_page = 0;}
					$total_testcase = 0;
					while($row = mysqli_fetch_array($result, MYSQL_NUM))
					{
						$id = $row[0];
						$parent_objects = get_number_of_child_objects($row[1]);
						echo "
							<tr>
								<td class='center'><div class='round' style='margin-left:10px'><input type='checkbox' value='$row[0]' id='chkRow$row[0]' data-parent-id='$row[1]' class='cbox-default'><label for='chkRow$row[0]'></label></div></td>
								<td><a href='?id=4&parent-id=$row[1]' >$row[2]</a></td>
								<td>$row[3]</td>
								<td class='center'>$parent_objects</td>
								<td>$row[5]</td>
								<td class='center'><i data-id='$row[0]' data-parent-id='$row[1]' data-object-name='$row[2]' data-rx-path=\"$row[3]\" data-control-name='$row[4]' data-description=\"$row[5]\" class='mdi mdi-pencil-box icon-xmd text-$editClass edit-class modalClass' data-toggle='$editModal' data-icon='pencil-box' data-target='#modalObjectRegForm' title='Redigere parent objekt'></i></td>
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
			$total_page = ceil($object_count/$limit);
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
				if ($_GET['page'] == 1)
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