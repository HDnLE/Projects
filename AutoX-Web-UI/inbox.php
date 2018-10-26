<div class="card-body">
	<a href="?id=18" class="text-13 right">Vis sendte meldinger</a>
	<br>
	<div class="card">
		<div class="btn-toolbar" role="toolbar">
			<div class="btn-group" role="group" aria-label="First group" style="border:0px">
				<button type="button" title="Send ny melding" class="btn btn-inverse-light icon-btn" style="padding:5px" data-toggle='modal' data-target='#modalMessageForm'><span class="mdi mdi-email-variant mdi-24px"></span> Ny melding</button>
				<button type="button" title="Slett valgt registrering..." class="btn btn-inverse-light icon-btn" style="padding:5px" id="mnuBtnDelete"><span class="mdi mdi-delete mdi-24px"></span> Slett</button>
				<button type="button" title="Vis nyeste innhold i aktiv side..." class="btn btn-inverse-light icon-btn" style="padding:5px" id="mnuBtnRefresh"><span class="mdi mdi-refresh mdi-24px"></span> Refresh</button>
			</div>
		</div>
		
		<table class="table table-bordered" id="testsuite">
			<thead>
				<tr class="bg-color-light">
					<th width="2%"><input type='checkbox' id="chkAction"></th>
					<th width="22%">Avsender</th>
					<th width="60%">Emne</th>
					<th width="16%">Dato</th>
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
					$query = "SELECT * FROM messages WHERE RECEIVER='$logged_user' ORDER BY DATE_SEND DESC LIMIT $page, $limit";
					$result = mysqli_query($users_connect,$query)or die(mysqli_error());
					$rec_count = get_record_count("messages", "users_connect");
					if ($rec_count == 0) {$curr_page = 0;}
					while($row = mysqli_fetch_array($result, MYSQL_NUM))
					{
						$read = ($row[7]) ? "" : "bold";
						$sender = get_user_info($row[3], 1) . " " . get_user_info($row[3], 2);
						echo "
							<tr class='$read row-message' data-message-id='$row[1]'>
								<td class='center'><input type='checkbox' value='$row[0]' class='cbox-default'></td>
								<td>$sender</td>
								<td>$row[5]</td>
								<td>" . date("d. F Y", strtotime($row[2])) . "</td>
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
			if ($total_page <= 1)
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