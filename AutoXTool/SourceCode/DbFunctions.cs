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
using MySql;
using MySql.Data.MySqlClient;
using System.Data;

namespace SystemXTest
{
	public static class MySQLConnector
	{	
		static string logFilename = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AppEvent.log");
		static string[] logData = new string[] {};
		static string connectionString = GetConnectionString();
		
		public static string GetConnectionString()
		{
			string dbName = (WinComponent.sxDBVersion == String.Empty) ? GlobalFunction.GetConfigSettings("DBName") : "sxtest_" + WinComponent.sxDBVersion.Replace(".", String.Empty);
			string strConnection = "SERVER=" + GlobalFunction.GetConfigSettings("DBServer") + ";" +
	             "DATABASE=" + dbName + ";" +
	             "UID=" + GlobalFunction.GetConfigSettings("DBUsername") + ";" +
	              "PASSWORD=" + GlobalFunction.Decrypt(GlobalFunction.GetConfigSettings("DBPassword"),true) + ";" +
					"Connection TIMEOUT=5;" +
				 "Respect Binary Flags=false;";
			return strConnection;
		}
		
		public static bool CheckDbConnection()
		{
			MySqlConnection myConnection = new MySqlConnection(connectionString);
			try 
			{
				myConnection.Open();
				myConnection.Close();
				return true;
			}
			catch (Exception e)
			{
				//File.WriteAllText("Logs\\DbConnect.log", e.ToString());
				logData = new string[] {"Error", e.ToString(), GlobalFunction.ExecutionTime()};
				GlobalFunction.WriteToLog(logFilename, logData);
				GlobalFunction.ShowMessage("Kan ikke koble til MySQL-database!", MessageBoxIcon.Exclamation);
				//throw new Exception("Feil: Kan ikke koble til MySQL-database");
				return false;
			}
		}
		public static DataTable RunQuery(string SQL)
		{
			if (SQL.Contains("designers"))
			{
				connectionString = "SERVER=" + GlobalFunction.GetConfigSettings("DBServer") + ";" +
	             "DATABASE=systemxtest_main;" +
	             "UID=" + GlobalFunction.GetConfigSettings("DBUsername") + ";" +
	              "PASSWORD=" + GlobalFunction.Decrypt(GlobalFunction.GetConfigSettings("DBPassword"),true) + ";" +
					"Connection TIMEOUT=5;" +
				 "Respect Binary Flags=false;";
			}
			else
			{
				connectionString = GetConnectionString();
			}
			MySqlConnection connection = new MySqlConnection(connectionString);
			MySqlDataReader reader = null;
			DataTable dt = new DataTable();
			try{
				MySqlCommand command = connection.CreateCommand();
				command.CommandText = SQL;
				connection.Open();
				reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
				//load the mysql reader into a databable - this allows the connection to the db to be closed while still
				//having access to the data that the query returned
				dt.Load(reader);
			}catch(IndexOutOfRangeException){
				//GlobalFunction.ShowMessage("Cannot connect to MySQL database. Check the database settings.", MessageBoxIcon.Exclamation);
			}finally{
				reader.Close();
				connection.Close();
			}
			
			return dt;
		}
		
		public static string GetTestStepInfo(string stepId, int intColumn)
		{
			string strQuery = "SELECT * FROM teststeps WHERE ID =" + stepId;
           	string data = RunQuery(strQuery).Rows[0][intColumn].ToString();
			if (data == "System.Byte[]")
			{
				byte[] blobData = (byte[])(RunQuery(strQuery).Rows[0][intColumn]);
				data = System.Text.Encoding.UTF8.GetString(blobData);
			}
            return data;
		}
			
		public static string GetTestCommandInfo(string cmdId, int intColumn)
		{
			string strQuery = "SELECT * FROM test_commands WHERE COMMAND_ID =" + cmdId;
			 return RunQuery(strQuery).Rows[0][intColumn].ToString();
		}
		
		public static string GetProgramInfo(string progId, int infoCol)
		{
			string strQuery = "SELECT * FROM main_programs WHERE program_id =" + progId;
			 return RunQuery(strQuery).Rows[0][infoCol].ToString();
		}
		
		public static string GetSubProgramInfo(string progId, int infoCol)
		{
			string strQuery = "SELECT * FROM sub_programs WHERE sub_prog_id =" + progId;
			 return RunQuery(strQuery).Rows[0][infoCol].ToString();
		}
		
		public static byte[] ObjectToByteArray(Object obj)
		{
		    if(obj == null)
		        return null;
		     System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new  System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
		    using(MemoryStream ms = new MemoryStream())
		    {
		        bf.Serialize(ms, obj);
		        return ms.ToArray();
		    }
		}
		
