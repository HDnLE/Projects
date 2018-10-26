using System;
using System.Windows.Forms;
using Ranorex;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace SystemXTest
{
	public static class WriteFunction
	{
		public static void InitializeScriptTemplate(string testSuite, string scriptName)
		{
			//Copy project template directory
			scriptName = scriptName.Replace(" ", "_").Replace("-", "_").Replace(":", "");
			string sourceDir = @"TestTemplate";
			string targetDir = @"WrittenTests\Raw\" + testSuite + @"\" + scriptName;
			//DirectoryCopy(sourceDir, targetDir, true);
			CopyDirectory(sourceDir, targetDir);
			
			//Rename all necessary files in template directory
			string[] dirs = Directory.GetFiles(targetDir, "TestTemplate*.*");
			string[] oldScripts = Directory.GetFiles(targetDir, scriptName + "*.*");
			foreach (string f in oldScripts)
			{
			    File.Delete(f);
			}
            foreach (string dir in dirs) 
            {
            	string newFilename = dir.Replace("TestTemplate", scriptName);
            	System.IO.File.Move(dir, newFilename);
            }
            
            //Modify contents of the file accordingly
            string[] files = Directory.GetFiles(targetDir, "*.*");
            
            foreach (string file in files)
            {
            	string content = System.IO.File.ReadAllText(file).Replace("TestTemplate", scriptName);;
				System.IO.File.WriteAllText(file, content);
            }
		}
		
		public static void InitializeMasterScriptTemplateRanorex(string testSuite)
		{
			//Copy project template directory
			string sourceDir = @"MasterScriptTemplate";
			string targetDir = @"WrittenTests\Raw\" + testSuite + @"\Masterscript\";
			//DirectoryCopy(sourceDir, targetDir, true);
			CopyDirectory(sourceDir, targetDir);
			
			//Rename all necessary files in template directory
			string[] dirs = Directory.GetFiles(targetDir, "MasterScriptTemplate*.*");
			string[] oldScripts = Directory.GetFiles(targetDir, testSuite + "*.*");
			foreach (string f in oldScripts)
			{
			    File.Delete(f);
			}
            foreach (string dir in dirs) 
            {
            	System.IO.File.Move(dir, dir.Replace("MasterScriptTemplate", testSuite));
            }
            
            //Modify contents of the file accordingly
            string[] files = Directory.GetFiles(targetDir, "*.*");
            foreach (string file in files)
            {
            	string content = System.IO.File.ReadAllText(file).Replace("MasterScriptTemplate", testSuite);;
				System.IO.File.WriteAllText(file, content);
            }
		}
		
		public static void InitializeMasterScriptTemplate(string testSuite)
		{
			string sourceDir = @"MasterScriptTemplate";
			string targetDir = @"WrittenTests\Raw\" + testSuite + @"\Masterscript\";
			
			//Copy project template directory
			CopyDirectory(sourceDir, targetDir);
			
			//Rename the name of the script file
			if (System.IO.File.Exists(targetDir + testSuite + ".au3"))
			{
				System.IO.File.Delete(targetDir + testSuite + ".au3");
			}
			System.IO.File.Move(targetDir + "ScriptTemplate.au3", targetDir + testSuite + ".au3");
			
			//Write name of the script
			GlobalFunction.ScriptStringReplace(targetDir + testSuite + ".au3", "%ScriptTemplate%", testSuite);
		}
		
		public static void CopyDirectory(string srcDir, string targetDir)
		{
            String[] Files;

            if(targetDir[targetDir.Length-1]!=Path.DirectorySeparatorChar) 
                targetDir+=Path.DirectorySeparatorChar;
            if (!Directory.Exists(targetDir))
            {
            	Directory.CreateDirectory(targetDir);
            }
            else
            {
            	//Directory.Delete(targetDir, true);
            }
            Files=Directory.GetFileSystemEntries(srcDir);
            foreach(string Element in Files){
                // Sub directories
                if(Directory.Exists(Element)) 
                    CopyDirectory(Element,targetDir+Path.GetFileName(Element));
                // Files in directory
                else 
                    File.Copy(Element,targetDir+Path.GetFileName(Element),true);
                }
		}
		
		public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
	    {
	        // Get the subdirectories for the specified directory.
	        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
	
	        if (!dir.Exists)
	        {
	            throw new DirectoryNotFoundException(
	                "Kilde katalogen finnes ikke eller ble ikke funnet: "
	                + sourceDirName);
	        }
	
	        DirectoryInfo[] dirs = dir.GetDirectories();
	        // If the destination directory doesn't exist, create it.
	        if (!Directory.Exists(destDirName))
	        {
	            Directory.CreateDirectory(destDirName);
	        }
	        else
	        {
	        	Directory.Delete(destDirName, true);
	        	Directory.CreateDirectory(destDirName);
	        }
	
	        // Get the files in the directory and copy them to the new location.
	        FileInfo[] files = dir.GetFiles();
	        foreach (FileInfo file in files)
	        {
	            string temppath = Path.Combine(destDirName, file.Name);
	            file.CopyTo(temppath, true);
	        }
	
	        // If copying subdirectories, copy them and their contents to new location.
	        if (copySubDirs)
	        {
	            foreach (DirectoryInfo subdir in dirs)
	            {
	                string temppath = Path.Combine(destDirName, subdir.Name);
	                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
	            }
	        }
	    }
		
		public static bool DoWriteScript(string testSuiteId, string testCaseId, string logFilename)
		{
			string strQuery = "SELECT * FROM teststeps WHERE TC_ID =" + testCaseId + " AND TS_ID=" + testSuiteId + " ORDER BY STEP_NUMBER ASC";
			int searchCount = MySQLConnector.RunQuery(strQuery).Rows.Count;
			string strId = "0";
			string designer = MySQLConnector.GetTestCaseInfo(testSuiteId, testCaseId, 6);
			designer = MySQLConnector.GetUserInfo(designer, 1) + " " + MySQLConnector.GetUserInfo(designer, 2);
			string testSuiteTitle = MySQLConnector.GetTestSuiteInfo(testSuiteId, 2);
			string testCaseTitle = MySQLConnector.GetTestCaseInfo(testSuiteId, testCaseId, 3);
			string strMasterScriptFolder = "TS_" + testSuiteId + "_" + testSuiteTitle.Replace(' ', '_').Replace("-", "_");
			string strScriptFolder = "TC_" + testCaseId + "_" + testCaseTitle.Replace(" ", "_").Replace("-", "_");
			string[] logData = new string[] {};
			string strWrittenScriptFolder = strMasterScriptFolder  + @"\" + strScriptFolder;
			string strPath = @"WrittenTests\Raw\" + strWrittenScriptFolder;
			string dbServer = GlobalFunction.GetConfigSettings("DBServer");
			
			//Initialize Mouse and Keyboard Speed
			WriteToScript(strPath + @"\Script.cs", "//Initialize Mouse and Keyboard Speed", 3);
			WriteToScript(strPath + @"\Script.cs", "Mouse.DefaultMoveTime = 1;", 3);
			WriteToScript(strPath + @"\Script.cs", "Keyboard.DefaultKeyPressTime = 1;", 3);
			WriteToScript(strPath + @"\Script.cs", "Delay.SpeedFactor = 1.0;", 3);
			WriteToScript(strPath + @"\Script.cs",  " ");
			
			//Write Test Report Header Info
			WriteToScript(strPath + @"\Script.cs", "//Write Test Report Header Info", 3);
			WriteToScript(strPath + @"\Script.cs", "string executionTime = GlobalFunction.ExecutionTime();", 3);
			WriteToScript(strPath + @"\Script.cs", "System.DateTime start = System.DateTime.Now;", 3);
			WriteToScript(strPath + @"\Script.cs", "string reportFilename = @\"Reports\\" + strScriptFolder + "\" + executionTime.Replace(\"-\", String.Empty).Replace(\":\", String.Empty).Replace(\" \", String.Empty) + \".htm\";", 3);
			WriteToScript(strPath + @"\Script.cs", "System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @\"\\logfilename.log\", reportFilename);", 3);
			WriteToScript(strPath + @"\Script.cs", @"System.IO.File.WriteAllText(reportFilename, System.IO.File.ReadAllText(@""Reports\report_template.htm""));", 3);
			WriteToScript(strPath + @"\Script.cs", @"GlobalFunction.WriteReportHeaderInfo(reportFilename, ""%ExecutionTime%"", executionTime);", 3);
			WriteToScript(strPath + @"\Script.cs", @"GlobalFunction.WriteReportHeaderInfo(reportFilename, ""%OperatingSystem%"", GlobalFunction.OSVersion());", 3);
			WriteToScript(strPath + @"\Script.cs", "GlobalFunction.WriteReportHeaderInfo(reportFilename, \"%TSID%\",\"" + testSuiteId + "\");", 3);
			WriteToScript(strPath + @"\Script.cs", "GlobalFunction.WriteReportHeaderInfo(reportFilename, \"%TSTitle%\",\"" + testSuiteTitle + "\");", 3);
			WriteToScript(strPath + @"\Script.cs", @"GlobalFunction.WriteReportHeaderInfo(reportFilename, ""%ComputerName%"", Environment.MachineName);", 3);
			WriteToScript(strPath + @"\Script.cs", "GlobalFunction.WriteReportHeaderInfo(reportFilename, \"%TCID%\",\"" + testCaseId + "\");", 3);
			WriteToScript(strPath + @"\Script.cs", "GlobalFunction.WriteReportHeaderInfo(reportFilename, \"%TCTitle%\",\"" + testCaseTitle + "\");", 3);
			WriteToScript(strPath + @"\Script.cs", "GlobalFunction.WriteReportHeaderInfo(reportFilename, \"%Designer%\",\"" + designer + "\");", 3);
			
			WriteToScript(strPath + @"\Script.cs",  " ");
			WriteToScript(strPath + @"\Script.cs", @"string strResponse = """";", 3);
			WriteToScript(strPath + @"\Script.cs", @"string timeStep = """";", 3);
			WriteToScript(strPath + @"\Script.cs", @"string strComments = """";", 3);
			WriteToScript(strPath + @"\Script.cs", @"string strResult = ""OK"";", 3);
			WriteToScript(strPath + @"\Script.cs", "string[] stepData = new string[] {};", 3);
			WriteToScript(strPath + @"\Script.cs", "TimeSpan stepDuration = new TimeSpan();", 3);
			WriteToScript(strPath + @"\Script.cs", "TimeSpan duration = new TimeSpan();", 3);
			WriteToScript(strPath + @"\Script.cs", @"string strData = """";", 3);
			WriteToScript(strPath + @"\Script.cs", @"int scriptError = 0;", 3);
			WriteToScript(strPath + @"\Script.cs", @"string restartResponse = """";", 3);
			WriteToScript(strPath + @"\Script.cs", @"string strErrResponse = """";", 3);
			WriteToScript(strPath + @"\Script.cs", @"string tempLogFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);", 3);
			WriteToScript(strPath + @"\Script.cs", "string runStatus =\"1\";", 3);
			WriteToScript(strPath + @"\Script.cs",  " ");
			
			//Retrieve all test steps based on Test Suite ID and Test Case ID
			for (int x=1; x<searchCount; x++)
			{
				strId = MySQLConnector.RunQuery(strQuery).Rows[x][0].ToString();
				string strComments = MySQLConnector.GetTestStepInfo(strId, 6);
				string isDisable = MySQLConnector.GetTestStepInfo(strId, 9);
				string stepCategory = MySQLConnector.GetTestStepInfo(strId, 4);
				string cmdId = MySQLConnector.GetTestStepInfo(strId, 5);
				string strCommand = MySQLConnector.GetTestCommandInfo(cmdId, 3);
				string userCodes = MySQLConnector.GetTestStepInfo(strId, 10);
				string restartSX = (userCodes == "False") ? "true" : "false";
				
				WriteToScript(strPath + @"\Script.cs", "//Step " + x + ": " + strComments, 3);
				if (isDisable == "True")
				{
					WriteToScript(strPath + @"\Script.cs", "timeStep = \"&minus;\";", 3);
					WriteToScript(strPath + @"\Script.cs", "strResult = \"&minus;\";", 3);
					WriteToScript(strPath + @"\Script.cs", "strComments = \"Dette trinnet hoppes\";", 3);
					WriteToScript(strPath + @"\Script.cs", "/*", 3);
				}
				WriteStep(strId, strPath);
				WriteToScript(strPath + @"\Script.cs", "stepDuration = System.DateTime.Now.Subtract(start);", 3);
				WriteToScript(strPath + @"\Script.cs", "timeStep = String.Format(@\"{0:hh\\:mm\\:ss\\.ff}\", stepDuration);", 3);
				WriteToScript(strPath + @"\Script.cs", "strComments = \"" + strComments + "\";", 3);
				WriteToScript(strPath + @"\Script.cs", "strResult = \"OK\";", 3);
				WriteToScript(strPath + @"\Script.cs", "strErrResponse = \"\";", 3);
				WriteToScript(strPath + @"\Script.cs",  " ");
				
				if (strCommand == "Program_Crash")
				{
					WriteToScript(strPath + @"\Script.cs", @"if (strResponse.Contains(""Kritisk""))", 3);
					WriteToScript(strPath + @"\Script.cs", "{", 3);
					WriteToScript(strPath + @"\Script.cs", @"strComments = strResponse.Replace(""Kritisk:"", String.Empty);", 4);
					WriteToScript(strPath + @"\Script.cs", @"strErrResponse = "" ("" + strResponse.Replace(""Kritisk:"", String.Empty) + "")"";", 4);
					WriteToScript(strPath + @"\Script.cs", "GlobalFunction.KillProcess(\"WerFault\");", 4);
					WriteToScript(strPath + @"\Script.cs", "GlobalFunction.KillProcess(\"WerFault\");", 4);
					WriteToScript(strPath + @"\Script.cs", "GlobalFunction.KillProcess(\"dw20\");", 4);
					WriteToScript(strPath + @"\Script.cs", "GlobalFunction.KillProcess(\"dw20\");", 4);
				}
				else
				{
					WriteToScript(strPath + @"\Script.cs", @"if (strResponse.Contains(""Feil""))", 3);
					WriteToScript(strPath + @"\Script.cs", "{", 3);
					WriteToScript(strPath + @"\Script.cs", @"strResult = ""Feil"";", 4);
					WriteToScript(strPath + @"\Script.cs", "scriptError++;", 4);
					WriteToScript(strPath + @"\Script.cs", @"strErrResponse = "" -- "" + strResponse.Replace(""Feil:"", String.Empty);", 4);
					WriteToScript(strPath + @"\Script.cs", "}", 3);
					WriteToScript(strPath + @"\Script.cs", @"else if (strResponse.Contains(""Kritisk""))", 3);
					WriteToScript(strPath + @"\Script.cs", "{", 3);
					WriteToScript(strPath + @"\Script.cs", @"strComments = strResponse.Replace(""Kritisk:"", String.Empty);", 4);
					WriteToScript(strPath + @"\Script.cs", @"strErrResponse = "" ("" + strResponse.Replace(""Kritisk:"", String.Empty) + "")"";", 4);
					
					/*if (strCommand == "Run_Windows_App")
					{
						restartSX = "false";
					}*/
				}
				WriteToScript(strPath + @"\Script.cs", @"strResult = ""Feil"";", 4);
				WriteToScript(strPath + @"\Script.cs", "scriptError++;", 4);
				WriteToScript(strPath + @"\Script.cs", "Report.Failure(strComments);", 4);
				WriteToScript(strPath + @"\Script.cs", "stepData = new string[] {timeStep, \"" + x.ToString() + "\", \"" + stepCategory + "\", \"" + strCommand + "\", strResult, strComments};", 4);
				WriteToScript(strPath + @"\Script.cs", "GlobalFunction.WriteStepLog(reportFilename, stepData);", 4);
				WriteToScript(strPath + @"\Script.cs", "restartResponse = GlobalFunction.RestartSystemX(" + restartSX + ");", 4);
				WriteToScript(strPath + @"\Script.cs", "stepData = new string[] {\"--:--:--\", \"--\", \"Application\", \"Restart SystemX\", strResult, restartResponse};", 4);
				WriteToScript(strPath + @"\Script.cs", "GlobalFunction.WriteStepLog(reportFilename, stepData);", 4);
				WriteToScript(strPath + @"\Script.cs", "duration = System.DateTime.Now.Subtract(start);", 4);
				WriteToScript(strPath + @"\Script.cs", @"GlobalFunction.WriteReportHeaderInfo(reportFilename, ""%Duration%"", String.Format(@""{0:hh\:mm\:ss\.ff}"", duration));", 4);
				WriteToScript(strPath + @"\Script.cs", @"GlobalFunction.WriteReportHeaderInfo(reportFilename, ""%TotalError%"", scriptError.ToString());", 4);
				WriteToScript(strPath + @"\Script.cs", @"System.IO.File.WriteAllText(tempLogFolder + @""\script_error.log"", scriptError.ToString());", 4);
				WriteToScript(strPath + @"\Script.cs", "GlobalFunction.RunQuery(\"" + dbServer + "\",\"sxtest_" + WinComponent.sxDBVersion.Replace(".", String.Empty) + "\", \"UPDATE testcases SET RUN_STATUS=0, LAST_RUN_DATE='\" + executionTime + \"', MACHINE_NAME='\" + Environment.MachineName + \"' WHERE TS_ID=" + testSuiteId + " AND TC_ID=" + testCaseId + "\");", 4);
				WriteToScript(strPath + @"\Script.cs", "throw new Exception();", 4);
				WriteToScript(strPath + @"\Script.cs", "}", 3);
				WriteToScript(strPath + @"\Script.cs", "stepData = new string[] {timeStep, \"" + x.ToString() + "\", \"" + stepCategory + "\", \"" + strCommand + "\", strResult, strComments + strErrResponse};", 3);
				WriteToScript(strPath + @"\Script.cs", "GlobalFunction.WriteStepLog(reportFilename, stepData);", 3);
				
				if (isDisable == "True")
				{
					WriteToScript(strPath + @"\Script.cs", "*/", 3);
				}
				WriteToScript(strPath + @"\Script.cs",  " ");
			}
			WriteToScript(strPath + @"\Script.cs",  " ");
			WriteToScript(strPath + @"\Script.cs", @"if (!strResponse.Contains(""Kritisk""))", 3);
			WriteToScript(strPath + @"\Script.cs", "{", 3);
			WriteToScript(strPath + @"\Script.cs", "stepData = new string[] {\"--:--:--\", \"--\", \"\", \"Execution Step\", \"OK\", \"End of Test\"};", 4);
			WriteToScript(strPath + @"\Script.cs", "GlobalFunction.WriteStepLog(reportFilename, stepData);", 4);
			WriteToScript(strPath + @"\Script.cs", "}", 3);
			
			WriteToScript(strPath + @"\Script.cs",  " ");
			WriteToScript(strPath + @"\Script.cs", "duration = System.DateTime.Now.Subtract(start);", 3);
			WriteToScript(strPath + @"\Script.cs", @"GlobalFunction.WriteReportHeaderInfo(reportFilename, ""%Duration%"", String.Format(@""{0:hh\:mm\:ss\.ff}"", duration));", 3);
			
			WriteToScript(strPath + @"\Script.cs", @"GlobalFunction.WriteReportHeaderInfo(reportFilename, ""%TotalError%"", scriptError.ToString());", 3);
			WriteToScript(strPath + @"\Script.cs",  " ");
			WriteToScript(strPath + @"\Script.cs", @"System.IO.File.WriteAllText(tempLogFolder + @""\script_error.log"", scriptError.ToString());", 3);
			WriteToScript(strPath + @"\Script.cs", "runStatus = (scriptError == 0) ? \"1\":\"0\";", 3);
			
			WriteToScript(strPath + @"\Script.cs", "GlobalFunction.RunQuery(\"" + dbServer + "\",\"sxtest_" + WinComponent.sxDBVersion.Replace(".", String.Empty) + "\", \"UPDATE testcases SET RUN_STATUS=\" + runStatus + \", LAST_RUN_DATE='\" + executionTime + \"', MACHINE_NAME='\" + Environment.MachineName + \"' WHERE TS_ID=" + testSuiteId + " AND TC_ID=" + testCaseId + "\");", 3);
			
			//Finalize the code for Script.cs "sxtest_" + sxDBVersion.Replace(".", String.Empty);	
			WriteToScript(strPath + @"\Script.cs", "\t\t}\n\t}\n}");
			
			logData = new string[] {"Information", "Writing Ranorex script for TC-" + testCaseId + "... Done", GlobalFunction.ExecutionTime()};
			GlobalFunction.WriteToLog(logFilename, logData);
			            	
			//Build the project
			//System.Diagnostics.Process.Start("cmd.exe", @"/C %systemroot%\Microsoft.NET\Framework\v4.0.30319\MSBuild WrittenTests\Raw\" + strWrittenScriptFolder + @"\" + strScriptFolder + ".csproj > Logs\\ScriptBuild.log");
			string workingDir = System.Environment.CurrentDirectory;
			GlobalFunction.MapLocalDrive(workingDir + @"\WrittenTests\Raw");
			Delay.Seconds(1);
			string mappedDrive = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MappedDrive.log");
			//int checkBuild = ScriptBuild(@"WrittenTests\Raw\" + strWrittenScriptFolder + @"\" + strScriptFolder + ".csproj");
			int checkBuild = ScriptBuildRanorex(mappedDrive + ":\\" + strWrittenScriptFolder + @"\" + strScriptFolder + ".csproj");
			
			string sourceDir = mappedDrive + ":\\" + strWrittenScriptFolder + @"\bin\Debug";
			string scriptFolder = (GlobalFunction.GetConfigSettings("ScriptsDir")).Replace("%DBName%", "v" + WinComponent.sxDBVersion);
			string targetDir = scriptFolder + @"\" + strWrittenScriptFolder;
			
			string sourceScriptDir =  mappedDrive + ":\\" + strWrittenScriptFolder;
			string scriptSourceFolder = (GlobalFunction.GetConfigSettings("ScriptSourceDir")).Replace("%DBName%", "v" + WinComponent.sxDBVersion);
			string targetSourceScriptDir = scriptSourceFolder + @"\" + strWrittenScriptFolder;
			
			if (checkBuild == 0)
			{
				//GlobalFunction.WriteToLog(logFilename, "Bygge skripter ..... Ferdig");
				logData = new string[] {"Information", "Compiling and building the script to EXE... Done", GlobalFunction.ExecutionTime()};
				GlobalFunction.WriteToLog(logFilename, logData);
				
				//Copy all executable scripts to scriptFolder
				CopyDirectory(sourceDir, targetDir);
				
				//Copy source code of scripts to scriptSourceFolder
				//CopyDirectory(sourceScriptDir, targetSourceScriptDir);
				
				//Disconnect mapped drive after copying the scripts
				GlobalFunction.RunCommandProcess("subst " + mappedDrive + ": /D");
				return true;
			}
			else
			{
				logData = new string[] {"Error", "An error occurred on compiling and building the script", GlobalFunction.ExecutionTime()};
				GlobalFunction.WriteToLog(logFilename, logData);
				GlobalFunction.ShowMessage("Bygge skriptfeil!\n\nSjekk skriptet eller refere til Logs\\ScriptBuild.log", MessageBoxIcon.Error);
				System.Diagnostics.Process.Start(@"Logs\ScriptBuild.log");
				GlobalFunction.RunCommandProcess("subst " + mappedDrive + ": /D");
				return false;
			}
		}
		
		public static bool DoWriteMasterScriptRanorex(string testsuiteID, string logFilename)
		{
			List<string> lstTestCases = MySQLConnector.GetTestCaseList(testsuiteID);
			string strTestSuiteTitle = MySQLConnector.GetTestSuiteInfo(testsuiteID, 2);
			string masterScriptFolder = "TS_" + testsuiteID + "_" + strTestSuiteTitle.Replace(' ', '_').Replace('-', '_');
			string strPath = @"WrittenTests\Raw\" + masterScriptFolder + @"\Masterscript\Script.cs";
			string designer = MySQLConnector.GetTestSuiteInfo(testsuiteID, 4);
			designer = MySQLConnector.GetUserInfo(designer, 1) + " " + MySQLConnector.GetUserInfo(designer, 2);
			
			WriteToScript(strPath, "GlobalFunction.CheckCapsLock();", 3);
			WriteToScript(strPath, "string executionTime = GlobalFunction.ExecutionTime();", 3);
			WriteToScript(strPath, "System.DateTime start_time_per_script = new System.DateTime();", 3);
			WriteToScript(strPath, "System.DateTime start_time_test_suite = System.DateTime.Now;", 3);
			WriteToScript(strPath, "TimeSpan scriptDuration = new TimeSpan();", 3);
			WriteToScript(strPath, "string timeScriptExecute = \"\";", 3);
			//WriteToScript(strPath, "string masterReportFile = @\"Reports\\TestSuiteSummaryReport_\" + executionTime.Replace(\"-\", String.Empty).Replace(\":\", String.Empty).Replace(\" \", String.Empty) + \".htm\";", 3);
			WriteToScript(strPath, "string masterReportFile = @\"Reports\\TestSuite" + testsuiteID + "SummaryReport.htm\";", 3);
			WriteToScript(strPath, "Process process = new Process();", 3);
			WriteToScript(strPath, "int script_pass_count = 0;", 3);
			WriteToScript(strPath, "int total_script_count = " + lstTestCases.Count + ";", 3);
			WriteToScript(strPath,  " ");
			WriteToScript(strPath, @"System.IO.File.WriteAllText(masterReportFile, System.IO.File.ReadAllText(@""Reports\master_report_template.htm""));", 3);
			WriteToScript(strPath, "GlobalFunction.WriteReportHeaderInfo(masterReportFile);", 3);
			WriteToScript(strPath, "File.WriteAllText(masterReportFile, System.IO.File.ReadAllText(masterReportFile).Replace(\"%TSID%\", \"" + testsuiteID + "\"));", 3);
			WriteToScript(strPath, "File.WriteAllText(masterReportFile, System.IO.File.ReadAllText(masterReportFile).Replace(\"%TSTitle%\", \"" + strTestSuiteTitle + "\"));", 3);
			WriteToScript(strPath, "File.WriteAllText(masterReportFile, System.IO.File.ReadAllText(masterReportFile).Replace(\"%Designer%\", \"" + designer + "\"));", 3);
			WriteToScript(strPath,  " ");
			
			foreach (string testcaseId in lstTestCases)
			{
				string tcTitle = MySQLConnector.GetTestCaseInfo(testsuiteID, testcaseId, 3);
				string script = "TC_" + testcaseId + "_" + tcTitle.Replace(' ', '_').Replace('-', '_');
				WriteToScript(strPath, "//Script #" + testcaseId, 3);
				WriteToScript(strPath, "start_time_per_script = System.DateTime.Now;", 3);
				WriteToScript(strPath, "GlobalFunction.RunScript(@\"..\\" + script + "\\" + script + ".exe\");", 3);
				/*WriteToScript(strPath, "process.Start();", 3);
				WriteToScript(strPath, "process.WaitForExit();", 3);*/
				WriteToScript(strPath, "scriptDuration = System.DateTime.Now.Subtract(start_time_per_script);", 3);
				WriteToScript(strPath, "timeScriptExecute = String.Format(@\"{0:hh\\:mm\\:ss\\.ff}\", scriptDuration);", 3);
				WriteToScript(strPath, "script_pass_count = GlobalFunction.LogScriptStatus(masterReportFile, \"TC-" + testcaseId + ": " + tcTitle + "\", timeScriptExecute) + script_pass_count;", 3);
				WriteToScript(strPath,  " ");
			}
			WriteToScript(strPath, "TimeSpan test_duration = System.DateTime.Now.Subtract(start_time_test_suite);", 3);
			WriteToScript(strPath, "string execution_duration = String.Format(@\"{0:hh\\:mm\\:ss\\.ff}\", test_duration);", 3);
			WriteToScript(strPath, "File.WriteAllText(masterReportFile, System.IO.File.ReadAllText(masterReportFile).Replace(\"%Duration%\", execution_duration));", 3);
			WriteToScript(strPath, "File.WriteAllText(masterReportFile, System.IO.File.ReadAllText(masterReportFile).Replace(\"%Pass%\", script_pass_count.ToString()));", 3);
			WriteToScript(strPath, "File.WriteAllText(masterReportFile, System.IO.File.ReadAllText(masterReportFile).Replace(\"%Fail%\", (total_script_count - script_pass_count).ToString()));", 3);
			WriteToScript(strPath,  " ");
			WriteToScript(strPath, "System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @\"\\script_error_count.log\", (total_script_count - script_pass_count).ToString());", 3);
			WriteToScript(strPath, "System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @\"\\script_duration.log\", execution_duration);", 3);
			WriteToScript(strPath,  " ");
			WriteToScript(strPath, "bool folderExists = Directory.Exists(@\"..\\..\\Reports\\\");",3);
			WriteToScript(strPath, "if (!folderExists)",3);
			WriteToScript(strPath, "{",3);
			WriteToScript(strPath, "Directory.CreateDirectory(@\"..\\..\\Reports\\\");",4);
			WriteToScript(strPath, "}",3);
			WriteToScript(strPath, "File.Copy(masterReportFile, @\"..\\..\\Reports\\TestSuite" + testsuiteID + "SummaryReport.htm\", true);",3);
			WriteToScript(strPath,  " ");
			WriteToScript(strPath, "System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @\"\\script_count.log\", \"" + lstTestCases.Count.ToString() + "\");", 3);
			
			//Finalize the code for Script.cs
			WriteToScript(strPath, "\t\t}\n\t}\n}");
			
			//Build the project			
			//System.Diagnostics.Process.Start("cmd.exe", @"/C C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild WrittenTests\Raw\" + masterScriptFolder.Replace("-", "_") + @"\Masterscript\" + masterScriptFolder.Replace("-", "_") + ".csproj > Logs\\ScriptBuild.log");
			ScriptBuildRanorex(@"WrittenTests\Raw\" + masterScriptFolder.Replace("-", "_") + @"\Masterscript\" + masterScriptFolder.Replace("-", "_") + ".csproj");
			Delay.Milliseconds(1500);
			
			if (GlobalFunction.ScriptBuildCheckRanorex())
			{
				//GlobalFunction.WriteToLog(logFilename, "Bygge skripter ..... Ferdig");
				//Copy all executable scripts to scriptFolder
				string sourceDir = @"WrittenTests\Raw\" + masterScriptFolder.Replace("-", "_") + @"\Masterscript\bin\Debug";
				string scriptFolder = (GlobalFunction.GetConfigSettings("ScriptsDir")).Replace("%DBName%", "v" + WinComponent.sxDBVersion); //GlobalFunction.GetConfigSettings("ScriptsDir");
				string targetDir = scriptFolder + @"\" + masterScriptFolder.Replace("-", "_") + @"\Masterscript";
				//DirectoryCopy(sourceDir, targetDir, true);
				CopyDirectory(sourceDir, targetDir);
				//GlobalFunction.WriteToLog(logFilename, "Kopiering master skript ..... Ferdig");
				return true;
			}
			else
			{
				GlobalFunction.ShowMessage("FEIL: Bygge skriptfeil!\n\nSjekk skriptet eller refere til Logs\\ScriptBuild.log", MessageBoxIcon.Error);
				return false;
			}
		}
		
		public static bool DoWriteMasterScript(string testsuiteID, string logFilename)
		{
			List<string> lstTestCases = MySQLConnector.GetTestCaseList(testsuiteID);
			string strTestSuiteTitle = MySQLConnector.GetTestSuiteInfo(testsuiteID, 2);
			string masterScriptFolder = "TS_" + testsuiteID + "_" + strTestSuiteTitle.Replace(' ', '_').Replace('-', '_');
			string strPath = @"WrittenTests\Raw\" + masterScriptFolder + @"\Masterscript\" + masterScriptFolder + ".au3";
			string designer = MySQLConnector.GetTestSuiteInfo(testsuiteID, 4);
			designer = MySQLConnector.GetUserInfo(designer, 1) + " " + MySQLConnector.GetUserInfo(designer, 2);
			string dbServer = GlobalFunction.GetConfigSettings("DBServer");
			string dbName = "sxtest_" + WinComponent.sxDBVersion.Replace(".", String.Empty);
			// Write Testsuite ID
			GlobalFunction.ScriptStringReplace(strPath, "%TestID%", testsuiteID);
			GlobalFunction.ScriptStringReplace(strPath, "%TestsuiteID%", testsuiteID);
			
			// Write total no. of testcases in this testsuite
			GlobalFunction.ScriptStringReplace(strPath, "%ScriptCount%", lstTestCases.Count.ToString());
			
			// Write testsuite title
			GlobalFunction.ScriptStringReplace(strPath, "%TestsuiteTitle%", strTestSuiteTitle);
			
			// Write the name of script's designer
			GlobalFunction.ScriptStringReplace(strPath, "%ScriptDesigner%", designer);
			
			// Write the version of System X
			GlobalFunction.ScriptStringReplace(strPath, "%SysXVersion%", WinComponent.sxDBVersion);
			
			WriteToScript(strPath,  "; ------------------------ Start of Testcases ------------------------");
			foreach (string testcaseId in lstTestCases)
			{
				string tcTitle = MySQLConnector.GetTestCaseInfo(testsuiteID, testcaseId, 3);
				string script = "TC_" + testcaseId + "_" + tcTitle.Replace(' ', '_').Replace('-', '_');
				WriteToScript(strPath, ";Run TC-" + testcaseId + ": " + tcTitle);
				WriteToScript(strPath, "$scriptCount = _RunScript(\"TS-" + testsuiteID + "\", \"" + script + "\", \"TC-" + testcaseId + ": " + tcTitle + "\") + $scriptCount");
				WriteToScript(strPath,  " ");
			}
			WriteToScript(strPath,  "; ------------------------- End of Testcases -------------------------");
			
			WriteToScript(strPath,  " ");
			
			WriteToScript(strPath, "Local $tDiff = TimerDiff($sTimer)");
			WriteToScript(strPath, "_TicksToTime($tDiff, $iHours, $iMins, $iSecs)");
			WriteToScript(strPath, "Local $duration = StringFormat(\"%02d:%02d:%02d\", $iHours, $iMins, $iSecs)");
			
			WriteToScript(strPath,  " ");
			
			WriteToScript(strPath, ";Write test duration in the header");
			WriteToScript(strPath,  "_UpdateHeaderInfo(\"%Duration%\", $duration)");
			
			WriteToScript(strPath,  " ");
			
			WriteToScript(strPath, ";Write passed testcases");
			WriteToScript(strPath, "_UpdateHeaderInfo(\"%Pass%\", $scriptCount)");
			
			WriteToScript(strPath,  " ");
			
			WriteToScript(strPath, ";Write failed testcases");
			WriteToScript(strPath, "_UpdateHeaderInfo(\"%Fail%\", $totalScriptCount - $scriptCount)");
			
			WriteToScript(strPath,  " ");
			
			WriteToScript(strPath, ";Write no. of failed testcases in the temp log");
			WriteToScript(strPath, "_WriteToFile(@AppDataDir & \"\\script_error_count.log\", $totalScriptCount - $scriptCount)");
			
			WriteToScript(strPath,  " ");
			
			WriteToScript(strPath, ";Write test duration in the temp log");
			WriteToScript(strPath, "_WriteToFile(@AppDataDir & \"\\script_duration.log\", $duration)");
			
			WriteToScript(strPath,  " ");
			
			WriteToScript(strPath, "_WriteEndTag()");
			
			WriteToScript(strPath,  " ");
			
			WriteToScript(strPath, "Local $folderExists = FileExists(\"..\\Reports\")");
			WriteToScript(strPath, "If Not $folderExists Then");
			WriteToScript(strPath, "DirCreate(\"..\\Reports\")", 1);
			WriteToScript(strPath, "EndIf");
			
			WriteToScript(strPath,  " ");
			
			WriteToScript(strPath,  "FileCopy($masterReportFile, \"..\\Reports\\TestSuite" + testsuiteID + "SummaryReport.htm\", 1)");
			
			WriteToScript(strPath,  " ");
			
			WriteToScript(strPath,  ";Write total no. of scripts run in this testsuite");
			WriteToScript(strPath,  "_WriteToFile(@AppDataDir & \"\\script_count.log\", $totalScriptCount)");
			
			WriteToScript(strPath,  " ");
			
			WriteToScript(strPath,  ";Connect to MySQL");
			WriteToScript(strPath,  "$sql = _MySQLConnect('ta_admin','dhocc648','" + dbName + "','" + dbServer + "')");
			
			WriteToScript(strPath,  " ");
			
			WriteToScript(strPath,  ";Run date");
			WriteToScript(strPath,  "$sDate = @YEAR & \"-\" & @MON & \"-\" & @MDAY & \" \" & _NowTime()");
			
			WriteToScript(strPath,  " ");
			
			WriteToScript(strPath,  ";Run status: 1 if pass; 0 if fail");
			WriteToScript(strPath,  "$runStatus = (($totalScriptCount - $scriptCount) = 0) ? 1:0");
			
			WriteToScript(strPath,  " ");
			
			WriteToScript(strPath,  ";Update testsuite");
			WriteToScript(strPath,  "$sQuery = \"UPDATE testsuites SET LAST_RUN_DATE='\" & $sDate & \"', RUN_STATUS=\" & $runStatus & \", MACHINE_NAME='\" & @ComputerName & \"' WHERE TS_ID=" + testsuiteID + "\"");
			
			WriteToScript(strPath,  " ");
			
			WriteToScript(strPath,  ";Execute the query");
			WriteToScript(strPath,  "_Query($sql, $sQuery)");
			
			WriteToScript(strPath,  " ");
			
			WriteToScript(strPath,  ";Close the MySQL connection");
			WriteToScript(strPath,  "_MySQLEnd($sql)");
			
			string scriptFolder = (GlobalFunction.GetConfigSettings("ScriptsDir")).Replace("%DBName%", "v" + WinComponent.sxDBVersion);
			string targetDir = scriptFolder + @"\" + masterScriptFolder.Replace("-", "_") + @"\Masterscript";
			Directory.CreateDirectory(targetDir);
			
			string scriptSourceFolder = (GlobalFunction.GetConfigSettings("ScriptSourceDir")).Replace("%DBName%", "v" + WinComponent.sxDBVersion);
			//string targetscriptSourceDir = scriptSourceFolder + @"\" + masterScriptFolder.Replace("-", "_") + @"\Masterscript";
			//Directory.CreateDirectory(targetscriptSourceDir);
			
			ScriptBuild(strPath, targetDir + @"\" + masterScriptFolder.Replace("-", "_") + ".exe");
			
			if (GlobalFunction.ScriptBuildCheck())
			{
				//File.Copy(strPath, targetscriptSourceDir + @"\" + masterScriptFolder.Replace("-", "_") + ".au3", true);
				//File.Copy(@"MasterScriptTemplate\MySQL.au3", targetscriptSourceDir + @"\MySQL.au3", true);
				CopyDirectory(@"MasterScriptTemplate\Reports", targetDir + @"\Reports");
				//CopyDirectory(@"MasterScriptTemplate\Reports", targetscriptSourceDir + @"\Reports");
				return true;
			}
			else
			{
				GlobalFunction.ShowMessage("FEIL: Bygge skriptfeil!", MessageBoxIcon.Error);
				return false;
			}
		}
		
		public static void WriteStep(string stepId, string scriptPath)
		{
			string stepCategory = MySQLConnector.GetTestStepInfo(stepId, 4);
			string commandId = MySQLConnector.GetTestStepInfo(stepId, 5);
			string commandType = MySQLConnector.GetTestCommandInfo(commandId, 3);
			
			switch (stepCategory)
			{
					case "Application":
						WriteRunApp(stepId, commandType, scriptPath);
						break;
					case "Keyboard":
						WriteKeyIn(stepId, scriptPath);
						break;
					case "Mouse":
						if (commandType == "Click_On_Object")
						{
							WriteObjectClick(stepId, scriptPath);
						}
						else if (commandType == "Drag_Drop_Object")
						{
							WriteObjectDragDrop(stepId, scriptPath);
						}
						else if (commandType == "Select_On_Tree")
						{
							WriteSelectItem(stepId, scriptPath);
						}
						else if (commandType == "Select_On_Cell")
						{
							WriteCellSelect(stepId, scriptPath);
						}
						else if (commandType == "Double_Click_On_Object")
						{
							WriteObjectDoubleClick(stepId, scriptPath);
						}
						else if (commandType == "Double_Click_On_Login")
						{
							WriteDoubleClickLogin(stepId, scriptPath);
						}
						else
						{
							WriteWindowMove(stepId, scriptPath);
						}
						break;
					case "Checkpoint":
						WriteCheckpoint(stepId, scriptPath);
						break;
					case "User":
						WriteUserAction(stepId, scriptPath);
						break;
			}
		}
		
		public static void WriteRunApp(string id, string cmdType, string scriptPath)
		{
			// Ranorex script file
			string strScriptFile = scriptPath + @"\Script.cs";
			
			// Name of the application to open
			string applicationList = MySQLConnector.GetTestStepInfo(id, 7);
			string stepNumber = MySQLConnector.GetTestStepInfo(id, 3);
			List<string> arrApplicationList = GlobalFunction.ExtractTestData(applicationList);
			string writeMainProgram = "";
			//string writeSubProgram = "";
			string [] commands = cmdType.Split('_');
			string commandType = commands[0];
			
			// Main Program ID
			string mainProgramID = arrApplicationList[0];
			
			// Open main SystemX application
			if (mainProgramID == "9999999")
			{
				if (commandType == "Restart")
				{
					writeMainProgram = "strResponse = GlobalFunction.RestartApplication();";
					//WriteToScript(strScriptFile, writeMainProgram, 3);
				}
				else
				{
					string applicationName = arrApplicationList[1];
					writeMainProgram = "strResponse = GlobalFunction.RunWinApplication(@\"" + applicationName + "\");";
					
					WriteToScript(strScriptFile, "File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @\"\\SystemxPath.log\", @\"" + applicationName + "\");", 3);
				}
				
			}
			else
			{
				string [] arrParam = mainProgramID.Split('-');
				writeMainProgram = "strResponse = GlobalFunction.ApplicationDelay(" + arrParam[1] + ");";
			}
			WriteToScript(strScriptFile, writeMainProgram, 3);
			/*
			// Open System X programs	
			else
			{
					string action = commands[1];
					string objectRole = "";
					string executeCommand = "";
					string targetWindowTitle = MySQLConnector.GetProgramInfo(mainProgramID, 5);
					string srcWindowTitle = "Startmeny";
					string keyMain = "";
					string roleMain = "";
					string clickMain = "";
					string programType = "main";
					
					if (commandType == "Key")
					{
						executeCommand = GlobalFunction.GetActualKeystrokes(MySQLConnector.GetProgramInfo(mainProgramID, 2));
						if (action == "Close")
						{
							executeCommand = "";
						}
					}
					else if (commandType == "Click")
					{
						objectRole = MySQLConnector.GetProgramInfo(mainProgramID, 4);
						executeCommand = MySQLConnector.GetProgramInfo(mainProgramID, 3);
						if (action == "Close")
						{
							executeCommand = "";
							objectRole = "";
							srcWindowTitle = "";
						}
					}
					
					writeMainProgram = "strResponse = GlobalFunction.ExecuteProgram(\"" + programType + "\", \"" + action + "\", \"" + commandType + "\", \"" + executeCommand + "\", \"" + objectRole + "\", \"" + srcWindowTitle + "\", \"" + targetWindowTitle + "\", \"" + keyMain + "\", \"" + roleMain + "\", \"" + clickMain + "\", reportFilename, \"" + stepNumber + "\");";
					
					if (arrApplicationList.Count == 2)
					{
						string subProgramID = arrApplicationList[1];
						programType = "sub";
						targetWindowTitle = MySQLConnector.GetSubProgramInfo(subProgramID, 6);
						srcWindowTitle = MySQLConnector.GetProgramInfo(mainProgramID, 5);
						if (commandType == "Key")
						{
							executeCommand = GlobalFunction.GetActualKeystrokes(MySQLConnector.GetSubProgramInfo(subProgramID, 3));
							keyMain = GlobalFunction.GetActualKeystrokes(MySQLConnector.GetProgramInfo(mainProgramID, 2));
						}
						else
						{
							executeCommand = MySQLConnector.GetSubProgramInfo(subProgramID, 4);
							objectRole = MySQLConnector.GetSubProgramInfo(subProgramID, 5);
							clickMain = MySQLConnector.GetProgramInfo(mainProgramID, 3);
							roleMain = MySQLConnector.GetProgramInfo(mainProgramID, 4);
						}
						if (action == "Close")
						{
							writeMainProgram = "";
							executeCommand = "";
						}
						writeSubProgram = "strResponse = GlobalFunction.ExecuteProgram(\"" + programType + "\", \"" + action + "\", \"" + commandType + "\", \"" + executeCommand + "\", \"" + objectRole + "\", \"" + srcWindowTitle + "\", \"" + targetWindowTitle + "\", \"" + keyMain + "\", \"" + roleMain + "\", \"" + clickMain + "\", reportFilename, \"" + stepNumber + "\");";
					
				
				
						// Write script to Script.cs file
					
						WriteToScript(strScriptFile, writeSubProgram, 3);
					}
				}*/
			
			}
		
		public static void WriteToScript(string strPath, string strScript, int intTab = 0)
		{
			File.AppendAllText(strPath, Environment.NewLine);
			for (int i=0; i<intTab; i++)
			{
				File.AppendAllText(strPath, "\t");
			}
			File.AppendAllText(strPath, strScript);
		}
		
		public static void WriteKeyIn(string id, string scriptPath)
		{
			string txtKey = MySQLConnector.GetTestStepInfo(id, 7);
			string cmdId = MySQLConnector.GetTestStepInfo(id, 5);
			string command = MySQLConnector.GetTestCommandInfo(cmdId, 3);
			string stepNumber = MySQLConnector.GetTestStepInfo(id, 3);
            string strScriptFile = scriptPath + @"\Script.cs";
            string[] arrObjects = txtKey.Split(';');
            string login = "false";
             string objectPath = "";
            /*if (command == "Key_User_Login")
			{
				WriteToScript(strScriptFile, "File.Create(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @\"\\login.log\").Close();", 3);
			}*/
            
            foreach (string strObject in arrObjects)
			{
            	List<string> arrTestData = GlobalFunction.ExtractTestData(strObject);
            	try
            	{
            		objectPath = MySQLConnector.GetObjectInfo(arrTestData[0], arrTestData[1], 5);
            	}
            	catch
            	{
            		GlobalFunction.ShowMessage("Fant feil på Trinn #" + stepNumber + " -- Databasefeil\n\nProgrammet skal startes på nytt...", MessageBoxIcon.Error);
            		Report.Failure("Database", "Fant feil på Trinn #" + stepNumber + " -- Databasefeil");
            		Process.Start(Application.StartupPath + @"\SystemXAuto-Test.exe");
        			Process.GetCurrentProcess().Kill();
            	}
            	string objectType = MySQLConnector.GetObjectInfo(arrTestData[0], arrTestData[1], 4);
            	txtKey = GlobalFunction.GetActualKeystrokes(arrTestData[2]);
            	/*string programName = MySQLConnector.GetObjectInfo(arrTestData[1], 1);
            	string windowTitle = MySQLConnector.QueryProgramInfoByName(programName, 5);
            	string key = GlobalFunction.GetActualKeystrokes(MySQLConnector.QueryProgramInfoByName(programName, 2));
            	*/
            	string key = "";
            	string windowTitle = "";
            	if (command == "Key_User_Login")
				{
	            	WriteToScript(strScriptFile, "File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @\"\\login.log\", \"" + txtKey + ";" + "\");", 3);
	            	login = "true";
				}
            	WriteToScript(strScriptFile, "strResponse = GlobalFunction.KeyIn(\"" + objectType + "\", \"" + objectPath + "\", @\"" + txtKey + "\", \"" + windowTitle + "\", \"" + key + "\", " + login + ");", 3);
            }
		}
		
		public static void WriteWindowMove(string id, string scriptPath)
		{
			string strScriptFile = scriptPath + @"\Script.cs";
            string testData = MySQLConnector.GetTestStepInfo(id, 7);
            List<string> arrTestData = GlobalFunction.ExtractTestData(testData);
            string objectPath = MySQLConnector.GetObjectInfo(arrTestData[0], arrTestData[1], 5);
            string[] location = arrTestData[2].Split(',');
			int x = Int16.Parse(location[0]);
			int y = Int16.Parse(location[1]);
            
            WriteToScript(strScriptFile, "strResponse = GlobalFunction.WindowMove(\"" + objectPath + "\",  " + x + ", " + y + ");", 3);
		}
		
		public static void WriteObjectDragDrop(string id, string scriptPath)
		{
			string strScriptFile = scriptPath + @"\Script.cs";
			string testData = MySQLConnector.GetTestStepInfo(id, 7);
			List<string> arrTestData = GlobalFunction.ExtractTestData(testData);
			string objectPath = MySQLConnector.GetObjectInfo(arrTestData[0], arrTestData[1], 5);
			string[] location = arrTestData[2].Split(',');
			string from = location[0] + "," + location[1];
			string to = location[2] + "," + location[3];
			
			 WriteToScript(strScriptFile, "strResponse = GlobalFunction.ObjectDragDrop(\"" + objectPath + "\",  \"" + from + "\", \"" + to + "\");", 3);
		}
		
		public static void WriteDoubleClickLogin(string id, string scriptPath)
		{
            string strScriptFile = scriptPath + @"\Script.cs";
            string testData = MySQLConnector.GetTestStepInfo(id, 7);
            string stepNumber = MySQLConnector.GetTestStepInfo(id, 3);
            string[] arrObjects = testData.Split(';');
            string objectPath = "";
            foreach (string strObject in arrObjects)
			{
            	List<string> arrTestData = GlobalFunction.ExtractTestData(strObject);
            	
            	try
            	{
            		objectPath = MySQLConnector.GetObjectInfo(arrTestData[0], arrTestData[1], 5);
            	}
            	catch
            	{
            		GlobalFunction.ShowMessage("Fant feil på Trinn #" + stepNumber + " -- Databasefeil\n\nProgrammet skal startes på nytt...", MessageBoxIcon.Error);
            		Report.Failure("Database", "Fant feil på Trinn #" + stepNumber + " -- Databasefeil");
            		Process.Start(Application.StartupPath + @"\SystemXAuto-Test.exe");
        			Process.GetCurrentProcess().Kill();
            	}
            	string objectType = MySQLConnector.GetObjectInfo(arrTestData[0], arrTestData[1], 4);
            	string direction = arrTestData[2];
            	string key = "";
            	string windowTitle = "";
            	string doctorName = direction.Replace("user-", String.Empty);
            	
            	/*if ((objectType == "Cell") || (arrTestData.Count > 2))
            	{
            		direction = arrTestData[2];
            	}*/
            	
            	WriteToScript(strScriptFile, "File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @\"\\login_doctor.log\", \"" + doctorName + ";" + "\");", 3);
            	WriteToScript(strScriptFile, "strResponse = GlobalFunction.DoubleClickLogin(\"" + objectType + "\", \"" + objectPath + "\",  \"" + windowTitle + "\", \"" + key + "\", \"" + direction + "\");", 3);
            }
		}
		
		public static void WriteObjectDoubleClick(string id, string scriptPath)
		{
            string strScriptFile = scriptPath + @"\Script.cs";
            string testData = MySQLConnector.GetTestStepInfo(id, 7);
            string stepNumber = MySQLConnector.GetTestStepInfo(id, 3);
            string[] arrObjects = testData.Split(';');
            string objectPath = "";
            foreach (string strObject in arrObjects)
			{
            	List<string> arrTestData = GlobalFunction.ExtractTestData(strObject);
            	
            	try
            	{
            		objectPath = MySQLConnector.GetObjectInfo(arrTestData[0], arrTestData[1], 5);
            	}
            	catch
            	{
            		GlobalFunction.ShowMessage("Fant feil på Trinn #" + stepNumber + " -- Databasefeil\n\nProgrammet skal startes på nytt...", MessageBoxIcon.Error);
            		Report.Failure("Database", "Fant feil på Trinn #" + stepNumber + " -- Databasefeil");
            		Process.Start(Application.StartupPath + @"\SystemXAuto-Test.exe");
        			Process.GetCurrentProcess().Kill();
            	}
            	string objectType = MySQLConnector.GetObjectInfo(arrTestData[0], arrTestData[1], 4);
            	string direction = "";
            	string key = "";
            	string windowTitle = "";
            	if ((objectType == "Cell") || (arrTestData.Count > 2))
            	{
            		direction = arrTestData[2];
            	}
            	
            	WriteToScript(strScriptFile, "strResponse = GlobalFunction.ObjectDoubleClick(\"" + objectType + "\", \"" + objectPath + "\",  \"" + windowTitle + "\", \"" + key + "\", \"" + direction + "\");", 3);
            }
		}
		
		public static void WriteObjectClick(string id, string scriptPath)
		{
            string strScriptFile = scriptPath + @"\Script.cs";
            string testData = MySQLConnector.GetTestStepInfo(id, 7);
            string stepNumber = MySQLConnector.GetTestStepInfo(id, 3);
            string[] arrObjects = testData.Split(';');
            string objectPath = "";
            foreach (string strObject in arrObjects)
			{
            	List<string> arrTestData = GlobalFunction.ExtractTestData(strObject);
            	
            	try
            	{
            		objectPath = MySQLConnector.GetObjectInfo(arrTestData[0], arrTestData[1], 5);
            	}
            	catch
            	{
            		GlobalFunction.ShowMessage("Fant feil på Trinn #" + stepNumber + " -- Databasefeil\n\nProgrammet skal startes på nytt...", MessageBoxIcon.Error);
            		Report.Failure("Database", "Fant feil på Trinn #" + stepNumber + " -- Databasefeil");
            		Process.Start(Application.StartupPath + @"\SystemXAuto-Test.exe");
        			Process.GetCurrentProcess().Kill();
            	}
            	string objectType = MySQLConnector.GetObjectInfo(arrTestData[0], arrTestData[1], 4);
            	/*string programName = MySQLConnector.GetObjectInfo(arrTestData[1], 1);
            	string windowTitle = MySQLConnector.QueryProgramInfoByName(programName, 5);
            	string key = GlobalFunction.GetActualKeystrokes(MySQLConnector.QueryProgramInfoByName(programName, 2));*/
            	string direction = "";
            	string key = "";
            	string windowTitle = "";
            	//if ((objectType == "Cell") || (objectType == "RawText" && arrTestData.Count > 2) || (objectType == "RawImage" && arrTestData.Count > 2) || (objectType == "Container" && arrTestData.Count > 2) || (objectType == "TreeItem" && arrTestData.Count > 2) || (objectType == "Button" && arrTestData.Count > 2))
            	if ((objectType == "Cell") || (arrTestData.Count > 2))
            	{
            		direction = arrTestData[2];
            	}
            	
            	WriteToScript(strScriptFile, "strResponse = GlobalFunction.ObjectClick(\"" + objectType + "\", \"" + objectPath + "\",  \"" + windowTitle + "\", \"" + key + "\", \"" + direction + "\");", 3);
            }
		}
		
		public static void WriteSelectItem(string id, string scriptPath)
		{
            string strScriptFile = scriptPath + @"\Script.cs";
            string testData = MySQLConnector.GetTestStepInfo(id, 7);
            string stepNumber = MySQLConnector.GetTestStepInfo(id, 3);
            string[] arrObjects = testData.Split(';');
            string objectPath = "";
            foreach (string strObject in arrObjects)
			{
            	List<string> arrTestData = GlobalFunction.ExtractTestData(strObject);
            	
            	try
            	{
            		objectPath = MySQLConnector.GetObjectInfo(arrTestData[0], arrTestData[1], 5);
            	}
            	catch
            	{
            		GlobalFunction.ShowMessage("Fant feil på Trinn #" + stepNumber + " -- Databasefeil\n\nProgrammet skal startes på nytt...", MessageBoxIcon.Error);
            		Report.Failure("Database", "Fant feil på Trinn #" + stepNumber + " -- Databasefeil");
            		Process.Start(Application.StartupPath + @"\SystemXAuto-Test.exe");
        			Process.GetCurrentProcess().Kill();
            	}
            	string direction = "";
            	if (arrTestData.Count > 2)
            	{
            		direction = arrTestData[2];
            	}
            	
            	WriteToScript(strScriptFile, "strResponse = GlobalFunction.ItemSelect(\"" + objectPath + "\", \"" + direction + "\");", 3);
            }
		}
		
		public static void WriteCellSelect(string id, string scriptPath)
		{
            string strScriptFile = scriptPath + @"\Script.cs";
            string testData = MySQLConnector.GetTestStepInfo(id, 7);
            string stepNumber = MySQLConnector.GetTestStepInfo(id, 3);
            string[] arrObjects = testData.Split(';');
            string objectPath = "";
            foreach (string strObject in arrObjects)
			{
            	List<string> arrTestData = GlobalFunction.ExtractTestData(strObject);
            	
            	try
            	{
            		objectPath = MySQLConnector.GetObjectInfo(arrTestData[0], arrTestData[1], 5);
            	}
            	catch
            	{
            		GlobalFunction.ShowMessage("Fant feil på Trinn #" + stepNumber + " -- Databasefeil\n\nProgrammet skal startes på nytt...", MessageBoxIcon.Error);
            		Report.Failure("Database", "Fant feil på Trinn #" + stepNumber + " -- Databasefeil");
            		Process.Start(Application.StartupPath + @"\SystemXAuto-Test.exe");
        			Process.GetCurrentProcess().Kill();
            	}
            	string direction = "";
            	if (arrTestData.Count > 2)
            	{
            		direction = arrTestData[2];
            	}
            	
            	WriteToScript(strScriptFile, "strResponse = GlobalFunction.CellSelect(\"" + objectPath + "\", \"" + direction + "\");", 3);
            }
		}
		
		public static void WriteCheckpoint(string id, string scriptPath)
		{
			string cpId = MySQLConnector.GetTestStepInfo(id, 3);
			string stepNumber = MySQLConnector.GetTestStepInfo(id, 3);
			string cmdId = MySQLConnector.GetTestStepInfo(id, 5);
			string checkArea = MySQLConnector.GetTestCommandInfo(cmdId, 3);
			List<string> strObjects = GlobalFunction.ExtractTestData(MySQLConnector.GetTestStepInfo(id, 7));
			string objectPath = MySQLConnector.GetObjectInfo(strObjects[0], strObjects[1], 5);
			string objectType = MySQLConnector.GetObjectInfo(strObjects[0], strObjects[1], 4);
            string strScriptFile = scriptPath + @"\Script.cs";
            //string caption = MySQLConnector.GetTestStepInfo(id, 8);
            string caption = "";
            WriteToScript(strScriptFile, "Delay.Seconds(2);", 3);
            /*if (checkArea == "Caption")
            {
            	WriteToScript(strScriptFile, "strData = @\"" + caption.Replace("\"", "\"\"") + "\";", 3);
            	WriteToScript(strScriptFile, "strResponse = GlobalFunction.CheckWindowCaption(\"" + objectPath + "\", strData);", 3);
            }
            else if (checkArea == "TextColor")
            {
            	string text = strObjects[2].Replace("datarow-", "");
            	WriteToScript(strScriptFile, "strData = @\"" + caption + "\";", 3);
            	WriteToScript(strScriptFile, "strResponse = GlobalFunction.CheckTextColor(\"" + objectPath + "\", \"" + text + "\", strData);", 3);
            }*/
            if (checkArea == "Object_Text")
            {
            	caption = strObjects[2];
            	WriteToScript(strScriptFile, "strData = \"" + caption.Replace("\"", "\\\"").Replace(Environment.NewLine, "\" + Environment.NewLine + \"") + "\";", 3);
            	WriteToScript(strScriptFile, "strResponse = GlobalFunction.CheckObjectText(\"" + objectPath + "\", \"" + objectType + "\", strData);", 3);
            }
            /*else if (checkArea == "Object_Size")
            {
            	WriteToScript(strScriptFile, "strData = @\"" + caption + "\";", 3);
            	WriteToScript(strScriptFile, "strResponse = GlobalFunction.CheckWindowSize(\"" + objectPath + "\", strData);", 3);
            }*/
            else if (checkArea == "Click_Exist")
            {
            	WriteToScript(strScriptFile, "strData = @\"" + caption + "\";", 3);
            	WriteToScript(strScriptFile, "strResponse = GlobalFunction.ClickExist(\"" + objectPath + "\", \"" + objectType + "\", strData);", 3);
            }
            else if (checkArea == "Press_Keys_Exist")
            {
            	WriteToScript(strScriptFile, "strData = @\"" + strObjects[2] + "\";", 3);
            	WriteToScript(strScriptFile, "strResponse = GlobalFunction.PressKeysExist(\"" + objectPath + "\", \"" + objectType + "\", strData);", 3);
            }
            /*else if (checkArea == "Record_Exist")
            {
            	string visibile = "false";
            	string dataRecord = strObjects[2];
				if (caption == "true")
				{
					visibile = "true";
				}
				WriteToScript(strScriptFile, "strData = @\"" + visibile + "\";", 3);
            	WriteToScript(strScriptFile, "strResponse = GlobalFunction.CheckRecordExist(\"" + objectType + "\", \"" + objectPath + "\"," + visibile + ", \"" + dataRecord + "\");", 3);
            }
            else if (checkArea == "Object_Location")
            {
            	WriteToScript(strScriptFile, "strData = @\"" + caption + "\";", 3);
            	WriteToScript(strScriptFile, "strResponse = GlobalFunction.CheckObjectLocation(\"" + objectType + "\", \"" + objectPath + "\", strData);", 3);
            }*/
            else if (checkArea == "Program_Crash")
            {
            	WriteToScript(strScriptFile, "strResponse = GlobalFunction.CheckProgramCrash(\"" + objectPath + "\");", 3);
            }
            else
            {
            	string visibile = "false";
            	string optionalText = "";
            	caption = strObjects[2];
				visibile = caption;
				if (strObjects.Count == 4)
				{
					optionalText = strObjects[2].Replace("\"", "\\\"");
					visibile = strObjects[3];
				}
				WriteToScript(strScriptFile, "strData = @\"" + visibile + "\";", 3);
            	WriteToScript(strScriptFile, "strResponse = GlobalFunction.CheckObjectExist(\"" + objectType + "\", \"" + objectPath + "\"," + visibile + ", \"" + optionalText + "\");", 3);
            }
		}
		
		public static void WriteUserAction(string id, string scriptPath)
		{
			string infoMsg = MySQLConnector.GetTestStepInfo(id, 7);
			string strScriptFile = scriptPath + @"\Script.cs";
			
			WriteToScript(strScriptFile, "GlobalFunction.ShowMessage(\"" + infoMsg + "\", MessageBoxIcon.Information);", 3);
		}
		
		public static int ScriptBuildRanorex(string scriptPath)
		{
			string msBuildPath = GlobalFunction.GetConfigSettings("MSBuild");
			//string msBuild = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe";
			string msBuild = msBuildPath + "\\MSBuild.exe";
			string scriptLog = "Logs\\ScriptBuild.log";
			int scriptBuildResult = 1;
			
			scriptBuildResult = GlobalFunction.RunCommandProcess(msBuild + " " +  scriptPath + " > " + scriptLog);
			
	        return scriptBuildResult;
		}
		
		public static void ScriptBuild(string inputFile, string outputFile)
		{
			string aiBuildPath = GlobalFunction.GetConfigSettings("AutoItBuild");
			string aiBuild = aiBuildPath + "\\Aut2exe.exe";
			
			GlobalFunction.RunCommandProcess("\"" + aiBuild + "\"" + @" /IN " +  inputFile + @" /OUT " + outputFile + " /CONSOLE /ICON hovemedical.ico");
			//GlobalFunction.RunCommandProcess(@"""C:\Program Files (x86)\AutoIt3\Aut2Exe\Aut2exe.exe"" /IN WrittenTests\Raw\TS_1002_Ukeoppsett_for_angitt_avtalebok\Masterscript\TS_1002_Ukeoppsett_for_angitt_avtalebok.au3 /OUT C:\Scripts\v5.2\AutomaticTesting\Scripts\TS_1002_Ukeoppsett_for_angitt_avtalebok\Masterscript\TS_1002_Ukeoppsett_for_angitt_avtalebok.exe /CONSOLE /ICON hovemedical.ico");
		}
		
	}
}