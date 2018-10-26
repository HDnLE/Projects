using System;
using System.IO;
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
using System.Data;
using System.Security.AccessControl;
using IWshRuntimeLibrary;
using MySql;
using MySql.Data.MySqlClient;

namespace SXAutoLibs
{
	public static class GlobalFunction
	{		
		public static string errorMsg = "Feil: Vennligst se vedlagte rapport for mer informasjon om denne feilen.";
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
		
		public static void WriteReportHeaderInfo(string reportFile, string headerInfo, string info)
		{
			System.IO.File.WriteAllText(reportFile, System.IO.File.ReadAllText(reportFile).Replace(headerInfo, info));
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
		
		public static string RunWinApplication(string appName)
		{
			string response = "";
			string appPath = Path.GetDirectoryName(appName);
			string shortcutName = Path.GetFileNameWithoutExtension(appName);
			CreateShortcut(shortcutName, appPath, appName);
			Delay.Seconds(2);
			appName = appPath + @"\" + shortcutName + ".lnk";
			
			Host.Local.RunApplication(appName);
			//Delay.Seconds(3);
			
			string logFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string logName = logFolder + "\\RunWinApp.log";
			string testcase = System.IO.File.ReadAllText(logFolder + "\\logfilename.log");
			try
			{
				Host.Local.FindSingle<Ranorex.Form>("/form[@title~'Startmeny' or @title='Bekreft']", 15000);
				
				//System.IO.File.WriteAllText(logName, testcase + " -- System X opened successfully...");
				WriteLog(logName, testcase + " -- System X opened successfully...");
				try
				{
					Ranorex.Form frmProgFail = Host.Local.FindSingle<Ranorex.Form>("/form[@title='Bekreft']", 5000);
					frmProgFail.Activate();
					frmProgFail.PressKeys("{AKey}");
				}
				catch (ElementNotFoundException)
				{
					// Do nothing
				}
				response = "OK";
			}
			catch (ElementNotFoundException)
			{
				Report.Failure("Application", errorMsg);
				response = "Kritisk:SystemX-programmet ikke klart å åpne";
				WriteLog(logName, testcase + " -- Failed to open System X...");
			}
			
			return response;
		}
		
		public static bool IsCaching()
		{
			bool response = false;
			try
			{
				Ranorex.Adapter adapterText = Host.Local.FindSingle<Unknown>("/form[@processname='Systemx' and (@accessiblerole='Dialog')]/?/?/text[@text~'unassigned code' or @text~'Error writing data to the connection' or @text~'Programmet kan ikke startes']", 10000);
				//Ranorex.Adapter adapterForm = Host.Local.FindSingle<Unknown>("/form[@processname='Systemx' and (@title='System X' or @title~'Program' or @title~'Programfeil')]", 1000);
				response = true;
				//adapterForm.PressKeys("{ENTER}");
				KillProcess("Systemx");
				KillProcess("Systemx");
				Delay.Seconds(5);
			}
			catch (ElementNotFoundException)
			{
				// Do nothing
			}
			return response;
		}
		
		public static string ApplicationDelay(double delayInSeconds)
		{
			string response = "OK";
			try
			{
				Delay.Seconds(delayInSeconds);
			}
			catch (Exception)
			{
				response = "Feil: Ugyldig parameter";
			}
			return response;
		}
		
		public static void CreateShortcut(string shortcutName, string shortcutPath, string targetFileLocation)
		{
		    string shortcutLocation = System.IO.Path.Combine(shortcutPath, shortcutName + ".lnk");
		    FileInfo fileInfo = new FileInfo(targetFileLocation);
		    string directoryFullPath = fileInfo.DirectoryName;
		    string path = Directory.GetCurrentDirectory();
		    WshShell shell = new WshShell();
		    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);
		 
		    //shortcut.IconLocation = path + @"\systemx.ico";
		    shortcut.TargetPath = targetFileLocation;
		    shortcut.WorkingDirectory = directoryFullPath;
		    shortcut.Save();
		}
		
		public static string ExecuteProgram(string programType, string action, string cmdType, string executeCommand, string objectRole, string srcWindowTitle, string targetWindowTitle, string keyMain, string roleMain, string commandMain, string logFile, string stepNumber)
		{
			string response = "OK";
			EnsureMainWindowVisible();
			Keyboard.DefaultKeyPressTime = 200;
			//string mainWindowTitle = "Startmeny";
			if (cmdType == "Key")
			{
				try
				{
					if (action == "Open")
					{
						if (programType == "sub")
						{
							if (!IsProgramOpen("/form[@title~'" + srcWindowTitle + "' and @processname='Systemx']"))
							{
								Ranorex.Form frmTmp = Host.Local.FindSingle<Ranorex.Form>("/form[@title~'Startmeny' and @processname='Systemx']", 5000);
								frmTmp.Activate();
								frmTmp.PressKeys(keyMain);
							}
						}
						if (!IsProgramOpen("/form[@title~'" + targetWindowTitle + "' and @processname='Systemx']"))
						{
							Ranorex.Form frmMainProgram = Host.Local.FindSingle<Ranorex.Form>("/form[@title~'" + srcWindowTitle + "' and @processname='Systemx']", 5000);
							frmMainProgram.Activate();
							frmMainProgram.PressKeys(executeCommand);
							response = "OK";
						}
					}
					else
					{
						Ranorex.Form frmMainProgram = Host.Local.FindSingle<Ranorex.Form>("/form[@title~'" + targetWindowTitle + "' and @processname='Systemx']", 5000);
						frmMainProgram.Activate();
						frmMainProgram.PressKeys("{escape}");
						response = "OK";
					}
					
				}
				catch (ElementNotFoundException)
				{
					Report.Failure("Application", errorMsg);
					response = "Kritisk:Klarte ikke å finne den objekt formen (" + srcWindowTitle + ")";
				}
			}
			else
			{
				if (action == "Open")
				{
					if (programType == "main")
					{
						if (!IsProgramOpen("/form[@title~'" + targetWindowTitle + "' and @processname='Systemx']"))
						{
							response = ClickOpenProgram("Startmeny", objectRole, executeCommand, logFile, stepNumber);
						}
					}
					else
					{
						if (!IsProgramOpen("/form[@title~'" + srcWindowTitle + "' and @processname='Systemx']"))
						{
							response = ClickOpenProgram("Startmeny", roleMain, commandMain, logFile, stepNumber);
						}
						
						if (!IsProgramOpen("/form[@title~'" + targetWindowTitle + "' and @processname='Systemx']"))
						{
							response = ClickOpenProgram(srcWindowTitle, objectRole, executeCommand, logFile, stepNumber);
						}
					}
					
				}
				
				else
				{
					try
					{
						Ranorex.Button btnExit = Host.Local.FindSingle<Ranorex.Button>("/form[@title~'" + targetWindowTitle + "']/toolbar[@processname='Systemx']/?/?/button[@accessiblerole='PushButton']", 1000);
						btnExit.Click();
						response = "OK";
					}
					catch (ElementNotFoundException)
					{
						/* Report.Failure("Application", errorMsg);
						response = "Kritisk:Klarte ikke å finne den EXIT-knappen"; */
						response = "OK";
					}
				}
			}
			Delay.Seconds(2);
			return response;
		}
		
