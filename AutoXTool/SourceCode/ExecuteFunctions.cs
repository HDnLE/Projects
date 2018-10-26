using System;
using System.Windows.Forms;
using Ranorex;
using Ranorex.Core;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace SystemXTest
{
	public static class ExecuteFunction
	{
		public static string ExecuteStep(string stepId)
		{
			string stepCategory = MySQLConnector.GetTestStepInfo(stepId, 4);
			string strResponse = "Pass";
			string cmdId = MySQLConnector.GetTestStepInfo(stepId, 5);
			string stepCommand = MySQLConnector.GetTestCommandInfo(cmdId, 2);
			switch (stepCategory)
			{
				case "Keyboard":
					strResponse = KeyIn(stepId);
					break;
				case "Mouse":
					if (stepCommand == "Click_On_Object")
					{
						strResponse = ObjectClick(stepId);
					}
					/*else if (stepCommand == "Drag_Drop_Object")
					{
						strResponse = ObjectDragDrop(stepId);
					}*/
					else
					{
						strResponse = WindowMove(stepId);
					}
					
					break;
				case "Checkpoint":
					strResponse = ValidateCheckpoint(stepId, stepCommand);
					break;
				case "Application":
					strResponse = RunApp(stepId, stepCommand);
					break;
				case "User":
					string infoMsg = MySQLConnector.GetTestStepInfo(stepId, 7);
					GlobalFunction.ShowMessage(infoMsg, MessageBoxIcon.Information);
					break;
					
			}
			return strResponse;
		}
		
		/*public static string ObjectDragDrop(string id)
		{
			string objects = MySQLConnector.GetTestStepInfo(id, 7);
			GlobalFunction.EnsureMainWindowVisible();
			List<string> arrTestData = GlobalFunction.ExtractTestData(objects);
			string objectPath = MySQLConnector.GetObjectInfo(arrTestData[1], 3);
			string[] location = arrTestData[2].Split(',');
			string from = location[0] + "," + location[1];
			string to = location[2] + "," + location[3];
			string response = "OK";
			
			try
			{
				Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objectPath, 2000);
				Mouse.MoveTo(adapterObject, from);
				Mouse.ButtonDown(MouseButtons.Left);
				Mouse.MoveTo(adapterObject, to);
				Mouse.ButtonUp(MouseButtons.Left);
			}
			catch (ElementNotFoundException)
            {
            	response = "Kritisk:Forventet objekt (" + objectPath + ") mangler. Kan ikke utføre denne testen. System X vil nå starte på nytt.";
            }
			
			return response;
		}*/
		
		public static string KeyIn(string id)
		{
			string txtKey = MySQLConnector.GetTestStepInfo(id, 7);
			string cmdId = MySQLConnector.GetTestStepInfo(id, 5);
			string command = MySQLConnector.GetTestCommandInfo(cmdId, 2);
			string[] arrObjects = txtKey.Split(';');
			string response = "OK";
			GlobalFunction.EnsureMainWindowVisible();
			if (command == "Key_User_Login")
			{
				File.Create(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\login.log").Close();
			}
			foreach (string strObject in arrObjects)
			{
				List<string> arrTestData = GlobalFunction.ExtractTestData(strObject);
	            string objectPath = MySQLConnector.GetObjectInfo(arrTestData[0], arrTestData[1], 5);
	            string objectType = MySQLConnector.GetObjectInfo(arrTestData[0], arrTestData[1], 4);
	            txtKey = GlobalFunction.GetActualKeystrokes(arrTestData[2]);
	            //string programName = MySQLConnector.GetObjectInfo(arrTestData[1], 1);
	            //string windowTitle = MySQLConnector.QueryProgramInfoByName(programName, 5);
	            
	            if (command == "Key_User_Login")
				{
	            	File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\login.log", txtKey + ";");
	            	try
		            {
		            	Host.Local.FindSingle<Ranorex.Form>("/form[@title~'Passord']",3000);
		            }
	            	catch (ElementNotFoundException)
	            	{
	            		GlobalFunction.RestartSystemX(false);
	            	}
				}
	            /*else
	            {
		            try
		            {
		            	Host.Local.FindSingle<Ranorex.Form>("/form[@title~'"+ windowTitle + "']",3000);
		            }
		            catch (ElementNotFoundException)
		            {
		            	string key = MySQLConnector.QueryProgramInfoByName(programName, 2);
	            		if (key != "")
		            	{
			            	Ranorex.Form frmMain = Host.Local.FindSingle<Ranorex.Form>("/form[@title~'Startmeny']",5000);
			            	key = GlobalFunction.GetActualKeystrokes(key);
			            	frmMain.Activate();
			            	frmMain.PressKeys(key);
		            	}
		            	else
		            	{
		            		Ranorex.Form frmMain = Host.Local.FindSingle<Ranorex.Form>("/form[@title~'Startmeny']",5000);
			            	frmMain.Activate();
			            	frmMain.PressKeys(txtKey);
		            	}
		            	
		            }
	            }*/
	            try
	            {
	            	if (objectType == "Text")
	            	{
		            	Ranorex.Text txtObj = Host.Local.FindSingle<Ranorex.Text>(objectPath, 5000);
		            	if (!txtObj.Visible)
		            	{
		            		response = "Kritisk:Forventet objekt (" + objectType + ") mangler. Kan ikke utføre denne testen. System X vil nå starte på nytt.";
		            	}
		            	else
		            	{
			            	txtObj.Focus();
			            	Delay.Seconds(1);
			            	
			            	System.DateTime today = System.DateTime.Today;
							System.DateTime newDate = new System.DateTime();
							
							if (txtKey.Contains("{backward}"))
							{
								newDate = today.AddDays(-1);
								txtKey = txtKey.Replace("{backward}",newDate.Day.ToString());
							}
							else if (txtKey.Contains("{forward}"))
							{
								newDate = today.AddDays(+1);
								txtKey = txtKey.Replace("{forward}",newDate.Day.ToString());
							}
							else if (txtKey.Contains("{today}"))
							{
								newDate = today;
								txtKey = txtKey.Replace("{today}",newDate.Day.ToString());
							}
							   
			            	txtObj.PressKeys(txtKey);
		            	}
	            	}
	            	else if (objectType == "Form")
	            	{
	            		Ranorex.Form frmObj = Host.Local.FindSingle<Ranorex.Form>(objectPath, 5000);
	            		frmObj.Activate();
	            		frmObj.PressKeys(txtKey);
	            	}
	            	else if (objectType == "Container")
	            	{
	            		Ranorex.Container conObj = Host.Local.FindSingle<Ranorex.Container>(objectPath, 5000);
	            		if (!conObj.Visible)
		            	{
		            		response = "Kritisk:Forventet objekt (" + objectType + ") mangler. Kan ikke utføre denne testen. System X vil nå starte på nytt.";
		            	}
	            		else
	            		{
	            			conObj.Focus();
		            		Delay.Seconds(1);
		            		conObj.PressKeys(txtKey);
	            		}
	            		
	            	}
	            }
	            catch (ElementNotFoundException)
	            {
	            	response = "Kritisk:Forventet objekt (" + objectPath + ") mangler. Kan ikke utføre denne testen. System X vil nå starte på nytt.";
	            }
			}
			return response;
		}
		
		public static string WindowMove(string id)
		{
			string objects = MySQLConnector.GetTestStepInfo(id, 7);
			GlobalFunction.EnsureMainWindowVisible();
			List<string> arrTestData = GlobalFunction.ExtractTestData(objects);
			string objectPath = MySQLConnector.GetObjectInfo(arrTestData[0], arrTestData[1], 5);
			string[] location = arrTestData[2].Split(',');
			int x = Int16.Parse(location[0]);
			int y = Int16.Parse(location[1]);
			string response = "OK";
			try
			{
				Ranorex.Form frmWindow = Host.Local.FindSingle<Ranorex.Form>(objectPath,2000);
				frmWindow.Move(x,y);
				Delay.Seconds(2);
			}
			catch (ElementNotFoundException)
			{
				response = "Kritisk:Forventet objekt (" + objectPath + ") mangler. Kan ikke utføre denne testen. System X vil nå starte på nytt.";
			}
			return response;
		}
		
		public static string ObjectClick(string id)
		{
			Mouse.DefaultMoveTime = 1;
			string objects = MySQLConnector.GetTestStepInfo(id, 7);
			string[] arrObjects = objects.Split(';');
			string response = "OK";
			
			GlobalFunction.EnsureMainWindowVisible();
			
			foreach (string strObject in arrObjects)
			{
				List<string> arrTestData = GlobalFunction.ExtractTestData(strObject);
				string objectPath = MySQLConnector.GetObjectInfo(arrTestData[0], arrTestData[1], 5);
	            string objectType = MySQLConnector.GetObjectInfo(arrTestData[0], arrTestData[1], 4);
	            //string programName = MySQLConnector.GetObjectInfo(arrTestData[1], 1);
	            //string windowTitle = MySQLConnector.QueryProgramInfoByName(programName, 5);
	            string templateText = "";
	            Location xy = "5,5";
	            /*try
	            {
	            	Host.Local.FindSingle<Ranorex.Form>("/form[@title~'"+ windowTitle + "']",3000);
	            }
	            catch (ElementNotFoundException)
	            {
	            	string key = GlobalFunction.GetActualKeystrokes(MySQLConnector.QueryProgramInfoByName(programName, 2));
	            	Ranorex.Form frmMain = Host.Local.FindSingle<Ranorex.Form>("/form[@title~'Startmeny']",3000);
	            	frmMain.Activate();
	            	Delay.Seconds(1);
	            	frmMain.PressKeys(key);
	            }*/
	            
	            try
	            {
	            	if (objectType == "RawText")
	            	{
	            		if (arrTestData.Count > 2)
	            		{
	            			System.DateTime today = System.DateTime.Today;
							System.DateTime newDate = new System.DateTime();
							if (arrTestData[2] == "backward")
							{
								newDate = today.AddDays(-1);
							}
							else if (arrTestData[2] == "forward")
							{
								newDate = today.AddDays(+1);
							}
							else
							{
								newDate = today;
							}
							
							if (arrTestData[2].Contains("day-"))
							{
								objectPath = objectPath.Replace("Man", arrTestData[2].Replace("day-", ""));
							}
							else if (arrTestData[2].Contains("button-"))
							{
								string rawText = arrTestData[2].Replace("button-","");
								if (arrTestData[2].Contains("ALT+"))
								{
									rawText = arrTestData[2].Replace("button-ALT+","&");
								}
								else if (arrTestData[2].Contains("CTRL+"))
								{
									rawText = arrTestData[2].Replace("button-CTRL+","");
								}
								templateText = GlobalFunction.GetTemplateText(objectPath, "@rawtext=");
								objectPath = objectPath.Replace(templateText, rawText);
							}
							else if (arrTestData[2].Contains("datarow-"))
							{
								string rawText = arrTestData[2].Replace("datarow-","");
								try
								{
									templateText = GlobalFunction.GetTemplateText(objectPath, "@rawtext=");
								}
								catch
								{
									templateText = GlobalFunction.GetTemplateText(objectPath, "@rawtext~");
								}
								objectPath = objectPath.Replace(templateText, rawText);
							}
							else if (arrTestData[2].Contains("hastext-"))
							{
								string rawText = arrTestData[2].Replace("hastext-","");
								templateText = GlobalFunction.GetTemplateText(objectPath, "@rawtext~");
								objectPath = objectPath.Replace(templateText, rawText);
							}
							else
							{
								string date = newDate.Day.ToString();
								if (arrTestData[2] == "fullday")
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
									templateText = GlobalFunction.GetTemplateText(objectPath, "@rawtext=");
								}
								catch
								{
									templateText = GlobalFunction.GetTemplateText(objectPath, "@rawtext~");
								}
								objectPath = objectPath.Replace(templateText, date);
							}
	            		}
	            		
	            	}
	            	
	            	else if (objectType == "RawImage")
	            	{
	            		if (arrTestData.Count == 3)
	            		{
	            			if (arrTestData[2].Contains("imageId-"))
	            			{
	            				string imageId = arrTestData[2].Replace("imageId-","");
								templateText = GlobalFunction.GetTemplateText(objectPath, "@id=");
								objectPath = objectPath.Replace(templateText, imageId);
	            			}
							else if (arrTestData[2].Contains("indexId-"))
							{
								string indexId = arrTestData[2].Replace("indexId-","");
								objectPath = objectPath + "[" + indexId + "]";
							}
	            		}
	            	}
	            	
	            	else if (objectType == "Cell")
	            	{
	            		System.DateTime today = System.DateTime.Today;
						System.DateTime newDate = new System.DateTime();
						if (arrTestData[2] == "backward")
						{
							newDate = today.AddDays(-1);
						}
						else if (arrTestData[2] == "forward")
						{
							newDate = today.AddDays(+1);
						}
						else
						{
							newDate = today;
						}
						string date = newDate.Day.ToString();
						Ranorex.Container container = "/form[@controlname='fpdt']/element[@controlname='mc']/container[@accessiblerole='Client']";
						templateText = GlobalFunction.GetTemplateText(objectPath, "@accessiblename=");
			            IList<Cell> cellList = container.Find<Ranorex.Cell>(objectPath.Replace(templateText,  "‎" + date));
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
			            templateText = GlobalFunction.GetTemplateText(objectPath, "@accessiblename=");
			            objectPath = objectPath.Replace(templateText, "‎" + date);
			            objectPath = objectPath + "[" + index + "]";
						/*Ranorex.Cell cellObject = Host.Local.FindSingle<Ranorex.Cell>(objectPath + "[" + index + "]", 2000);
	            		cellObject.Click();*/
	            	}
	            	else if (objectType == "TabPage")
	            	{
	            		templateText = GlobalFunction.GetTemplateText(objectPath, "@accessiblename~");
						objectPath = objectPath.Replace(templateText, arrTestData[2]);
	            	}
	            	else if (objectType == "TreeItem")
	            	{
	            		if (arrTestData[2].Contains("item-"))
						{
							string treeItem = arrTestData[2].Replace("item-","");
							templateText = GlobalFunction.GetTemplateText(objectPath, "@text=");
							objectPath = objectPath.Replace(templateText, treeItem);
						}						
	            	}
	            	else if (objectType == "Container" || objectType == "Toolbar" || objectType == "Element")
	            	{
	            		if (arrTestData.Count == 3)
	            		{
	            			xy = arrTestData[2];
	            		}
	            	}
	            	else if (objectType == "Button")
	            	{
	            		if (arrTestData.Count == 3)
	            		{
	            			string button = arrTestData[2].Replace("button-","");
	            			templateText = GlobalFunction.GetTemplateText(objectPath, "@accessiblename=");
							objectPath = objectPath.Replace(templateText, button);
	            		}
	            	}
	            	Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objectPath, 5000);
	            	adapterObject.Click(xy);
	            }
	            catch (ElementNotFoundException)
	            {
	            	response = "Kritisk:Forventet objekt (" + objectPath + ") mangler. Kan ikke utføre denne testen. System X vil nå starte på nytt.";
	            }
			}
			return response;
		}
		
		public static string ValidateCheckpoint(string id, string checkArea)
		{
			string cpId = MySQLConnector.GetTestStepInfo(id, 3);
			string tsId = MySQLConnector.GetTestStepInfo(id, 2);
			string tcId = MySQLConnector.GetTestStepInfo(id, 1);
			string result = "";
			List<string> strObjects = GlobalFunction.ExtractTestData(MySQLConnector.GetTestStepInfo(id, 7));
			string infoMsg = MySQLConnector.GetTestStepInfo(id, 8);
			string objectPath = MySQLConnector.GetObjectInfo(strObjects[0], strObjects[1], 5);
			string objectType = MySQLConnector.GetObjectInfo(strObjects[0], strObjects[1], 4);
			Delay.Seconds(2);
			switch (checkArea)
			{
				case "Object_Exist":
					bool visibile = false;
					string optionalText = "";
					if (infoMsg == "true")
					{
						visibile = true;
					}
					
					if (strObjects.Count == 3)
					{
						optionalText = strObjects[2];
					}
					result = CheckObjectExist(objectType, objectPath, visibile, optionalText);
					break;
				case "Caption":
					result = CheckWindowCaption(infoMsg, objectPath);
					break;
				case "TextColor":
					string text = strObjects[2].Replace("datarow-", "");
					result = CheckTextColor(objectPath, text, infoMsg);
					break;
				case "Object_Text":
					result = CheckObjectText(objectPath, objectType, infoMsg);
					break;
				case "Watch":
					result = InsertWatch(objectPath, objectType, infoMsg);
					break;
				case "Object_Size":
					result = CheckWindowSize(objectPath, infoMsg);
					break;
				case "Click_Exist":
					try
					{
						if (objectType == "RawText")
						{
							string rawText = MySQLConnector.GetTestStepInfo(id, 8);
							if (rawText != "")
							{
								string templateText = GlobalFunction.GetTemplateText(objectPath, "@rawtext=");
								objectPath = objectPath.Replace(templateText, rawText);
							}
						}
						Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objectPath, 2000);
						adapterObject.Click();
					}
					catch (ElementNotFoundException)
					{
						//Do nothing
					}
					result = "OK";
					break;
				case "Press_Keys_Exist":
					try
					{
						string keys = GlobalFunction.GetActualKeystrokes(strObjects[2]);
						Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objectPath, 5000);
						adapterObject.PressKeys(keys);
					}
					catch (ElementNotFoundException)
					{
						//Do nothing
					}
					break;
				case "Record_Exist":
					visibile = false;
					optionalText = strObjects[2];
					if (infoMsg == "true")
					{
						visibile = true;
					}
					result = CheckRecordExist(objectType, objectPath, visibile, optionalText);
					break;
				case "Object_Location":
					result = CheckObjectLocation(objectType, objectPath, infoMsg);
					break;
				case "Program_Crash":
					result = CheckProgramCrash(objectPath);
					break;
			}
			return result;
		}
		
		public static string CheckProgramCrash(string objectPath)
		{
			string response = "Kritisk:System X har krasjet!";
			try
			{
				Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objectPath, 2000);
			}
			catch (ElementNotFoundException)
			{
				response = "OK";
			}
			return response;
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
				response = "Kritisk:Kan ikke validere. Objektet eksisterer ikke";
			}
			return response;
		}
		
		public static string CheckRecordExist(string objectType, string objectPath, bool visible, string dataRecord)
		{
			string response = "OK";
			string templateText = GlobalFunction.GetTemplateText(objectPath, "@rawtext=");
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
			if (objectType == "RawText")
			{
				if (optionalText != "")
				{
					try
					{
						templateText = GlobalFunction.GetTemplateText(objectPath, "@rawtext~");
					}
					catch (IndexOutOfRangeException)
					{
						templateText = GlobalFunction.GetTemplateText(objectPath, "@rawtext=");
					}
					objectPath = objectPath.Replace(templateText, optionalText.Replace("text-", ""));
					if (optionalText.Contains("date-today"))
					{
						objectPath = objectPath.Replace("date-today", dato);
					}
					
				}
			}
			
			else if (objectType == "ListItem")
			{
				if (optionalText != "")
				{
					templateText = GlobalFunction.GetTemplateText(objectPath, "@text~");
					objectPath = objectPath.Replace(templateText, optionalText.Replace("text-", ""));
				}
			}
			
			if (visible)
			{
				try
	            {
					Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objectPath, 5000);
					if (!adapterObject.Visible)
					{
						response = "Feil:Objektet (" + objectPath + ") eksisterer ikke";
					}
					
					return response;
				}
				catch (ElementNotFoundException)
	            {
					return "Feil:Objektet (" + objectPath + ") eksisterer ikke";
	            }
			}
			else
			{
				response = "Feil:Objektet (" + objectPath + ") fortsatt eksisterer";
				try
	            {
					Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objectPath, 2000);
					Delay.Seconds(1);
					if (!adapterObject.Visible)
					{
						response = "OK";
					}
					if (adapterObject.Visible && objectType == "Form")
					{
						response = "Kritisk:Objektet (" + objectPath + ") fortsatt eksisterer. Kan ikke lukkes!";
					}
					return response;
				}
				catch (ElementNotFoundException)
	            {
	            	return "OK";
	            }
			}
			
		}
		
		public static string CheckWindowCaption(string expectedMsg, string objPath)
		{
			GlobalFunction.EnsureMainWindowVisible();
			try
			{
				string response = "OK";
				Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objPath, 5000);
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
				return "Kritisk:Kan ikke validere. Vinduet eksisterer ikke";
			}
		}
		
		public static string RunApp(string id, string appType)
		{
			string appName = MySQLConnector.GetTestStepInfo(id, 7);
			List<string> arrTestData = GlobalFunction.ExtractTestData(appName);
			File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\LastSystemXProgramOpen.log", appName);
			string cmdId = MySQLConnector.GetTestStepInfo(id, 5);
			string response = "OK";
			string keyShortcut = "";
			
			// Program ID
			string programId = arrTestData[0];
			
			// Run Windows application
			if (arrTestData[0] == "9999999")
			{
				try
				{
					appName = arrTestData[1];
					//Process.Start("cmd.exe", "/C ShortcutCreator.bat");
					Delay.Seconds(1);
					Process.Start(appName);
					Delay.Seconds(2);
					try
					{
						Host.Local.FindSingle<Ranorex.Form>("/form[@processname='Systemx']", 5000);
						File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SystemxPath.log", appName);
						response = "OK";
						try
						{
							Ranorex.Form frmProgFail = Host.Local.FindSingle<Ranorex.Form>("/form[@title='Bekreft']", 2000);
							frmProgFail.Activate();
							frmProgFail.PressKeys("{escape}");
						}
						catch (ElementNotFoundException)
						{
							// Do nothing
						}
					}
					catch (ElementNotFoundException)
					{
						response =  "Kritisk:Programmet ikke klart å åpne";
					}
				}
				catch (Exception e)
				{
					response =  "Kritisk:Kan ikke finne den eksekverbar filen (" + e.ToString() + ")";
				}
			}
			
			// System X Program
			else
			{
				GlobalFunction.EnsureMainWindowVisible();
				string ctrlName = "Startmeny";
				string windowTitle = MySQLConnector.GetProgramInfo(programId, 5);
				if (appType.Contains("Key_"))
				{
					if (appType.Contains("Open_"))
					{
						if (arrTestData.Count == 1)
						{
							keyShortcut = GlobalFunction.GetActualKeystrokes(MySQLConnector.GetProgramInfo(programId, 2));
						}
						else if (arrTestData.Count == 2)
						{
							ctrlName = MySQLConnector.GetProgramInfo(programId, 5);
							programId = arrTestData[1];
							keyShortcut = GlobalFunction.GetActualKeystrokes(MySQLConnector.GetSubProgramInfo(programId, 3));
							
							if (!GlobalFunction.IsProgramOpen("/form[@title~'" + windowTitle + "' and @processname='Systemx']"))
							{
								string tmpKey = GlobalFunction.GetActualKeystrokes(MySQLConnector.GetProgramInfo(arrTestData[0], 2));
								Ranorex.Form frmMainProgram = Host.Local.FindSingle<Ranorex.Form>("/form[@title~'Startmeny' and @processname='Systemx']", 5000);
								frmMainProgram.Activate();
				        		frmMainProgram.PressKeys(tmpKey);
							}
							windowTitle = MySQLConnector.GetSubProgramInfo(programId, 6);
						}
					}
					else if (appType.Contains("Close_"))
					{
						keyShortcut = "{escape}";
						ctrlName = MySQLConnector.GetProgramInfo(arrTestData[0], 5);
						if (arrTestData.Count == 2)
						{
							ctrlName = MySQLConnector.GetSubProgramInfo(arrTestData[1], 6);
						}
					}
					
					
					RxPath programPath = "/form[@title~'" + windowTitle + "' and @processname='Systemx']";
					if (!GlobalFunction.IsProgramOpen(programPath) || (appType.Contains("Close_")))
					{
						try
						{
							Ranorex.Form frmMainProgram = Host.Local.FindSingle<Ranorex.Form>("/form[@title~'" + ctrlName + "' and @processname='Systemx']", 5000);
							frmMainProgram.Activate();
			        		frmMainProgram.PressKeys(keyShortcut);
			        		response = "OK";
						}
						catch (ElementNotFoundException)
						{
							response =  "Kritisk:Programmet ikke klart å åpne";
						}
					}
				}
				
				if (appType.Contains("Click_"))
				{
					if (appType.Contains("Open_"))
					{
						string objRole = MySQLConnector.GetProgramInfo(arrTestData[0], 4);
						string btnName = MySQLConnector.GetProgramInfo(arrTestData[0], 3);
						if (arrTestData.Count == 1)
						{
							RxPath programPath = "/form[@title~'" + windowTitle + "' and @processname='Systemx']";
							if (!GlobalFunction.IsProgramOpen(programPath) || (appType.Contains("Close_")))
							{
								response = ClickOpenProgram("Startmeny", objRole, btnName);
							}
						}
						else if (arrTestData.Count == 2)
						{
							string objectRoleSub = MySQLConnector.GetSubProgramInfo(arrTestData[1], 5);
							string accessKeySub = MySQLConnector.GetSubProgramInfo(arrTestData[1], 4);
							if (!GlobalFunction.IsProgramOpen("/form[@title~'" + MySQLConnector.GetProgramInfo(arrTestData[0], 5) + "' and @processname='Systemx']"))
							{
								response = ClickOpenProgram("Startmeny", objRole, btnName);
							}
							if (!GlobalFunction.IsProgramOpen("/form[@title~'" + MySQLConnector.GetSubProgramInfo(arrTestData[1], 6) + "' and @processname='Systemx']"))
							{
								response = ClickOpenProgram(MySQLConnector.GetProgramInfo(arrTestData[0], 5), objectRoleSub, accessKeySub);
							}
						}
					}
					
					if (appType.Contains("Close_"))
					{
						ctrlName = MySQLConnector.GetProgramInfo(arrTestData[0], 5);
						if (arrTestData.Count == 2)
						{
							ctrlName = MySQLConnector.GetSubProgramInfo(arrTestData[1], 6);
						}
						response = ClickCloseProgram(ctrlName);
					}
				}
			}
			
			return response;
		}
		
		public static string DoExecuteMode(string testSuiteId, string testCaseId, string scriptMode, int stepNumber)
		{	
			string strQuery = "SELECT * FROM teststeps WHERE tc_id =" + testCaseId + " AND ts_id=" + testSuiteId + " AND disable=0 ORDER BY step_number ASC";
			string strId = "0";
			string executionTime = GlobalFunction.ExecutionTime();
			System.DateTime start = System.DateTime.Now;
			int searchCount = MySQLConnector.RunQuery(strQuery).Rows.Count;
			string reportFilename = "Reports\\Utility\\report_" + executionTime.Replace("-", String.Empty).Replace(":", String.Empty).Replace(" ", String.Empty) + ".htm";
			string designer = MySQLConnector.GetTestCaseInfo(testSuiteId, testCaseId, 6);
			designer = MySQLConnector.GetUserInfo(designer, 1) + " " + MySQLConnector.GetUserInfo(designer, 2);
			string testCaseTitle = MySQLConnector.GetTestCaseInfo(testSuiteId, testCaseId, 3);
			string testSuiteTitle = MySQLConnector.GetTestSuiteInfo(testSuiteId, 2);
			string programLog = GlobalFunction.GetConfigSettings("EnableErrorLog");
			if (programLog == "true")
			{
				System.IO.File.WriteAllText(reportFilename, System.IO.File.ReadAllText("Reports\\Utility\\report_template.htm"));
			}
			
			GlobalFunction.WriteReportHeaderInfo(reportFilename, "%ExecutionTime%", executionTime);
			GlobalFunction.WriteReportHeaderInfo(reportFilename, "%OperatingSystem%", GlobalFunction.OSVersion());
			GlobalFunction.WriteReportHeaderInfo(reportFilename, "%TSID%", testSuiteId);
			GlobalFunction.WriteReportHeaderInfo(reportFilename, "%TSTitle%", testSuiteTitle);
			GlobalFunction.WriteReportHeaderInfo(reportFilename, "%ComputerName%", Environment.MachineName);
			GlobalFunction.WriteReportHeaderInfo(reportFilename, "%TCID%", testCaseId);
			GlobalFunction.WriteReportHeaderInfo(reportFilename, "%TCTitle%", testCaseTitle);
			GlobalFunction.WriteReportHeaderInfo(reportFilename, "%Designer%", designer);
			int errorCount = 0;
			for (int x=1; x<searchCount; x++)
			{
				if (x == stepNumber)
				{
					string errorMsg = "Skriv inn trinn #";
					int maxStep = searchCount;
					while (stepNumber <= x)
					{
						string value = (stepNumber+1).ToString();
						DialogResult stopConfirm = GlobalFunction.InputBox("Stoppet på trinn #" + x.ToString(), "Neste trinn: (" + errorMsg + ")", ref value, ref maxStep);
						if (stopConfirm == DialogResult.OK)
						{
			            	stepNumber = Int16.Parse(value);
			            	if ((stepNumber >= searchCount) || (stepNumber <= x))
			            	{
			            		stepNumber = x;
			            		errorMsg = "Feil: Gyldig trinn # er " + (x+1).ToString() + " - " + (searchCount-1).ToString();
			            	}
						}
						else if (stopConfirm == DialogResult.Abort)
			            {
			            	stepNumber = searchCount;
			            }
						else
						{
							break;
						}
					}
					if (stepNumber == searchCount)
					{
						string[] stepDataAbort = new string[] {"--:--:--", "--", "User", "Abort", "-X-", "Skriptet ble avbrutt av bruker"};
						GlobalFunction.WriteStepLog(reportFilename, stepDataAbort);
						break;
					}
				}
				strId = MySQLConnector.RunQuery(strQuery).Rows[x][0].ToString();
				
				var stepDuration = System.DateTime.Now.Subtract(start);
				string timeStep = String.Format(@"{0:hh\:mm\:ss\.ff}", stepDuration);
				string stepCategory = MySQLConnector.GetTestStepInfo(strId, 4);
				string cmdId = MySQLConnector.GetTestStepInfo(strId, 5);
				string strCommand = MySQLConnector.GetTestCommandInfo(cmdId, 2);
				string strComments = MySQLConnector.GetTestStepInfo(strId, 6);
				string strResult = "OK";
				Ranorex.Report.Info(stepCategory, "Step " + x.ToString() + ": " + strComments);
				string strResponse = ExecuteStep(strId);
				if (strResponse.Contains("Feil") || strResponse.Contains("Kritisk"))
				{
					strResult = "Feil";
					strComments = strResponse.Replace("Feil:", String.Empty);
					//strComments = strResponse.Replace("Kritisk:", String.Empty);
					errorCount++;
				}
				string[] stepData = new string[] {timeStep, x.ToString(), stepCategory, strCommand, strResult, strComments};
				GlobalFunction.WriteStepLog(reportFilename, stepData);
				if (strResponse.Contains("Kritisk"))
				{
					string restartResponse = GlobalFunction.RestartSystemX();
					stepData = new string[] {"--:--:--", "--", "Application", "Restart SystemX", "Feil", restartResponse};
					GlobalFunction.WriteStepLog(reportFilename, stepData);
					break;
				}
				
			}
			GlobalFunction.WriteReportHeaderInfo(reportFilename, "%TotalError%", errorCount.ToString());
			var duration = System.DateTime.Now.Subtract(start);
			GlobalFunction.WriteReportHeaderInfo(reportFilename, "%Duration%", String.Format(@"{0:hh\:mm\:ss\.ff}", duration));
			return reportFilename;
		}
		
		public static string ClickOpenProgram(string formTitle, string objectRole, string accessKey)
		{
			string response = "";
			GlobalFunction.EnsureMainWindowVisible();
			// If object role is a Button
			if (objectRole == "Button")
			{
				try
				{
					Ranorex.Button btnMainProgram = Host.Local.FindSingle<Ranorex.Button>("/form[@title~'Startmeny']/toolbar[@instance='0']/?/?/button[@accessiblename='" + accessKey + "']", 3000);
					btnMainProgram.Click(300);
					response = "OK";
				}
				catch (ElementNotFoundException)
				{
					response =  "Kritisk:Klarte ikke å finne den knappen ([@accessiblename='" + accessKey + "'])";
				}
			}
			
			// If object role is a toolbar, click on specific location
			else
			{
				try
				{
					string textColor = "#FF0000";
					if (accessKey.Contains("&"))
					{
						textColor = "#000000";
					}
					//Ranorex.ToolBar tbMainProgram = Host.Local.FindSingle<Ranorex.ToolBar>("/form[@title~'" + formTitle + "']/toolbar[@instance='" + instance + "']", 3000);
					Ranorex.RawText rtMainProgram = Host.Local.FindSingle<Ranorex.RawText>("/form[@title~'" + formTitle + "']/?/?/rawtext[@rawtext='" + accessKey + "' and @row!='0' and @textcolor='" + textColor + "']", 3000);
					rtMainProgram.Click();
					response = "OK";
				}
				catch (ElementNotFoundException)
				{
					response =  "Kritisk:Klarte ikke å finne det RawText-objektet";
				}
			}
			
			return response;
		}
		
		public static string ClickCloseProgram(string formTitle)
		{
			try
			{
				Ranorex.Button btnMainProgram = Host.Local.FindSingle<Ranorex.Button>("/form[@title~'" + formTitle + "']/toolbar[@processname='Systemx']/?/?/button[@accessiblerole='PushButton']", 1000);
				btnMainProgram.Click(300);
				return "OK";
			}
			catch (ElementNotFoundException)
			{
				//return "Kritisk:Klarte ikke å finne den EXIT-knappen";
				return "OK";
			}
		}
		
		public static string CheckTextColor(string objPath, string text, string textColor)
		{
			string response = "OK";
			string templateText = GlobalFunction.GetTemplateText(objPath, "@rawtext=");
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
					response =  "Kritisk:Klarte ikke å finne objektet";
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
						templateText = GlobalFunction.GetTemplateText(objPath, "@rawtext=");
					}
					catch (IndexOutOfRangeException)
					{
						try
						{
							templateText = GlobalFunction.GetTemplateText(objPath, "@rawtext~");
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
					}
					else if (direction.Contains("date-"))
					{
						objPath = objPath.Replace(templateText, direction.Replace("date-", ""));
					}*/
					
					attrValue = "Rawtext";
				}
				
				else if (objType == "Button" || objType == "ComboBox")
				{
					attrValue = "Text";
					if (direction.Contains("item-"))
					{
						templateText = GlobalFunction.GetTemplateText(objPath, "@text=");
						objPath = objPath.Replace(templateText, direction.Replace("item-", ""));
					}
				}
				else if (objType == "RawTextBlock")
				{
					if (direction.Contains("dbgridtext-"))
					{
						attrValue = "Rawtext";
					}
				}
				else if (objType == "Cell" || objType == "Element")
				{
					attrValue = "Accessiblename";
				}
				else if (objType == "Text" || objType == "ListItem")
				{
					attrValue = "Text";
					if (direction.Contains("item-"))
					{
						templateText = GlobalFunction.GetTemplateText(objPath, "@text~");
						objPath = objPath.Replace(templateText, direction.Replace("item-", ""));
					}
				}
				else if (objType == "Form")
				{
					attrValue = "Title";
				}
				
				Ranorex.Adapter adapterObject = Host.Local.FindSingle<Unknown>(objPath, 5000);
				actual = adapterObject.Element.GetAttributeValueText(attrValue);
				if (objType == "Container")
				{
					Container container = objPath;
					if (direction.Contains("dbgridtext-"))
					{
						string textToFind = direction.Replace("dbgridtext-", "");
						if (GlobalFunction.CheckContainerText(container, textToFind))
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
						expected = GlobalFunction.GetShortDay(today);
					}
				}
				
				else if (direction.Contains("week-"))
				{
					System.DateTime startDate = GlobalFunction.GetFirstDayOfWeek(today);
					System.DateTime monday = startDate.AddDays(0);
					System.DateTime friday = startDate.AddDays(4);
					string monDay = monday.Day.ToString();
					string monMonth = monday.Month.ToString();
					string friDay = friday.Day.ToString();
					string friMonth = friday.Month.ToString();
					if (monDay.Length == 1)
					{
						monDay = "0" + monDay;
					}
					if (monMonth.Length == 1)
					{
						monMonth = "0" + monMonth;
					}
					if (friDay.Length == 1)
					{
						friDay = "0" + friDay;
					}
					if (friMonth.Length == 1)
					{
						friMonth = "0" + friMonth;
					}
					expected = "(" + monDay + "." + monMonth + "." + monday.Year.ToString() + " - " + friDay + "." + friMonth + "." + friday.Year.ToString() + ")";
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
						expectedText = "";
					}
					if (actual != expectedText)
					{
						response = "Feil:Innholdet er ikke like <br>Funnet: <br>" + actual + "<br><br>Forventet:<br>" + expected;
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
			catch (ElementNotFoundException)
			{
				response = "Feil:Kan ikke validere teksten. Kan ikke finne objektet!";
			}
			return response;
		}
		
		public static string InsertWatch(RxPath objPath, string objType, string watchArea)
		{
			string watch = "";
			string response = "OK";
			try
			{
				if (watchArea == "Size")
				{
					if (objType == "Form")
					{
						Ranorex.Form frmObject = Host.Local.FindSingle<Ranorex.Form>(objPath, 3000);
						watch = frmObject.ScreenRectangle.Width.ToString() + "," + frmObject.ScreenRectangle.Height.ToString();
					}
				}
				File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\watch.log", watch);
			}
			catch (ElementNotFoundException)
			{
				response = "Kritisk:Kan ikke finne objektet";
			}
			return response;
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
				response = "Kritisk:Kan ikke finne objektet";
			}
			return response;
		}
	}
}