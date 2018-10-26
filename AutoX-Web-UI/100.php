<?php
	$parent_id = $_GET['parent-id'];
	$parent_name = get_parent_info($parent_id, "PARENT_NAME");
	
	echo "<h3>PARENT OBJECT: $parent_id: $parent_name</h3>";
	$next = $parent_id + 1;
	echo "<h3><a href='?id=100&parent-id=$next'>NEXT >></a></h3>";
?>
<table class="table table-bordered table-hover">
	<thead>
		<tr class="bg-color-light">
			<th width="33%" class="center">CHILD_ID</th>
			<th width="34%" class="center">CONTROL_NAME</th>
			<th width="33%">CHILD_TYPE</th>
		</tr>
	</thead>
	<tbody>
		<?php
			$query = "SELECT * FROM OBJECTS_CHILDREN WHERE PARENT_ID='$parent_id' ORDER BY CONTROL_NAME ASC";
			$result = mysqli_query($systemx_connect,$query)or die(mysqli_error());
			while($row = mysqli_fetch_array($result, MYSQL_NUM))
			{
				echo "
					<tr>
						<td class='center'>$row[2]</td>
						<td>$row[6]</td>
						<td>$row[4]</td>
					</tr>
				";
				UpdateTeststeps($row[2], $parent_id);
			}
		?>
	</tbody>
</table>
<?php
	function UpdateTeststeps($childID, $parentID)
	{
		mysqli_query($GLOBALS['systemx_connect'], "UPDATE teststeps SET TEST_DATA=REPLACE(TEST_DATA, '[$childID][$childID]', '[$parentID][$childID]')") or die(mysqli_error());
	}
?>
	