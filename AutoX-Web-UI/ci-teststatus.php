<html lang="en">
<head>
	<?php
		$autoRefresh = true;
		$iconColor = "text-muted";
		$iconTitle = "Slå på auto-refresh hver 5 minutter";
		if (isset($_GET['auto-refresh']))
		{
			$autoRefresh = $_GET['auto-refresh'];
		}
		if ($autoRefresh == "true")
		{
			echo "<meta http-equiv='refresh' content='300'>";
			$iconColor = "text-success";
			$iconTitle = "Slå av auto-refresh";
		}
	?>
</head>
<body>
<div class="card">
		<div>
			<div style="padding-left:10px;float:left" class="text-13">
				<img src="images\jenkins\success.png"> (Success)
				<img src="images\jenkins\failure.png"> (Failed)
				<img src="images\jenkins\aborted.png"> (Aborted)
			</div>
			<div style="float:right;padding-right:10px">
				<span class="mdi mdi-refresh icon-xmd pointer <?php echo $iconColor; ?>" title="<?php echo $iconTitle; ?>" data-auto-refresh="<?php echo $autoRefresh; ?>"></span>
			</div>
		<div>
		<table class="table table-bordered" id="testsuite">
			<?php
				$versions = array("4.1", "5.1", "5.2");
				//$versions = array("4.1");
				foreach ($versions as $version)
				{
					echo "<thead>";
					echo "<tr class='bg-color-light'>";
					if ($version == "4.1")
					{ 
						echo "<th width='20%'>System X $version Ranorex Test</th>";
						echo "
							<th width='13%' class='center'>Set</th>
							<th width='13%' class='center'>Buildnummer</th>
							<th width='13%' class='center'>Teststatus</th>
							<th width='14%' class='center'>Kjøredato</th>
							<th width='14%' class='center'>Varighet</th>
							<th width='13%' class='center'>Aksjon</th>
						";
					}
					else 
					{ 
						echo "<th>System X $version Ranorex Test</th>";
						for ($i=1; $i<=6; $i++) { echo "<th></th>"; }
					}
					echo "</tr>";
					echo "</thead>";
					echo "<tbody>";
					$sets = range('A', 'D');
					$totalDurationInMS = 0;
					$inProgress = 0;
					foreach ($sets as $set)
					{
						$url = getJenkinsAPI($version, $set, "url") . "console"; 
						$buildID = getJenkinsAPI($version, $set, "id");
						$result = trim(strtolower(getJenkinsAPI($version, $set, "result")));
						$icon =  "$result.png";
						if (empty($result))
						{
							$prevResult = strtolower(getPreviousBuildStatus($version, $set, $buildID - 1));
							$icon =  $prevResult . "_anime.gif";
						}
						$timestamp = getJenkinsAPI($version, $set, "timestamp");
						$progress = getProgress($version, $set, $buildID);
						$durationInMS = (empty($result)) ? 0 : getJenkinsAPI($version, $set, "duration");
						$duration = (empty($durationInMS)) ? "<i class='blink bold'>(Testen er pågår...)</i>" : formatMilliseconds($durationInMS);
						
						if (empty($result)) {$inProgress++;}
						else {$inProgress = $inProgress;}
						$totalDurationInMS = $totalDurationInMS + $durationInMS;
						
						$pendingJob = getPendingJobInfo($version, $set);
						$testStatus = "<img src='images\jenkins\\$icon'>";
						$buildNumber = "<a href='$url' target='_blank'>$buildID</a>";
						if (!empty($pendingJob))
						{
							$splitData = explode(":", $pendingJob);
							$buildNumber = $splitData[0] . "<br><i class='text-12'>(Forventet buildnr.)</i>";
							$testStatus = "<img src='images\jenkins\ajax-loader.gif'><br><i class='text-12'>(Ventende: " . $splitData[1] . ")</i>";
						}
						
						echo "<tr>";
						
						//TEST SET
						if ($set == "A") { echo "<td class='center' rowspan=4></td>"; }
						echo "<td class='center'>
							<a href='http://cistaging:8080/view/Ranorex/job/System%20X%20$version%20Ranorex%20Test%20$set/' target='_blank'>$set</a><br>
							(<a href='?id=20&SX-Version=$version&group=$set' target='_blank'>Vis testsuiter etter testgruppe</a>)
						</td>";
						
						//BUILD NUMBER
						echo "<td class='center'>";
						echo $buildNumber;
						echo "</td>";
						
						//TEST STATUS
						echo "<td class='center'>";
						echo $testStatus; 
						
						if (empty($result))
						{
							echo "
								<div class='progressbar-wrapper' data-content-id='#progressbar_$timestamp' title='$progress% fullført...' data-version='$version' data-set='$set' data-build-id='$buildID'>
									<div id='progressbar_$timestamp' class='progressbar-content' style='width:$progress%'>$progress%</div>
								</div>
							";
						}
						echo "</td>";
						
						//RUN DATE
						echo "<td>";
						setlocale(LC_TIME, 'norwegian');
						echo strftime("%d.%B %Y kl. %H:%M", substr($timestamp, 0, 10));
						echo "</td>";
						
						//TEST DURATION
						echo "<td>";
							echo $duration;
						echo "</td>";
						
						//ACTION
						$isLogin = 0;
						$imgAction =  "build";
						$title = "Kjøre denne jobben...";
						if ((isset($_COOKIE['user'])) && (isset($_COOKIE['token'])))
						{
							$isLogin = 1;
						}
						if (empty($result))
						{
							$imgAction = "stop-build";
							$title = "Stopp å kjøre denne jobben...";
						}
						echo "<td class='center'>";
						echo "<img src='images\jenkins\\$imgAction.png' title='$title' class='action-build modalClass' data-is-login='$isLogin' data-icon='account-key' data-sxversion='$version' data-set='$set' data-build-id='$buildID'>";
						echo "</td>";
						echo "</tr>";
					}
					echo "<tr>";
					echo "<td colspan=5 style='font-weight:bold;text-align:right'>TOTAL VARIGHET</td>";
					$totalDuration = ($inProgress > 0) ? "Vennligst vent..." : formatMilliseconds($totalDurationInMS);
					echo "<td class='bold'>$totalDuration</td>";
					echo "<td></td>";
					echo "</tr>";
					echo "</tbody>";
				}
			?>
		</table>
	</div>
	