		public static string GetTestCaseInfo(string testSuiteId, string testCaseId, int intColumn)
		{
			string strQuery = "SELECT * FROM testcases WHERE TC_ID =" + testCaseId + " AND TS_ID=" + testSuiteId;
			return RunQuery(strQuery).Rows[0][intColumn].ToString();
		}
		
		public static string GetTestSuiteInfo(string testSuiteId, int intColumn)
		{
			string strQuery = "SELECT * FROM testsuites WHERE TS_ID =" + testSuiteId;
			return RunQuery(strQuery).Rows[0][intColumn].ToString();
		}
		
		public static string GetUserInfo(string username, int intColumn)
		{
			string strQuery = "SELECT * FROM designers WHERE username ='" + username + "'";
			return RunQuery(strQuery).Rows[0][intColumn].ToString();
		}
		
		public static int GetNumberOfSteps(string testSuiteId, string testCaseId)
		{
			string strQuery = "SELECT * FROM teststeps WHERE TS_ID =" + testSuiteId + " AND TC_ID=" + testCaseId;
			return RunQuery(strQuery).Rows.Count;
		}
		
		public static int GetNumberOfTestsuites()
		{
			string strQuery = "SELECT * FROM testsuites";
			return RunQuery(strQuery).Rows.Count;
		}
		
		public static int GetNumberOfTestcases(bool write=false)
		{
			string strQuery = "SELECT * FROM testcases";
			if (write)
			{
				strQuery = "SELECT * FROM testcases WHERE WRITE_STATUS=1";
			}
			return RunQuery(strQuery).Rows.Count;
		}
		
		public static int GetTestcasesCount(string testSuiteId, bool activeOnly=false)
		{
			string strQuery = "SELECT * FROM testcases WHERE TS_ID=" + testSuiteId;
			if (activeOnly)
			{
				strQuery = "SELECT * FROM testcases WHERE ACTIVE=1 AND TS_ID=" + testSuiteId;
			}
			return RunQuery(strQuery).Rows.Count;
		}
		
		public static bool IsTestCaseExist(string testSuiteId, string testCaseId)
		{
			string strQuery = "SELECT * FROM testcases WHERE TS_ID =" + testSuiteId + " AND TC_ID=" + testCaseId;
			bool result = false;
			if (RunQuery(strQuery).Rows.Count > 0)
			{
				if (GetNumberOfSteps(testSuiteId, testCaseId) > 1)
				{
					result = true;
				}
			}
			return result;
		}
		
		public static bool IsTestCaseActive(string testSuiteId, string testCaseId)
		{
			string strQuery = "SELECT * FROM testcases WHERE TS_ID =" + testSuiteId + " AND TC_ID=" + testCaseId + " AND ACTIVE=1";
			return (RunQuery(strQuery).Rows.Count > 0);
		}
		
		public static string GetLatestTestcaseID(string testSuiteId)
		{
			string strQuery = "SELECT * FROM testcases WHERE TS_ID ='" + testSuiteId + "' ORDER BY TC_ID DESC LIMIT 1";
			return RunQuery(strQuery).Rows[0][1].ToString();
		}
		
		public static bool IsTestsuiteExist(string testSuiteId)
		{
			string strQuery = "SELECT * FROM testsuites WHERE TS_ID =" + testSuiteId;
			bool result = false;
			if (RunQuery(strQuery).Rows.Count > 0)
			{
				result = true;
			}
			return result;
		}
		
		public static List<string> GetTestCaseList(string testSuiteId)
		{
			string strQuery = "SELECT * FROM testcases WHERE TS_ID =" + testSuiteId + " AND ACTIVE=1 ORDER BY TC_ID ASC";
			List<string> result = new List<string>();
			int rowCount = RunQuery(strQuery).Rows.Count;
			if (rowCount > 0)
			{
				for (int i=0; i<rowCount; i++)
				{
					result.Add(RunQuery(strQuery).Rows[i][1].ToString());
				}
			}
			return result;
		}
		
		public static void SetScriptWriteStatus(string testSuiteId, string testCaseId)
		{
			string ranorexVersion = Ranorex.Host.Local.RanorexVersion;
        	ranorexVersion = ranorexVersion.Remove(ranorexVersion.Length - 2);
        	string executionTime = GlobalFunction.ExecutionTime();
        	string userName = WinComponent.loginUser;
			MySQLConnector.RunQuery("UPDATE testcases SET WRITE_STATUS=1, RX_VERSION='" + ranorexVersion + "', BUILD_DATE='" + executionTime + "', BUILDER='" + userName + "' WHERE TS_ID=" + testSuiteId + " AND TC_ID=" + testCaseId);
		}
		
