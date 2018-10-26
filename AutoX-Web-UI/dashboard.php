<div class="row">
	<div class="col-xl-3 col-lg-3 col-md-3 col-sm-6 grid-margin stretch-card">
		<div class="card card-statistics">
			<div class="card-body">
				<div class="clearfix">
					<div class="float-left">
						<i class="mdi mdi-hexagon-multiple text-danger icon-lg"></i>
					</div>
					<div class="float-right">
						<p class="card-text text-right">Testsuiter</p>
						<div class="fluid-container">
							<h3 class="card-title font-weight-bold text-right mb-0"><?php echo get_testsuite_count(); ?></h3>
						</div>
					</div>
				</div>
				<p class="text-muted mt-3">
					<i class="mdi mdi-alert-octagon mr-1 text-info" aria-hidden="true"></i> Nåværende antall testsuiter med FEIL status: <b><?php echo getTestStatusCount("TS", 0); ?></b>
				</p>
			</div>
		</div>
	</div>

	<div class="col-xl-3 col-lg-3 col-md-3 col-sm-6 grid-margin stretch-card">
		<div class="card card-statistics">
			<div class="card-body">
				<div class="clearfix">
					<div class="float-left">
						<i class="mdi mdi-hexagon text-warning icon-lg"></i>
					</div>
					<div class="float-right">
						<p class="card-text text-right">Testcaser</p>
						<div class="fluid-container">
							<h3 class="card-title font-weight-bold text-right mb-0"><?php echo number_format(get_testcase_count(),0,0," "); ?></h3>
						</div>
					</div>
				</div>
				<p class="text-muted mt-3">
					<i class="mdi mdi-alert-octagon mr-1 text-info" aria-hidden="true"></i> Antall alle testcaser
				</p>
			</div>
		</div>
	</div>
	
	<div class="col-xl-3 col-lg-3 col-md-3 col-sm-6 grid-margin stretch-card">
		<div class="card card-statistics">
			<div class="card-body">
				<div class="clearfix">
					<div class="float-left">
						<i class="mdi mdi-hexagon text-success icon-lg"></i>
					</div>
					<div class="float-right">
						<p class="card-text text-right">Aktive testcaser</p>
						<div class="fluid-container">
							<h3 class="card-title font-weight-bold text-right mb-0"><?php echo number_format(get_active_testcase_count(),0,0," "); ?></h3>
						</div>
					</div>
				</div>
				<p class="text-muted mt-3">
					<i class="mdi mdi-alert-octagon mr-1 text-info" aria-hidden="true"></i> Nåværende antall testcaser med FEIL status: <b><?php echo getTestStatusCount("TC", 0); ?></b>
				</p>
			</div>
		</div>
	</div>
	
	<div class="col-xl-3 col-lg-3 col-md-3 col-sm-6 grid-margin stretch-card">
		<div class="card card-statistics">
			<div class="card-body">
				<div class="clearfix">
					<div class="float-left">
						<i class="mdi mdi-jira text-primary icon-lg"></i>
					</div>
					<div class="float-right">
						<p class="card-text text-right">Jira tickets</p>
						<div class="fluid-container">
							<h3 class="card-title font-weight-bold text-right mb-0">
								<?php 
									$result = mysqli_query($systemx_connect, "SELECT COUNT(DISTINCT JIRA_TICKET) AS COUNT FROM testcases WHERE JIRA_TICKET IS NOT NULL");
									$row = mysqli_fetch_assoc($result);
									echo $row['COUNT'];
								?>
							</h3>
						</div>
					</div>
				</div>
				<p class="text-muted mt-3">
					<i class="mdi mdi-alert-octagon mr-1 text-info" aria-hidden="true"></i> Antall Jira tickets opprettet (<a href="?id=18">Vis testcaser</a>)
				</p>
			</div>
		</div>
	</div>
</div>

<?php
	$role_id = get_user_info($_SESSION["username"], 7);
	if (get_role_info($role_id, 19))
	{
		include "tasks.php";
	}
?>

<div class="row">
	<div class="col-12 grid-margin">
		<div class="card">
			<div class="card-body">
				<h5 class="card-title mb-4"><i class="mdi mdi-update text-primary icon-lg"></i> Endringer/Oppdateringer</h5>
				<div class="fluid-container text-13">
					<?php
						$updates_limit = 10;
						$query = "SELECT * FROM update_log ORDER BY LOG_DATE DESC LIMIT $updates_limit";
						$result = mysqli_query($users_connect,$query)or die(mysqli_error());
						$rows = mysqli_num_rows($result);
						while($row = mysqli_fetch_array($result, MYSQL_NUM))
						{
							echo "
								<div class='row ticket-card mt-3 pb-2 border-bottom'>
									<div class='col-2'>
										$row[1]
									</div>
									<div class='ticket-details col-9'>
										<div class='d-flex'>
											<p class='text-gray mb-0 ellipsis'>" . nl2br($row[2]) . "</p>
										</div>
									</div>
								</div>
							";
						}
					?>
					<div style="margin-top:20px;float:right" class="text-13">
						<a href="?id=5">
							<?php
								$href_text = "Ingen endringer/oppdateringer registrert. Registrer ny her.";
								if ($rows > 0)
								{
									$href_text = "Registrer ny her eller vis alle";
									$rec_count = get_record_count("update_log", "users_connect");
									if ($rec_count > $updates_limit)
									{
										$more_rows = $rec_count - $updates_limit;
										$href_text = "Vis <b class='text-warning'>$more_rows</b> flere elemente(r)...";
									}
								}
								if (is_admin($_SESSION["username"]))
								{
									echo $href_text;
								}
							?>
						</a>
					</div>
				</div>
			</div>
		</div>
	</div>
</div>
