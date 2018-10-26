using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Security.Cryptography;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;
using MySql;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using System.Diagnostics;

namespace SystemXTest
{
	public static class GlobalFunction
	{		
		public static void ShowMessage(string message, MessageBoxIcon icon)
		{
			string utilityName = "Auto X Tool";
			MessageBox.Show(new System.Windows.Forms.Form { TopMost = true }, message, utilityName, MessageBoxButtons.OK, icon);
		}
		
		public static DialogResult ConfirmMessage(string message)
		{
			string utilityName = "System XAuto-Test";
			DialogResult answer = MessageBox.Show(new System.Windows.Forms.Form { TopMost = true }, message, utilityName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			return answer;
		}
		
		public static string ExecutionTime()
		{
			string date = System.DateTime.Now.Day.ToString();
            string month = System.DateTime.Now.Month.ToString();
            string year = System.DateTime.Now.Year.ToString();
            string hour = System.DateTime.Now.Hour.ToString();
            string minute = System.DateTime.Now.Minute.ToString();
            string second = System.DateTime.Now.Second.ToString();
            if (date.Length == 1)
            {
            	date = "0" + date;
            }
            if (month.Length == 1)
            {
            	month = "0" + month;
            }
            if (hour.Length == 1)
            {
            	hour = "0" + hour;
            }
            if (minute.Length == 1)
            {
            	minute = "0" + minute;
            }
            if (second.Length == 1)
            {
            	second = "0" + second;
            }
            string exe_time = year + "-" + month + "-" + date + " " + hour + ":" + minute + ":" + second;
            return exe_time;
		}
		
		public static string OSVersion()
		{
			string subKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
			string productName = HKLM_GetString(subKey, "ProductName");
			string csdVersion = HKLM_GetString(subKey, "CSDVersion");
			string platform = Environment.OSVersion.Platform.ToString();
			string ntVer = "64bit";
			if (platform == "Win32NT")
			{
				ntVer = "32bit";
			}
			string osVersion = productName + " " + csdVersion + " " + ntVer;
			return osVersion;
		}
		
		public static string HKLM_GetString(string path, string key)
	    {
	        try
	        {
	            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(path);
	            if (rk == null) return "";
	            return (string)rk.GetValue(key);
	        }
	        catch { return ""; }
	    }
		
		public static void WriteReportHeaderInfo(string reportFile, string headerInfo, string info)
		{
			string programLog = GlobalFunction.GetConfigSettings("EnableErrorLog");
			if (programLog == "true")
			{
				File.WriteAllText(reportFile, System.IO.File.ReadAllText(reportFile).Replace(headerInfo, info));
			}
		}
		
		public static void WriteReportLog(string filename, string oldString, string newString)
		{
			string programLog = GlobalFunction.GetConfigSettings("EnableErrorLog");
			if (programLog == "true")
			{
				File.WriteAllText(filename, System.IO.File.ReadAllText(filename).Replace(oldString, newString));
			}
		}
		
		public static void WriteStepLog(string filename, string[] stepData)
		{
			string programLog = GlobalFunction.GetConfigSettings("EnableErrorLog");
			if (programLog == "true")
			{
				using (StreamWriter sw = File.AppendText(filename)) 
		        {
					string trHtml = "\t\t\t\t\t<tr>";
					foreach (string data in stepData)
					{
						if ((data == "Feil") || (data == "-X-"))
						{
							trHtml = "\t\t\t\t\t<tr class='bold feil'>";
							break;
						}
					}
					sw.WriteLine(trHtml);
					foreach (string data in stepData)
					{
						string html = "\t\t\t\t\t\t<td>" + data + "</td>";
						if ((data == "Feil") || (data == "OK"))
						{
							html = "\t\t\t\t\t\t<td class='bold " + data.ToLower() + "'>" + data + "</td>";
						}
						sw.WriteLine(html);
					}
					sw.WriteLine("\t\t\t\t\t</tr>");
		        }	
			}
		}
		
		public static string ExtractText(string text)
		{
            int index = text.IndexOf(":");
           return text.Replace(text.Substring(index), String.Empty).Replace("TC-", String.Empty);
		}
		
		public static List<string> GetEntryList(string strEntry)
        {
        	List<string> result = new List<string>();
        	string[] rangeList = new string[] {};
        	if ((strEntry.Contains("-")) && (!strEntry.Contains(",")))
        	{
        		rangeList = strEntry.Split('-');
				int start = Int32.Parse(rangeList[0]);
				int end = Int32.Parse(rangeList[1]);
				for (int x=start; x<=end; x++)
				{
						result.Add(x.ToString());
				}
        	}
        	else if (strEntry.Contains(","))
        	{
        		rangeList = strEntry.Split(',');
        		foreach (string list in rangeList)
        		{
        			if (list.Contains("-"))
        			{
        				rangeList = list.Split('-');
        				if (rangeList[0].Contains(","))
        				{
        					string[] rangeList2 = rangeList[0].Split(',');
        					rangeList[0] = rangeList2[1];
        				}
        				if (rangeList[1].Contains(","))
        				{
        					string[] rangeList2 = rangeList[1].Split(',');
        					rangeList[1] = rangeList2[0];
        				}
						int start = Int32.Parse(rangeList[0]);
						int end = Int32.Parse(rangeList[1]);
						for (int x=start; x<=end; x++)
						{
								result.Add(x.ToString());
						}
        			}
        			else
        			{
        				result.Add(list);
        			}
        		}
        	}
        	else
        	{
        		result.Add(strEntry);
        	}
        	return result;
        }
		
		public static bool ScriptBuildCheckRanorex()
        {
        	string buildLog = @"Logs\ScriptBuild.log";
        	bool buildResponse = false;
        	Delay.Seconds(3);
        	var logLines = System.IO.File.ReadAllLines(buildLog);
            
            foreach (var singleLine in logLines) 
            {
            	if (singleLine.Contains("Build succeeded") || singleLine.Contains("Bygging var vellykket"))
            	{
            		buildResponse = true;
            		File.Delete(buildLog);
            		break;
            	}
			}
            return buildResponse;
        }
		
		public static bool ScriptBuildCheck()
        {
        	Delay.Seconds(5);
        	try
			{
				Host.Local.FindSingle<Ranorex.Form>("/form[@title~'Aut2Exe Error' and @accessiblerole='Dialog']",5000);
				return false;
			}
			catch (ElementNotFoundException)
			{
				return true;
			}
        }
		
		public static void WriteToLog(string filename, string[] stepData)
		{
			string errorLog = GlobalFunction.GetConfigSettings("EnableErrorLog");
			if (errorLog == "true")
			{
				using (StreamWriter sw = File.AppendText(filename)) 
		        {
					string trHtml = "\t\t\t\t\t<tr>";
					foreach (string data in stepData)
					{
						if ((data == "Error") || (data == "-X-"))
						{
							trHtml = "\t\t\t\t\t<tr class='bold error'>";
							break;
						}
					}
					sw.WriteLine(trHtml);
					foreach (string data in stepData)
					{
						string html = "\t\t\t\t\t\t<td>" + data + "</td>";
						if ((data == "Error") || (data == "OK"))
						{
							html = "\t\t\t\t\t\t<td class='bold " + data.ToLower() + "'>" + data + "</td>";
						}
						sw.WriteLine(html);
					}
					sw.WriteLine("\t\t\t\t\t</tr>");
		        }	
			}
		}
		
		public static bool CheckScriptsExist(string testsuite_id)
		{
			string test_suite_title = MySQLConnector.GetTestSuiteInfo(testsuite_id, 2);
			string scriptFolder = (GlobalFunction.GetConfigSettings("ScriptsDir")).Replace("%DBName%", "v" + WinComponent.sxDBVersion); //GlobalFunction.GetConfigSettings("ScriptsDir");
			string script_path = scriptFolder + @"\TS_" + testsuite_id + "_" + test_suite_title.Replace(' ', '_').Replace('-', '_') + "\\";
			List<string> test_cases = MySQLConnector.GetTestCaseList(testsuite_id);
			bool result = true;
			if (test_cases.Count == 0)
			{
				result = false;
			}
			foreach (string test_case in test_cases)
			{
				string test_case_title = "TC_" + test_case + "_" + MySQLConnector.GetTestCaseInfo(testsuite_id, test_case, 3).Replace(' ', '_').Replace('-', '_');
				if (!File.Exists(script_path + test_case_title + "\\" + test_case_title + ".exe"))
				{
					result = false;
					break;
				}
			}
			return result;
		}
		
		public static string GetConfigSettings(string configKey)
        {
        	System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
        	try
        	{
            	string key = (string)settingsReader.GetValue(configKey, typeof(String));
            	return key;
        	}
        	catch
        	{
        		GlobalFunction.ShowMessage("Nøkkelen " + configKey + " finnes ikke i appSettings-konfigurasjonsdelen", MessageBoxIcon.Error);
        		throw new Exception("Nøkkelen " + configKey + " finnes ikke i appSettings-konfigurasjonsdelen");
        	}
            
            
        }
		public static string Encrypt(string toEncrypt, bool useHashing)
		{
		    byte[] keyArray;
		    byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
		
		    System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
		    string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
		    //If hashing use get hashcode regards to your key
		    if (useHashing)
		    {
		        MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
		        keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
		        hashmd5.Clear();
		    }
		    else
		        keyArray = UTF8Encoding.UTF8.GetBytes(key);
		
		    TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
		    tdes.Key = keyArray;
		    tdes.Mode = CipherMode.ECB;
		
		    tdes.Padding = PaddingMode.PKCS7;
		
		    ICryptoTransform cTransform = tdes.CreateEncryptor();
		    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
		    tdes.Clear();
		    return Convert.ToBase64String(resultArray, 0, resultArray.Length);
		}
		
		public static string Decrypt(string cipherString, bool useHashing)
		{
		    byte[] keyArray;
		    //get the byte code of the string
		
		    byte[] toEncryptArray = Convert.FromBase64String(cipherString);
		
		    System.Configuration.AppSettingsReader settingsReader = 
		                                        new System.Configuration.AppSettingsReader();
		    //Get your key from config file to open the lock!
		    string key = (string)settingsReader.GetValue("SecurityKey", 
		                                                 typeof(String));
		            
		    if (useHashing)
		    {
		        //if hashing was used get the hash code with regards to your key
		        MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
		        keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
		        //release any resource held by the MD5CryptoServiceProvider
		
		        hashmd5.Clear();
		    }
		    else
		    {
		        //if hashing was not implemented get the byte code of the key
		        keyArray = UTF8Encoding.UTF8.GetBytes(key);
		    }
		
		    TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
		    //set the secret key for the tripleDES algorithm
		    tdes.Key = keyArray;
		    //mode of operation. there are other 4 modes. 
		    //We choose ECB(Electronic code Book)
		
		    tdes.Mode = CipherMode.ECB;
		    //padding mode(if any extra byte added)
		    tdes.Padding = PaddingMode.PKCS7;
		
		    ICryptoTransform cTransform = tdes.CreateDecryptor();
		    byte[] resultArray = cTransform.TransformFinalBlock(
		                         toEncryptArray, 0, toEncryptArray.Length);
		    //Release resources held by TripleDes Encryptor                
		    tdes.Clear();
		    //return the Clear decrypted TEXT
		    return UTF8Encoding.UTF8.GetString(resultArray);
		}
		
		public static List<string> ExtractTestData(string strData)
        {
        	var pattern = @"\[(.*?)\]";
			var matches = Regex.Matches(strData, pattern);
			List<string> result = new List<string>();
			
			foreach (Match m in matches) 
			{
				result.Add(m.Groups[1].ToString());
			}
			
			return result;
        }
		
		public static string GetActualKeystrokes(string keys)
		{
			string result = keys;
			if (result.Length == 1)
			{
				result = "{" + keys + "key}";
			}
			// Keystroke with a combination of Alt + key
			if (keys.ToUpper().Contains("ALT+"))
			{
				string[] altKey = keys.Split('+');
				result = "{Alt DOWN}{" + altKey[1] + "key}{Alt UP}";
				if (isNumeric(altKey[1]))
				{
					result = "{Alt DOWN}{D" + altKey[1] + "}{Alt UP}";
				}
			}
			
			// Keystroke with a combination of Shift + key
			if (keys.ToUpper().Contains("SHIFT+"))
			{
				string[] altKey = keys.Split('+');
				result = "{Shift DOWN}{" + altKey[1] + "key}{Shift UP}";
				if (altKey[1].Length > 1)
				{
					result = "{Shift DOWN}{" + altKey[1] + "}{Shift UP}";
				}
			}
			
			// Keystroke with a combination of Ctrl + key
			if (keys.ToUpper().Contains("CTRL+"))
			{
				IList<string> altKey = keys.Split('+');
				result = "{ControlKey DOWN}{" + altKey[1] + "key}{ControlKey UP}";
				if (altKey[1].Length > 1 && altKey.Count == 2)
				{
					result = "{ControlKey DOWN}{" + altKey[1] + "}{ControlKey UP}";
				}
				
				if (isNumeric(altKey[1]))
				{
					result = "{ControlKey DOWN}{D" + altKey[1] + "}{ControlKey UP}";
				}
				if (altKey.Count == 3)
				{
					string key2 = "";
					if (altKey[1].Contains("Shift"))
					{
						key2 = "{" + altKey[1] + " DOWN}{" + altKey[2] + "}{" + altKey[1] + " UP}";
					}
					result = "{ControlKey DOWN}" + key2 + "{ControlKey UP}";
				}
			}
			
			return result;
		}
		
		public static bool isNumeric(string text)
		{
			int n;
			return int.TryParse(text, out n);
		}
		
		public static void ConfirmContinue(RxPath objPath)
		{
			DialogResult confirm = ConfirmMessage("Finner ikke objektet. Har du fortsatt ønsker at testen skal fortsette?");
        	if (confirm == DialogResult.No)
        	{
        		Report.Failure("Object Check", "Kan ikke finne objektet");
        		throw new ElementNotFoundException("Kan ikke finne objektet", objPath);
        	}
		}
		
		public static void KillCrashProgram()
		{
			try
			{
				// Kill WerFault process
				Process winError = Process.GetProcessesByName("WerFault")[0];
				winError.Kill();
				Delay.Seconds(2);
			}
			catch (ElementNotFoundException)
			{
				// Do nothing
			}
		}
		
		public static string RestartSystemX(bool autoLogin=true)
		{
			string response = "Det oppsto kritisk feil på den foregående test. Starte System X program";
			try
			{
				// Kill System X process
				Process systemxProcess = Process.GetProcessesByName("Systemx")[0];
				systemxProcess.Kill();
				Delay.Seconds(2);
				KillCrashProgram();
				try
				{
					// Get the System X path and run it
					string appName = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SystemxPath.log");
					Process.Start(appName);
					
					// If System X is not properly terminated, this window will popup
					try
					{
						Ranorex.Form frmProgFail = Host.Local.FindSingle<Ranorex.Form>("/form[@title='Bekreft']", 5000);
						frmProgFail.PressKeys("{ESCAPE}");
					}
					catch (ElementNotFoundException)
					{
						// Do nothing
					}
					
					if (autoLogin)
					{
						try
						{
							// Get login details
							string[] details = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\login.log").Split(';');
							string username = details[0];
							string password = details[1];
							try
							{
								Ranorex.Text txtUsername = Host.Local.FindSingle<Ranorex.Text>("/form[@controlname='FPassord']/text[@controlname='UsrName']", 2000);
								Ranorex.Text txtPassword = Host.Local.FindSingle<Ranorex.Text>("/form[@controlname='FPassord']/text[@controlname='PWord']", 2000);
								txtUsername.Focus();
								txtUsername.PressKeys(username);
								txtPassword.Focus();
								txtPassword.PressKeys(password);
								Ranorex.Form frmCalendar = Host.Local.FindSingle<Ranorex.Form>("/form[@controlname='fdt']", 3000);
								frmCalendar.Activate();
								frmCalendar.PressKeys("{ENTER}");					
								Delay.Seconds(2);
							}
							catch (ElementNotFoundException)
							{
								//Do nothing
							}
						}
						catch (FileNotFoundException)
						{
							response = "Kan ikke logge inn automatisk. Finner ikke loggfilen som inneholder de siste innloggingsdetaljer";
						}
					}
				}
				catch (FileNotFoundException)
				{
					response = "Kan ikke starte System X. Loggfilen som inneholder mangler System X stien";
				}
			}
			catch (IndexOutOfRangeException)
			{
				response = "Kan ikke avslutte System X prosess. Kan ikke finne prosessnavnet";
			}
			return response;
		}
		
		public static void EnsureMainWindowVisible()
		{
			Ranorex.Form frmMainWindow = Host.Local.FindSingle<Ranorex.Form>("/form[@title='SystemX']", 5000);
			frmMainWindow.Activate();
		}
		
		public static bool IsProgramOpen(RxPath programPath)
		{
			try
			{
				Host.Local.FindSingle<Ranorex.Form>(programPath, 1000);
				return true;
			}
			catch (ElementNotFoundException)
			{
				return false;
			}	
		}
		
		public static DialogResult InputBox(string title, string promptText, ref string value, ref int maxStep)
		{
		  System.Windows.Forms.Form form = new System.Windows.Forms.Form();
		  Label label = new Label();
		  TextBox textBox = new TextBox();
		  System.Windows.Forms.Button buttonOk = new System.Windows.Forms.Button();
		  System.Windows.Forms.Button buttonCancel = new System.Windows.Forms.Button();
		  System.Windows.Forms.Button buttonAbort = new System.Windows.Forms.Button();
		
		  form.Text = title;
		  label.Text = promptText;
		  label.BackColor = System.Drawing.Color.Transparent;
		  textBox.Text = value;
		  textBox.BorderStyle = BorderStyle.FixedSingle;
		  textBox.KeyPress+= new System.Windows.Forms.KeyPressEventHandler(WinComponent.txtStep_Keypress);
		
		  buttonOk.Text = "Kjør til trinn";
		  buttonCancel.Text = "Fortsett";
		  buttonAbort.Text = "Avbryt";
		  buttonOk.DialogResult = DialogResult.OK;
		  buttonCancel.DialogResult = DialogResult.Cancel;
		  buttonAbort.DialogResult = DialogResult.Abort;
		
		  label.SetBounds(9, 18, 372, 13);
		  textBox.SetBounds(12, 36, 372, 20);
		  buttonOk.SetBounds(147, 72, 75, 23);
		  buttonCancel.SetBounds(228, 72, 75, 23);
		  buttonAbort.SetBounds(309, 72, 75, 23);
		
		  label.AutoSize = true;
		  textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
		  buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		  buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		  buttonAbort.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		
		  form.ClientSize = new Size(396, 107);
		  form.Controls.AddRange(new System.Windows.Forms.Control[] { label, textBox, buttonOk, buttonCancel, buttonAbort });
		  form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
		  form.FormBorderStyle = FormBorderStyle.FixedDialog;
		  form.StartPosition = FormStartPosition.CenterScreen;
		  form.MinimizeBox = false;
		  form.MaximizeBox = false;
		  form.AcceptButton = buttonOk;
		  form.CancelButton = buttonCancel;
		  form.TopMost = true;
		  
		  if (Int16.Parse(value) == maxStep)
		  {
		  	buttonOk.Enabled = false;
		  	buttonAbort.Enabled = false;
		  	textBox.Text = (maxStep-1).ToString();
		  	textBox.Enabled = false;
		  	label.Text = "Du har nå kommet til det siste trinnet";
		  }
		  
		  DialogResult dialogResult = form.ShowDialog();
		  
		  value = textBox.Text;
		  return dialogResult;
		}
		
		public static string GetShortDay(System.DateTime today)
		{
			var cultureInfo = new System.Globalization.CultureInfo("nb-NO");
            var dateTimeInfo = cultureInfo.DateTimeFormat.GetAbbreviatedDayName(today.DayOfWeek);
            string shortDay = dateTimeInfo.ToString();
            return UppercaseFirst(shortDay.Substring(0,2));
		}
		
		public static string UppercaseFirst(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
			    return string.Empty;
			}
			
			return char.ToUpper(s[0]) + s.Substring(1);
		}
		
