/*
 * Created by Ranorex
 * User: rommel.lamanilao
 * Date: 08.05.2018
 * Time: 11:00
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;
using System.IO;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;
using System.IO.Compression;

namespace SystemXTest
{
    /// <summary>
    /// Description of TempCode.
    /// </summary>
    [TestModule("89A42EAF-B2A9-48CD-BA48-211DE4EEB3E0", ModuleType.UserCode, 1)]
    public class TempCode : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public TempCode()
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
           // Directory.SetCurrentDirectory(@"C:\Scripts\v4.1\AutomaticTesting");
           // int exitCode = GlobalFunction.RunCommandProcess("git checkout version_4.1 > %APPDATA%\\GitCheckout.log");
           // List<string> untrackedFiles = WinComponent.GetUntrackedFiles();
           // bool x = WinComponent.GetGitStatus();
           string startPath = @"C:\Scripts\v5.2\AutomaticTesting\Source\TS_1058_Opplæring___Konsultasjon_allmennlege";
           string zipPath = @"C:\Scripts\v5.2\AutomaticTesting\Source\TS_1058_Opplæring___Konsultasjon_allmennlege.zip";
           //string extractPath = @"C:\Scripts\v5.2\AutomaticTesting\Source";
           ZipFile.CreateFromDirectory(startPath, zipPath);
		   
        }
        
        
        static float CalculateDirectorySize(DirectoryInfo directory, bool includeSubdirectories) 
        {
	        float totalSize = 0;
	
	        FileInfo[] files = directory.GetFiles();
	        foreach (FileInfo file in files) {
	            totalSize += file.Length;
	        }
	
	        if (includeSubdirectories) {
	            DirectoryInfo[] dirs = directory.GetDirectories();
	            foreach (DirectoryInfo dir in dirs) {
	                totalSize += CalculateDirectorySize(dir, true);
	            }
	        }
	
	        return totalSize;
	    }
        
        public static void Log(string logMessage, TextWriter w)
	    {
	        w.WriteLine(logMessage);
	    }
    }
}
