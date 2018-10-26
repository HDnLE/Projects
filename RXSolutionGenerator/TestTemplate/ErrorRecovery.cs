using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace SXNamespace
{
    /// <summary>
    /// Description of UserCodeModule1.
    /// </summary>
    [TestModule("0C674259-F76B-45E3-9882-86FCA6C8401A", ModuleType.UserCode, 1)]
    public class ErrorRecovery : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ErrorRecovery()
        {
            // Do not delete - a parameterless constructor is required!
        }

        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        /// 
        void ITestModule.Run()
        {
            Mouse.DefaultMoveTime = 300;
            Keyboard.DefaultKeyPressTime = 100;
            Delay.SpeedFactor = 1.0;
            
            //Get Windows AppData folder
            string tempLogFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            
            //Get the status of testcase
            string tcStatus = Ranorex.Core.Testing.TestSuite.Current.CurrentTestContainer.Status.ToString();
            
            //Get the total number of failed testcases
            int errorCount = Int32.Parse(System.IO.File.ReadAllText(tempLogFolder + @"\script_error_count.log"));
            
            //Get the current total number of testcases
            int scriptCount = Int32.Parse(System.IO.File.ReadAllText(tempLogFolder + @"\script_count.log"));
            scriptCount++;
            
            //If testcase fails
            if (tcStatus == "Failed")
            {
            	Report.Failure("AUT State", "En testfeil har oppstått. System X må startes på nytt");
            	errorCount++;
            	
            	//Restart System X if testcase fails
            	TestTemplate.RestartSystemX.Start();
            }
            else
            {
            	Report.Success("AUT State", "OK");
            }
            
            //Update the testcase error count
            SXCollections.WriteLogData(tempLogFolder + @"\script_error_count.log", errorCount.ToString());
            
            //Update the testcase count
            SXCollections.WriteLogData(tempLogFolder + @"\script_count.log", scriptCount.ToString());
            
            //Store the filename of the Ranorex report in the log file
            SXCollections.WriteLogData(tempLogFolder + @"\rxlogfile.log", Ranorex.Core.Reporting.TestReport.ReportEnvironment.ReportName);
            
            //Get the duration of the current testcase and store it in the log file
            string duration = Ranorex.Core.Reporting.TestReport.CurrentTestContainerActivity.ElapsedTime.ToString();
            SXCollections.WriteLogData(tempLogFolder + @"\script_duration.log", duration);
        }
    }
}
