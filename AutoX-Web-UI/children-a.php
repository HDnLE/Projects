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
				<button type="button" title="Registrere nytt child objekt" class="btn btn-inverse-light icon-btn modalClass <?php echo $isObjAdd; ?>" style="padding:5px" data-icon="new-box" data-toggle='modal' data-target='#modalChildObjectRegForm' id="mnuBtnNew"><span class="mdi mdi-new-box mdi-24px text-warning"></span> Ny registrering</button>
				<button type="button" title="Slett valgt registrering" class="btn btn-inverse-light icon-btn modalClass <?php echo $isObjDelete; ?>" style="padding:5px" data-icon="delete" id="mnuBtnDelete"><span class="mdi mdi-delete mdi-24px text-warning"></span> Slett</button>
				<button type="button" title="Vis nyeste innhold i aktiv side..." class="btn btn-inverse-light icon-btn" style="padding:5px" id="mnuBtnRefresh"><span class="mdi mdi-refresh mdi-24px text-warning"></span> Refresh</button>
			</div>
		</div>
		
		<table class="table table-bordered" id="testsuite" style="table-layout: fixed">
			<thead>
				<tr class="bg-color-light">
					<th width="2%"><div class="round"><input type="checkbox" class="pointer" id="chkAction"> <label for="chkAction"></label></div></th>
					<th width="6%" class="center">Kontrolnavn</th>
					<th width="10%">Objekttype</th>
					<th width="43%">Ranorex sti</th>
					<th width="20%">Beskrivelse</th>
					<th width="2%"></th>
				</tr>
			</thead>
			<tbody>
				<?php
					$parent_id = $_GET['parent-id'];
					$query = "SELECT * FROM OBJECTS_CHILDREN WHERE PARENT_ID=$parent_id ORDER BY CONTROL_NAME ASC";
					$result = mysqli_query($systemx_connect,$query)or die(mysqli_error());
					while($row = mysqli_fetch_array($result, MYSQL_NUM))
					{
						$id = $row[0];
						$modalTarget = "#modalChildObjectRegForm";
						$parent_name = get_parent_info($row[1], "PARENT_NAME");
						if ($row[4] == "Form")
						{
							$modalTarget = "#modalObjectRegForm";
						}
						echo "
							<tr>
								<td class='center'><div class='round' style='margin-left:10px'><input type='checkbox' value='$row[0]' id='chkRow$row[0]' class='cbox-default'><label for='chkRow$row[0]'></label></div></td>
								<td>$row[6]</td>
								<td>$row[4]</td>
								<td style='white-space:break-word'>$row[5]</td>
								<td>$row[7]</td>
								<td class='center'><i data-id='$row[0]' data-parent-name='$parent_name' data-parent-id='$row[1]' data-child-id='$row[2]' data-control-name='$row[6]' data-rx-path=\"$row[5]\" data-object-type='$row[4]' data-description='$row[7]' data-icon='pencil-box' class='mdi mdi-pencil-box icon-xmd text-$editClass edit-class modalClass' data-toggle='$editModal' data-target='$modalTarget' title='Endre child objekt'></i></td>
							</tr>
						";
					}
				?>
			</tbody>
		</table>
	</div>
</div>

<?php
	include "modals.php";
?>