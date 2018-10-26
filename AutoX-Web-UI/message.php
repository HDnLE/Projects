<div class="card-body">
	<a href="?id=17" class="text-13 right">Tilbake til innboksen</a>
	<br>
	<div class="card">
		<?php
			$message_id = $_GET['message'];
			$query = "SELECT * FROM messages WHERE MESSAGE_ID='$message_id' ORDER BY DATE_SEND DESC";
			$result = mysqli_query($users_connect,$query)or die(mysqli_error());
			while($row = mysqli_fetch_array($result, MYSQL_NUM))
			{
				$avatar = get_user_info($row[3], 10);
				$sender = get_user_info($row[3], 1) . " " . get_user_info($row[3], 2);
				echo "
					<div>
						<div style='float:left;width:5%'>
							<img class='img-lg rounded-circle' src='$avatar'>
						</div>
						<div style='float:right;width:95%;padding-top:10px'>
							<h4>$sender</h4>
							<p class='subject'>$row[5]</p>
							<p class='text-13' style='margin-top:-15px'>$row[2]</p>
						</div>
					</div>
					<div class='message-content text-13'>
							" . nl2br($row[6]) . "
					</div>
				";
			}
		?>
	</div>
</div>