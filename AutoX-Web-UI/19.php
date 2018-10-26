<div class='alert alert-danger text-13 hidden' id="divAlert">
</div>
<h4 class="headliner" id="category" data-category='testsuite'>CI-Teststatus</h4>
<div class="card-body">
	<?php
		$response = checkJenkinsAPIResponse();
		if ($response == "SUCCESS")
		{
			include "ci-teststatus.php";
		}
		else
		{
			echo "<div class='alert alert-danger'>$response</div>";
		}
	?>
</div>