		public static string ClickOpenProgram(string formTitle, string objectRole, string accessKey, string logFile, string stepNumber)
		{
			string response = "";
			EnsureMainWindowVisible();
			// If object role is a Button
			if (objectRole == "Button")
			{
				try
				{
					Ranorex.Button btnMainProgram = Host.Local.FindSingle<Ranorex.Button>("/form[@title~'Startmeny']/toolbar[@instance='0']/?/?/button[@accessiblename='" + accessKey + "']", 3000);
					btnMainProgram.Click();
					response = "OK";
				}
				catch (ElementNotFoundException)
				{
					Report.Failure("Application", errorMsg);
					response =  "Kritisk:Klarte ikke å finne den knappen ([@accessiblename='" + accessKey + "'])";
				}
			}
			
			// If object role is a toolbar, click on specific location
			else
			{
				string textColor = "#FF0000";
				if (accessKey.Contains("&"))
				{
					textColor = "#000000";
				}
				try
				{
					/*if (objectRole == "Toolbar 2") { instance = "1"; }
					Ranorex.ToolBar tbMainProgram = Host.Local.FindSingle<Ranorex.ToolBar>("/form[@title~'" + formTitle + "']/toolbar[@instance='" + instance + "']", 3000);*/
					Ranorex.RawText rtMainProgram = Host.Local.FindSingle<Ranorex.RawText>("/form[@title~'" + formTitle + "']/?/?/rawtext[@rawtext='" + accessKey + "' and @row!='0' and @textcolor='" + textColor + "']", 3000);
					rtMainProgram.Click();
					response = "OK";
				}
				catch (ElementNotFoundException)
				{
					Report.Failure("Application", errorMsg);
					response =  "Kritisk:Klarte ikke å finne den verktøylinjen";
				}
			}
			Delay.Seconds(2);
			return response;
		}
		
		public static void WriteStepLog(string filename, string[] stepData)
		{
			using (StreamWriter sw = System.IO.File.AppendText(filename)) 
	        {
				string trHtml = "\t\t\t\t\t<tr>";
				foreach (string data in stepData)
				{
					if (data == "Feil")
					{
						trHtml = "\t\t\t\t\t<tr class='bold feil'>";
						break;
					}
					if (data == "&minus;")
					{
						trHtml = "\t\t\t\t\t<tr style='color:#BBB'>";
						break;
					}
				}
				sw.WriteLine(trHtml);
				foreach (string data in stepData)
				{
					string html = "\t\t\t\t\t\t<td>" + data + "</td>";
					if ((data == "Feil") || (data == "OK"))
					{
						html = "\t\t\t\t\t\t<td style='white-space:break-word' class='bold " + data.ToLower() + "'>" + data + "</td>";
					}
					sw.WriteLine(html);
				}
				sw.WriteLine("\t\t\t\t\t</tr>");
	        }	
		}
		
		public static void WriteLog(string filename, string logData)
		{
			using (StreamWriter sw = System.IO.File.AppendText(filename)) 
	        {
				sw.WriteLine(logData);
	        }	
		}
		
		public static string KeyIn(string objType, string objPath, string strTxt, string windowTitle, string key, bool login)
		{
			string response = "OK";
			EnsureMainWindowVisible();
			Keyboard.DefaultKeyPressTime = 200;
			if (login)
			{
				try
				{
					Host.Local.FindSingle<Ranorex.Form>("/form[@title~'Passord']",3000);
				}
				catch (ElementNotFoundException)
				{
					RestartSystemX(false);
				}
			}
			else
			{
				/*try
				{
					Host.Local.FindSingle<Ranorex.Form>("/form[@title~'"+ windowTitle + "']",3000);
				}
				catch (ElementNotFoundException)
				{
					if (key != "{-key}")
					{
						Ranorex.Form frmMain = Host.Local.FindSingle<Ranorex.Form>("/form[@title~'Startmeny']",3000);
						frmMain.Activate();
						frmMain.PressKeys(key);
					}
				}*/
			}
            try
            {
				if (objType == "Text")
				{
					/*Ranorex.Text txtObj = Host.Local.FindSingle<Ranorex.Text>(objPath, 2000);
					txtObj.Focus();*/
					
					System.DateTime today = System.DateTime.Today;
					System.DateTime newDate = new System.DateTime();
					
					if (strTxt.Contains("{backward}"))
					{
						newDate = today.AddDays(-1);
						strTxt = strTxt.Replace("{backward}",newDate.Day.ToString());
					}
					else if (strTxt.Contains("{forward}"))
					{
						newDate = today.AddDays(+1);
						strTxt = strTxt.Replace("{forward}",newDate.Day.ToString());
					}
					else if (strTxt.Contains("{today}"))
					{
						newDate = today;
						strTxt = strTxt.Replace("{today}",newDate.Day.ToString());
					}
					
					//txtObj.PressKeys(strTxt);
				}
				/*else if (objType == "Form")
            	{
            		Ranorex.Form frmObj = Host.Local.FindSingle<Ranorex.Form>(objPath, 2000);
            		frmObj.Focus();
					Delay.Seconds(1);
            		frmObj.PressKeys(strTxt);
            	}	
				else if (objType == "Container")
				{
					Ranorex.Container conObj = Host.Local.FindSingle<Ranorex.Container>(objPath, 2000);
					if (!conObj.Visible)
					{
						response = "Kritisk:Ingen element funnet på AUT ELLER AUT kjører ikke";
					}
					else
					{
						conObj.Focus();
						Delay.Seconds(1);
						conObj.PressKeys(strTxt);
					}
				}*/
				
				Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objPath, 15000);
				adapterObject.PressKeys(strTxt);
				
				
				Report.Success("Keyboard","Sender tekst ('" + strTxt + "') fra tastaturen... OK");
            	
            }
            catch (ElementNotFoundException)
            {
				Report.Failure("Keyboard", errorMsg);
            	response = "Kritisk:Ingen element funnet på AUT ELLER AUT kjører ikke";
            }
			/*if (IsProgramCrash())
			{
				Report.Failure("Mouse", errorMsg);
				return "Kritisk:System X har krasjet!";
			}
			else { return response; }*/
			return response;
		}
		
		public static string WindowMove(string objPath, int x, int y)
		{
			string response = "OK";
			try
			{
				Ranorex.Form frmWindow = Host.Local.FindSingle<Ranorex.Form>(objPath,2000);
				frmWindow.Move(x,y);
				
			}
			catch (ElementNotFoundException)
			{
				Report.Failure("Mouse", errorMsg);
				response = "Kritisk:Forventet objekt (" + objPath + ") mangler. Kan ikke utføre denne testen. System X vil nå starte på nytt.";
			}
			return response;
		}
		
