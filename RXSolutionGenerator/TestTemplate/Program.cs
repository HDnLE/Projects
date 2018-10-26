using System;
using System.Threading;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WinForms = System.Windows.Forms;
using Ranorex.Plugin;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Reporting;
using Ranorex.Core.Testing;

namespace TestTemplate
{
    class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            Keyboard.AbortKey = System.Windows.Forms.Keys.Escape;
            int error = 0;
            
            Mouse.DefaultMoveTime = 300;
            Keyboard.DefaultKeyPressTime = 100;
            Delay.SpeedFactor = 1.0;
            
            //Disable Delphi plugin
            Ranorex.Core.ElementEngine.Instance.Flavors["delphi"].Enabled = false;
			
            //Add process name to GDI capture list
            RawTextFlavor.Instance.ClassNames.Add(new Regex("^TToolBar$"));
			RawTextFlavor.Instance.ClassNames.Add(new Regex("^TDBGrid$"));
			RawTextFlavor.Instance.ClassNames.Add(new Regex("^TPanel$"));
			RawTextFlavor.Instance.ClassNames.Add(new Regex("^TCalendar$"));
			RawTextFlavor.Instance.ClassNames.Add(new Regex("^TSXStringGrid$"));
			RawTextFlavor.Instance.ClassNames.Add(new Regex("^TStringGrid$"));
			RawTextFlavor.Instance.ClassNames.Add(new Regex("^TSXEditor$"));
			RawTextFlavor.Instance.ClassNames.Add(new Regex("^TRichEdit$"));
			RawTextFlavor.Instance.ClassNames.Add(new Regex("^TDBRichEdit$"));
			RawTextFlavor.Instance.ClassNames.Add(new Regex("^TSXVSTreeView$"));
			RawTextFlavor.Instance.ClassNames.Add(new Regex("^TStatusBar$"));
			RawTextFlavor.Instance.ClassNames.Add(new Regex("^TEdit$"));
			RawTextFlavor.Instance.ClassNames.Add(new Regex("^TTreeView$"));
			RawTextFlavor.Instance.ClassNames.Add(new Regex("^TListBox$"));
			RawTextFlavor.Instance.ClassNames.Add(new Regex("^TGroupBox$"));
			RawTextFlavor.Instance.ClassNames.Add(new Regex("^TJvRichEdit$"));
			RawTextFlavor.Instance.ProcessNames.Add(new Regex("^Systemx$"));
			RawTextFlavor.Instance.ProcessNames.Add(new Regex("^TSXToolBar$"));
			RawTextFlavor.Instance.ProcessNames.Add(new Regex("^TSXPanel$"));
			RawTextFlavor.Instance.ProcessNames.Add(new Regex("^TSXRichEdit$"));
            
            try
            {
                error = TestSuiteRunner.Run(typeof(Program), Environment.CommandLine);
            }
            catch (Exception e)
            {
                Report.Error("Unexpected exception occurred: " + e.ToString());
                error = -1;
            }
            return error;
        }
    }
}
