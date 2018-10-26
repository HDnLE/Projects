<?php
	$testGroup = (isset($_GET['group'])) ? $_GET['group'] : "<i class='text-danger'>Ukjent gruppe</i>";
	$sxVersion = (isset($_GET['SX-Version'])) ? $_GET['SX-Version'] : "<i class='text-danger'>Ukjent versjon</i>";
?>	
<h4 class="headliner" id="category" data-category='testsuite'><?php echo "Testsuiter etter SX versjon ($sxVersion) og testgruppe ($testGroup)"; ?> </h4>
<div class="card-body">
	<div class="card">
		<table class="table table-bordered" id="testsuite">
			<thead>
				<tr class="bg-color-light">
					<th width="5%" class="center">TS-ID</th>
					<th width="37%">Testsuite tittel</th>
					<th width="15%" class="center">Siste commitdato</th>
					<th width="15%">Sist endret av</th>
					<th width="10%" class="center">Siste kjøredato</th>
					<th width="10%" class="center">Kjører fra</th>
					<th width="8%" class="center">Teststatus</th>
				</tr>
			</thead>
			<tbody>
				<?php
					$page = 0;
					$default_limit = 25;
					$limit = $default_limit;
					$curr_page = 1;
					$database = "sxtest_" . str_replace(".", "", $sxVersion);
					$hostname = '192.168.10.61';
					$username = 'ta_admin';
					$password = 'dhocc648';
					$sx_connect_20 = mysqli_connect($hostname, $username, $password, $database);
					
					if (isset($_GET['page'])) 
					{
						$curr_page = $_GET['page'];
						$limit = $_GET['limit']; 
						$page = (($curr_page - 1) * $limit);
					}
					$curr_page++;
					$limit = ($limit == "Alle" ? $limit = 999999 : $limit);
					$query = "SELECT * FROM testsuites WHERE TEST_GROUP='$testGroup' ORDER BY ts_id ASC";
					$result = mysqli_query($sx_connect_20,$query)or die(mysqli_error());
					$testsuite_count = get_testsuite_count();
										
					while($row = mysqli_fetch_array($result, MYSQL_NUM))
					{
						$fname = ucwords(strtolower(get_user_info($row[4], 1)));
						$user_info = $fname{0} . ". " . ucwords(strtolower(get_user_info($row[4], 2)));
						$id = $row[0];
						
						$commitDate = (empty($row[8])) ? "" : date("d.m.Y H:i", strtotime($row[8]));
						$run_test_from = (empty($row[9])) ? "" : "<a href='http://cistaging:8080/computer/$row[9]/' target='_blank'>$row[9]</a>";
						$run_status = "<img src='images/jenkins/aborted.png' title='Not Executed'>";
						$run_date = "";
						$disabled = ($row[11]) ? "" : "text-disabled";
						if (!$row[11])
						{
							$row[6] = null;
							$row[9] = "";
						}
						if ($row[6] != null)
						{
							$run_date = date("d.m.Y H:i", strtotime($row[7]));
							//$run_status = (!$row[6]) ? "<span class='mdi mdi-thumb-down icon-sm text-danger'></span>":"<span class='mdi mdi-thumb-up icon-sm text-success'></span>";
							$run_status = (!$row[6]) ? "<img src='images/jenkins/failure.png' title='Failed'>":"<img src='images/jenkins/success.png' title='Success'>";
						}
						$run_test_from = (empty($row[9])) ? "" : "<a href='http://cistaging:8080/computer/$row[9]/' target='_blank'>$row[9]</a>";
						$tsDescription = str_replace("<b>", "", $row[3]);
						$tsDescription = str_replace("</b>", "", $tsDescription);
						echo "
							<tr class='$disabled'>
								<td class='center'><a href='?id=2&ts-id=$row[1]'>$row[1]</a></td>
								<td title='$tsDescription'><a href='?id=2&ts-id=$row[1]'>$row[2]</a></td>
								<td class='center' title='$row[10]'>$commitDate</td>
								<td>$user_info</td>
								<td class='center'>$run_date</td>
								<td class='center'>$run_test_from</td>
								<td class='center'>$run_status</td>
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