		public static string ObjectClick(string objType, string objPath, string windowTitle, string key, string direction="")
		{
			Mouse.DefaultMoveTime = 150;
			Mouse.DefaultClickTime = 200;
			EnsureMainWindowVisible();
			string templateText = "";
			Location xy = "2,2";
			bool userXY = false;
			/*try
			{
				Host.Local.FindSingle<Ranorex.Form>("/form[@title~'"+ windowTitle + "']",3000);
			}
			catch (ElementNotFoundException)
			{
				Ranorex.Form frmMain = Host.Local.FindSingle<Ranorex.Form>("/form[@title~'Startmeny']",3000);
				frmMain.Activate();
				frmMain.PressKeys(key);
			}*/
			
            try
            {
				if (objType == "RawText")
				{
					if (direction != "")
					{
						System.DateTime today = System.DateTime.Today;
						System.DateTime newDate = new System.DateTime();
						if (direction == "backward")
						{
							newDate = today.AddDays(-1);
						}
						else if (direction == "forward")
						{
							newDate = today.AddDays(+1);
						}
						else
						{
							newDate = today;
						}
						
						if (direction.Contains("day-"))
						{
							objPath = objPath.Replace("Man", direction.Replace("day-", ""));
						}
						else if (direction.Contains("button-"))
						{
							string rawText = direction.Replace("button-","");
							if (direction.Contains("ALT+"))
							{
								rawText = direction.Replace("button-ALT+","&");
							}
							else if (direction.Contains("CTRL+"))
							{
								rawText = direction.Replace("button-CTRL+","");
							}
							templateText = GetTemplateText(objPath, "@rawtext=");
							objPath = objPath.Replace(templateText, rawText);
						}
						else if (direction.Contains("text-"))
						{
							string rawText = direction.Replace("text-","");
							try
							{
								templateText = GetTemplateText(objPath, "@rawtext=");
							}
							catch
							{
								templateText = GetTemplateText(objPath, "@rawtext~");
							}
							objPath = objPath.Replace(templateText, rawText);
						}
						else if (direction.Contains("contains-"))
						{
							string rawText = direction.Replace("contains-","");
							templateText = GetTemplateText(objPath, "@rawtext~");
							objPath = objPath.Replace(templateText, rawText);
						}
						else
						{
							string date = newDate.Day.ToString();
							if (direction == "fullday")
							{
								string month = today.Month.ToString();
								string year = today.Year.ToString();
								if (date.Length == 1)
								{
									date = "0" + date;
								}
								if (month.Length == 1)
								{
									month = "0" + month;
								}
								date = date + "." + month + "." + year;
							}
							try
							{
								templateText = GetTemplateText(objPath, "@rawtext=");
							}
							catch
							{
								templateText = GetTemplateText(objPath, "@rawtext~");
							}
							objPath = objPath.Replace(templateText, date);
						}
					}
				}
				else if (objType == "RawImage")
				{
					if (direction!= "")
					{
						if (direction.Contains("imageId-"))
						{
							string imageId = direction.Replace("imageId-","");
							templateText = GetTemplateText(objPath, "@id=");
							objPath = objPath.Replace(templateText, imageId);
						}
						else if (direction.Contains("indexId-"))
						{
							string indexId = direction.Replace("indexId-","");
							objPath = objPath + "[" + indexId + "]";
						}
					}
				}
				else if (objType == "RadioButton")
				{
					if (direction!= "")
					{
						if (direction.Contains("rbbutton-"))
						{
							string txtButton = direction.Replace("rbbutton-","");
							templateText = GetTemplateText(objPath, "@text=");
							objPath = objPath.Replace(templateText, txtButton);
						}
					}
				}
				else if (objType == "CheckBox")
				{
					if (direction!= "")
					{
						if (direction.Contains("chkbox-"))
						{
							string txtName = direction.Replace("chkbox-","");
							templateText = GetTemplateText(objPath, "@accessiblename=");
							objPath = objPath.Replace(templateText, txtName);
						}
					}
				}
				else if (objType == "Cell")
				{
					System.DateTime today = System.DateTime.Today;
					System.DateTime newDate = new System.DateTime();
					if (direction == "backward")
					{
						newDate = today.AddDays(-1);
					}
					else if (direction == "forward")
					{
						newDate = today.AddDays(+1);
					}
					else
					{
						newDate = today;
					}
					string date = newDate.Day.ToString();
					Ranorex.Container container = "/form[@controlname='fpdt']/element[@controlname='mc']/container[@accessiblerole='Client']";
					templateText = GetTemplateText(objPath, "@accessiblename=");
					IList<Cell> cellList = container.Find<Ranorex.Cell>(objPath.Replace(templateText,  "‎" + date));
					string index = "1";
					if (cellList.Count == 2)
					{
						int dateToday = Int16.Parse(date);
						index = "2";
						if (dateToday <= 12)
						{
							index = "1";
						}
					}
					templateText = GetTemplateText(objPath, "@accessiblename=");
					objPath = objPath.Replace(templateText,  "‎" + date)+ "[" + index + "]";
				}
				else if (objType == "TabPage")
				{
					templateText = GetTemplateText(objPath, "@accessiblename=");
					objPath = objPath.Replace(templateText, direction);
				}
				else if (objType == "TreeItem")
				{
					string treeItem = direction.Replace("item-","");
					templateText = GetTemplateText(objPath, "@text=");
					objPath = objPath.Replace(templateText, treeItem);
				}
				else if (objType == "Container" || objType == "Toolbar" || objType == "Element")
				{
					if (direction != "")
					{
						xy = direction;
						userXY = true;
					}
				}
				else if (objType == "Button")
				{
					if (direction != "")
					{
						string button = direction.Replace("button-","");
						templateText = GetTemplateText(objPath, "@accessiblename=");
						//templateText = GetTemplateText(objPath, "@caption=");
						objPath = objPath.Replace(templateText, button);
					}
				}
				
				Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objPath, 15000);
				if (!userXY)
				{
					string x = (adapterObject.Element.ScreenRectangle.Size.Width / 2).ToString();
					string y = (adapterObject.Element.ScreenRectangle.Size.Height / 2).ToString();
					xy = x + "," + y;
				}
	            adapterObject.Click(xy);
				/*if (IsProgramCrash())
				{
					Report.Failure("Mouse", errorMsg);
					return "Kritisk:System X har krasjet!";
				}*/
				Report.Success("Mouse","Klikk " + objType + "-objektet... OK");
				return "OK";
            }
            catch (ElementNotFoundException)
            {
				Report.Failure("Mouse", errorMsg);
            	return "Kritisk:Ingen element (" + objPath + ") funnet på AUT ELLER AUT kjører ikke";
            }
		}
		
		public static string ObjectDoubleClick(string objType, string objPath, string windowTitle, string key, string direction="")
		{
			Mouse.DefaultMoveTime = 150;
			Mouse.DefaultClickTime = 200;
			EnsureMainWindowVisible();
			string templateText = "";
			Location xy = "2,2";
			bool userXY = false;
			/*try
			{
				Host.Local.FindSingle<Ranorex.Form>("/form[@title~'"+ windowTitle + "']",3000);
			}
			catch (ElementNotFoundException)
			{
				Ranorex.Form frmMain = Host.Local.FindSingle<Ranorex.Form>("/form[@title~'Startmeny']",3000);
				frmMain.Activate();
				frmMain.PressKeys(key);
			}*/
			
            try
            {
				if (objType == "RawText")
				{
					if (direction != "")
					{
						System.DateTime today = System.DateTime.Today;
						System.DateTime newDate = new System.DateTime();
						if (direction == "backward")
						{
							newDate = today.AddDays(-1);
						}
						else if (direction == "forward")
						{
							newDate = today.AddDays(+1);
						}
						else
						{
							newDate = today;
						}
						
						if (direction.Contains("day-"))
						{
							objPath = objPath.Replace("Man", direction.Replace("day-", ""));
						}
						else if (direction.Contains("button-"))
						{
							string rawText = direction.Replace("button-","");
							if (direction.Contains("ALT+"))
							{
								rawText = direction.Replace("button-ALT+","&");
							}
							else if (direction.Contains("CTRL+"))
							{
								rawText = direction.Replace("button-CTRL+","");
							}
							templateText = GetTemplateText(objPath, "@rawtext=");
							objPath = objPath.Replace(templateText, rawText);
						}
						else if (direction.Contains("text-"))
						{
							string rawText = direction.Replace("text-","");
							try
							{
								templateText = GetTemplateText(objPath, "@rawtext=");
							}
							catch
							{
								templateText = GetTemplateText(objPath, "@rawtext~");
							}
							objPath = objPath.Replace(templateText, rawText);
						}
						else if (direction.Contains("contains-"))
						{
							string rawText = direction.Replace("contains-","");
							templateText = GetTemplateText(objPath, "@rawtext~");
							objPath = objPath.Replace(templateText, rawText);
						}
						else
						{
							string date = newDate.Day.ToString();
							if (direction == "fullday")
							{
								string month = today.Month.ToString();
								string year = today.Year.ToString();
								if (date.Length == 1)
								{
									date = "0" + date;
								}
								if (month.Length == 1)
								{
									month = "0" + month;
								}
								date = date + "." + month + "." + year;
							}
							try
							{
								templateText = GetTemplateText(objPath, "@rawtext=");
							}
							catch
							{
								templateText = GetTemplateText(objPath, "@rawtext~");
							}
							objPath = objPath.Replace(templateText, date);
						}
					}
				}
				else if (objType == "RawImage")
				{
					if (direction!= "")
					{
						if (direction.Contains("imageId-"))
						{
							string imageId = direction.Replace("imageId-","");
							templateText = GetTemplateText(objPath, "@id=");
							objPath = objPath.Replace(templateText, imageId);
						}
						else if (direction.Contains("indexId-"))
						{
							string indexId = direction.Replace("indexId-","");
							objPath = objPath + "[" + indexId + "]";
						}
					}
				}
				else if (objType == "RadioButton")
				{
					if (direction!= "")
					{
						if (direction.Contains("rbbutton-"))
						{
							string txtButton = direction.Replace("rbbutton-","");
							templateText = GetTemplateText(objPath, "@text=");
							objPath = objPath.Replace(templateText, txtButton);
						}
					}
				}
				else if (objType == "CheckBox")
				{
					if (direction!= "")
					{
						if (direction.Contains("chkbox-"))
						{
							string txtName = direction.Replace("chkbox-","");
							templateText = GetTemplateText(objPath, "@accessiblename=");
							objPath = objPath.Replace(templateText, txtName);
						}
					}
				}
				else if (objType == "Cell")
				{
					System.DateTime today = System.DateTime.Today;
					System.DateTime newDate = new System.DateTime();
					if (direction == "backward")
					{
						newDate = today.AddDays(-1);
					}
					else if (direction == "forward")
					{
						newDate = today.AddDays(+1);
					}
					else
					{
						newDate = today;
					}
					string date = newDate.Day.ToString();
					Ranorex.Container container = "/form[@controlname='fpdt']/element[@controlname='mc']/container[@accessiblerole='Client']";
					templateText = GetTemplateText(objPath, "@accessiblename=");
					IList<Cell> cellList = container.Find<Ranorex.Cell>(objPath.Replace(templateText,  "‎" + date));
					string index = "1";
					if (cellList.Count == 2)
					{
						int dateToday = Int16.Parse(date);
						index = "2";
						if (dateToday <= 12)
						{
							index = "1";
						}
					}
					templateText = GetTemplateText(objPath, "@accessiblename=");
					objPath = objPath.Replace(templateText,  "‎" + date)+ "[" + index + "]";
				}
				else if (objType == "TabPage")
				{
					templateText = GetTemplateText(objPath, "@accessiblename=");
					objPath = objPath.Replace(templateText, direction);
				}
				else if (objType == "TreeItem")
				{
					string treeItem = direction.Replace("item-","");
					templateText = GetTemplateText(objPath, "@text=");
					objPath = objPath.Replace(templateText, treeItem);
				}
				else if (objType == "Container" || objType == "Toolbar" || objType == "Element")
				{
					if (direction != "")
					{
						xy = direction;
						userXY = true;
					}
				}
				else if (objType == "Button")
				{
					if (direction != "")
					{
						string button = direction.Replace("button-","");
						templateText = GetTemplateText(objPath, "@accessiblename=");
						//templateText = GetTemplateText(objPath, "@caption=");
						objPath = objPath.Replace(templateText, button);
					}
				}
				
				Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objPath, 15000);
				if (!userXY)
				{
					string x = (adapterObject.Element.ScreenRectangle.Size.Width / 2).ToString();
					string y = (adapterObject.Element.ScreenRectangle.Size.Height / 2).ToString();
					xy = x + "," + y;
				}
	            adapterObject.DoubleClick(xy);
				/*if (IsProgramCrash())
				{
					Report.Failure("Mouse", errorMsg);
					return "Kritisk:System X har krasjet!";
				}*/
				Report.Success("Mouse","Dobbelklikk " + objType + "-objektet... OK");
				return "OK";
            }
            catch (ElementNotFoundException)
            {
				Report.Failure("Mouse", errorMsg);
            	return "Kritisk:Ingen element (" + objPath + ") funnet på AUT ELLER AUT kjører ikke";
            }
		}
		
		public static string DoubleClickLogin(string objType, string objPath, string windowTitle, string key, string direction="")
		{
			Mouse.DefaultMoveTime = 150;
			Mouse.DefaultClickTime = 200;
			EnsureMainWindowVisible();
			string templateText = "";
			Location xy = "2,2";
			bool userXY = false;
			
            try
            {
				if (objType == "RawText")
				{
					if (direction != "")
					{
						if (direction.Contains("user-"))
						{
							string rawText = direction.Replace("user-","");
							try
							{
								templateText = GetTemplateText(objPath, "@rawtext=");
							}
							catch
							{
								templateText = GetTemplateText(objPath, "@rawtext~");
							}
							objPath = objPath.Replace(templateText, rawText);
						}
						else if (direction.Contains("contains-"))
						{
							string rawText = direction.Replace("contains-","");
							templateText = GetTemplateText(objPath, "@rawtext~");
							objPath = objPath.Replace(templateText, rawText);
						}
					}
				}
				
				Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objPath, 15000);
				if (!userXY)
				{
					string x = (adapterObject.Element.ScreenRectangle.Size.Width / 2).ToString();
					string y = (adapterObject.Element.ScreenRectangle.Size.Height / 2).ToString();
					xy = x + "," + y;
				}
	            adapterObject.DoubleClick(xy);
				Report.Success("Mouse","Dobbelklikk " + objType + "-objektet... OK");
				return "OK";
            }
            catch (ElementNotFoundException)
            {
				Report.Failure("Mouse", errorMsg);
            	return "Kritisk:Ingen element (" + objPath + ") funnet på AUT ELLER AUT kjører ikke";
            }
		}
		
		public static string ItemSelect(string objPath, string treeItem)
		{
			try
			{
				treeItem = treeItem.Replace("item-","");
				string templateText = GetTemplateText(objPath, "@text=");
				objPath = objPath.Replace(templateText, treeItem);
				Ranorex.TreeItem tiMsgGroup = Host.Local.FindSingle<TreeItem>(objPath, 15000);
				tiMsgGroup.Select();
				/*if (IsProgramCrash())
				{
					Report.Failure("Mouse", errorMsg);
					return "Kritisk:System X har krasjet!";
				}*/
				Report.Success("Mouse","Velg " + objPath + "-objektet... OK");
				return "OK";
			}
			catch (ElementNotFoundException)
            {
				Report.Failure("Mouse", errorMsg);
            	return "Kritisk:Ingen element (" + objPath + ") funnet på AUT ELLER AUT kjører ikke";
            }
		}
		
		public static string CellSelect(string objPath, string cellData)
		{
			try
			{
				cellData = cellData.Replace("data-","");
				string templateText = GetTemplateText(objPath, "@text=");
				objPath = objPath.Replace(templateText, cellData);
				Ranorex.Cell cellDataRow = Host.Local.FindSingle<Cell>(objPath, 15000);
				cellDataRow.Click();
				/*if (IsProgramCrash())
				{
					Report.Failure("Mouse", errorMsg);
					return "Kritisk:System X har krasjet!";
				}
				else
				{
					Report.Success("Mouse","Velg " + objPath + "-objektet... OK");
					return "OK";
				}*/
				Report.Success("Mouse","Velg " + objPath + "-objektet... OK");
				return "OK";
			}
			catch (ElementNotFoundException)
            {
				Report.Failure("Mouse", errorMsg);
            	return "Kritisk:Ingen element (" + objPath + ") funnet på AUT ELLER AUT kjører ikke";
            }
		}
		
		public static string CheckWindowCaption(string objPath, string expectedMsg)
		{
			try
			{
				string response = "OK";
				Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objPath, 2000);
				string actualMsg = adapterObject.Element.GetAttributeValueText("Caption");
				if (expectedMsg.Contains("%date%"))
				{
					System.DateTime today = System.DateTime.Today;
					string strToday = today.ToString().Replace(" 00:00:00", "");
					expectedMsg = expectedMsg.Replace("%date%", strToday);
				}
				if (actualMsg != expectedMsg)
				{
					var newLine = "<br>";
					response = "Feil:Info meldingen som vises er feil" + newLine + newLine + "Aktuell:" + newLine + "\"" + actualMsg.Replace("\n", "<br>") + "\"" + newLine + newLine + "Forventet:" + newLine + "\"" + expectedMsg.Replace("\n", "<br>") + "\"";
				}
				
				return response;
			}
			catch (ElementNotFoundException)
			{
				Report.Failure("Checkpoint", errorMsg);
				return "Kritisk:Kan ikke validere. Vinduet eksisterer ikke";
			}
		}
		
		public static string CheckRecordExist(string objectType, string objectPath, bool visible, string dataRecord)
		{
			string response = "OK";
			string templateText = GetTemplateText(objectPath, "@rawtext=");
			if (visible)
			{
				try
				{
					if (objectType == "RawText")
					{
						Ranorex.RawText rtObject = Host.Local.FindSingle<Ranorex.RawText>(objectPath.Replace(templateText, dataRecord), 2000);
					}
				}
				catch (ElementNotFoundException)
				{
					response = "Feil:Oppføringen (" + dataRecord + ") eksisterer ikke";
				}
			}
			else
			{
				try
				{
					if (objectType == "RawText")
					{
						Ranorex.RawText rtObject = Host.Local.FindSingle<Ranorex.RawText>(objectPath.Replace(templateText, dataRecord), 2000);
					}
					response = "Feil:Oppføringen (" + dataRecord + ") fortsatt eksisterer";
				}
				catch (ElementNotFoundException)
				{
					//Do nothing
				}
			}
			return response;
		}
		
		public static string CheckObjectExist(string objectType, string objectPath, bool visible, string optionalText="")
		{
			string response = "OK";
			string templateText = "";
			
			if (objectType == "RawText")
			{
				if (optionalText != "")
				{
					if (objectPath.Contains("@rawtext~"))
					{
						templateText = GetTemplateText(objectPath, "@rawtext~");
					}
					else
					{
						templateText = GetTemplateText(objectPath, "@rawtext=");
					}
					
					objectPath = objectPath.Replace(templateText, optionalText.Replace("text-", ""));
					if (optionalText.Contains("date-today"))
					{
						System.DateTime today = System.DateTime.Today;
						string date = today.Day.ToString();
						string month = today.Month.ToString();
						string year = today.Year.ToString();
						if (date.Length == 1)
						{
							date = "0" + date;
						}
						if (month.Length == 1)
						{
							month = "0" + month;
						}
						string dato = date + "." + month + "." + year;
						objectPath = objectPath.Replace("date-today", dato);
					}
				}
			}
			else if (objectType == "ListItem")
			{
				if (optionalText != "")
				{
					templateText = GetTemplateText(objectPath, "@text~");
					objectPath = objectPath.Replace(templateText, optionalText.Replace("text-", ""));
				}
			}
			else if (objectType == "Cell")
			{
				if (optionalText != "")
				{
					templateText = GetTemplateText(objectPath, "@text=");
					objectPath = objectPath.Replace(templateText, optionalText.Replace("data-", ""));
				}
			}
			
			if (visible)
			{
				try
	            {
					Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objectPath, 5000);
					/*if (!adapterObject.Visible)
					{
						response = "Feil:Objektet (" + objectType + " - " + objectPath + ") eksisterer ikke";
					}*/
				}
				catch (ElementNotFoundException)
	            {
					Report.Failure("Checkpoint", errorMsg);
	            	response = "Feil:Objektet (" + objectType + " - " + objectPath + ") eksisterer ikke";
	            }
			}
			else
			{
				response = "Feil:Objektet (" + objectType + ") fortsatt eksisterer";
				try
	            {
					Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objectPath, 5000);
					/*if (!adapterObject.Visible)
					{
						response = "OK";
					}
					if (adapterObject.Visible && objectType == "Form")
					{
						response = "Kritisk:Objektet (" + objectPath + ") eksisterer fortsatt eller åpner uventet";
					}*/
				}
				catch (ElementNotFoundException)
	            {
	            	response = "OK";
	            }
			}
			
			return response;
		}
		
		public static void ShowMessage(string message, MessageBoxIcon icon)
		{
			string utilityName = "System XAuto-Test";
			MessageBox.Show(new System.Windows.Forms.Form { TopMost = true }, message, utilityName, MessageBoxButtons.OK, icon);
		}
		
		public static void ConfirmContinue(RxPath objPath, string logFile, string stepNumber)
		{
			DialogResult confirm = ConfirmMessage("Finner ikke objektet. Har du fortsatt ønsker at testen skal fortsette?");
        	if (confirm == DialogResult.No)
        	{
        		Report.Failure("Object Check", errorMsg);
				string[] stepData = new string[] {"&minus;", stepNumber, "&minus;", "&minus;", "Feil", "Kan ikke finne objektet"};
				WriteStepLog(logFile, stepData);
        		throw new ElementNotFoundException("Kan ikke finne objektet", objPath);
        	}
		}
		
		public static string ClickExist(string objPath, string objType, string textHandler)
		{
			Mouse.DefaultMoveTime = 250;
			Mouse.DefaultClickTime = 300;
			try
			{
				if (objType == "Button")
				{
					Ranorex.Button btnObject = Host.Local.FindSingle<Ranorex.Button>(objPath, 5000);
					btnObject.Click();
				}
				else if (objType == "RawImage")
				{
					Ranorex.RawImage riObject = Host.Local.FindSingle<Ranorex.RawImage>(objPath, 5000);
					riObject.Click();
				}
				else if (objType == "RawText")
				{
					if (textHandler != "")
					{
						string templateText = GetTemplateText(objPath, "@rawtext=");
						objPath = objPath.Replace(templateText, textHandler);
					}
					Ranorex.RawText rtObject = Host.Local.FindSingle<Ranorex.RawText>(objPath, 5000);
					rtObject.Click();
				}
				
			}
			catch (ElementNotFoundException)
			{
				//Do nothing
			}
			/*if (IsProgramCrash())
			{
				Report.Failure("Mouse", errorMsg);
				return "Kritisk:System X har krasjet!";
			}
			else
			{
				return "OK";
			}*/
			return "OK";
		}
		
		public static string PressKeysExist(string objPath, string objType, string textHandler)
		{
			Keyboard.DefaultKeyPressTime = 500;
			/*
			while (true)
			{
				try
				{
					Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objPath, 10000);
					adapterObject.PressKeys(textHandler);
				}
				catch (ElementNotFoundException)
				{
					break;
				}
			}
			*/
			try
			{
				Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objPath, 5000);
				adapterObject.PressKeys(textHandler);
			}
			catch (ElementNotFoundException)
			{
				//Do nothing
			}
			/*if (IsProgramCrash())
			{
				Report.Failure("Mouse", errorMsg);
				return "Kritisk:System X har krasjet!";
			}
			else { return "OK"; }*/
			return "OK";
		}
		
		public static DialogResult ConfirmMessage(string message)
		{
			string utilityName = "System XAuto-Test";
			DialogResult answer = MessageBox.Show(new System.Windows.Forms.Form { TopMost = true }, message, utilityName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			return answer;
		}
		
		public static void KillProcess(string processName)
		{
			try
			{
				// Kill System X process
				Process programProcess = Process.GetProcessesByName(processName)[0];
				programProcess.Kill();
				Delay.Seconds(2);
			}
			catch (IndexOutOfRangeException)
			{
				//Do nothing
			}
		}
		
		public static string RestartApplication()
		{
			string response = "OK";
			//Quit System X
			try
			{
				Ranorex.Form frmStartmeny = Host.Local.FindSingle<Ranorex.Form>("/form[@title~'Startmeny' and @processname='Systemx']", 5000);
				frmStartmeny.Activate();
				frmStartmeny.PressKeys("{ControlKey DOWN}{GKey}{ControlKey UP}");
			}
			catch (ElementNotFoundException)
			{
				response = "Finner ikke vinduet (/form[@title~'Startmeny' and @processname='Systemx'])";
			}
			
			//Check if System X crashes after closing
			//CheckProgramCrash("/form[(@name='Systemx.exe' or @name='EPJ, EMR, PAS, HIS' or @name='System X Client') and @processname='WerFault']");
			
			//Open System X
			string logFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string appName = System.IO.File.ReadAllText(logFolder + "\\SystemxPath.log");
			response = RunWinApplication(appName);
			
			//Press A if a dialog opens
			//PressKeysExist("/form[@title='Bekreft']", "Form", "{AKey}");
			
			//Get login details
			string[] details = System.IO.File.ReadAllText(logFolder + "\\login.log").Split(';');
			int cntDetails = details.Length;
			string strUsername = details[cntDetails - 3];
			string strPassword = details[cntDetails - 2];
			
			//Enter username and password
			try
			{
				//Ranorex.Text txtUsername = Host.Local.FindSingle<Ranorex.Text>("/form[@title~'Passord' and @processname='Systemx']/text[@controlname='UsrName']", 2000);
				//Ranorex.Text txtPassword = Host.Local.FindSingle<Ranorex.Text>("/form[@title~'Passord' and @processname='Systemx']/text[@controlname='PWord']", 2000);
				Ranorex.Text txtUsername = Host.Local.FindSingle<Ranorex.Text>("/form[@title~'Passord' and @processname='Systemx']/text[@instance='0']", 2000);
				Ranorex.Text txtPassword = Host.Local.FindSingle<Ranorex.Text>("/form[@title~'Passord' and @processname='Systemx']/text[@instance='1']", 2000);
				txtUsername.Focus();
				txtUsername.PressKeys(strUsername);
				txtPassword.Focus();
				txtPassword.PressKeys(strPassword);
			}
			catch (ElementNotFoundException)
			{
				Ranorex.Text txtUsername_ = Host.Local.FindSingle<Ranorex.Text>("/form[@title~'Passord' and @processname='Systemx']/element[@controlname='UsrName']/text[@accessiblerole='Text']", 2000);
				Ranorex.Text txtPassword_ = Host.Local.FindSingle<Ranorex.Text>("/form[@title~'Passord' and @processname='Systemx']/element[@controlname='PWord']/text[@accessiblerole='Text']", 2000);
				txtUsername_.Focus();
				txtUsername_.PressKeys(strUsername);
				txtPassword_.Focus();
				txtPassword_.PressKeys(strPassword);
			}
			
			//If the logged in user is a Medhjelper
			if (strUsername.Contains("mh-"))
			{
				//Get the doctor's name the medarbeider has logged in to
				string[] detailsDoctor = System.IO.File.ReadAllText(logFolder + "\\login_doctor.log").Split(';');
				int lenLog = detailsDoctor.Length;
				string strDoctor = detailsDoctor[lenLog - 2];
				
				try
				{
					try
					{
						Ranorex.RawText rtDoctor = Host.Local.FindSingle<Ranorex.RawText>("/form[@controlname~'fUserList' or @controlname~'FBrukere']/element[@controlname='pnlUserList']/?/?/rawtext[@rawtext='" + strDoctor + "']", 10000);
						rtDoctor.Click();
					}
					catch (ElementNotFoundException)
					{
						try
						{
							Ranorex.RawText rtDoctor = Host.Local.FindSingle<Ranorex.RawText>("/form[@controlname~'fUserList' or @controlname~'FBrukere']/container[@controlname='pnlUserList']/?/?/rawtext[@rawtext='" + strDoctor + "']", 10000);
							rtDoctor.Click();
						}
						catch (ElementNotFoundException)
						{
							//Do nothing
						}
					}
					Ranorex.Form frmUserList = Host.Local.FindSingle<Ranorex.Form>("/form[@title~'Brukerliste']", 10000);
					frmUserList.PressKeys("{ENTER}");
				}
				catch (ElementNotFoundException)
				{
					response = "Kan ikke finne Brukerliste vinduet";
				}
			}
			
			//This is just to delay
			Delay.Seconds(5);
			
			return response;
		}
		
		public static string RestartSystemX(bool autoLogin=true)
		{
			string response = "Det var en kritisk feil i testen og System X må startes på nytt";
			string logFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			Keyboard.DefaultKeyPressTime = 200;
			try
			{
				// Kill System X process
				KillProcess("Systemx");
				KillProcess("Systemx");
				
				// Update daddress.ini file
				string configFile = @"C:\HMSCLIENT\daddress.ini";
				
				/*if (SetINIAccess())
				{
					File.WriteAllText(configFile, System.IO.File.ReadAllText(configFile).Replace("text=DCOM", "text=CS"));
				}
				else
				{
					response = "Kunne ikke oppdatere " + configFile;
				}*/
				
				System.IO.File.WriteAllText(configFile, System.IO.File.ReadAllText(configFile).Replace("text=DCOM", "text=CS"));
				
				try
				{
					// Get the System X path and run it
					string appName = System.IO.File.ReadAllText(logFolder + "\\SystemxPath.log");
					//Process.Start(appName);
					RunWinApplication(appName);
					
					// If System X is not properly terminated, this window will popup
					try
					{
						Ranorex.Form frmProgFail = Host.Local.FindSingle<Ranorex.Form>("/form[@title='Bekreft']", 10000);
						frmProgFail.PressKeys("{escape}");
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
							string[] details = System.IO.File.ReadAllText(logFolder + "\\login.log").Split(';');
							int cntDetails = details.Length;
							string username = details[cntDetails - 3];
							string password = details[cntDetails - 2];
							
							try
							{
								Ranorex.Form frmPassword = Host.Local.FindSingle<Ranorex.Form>("/form[@title~'Passord' and @processname='Systemx']", 5000);
								frmPassword.Activate();
								try
								{
									Ranorex.Text txtUsername = Host.Local.FindSingle<Ranorex.Text>("/form[@title~'Passord' and @processname='Systemx']/text[@instance='0']", 2000);
									Ranorex.Text txtPassword = Host.Local.FindSingle<Ranorex.Text>("/form[@title~'Passord' and @processname='Systemx']/text[@instance='1']", 2000);
									txtUsername.Focus();
									txtUsername.PressKeys(username);
									txtPassword.Focus();
									txtPassword.PressKeys(password);
								}
								catch (ElementNotFoundException)
								{
									Ranorex.Text txtUsername_ = Host.Local.FindSingle<Ranorex.Text>("/form[@title~'Passord' and @processname='Systemx']/element[@controlname='UsrName']/text[@accessiblerole='Text']", 2000);
									Ranorex.Text txtPassword_ = Host.Local.FindSingle<Ranorex.Text>("/form[@title~'Passord' and @processname='Systemx']/element[@controlname='PWord']/text[@accessiblerole='Text']", 2000);
									txtUsername_.Focus();
									txtUsername_.PressKeys(username);
									txtPassword_.Focus();
									txtPassword_.PressKeys(password);
								}
								
								//If the logged in user is a Medhjelper
								if (username.Contains("mh-"))
								{
									//Get the doctor's name the medarbeider has logged in to
									string[] detailsDoctor = System.IO.File.ReadAllText(logFolder + "\\login_doctor.log").Split(';');
									int lenLog = detailsDoctor.Length;
									string strDoctor = detailsDoctor[lenLog - 2];
									
									try
									{
										try
										{
											Ranorex.RawText rtDoctor = Host.Local.FindSingle<Ranorex.RawText>("/form[@controlname~'fUserList' or @controlname~'FBrukere']/element[@controlname='pnlUserList']/?/?/rawtext[@rawtext='" + strDoctor + "']", 10000);
											rtDoctor.Click();
										}
										catch (ElementNotFoundException)
										{
											try
											{
												Ranorex.RawText rtDoctor = Host.Local.FindSingle<Ranorex.RawText>("/form[@controlname~'fUserList' or @controlname~'FBrukere']/container[@controlname='pnlUserList']/?/?/rawtext[@rawtext='" + strDoctor + "']", 10000);
												rtDoctor.Click();
											}
											catch (ElementNotFoundException)
											{
												//Do nothing
											}
										}
										Ranorex.Form frmUserList = Host.Local.FindSingle<Ranorex.Form>("/form[@title~'Brukerliste']", 10000);
										frmUserList.PressKeys("{ENTER}");
									}
									catch (ElementNotFoundException)
									{
										response = "Kan ikke finne Brukerliste vinduet";
									}
								}
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
				Delay.Seconds(2);
			}
			catch (IndexOutOfRangeException)
			{
				response = "Kan ikke avslutte System X prosess. Kan ikke finne prosessnavnet";
			}
			return response;
		}
		
		public static void EnsureMainWindowVisible()
		{
			try
			{
				Ranorex.Form frmMainWindow = Host.Local.FindSingle<Ranorex.Form>("/form[(@title='System X' or @title='SystemX') and @processname='Systemx']", 5000);
				frmMainWindow.Activate();
				
			}
			catch (ElementNotFoundException)
			{
				Report.Failure("Application", "System X er ikke åpent!");
			}
		}
		
		public static bool IsProgramOpen(RxPath programPath)
		{
			try
			{
				Host.Local.FindSingle<Ranorex.Form>(programPath, 5000);
				return true;
			}
			catch (ElementNotFoundException)
			{
				return false;
			}	
		}
		
		public static string CheckTextColor(string objPath, string text, string textColor)
		{
			string response = "OK";
			string templateText = GetTemplateText(objPath, "@rawtext=");
			try
			{
				Ranorex.RawText rtObjectText = Host.Local.FindSingle<Ranorex.RawText>(objPath.Replace(templateText, text) + "[2]", 3000);
				string actualTextColor = rtObjectText.TextColor;
				if (actualTextColor != textColor)
				{
					response = "Feil:Fargen på teksten endres ikke";
				}
			}
			catch (ElementNotFoundException)
			{
				try
				{
					Ranorex.RawText rtObjectText = Host.Local.FindSingle<Ranorex.RawText>(objPath.Replace(templateText, text) + "[1]", 3000);
					string actualTextColor = rtObjectText.TextColor;
					if (actualTextColor != textColor)
					{
						response = "Feil:Fargen på teksten endres ikke";
					}
				}
				catch (ElementNotFoundException)
				{
					Report.Failure("Checkpoint", errorMsg);
					response = "Kritisk:Objektet eksisterer ikke";
				}
			}
			return response;
		}
		
		public static string CheckObjectText(string objPath, string objType, string direction)
		{
			string response = "OK";
			System.DateTime today = System.DateTime.Today;
			System.DateTime newDate = new System.DateTime();
			bool exception = true;
			string templateText = "";
			if (direction == "backwardNoWeekend")
			{
				newDate = today.AddDays(-1);
				if (today.DayOfWeek == DayOfWeek.Monday)
				{
					newDate = today.AddDays(-3);
				}
			}
			else if (direction == "forwardNoWeekend")
			{
				newDate = today.AddDays(+1);
				if (today.DayOfWeek == DayOfWeek.Friday)
				{
					newDate = today.AddDays(+3);
				}
			}
			else if (direction == "backward")
			{
				newDate = today.AddDays(-1);
				exception = false;
			}
			else if (direction == "forward")
			{
				newDate = today.AddDays(+1);
				exception = false;
			}
			else if (direction == "backward10")
			{
				newDate = today.AddDays(-9);
				exception = false;
			}
			else if (direction == "forward10")
			{
				newDate = today.AddDays(+9);
				exception = false;
			}
			else if (direction.Contains("forward-"))
			{
				string strForwardVal = direction.Replace("forward-", "");
				int intForwardVal = Int16.Parse(strForwardVal);
				newDate = today.AddDays(+intForwardVal);
				exception = false;
			}
			else if (direction.Contains("weekForward-"))
			{
				string strForward = direction.Replace("weekForward-", "");
				int intForward = Int16.Parse(strForward);
				intForward = intForward*7;
				newDate = today.AddDays(+intForward);
				exception = false;
			}
			else if (direction.Contains("monthInterval"))
			{
				string strInterval = direction.Replace("monthInterval_", "");
				int intInterval = Int16.Parse(strInterval);
				newDate = today.AddMonths(+intInterval);
				exception = false;
			}
			else if (direction.Contains("weekInterval"))
			{
				string strInterval = direction.Replace("weekInterval_", "");
				int intInterval = Int16.Parse(strInterval) * 7;
				newDate = today.AddDays(+intInterval);
				exception = false;
			}
			else
			{
				newDate = today;
			}
			if (((newDate.DayOfWeek == DayOfWeek.Saturday) || (newDate.DayOfWeek == DayOfWeek.Sunday)) && exception)
			{
				newDate = today;
			}
			string date = newDate.Day.ToString();
			string month = newDate.Month.ToString();
            string year = newDate.Year.ToString();
            if (date.Length == 1)
            {
            	date = "0" + date;
            }
            if (month.Length == 1)
            {
            	month = "0" + month;
            }
            
			try
			{
				string actual = "";
				string attrValue = "";
				if (objType == "RawText")
				{
					try
					{
						templateText = GetTemplateText(objPath, "@rawtext=");
					}
					catch (IndexOutOfRangeException)
					{
						try
						{
							templateText = GetTemplateText(objPath, "@rawtext~");
						}
						catch (IndexOutOfRangeException)
						{
							//Do nothing
						}
					}
					try
					{
						string[] arrDir = direction.Split('-');
						objPath = objPath.Replace(templateText, direction.Replace(arrDir[0] + "-", ""));
					}
					catch (Exception)
					{
						//Do nothing
					}
					/*if (direction.Contains("dbgridtext-"))
					{
						objPath = objPath.Replace(templateText, direction.Replace("dbgridtext-", ""));
					}
					else if (direction.Contains("textheader-"))
					{
						objPath = objPath.Replace(templateText, direction.Replace("textheader-", ""));
					}
					else if (direction.Contains("match-"))
					{
						objPath = objPath.Replace(templateText, direction.Replace("match-", ""));
					}
					else if (direction.Contains("contains-"))
					{
						objPath = objPath.Replace(templateText, direction.Replace("contains-", ""));
					}*/
					attrValue = "Rawtext";
				}
				else if (objType == "Button")
				{
					attrValue = "Text";
				}
				else if (objType == "RawTextBlock")
				{
					attrValue = "Rawtext";
				}
				else if (objType == "Form")
				{
					attrValue = "Title";
				}
				else if (objType == "Cell" || objType == "CheckBox")
				{
					attrValue = "Accessiblename";
				}
				else if (objType == "Element")
				{
					attrValue = "WindowText";
				}
				else if (objType == "Text" || objType == "ListItem")
				{
					attrValue = "Text";
					if (direction.Contains("item-"))
					{
						templateText = GetTemplateText(objPath, "@text~");
						objPath = objPath.Replace(templateText, direction.Replace("item-", ""));
					}
				}
				
				Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objPath, 10000);
				actual = adapterObject.Element.GetAttributeValueText(attrValue);
				if (objType == "Container")
				{
					Container container = objPath;
					if (direction.Contains("dbgridtext-"))
					{
						string textToFind = direction.Replace("dbgridtext-", "");
						if (CheckContainerText(container, textToFind))
						{
							actual = textToFind;
						}
					}
				}
				
				string expected = date + "." + month + "." + year;
				if (direction.Contains("%date%"))
				{
					string strToday = today.ToString().Replace(" 00:00:00", "");
					direction = direction.Replace("%date%", strToday);
				}
				if (direction.Contains("clock-"))
				{
					expected = direction.Replace("clock-","");
				}
				else if (direction.Contains("day-"))
				{
					expected = direction.Replace("day-","");
					if (expected == "today")
					{
						expected = GetShortDay(today);
					}
				}
				else if (direction.Contains("week-"))
				{
					System.DateTime startDate = GetFirstDayOfWeek(today);
					System.DateTime monday = startDate.AddDays(0);
					System.DateTime sunday = startDate.AddDays(6);
					string monDay = monday.Day.ToString();
					string monMonth = monday.Month.ToString();
					string sunDay = sunday.Day.ToString();
					string friMonth = sunday.Month.ToString();
					if (monDay.Length == 1)
					{
						monDay = "0" + monDay;
					}
					if (monMonth.Length == 1)
					{
						monMonth = "0" + monMonth;
					}
					if (sunDay.Length == 1)
					{
						sunDay = "0" + sunDay;
					}
					if (friMonth.Length == 1)
					{
						friMonth = "0" + friMonth;
					}
					expected = "(" + monDay + "." + monMonth + "." + monday.Year.ToString() + " - " + sunDay + "." + friMonth + "." + sunday.Year.ToString() + ")";
				}
				else if (direction.Contains("dbgridtext-"))
				{
					expected = direction.Replace("dbgridtext-", "");
					expected = expected.Replace("\\TAB", "	");
				}
				else if (direction.Contains("bookName-"))
				{
					expected = direction.Replace("bookName-", "");
				}
				else if (direction.Contains("textheader-"))
				{
					expected = direction.Replace("textheader-", "");
				}
				else if (direction.Contains("item-"))
				{
					expected = direction.Replace("item-", "");
				}
				else if (direction.Contains("contains-"))
				{
					expected = direction.Replace("contains-", "");
				}
				else if (direction.Contains("hastext-"))
				{
					expected = direction.Replace("hastext-", "");
				}
				
				if (direction.Contains("match-"))
				{
					expected = direction.Replace("match-", "");
					string expectedText = expected;
					if (expected.Contains("{EMPTY}"))
					{
						//expectedText = "";
						expectedText = string.Empty;
						//expectedText = null;
					}
					if (expected.Contains("{SPACE}"))
					{
						//expectedText = "";
						expectedText = " ";
					}
					if (actual != expectedText)
					{
						response = "Feil:Innholdet er ikke like <br>Funnet: <br>" + actual + "<br><br>Forventet:<br>" + expected;
					}
				}
				else
				{
					if (direction.Contains("notcontain-"))
					{
						expected = direction.Replace("notcontain-", "");
						if (actual.Contains(expected))
						{
							response = "Feil:Denne teksten (" + expected + ") skal ikke vises";
						}
					}
					else
					{
						if (!actual.Contains(expected))
						{
							response = "Feil:Inneholder ikke forventet tekst <br>Funnet: <br>" + actual + "<br><br>Forventet:<br>" + expected;
						}
					}
				}
			}
			catch (ElementNotFoundException)
			{
				Report.Failure("Checkpoint", errorMsg);
				response = "Feil:Kan ikke validere teksten. Kan ikke finne objektet!";
			}
			
			return response;
		}
		
		public static string CheckProgramCrash(string objectPath)
		{
			string response = "Kritisk:System X har krasjet!";
			try
			{
				Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objectPath, 5000);
				Report.Failure("Checkpoint", "System X har krasjet...");
			}
			catch
			{
				Report.Success("Checkpoint", "System X har ikke krasjet...");
				response = "OK";
			}
			Delay.Seconds(2);
			return response;
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
				try
				{
					// Kill dw20 process
					Process winError2 = Process.GetProcessesByName("dw20")[0];
					winError2.Kill();
					Delay.Seconds(2);
				}
				catch (ElementNotFoundException)
				{
					// Do nothing
				}
			}
		}
		
		public static string CheckWindowSize(RxPath objPath, string size)
		{
			string response = "OK";
			try
			{
				Ranorex.Form frmObject = Host.Local.FindSingle<Ranorex.Form>(objPath, 3000);
				string actual = frmObject.ScreenRectangle.Width.ToString() + "," + frmObject.ScreenRectangle.Height.ToString();
				if (actual != size)
				{
					response = "Feil:Ingen endringer på størrelsen på vinduet";
				}
			}
			catch (ElementNotFoundException)
			{
				Report.Failure("Checkpoint", errorMsg);
				response = "Kritisk:Kan ikke finne objektet";
			}
			
			return response;
		}
		
		public static string GetShortDay(System.DateTime today)
		{
			var cultureInfo = new System.Globalization.CultureInfo("nb-NO");
            var dateTimeInfo = cultureInfo.DateTimeFormat.GetAbbreviatedDayName(today.DayOfWeek);
            string shortDay = dateTimeInfo.ToString();
            //return UppercaseFirst(shortDay.TrimEnd(shortDay[shortDay.Length - 1]));
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
		
		public static string CheckObjectLocation(string objectType, string objectPath, string location)
		{
			string response = "OK";
			string x = "";
			string y = "";
			try
			{
				Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objectPath, 2000);
				x = adapterObject.Element.Location.X.ToString();
				y = adapterObject.Element.Location.Y.ToString();
				string actual = x + "," + y;
				if (actual != location)
				{
					response = "Feil: Den nye posisjonen ble ikke lagret.<br><br>Funnet:<br>" + actual + "<br><br>Forventet:<br>" + location;
				}
			}
			catch (ElementNotFoundException)
			{
				Report.Failure("Checkpoint", errorMsg);
				response = "Kritisk:Kan ikke validere. Objektet eksisterer ikke";
			}
			return response;
		}
		
		public static string ObjectDragDrop(string objectPath, string origin, string destination)
		{
			EnsureMainWindowVisible();
			string response = "OK";
			Mouse.DefaultMoveTime = 250;
			Mouse.DefaultClickTime = 300;
			try
			{
				Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objectPath, 2000);
				Mouse.MoveTo(adapterObject, origin);
				Mouse.ButtonDown(MouseButtons.Left);
				Mouse.MoveTo(adapterObject, destination);
				Mouse.ButtonUp(MouseButtons.Left);
			}
			catch (ElementNotFoundException)
            {
				Report.Failure("Checkpoint", errorMsg);
            	response = "Kritisk:Forventet objekt (" + objectPath + ") mangler. Kan ikke utføre denne testen. System X vil nå starte på nytt.";
            }
			
			return response;
		}
		
		public static bool SetINIAccess()
		{
			string configFile = @"C:\HMSCLIENT\daddress.ini";
			string account = Environment.UserName;
			FileSecurity fSecurity = System.IO.File.GetAccessControl(configFile);
			bool response = true;
			try
			{
				fSecurity.AddAccessRule(new FileSystemAccessRule(account, FileSystemRights.FullControl, AccessControlType.Allow));
				System.IO.File.SetAccessControl(configFile, fSecurity);
			}
			catch
			{
				response = false;
			}
			return response;
		}
		
		public static DataTable RunQuery(string dbServer, string dbName, string SQL)
		{
			string connectionString = "SERVER=" + dbServer + ";" +
	             "DATABASE=" + dbName + ";" +
	             "UID=ta_admin;" +
	              "PASSWORD=dhocc648;" +
					"Connection TIMEOUT=5;" +
				 "Respect Binary Flags=false;";
			MySqlConnection connection = new MySqlConnection(connectionString);
			MySqlDataReader reader = null;
			DataTable dt = new DataTable();
			try
			{
				MySqlCommand command = connection.CreateCommand();
				command.CommandText = SQL;
				connection.Open();
				reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
				dt.Load(reader);
			}
			catch(IndexOutOfRangeException)
			{
			}
			finally
			{
				reader.Close();
				connection.Close();
			}
			
			return dt;
		}
		
		public static bool IsProgramCrash()
		{
			bool response = true;
			string responseCrash = CheckProgramCrash("/form[(@name='Systemx.exe' or @name='EPJ, EMR, PAS, HIS' or @name='System X Client') and @processname='WerFault']");
			if (responseCrash == "OK")
			{
				response = false;
			}
			else
			{
				KillCrashProgram();
				RestartSystemX();
				
			}
			return response;
		}
	}
}