		public static void SetScriptCommit(string testSuiteId, string testCaseId, string commitMessage)
		{
			string commitDate = GlobalFunction.ExecutionTime();
			MySQLConnector.RunQuery("UPDATE testsuites SET LAST_COMMIT_DATE='" + commitDate + "', LAST_COMMIT_MESSAGE='" + commitMessage + "' WHERE ts_id=" + testSuiteId);
			if (testCaseId != "XXX")
			{
				MySQLConnector.RunQuery("UPDATE testcases SET LAST_COMMIT_DATE='" + commitDate + "', LAST_COMMIT_MESSAGE='" + commitMessage + "' WHERE ts_id=" + testSuiteId + " AND tc_id=" + testCaseId);
			}
		}
		
		public static void SetMasterScriptWriteStatus(string testSuiteId)
		{
			string userName = WinComponent.loginUser;
			string buildDate = GlobalFunction.ExecutionTime();
			MySQLConnector.RunQuery("UPDATE testsuites SET WRITE_STATUS=1, BUILD_DATE='" + buildDate + "', BUILDER='" + userName + "' WHERE TS_ID=" + testSuiteId);
		}
		
		public static string GetObjectInfo(string parentId, string childId, int intColumn)
		{
			string strQuery = "SELECT * FROM objects_children WHERE PARENT_ID ='" + parentId + "' AND CHILD_ID = '" + childId + "'";
			return RunQuery(strQuery).Rows[0][intColumn].ToString();
		}
		
		public static string QueryProgramInfoByName(string programName, int column)
		{
			string strQuery = "SELECT * FROM main_programs WHERE prog_name ='" + programName + "'";
			string result = "";
			if (RunQuery(strQuery).Rows.Count > 0)
			{
				result = RunQuery(strQuery).Rows[0][column].ToString();
			}
			return result;
		}
		
		public static void SetUserStatus(string userName, bool logout)
		{
			if (WinComponent.isDbOk)
			{
				if (logout)
				{
					MySQLConnector.RunQuery("UPDATE designers SET ACCESS_POINT=REPLACE(ACCESS_POINT, 'Main|','') WHERE USERNAME='" + userName + "' AND ACCESS_POINT LIKE '%Web%'");
					MySQLConnector.RunQuery("UPDATE designers SET STATUS=0, ACCESS_POINT=NULL WHERE USERNAME='" + userName + "' AND ACCESS_POINT='Main|'");
				}
				else
				{
					MySQLConnector.RunQuery("UPDATE designers SET STATUS=1, ACCESS_POINT=CONCAT(ACCESS_POINT, 'Main|') WHERE USERNAME='" + userName + "' AND ACCESS_POINT NOT LIKE '%Main%'");
					MySQLConnector.RunQuery("UPDATE designers SET STATUS=1, ACCESS_POINT='Main|' WHERE USERNAME='" + userName + "' AND ACCESS_POINT IS NULL");
				}
			}
		}
		
		public static bool IsLoginOK(string userName, string password)
		{
			string strQuery = "SELECT * FROM designers WHERE username ='" + userName + "' AND password = '" + password + "'";
			bool result = false;
			if (RunQuery(strQuery).Rows.Count > 0)
			{
				result = true;
				SetUserStatus(userName, false);
			}
			return result;
		}
		
		public static void DatabaseBackup(string backupDir)
		{
			string mysqldumpPath = GlobalFunction.GetConfigSettings("MySQL");
			string mySqlDump = mysqldumpPath + "\\mysqldump.exe";
			System.DateTime today = System.DateTime.Today;
			//string backupFile = today.Year.ToString() + "-" + today.Month.ToString() + "-" + today.Day.ToString() + "-Database-Backup.sql";
			string backupFile = "ScriptsDB-Backup.sql";
			string serverName = GlobalFunction.GetConfigSettings("DBServer");
			string dbUserName = GlobalFunction.GetConfigSettings("DBUsername");
			string dbPassword = GlobalFunction.Decrypt(GlobalFunction.GetConfigSettings("DBPassword"),true);
			string dbName = GlobalFunction.GetConfigSettings("DBName");
			
			Process dbBackupProcess = new Process();
			dbBackupProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            dbBackupProcess.StartInfo.FileName = "cmd.exe";
            
            if (File.Exists(mySqlDump))
			{
	            dbBackupProcess.StartInfo.Arguments = "/C \"" + mySqlDump + "\" -h " +  serverName + " -u " + dbUserName + " -p" + dbPassword + " " + dbName + " > " + backupDir + "\\" + backupFile + " 2> %appdata%\\Backup-error.log";
			}
			else
			{
				dbBackupProcess.StartInfo.Arguments = @"/C echo Systemet finner ikke angitt bane (" + mySqlDump + ") > %appdata%\\Backup-error.log";
			}
            
            
            dbBackupProcess.Start();
		}
	}
}
