using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;
using Ranorex.Core.Repository;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;
using System.IO;

namespace SXNamespace
{
    /// <summary>
    /// Ranorex user code collection. A collection is used to publish user code methods to the user code library.
    /// </summary>
    [UserCodeCollection]
    public class SXCollections
    {
        /// <summary>
        /// Purpose: To press keys if a certain object exist
        /// </summary>
        [UserCodeMethod]
        public static void PressKeyExist(RepoItemInfo objInfo, string strKeys)
        {
        	if (objInfo.Exists(3000))
        	{
        		objInfo.FindAdapter<Unknown>().PressKeys(strKeys);
        	}
        }
        
        
        /// <summary>
        /// Purpose: To write data/info to a specified logfile
        /// </summary>
        [UserCodeMethod]
        public static void WriteLogData(string logFilename, string logData)
        {
        	System.IO.File.WriteAllText(logFilename, logData);
        }
        
        
        /// <summary>
        /// Purpose: To login to System X
        /// </summary>
        [UserCodeMethod]
        public static void SystemXLogin(string strUsername, string strPassword)
        {
        	// A Bekreft dialog pops up if System X is not properly closed. Press Avbryt button if this dialog pops up
        	PressKeyExist(TestTemplate.SXRepos.Instance.frmConfirm.SelfInfo, "{AKey}");
        	
        	try
        	{
        		TestTemplate.SXRepos.Instance.frmPassord.txtUsername.PressKeys(strUsername);				// Enter username
        		TestTemplate.SXRepos.Instance.frmPassord.txtPassword.PressKeys(strPassword + "{ENTER}");	// Enter password
        		File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\login_details.log", strUsername + "|" + strPassword);
        		Report.Info("Login", "Login to System X");
        		Validate.Exists(TestTemplate.SXRepos.Instance.frmStartmeny.BasePath.ToString(), 10000);	// Check if Startmeny opens after login
        	}
        	catch (ElementNotFoundException)
        	{
        		Report.Failure("Login", "Element not found");	// If Password dialog does not exist, report as failure
        	}
        }
		
		/// <summary>
        /// Purpose: To login to System X automatically based on the last login
        /// </summary>
        [UserCodeMethod]
        public static void SXAutoLogin()
        {
        	// A Bekreft dialog pops up if System X is not properly closed. Press Avbryt button if this dialog pops up
        	PressKeyExist(TestTemplate.SXRepos.Instance.frmConfirm.SelfInfo, "{AKey}");
        	
        	try
        	{
        		string loginDetails = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\login_details.log");
        		var arrDetails = loginDetails.Split('|');
        		string strUsername = arrDetails[0];
        		string strPassword = arrDetails[1];
        		TestTemplate.SXRepos.Instance.frmPassord.txtUsername.PressKeys(strUsername);				// Enter username
        		TestTemplate.SXRepos.Instance.frmPassord.txtPassword.PressKeys(strPassword + "{ENTER}");	// Enter password
        		Report.Info("Login", "Login to System X");
        		Validate.Exists(TestTemplate.SXRepos.Instance.frmStartmeny.BasePath.ToString(), 10000);	// Check if Startmeny opens after login
        	}
        	catch (ElementNotFoundException)
        	{
        		Report.Failure("Login", "Element not found");	// If Password dialog does not exist, report as failure
        	}
        }
    }
}