<!-- ***************** JENKINS LOGIN MODAL **************** -->
<div class="modal fade" id="modalLoginJenkins" tabindex="-1" role="dialog" aria-labelledby="modalTitle" aria-hidden="true">
	<div class="modal-dialog" role="document" style="width:300px">
		<div class="modal-content">
			<div class="modal-header hdr">
				<h5 class="modal-title" id="modalTitle">Jenkins Login</h5>
			</div>
			<form class="forms-sample" action="" method='POST' id="frmLoginJenkins">
				<div class="modal-body">
					<div class="col-sm-9">
						<input type="text" class="form-control" placeholder="Brukernavn" name="user" style="width:250px" required>
					</div>
					<div class="col-sm-9" style="margin-top:5px">
						<input type="text" class="form-control" placeholder="Token" name="token" style="width:250px" required>
						<label class="text-12" style="width:250px">Du kan få din token ved å gå til <b>Jenkins > {Brukernavn} > Configure</b> og deretter klikke <b>Show API Token...</b></label>
					</div>
				</div>
				<div class="modal-footer hdr">
					<button type="button" class="btn btn-primary" id="btnConfirmJenkinsLogin" data-form="#frmLoginJenkins">Logg inn</button>
					<label class="text-13 pointer text-primary top-10" data-dismiss="modal">Avbryt</label>
				</div>
			</form>
			<div id="loginError" class="alert alert-danger hidden text-13"></div>
		</div>
	</div>
</div>
</body>
</html>