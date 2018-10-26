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
    /// Description of Setup.
    /// </summary>
    [TestModule("5987C616-A8F8-4591-91E8-18E65D96BACE", ModuleType.UserCode, 1)]
    public class Setup : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public Setup()
        {
            // Do not delete - a parameterless constructor is required!
        }

        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        void ITestModule.Run()
        {
            Mouse.DefaultMoveTime = 300;
            Keyboard.DefaultKeyPressTime = 100;
            Delay.SpeedFactor = 1.0;
            
            //Get Windows AppData folder
            string tempLogFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            
            //This log file stores the number of failed scripts or testcases
            //Set initial value to 0
            SXCollections.WriteLogData(tempLogFolder + @"\script_error_count.log", "0");
            
            //This log file stores the number of all active testcases
            //Set initial value to 0
            SXCollections.WriteLogData(tempLogFolder + @"\script_count.log", "0");
            
            //This log file stores the duration of each testcase
            //Set initial value to 0
            SXCollections.WriteLogData(tempLogFolder + @"\script_duration.log", "0");
            
            //This log file stores the filename of the Ranorex compressed report
            //Set initial value to empty
            SXCollections.WriteLogData(tempLogFolder + @"\rxlogfile.log", "");
        }
    }
}
