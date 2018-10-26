<h4 class="headliner">Brukerliste</h4>
<div class="card-body">
	<input type="radio" name="rbShowOptions" id="rbShowOnly" value="online" <?php echo rb_checked("online",1); ?>> <label class="text-13 pointer <?php echo rb_checked("online",2); ?>" for="rbShowOnly">Vis kun ONLINE</label>
	<input type="radio" name="rbShowOptions" id="rbShowOffline" style="margin-left:20px" value="offline" <?php echo rb_checked("offline",1); ?>> <label class="text-13 pointer <?php echo rb_checked("offline",2); ?>" for="rbShowOffline">Vis kun OFFLINE</label>
	<input type="radio" name="rbShowOptions" id="rbShowAll" style="margin-left:20px" value="all" <?php echo rb_checked("all",1); ?>> <label class="text-13 pointer <?php echo rb_checked("all",2); ?>" for="rbShowAll">Vis alle</label>
	<div class="card">
		<table class="table table-bordered table-hover">
			<thead>
				<tr class="bg-color-light">
					<th width="5%"></th>
					<th width="25%">Etternavn</th>
					<th width="25%">Fornavn</th>
					<th width="20%">Bruker siden</th>
					<th width="20%">Rolle</th>
					<th width="5%">Status</th>
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
					$logged_user = $_SESSION["username"];
					$query = "SELECT * FROM designers WHERE ACTIVE=1 ORDER BY LNAME ASC LIMIT $page, $limit";
					if (isset($_GET['show']))
					{
						$show = $_GET['show'];
						if ($show == "online") { $query = "SELECT * FROM designers WHERE STATUS=1 AND ACTIVE=1 ORDER BY LNAME ASC LIMIT $page, $limit"; }
						if ($show == "offline") { $query = "SELECT * FROM designers WHERE STATUS=0 AND ACTIVE=1 ORDER BY LNAME ASC LIMIT $page, $limit"; }
					}
					$result = mysqli_query($users_connect,$query)or die(mysqli_error());
					$rec_count = get_record_count("designers", "users_connect");
					if ($rec_count == 0) {$curr_page = 0;}
					while($row = mysqli_fetch_array($result, MYSQL_NUM))
					{
						$online_status = ($row[5]) ? "success" : "muted";
						$status = ($row[5]) ? "online" : "offline";
						$role = get_role_name($row[7]);
						echo "
							<tr>
								<td class='center'><img class='img-xs rounded-circle' src='$row[10]'></td>
								<td>$row[2]</td>
								<td>$row[1]</td>
								<td>$row[4]</td>
								<td>$role</td>
								<td class='center'><i class='mdi mdi-account-circle text-$online_status' style='font-size:25px' title='$row[1] $row[2] er $status'></i></td>
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