		private static System.DateTime GetFirstDayOfWeek(System.DateTime dayInWeek, System.Globalization.CultureInfo cultureInfo)
	    {
	        DayOfWeek firstDay = cultureInfo.DateTimeFormat.FirstDayOfWeek;
	        System.DateTime firstDayInWeek = dayInWeek.Date;
	        while (firstDayInWeek.DayOfWeek != firstDay)
	            firstDayInWeek = firstDayInWeek.AddDays(-1);
	
	        return firstDayInWeek;
	    }
        
        public static System.DateTime GetFirstDayOfWeek(System.DateTime dayInWeek)
	    {
	        System.Globalization.CultureInfo defaultCultureInfo = System.Globalization.CultureInfo.CurrentCulture;
	        return GetFirstDayOfWeek(dayInWeek, defaultCultureInfo);
	    }
        
        public static bool CheckContainerText(Container container, string textToFind)
        {
        	bool response = false;
        	RxPath containerPath = container.GetPath();
            IList<RawText> rawTextList = container.Find<RawText>(containerPath + "/?/?/rawtext[@rawtext='" + textToFind + "']");
            if (rawTextList.Count > 0)
            {
            	response = true;
            }
            return response;
        }
        
        public static string GetTemplateText(string objectPath, string textHandler)
        {
        	string[] stringSeparators = new string[] {textHandler};
			string[] arrString = objectPath.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
			string result = arrString[1];
			var reg = new Regex("\'.*?\'");
		    var matches = reg.Matches(result);
		    result = matches[0].ToString().Replace("'", "");
		    return result;
        }
        
        

