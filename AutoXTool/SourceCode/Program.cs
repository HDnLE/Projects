using System;
using System.Windows.Forms;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Reporting;
using Ranorex.Core.Testing;
using Ranorex.Plugin;
using System.Text.RegularExpressions;

namespace SystemXTest 
{
    class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            // Uncomment the following 2 lines if you want to automate Windows apps
            // by starting the test executable directly
            //if (Util.IsRestartRequiredForWinAppAccess)
            //    return Util.RestartWithUiAccess();
			
            Keyboard.AbortKey = System.Windows.Forms.Keys.Pause;
            int error = 0;
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
			
			System.Windows.Forms.Application.Run(new Splash.WinSplash());
		 	
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