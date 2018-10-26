<h4 class="headliner" id="category" data-category='roles'>User Roles</h4>
<div class="card-body">
	<div class="card">
		<div class="btn-toolbar" role="toolbar">
			<div class="btn-group" role="group" aria-label="First group" style="border:0px">
				<button type="button" title="Add New Role" class="btn btn-inverse-light icon-btn modalClass" data-toggle='modal' data-target="#modalAddRoleForm" data-icon="account-settings-variant" style="padding:5px" id="mnuBtnAddRole"><span class="mdi mdi-account-settings-variant mdi-24px text-warning"></span> Add New Role</button>
				<button type="button" title="Slett valgt registrering" class="btn btn-inverse-light icon-btn modalClass" style="padding:5px" id="mnuBtnDelete" data-icon="delete"><span class="mdi mdi-delete mdi-24px text-warning"></span> Delete Role</button>
			</div>
		</div>
		<table class="table table-bordered table-hover">
			<thead>
				<tr class="bg-color-light">
					<th width="2%"><div class="round"><input type="checkbox" class="pointer" id="chkAction"> <label for="chkAction"></label></div></th>
					<th width="23%">Role</th>
					<th width="10%" class="center">TESTSUITE<br>Add|View|Edit|Delete</th>
					<th width="10%" class="center">TESTCASE<br>Add|View|Edit|Delete</th>
					<th width="10%" class="center">TEST STEP<br>Add|View|Edit|Delete</th>
					<th width="10%" class="center">TEST COMMAND<br>Add|View|Edit|Delete</th>
					<th width="10%" class="center">TEST OBJECT<br>Add|View|Edit|Delete</th>
					<th width="10%" class="center">TASK<br>Add|View|Edit|Delete</th>
					<th width="10%" class="center">Testcase Manager</th>
					<th width="5%"></th>
				</tr>
			</thead>
			<tbody>
				<?php
					$query = "SELECT * FROM roles ORDER BY ROLE_ID ASC";
					$result = mysqli_query($users_connect,$query)or die(mysqli_error());
					while($row = mysqli_fetch_array($result, MYSQL_NUM))
					{
						$tsAdd = ($row[2]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$tsView = ($row[3]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$tsEdit = ($row[4]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$tsDelete = ($row[5]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$tcAdd = ($row[6]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$tcView = ($row[7]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$tcEdit = ($row[8]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$tcDelete = ($row[9]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$cmdAdd = ($row[10]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$cmdView = ($row[11]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$cmdEdit = ($row[12]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$cmdDelete = ($row[13]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$objAdd = ($row[14]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$objView = ($row[15]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$objEdit = ($row[16]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$objDelete = ($row[17]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$taskAdd = ($row[18]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$taskView = ($row[19]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$taskEdit = ($row[20]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$taskDelete = ($row[21]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$tcManager = ($row[22]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$stepAdd = ($row[23]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$stepView = ($row[24]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$stepEdit = ($row[25]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						$stepDelete = ($row[26]) ? "mdi-checkbox-marked-circle text-success" : "mdi-close-circle text-muted";
						echo "
							<tr>
								<td class='center'><div class='round' style='margin-left:10px'><input type='checkbox' value='$row[0]' id='chkRow$row[0]' class='cbox-default'><label for='chkRow$row[0]'></label></div></td>
								<td>$row[1]</td>
								<td class='center'><i class='mdi $tsAdd icon-xmd'></i><i class='mdi $tsView icon-xmd'></i><i class='mdi $tsEdit icon-xmd'></i><i class='mdi $tsDelete icon-xmd'></i></td>
								<td class='center'><i class='mdi $tcAdd icon-xmd'></i><i class='mdi $tcView icon-xmd'></i><i class='mdi $tcEdit icon-xmd'></i><i class='mdi $tcDelete icon-xmd'></i></td>
								<td class='center'><i class='mdi $stepAdd icon-xmd'></i><i class='mdi $stepView icon-xmd'></i><i class='mdi $stepEdit icon-xmd'></i><i class='mdi $stepDelete icon-xmd'></i></td>
								<td class='center'><i class='mdi $cmdAdd icon-xmd'></i><i class='mdi $cmdView icon-xmd'></i><i class='mdi $cmdEdit icon-xmd'></i><i class='mdi $cmdDelete icon-xmd'></i></td>
								<td class='center'><i class='mdi $objAdd icon-xmd'></i><i class='mdi $objView icon-xmd'></i><i class='mdi $objEdit icon-xmd'></i><i class='mdi $objDelete icon-xmd'></i></td>
								<td class='center'><i class='mdi $taskAdd icon-xmd'></i><i class='mdi $taskView icon-xmd'></i><i class='mdi $taskEdit icon-xmd'></i><i class='mdi $taskDelete icon-xmd'></i></td>
								<td class='center'><i class='mdi $tcManager icon-xmd'></i></td>
								<td class='center'>
									<i data-id='$row[0]' data-role='$row[1]' data-permission='$row[2]|$row[3]|$row[4]|$row[5]|$row[6]|$row[7]|$row[8]|$row[9]|$row[10]|$row[11]|$row[12]|$row[13]|$row[14]|$row[15]|$row[16]|$row[17]|$row[18]|$row[19]|$row[20]|$row[21]|$row[22]|$row[23]|$row[24]|$row[25]|$row[26]' class='mdi mdi-pencil-box text-primary pointer icon-xmd edit-class modalClass' data-icon='pencil-box' data-toggle='modal' data-target='#modalAddRoleForm' title='Edit Role'>
									</i>
								</td>
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