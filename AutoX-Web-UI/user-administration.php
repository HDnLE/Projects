<h4 class="headliner" id="category" data-category='users'>User Administration</h4>
<div class="card-body">
	<input type="radio" name="rbOptions" id="rbShowActivate" value="activated" <?php echo rb_checked("activated",1); ?>> <label class="text-13 pointer <?php echo rb_checked("activated",2); ?>" for="rbShowActivate">Show activated users only</label>
	<input type="radio" name="rbOptions" id="rbShowDeactivate" style="margin-left:20px" value="deactivated" <?php echo rb_checked("deactivated",1); ?>> <label class="text-13 pointer <?php echo rb_checked("deactivated",2); ?>" for="rbShowDeactivate">Show deactivated users only</label>
	<input type="radio" name="rbOptions" id="rbShowAll" style="margin-left:20px" value="all" <?php echo rb_checked("all",1); ?>> <label class="text-13 pointer <?php echo rb_checked("all",2); ?>" for="rbShowAll">Show all</label>
	<div class="card">
		<div class="btn-toolbar" role="toolbar">
			<div class="btn-group" role="group" aria-label="First group" style="border:0px">
				<button type="button" title="Add New User" class="btn btn-inverse-light icon-btn modalClass" style="padding:5px" data-toggle='modal' data-icon="account-plus" data-target='#modalAddUserForm' id="mnuBtnNew"><span class="mdi mdi-account-plus mdi-24px text-warning"></span> Add New User</button>
				<button type="button" title="Activate/Deactivate User" class="btn btn-inverse-light icon-btn modalClass" data-icon="account-off" style="padding:5px" id="mnuBtnActivate"><span class="mdi mdi-account-off mdi-24px text-warning"></span> Activate/Deactivate User</button>
				<button type="button" title="Reset Password" class="btn btn-inverse-light icon-btn modalClass" data-icon="lock-reset" style="padding:5px" id="mnuBtnReset"><span class="mdi mdi-lock-reset mdi-24px text-warning"></span> Reset Password</button>
			</div>
		</div>
		<table class="table table-bordered table-hover" id="testsuite">
			<thead>
				<tr class="bg-color-light">
					<th width="2%"><div class="round"><input type="checkbox" class="pointer" id="chkAction"> <label for="chkAction"></label></div></th>
					<th width="30%">Last Name</th>
					<th width="25%">First Name</th>
					<th width="20%">Username</th>
					<th width="10%">Role</th>
					<th width="8%">Status</th>
					<th width="5%"></th>
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
					$query = "SELECT * FROM designers WHERE USERNAME <>'admin' ORDER BY LNAME ASC LIMIT $page, $limit";
					if (isset($_GET['show']))
					{
						$show = $_GET['show'];
						if ($show == "activated") { $query = "SELECT * FROM designers WHERE USERNAME <>'admin' AND ACTIVE=1 ORDER BY LNAME ASC LIMIT $page, $limit"; }
						if ($show == "deactivated") { $query = "SELECT * FROM designers WHERE USERNAME <>'admin' AND ACTIVE=0 ORDER BY LNAME ASC LIMIT $page, $limit"; }
					}
					$result = mysqli_query($users_connect,$query)or die(mysqli_error());
					$rec_count = get_record_count("designers", "users_connect");
					if ($rec_count == 0) {$curr_page = 0;}
					while($row = mysqli_fetch_array($result, MYSQL_NUM))
					{
						$online_status = ($row[12]) ? "success" : "muted";
						$icon = ($row[12]) ? "" : "-off";
						$role = get_role_name($row[7]);
						echo "
							<tr>
								<td class='center'><div class='round' style='margin-left:10px'><input type='checkbox' value='$row[3]' id='chkRow$row[0]' class='cbox-default'><label for='chkRow$row[0]'></label></div></td>
								<td>$row[2]</td>
								<td>$row[1]</td>
								<td>$row[3]</td>
								<td>$role</td>
								<td class='center'><i class='mdi mdi-account$icon text-$online_status' style='font-size:25px'></i></td>
								<td class='center'><i data-id='$row[0]' data-role='$row[7]' data-username='$row[3]' data-active='$row[12]' class='mdi mdi-pencil-box text-primary pointer icon-xmd edit-class modalClass' data-icon='pencil-box' data-toggle='modal' data-target='#modalAddUserForm' title='Edit Info ($row[1] $row[2])'></i></td>
							</tr>
						";
					}
				?>
			</tbody>
		</table>
	</div>
	<a href="?id=21" class="text-13">View user roles</a>
	<br>
		<?php
			$first_button = "disabled";
			$next_button = "";
			$prev_button = "disabled";
			$last_button = "";
			$page_select = "";
			$total_page = ceil($rec_count/$limit);
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