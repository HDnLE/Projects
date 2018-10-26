<h4 class="headliner" id="category" data-category='tasks'>Testcaser med Jira tickets</h4>
<div class="card-body">
	<div class="card">
		<table class="table table-bordered">
			<thead>
				<tr class="bg-color-light">
					<th width="5%" class='center'>TS-ID</th>
					<th width="5%" class='center'>TC-ID</th>
					<th width="45%">Tittel</th>
					<th width="38%">Kommentar</th>
					<th width="7%">JIRA Ticket</th>
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
					$query = "SELECT TS_ID, TC_ID, JIRA_TICKET, COMMENTS, TC_TITLE FROM testcases WHERE JIRA_TICKET IS NOT NULL AND JIRA_TICKET <> '' ORDER BY TS_ID, TC_ID ASC LIMIT $page, $limit";
					$result = mysqli_query($systemx_connect,$query)or die(mysqli_error());
					$result2 = mysqli_query($systemx_connect, "SELECT COUNT(*) AS COUNT FROM testcases WHERE JIRA_TICKET IS NOT NULL AND JIRA_TICKET <> ''");
					$row2 = mysqli_fetch_assoc($result2);
					$testcase_count = $row2['COUNT'];
					while($row = mysqli_fetch_array($result, MYSQL_NUM))
					{
						$id = $row[0];
						$testsuite_title = get_testsuite_info($row[0], 2);
						echo "
							<tr>
								<td class='center'><a href='?id=2&ts-id=$row[0]&tc-id=$row[1]'>$row[0]</a></td>
								<td class='center'><a href='?id=2&ts-id=$row[0]&tc-id=$row[1]'>$row[1]</a></td>
								<td><a href='?id=2&ts-id=$row[0]&tc-id=$row[1]'>$row[4]</a></td>
								<td>$row[3]</td>
								<td><a href='http://jiracon-appsrv:8080/browse/$row[2]' target='_blank'>$row[2]</a></td>
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