<table class="table table-bordered table-hover">
	<thead>
		<tr class="bg-color-light">
			<th width="20%" class="center">PARENT_ID</th>
			<th width="20%" class="center">windowTitle</th>
			<th width="20%">RX_PATH</th>
			<th width="20%">CONTROL_NAME</th>
			<th width="20%">DESCRIPTION</th>
		</tr>
	</thead>
	<tbody>
		<?php
			$query = "SELECT * FROM OBJECTS_CHILDREN WHERE CHILD_TYPE='Form' AND PARENT_ID IS NULL ORDER BY windowTitle ASC LIMIT 1";
			$result = mysqli_query($systemx_connect,$query)or die(mysqli_error());
			while($row = mysqli_fetch_array($result, MYSQL_NUM))
			{
				echo "
					<tr>
						<td class='center'>$row[1]</td>
						<td>$row[3]</td>
						<td>$row[5]</td>
						<td>$row[6]</td>
						<td>$row[7]</td>
					</tr>
				";
				insertObject($row[3], $row[5], $row[6], $row[7]);
			}
		?>
	</tbody>
</table>
<?php
	function insertObject($parentName, $rxPath, $controlName, $description)
	{
		$parent_id = get_new_parent_id();
		mysqli_query($GLOBALS['systemx_connect'], "INSERT INTO OBJECTS_PARENTS (PARENT_ID, PARENT_NAME, RX_PATH, CONTROL_NAME, DESCRIPTION) VALUES ('$parent_id', '$parentName', \"$rxPath\", '$controlName', '$description')") or die(mysqli_error());
		mysqli_query($GLOBALS['systemx_connect'], "UPDATE OBJECTS_CHILDREN SET PARENT_ID=$parent_id WHERE windowTitle='$parentName'") or die(mysqli_error());
		//echo "INSERT INTO OBJECTS_PARENTS (PARENT_ID, PARENT_NAME, RX_PATH, CONTROL_NAME, DESCRIPTION) VALUES ('$parent_id', '$parentName', \"$rxPath\", '$controlName', '$description')";
	}
?>
	