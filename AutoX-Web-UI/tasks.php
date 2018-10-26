<div class="row">
	<div class="col-12 grid-margin">
		<div class="card">
			<div class="card-body">
				<h5 class="card-title mb-4"><i class="mdi mdi-note-text text-primary icon-lg"></i> Oppgaver</h5>
				<div class="fluid-container text-13">
					<?php
						$task_limit = 10;
						$logged_user = $_SESSION["username"];
						$query = "SELECT * FROM tasks WHERE USER='$logged_user' ORDER BY TASK_DATE DESC LIMIT $task_limit";
						$result = mysqli_query($users_connect,$query)or die(mysqli_error());
						$rows = mysqli_num_rows($result);
						while($row = mysqli_fetch_array($result, MYSQL_NUM))
						{
							$lc_status = strtolower($row[3]);
							$status_class = str_replace(" ", "-", $lc_status);
							echo "
								<div class='row ticket-card mt-3 pb-2 border-bottom'>
									<div class='col-2'>
										$row[1]
									</div>
									<div class='ticket-details col-8'>
										<div class='d-flex'>
											<p class='text-gray mb-0 ellipsis'>$row[5]: $row[2]</p>
										</div>
									</div>
									<div class='col-2'>
										<div class='status $status_class'>$row[3]</div>
									</div>
								</div>
							";
						}
					?>
					
					<div style="margin-top:20px;float:right" class="text-13">
						<a href="?id=6">
							<?php
								$href_text = "Ingen oppgave registrert. Registrer ny her.";
								if ($rows > 0)
								{
									$href_text = "Registrer ny her eller vis alle";
									$rec_count = get_record_count("tasks", "users_connect");
									if ($rec_count > $task_limit)
									{
										$more_rows = $rec_count - $task_limit;
										$href_text = "Vis <b class='text-warning'>$more_rows</b> flere elemente(r)...";
									}
								}
								echo $href_text;
							?>
						</a>
					</div>
				</div>
			</div>
		</div>
	</div>
</div>