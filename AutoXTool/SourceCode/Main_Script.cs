using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;
using System.IO;

namespace SystemXTest
{
    [TestModule("01A940E2-4B34-465B-B7FB-B57D602234C6", ModuleType.UserCode, 1)]
    public class Main_Script : ITestModule
    {
        public Main_Script()
        {
            // Do not delete - a parameterless constructor is required!
        }

        void ITestModule.Run()
        {
            Mouse.DefaultMoveTime = 1;
            Keyboard.DefaultKeyPressTime = 5;
            Delay.SpeedFactor = 1.0;
            string programLog = GlobalFunction.GetConfigSettings("EnableErrorLog");
            string dbVersion = "v" + WinComponent.sxDBVersion; //GlobalFunction.GetConfigSettings("DBVersion");
            string rxVersion = Ranorex.Host.Local.RanorexVersion;
            rxVersion = rxVersion.Remove(rxVersion.Length - 2);
            string executionTime = GlobalFunction.ExecutionTime();
			string logFilename = "";
			if (programLog == "true")
			{
				logFilename = @"Logs\System-XAuto-Test - EventLog_" + executionTime.Replace("-", String.Empty).Replace(":", String.Empty).Replace(" ", String.Empty) + ".htm";
				System.IO.File.WriteAllText(logFilename, System.IO.File.ReadAllText("Logs\\System-XAuto-Test - EventLog.htm"));
				System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AppEvent.log", logFilename);
				GlobalFunction.WriteReportHeaderInfo(logFilename, "%Version%", WinComponent.GetAppInfo("Version"));
				GlobalFunction.WriteReportHeaderInfo(logFilename, "%BuildDate%", WinComponent.GetAppInfo("BuildDate"));
				GlobalFunction.WriteReportHeaderInfo(logFilename, "%DBVersion%", dbVersion);
				GlobalFunction.WriteReportHeaderInfo(logFilename, "%RXVersion%", rxVersion);
			}
            
            //bool dbcheck = MySQLConnector.CheckDbConnection();
            /*bool dbcheck = true;
            
            if (dbcheck)
            {
            	if (!GlobalFunction.IsConfigFileOK())
            	{
            		GlobalFunction.ShowMessage("Feil på config-fil. Sjekk opsjoner eller logfil for mer informasjon.", MessageBoxIcon.Warning);
            		if (programLog == "true") {Process.Start(logFilename);}
            		dbcheck = false;
            	}
            }
            
    	    SystemXTest.WinComponent Component = new SystemXTest.WinComponent(dbcheck);
        	Component.InitializeComponent(dbcheck);*/
        	if (!GlobalFunction.IsConfigFileOK())
        	{
        		GlobalFunction.ShowMessage("Feil på config-fil. Sjekk opsjoner eller logfil for mer informasjon.", MessageBoxIcon.Exclamation);
        		if (programLog == "true") 
        		{
        			Process.Start(logFilename);
        		}
        	}
        	else
        	{
        		//bool dbcheck = MySQLConnector.CheckDbConnection();
        		SystemXTest.WinComponent Component = new SystemXTest.WinComponent();
        		Component.InitializeComponent();
        	}
        }
    }
}
