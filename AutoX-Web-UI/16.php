<h4 class="headliner">Scripts on XStudio</h4>
<div class="card-body">
	<div class="card" style="padding:10px">
		<p class="help-content">&nbsp;</p>
		<h5>Setup XStudio to Run Automated Scripts Remotely</h5>
		<p class="help-content">&nbsp;</p>
		<p class="help-content">
			Running automated scripts to any of the remote computers is one of the features of XStudio. XStudio comes with an agent called 
			<a href="http://www.xqual.com/documentation/xagent/xagent.html" target="_blank">XAgent</a> that you can run in 
			background on any computer. XAgent is aimed at being run in background and pool the database to check if there are jobs ready for it. If a session is 
			scheduled to be executed on this agent, the agent will pick the job and will execute it.
		</p>
		
		<p class="help-content">&nbsp;</p>
		<p style="font-weight:bold">Pre-requisites</p>
		<ul style="margin:-15px 0px 0px -20px">
			<li>All the Ranorex scripts must have been compiled OR</li>
			<li>Run the <b>SystemX Test Utility</b> to create and compile the scripts</li>
		</ul>
		
		<p class="help-content">&nbsp;</p>
		<p style="font-weight:bold" id="copy-testsuite">Create a Dedicated Category for Automated Tests</p>
		<ol>
			<li>On the side menu, click on <b>Tests</b> <img src="images\help\xstudio-tests.png"></li>
			<li>In the Test tree, select the root folder <img src="images\help\xstudio-root.png"> and click on create category button <img src="images\help\xstudio-create-category.png"></li>
			<li>Key in the name of the category (ie. <b>Automated Tests</b>) and select <b>ranorex.jar</b> from the list of launchers</li>
				<img src="images\help\xstudio-category-creation.png" height="500">
			<li>Click on <b>Submit</b> button when done</li>
			<li>Select the newly created category (ie. <b>Automated Tests</b>)</li>
			<li>On the right pane, click on create folder <img src="images\help\xstudio-create-folder.png"> button</li>
			<li>Key in name of the folder (ie. <b>SystemX</b>) then click on Submit button when done</li>
			<li>On the right pane, click on <b>create test</b> <img src="images\help\xstudio-create-test.png"> button</li>
			<li>Key in the name of the test (ie. <b>TC_100_Login_With_Invalid_Master_Key</b>)</li>
				<b>IMPORTANT:</b> The name must be the filename of the executable script (<mark><code>&lt;name_of_the_script&gt;.exe</code></mark>)
			<li>Key in the Canonical path (ie. <b>TC_100_Login_With_Invalid_Master_Key</b>)</li>
				<b>IMPORTANT:</b> The canonical path is the sub-folder where the executable script is located. For example, your script to be run is at 
		</ol>
	</div>
</div>