		public static string MD5Hash(string input)
		{
		   StringBuilder hash = new StringBuilder();
		   MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
		   byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));
		
		   for (int i = 0; i < bytes.Length; i++)
		   {
		     hash.Append(bytes[i].ToString("x2"));
		   }
		   return hash.ToString();
		}
		
		public static void DbBackupCheck(string errorLog)
        {
        	var logLines = System.IO.File.ReadAllLines(errorLog);
            
            foreach (var singleLine in logLines) 
            {
            	if ((singleLine.Contains("Got error")) || (singleLine.Contains("Systemet finner ikke angitt bane")))
            	{
            		ShowMessage(singleLine, MessageBoxIcon.Error);
            		break;
            	}
			}
        }
		
		public static bool IsConfigFileOK()
		{
			string logFilename = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AppEvent.log");
			string[] logData = new string[] {};
			string mysqldumpPath = GlobalFunction.GetConfigSettings("MySQL") + "\\mysqldump.exe";
			string msBuildPath = GlobalFunction.GetConfigSettings("MSBuild") + "\\MSBuild.exe";
			string aiBuildPath = GlobalFunction.GetConfigSettings("AutoItBuild") + "\\Aut2exe.exe";
			string logMessage = "";
			string status = "Information";
			bool response = true;
			int errCount = 0;
						
			//Checking MySQL
			if (!File.Exists(mysqldumpPath))
			{
				logMessage = "Unable to locate MySQL (" + mysqldumpPath + ")";
				status = "Feil";
				errCount++;
				logData = new string[] {status, logMessage, GlobalFunction.ExecutionTime()};
				GlobalFunction.WriteToLog(logFilename, logData);
			}
			
			
			//Checking MSBuild
			logMessage = "Checking if MSBuild exists";
			status = "Information";
			if (!File.Exists(msBuildPath))
			{
				logMessage = "Unable to locate MSBuild (" + msBuildPath + ")";
				status = "Error";
				errCount++;
				logData = new string[] {status, logMessage, GlobalFunction.ExecutionTime()};
				GlobalFunction.WriteToLog(logFilename, logData);
			}
			
			
			//Checking AutoIt Compiler
			logMessage = "Checking if AutoIt Compiler exists";
			status = "Information";
			if (!File.Exists(aiBuildPath))
			{
				logMessage = "Unable to locate AutoIt Compiler (" + aiBuildPath + ")";
				status = "Error";
				errCount++;
				logData = new string[] {status, logMessage, GlobalFunction.ExecutionTime()};
				GlobalFunction.WriteToLog(logFilename, logData);
			}
			
			if (errCount > 0)
			{
				response = false;
			}
			return response;
		}
		
		public static int RunCommandProcess(string strArg)
        {
        	Process mapDriveProcess = new Process();
        	mapDriveProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            mapDriveProcess.StartInfo.FileName = "cmd.exe";
            mapDriveProcess.StartInfo.Arguments = "/C " + strArg;
            mapDriveProcess.Start();
	        mapDriveProcess.WaitForExit();
	        return mapDriveProcess.ExitCode;
        }
		
		public static void MapLocalDrive(string pathToMap)
		{
            List<string> driveList = new List<string>();
            string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] drives = letters.ToCharArray();
            int runResult = 1;
            int i = 0;
            
            while (runResult == 1)
            {
            	runResult = RunCommandProcess("subst " + drives[i].ToString() + ": " + pathToMap);
            	i++;
            }
            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MappedDrive.log", drives[i-1].ToString());
		}
		
		public static void ScriptStringReplace(string strScript, string strSource, string strReplacement)
		{
			string content = System.IO.File.ReadAllText(strScript).Replace(strSource, strReplacement);;
			System.IO.File.WriteAllText(strScript, content);
		}
		
		public static float GetDirectorySize(DirectoryInfo directory, bool includeSubdirectories) 
        {
	        float totalSize = 0;
	
	        FileInfo[] files = directory.GetFiles();
	        foreach (FileInfo file in files) {
	            totalSize += file.Length;
	        }
	
	        if (includeSubdirectories) {
	            DirectoryInfo[] dirs = directory.GetDirectories();
	            foreach (DirectoryInfo dir in dirs) {
	                totalSize += GetDirectorySize(dir, true);
	            }
	        }
	
	        return totalSize;
	    }
		
		public static string GetLastUsedTSID()
		{
			try
			{
				string logSource = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LastUsedID.log";
				return File.ReadAllText(logSource);
			}
			catch (FileNotFoundException)
			{
				return string.Empty;
			}
		}
	}
}
