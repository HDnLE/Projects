<h4 class="headliner">Activity Logs</h4> 
<div style='float:right'>
	<button type="button" class="btn btn-primary modalClass" data-toggle="modal" data-icon="speedometer" title="Threshold Limit" data-target="#modalLogLimit">Threshold Limit</button>
	<button type="button" class="btn btn-primary modalClass" data-toggle="modal" data-icon="delete-sweep" data-target="#modalCleanLogs" title="Clean Logs">Clean Logs</button>
</div>
<br>
<br>
<div class="card-body">
	<div class="card">
		<table class="table table-bordered table-hover">
			<thead>
				<tr class="bg-color-light">
					<th width="10%">Date/Time</th>
					<th width="60%">Activity Log</th>
					<th width="30%">User</th>
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
					$query = "SELECT * FROM activity_logs ORDER BY LOG_DATE DESC LIMIT $page, $limit";
					$result = mysqli_query($users_connect,$query)or die(mysqli_error());
					
					if ($rec_count == 0) {$curr_page = 0;}
					while($row = mysqli_fetch_array($result, MYSQL_NUM))
					{
						$user_login = get_user_info($row[3], 1) . " " . get_user_info($row[3], 2);
						echo "
							<tr>
								<td>$row[1]</td>
								<td>$row[2]</td>
								<td>$user_login</td>
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