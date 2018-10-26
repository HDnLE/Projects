<h4 class="headliner">Web UI</h4>
<div class="card-body">
	<div class="card" style="padding:10px">
		
		<p style="text-align:justify">
			Instead of entering the instructions manually to the MySql database, we design an interface to help you do that. This web-based interface uses PHP web programming language to 
			design it and accesses MySql database which the utility uses.
		</p>
		<p style="font-weight:bold">This page contains:</p>
		<ul>
			<li><a href="#testsuites">Testsuites</a></li>
			<ul>
				<li><a href="#testsuites">Overview</a></li>
				<li><a href="#add-testsuite">Add a New Testsuite</a></li>
				<li><a href="#edit-testsuite">Modify a Testsuite</a></li>
				<li><a href="#copy-testsuite">Copy a Testsuite</a></li>
				<li><a href="#delete-testsuite">Delete a Testsuite</a></li>
			</ul>
			<li>Testcases</li>
			<ul>
				<li><a href="#testcase">Overview</a></li>
				<li><a href="#add-testcase">Add a New Testcase</a></li>
				<li><a href="#edit-testcase">Modify a Testcase</a></li>
				<li><a href="#activate-deactivate-testcase">Activate/Deactivate a Testcase</a></li>
				<li><a href="#delete-testcase">Delete a Testcase</a></li>
				<li><a href="#copy-testcase">Copy a Testcase</a></li>
				<li><a href="#move-testcase">Move a Testcase to a New Testsuite</a></li>
			</ul>
			<li><a href="#teststeps">Test Steps</a></li>
			<ul>
				<li><a href="#teststeps">Overview</a></li>
				<li><a href="#add-teststep">Add/Insert a New Test Step</a></li>
				<li><a href="#edit-teststep">Modify a Test Step</a></li>
				<li><a href="#activate-deactivate-teststep">Enable/Disable a Test Step</a></li>
				<li><a href="#delete-teststep">Delete a Test Step</a></li>
				<li><a href="#copy-teststep">Copy a Test Step</a></li>
				<li><a href="#move-teststep">Move a Test Step</a></li>
			</ul>
		</ul>
		<h5 id="testsuites">Testsuites</h5>
		<p style="font-weight:bold">Overview</p>
		<p class="help-content">
			A test suite is a collection of test cases that are intended to be used to test a software program to show that it has some specified set of behaviours. 
			A test suite often contains detailed instructions or goals for each collection of test cases and information on the system configuration to be used 
			during testing. A group of test cases may also contain prerequisite states or steps, and descriptions of the following tests.
		</p>
		
		<p class="help-content">&nbsp;</p>
		<p class="help-content">
			Testsuite can be accessed by going to <a href="?id=1">Testskripter -> Testsuiter</a>
		</p>
		<p class="help-content">&nbsp;</p>
		<p class="help-content" style="text-align:right">
			<b>Source: </b> <a href="https://en.wikipedia.org/wiki/Test_suite" target="_blank">https://en.wikipedia.org/wiki/Test_suite</a>
		</p>
		<p style="font-weight:bold" id="add-testsuite">Add a New Testsuite</p>
		<ol>
			<li>Click on <b>Ny registrering</b> button</li>
			<li>Write a title of the testsuite in the <b>Tittel</b> field</li>
			<li>Write a short description of the testsuite in the <b>Beskrivelse</b> field</li>
			<li>Click on <b>Lagre</b> button</li>
		</ol>
		
		<p class="help-content">&nbsp;</p>
		<p style="font-weight:bold" id="edit-testsuite">Modify a Testsuite</p>
		<ol>
			<li>Click on this edit button <i class='mdi mdi-pencil-box-outline text-primary'></i>. (Located at the rightmost of the testsuite you want to modify)</li>
			<li>Make your changes</li>
			<li>Click on <b>Lagre</b> button</li>
		</ol>
		
		<p class="help-content">&nbsp;</p>
		<p style="font-weight:bold" id="copy-testsuite">Copy a Testsuite</p>
		<ol>
			<li>Select a testsuite you want to copy by ticking its checkbox</li>
			<li>Click on <b>Kopi</b> button</li>
			<li>Tick <b>Kopi også testcaser</b> checkbox IF you want to include its testcases in copying the testsuite</li>
			<li>Click on <b>Bekreft</b> button</li>
		</ol>
		
		<p class="help-content">&nbsp;</p>
		<p style="font-weight:bold" id="delete-testsuite">Delete a Testsuite</p>
		<ol>
			<li>Select a testsuite you want to delete by clicking on the edit button <i class='mdi mdi-pencil-box-outline text-primary'></i></li>
			<li>Click on <b>Slett</b> button</li>
			<li>Click on <b>OK</b> button if you want to continue</li>
		</ol>
		<p class="help-content">&nbsp;</p>
		<p class="help-content">Or...</p>
		<ol>
			<li>Select a testsuite you want to delete by ticking its checkbox</li>
			<li>Click on <b>Slett</b> button</li>
			<li>Click on <b>OK</b> button if you want to continue</li>
		</ol>
		
		<p class="help-content">&nbsp;</p>
		<div class="alert alert-warning text-13"><b>Note:</b> Deleting a testsuite will also delete all testcases associated on the selected testsuite.</div>
		
		<p class="help-content">&nbsp;</p>
		<h5 id="testcase">Testcases</h5>
		<p style="font-weight:bold">Overview</p>
		<p class="help-content">
			A test case is a set of conditions or variables under which a tester will determine whether an application or software system is working correctly 
			or not. It may take many test cases to determine that a software program or system is considered sufficiently scrutinized to be released. 
			Test cases are often referred to as test scripts, particularly when written. Written test cases are usually collected into test suites.
		</p>
		<p class="help-content">&nbsp;</p>
		<p class="help-content">
			Testcase can be accessed by going to any of the following:
		</p>
		<ul>
			<li><a href="?id=2">Testskripter -> Testcaser</a> (This will list down all the combined test cases of all testsuites. If you want to view test cases of specific testsuite, just select a testsuite from the dropdown list which is located on the right side of the page)</li>
			<li><a href="?id=1">Testskripter -> Testsuiter</a> and then click on a Testsuite ID</li>
		</ul>
		<p class="help-content">&nbsp;</p>
		<p class="help-content" style="text-align:right">
			<b>Source: </b> <a href="https://en.wikipedia.org/wiki/Test_case" target="_blank">https://en.wikipedia.org/wiki/Test_case</a>
		</p>
		<p style="font-weight:bold" id="add-testcase">Add a New Testcase</p>
		<ol>
			<li>Click on <b>Ny registrering</b> button</li>
			<li>Write a title of the testcase in the <b>Tittel</b> field</li>
			<li>Select a testsuite from the dropdown list</li>
			<li>Write a comment in the <b>Kommentar</b> field (Optional)</li>
			<li>Click on <b>Lagre</b> button</li>
		</ol>
		<p class="help-content">&nbsp;</p>
		<p style="font-weight:bold" id="edit-testcase">Modify a Testcase</p>
		<ol>
			<li>Click on this edit button <i class='mdi mdi-pencil-box-outline text-primary'></i>. (Located at the rightmost of the testcase you want to modify)</li>
			<li>Make your changes</li>
			<li>Click on <b>Lagre</b> button</li>
		</ol>
		
		<p class="help-content">&nbsp;</p>
		<p style="font-weight:bold" id="activate-deactivate-testcase">Activate/Deactivate a Testcase</p>
		<ol>
			<li>Select a testcase you want to activate/deactivate by clicking on the edit button <i class='mdi mdi-pencil-box-outline text-primary'></i></li>
			<li>Click on <b>Aktiver</b> or <b>Deaktiver</b> button</li>
		</ol>
		<p class="help-content">&nbsp;</p>
		<p class="help-content">Or...</p>
		<ol>
			<li>Select a testcase you want to delete by ticking its checkbox</li>
			<li>Click on <b>Aktiver/Deaktiver</b> button</li>
			<li>Select <b>Kjør skriptet</b> option if you want the selected testcases to be activated. <b>Ikke kjør skriptet</b> if deactivated</li>
			<li>Click on <b>Bekreft</b> button</li>
		</ol>
		
		<p class="help-content">&nbsp;</p>
		<p style="font-weight:bold" id="delete-testcase">Delete a Testcase</p>
		<ol>
			<li>Select a testcase you want to delete by clicking on the edit button <i class='mdi mdi-pencil-box-outline text-primary'></i></li>
			<li>Click on <b>Slett</b> button</li>
			<li>Click on <b>OK</b> button if you want to continue</li>
		</ol>
		<p class="help-content">&nbsp;</p>
		<p class="help-content">Or...</p>
		<ol>
			<li>Select a testcase you want to delete by ticking its checkbox</li>
			<li>Click on <b>Slett</b> button</li>
			<li>Click on <b>OK</b> button if you want to continue</li>
		</ol>
		
		<p class="help-content">&nbsp;</p>
		<p style="font-weight:bold" id="copy-testcase">Copy a Testcase</p>
		<ol>
			<li>Select a testcase you want to copy by ticking its checkbox</li>
			<li>Click on <b>Kopi</b> button</li>
			<li>Select a testsuite from the dropdown list where the selected testcases to be copied to</li>
			<li>Click on <b>Bekreft</b> button</li>
		</ol>
		
		<p class="help-content">&nbsp;</p>
		<p style="font-weight:bold" id="move-testcase">Move a Testcase to a New Testsuite</p>
		<ol>
			<li>View testcases per testsuite</li>
			<li>Select a testcase you want to copy by ticking its checkbox</li>
			<li>Click on <b>Flytt</b> button</li>
			<li>Select a testsuite from the dropdown list where the selected testcases to be moved to</li>
			<li>Click on <b>Bekreft</b> button</li>
		</ol>
		
		<p class="help-content">&nbsp;</p>
		<h5 id="teststeps">Test Steps</h5>
		<p style="font-weight:bold">Overview</p>
		<p class="help-content">
			A test step is a detailed instruction on what/how to perform the test.
		</p>
		<p class="help-content">&nbsp;</p>
		<p class="help-content">
			Test Steps can be accessed by going to <a href="?id=2">Testskripter -> Testcaser</a> and then select a test case by clicking on the Test Case ID.
		</p>
		
		<p style="font-weight:bold" id="add-teststep">Add/Insert a New Test Step</p>
		<ol>
			<li>Click on <b>Ny registrering</b> button</li>
			<li>Select category from the <b>Kategory</b> dropdown list</li>
				<ul>
					<li><b>Application</b> -- To launch or restart System X</li>
					<li><b>Mouse</b> -- To execute a mouse click or move a window</li>
					<li><b>Keyboard</b> -- To send keystrokes to the window</li>
					<li><b>Checkpoint</b> -- To insert a validation in the test</li>
				</ul>
			<li>Select test command from the <b>Test kommnado</b> dropdown list</li>
			<li>Enter a parameter or use <b>Test Command Generator</b></li>
			<li>Write a short description in the <b>Beskrivelse</b> field (Optional)</li>
			<li>Click on <b>Lagre</b> button</li>
		</ol>
		
		<p class="help-content">&nbsp;</p>
		<p style="font-weight:bold" id="edit-teststep">Modify a Test Step</p>
		<ol>
			<li>Click on this edit button <i class='mdi mdi-pencil-box-outline text-primary'></i>. (Located at the rightmost of the test step you want to modify)</li>
			<li>Make your changes</li>
			<li>Click on <b>Lagre</b> button</li>
		</ol>
		
		<p class="help-content">&nbsp;</p>
		<p style="font-weight:bold" id="activate-deactivate-teststep">Activate/Deactivate a Test Step</p>
		<ol>
			<li>Select a test step you want to activate/deactivate by ticking its checkbox</li>
			<li>Click on <b>Aktiver/Deaktiver</b> button</li>
			<li>Select <b>Utfør dette steget</b> option if you want the selected test steps to be deactivated/skipped. <b>IKKE utfør dette steget</b> if activated/run</li>
			<li>Click on <b>Bekreft</b> button</li>
		</ol>
		
		<p class="help-content">&nbsp;</p>
		<p style="font-weight:bold" id="delete-teststep">Delete a Test Step</p>
		<ol>
			<li>Select a test step you want to delete by clicking on the edit button <i class='mdi mdi-pencil-box-outline text-primary'></i></li>
			<li>Click on <b>Slett</b> button</li>
			<li>Click on <b>OK</b> button if you want to continue</li>
		</ol>
		<p class="help-content">&nbsp;</p>
		<p class="help-content">Or...</p>
		<ol>
			<li>Select a test step you want to delete by ticking its checkbox</li>
			<li>Click on <b>Slett</b> button</li>
			<li>Click on <b>OK</b> button if you want to continue</li>
		</ol>
		
		<p class="help-content">&nbsp;</p>
		<p style="font-weight:bold" id="copy-teststep">Copy a Test Step</p>
		<ol>
			<li>Select a test step you want to copy by ticking its checkbox</li>
			<li>Click on <b>Kopi</b> button</li>
			<li>Select a location where the selected testcases to be copied to</li>
				<ul>
					<li><b>Kopi før steg nr.</b> -- Copied before the entered step number</li>
					<li><b>Kopi til toppen</b> -- Copied at the top or at the beginning</li>
					<li><b>Kopi til bunnen</b> -- Copied at the bottom or at the end</li>
				</ul>
			<li>Click on <b>Bekreft</b> button</li>
		</ol>
		
		<p class="help-content">&nbsp;</p>
		<p style="font-weight:bold" id="move-teststep">Move a Test Step</p>
		<ol>
			<li>Select a test step you want to move by ticking its checkbox</li>
			<li>Click on <b>Flytt</b> button</li>
			<li>Select a location where the selected testcases to be moved to</li>
				<ul>
					<li><b>Flytt før steg nr.</b> -- Moved before the entered step number</li>
					<li><b>Flytt til toppen</b> -- Moved at the top or at the beginning</li>
					<li><b>Flytt til bunnen</b> -- Moved at the bottom or at the end</li>
				</ul>
			<li>Click on <b>Bekreft</b> button</li>
		</ol>
	</div>
</div>