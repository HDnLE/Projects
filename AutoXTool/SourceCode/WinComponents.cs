using System;
using System.Windows.Forms;
using System.IO;
using MySql;
using MySql.Data.MySqlClient;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Reporting;
using Ranorex.Core.Testing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Resources;
using System.Reflection;

namespace SystemXTest
{
	public class WinComponent: System.Windows.Forms.Form
	{
		public MainMenu mnuMain;
		public static string loginUser; 
		public static bool isDbOk;
		//string lastUsedID = GlobalFunction.GetConfigSettings("LastUsedID");
		public string lastLoginID = GlobalFunction.GetConfigSettings("LastLoginID");
		string startMenu = GlobalFunction.GetConfigSettings("StartMenu");
		//string dbVersion = GlobalFunction.GetConfigSettings("DBName");
		ResourceManager resMgr = new ResourceManager("SystemXTest.Images", Assembly.GetExecutingAssembly());
		string menuItemsSource = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GitMessageList.log";
		
		private System.Windows.Forms.ComboBox cboDatabase;
		
		public static string sxDBVersion;
		// Commit/Push to Github
		private TextBox txtCommitMessage;
		private TextBox txtGitRepo;
		
		private DateTimePicker timePicker;
		private System.Windows.Forms.CheckBox chkAutoBackup;
		private NumericUpDown numTestcase;
		
		private ImageList toolBarIcons;
	  	private ToolBarButton tbExit;
	  	private ToolBarButton tbLogout;
	  	private ToolBarButton tbWrite;
	  	private ToolBarButton tbSeparator;
	  	private ToolBarButton tbMasterscript;
	  	private ToolBarButton tbRunAll;
	  	private ToolBarButton tbRunStep;
	  	private ToolBarButton tbOptions;
	  	private ToolBarButton tbClear;
	  	private ToolBarButton tbBackupDb;
	  	private ToolBarButton tbUserManual;
	  	private ToolBarButton tbAbout;
	  	private ToolBarButton tbWebUI;
	  	private ToolBarButton tbJira;
	  	private ToolBarButton tbJenkins;
	  	private ToolBarButton tbGithub;
	  	private ToolBarButton tbDeployFile;
	  	private ToolBarButton tbUsers;
	  	private ToolBarButton tbScripts;
	 	private System.Windows.Forms.ToolBar toolBar;
	  	private System.ComponentModel.Container components = null;
		
		//Welcome Screen
		private PictureBox pb1;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label lblAddress;
		private System.Windows.Forms.Timer tmrClock;
		private System.Windows.Forms.Label lblClock;
		private System.Windows.Forms.Label lblAppInfo;
		
		// 1. Main Menu Items
		private System.Windows.Forms.MenuItem mnuApplication;
		private System.Windows.Forms.MenuItem mnuTools;
		private System.Windows.Forms.MenuItem mnuView;
		private System.Windows.Forms.MenuItem mnuHelp;
		
		// 1.1 Application Menu Items
		private System.Windows.Forms.MenuItem mnuWrite;
		private System.Windows.Forms.MenuItem mnuExecute;
		private System.Windows.Forms.MenuItem mnuExit;
		private System.Windows.Forms.MenuItem mnuLogout;
		
		// 1.1.1 Write Script Sub-menu Items
		private System.Windows.Forms.MenuItem mnuSingle;
		private System.Windows.Forms.MenuItem mnuMultiple;
		private System.Windows.Forms.MenuItem mnuMaster;
		
		// 1.1.2 Execute Sub-menu Items
		private System.Windows.Forms.MenuItem mnuAll;
		private System.Windows.Forms.MenuItem mnuStep;
		
		// 1.2 Tools Menu Items
		private System.Windows.Forms.MenuItem mnuOptions;
		private System.Windows.Forms.MenuItem mnuCleaner;
		private System.Windows.Forms.MenuItem mnuBackup;
		private System.Windows.Forms.MenuItem mnuDeployFile;
		
		// 1.2.1 Settings Menu Items
		private System.Windows.Forms.MenuItem mnuDbSettings;
		private System.Windows.Forms.MenuItem mnuScriptSettings;
		
		// 1.3.1
		private System.Windows.Forms.MenuItem mnuUserlist;
		private System.Windows.Forms.MenuItem mnuScriptStatus;
		private System.Windows.Forms.MenuItem mnuGithub;
		
		// 1.4 Help Menu Items
		private System.Windows.Forms.MenuItem mnuGuide;
		private System.Windows.Forms.MenuItem mnuAbout;
		private System.Windows.Forms.MenuItem mnuWeb;
		
		// 1.4.3 Web Sub-menu Items
		private System.Windows.Forms.MenuItem mnuUI;
		private System.Windows.Forms.MenuItem mnuJira;
		private System.Windows.Forms.MenuItem mnuJenkins;
		
		// Options
		private System.Windows.Forms.TreeNode tnOptions;
		private TreeView treeView1;
		private System.Windows.Forms.Label lblOption;
		private System.Windows.Forms.GroupBox gbOptions;
		private System.Windows.Forms.RadioButton rbButton1;
		private System.Windows.Forms.RadioButton rbButton2;
		private System.Windows.Forms.RadioButton rbButton3;
		private System.Windows.Forms.RadioButton rbButton4;
		private System.Windows.Forms.RadioButton rbButton5;
		private System.Windows.Forms.Label lblPath; 
		private System.Windows.Forms.TextBox txtPath;
		
		private System.Windows.Forms.Button btnCommit;
		private System.Windows.Forms.Button btnDeploy;
		
		// Write and Execute Objects
		private System.Windows.Forms.Button btnAction;
		private System.Windows.Forms.TextBox txtSingleScript;
		private System.Windows.Forms.TextBox txtRangeScript;
		private System.Windows.Forms.TextBox rbSelected;
		private System.Windows.Forms.TextBox chkSelected;
		private System.Windows.Forms.CheckBox chkWrite;
		private System.Windows.Forms.CheckBox chkExecute;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox cboMasterScript;
		private System.Windows.Forms.ComboBox cboTestCase;
		public System.Windows.Forms.Label label1;
		public System.Windows.Forms.Label lblInfoMsg;
		private System.Windows.Forms.Label lblSelTestCase;
		private System.Windows.Forms.Label lblStepNumber;
		private System.Windows.Forms.Label hLine;
		private System.Windows.Forms.Label lblHelp;
		private System.Windows.Forms.Label lblTemp;
		private System.Windows.Forms.TextBox txtTestsuite;
		private System.Windows.Forms.CheckBox chkWriteAll;
		private System.Windows.Forms.CheckBox chkWriteActiveOnly;
		public System.Windows.Forms.Label lblWriteAll;
		
		// Database Settings
		private System.Windows.Forms.Label lblDbServer;
		private System.Windows.Forms.Label lblDbName;
		private System.Windows.Forms.Label lblDbUser;
		private System.Windows.Forms.Label lblDbPass;
		private System.Windows.Forms.TextBox txtServerName;
		private System.Windows.Forms.TextBox txtDbName;
		private System.Windows.Forms.TextBox txtDbUser;
		private System.Windows.Forms.TextBox txtDbPass;
		private System.Windows.Forms.Button btnUpdate;
		
		// Script Settings
		private FolderBrowserDialog folderBrowserDialog1;
		private Label lblScriptFolder;
		private TextBox txtScriptFolder;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.Button btnSave;
		
		// Clear Files
		private System.Windows.Forms.CheckBox chkLogs;
		private System.Windows.Forms.CheckBox chkReports;
		private System.Windows.Forms.CheckBox chkRawScripts;
		private System.Windows.Forms.Label lblLimitSize;
		private System.Windows.Forms.TextBox txtLimitSize;
		private System.Windows.Forms.CheckBox chkDelAtExit;
		private System.Windows.Forms.CheckBox chkOpenLog;		
		
		private TextBox txtBackupDir;
		private System.Windows.Forms.Button btnBackup;
		
		// About
		private System.Windows.Forms.Label lblAbout;
		private System.Windows.Forms.LinkLabel lnkMail;
		
		//Deploy File
		private System.Windows.Forms.Label lblFileSource;
		private System.Windows.Forms.TextBox txtFileSource;
		//private System.Windows.Forms.Button btnBrowseDllSource;
		private System.Windows.Forms.Button btnDeployFile;
		private OpenFileDialog ofdBrowseSource;
		private System.Windows.Forms.Timer tmrClock2;
		
		/*string connectionString = "SERVER=" + GlobalFunction.GetConfigSettings("DBServer") + ";" +
	             "DATABASE=" + GlobalFunction.GetConfigSettings("DBName") + ";" +
	             "UID=" + GlobalFunction.GetConfigSettings("DBUsername") + ";" +
			"PASSWORD=" + GlobalFunction.Decrypt(GlobalFunction.GetConfigSettings("DBPassword"),true) + ";";*/
		
		public WinComponent()
		{
			this.StartInitializeComponent();
			this.StartPosition = FormStartPosition.CenterScreen;
		}
		
		public void InitializeComponent()
		{
			string userName = GlobalFunction.GetConfigSettings("LastLoginID");
			string oldUserLogin =  "<add key=\"LastLoginID\" value=\"" + userName + "\"";
			string password = "";
			string database = "";
			bool response = false;
			/*if (!isDbOk)
			{
				response = true;
				System.Windows.Forms.Application.Run(new WinComponent());
			}*/
			
			while (!response)
			{
				sxDBVersion = database;
				isDbOk = MySQLConnector.CheckDbConnection();
				if (!isDbOk)
				{
					System.Windows.Forms.Application.Run(new WinComponent());
					response = true;
				}
				else
				{
					DialogResult userLogin = UserLogin(ref userName, ref password, ref database);
				
					if (userLogin == DialogResult.OK)
					{
						response = true;
						if (userName == "" && password == "")
						{
							response = false;
							GlobalFunction.ShowMessage("Begge felter må fylles ut!", MessageBoxIcon.Error);
						}
						else if (cboDatabase.Text == "")
						{
							response = false;
							GlobalFunction.ShowMessage("Ingen database valgt!", MessageBoxIcon.Error);
						}
						else
						{
							sxDBVersion = cboDatabase.Text;
							response = MySQLConnector.CheckDbConnection();
							if (response)
							{
								string md5password = GlobalFunction.MD5Hash(password);
								if (!MySQLConnector.IsLoginOK(userName, md5password))
								{
									response = false;
									GlobalFunction.ShowMessage("Feil brukernavn eller passord!", MessageBoxIcon.Error);
								}
							}
						}
						
						if (response)
						{
							string newUserLogin =  "<add key=\"LastLoginID\" value=\"" + userName + "\"";
							string dbSettingsFile = "SystemXAuto-Test.exe.config";
							//Update user login
							File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(oldUserLogin, newUserLogin));
							//lblTemp.Text = userName;
							loginUser = userName;
							System.Windows.Forms.Application.Run(new WinComponent());
						}
					}
					else
					{
						response = true;
					}
				}
			}
		}
		public void StartInitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
		    this.Size = new System.Drawing.Size(300,300);
		
		    toolBar = new System.Windows.Forms.ToolBar();
		    toolBarIcons = new ImageList();
		    tbWrite = new ToolBarButton();
		    tbExit = new ToolBarButton();
		    tbLogout = new ToolBarButton();
		    tbSeparator = new ToolBarButton();
		    tbMasterscript = new ToolBarButton();
		    tbRunAll = new ToolBarButton();
		    tbRunStep = new ToolBarButton();
		    tbOptions = new ToolBarButton();
		    tbClear = new ToolBarButton();
		    tbBackupDb = new ToolBarButton();
		    tbUserManual = new ToolBarButton();
		    tbAbout = new ToolBarButton();
		    tbWebUI = new ToolBarButton();
		    tbJira = new ToolBarButton();
		    tbJenkins = new ToolBarButton();
		    tbGithub = new ToolBarButton();
		    tbDeployFile = new ToolBarButton();
		    tbUsers = new ToolBarButton();
		    tbScripts = new ToolBarButton();
		    
		    ofdBrowseSource = new OpenFileDialog();
		    
			
			//********** INITIALIZE MENU ITEMS **********//
			this.mnuMain = new MainMenu();
			this.Menu = mnuMain;
			
			//********** MAIN MENU ITEMS **********//
			this.mnuApplication = new System.Windows.Forms.MenuItem("&Program");
			this.mnuTools = new System.Windows.Forms.MenuItem("&Verktøy");
			this.mnuView = new System.Windows.Forms.MenuItem("Visnin&g");
			this.mnuHelp = new System.Windows.Forms.MenuItem("&Hjelp");
			
			//********** 1. APPLICATION MENU ITEMS **********//
			// 1.1 WRITE SCRIPT SUB-MENU ITEMS
			this.mnuWrite = new System.Windows.Forms.MenuItem("&Skriv skript");
			this.mnuWrite.Enabled = isDbOk;
			// 1.1.1 Single Script
			this.mnuSingle = new System.Windows.Forms.MenuItem("&Enkelt skript");
			this.mnuSingle.Click+= new EventHandler(mnuWriteSingle_Click);
			this.mnuSingle.Shortcut = Shortcut.CtrlE;
			this.mnuSingle.ShowShortcut = true;
			// 1.1.2 Multiple Scripts
			this.mnuMultiple = new System.Windows.Forms.MenuItem("Test&case");
			this.mnuMultiple.Click+= new EventHandler(mnuWriteMultiple_Click);
			this.mnuMultiple.Shortcut = Shortcut.CtrlC;
			this.mnuMultiple.ShowShortcut = true;
			// 1.1.3 Master Script
			this.mnuMaster = new System.Windows.Forms.MenuItem("&Master skript");
			this.mnuMaster.Click+= new EventHandler(mnuWriteMaster_Click);
			this.mnuMaster.Shortcut = Shortcut.CtrlM;
			this.mnuMaster.ShowShortcut = true;
			
			// 1.2 EXECUTE SCRIPT SUB-MENU ITEMS
			this.mnuExecute = new System.Windows.Forms.MenuItem("&Kjøre skript");
			// 1.2.1 Execute All Steps
			this.mnuAll = new System.Windows.Forms.MenuItem("&Alle trinn");
			this.mnuAll.Click+= new EventHandler(mnuExecute_Click);
			this.mnuAll.Shortcut = Shortcut.CtrlA;
			this.mnuAll.ShowShortcut = true;
			// 1.2.2 Execute Stop at Specific Step
			this.mnuStep = new System.Windows.Forms.MenuItem("&Stoppe på trinn");
			this.mnuStep.Click+= new EventHandler(mnuExecuteStep_Click);
			this.mnuStep.Shortcut = Shortcut.CtrlS;
			this.mnuStep.ShowShortcut = true;
			
			// 1.3 EXIT
			this.mnuExit = new System.Windows.Forms.MenuItem("Avslutt");
			this.mnuExit.Click+= new EventHandler(mnuExit_Click);
			this.mnuExit.ShowShortcut = true;
			
			// 1.4 LOGOUT
			this.mnuLogout = new System.Windows.Forms.MenuItem("Logg &ut");
			this.mnuLogout.Click+= new EventHandler(mnuLogout_Click);
			this.mnuLogout.Shortcut = Shortcut.CtrlU;
			this.mnuLogout.ShowShortcut = true;
			this.mnuLogout.Enabled = isDbOk;
			
			//********** 2. TOOLS MENU ITEMS **********//
			// 2.1 Settings
			this.mnuOptions = new System.Windows.Forms.MenuItem("&Opsjoner");
			this.mnuOptions.Click+= new EventHandler(mnuOptions_Click);
			this.mnuOptions.Shortcut = Shortcut.CtrlO;
			this.mnuOptions.ShowShortcut = true;
			// 2.1.1 Database Settings
			this.mnuDbSettings = new System.Windows.Forms.MenuItem("&Database");
			this.mnuDbSettings.Click+= new EventHandler(mnuDbSettings_Click);
			this.mnuDbSettings.Shortcut = Shortcut.CtrlD;
			this.mnuDbSettings.ShowShortcut = true;
			// 2.1.2 Script Settings
			this.mnuScriptSettings = new System.Windows.Forms.MenuItem("&Skript");
			this.mnuScriptSettings.Click+= new EventHandler(mnuOptions_Click);
			this.mnuScriptSettings.Shortcut = Shortcut.CtrlS;
			this.mnuScriptSettings.ShowShortcut = true;
			// 2.2 Cleaner
			this.mnuCleaner = new System.Windows.Forms.MenuItem("&Tømme mappen for logger");
			this.mnuCleaner.Click+= new EventHandler(mnuCleaner_Click);
			this.mnuCleaner.Shortcut = Shortcut.CtrlT;
			this.mnuCleaner.ShowShortcut = true;
			// 2.3 Backup
			this.mnuBackup = new System.Windows.Forms.MenuItem("Backu&p database");
			this.mnuBackup.Click+= new EventHandler(mnuBackup_Click);
			this.mnuBackup.Shortcut = Shortcut.CtrlP;
			this.mnuBackup.ShowShortcut = true;
			this.mnuBackup.Enabled = isDbOk;
			
			// 2.4 Github
			this.mnuGithub = new System.Windows.Forms.MenuItem("Commit/Push til &Github");
			this.mnuGithub.Click+= new EventHandler(mnuGithub_Click);
			this.mnuGithub.Shortcut = Shortcut.CtrlG;
			this.mnuGithub.ShowShortcut = true;
			this.mnuGithub.Enabled = isDbOk;
			
			// 2.5 Deploy new File
			this.mnuDeployFile = new System.Windows.Forms.MenuItem("&Deploy fil");
			this.mnuDeployFile.Click+= new EventHandler(mnuDeployFile_Click);
			this.mnuDeployFile.Shortcut = Shortcut.CtrlD;
			this.mnuDeployFile.ShowShortcut = true;
			
			//********** 3. VIEW MENU ITEMS **********//
			// 3.1 Userlist
			this.mnuUserlist = new System.Windows.Forms.MenuItem("Bruker&liste");
			this.mnuUserlist.Click += new EventHandler(mnuUserlist_Click);
			this.mnuUserlist.Shortcut = Shortcut.CtrlL;
			this.mnuUserlist.ShowShortcut = true;
			this.mnuUserlist.Enabled = isDbOk;
			
			// 3.1 Script Status
			this.mnuScriptStatus = new System.Windows.Forms.MenuItem("S&kript status");
			this.mnuScriptStatus.Click += new EventHandler(mnuScriptStatus_Click);
			this.mnuScriptStatus.Shortcut = Shortcut.CtrlK;
			this.mnuScriptStatus.ShowShortcut = true;
			this.mnuScriptStatus.Enabled = isDbOk;
			
			//********** 4. HELP MENU ITEMS **********//
			// 4.1 USER GUIDE
			this.mnuGuide = new System.Windows.Forms.MenuItem("&Brukermanual");
			this.mnuGuide.Click+= new EventHandler(mnuHelp_Click);
			this.mnuGuide.Shortcut = Shortcut.CtrlB;
			this.mnuGuide.ShowShortcut = true;
			// 4.2 ABOUT
			this.mnuAbout = new System.Windows.Forms.MenuItem("Om Auto &X Tool");
			this.mnuAbout.Click+= new EventHandler(mnuAbout_Click);
			this.mnuAbout.Shortcut = Shortcut.CtrlX;
			this.mnuAbout.ShowShortcut = true;
			// 4.3 WEB
			this.mnuWeb = new System.Windows.Forms.MenuItem("&Web");
			// 4.3.1 Web UI
			this.mnuUI = new System.Windows.Forms.MenuItem("&Web UI");
			this.mnuUI.Click+= new EventHandler(mnuUI_Click);
			this.mnuUI.Shortcut = Shortcut.CtrlW;
			this.mnuUI.ShowShortcut = true;
			
			// 4.3.2 Jira
			this.mnuJira = new System.Windows.Forms.MenuItem("&Jira");
			this.mnuJira.Click+= new EventHandler(mnuJira_Click);
			this.mnuJira.Shortcut = Shortcut.CtrlJ;
			this.mnuJira.ShowShortcut = true;
			
			// 4.3.3 Jenkins
			this.mnuJenkins = new System.Windows.Forms.MenuItem("J&enkins");
			this.mnuJenkins.Click+= new EventHandler(mnuJenkins_Click);
			this.mnuJenkins.Shortcut = Shortcut.CtrlE;
			this.mnuJenkins.ShowShortcut = true;
			
			//********** ADD MENU ITEMS **********//
			// MAIN MENU ITEMS
			this.mnuMain.MenuItems.Add(this.mnuApplication);
			this.mnuMain.MenuItems.Add(this.mnuTools);
			this.mnuMain.MenuItems.Add(this.mnuView);
			this.mnuMain.MenuItems.Add(this.mnuHelp);
			
			// 1. APPLICATION MENU ITEMS
			// 1.1 WRITE MODE SUB-MENU ITEMS
			this.mnuApplication.MenuItems.Add(this.mnuWrite);
			// 1.1.1 Single Script
			//this.mnuWrite.MenuItems.Add(this.mnuSingle);
			// 1.1.2 Multiple Scripts
			this.mnuWrite.MenuItems.Add(this.mnuMultiple);
			// 1.1.3 Master Script
			this.mnuWrite.MenuItems.Add(this.mnuMaster);
			// 1.2 EXECUTE MODE
			this.mnuApplication.MenuItems.Add(this.mnuExecute);
			// 1.2.1 Execute All Steps
			this.mnuExecute.MenuItems.Add(this.mnuAll);
			// 1.2.2 Execute and Stop at Specific Step
			this.mnuExecute.MenuItems.Add(this.mnuStep);
			//Divider
			this.mnuApplication.MenuItems.Add(new System.Windows.Forms.MenuItem("-"));
			
			// 1.3 EXIT
			this.mnuApplication.MenuItems.Add(this.mnuExit);
			
			// 1.4 LOGOUT
			this.mnuApplication.MenuItems.Add(this.mnuLogout);
			
			this.mnuWrite.Enabled = isDbOk;
			this.mnuExecute.Enabled = false;
			
			
			// 2. TOOLS MENU ITEMS
			// 2.1 Settings
			this.mnuTools.MenuItems.Add(this.mnuOptions);
			// 2.1 Database
			//this.mnuOptions.MenuItems.Add(this.mnuDbSettings);
			// 2.2 Script
			//this.mnuOptions.MenuItems.Add(this.mnuScriptSettings);
			// 2.2 Clear files
			this.mnuTools.MenuItems.Add(this.mnuCleaner);
			// 2.3 Backup database
			this.mnuTools.MenuItems.Add(this.mnuBackup);
			
			// 2.4 Github
			this.mnuTools.MenuItems.Add(this.mnuGithub);
			
			// 2.5 Deploy File
			this.mnuTools.MenuItems.Add(this.mnuDeployFile);
			
			// 3. VIEW MENU ITEMS
			// 3.1 Userlist
			this.mnuView.MenuItems.Add(this.mnuUserlist);
			// 3.2 Script Status
			this.mnuView.MenuItems.Add(this.mnuScriptStatus);
			
			// 4. HELP
			// 4.1 User Guide
			this.mnuHelp.MenuItems.Add(this.mnuGuide);
			// 4.2 About
			this.mnuHelp.MenuItems.Add(this.mnuAbout);
			// 4.3 Web
			this.mnuHelp.MenuItems.Add(this.mnuWeb);
			// 4.3.1 Web UI
			this.mnuWeb.MenuItems.Add(this.mnuUI);
			// 4.3.2 Jira
			this.mnuWeb.MenuItems.Add(this.mnuJira);
			// 4.3.3 Jenkins
			this.mnuWeb.MenuItems.Add(this.mnuJenkins);
			
			
			//********** WINDOWS FORMS INSTANCE SETTINGS **********//
			// WELCOME SCREEN
			this.lblAddress = new System.Windows.Forms.Label();
			this.lblVersion = new System.Windows.Forms.Label();
			this.lblClock = new Label();
			this.pb1 = new PictureBox();
			this.tmrClock = new Timer();
			this.lblAppInfo = new Label();
			
			// DATABASE SETTINGS
			this.lblDbServer = new Label();		//Label for Database Server
			this.txtServerName = new TextBox();	//Input box for Database Server
			this.lblDbName = new Label();		//Label for Database Name
			this.txtDbName = new TextBox();		//Input box for Database Name
			this.lblDbUser = new Label();		//Label for Database Username
			this.txtDbUser = new TextBox();		//Input box for Database Username
			this.lblDbPass = new Label();		//Label for Database User Password
			this.txtDbPass = new TextBox();		//Input box for Database User Password
			this.btnUpdate = new System.Windows.Forms.Button();	//Command button to save modified settings
			// SCRIPT SETTINGS
			this.folderBrowserDialog1 = new FolderBrowserDialog();	//Folder browser dialog for script storage			
			this.btnBrowse = new System.Windows.Forms.Button();	//Command button to browse a folder for script storage
			this.btnSave = new System.Windows.Forms.Button();	//Command button to save modified settings
			this.lblScriptFolder = new Label();
			this.txtScriptFolder = new TextBox();
			// CLEAR FILES
			this.chkLogs = new System.Windows.Forms.CheckBox();
			this.chkReports = new System.Windows.Forms.CheckBox();
			this.chkRawScripts = new System.Windows.Forms.CheckBox();
			this.chkDelAtExit = new System.Windows.Forms.CheckBox();
			this.lblLimitSize = new Label();
			this.txtLimitSize = new TextBox();
			this.chkOpenLog = new System.Windows.Forms.CheckBox();
			
			
			// OPTIONS
			this.rbButton1 = new System.Windows.Forms.RadioButton();
			this.rbButton2 = new System.Windows.Forms.RadioButton();
			this.rbButton3 = new System.Windows.Forms.RadioButton();
			this.rbButton4 = new System.Windows.Forms.RadioButton();
			this.rbButton5 = new System.Windows.Forms.RadioButton();
			
			txtBackupDir = new TextBox();
			btnBackup = new System.Windows.Forms.Button();
			
			this.tmrClock2 = new Timer();
			
			// ABOUT
			this.lblAbout = new Label();		//Label for contents in About
			this.lnkMail = new LinkLabel();		//Link label for email address
			// COMMON FORMS FOR WRITE AND EXECUTE MODES
			this.btnAction = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.hLine = new System.Windows.Forms.Label();
			this.lblSelTestCase = new System.Windows.Forms.Label();
			this.lblStepNumber = new System.Windows.Forms.Label();
			this.lblInfoMsg = new System.Windows.Forms.Label();
			this.lblTemp = new System.Windows.Forms.Label();
			this.txtSingleScript = new System.Windows.Forms.TextBox();
			this.txtRangeScript = new System.Windows.Forms.TextBox();
			this.numTestcase = new NumericUpDown();
			this.txtTestsuite = new TextBox();
			this.chkWrite = new System.Windows.Forms.CheckBox();
			this.chkExecute = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rbSelected = new System.Windows.Forms.TextBox();
			this.chkSelected = new System.Windows.Forms.TextBox();
			this.cboMasterScript = new System.Windows.Forms.ComboBox();
			this.cboTestCase = new System.Windows.Forms.ComboBox();
			this.lblHelp = new System.Windows.Forms.Label();
			this.lblWriteAll = new System.Windows.Forms.Label();
			
			this.gbOptions = new System.Windows.Forms.GroupBox();
			this.lblOption = new Label();
			this.tnOptions = new TreeNode("Database");
			this.treeView1 = new TreeView();
			treeView1.Nodes.Add(tnOptions);
			this.tnOptions = new TreeNode("Skripter");
			treeView1.Nodes.Add(tnOptions);
			this.tnOptions = new TreeNode("Logger");
			treeView1.Nodes.Add(tnOptions);
			this.tnOptions = new TreeNode("Startmeny");
			treeView1.Nodes.Add(tnOptions);
			this.tnOptions = new TreeNode("MSBuild");
			treeView1.Nodes.Add(tnOptions);
			this.tnOptions = new TreeNode("MySQL");
			treeView1.Nodes.Add(tnOptions);
			this.tnOptions = new TreeNode("AutoItBuild");
			treeView1.Nodes.Add(tnOptions);
			treeView1.Location = new System.Drawing.Point(5, 15);
			this.treeView1.Size = new System.Drawing.Size(75, 150);
			treeView1.BorderStyle = BorderStyle.None;
			treeView1.NodeMouseClick += new TreeNodeMouseClickEventHandler(treeView1_NodeMouseClick);
			treeView1.KeyPress += new KeyPressEventHandler(frmMain_Keypress);
			chkWriteAll = new System.Windows.Forms.CheckBox();
			chkWriteActiveOnly = new System.Windows.Forms.CheckBox();
			
			//********** WINDOWS FORMS PROPERTY SETTINGS **********//
			// WELCOME SCREEN
			// Label: HMS Address
			this.lblAddress.Location = new System.Drawing.Point(280, 168);
			this.lblAddress.Size = new System.Drawing.Size(330, 23);
            this.lblAddress.TabIndex = 0;
            this.lblAddress.Text = "Hove Medical Systems AS | Dyrmyrgata 35, 3611 Kongsberg";
            this.lblAddress.Enabled = false;
            
            // Image: HMS and SystemX Logos
			this.pb1.Location = new System.Drawing.Point(290, 28);
			this.pb1.Image = (Image)resMgr.GetObject("Logo");
			this.pb1.Size = new System.Drawing.Size(300,196);
			
			// Label: System XAuto-Test Version Info
			this.lblVersion.Location = new System.Drawing.Point(505, 143);
			this.lblVersion.Size = new System.Drawing.Size(150, 23);
			this.lblVersion.TabIndex = 0;
			this.lblVersion.Text = "v" + GetAppInfo("Version");
			this.lblVersion.ForeColor = System.Drawing.Color.DimGray;
			
			// Label: Clock
			this.lblClock.Location = new System.Drawing.Point(463, 202);
			this.lblClock.Size = new System.Drawing.Size(200, 30);
            this.lblClock.TabIndex = 0;
            this.lblClock.Enabled = false;
            
            // Label: System XAuto-Test Info
			this.lblAppInfo.Location = new System.Drawing.Point(450, 45);
			this.lblAppInfo.Size = new System.Drawing.Size(170, 50);
			this.lblAppInfo.TabIndex = 0;
			//string databaseVersion = GlobalFunction.GetConfigSettings("DBVersion");
			string databaseVersion = "v" + sxDBVersion;
			
			string ranorexVersion = Ranorex.Host.Local.RanorexVersion;
        	ranorexVersion = ranorexVersion.Remove(ranorexVersion.Length - 2);
			
			this.lblAppInfo.Text = "Version " + GetAppInfo("Version") + "\n" +
									"Build Date: " + GetAppInfo("BuildDate") + "\n" +
									"Database Version: " +  databaseVersion + "\n" +
									"Ranorex Version: " + ranorexVersion;
			this.lblAppInfo.BackColor = System.Drawing.Color.Transparent;
			this.lblAppInfo.ForeColor = System.Drawing.Color.DimGray;
			
			// Timer: Clock
			this.tmrClock.Tick+= new EventHandler(tmrClock_Tick);
			this.tmrClock.Enabled = true;
			this.tmrClock.Interval = 1000;

						
            
			// DATABASE SETTINGS (Begin)
			//Label: Database Server
            this.lblDbServer.Location = new System.Drawing.Point(1, 7);
			this.lblDbServer.Size = new System.Drawing.Size(65, 20);
            this.lblDbServer.Text = "Server navn";
            this.lblDbServer.TextAlign = ContentAlignment.MiddleRight;
            //Textbox: Database Server
            this.txtServerName.Location = new System.Drawing.Point(65, 7);
			this.txtServerName.Size = new System.Drawing.Size(110, 20);
			this.txtServerName.BorderStyle = BorderStyle.FixedSingle;
			
			
			//Label: Database Name
            this.lblDbName.Location = new System.Drawing.Point(1, 33);
			this.lblDbName.Size = new System.Drawing.Size(63, 20);
            this.lblDbName.Text = "DB navn";
            this.lblDbName.TextAlign = ContentAlignment.MiddleRight;
            //Textbox: Database Name
            this.txtDbName.Location = new System.Drawing.Point(65, 33);
			this.txtDbName.Size = new System.Drawing.Size(110, 20);
			this.txtDbName.BorderStyle = BorderStyle.FixedSingle;
			
			this.cboMasterScript.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
                        | System.Windows.Forms.AnchorStyles.Right);
			this.cboMasterScript.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cboMasterScript.Location = new System.Drawing.Point(65,33);
			this.cboMasterScript.Size = new System.Drawing.Size(110, 21);
            this.cboMasterScript.TabIndex = 7;
            this.cboMasterScript.SelectedValueChanged+= new EventHandler(cboMasterScript_Change);
            			
			//Label: Database Username
            this.lblDbUser.Location = new System.Drawing.Point(1, 59);
			this.lblDbUser.Size = new System.Drawing.Size(63, 20);
            this.lblDbUser.Text = "Brukernavn";
            this.lblDbUser.TextAlign = ContentAlignment.MiddleRight;
            //Textbox: Database Username
            this.txtDbUser.Location = new System.Drawing.Point(65, 59);
			this.txtDbUser.Size = new System.Drawing.Size(110, 20);
			this.txtDbUser.BorderStyle = BorderStyle.FixedSingle;
			
			//Label: Database Password
            this.lblDbPass.Location = new System.Drawing.Point(1, 85);
			this.lblDbPass.Size = new System.Drawing.Size(63, 20);
            this.lblDbPass.Text = "Passord";
            this.lblDbPass.TextAlign = ContentAlignment.MiddleRight;
            //Textbox: Database Password
            this.txtDbPass.Location = new System.Drawing.Point(65, 85);
			this.txtDbPass.Size = new System.Drawing.Size(110, 20);
			this.txtDbPass.PasswordChar = '•';
			this.txtDbPass.BorderStyle = BorderStyle.FixedSingle;
			
			//Button: Update Database Settings
			this.btnUpdate.Location = new System.Drawing.Point(65, 134);
			this.btnUpdate.Size = new System.Drawing.Size(110, 24);
			
			// DATABASE SETTINGS (End)
			
			// SCRIPT SETTINGS (Begin)
			//Button: Update Script Settings
			this.btnSave.Location = new System.Drawing.Point(40, 107);
			this.btnSave.Size = new System.Drawing.Size(110, 24);
			this.btnSave.Text = "&Lagre";
			this.btnSave.Click += new EventHandler(btnSave_Click);
			

			//Button: Browse a folder
			this.btnBrowse.Location = new System.Drawing.Point(150, 50);
			this.btnBrowse.Size = new System.Drawing.Size(25, 20);
			this.btnBrowse.Text = "...";
			btnBrowse.Font = new System.Drawing.Font(btnBrowse.Font, System.Drawing.FontStyle.Bold);
			this.btnBrowse.Click+= new EventHandler(browseFolder_Click);	
			
			//Label: Script Storage
			this.lblScriptFolder.Location = new System.Drawing.Point(1, 34);
			this.lblScriptFolder.Size = new System.Drawing.Size(150, 20);
            this.lblScriptFolder.Text = "Lagringsplass for skripter";
            //Textbox: Script Storage
            this.txtScriptFolder.Location = new System.Drawing.Point(3, 50);
			this.txtScriptFolder.Size = new System.Drawing.Size(145, 20);
			this.txtScriptFolder.BorderStyle = BorderStyle.FixedSingle;
			this.txtScriptFolder.KeyPress+= new System.Windows.Forms.KeyPressEventHandler(txtScriptFolder_Keypress);
			txtScriptFolder.ContextMenu = new System.Windows.Forms.ContextMenu();
			// SCRIPT SETTINGS (End)
			
			// CLEAR FILES
			this.chkLogs.Checked = true;
			this.chkLogs.Size = new System.Drawing.Size(220, 17);
			
			this.chkReports.Location = new System.Drawing.Point(15, 85);
			this.chkReports.Size = new System.Drawing.Size(220, 17);
			this.chkReports.Text = "Testrapporter";
			this.chkReports.Checked = true;
			
			this.chkRawScripts.Location = new System.Drawing.Point(15, 105);
			this.chkRawScripts.AutoSize = true;
			this.chkRawScripts.Text = "RX prosjekter";
			this.chkRawScripts.Checked = true;
			
			this.lblLimitSize.Text = "Logger / Rapporter grense\r\n(Aktiveres etter omstart)";
			this.lblLimitSize.Location = new System.Drawing.Point(1, 34);
			this.lblLimitSize.Size = new System.Drawing.Size(220, 30);
			this.lblLimitSize.BackColor = System.Drawing.Color.Transparent;
			
			this.txtLimitSize.Location = new System.Drawing.Point(135, 38);
			this.txtLimitSize.Size = new System.Drawing.Size(40, 30);
			this.txtLimitSize.TabIndex = 5;
			this.txtLimitSize.KeyPress+= new System.Windows.Forms.KeyPressEventHandler(InputValueControl);
			this.txtLimitSize.BorderStyle = BorderStyle.FixedSingle;
			txtLimitSize.ContextMenu = new System.Windows.Forms.ContextMenu();
			
			this.chkDelAtExit.Location = new System.Drawing.Point(15, 105);
			this.chkDelAtExit.Size = new System.Drawing.Size(220, 17);
			this.chkDelAtExit.Text = "Slett logger på exit (Krever omstart)";
			this.chkDelAtExit.Checked = false;
			
			this.chkOpenLog.Checked = true;
			this.chkOpenLog.Text = "Åpne loggfilen under skriving \r\nav script";
			this.chkOpenLog.Location = new System.Drawing.Point(3, 65);
			this.chkOpenLog.Size = new System.Drawing.Size(170, 30);
			
			// OPTIONS
			lblOption.Location = new System.Drawing.Point(85, 15);
			lblOption.Size = new System.Drawing.Size(180, 20);
            lblOption.TabIndex = 0;
            lblOption.TextAlign = ContentAlignment.MiddleCenter;
            lblOption.Font = new System.Drawing.Font(lblOption.Font.FontFamily, 11);
            lblOption.Font = new System.Drawing.Font(lblOption.Font, System.Drawing.FontStyle.Bold);
            
            rbButton1.Text = "Skrive skripter";
            rbButton1.Location = new System.Drawing.Point(5, 7);
            rbButton1.CheckedChanged += new EventHandler(rbButton_CheckedChanged);
            rbButton1.Name = "write_script";
            rbButton2.Text = "Skrive masterskript";
            rbButton2.Location = new System.Drawing.Point(5, 27);
            rbButton2.Size = new System.Drawing.Size(120, 18);
            rbButton2.BackColor = System.Drawing.Color.Transparent;
            rbButton2.CheckedChanged += new EventHandler(rbButton_CheckedChanged);
            rbButton2.Name = "write_masterscript";
            rbButton3.Text = "Kjøre skripter";
            rbButton3.Location = new System.Drawing.Point(5, 40);
            rbButton3.CheckedChanged += new EventHandler(rbButton_CheckedChanged);
            rbButton3.Name = "run_script";
            rbButton4.Text = "Kjøre skript og stoppe på trinn";
            rbButton4.Location = new System.Drawing.Point(5, 60);
            rbButton4.Size = new System.Drawing.Size(172, 18);
            rbButton4.CheckedChanged += new EventHandler(rbButton_CheckedChanged);
            rbButton4.Name = "run_to_step";
            rbButton5.Text = "Om dette programmet";
            rbButton5.Location = new System.Drawing.Point(5, 75);
            rbButton5.Size = new System.Drawing.Size(170, 18);
            rbButton5.CheckedChanged += new EventHandler(rbButton_CheckedChanged);
            rbButton5.Name = "about";
			
			// ************************
			this.groupBox1.Location = new System.Drawing.Point(8, 25);
			this.groupBox1.Size = new System.Drawing.Size(270, 170);
			this.groupBox1.Visible = false;
			groupBox1.KeyPress += new KeyPressEventHandler(frmMain_Keypress);
			
			
			this.gbOptions.Location = new System.Drawing.Point(85, 32);
			this.gbOptions.Size = new System.Drawing.Size(181, 134);
			this.gbOptions.Visible = false;
			
			this.btnAction.Location = new System.Drawing.Point(65, 138);
			this.btnAction.Size = new System.Drawing.Size(110, 24);
			this.btnAction.Click += new EventHandler(btnAction_Click);
									
			this.chkWrite.Location = new System.Drawing.Point(15, 25);
			this.chkWrite.Size = new System.Drawing.Size(100, 17);
			this.chkWrite.Text = "Write Script";
						
			this.chkExecute.Location = new System.Drawing.Point(15, 45);
			this.chkExecute.Size = new System.Drawing.Size(100, 17);
			this.chkExecute.Text = "Execute Script";
						
			this.txtRangeScript.Location = new System.Drawing.Point(10, 90);
			this.txtRangeScript.Size = new System.Drawing.Size(230, 20);
			this.txtRangeScript.TabIndex = 5;
			this.txtRangeScript.Text = "";
			this.txtRangeScript.BorderStyle = BorderStyle.FixedSingle;
			
			this.numTestcase.Location = new System.Drawing.Point(10, 90);
			this.numTestcase.Size = new System.Drawing.Size(230, 20);
			this.numTestcase.TabIndex = 5;
			this.numTestcase.Text = "";
			this.numTestcase.BorderStyle = BorderStyle.FixedSingle;
			numTestcase.Minimum = 100;
			
			this.txtSingleScript.Location = new System.Drawing.Point(130, 90);
			this.txtSingleScript.Size = new System.Drawing.Size(110, 20);
			this.txtSingleScript.TabIndex = 5;
			this.txtSingleScript.BorderStyle = BorderStyle.FixedSingle;
			
			this.txtTestsuite.Location = new System.Drawing.Point(10, 40);
			this.txtTestsuite.Size = new System.Drawing.Size(230, 21);
			this.txtTestsuite.TabIndex = 5;
			this.txtTestsuite.BorderStyle = BorderStyle.FixedSingle;
			this.txtTestsuite.Text = GlobalFunction.GetLastUsedTSID();
			txtTestsuite.ContextMenu = new System.Windows.Forms.ContextMenu();
			
			this.label1.Location = new System.Drawing.Point(10, 25);
			this.label1.Size = new System.Drawing.Size(130, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Skriv inn testsuite ID:";
            
            this.lblSelTestCase.Location = new System.Drawing.Point(10, 75);
			this.lblSelTestCase.Size = new System.Drawing.Size(180, 20);
            this.lblSelTestCase.TabIndex = 0;
            this.lblSelTestCase.Text = "Velg en Test&case:";
            
            this.lblStepNumber.Location = new System.Drawing.Point(130, 75);
			this.lblStepNumber.Size = new System.Drawing.Size(180, 20);
            this.lblStepNumber.TabIndex = 0;
            this.lblStepNumber.Text = "Trinn nr. å stoppe";
            this.lblStepNumber.BackColor = System.Drawing.Color.Transparent;
            
            this.lblInfoMsg.Location = new System.Drawing.Point(0, 202);
			this.lblInfoMsg.Size = new System.Drawing.Size(684, 30);
			this.lblInfoMsg.BackColor = System.Drawing.Color.Empty;
            this.lblInfoMsg.TabIndex = 0;
            
            this.lblHelp.Location = new System.Drawing.Point(10, 110);
			this.lblHelp.Size = new System.Drawing.Size(230, 20);
            this.lblHelp.Text = "(Eksampel: 1,5,7-15)";
            this.lblHelp.Enabled = false;
                        
			// ABOUT (Begin)
			this.lblAbout.Location = new System.Drawing.Point(10, 25);
			this.lblAbout.Size = new System.Drawing.Size(235, 100);
            this.lblAbout.BackColor = System.Drawing.Color.Transparent;
            this.lnkMail.Location = new System.Drawing.Point(10, 140);
			this.lnkMail.Size = new System.Drawing.Size(140, 20);
			this.lnkMail.LinkColor = System.Drawing.Color.Navy;
			this.lnkMail.BackColor = System.Drawing.Color.Transparent;
			this.lnkMail.Text = "rommel@hovemedical.no";
			this.lnkMail.Click += new EventHandler(lnkMail_Click);
            
            this.cboTestCase.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
                        | System.Windows.Forms.AnchorStyles.Right);
			this.cboTestCase.DropDownWidth = 130;
			this.cboTestCase.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cboTestCase.Location = new System.Drawing.Point(10,90);
			this.cboTestCase.Size = new System.Drawing.Size(230, 21);
            this.cboTestCase.TabIndex = 7;
            
            this.hLine.AutoSize = false;
            this.hLine.Size = new System.Drawing.Size(584, 2);
            this.hLine.BorderStyle = BorderStyle.Fixed3D;
            this.hLine.Location = new System.Drawing.Point(0, 200);
			
			this.ClientSize = new System.Drawing.Size(584, 220);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
			                       	this.lblAddress,
			                       	this.lblAppInfo,
			                       	this.lblClock,
			                       	this.lblInfoMsg,
			                       	this.hLine,
			                       	this.pb1,
			                       	this.groupBox1
			                       });
			this.Icon = (Icon)resMgr.GetObject("HMS");
			string fullUserInfo = "";
			if (loginUser != null) 
			{
				fullUserInfo = MySQLConnector.GetUserInfo(loginUser,1) + " " + MySQLConnector.GetUserInfo(loginUser,2);
			}
			
			this.Text = "Auto X Tool (" + fullUserInfo + ")";
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(600, 277);
			this.MinimumSize = new System.Drawing.Size(600, 277);
			this.KeyPreview = true;
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(frmMain_Keypress);
			this.FormClosing += MainFormClosing;
			
			// Separator button
			tbSeparator.ImageIndex = 2;
		    tbSeparator.Style = ToolBarButtonStyle.Separator;
		    
		    // Exit button
		    tbExit.ImageIndex = 0;
		    tbExit.ToolTipText = "Ut av programmet (ESC)";
		    
		    // Logout button
		    tbLogout.ImageIndex = 1;
		    tbLogout.ToolTipText = "Logg ut (Ctrl+U)";
		    tbLogout.Enabled = isDbOk;
		    
		    
		    // Write button
		    tbWrite.ImageIndex = 2;
		    tbWrite.ToolTipText = "Skrive skripter (Ctrl+C)";
		    tbWrite.Enabled = isDbOk;
		    
		    // Write masterscript button
		    tbMasterscript.ImageIndex = 3;
		    tbMasterscript.ToolTipText = "Skrive masterskript (Ctrl+M)";
		    tbMasterscript.Enabled = isDbOk;
		    
		    // Run testcase
		    tbRunAll.ImageIndex = 4;
		    tbRunAll.ToolTipText = "Kjøre skript (Ctrl+A)";
		    tbRunAll.Enabled = false;
		    
		    // Run at step button
		    tbRunStep.ImageIndex = 5;
		    tbRunStep.ToolTipText = "Kjøre skript og stoppe på bestemte trinn (Ctrl+S)";
		    tbRunStep.Enabled = false;
		    
		    // Option button
		    tbOptions.ImageIndex = 6;
		    tbOptions.ToolTipText = "Sette opsjoner for dette programmet (Ctrl+O)";
		    
		    // Clear button
		    tbClear.ImageIndex = 7;
		    tbClear.ToolTipText = "Tømme mappen for logger (Ctrl+T)";
		    
		    // Backup database button
		    tbBackupDb.ImageIndex = 8;
		    tbBackupDb.ToolTipText = "Backup database (Ctrl+P)";
		    tbBackupDb.Enabled = isDbOk;
		    
		    // Commit/Push to GitHub
		    tbGithub.ImageIndex = 9;
		    tbGithub.ToolTipText = "Commit/Push til Github (Ctrl+G)";
		    tbGithub.Enabled = isDbOk;
		    
		    // Deploy File
		    tbDeployFile.ImageIndex = 10;
		    tbDeployFile.ToolTipText = "Deploy fil";
		    
		    // Users button
		    tbUsers.ImageIndex = 11;
		    tbUsers.ToolTipText = "Vise brukerliste (Ctrl+L)";
		    tbUsers.Enabled = isDbOk;
		    
		    // Users button
		    tbScripts.ImageIndex = 12;
		    tbScripts.ToolTipText = "Vise skript status (Ctrl+K)";
		    tbScripts.Enabled = isDbOk;
		    
		    // User manual button
		    tbUserManual.ImageIndex = 13;
		    tbUserManual.ToolTipText = "Åpne brukermanual (Ctrl+B)";
		    
		    // About button
		    tbAbout.ImageIndex = 14;
		    tbAbout.ToolTipText = "Om dette programmet (Ctrl+X)";
		    
		    // Web UI button
		    tbWebUI.ImageIndex = 15;
		    tbWebUI.ToolTipText = "Åpne Web UI (Ctrl+W)";
		    
		    // Jira button
		    tbJira.ImageIndex = 16;
		    tbJira.ToolTipText = "Åpne Jira (Ctrl+J)";
		    
		    // Jenkins button
		    tbJenkins.ImageIndex = 17;
		    tbJenkins.ToolTipText = "Åpne Jenkins (Ctrl+E)";
		    
		    toolBar.ImageList = toolBarIcons;
		    toolBar.Size = new System.Drawing.Size(272, 28);
		    toolBar.BorderStyle = System.Windows.Forms.BorderStyle.None;
		    
		    toolBar.ShowToolTips = true;
		    toolBar.Buttons.AddRange(new ToolBarButton[] {
		                             	tbExit,
										tbLogout,		                             	
		                             	tbSeparator, 
		                             	tbWrite, 
		                             	tbMasterscript, 
		                             	tbSeparator, 
		                             	tbRunAll, 
		                             	tbRunStep, 
		                             	tbSeparator,
		                             	tbOptions,
		                             	tbClear,
		                             	tbBackupDb,
		                             	tbGithub,
		                             	tbDeployFile,
		                             	tbSeparator,
		                             	tbUsers,
		                             	tbScripts,
		                             	tbSeparator,
		                             	tbUserManual,
		                             	tbAbout,
		                             	tbSeparator,
		                             	tbWebUI,
		                             	tbJira,
		                             	tbJenkins
		                             });
		    toolBar.ButtonClick += new ToolBarButtonClickEventHandler(ToolBar_Clicked);
		    
		    
		    toolBarIcons.ImageSize = new System.Drawing.Size(20, 20);
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("Exit"));
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("Logout"));
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("Write"));
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("Masterscript"));
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("Run-all"));
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("Run-Step"));
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("Options"));
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("Delete"));
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("Backup"));
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("Github"));
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("File"));
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("Users"));
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("Status"));
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("Manual"));
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("About"));
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("HMS"));
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("Jira"));
		    toolBarIcons.Images.Add((Icon)resMgr.GetObject("Jenkins"));
		    toolBarIcons.TransparentColor = System.Drawing.Color.Transparent;
						
		    this.Controls.Add(toolBar);  
			toolBar.Appearance = ToolBarAppearance.Flat;
			
			string infoMsg = "Info: Klar";
			lblInfoMsg.BackColor = Color.Empty;
			lblClock.BackColor = Color.Empty;
            if (!isDbOk) 
            { 
            	infoMsg = "Feil: Kan ikke koble til MySQL-database. Sjekk database opsjon."; 
            	startMenu = "options";
            }
            
            
            float rawLimit = float.Parse(GlobalFunction.GetConfigSettings("RawLimitSize"));
            DirectoryInfo dir = new DirectoryInfo(@"C:\Projects\SystemXAuto-Test\WrittenTests\Raw");
            float size = (float)(Math.Round((GlobalFunction.GetDirectorySize(dir, true) / (1024 * 1024)),1));
            
            if (size > rawLimit)
            {
            	infoMsg = "Advarsel: Lagringskatalog for Raw skripter er nå over grenseverdien"; 
            	startMenu = "clear_logs";
            	lblInfoMsg.BackColor = Color.Gold;
            	lblClock.BackColor = Color.Gold;
            }
                
			switch (startMenu)
			{
				case "about":
					ShowInfo();
					break;
				case "write_script":
					WriteScript();
					break;
				case "write_masterscript":
					WriteMasterscript();
					break;
				case "run_script":
					ExecuteScript();
					break;
				case "run_at_step":
					ExecuteScriptAtStep();
					break;
				case "options":
					ProgramOptions();
					break;
				case "clear_logs":
					ClearLogs();
					break;
			}
			this.lblInfoMsg.Text = infoMsg;
			CheckLogs();
		}
		
		private void ToolBar_Clicked(object sender, ToolBarButtonClickEventArgs e) 
		{
			int clickedButton = e.Button.ImageIndex;
			switch (clickedButton)
			{
				case 0:
					//MySQLConnector.SetUserStatus(lastLoginID, true);
					this.Close();
					break;
				case 1:
					Logout();
					break;
				case 2:
					WriteScript();
					break;
				case 3:
					WriteMasterscript();
					break;
				case 4:
					ExecuteScript();
					break;
				case 5:
					ExecuteScriptAtStep();
					break;
				case 6:
					ProgramOptions();
					break;
				case 7:
					ClearLogs();
					break;
				case 8:
					BackupDatabase();
					break;
				case 9:
					CommitPush();
					break;
				case 10:
					DeployFile();
					break;
				case 11:
					ShowUsers();
					break;
				case 12:
					ShowScriptStatus();
					break;
				case 13:
					lblInfoMsg.Text = "Info: Klar";
					CheckLogs();
					//Host.Local.RunApplication("Help\\Help.htm");
					Process.Start(@"http://utv-sxtest1/autox-web-ui/?id=15");
					break;
				case 14:
					lblInfoMsg.Text = "Info: Klar";
					ShowInfo();
					break;
				case 15:
					lblInfoMsg.Text = "Info: Klar";
					CheckLogs();
					Process.Start(@"http://utv-sxtest1/autox-web-ui");
					break;
				case 16:
					lblInfoMsg.Text = "Info: Klar";
					CheckLogs();
					Process.Start(@"http://jiracon-appsrv:8080/secure/MyJiraHome.jspa");
					break;
				case 17:
					lblInfoMsg.Text = "Info: Klar";
					CheckLogs();
					Process.Start(@"http://cistaging:8080/view/Ranorex/");
					break;
			}
		}
		
		private void CheckLogs()
		{
			int reportCount = Directory.GetFiles(@"Reports\", "*", SearchOption.AllDirectories).Length - 3 ;
			int logCount = Directory.GetFiles(@"Logs\", "*", SearchOption.AllDirectories).Length;
			this.lblInfoMsg.BackColor = System.Drawing.Color.Empty;
			this.lblClock.BackColor = System.Drawing.Color.Empty;
			int logLimit = Int32.Parse(GlobalFunction.GetConfigSettings("LimitSize"));
			string enableErrorLog = GlobalFunction.GetConfigSettings("EnableErrorLog");
			
			float rawLimit = float.Parse(GlobalFunction.GetConfigSettings("RawLimitSize"));
            DirectoryInfo dir = new DirectoryInfo(@"C:\Projects\SystemXAuto-Test\WrittenTests\Raw");
            float size = (float)(Math.Round((GlobalFunction.GetDirectorySize(dir, true) / (1024 * 1024)),1));
            
			if (enableErrorLog == "true")
			{
				if (reportCount >= logLimit || logCount >= logLimit)
				{
					this.lblInfoMsg.Text = "Advarsel: Loggfiler og / eller rapportere filer er over grensen!";
					lblInfoMsg.BackColor = Color.Gold;
            		lblClock.BackColor = Color.Gold;
				}
			}
			
            if (size > rawLimit)
            {
            	this.lblInfoMsg.Text = "Advarsel: Lagringskatalog for RX prosjekter er nå over grenseverdien"; 
            	lblInfoMsg.BackColor = Color.Gold;
            	lblClock.BackColor = Color.Gold;
            }
		}
		
		private void btnAction_Click(object sender, System.EventArgs e)
		{
			this.btnAction.Enabled = false;
			if (txtTestsuite.Text == "")
			{
				ReportError("Ingen testsuite ID angitt!");
				txtTestsuite.Focus();
				this.btnAction.Enabled = true;
			}
			else
			{
				DoAction();
			}
			CheckLogs();
		}
		
		private async void DoAction()
		{
			bool response = true;
			//string dbSettingsFile = "SystemXAuto-Test.exe.config";
			string[] rangeList = new string[] {};
			string executionTime = GlobalFunction.ExecutionTime();
            string logFilename = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AppEvent.log");
            string testSuiteId = txtTestsuite.Text;
			//string oldID = "value=\"" + lastUsedID + "\"";
			string newID = "value=\"" + testSuiteId + "\"";
			string logSource = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LastUsedID.log";
			//File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(oldID, newID));
			File.WriteAllText(logSource, testSuiteId);
			if (!MySQLConnector.IsTestsuiteExist(testSuiteId))
			{
				ReportError("Testsuite eksisterer ikke");
				txtTestsuite.Focus();
			}
			else
			{
				string tsTitle = MySQLConnector.GetTestSuiteInfo(testSuiteId,2);
				string masterScriptFolder = "TS_" + testSuiteId + "_" + tsTitle.Replace(' ', '_').Replace('-', '_');
				List<string> validList = new List<string>();
				bool tcExistFlag = true;
				string scriptFolder = (GlobalFunction.GetConfigSettings("ScriptsDir")).Replace("%DBName%", "v" + sxDBVersion); //GlobalFunction.GetConfigSettings("ScriptsDir");
				string selectedMode = this.lblTemp.Text;
				int stepNumber = 0;
				
	            if ((selectedMode == "execute all") || (selectedMode == "execute step"))
				{
					string strEntry = txtRangeScript.Text;
					
					// Check if the entered testcase IDs are valid
					if (!IsEntryValid(strEntry))
					{
						response = false;
						ReportError("Ugyldig dataregistrering");
					}
					else
					{
						List<string> entryList = GlobalFunction.GetEntryList(strEntry);
			            foreach (string list in entryList)
			            {
			            	if (!MySQLConnector.IsTestCaseExist(testSuiteId, list))
							{
								tcExistFlag = false;
								response = false;
								ReportError("En eller flere testcaser er ikke funnet eller ikke inneholder testtrinn");
							}
			            	else
			            	{
			            		validList.Add(list);
			            	}
			            }
					}
					
					if (response)
					{
						lblInfoMsg.Text = "Info: Kjører skript... Vennligst vent!";
						List<string> reportLogs = new List<string>();
						string htmlReport = "";
						if (selectedMode == "execute step")
						{
							stepNumber = Int16.Parse(txtSingleScript.Text);
						}
						foreach (string list in validList)
			            {
							string tcTitle = MySQLConnector.GetTestCaseInfo(testSuiteId, list, 3);
							/*GlobalFunction.WriteToLog(logFilename, "Valgt modus: Kjøre modus", false);
							GlobalFunction.WriteToLog(logFilename, "Testsuite: TS-" + testSuiteId + ": " + tsTitle, false);
							GlobalFunction.WriteToLog(logFilename, "Testcase: TC-" + list + ": " + tcTitle, false);*/
							htmlReport = ExecuteFunction.DoExecuteMode(testSuiteId, list, selectedMode, stepNumber);
							
							/*GlobalFunction.WriteToLog(logFilename, "", false);
							GlobalFunction.WriteToLog(logFilename, "Kjøre skript ..... Ferdig");*/
							reportLogs.Add(htmlReport);
						}
						string programLog = GlobalFunction.GetConfigSettings("EnableErrorLog");
						if (programLog == "true")
						{
							System.Windows.Forms.DialogResult confirm = GlobalFunction.ConfirmMessage("Kjøre skript ..... Ferdig\n\nØnsker du å vise rapportene?");
							if (confirm == System.Windows.Forms.DialogResult.Yes)
							{
								foreach (string reportLog in reportLogs)
								{
									Host.Local.RunApplication(reportLog);
								}
							}
						}
						else
						{
							GlobalFunction.ShowMessage("Kjøre skript ..... Ferdig", MessageBoxIcon.Information);
						}
					}
				}
				
				// Write Multiple Scripts
				else if (this.lblTemp.Text == "write_multiple")
				{
					string strEntry = txtRangeScript.Text;
					
					lblInfoMsg.Text = "Info: Skriver og kompilerer skripter... Vennligst vent!";
					/*Delay.Seconds(1);*/
					
					if (!IsEntryValid(txtRangeScript.Text))
					{
						response = false;
						ReportError("Ugyldig dataregistrering (" + strEntry + ")");
					}
					else
					{
						List<string> entryList = GlobalFunction.GetEntryList(strEntry);
			            foreach (string list in entryList)
			            {
			            	if ((!MySQLConnector.IsTestCaseExist(testSuiteId, list)) && (MySQLConnector.IsTestCaseActive(testSuiteId, list)))
							{
								string[] tcData = new string[] {"Error", "Testcase does not exist or does not contain test steps", GlobalFunction.ExecutionTime()};
								GlobalFunction.WriteToLog(logFilename, tcData);
								tcExistFlag = false;
							}
			            	else
			            	{
			            		string[] tcData = new string[] {"Information", "Checking if TC-" + list + " exists in database... Done", GlobalFunction.ExecutionTime()};
			            		GlobalFunction.WriteToLog(logFilename, tcData);
			            		validList.Add(list);
			            	}
			            }
						
					}
					if (!tcExistFlag)
					{
						ReportError("Det er testcaser som ikke klart å skrive.. De eksisterer ikke eller inneholder ikke steg.");
						response = false;
					}
					if (response)
					{
						string openLog = GlobalFunction.GetConfigSettings("OpenLogAtWrite");
						if (openLog == "true")
						{
							Process.Start(logFilename);
						}
						bool buildSuccess = true;
						int tcCounter = 1;
						int tcCount = 0;
						foreach (string list2 in validList)
						{
							if (this.chkWriteActiveOnly.Checked)
							{
								if (MySQLConnector.IsTestCaseActive(testSuiteId, list2))
								{
									tcCount++;
								}
							}
							else
							{
								tcCount++;
							}
						}
						
						foreach (string list in validList)
			            {
							bool onlyActive = (MySQLConnector.IsTestCaseActive(testSuiteId, list));
							if (!this.chkWriteActiveOnly.Checked)
							{
								onlyActive = true;
							}
							if (onlyActive)
							{
								WriteFunction.InitializeScriptTemplate(masterScriptFolder, "TC-" + list + ": " + MySQLConnector.GetTestCaseInfo(testSuiteId, list, 3));
								if (!WriteFunction.DoWriteScript(testSuiteId, list, logFilename))
								{
									buildSuccess = false;
									break;
								}
								string tcTitle = MySQLConnector.GetTestCaseInfo(testSuiteId, list, 3);
								if (buildSuccess)
								{
									MySQLConnector.SetScriptWriteStatus(testSuiteId, list);
									lblInfoMsg.Text = "Info: Skriver TC-" + list + ": " + tcTitle + "... (" + tcCounter.ToString() + " av " + tcCount.ToString() + ")";
									await System.Threading.Tasks.Task.Delay(500);
									tcCounter++;
								}
							}
						}
						
						if (buildSuccess)
						{
							lblInfoMsg.Text = "Info: Ferdig...";
							GlobalFunction.ShowMessage("Skrive skripter...Ferdig\n\n EXE skripter kan finnes på " + scriptFolder + " mappen", MessageBoxIcon.Information);
						}
					}
				}
				else
				{
					if (GlobalFunction.CheckScriptsExist(testSuiteId))
					{
						WriteFunction.InitializeMasterScriptTemplate(masterScriptFolder);
						if (WriteFunction.DoWriteMasterScript(testSuiteId, logFilename))
						{
							GlobalFunction.ShowMessage("Skrive skript...Ferdig\n\n Det eksekverbar master skript kan finnes på " + scriptFolder + " mappen", MessageBoxIcon.Information);
							MySQLConnector.SetMasterScriptWriteStatus(testSuiteId);
						}
					}
					else
					{
						GlobalFunction.ShowMessage("FEIL: Klarte ikke å skrive skripter! Mulige årsaker:\n\n- Ingen skriptene er skrevet ennå\n- Ingen definerte testcaser på denne testsuite\n- Ingen testcaser lagt til master skript", MessageBoxIcon.Error);
					}
				}
			}
			this.btnAction.Enabled = true;
			lblInfoMsg.Text = "Info: Klar";
		}
		
		private void mnuExecute_Click(object sender, System.EventArgs e)
		{
			ExecuteScript();
		}
		private void ExecuteScript()
		{
			lblInfoMsg.Text = "Info: Klar";
			lblInfoMsg.ForeColor = Color.Empty;
			lblInfoMsg.ForeColor = Color.Empty;
			this.groupBox1.Controls.Clear();
			this.lblTemp.Text = "execute all";
			this.groupBox1.Visible = true;
			//this.groupBox1.Controls.Add(this.cboMasterScript);
			this.groupBox1.Controls.Add(this.txtTestsuite);
			this.btnAction.Text = "&Kjøre";
			//this.lblInfoMsg.Text = "Info: Kjøre modusen velges";
			this.groupBox1.Text = " Kjøre skript - Alle trinn ";
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.txtRangeScript);
			this.groupBox1.Controls.Add(this.lblSelTestCase);
			this.groupBox1.Controls.Add(this.lblHelp);
			this.groupBox1.Controls.Add(this.btnAction);
			//this.lblSelTestCase.Text = "Select Test &Case";
			this.lblSelTestCase.Text = "Skriv inn testcase ID";
			this.txtRangeScript.KeyPress+= new System.Windows.Forms.KeyPressEventHandler(InputValueControl);
			this.txtTestsuite.KeyPress+= new System.Windows.Forms.KeyPressEventHandler(InputValueControl);
			txtRangeScript.KeyDown += new System.Windows.Forms.KeyEventHandler(InputValueResize);
			this.txtRangeScript.Size = new System.Drawing.Size(230, 20);
			this.txtTestsuite.Focus();
			CheckLogs();
		}
		
		private void mnuExecuteStep_Click(object sender, System.EventArgs e)
		{
			ExecuteScriptAtStep();
		}
		
		private void ExecuteScriptAtStep()
		{
			lblInfoMsg.Text = "Info: Klar";
			lblInfoMsg.ForeColor = Color.Empty;
			this.groupBox1.Controls.Clear();
			this.lblTemp.Text = "execute step";
			this.groupBox1.Visible = true;
			//this.groupBox1.Controls.Add(this.cboMasterScript);
			this.groupBox1.Controls.Add(this.txtTestsuite);
			this.btnAction.Text = "&Kjøre";
			//this.lblInfoMsg.Text = "Info: Kjøre modusen velges";
			this.groupBox1.Text = " Kjøre skript - Stoppe på trinn ";
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.txtRangeScript);
			this.groupBox1.Controls.Add(this.txtSingleScript);
			this.groupBox1.Controls.Add(this.lblStepNumber);
			this.groupBox1.Controls.Add(this.lblSelTestCase);
			this.groupBox1.Controls.Add(this.btnAction);
			this.lblSelTestCase.Text = "Testcase ID";
			this.txtRangeScript.KeyPress+= new System.Windows.Forms.KeyPressEventHandler(txtStep_Keypress);
			this.txtRangeScript.KeyPress+= new System.Windows.Forms.KeyPressEventHandler(InputValueControl);
			this.txtTestsuite.KeyPress+= new System.Windows.Forms.KeyPressEventHandler(InputValueControl);
			this.txtRangeScript.Size = new System.Drawing.Size(110, 20);
			//this.cboMasterScript.SelectedIndex = 0;
			this.btnAction.Enabled = true;
			this.txtSingleScript.KeyPress+= new System.Windows.Forms.KeyPressEventHandler(txtStep_Keypress);
			this.txtSingleScript.KeyPress+= new System.Windows.Forms.KeyPressEventHandler(InputValueControl);
			txtRangeScript.KeyDown += new System.Windows.Forms.KeyEventHandler(InputValueResize);
			this.txtTestsuite.Focus();
			CheckLogs();
		}
		
		private void mnuWriteSingle_Click(object sender, System.EventArgs e)
		{
			this.lblInfoMsg.Enabled = false;
			this.lblInfoMsg.ForeColor = System.Drawing.Color.Black;
			this.groupBox1.Controls.Clear();
			this.lblTemp.Text = "write_single";
			this.groupBox1.Visible = true;
			//this.groupBox1.Controls.Add(this.cboMasterScript);
			this.groupBox1.Controls.Add(this.txtTestsuite);
			this.btnAction.Text = "&Skriv";
			//this.lblInfoMsg.Text = "Info: Skrivemodusen (Enkelt skript) velges";
			this.groupBox1.Text = " Skriv enkelt skript ";
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.cboTestCase);
			this.groupBox1.Controls.Add(this.lblSelTestCase);
			this.groupBox1.Controls.Add(this.btnAction);
			this.lblSelTestCase.BackColor = System.Drawing.Color.Transparent;
			this.lblSelTestCase.Text = "Velg en testcase";
			//this.cboMasterScript.SelectedIndex = 0;
			CheckLogs();
		}
		private void mnuWriteMultiple_Click(object sender, System.EventArgs e)
		{
			WriteScript();
		}
		private void WriteScript()
		{
			lblInfoMsg.Text = "Info: Klar";
			lblInfoMsg.ForeColor = Color.Empty;
			lblInfoMsg.BackColor = Color.Empty;
			lblClock.BackColor = Color.Empty;
			this.groupBox1.Controls.Clear();
			this.lblTemp.Text = "write_multiple";
			this.groupBox1.Visible = true;
			this.btnAction.Text = "&Skriv";
			this.groupBox1.Text = " Skriv flere skripter ";
			this.groupBox1.Controls.Add(this.btnAction);
			this.groupBox1.Controls.Add(this.txtTestsuite);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.txtRangeScript);
			this.groupBox1.Controls.Add(this.lblSelTestCase);
			this.groupBox1.Controls.Add(this.lblHelp);
			this.groupBox1.Controls.Add(this.chkWriteAll);
			this.groupBox1.Controls.Add(this.chkWriteActiveOnly);
			
			//Testsuite ID
			this.label1.Location = new System.Drawing.Point(10, 25);
			this.label1.AutoSize = true;
			this.label1.Text = "TS-ID:";
			this.txtTestsuite.Location = new System.Drawing.Point(50, 20);
			this.txtTestsuite.Size = new System.Drawing.Size(40, 20);
			this.txtTestsuite.TextAlign = HorizontalAlignment.Center;
			this.txtTestsuite.Text = GlobalFunction.GetLastUsedTSID();
			this.txtTestsuite.KeyUp-= TestsuiteNavigator;
			this.txtTestsuite.KeyUp+= new System.Windows.Forms.KeyEventHandler(TestsuiteNavigator);
			
			//Testcase ID
			this.lblSelTestCase.Text = "TC-ID:";
			this.lblSelTestCase.Location = new System.Drawing.Point(10, 50);
			this.lblSelTestCase.AutoSize = true;
			this.txtRangeScript.Location = new System.Drawing.Point(50, 45);
			this.txtRangeScript.Size = new System.Drawing.Size(205, 20);
			
			//Example
			this.lblHelp.Location = new System.Drawing.Point(50, 65);
			this.lblHelp.AutoSize = true;
			
			//Write all checkbox
			this.chkWriteAll.Location = new System.Drawing.Point(15, 85);
			chkWriteAll.AutoSize = true;
			chkWriteAll.Checked = false;
			chkWriteAll.Text = "Skrive alle testcaser";
			chkWriteAll.CheckedChanged += checkBox_WriteAllChanged;
			
			//Write only active testcases checkbox
			this.chkWriteActiveOnly.Location = new System.Drawing.Point(15, 110);
			chkWriteActiveOnly.AutoSize = true;
			chkWriteActiveOnly.Text = "Skrive kun aktive testcaser";
			chkWriteActiveOnly.Checked = true;
			chkWriteActiveOnly.CheckedChanged += checkBox_WriteActiveOnly;
			
			txtRangeScript.ReadOnly = false;
			this.txtRangeScript.Text = "";
			
			this.txtRangeScript.KeyPress+= new System.Windows.Forms.KeyPressEventHandler(InputValueControl);
			this.txtTestsuite.KeyPress+= new System.Windows.Forms.KeyPressEventHandler(InputValueControl);
			txtRangeScript.KeyDown += new System.Windows.Forms.KeyEventHandler(InputValueResize);
			this.txtTestsuite.Focus();
			CheckLogs();
		}
		
		private void checkBox_WriteAllChanged(object sender, EventArgs e)
	    {
			this.chkWriteAll.Text = "Skrive alle testcaser";
			txtRangeScript.ReadOnly = false;
			this.txtRangeScript.Text = "";
			if (this.txtTestsuite.Text == "" && chkWriteAll.Checked)
			{
				ReportError("Testsuite ID-feltet må fylles ut!");
				chkWriteAll.Checked = false;
				this.txtTestsuite.Focus();
			}
			else if (this.txtTestsuite.Text != "" && chkWriteAll.Checked)
			{
				if (MySQLConnector.IsTestsuiteExist(txtTestsuite.Text))
				{
					this.chkWriteActiveOnly.Text = "Skrive kun aktive testcaser (" + MySQLConnector.GetTestcasesCount(this.txtTestsuite.Text, this.chkWriteActiveOnly.Checked) + " testcaser)";
					if (this.chkWriteAll.Checked)
					{
						this.txtRangeScript.Text = "100-" + MySQLConnector.GetLatestTestcaseID(this.txtTestsuite.Text);
						txtRangeScript.ReadOnly = true;
						this.chkWriteAll.Text = "Skrive alle testcaser (" + MySQLConnector.GetTestcasesCount(this.txtTestsuite.Text, this.chkWriteActiveOnly.Checked) + " testcaser)";
						this.chkWriteActiveOnly.Text = "Skrive kun aktive testcaser (" + MySQLConnector.GetTestcasesCount(this.txtTestsuite.Text, this.chkWriteActiveOnly.Checked) + " testcaser)";
					}
				}
				else
				{
					ReportError("Testsuite ID (" + txtTestsuite.Text + ") eksisterer ikke!");
					txtTestsuite.Text = "";
					txtTestsuite.Focus();
				}
			}
	    }
		
		private void checkBox_WriteActiveOnly(object sender, EventArgs e)
	    {
			if (this.txtTestsuite.Text == "")
			{
				ReportError("Testsuite ID-feltet må fylles ut!");
				this.txtTestsuite.Focus();
			}
			else
			{
				if (MySQLConnector.IsTestsuiteExist(txtTestsuite.Text))
				{
					this.chkWriteActiveOnly.Text = "Skrive kun aktive testcaser (" + MySQLConnector.GetTestcasesCount(this.txtTestsuite.Text, this.chkWriteActiveOnly.Checked) + " testcaser)";
					this.chkWriteAll.Text = "Skrive alle testcaser";
					if (chkWriteAll.Checked)
					{
						this.chkWriteAll.Text = "Skrive alle testcaser (" + MySQLConnector.GetTestcasesCount(this.txtTestsuite.Text, this.chkWriteActiveOnly.Checked) + " testcaser)";
					}
				}
				else
				{
					ReportError("Testsuite ID (" + txtTestsuite.Text + ") eksisterer ikke!");
					txtTestsuite.Text = "";
					txtTestsuite.Focus();
				}
			}
	    }
		
		private void InputValueResize(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			TextBox input = sender as TextBox;
			if ((input.Text == "") && ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down)))
			{
				input.Text = "99";
			}
			if (e.KeyCode == Keys.Up)
			{
				input.Text = (Int32.Parse(input.Text) + 1).ToString();
			}
			if ((e.KeyCode == Keys.Down) && (Int32.Parse(input.Text) > 100))
			{
				input.Text = (Int32.Parse(input.Text) - 1).ToString();
			}
			if ((e.KeyCode == Keys.Down) && (Int32.Parse(input.Text) == 99))
			{
				input.Text = (Int32.Parse(input.Text) + 1).ToString();
			}
		}
		private void mnuWriteMaster_Click(object sender, System.EventArgs e)
		{
			WriteMasterscript();
		}
		
		private void WriteMasterscript()
		{
			lblInfoMsg.Text = "Info: Klar";
			lblInfoMsg.ForeColor = Color.Empty;
			this.groupBox1.Controls.Clear();
			this.lblTemp.Text = "write_master";
			this.groupBox1.Visible = true;
			this.btnAction.Text = "&Skriv";
			//this.lblInfoMsg.Text = "Info: Skrive ";
			this.groupBox1.Text = " Skriv master skript ";
			this.groupBox1.Controls.Add(this.btnAction);
			//this.groupBox1.Controls.Add(this.cboMasterScript);
			this.groupBox1.Controls.Add(this.txtTestsuite);
			this.groupBox1.Controls.Add(this.label1);
			this.txtTestsuite.Text = GlobalFunction.GetLastUsedTSID();
			this.txtTestsuite.KeyPress+= new System.Windows.Forms.KeyPressEventHandler(InputValueControl);
			this.txtTestsuite.Focus();
			CheckLogs();
		}
		
		private void mnuExit_Click(object sender, System.EventArgs e)
		{
			//MySQLConnector.SetUserStatus(lastLoginID, true);
			this.Close();
		}
		
		private void mnuLogout_Click(object sender, System.EventArgs e)
		{
			Logout();
		}
		
		private void Logout()
		{
			MySQLConnector.SetUserStatus(lastLoginID, true);
			System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(OpenLoginForm));
	        t.Start();
	        this.Close(); 
		}
		
		private void OpenLoginForm()
		{
			SystemXTest.WinComponent Component = new SystemXTest.WinComponent();
        	Component.InitializeComponent();
		}
		
		private void mnuHelp_Click(object sender, System.EventArgs e)
		{
			lblInfoMsg.Text = "Info: Klar";
			CheckLogs();
			Process.Start(@"http://utv-sxtest1/autox-web-ui/?id=15");
		}
		
		private void mnuAbout_Click(object sender, System.EventArgs e)
		{
			ShowInfo();
		}
		
		private void ShowInfo()
		{
			this.groupBox1.Controls.Clear();
			this.groupBox1.Text = " Om Auto X Tool ";
			this.groupBox1.Visible = true;
			this.lblAbout.Text = "Auto X Tool (v" + GetAppInfo("Version") + ")\r\n\nDette verktøyet er ment å skape Ranorex skript for bruk i automatisering testing av System X-program\r\n\nFor spørsmål, forslag eller kommentarer, skal sendes til:";
			this.groupBox1.Controls.Add(this.lnkMail);
			this.groupBox1.Controls.Add(this.lblAbout);
			lblInfoMsg.Text = "Info: Klar";
			CheckLogs();
		}
		
		private void mnuUI_Click(object sender, System.EventArgs e)
		{
			lblInfoMsg.Text = "Info: Klar";
			CheckLogs();
			Process.Start(@"http://utv-sxtest1/autox-web-ui");
		}
		
		private void mnuJira_Click(object sender, System.EventArgs e)
		{
			lblInfoMsg.Text = "Info: Klar";
			CheckLogs();
			Process.Start(@"http://jiracon-appsrv:8080/secure/MyJiraHome.jspa");
		}
		
		private void mnuJenkins_Click(object sender, System.EventArgs e)
		{
			lblInfoMsg.Text = "Info: Klar";
			CheckLogs();
			Process.Start(@"http://cistaging:8080/view/Ranorex/");
		}
		
		private void mnuGithub_Click(object sender, System.EventArgs e)
		{
			CommitPush();
		}
		
		private void mnuDeployFile_Click(object sender, System.EventArgs e)
		{
			DeployFile();
		}
		
		public void mnuCleaner_Click(object sender, System.EventArgs e)
		{
			ClearLogs();
		}
		
		public void ClearLogs()
		{
			lblInfoMsg.Text = "Info: Klar";
			lblInfoMsg.ForeColor = Color.Empty;
			string strLimitSize = GlobalFunction.GetConfigSettings("LimitSize");
			int limitSize = Int32.Parse(strLimitSize);
			this.groupBox1.Controls.Clear();
			this.btnUpdate.Text = "&Slett";
			this.btnUpdate.Click -= new EventHandler(btnClear_Click);
			this.btnUpdate.Click += new EventHandler(btnClear_Click);
			this.btnUpdate.Click -= new EventHandler(btnUpdate_Click);
			this.groupBox1.Text = " Slette logger/RX prosjekter";
			this.groupBox1.Visible = true;
			int reportCount = Directory.GetFiles(@"Reports\", "*", SearchOption.AllDirectories).Length - 3 ;
			int logCount = Directory.GetFiles(@"Logs\", "*_2*", SearchOption.AllDirectories).Length + Directory.GetFiles(@"Logs\", "*.log", SearchOption.AllDirectories).Length;
			int rxCount = Directory.GetDirectories(@"WrittenTests\Raw\").Length;
			this.chkLogs.ForeColor = System.Drawing.Color.Black;
			this.chkReports.ForeColor = System.Drawing.Color.Black;
			this.btnUpdate.Enabled = true;
			chkRawScripts.Enabled = true;
			chkReports.Enabled = true;
			chkLogs.Enabled = true;
			chkRawScripts.Checked = true;
			chkReports.Checked = true;
			chkLogs.Checked = true;
			if (reportCount >= limitSize)
			{
				this.chkReports.ForeColor = System.Drawing.Color.Red;
			}
			if (logCount >= limitSize)
			{
				this.chkLogs.ForeColor = System.Drawing.Color.Red;
			}
			if (reportCount == 0 && logCount == 0 && rxCount == 0)
			{
				this.btnUpdate.Enabled = false;
			}
			if (rxCount == 0)
			{
				chkRawScripts.Enabled = false;
				chkRawScripts.Checked = false;
			}
			if (reportCount == 0)
			{
				chkReports.Enabled = false;
				chkReports.Checked = false;
			}
			if (logCount == 0)
			{
				chkLogs.Enabled = false;
				chkLogs.Checked = false;
			}
			this.lblAbout.Text = "Her kan du tømme ut loggene, slette alle testrapporter og RX prosjekter.";
			this.chkReports.Text = "Testrapporter (" + reportCount.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("nb-NO")) + " fil(er) funnet)";
			this.chkLogs.Text = "Program logger (" + logCount.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("nb-NO")) + " fil(er) funnet)";
			this.chkRawScripts.Text = "RX-prosjekter (" + rxCount.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("nb-NO")) + " mappe(r) funnet)";
			this.groupBox1.Controls.Add(this.chkRawScripts);
			this.groupBox1.Controls.Add(this.chkReports);
			this.groupBox1.Controls.Add(this.chkLogs);
			this.groupBox1.Controls.Add(this.btnUpdate);
			this.groupBox1.Controls.Add(this.lblAbout);
			this.chkLogs.Location = new System.Drawing.Point(15, 65);
			CheckLogs();
		}
			
		public void mnuDbSettings_Click(object sender, System.EventArgs e)
		{
			this.lblInfoMsg.Enabled = false;
			this.lblInfoMsg.ForeColor = System.Drawing.Color.Black;
			this.btnUpdate.Enabled = true;
			this.groupBox1.Controls.Clear();
			this.pb1.Location = new System.Drawing.Point(285, 10);
			this.lblAddress.Location = new System.Drawing.Point(270, 150);
			this.lblVersion.Location = new System.Drawing.Point(500, 125);
			this.groupBox1.Text = " Database innstillinger ";
			this.btnUpdate.Text = "&Oppdater";
			this.btnUpdate.Click -= new EventHandler(btnUpdate_Click);
			this.btnUpdate.Click += new EventHandler(btnUpdate_Click);
			this.btnUpdate.Click -= new EventHandler(btnClear_Click);
			this.groupBox1.Visible = true;
			this.groupBox1.Controls.Add(this.txtServerName);
			this.groupBox1.Controls.Add(this.lblDbServer);
			this.groupBox1.Controls.Add(this.txtDbName);
			
			this.groupBox1.Controls.Add(this.lblDbName);
			this.groupBox1.Controls.Add(this.txtDbUser);
			this.groupBox1.Controls.Add(this.lblDbUser);
			this.groupBox1.Controls.Add(this.txtDbPass);
			this.groupBox1.Controls.Add(this.lblDbPass);
			this.groupBox1.Controls.Add(this.btnUpdate);
			this.groupBox1.Controls.Add(this.treeView1);
			if (GlobalFunction.GetConfigSettings("DBServer") == "")
			{ this.Close();}
			this.txtServerName.Text = GlobalFunction.GetConfigSettings("DBServer");
			//this.txtDbName.Text = GlobalFunction.GetConfigSettings("DBName");
			this.txtDbName.Text = "sxtest_" + sxDBVersion.Replace(".", String.Empty);
			this.txtDbUser.Text = GlobalFunction.GetConfigSettings("DBUsername");
			this.txtDbPass.Text = GlobalFunction.Decrypt(GlobalFunction.GetConfigSettings("DBPassword"),true);
			CheckLogs();
		}
			
		public void mnuOptions_Click(object sender, System.EventArgs e)
		{
			ProgramOptions();
		}
		
		public void ProgramOptions()
		{
			this.groupBox1.Controls.Clear();
			this.groupBox1.Text = " Opsjoner ";
			this.groupBox1.Visible = true;
			this.groupBox1.Controls.Add(this.treeView1);
			groupBox1.Controls.Add(lblOption);
			lblInfoMsg.Text = "Info: Klar";
			lblInfoMsg.ForeColor = Color.Empty;
			gbOptions.Controls.Clear();
			lblOption.BackColor = System.Drawing.Color.LightGray;
			lblOption.Text = "Database";
			groupBox1.Controls.Add(gbOptions);
            gbOptions.Visible = true;
            gbOptions.Controls.Add(this.btnSave);
            gbOptions.Controls.Add(txtServerName);
    		gbOptions.Controls.Add(lblDbServer);
    		gbOptions.Controls.Add(txtDbName);
    		gbOptions.Controls.Add(lblDbName);
    		gbOptions.Controls.Add(txtDbUser);
    		gbOptions.Controls.Add(lblDbUser);
    		gbOptions.Controls.Add(txtDbPass);
    		gbOptions.Controls.Add(lblDbPass);
    		this.txtServerName.Text = GlobalFunction.GetConfigSettings("DBServer");
    		
    		if ((sxDBVersion == null) || (sxDBVersion == ""))
    		{
    			this.txtDbName.Text = GlobalFunction.GetConfigSettings("DBName");
    		}
			else
			{
				this.txtDbName.Text = "sxtest_" + sxDBVersion.Replace(".", String.Empty);
			}
			this.txtDbUser.Text = GlobalFunction.GetConfigSettings("DBUsername");
			this.txtDbPass.Text = GlobalFunction.Decrypt(GlobalFunction.GetConfigSettings("DBPassword"),true);
			CheckLogs();
		}
		
		private void cboMasterScript_Change(object sender, System.EventArgs e)
		{
			string testSuiteId = cboMasterScript.SelectedValue.ToString();
			string info = "Info: " + this.cboMasterScript.Text + " er valgt...";
			if (testSuiteId == "")
			{
				testSuiteId = "0";
				info = "Info: Vennligst velg en testsuite";
			}
			if (this.lblTemp.Text != "write_master")
			{
				string strQuery = "SELECT * FROM testcases WHERE ts_id=" + testSuiteId + " ORDER BY tc_id ASC";
				int searchCount = MySQLConnector.RunQuery(strQuery).Rows.Count;
				this.cboTestCase.Items.Clear();
				this.cboTestCase.Items.Add(String.Empty);
				for (int x=0; x<searchCount; x++)
				{
					string title = MySQLConnector.RunQuery(strQuery).Rows[x][3].ToString();
					string tcNumber = MySQLConnector.RunQuery(strQuery).Rows[x][1].ToString();
					this.cboTestCase.Items.Add("TC-" + tcNumber + ": " + title);
				}
				this.cboTestCase.SelectedValueChanged+= new EventHandler(cboTestCase_Change);
			}
			else
			{
				if (this.cboMasterScript.Text != "")
				{
					this.btnAction.Enabled = true;
				}
			}
		}
		
		private void cboTestCase_Change(object sender, System.EventArgs e)
		{
			//this.lblInfoMsg.Text = "Info: Vennligst velg en testcase";
			try
			{
				string testSuiteId = cboMasterScript.SelectedValue.ToString();
				string testCaseNum = GlobalFunction.ExtractText(this.cboTestCase.SelectedItem.ToString());
				int stepsCount = MySQLConnector.GetNumberOfSteps(testSuiteId, testCaseNum);
				
				if (stepsCount > 1)
				{
					this.btnAction.Enabled = true;
					//this.lblInfoMsg.Text = "Info: " + this.cboTestCase.Text + " er valgt...";
				}
				else
				{
					//this.lblInfoMsg.Text = "Info: Ingen definerte test trinn";
				}
			}
			catch (Exception)
			{
				//Do nothing
			}
		}
		
		private void TestsuiteNavigator(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (txtTestsuite.Text != String.Empty)
			{
				int tsID = Int32.Parse(txtTestsuite.Text);
				if (e.KeyCode == Keys.Down)
				{
					if (tsID > 1000)
					{
						tsID = tsID - 1;
						txtTestsuite.Text = tsID.ToString();
					}
				}
				else if (e.KeyCode == Keys.Up)
				{
					if (tsID < 9999)
					{
						tsID = tsID + 1;
						txtTestsuite.Text = tsID.ToString();
					}
				}
			}
			e.Handled = true;
		}
		
		private void InputValueControl(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			TextBox input = sender as TextBox;
			
			if (!char.IsDigit(e.KeyChar))
			{		
				e.Handled = true;
				if ((e.KeyChar == '-') || (e.KeyChar == ',') || (e.KeyChar == (char)8))
				{
					e.Handled = false;
				}	
			}
			if (e.KeyChar == (char)Keys.Escape)
			{
				string logFilename = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AppEvent.log");
				this.Close();
			}
		}
		
		public static void txtStep_Keypress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (!char.IsDigit(e.KeyChar))
			{		
				e.Handled = true;
				if (e.KeyChar == (char)8)
				{
					e.Handled = false;
				}	
			}
		}
		
		private bool IsEntryValid(string strEntry)
		{
			bool boolResult = true;
			string scriptMode = this.lblTemp.Text;
			string strInfo = lblInfoMsg.Text;
			string strTestsuite = txtTestsuite.Text;
			if ((scriptMode == "execute all") || (scriptMode == "write_multiple"))
			{
				string[] entryList = new string[] {};
				if (strEntry.Contains(","))
				{
					entryList = strEntry.Split(',');
					foreach (string entryMulti in entryList)
					{
						if (entryMulti == String.Empty)
						{
							boolResult =  false;
							break;
						}
						if (entryMulti.Contains("-"))
						{
							entryList = entryMulti.Split('-');
							if (entryList.Length == 2)
							{
								foreach (string entryRange in entryList)
								{
									if (entryRange == String.Empty)
									{
										boolResult =  false;
										break;
									}
								}
								if (boolResult)
								{
									if (Int32.Parse(entryList[0]) > Int32.Parse(entryList[1]))
									{
										boolResult = false;
									}
								}
							}
							else
							{
								boolResult = false;
								break;
							}
						}
					}
				}
				else if ((strEntry.Contains("-")) && (!strEntry.Contains(",")))
				{
					entryList = strEntry.Split('-');
					if (entryList.Length == 2)
					{
						foreach (string entryRange in entryList)
						{
							if (entryRange == String.Empty)
							{
								boolResult =  false;
								break;
							}
						}
						if (boolResult)
						{
							if (Int32.Parse(entryList[0]) > Int32.Parse(entryList[1]))
							{
								boolResult = false;
							}
						}
					}
					else
					{
						boolResult = false;
					}
				}
				if (!boolResult)
				{
					strInfo = "Feil: Ugyldig dataregistrering";
				}
				if ((strEntry == "") || (strTestsuite == ""))
				{
					boolResult = false;
					strInfo = "Feil: Alle felt må fylles ut";
				}
			}
			
			// Execute and stop at specific step
			else if (scriptMode == "execute step")
			{
				string strStep = txtSingleScript.Text;
				string testSuiteId = txtTestsuite.Text;
				if ((strTestsuite == "") || (strEntry == "") || (strStep == ""))
				{
					boolResult = false;
					strInfo = "Feil: Alle felt må fylles ut";
				}
				if (Int16.Parse(strStep) <=1)
				{
					boolResult = false;
					strInfo = "Feil: Trinn nummer må være større enn 1";
				}
				if (boolResult)
				{
					int maxStep = MySQLConnector.GetNumberOfSteps(testSuiteId, strEntry);
					if (Int16.Parse(strStep) >= maxStep)
					{
						boolResult = false;
						strInfo = "Feil: Du har nådd over maksimal trinnet (" + (maxStep-1).ToString() + ")";
					}
				}
			}
			
			// Write single script
			else if (scriptMode == "write_single")
			{
				if ((strEntry == "") || (cboTestCase.Text == ""))
				{
					boolResult = false;
					strInfo = "Feil: Alle felt må fylles ut";
				}
			}
			lblInfoMsg.Text = strInfo;
			return boolResult;
		}
		
		private void DeleteLogs(string logType)
		{
			switch (logType)
			{
				case "Logs":
					//Array.ForEach(Directory.GetFiles(@"Logs\"), File.Delete);
					string[] eventLogs = Directory.GetFiles(@"Logs\", "System-XAuto-Test - EventLog_2*.htm");
					foreach (string eventLog in eventLogs)
					{
					    File.Delete(eventLog);
					}
					string[] buildLogs = Directory.GetFiles(@"Logs\", "*.log");
					foreach (string buildLog in buildLogs)
					{
					    File.Delete(buildLog);
					}
					break;
				case "Reports":
					//Array.ForEach(Directory.GetFiles(@"Reports\"), File.Delete);
					string[] testReports = Directory.GetFiles(@"Reports\Utility\", "report_2*.htm");
					foreach (string testReport in testReports)
					{
					    File.Delete(testReport);
					}
					string[] rxLogs = Directory.GetFiles(@"Reports\", "*.*");
					foreach (string rxLog in rxLogs)
					{
					    File.Delete(rxLog);
					}
					break;
				case "RX_Projects":
					/*string[] rxProjects = Directory.GetDirectories(@"WrittenTests\Raw\");
					foreach (string rxProject in rxProjects)
					{
					    Directory.Delete(rxProject, true);
					}*/
					/*GlobalFunction.RunCommandProcess(@"RD /S /Q WrittenTests\Raw\");
					Directory.CreateDirectory(@"WrittenTests\Raw\");*/
					var testsuites = Directory.GetDirectories(@"WrittenTests\Raw", "TS_*");
					foreach (var directory in testsuites)
				  	{
						try
						{
			      			Directory.Delete(directory, true);
						}
						catch (Exception)
						{
							GlobalFunction.ShowMessage("WARNING: Kan ikke slette alle loggene!", MessageBoxIcon.Warning);
						}
				  	}
					break;
			}
			
			/*string executionTime = GlobalFunction.ExecutionTime();
			string logFilename = @"Logs\System-XAuto-Test - EventLog_" + executionTime.Replace("-", String.Empty).Replace(":", String.Empty).Replace(" ", String.Empty) + ".htm";
			System.IO.File.WriteAllText(logFilename, System.IO.File.ReadAllText("Logs\\System-XAuto-Test - EventLog.htm"));
			System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AppEvent.log", logFilename);
			GlobalFunction.WriteReportHeaderInfo(logFilename, "%Version%", WinComponent.GetAppInfo("Version"));*/
		}
		
		private void btnClear_Click(object sender, System.EventArgs e)
		{
			bool isLogChecked = this.chkLogs.Checked;
			bool isReportChecked = this.chkReports.Checked;
			bool isRXChecked = this.chkRawScripts.Checked;
			string oldValue = GlobalFunction.GetConfigSettings("LimitSize");
			string deleteLogsAtExitOld = "value=\"" + oldValue + "\"";
			string newValue = this.txtLimitSize.Text;
			
			int reportCount = Directory.GetFiles(@"Reports\", "*", SearchOption.AllDirectories).Length - 3 ;
			//int logCount = (Directory.GetFiles(@"Logs\", "*_2*", SearchOption.AllDirectories).Length + Directory.GetFiles(@"Logs\", "*.log", SearchOption.AllDirectories).Length) - 1;
			int logCount = Directory.GetFiles(@"Logs\", "*", SearchOption.AllDirectories).Length - 1;
			int rxCount = Directory.GetDirectories(@"WrittenTests\Raw\").Length;
			
			if (!isLogChecked && !isReportChecked && !isRXChecked)
			{
				GlobalFunction.ShowMessage("Feil: Ingen valgt funnet!", MessageBoxIcon.Error);
			}
			else
			{
				if (isLogChecked)
				{
					DeleteLogs("Logs");
					logCount = 0;
					chkLogs.Enabled = false;
				}
				if (isReportChecked)
				{
					DeleteLogs("Reports");
					reportCount = 0;
					chkReports.Enabled = false;
				}
				if (isRXChecked)
				{
					DeleteLogs("RX_Projects");
					rxCount = 0;
					chkRawScripts.Enabled = false;
				}
				
				lblInfoMsg.Text = "Info: Sletter logger og RX-prosjekter. Vennligst vent...";
				btnUpdate.Enabled = false;
				Delay.Seconds(2);
				lblInfoMsg.Text = "Info: Ferdig";
				btnUpdate.Enabled = true;
				
				if (reportCount == 0 && logCount == 0 && rxCount == 0)
				{
					this.btnUpdate.Enabled = false;
				}
				
				this.chkReports.Text = "Testrapporter (" + reportCount.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("nb-NO")) + " fil(er) funnet)";
				this.chkLogs.Text = "Program logger (" + logCount.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("nb-NO")) + " fil(er) funnet)";
				this.chkRawScripts.Text = "RX-prosjekter (" + rxCount.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("nb-NO")) + " mappe(r) funnet)";
			}
		}
		
		private void btnUpdate_Click(object sender, System.EventArgs e)
		{
			string dbSettingsFile = "SystemXAuto-Test.exe.config";
			//Database Server
			string currentDbServer = GlobalFunction.GetConfigSettings("DBServer");
			string newDbServer = this.txtServerName.Text;
			
			//Database Name
			string currentDbName = "sxtest_" + sxDBVersion.Replace(".", String.Empty); //GlobalFunction.GetConfigSettings("DBName");
			string newDbName = this.txtDbName.Text;
			
			//Database Username
			string currentDbUsername = GlobalFunction.GetConfigSettings("DBUsername");
			string newDbUsername = this.txtDbUser.Text;
			
			//Database Password
			string currentDbPass = GlobalFunction.GetConfigSettings("DBPassword");
			string newDbPass = GlobalFunction.Encrypt(this.txtDbPass.Text, true);
			
			//Update Database Server
			File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(currentDbServer, newDbServer));
			
			//Update Database Name
			File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(currentDbName, newDbName));
			
			//Update Database Username
			File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(currentDbUsername, newDbUsername));
			
			//Update Database Password
			File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(currentDbPass, newDbPass));
			
			this.lblInfoMsg.Text = "Info: Databaseinnstillinger har blitt oppdatert";
			GlobalFunction.ShowMessage("Nye databaseinnstillinger har blitt lagret!\r\n\nKlikk OK for å lukke programmet og legg de nye databaseinnstillinger.", MessageBoxIcon.Information);
			Process.Start(Application.StartupPath + @"\SystemXAuto-Test.exe");
			Process.GetCurrentProcess().Kill();
		}
		
		private void lnkMail_Click(object sender, System.EventArgs e)
		{
			System.Diagnostics.Process.Start("mailto:rommel@systemx.no");
		}
		
		private void browseFolder_Click(object sender, System.EventArgs e)
		{
			string selected = groupBox1.Text;
			string promptMsg = "Velge en mappe hvor du vil lagre SQL-fil:";
			TextBox txtPath = txtBackupDir;
			
			if (selected.Contains("Opsjoner"))
			{
				promptMsg = "Velge en mappe hvor du vil lagre skriptene:";
				if ((lblOption.Text == "MSBuild") || (lblOption.Text == "MySQL") || (lblOption.Text == "AutoItBuild"))
				{
					promptMsg = "Finn banen for " + lblOption.Text + ":";
				}
				txtPath = txtScriptFolder;
			}
			
			this.folderBrowserDialog1.Description = promptMsg;
			folderBrowserDialog1.SelectedPath = txtPath.Text;
			DialogResult result = folderBrowserDialog1.ShowDialog();
		    if (result == DialogResult.OK)
		    {
				txtPath.Text = folderBrowserDialog1.SelectedPath;
		    }
		}
		
		private void txtScriptFolder_Keypress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Escape)
			{
				string logFilename = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AppEvent.log");
				this.Close();
			}
			else
			{
				e.Handled = true;
			}
		}
		
		private void frmMain_Keypress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Escape)
			{
				//MySQLConnector.SetUserStatus(lastLoginID, true);
				this.Close();
			}
		}
		
		private void btnSave_Click(object sender, System.EventArgs e)
		{
			string optionSelected = lblOption.Text;
			string dbSettingsFile = "SystemXAuto-Test.exe.config";
			
			switch (optionSelected)
			{
				case "Database":
					//Database Server
					string currentDbServer = GlobalFunction.GetConfigSettings("DBServer");
					string newDbServer = this.txtServerName.Text;
					
					//Database Name
					string currentDbName = "sxtest_" + sxDBVersion.Replace(".", String.Empty); //GlobalFunction.GetConfigSettings("DBName");
					string newDbName = this.txtDbName.Text;
					
					//Database Username
					string currentDbUsername = GlobalFunction.GetConfigSettings("DBUsername");
					string newDbUsername = this.txtDbUser.Text;
					
					//Database Password
					string currentDbPass = GlobalFunction.GetConfigSettings("DBPassword");
					string newDbPass = GlobalFunction.Encrypt(this.txtDbPass.Text, true);
					
					//Update Database Server
					File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(currentDbServer, newDbServer));
					
					//Update Database Name
					File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(currentDbName, newDbName));
					
					//Update Database Username
					File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(currentDbUsername, newDbUsername));
					
					//Update Database Password
					File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(currentDbPass, newDbPass));
					break;
				case "Skripter":
					string scriptFolder = (GlobalFunction.GetConfigSettings("ScriptsDir")).Replace("%DBName%", "v" + sxDBVersion); //GlobalFunction.GetConfigSettings("ScriptsDir");
					//New script storage
					string newFolder = this.txtScriptFolder.Text;
					//Update Script Storage
					File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(scriptFolder, newFolder));
					break;
				case "Logger":
					string limitSize = GlobalFunction.GetConfigSettings("LimitSize");
					string enableProgramLog = GlobalFunction.GetConfigSettings("EnableErrorLog");
					string openLog = GlobalFunction.GetConfigSettings("OpenLogAtWrite");
					string oldLimitSize = "value=\"" + limitSize + "\"";
					string oldEnableProgramLog = "value=\"" + enableProgramLog + "\"";
					string oldOpenLog = "value=\"" + openLog + "\"";
					//New limit size
					string newLimitSize =  "value=\"" + this.txtLimitSize.Text + "\"";
					string newEnableProgramLog =  "value=\"" + chkLogs.Checked.ToString().ToLower() + "\"";
					string newOpenLog =  "value=\"" + chkOpenLog.Checked.ToString().ToLower() + "\"";
					
					//Update Limit Size
					File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(oldLimitSize, newLimitSize));
					
					//Update Enable Program Log
					File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(oldEnableProgramLog, newEnableProgramLog));
					
					//Update Open Log
					File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(oldOpenLog, newOpenLog));
					
					break;
				case "Startmeny":
					string oldStartMenu = GlobalFunction.GetConfigSettings("StartMenu");
					string newStartMenu = lblTemp.Text;
					File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(oldStartMenu, newStartMenu));
					break;
				case "MSBuild": case "MySQL":
					string oldPath = GlobalFunction.GetConfigSettings(optionSelected);
					string newPath = txtScriptFolder.Text;
					string valOldPath = "key=\"" + optionSelected + "\" value=\"" + oldPath + "\"";
					string valNewPath = "key=\"" + optionSelected + "\" value=\"" + newPath + "\"";
					File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(valOldPath, valNewPath));
					break;
			}
			this.lblInfoMsg.Text = "Info: Nye endringer er lagret!";
			//lblInfoMsg.BackColor = System.Drawing.Color.Gold;
			//lblClock.BackColor = System.Drawing.Color.Gold;
			
			
			//GlobalFunction.ShowMessage("Opsjoner har blitt oppdatert!\r\n\nProgrammet skal starte på nytt...", MessageBoxIcon.Information);
			lblInfoMsg.Text = "Info: Opsjoner har blitt oppdatert. Programmet skal starte på nytt...";
			Delay.Seconds(2);
			Process.Start(Application.StartupPath + @"\SystemXAuto-Test.exe");
			Process.GetCurrentProcess().Kill();
		}
		
		private void ReportError(string errorMessage)
		{
			lblInfoMsg.Text = "Feil: " + errorMessage;
			GlobalFunction.ShowMessage("Feil: " + errorMessage, MessageBoxIcon.Error);
		}
		
		private void tmrClock_Tick(object sender, EventArgs e)
		{
			string backupTime = GlobalFunction.GetConfigSettings("BackupTime") + ":00";
			string autoBackup = GlobalFunction.GetConfigSettings("AutoBackup");
			string backupDir = (GlobalFunction.GetConfigSettings("DbBackupDir")).Replace("%DBName%", "v" + sxDBVersion);
			this.lblClock.Text = " | " + System.DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss");
			if ((autoBackup == "true") && (backupTime == System.DateTime.Now.ToString("HH:mm:ss")))
			{
				MySQLConnector.DatabaseBackup(backupDir);
			}
		}
		
		private void tmrClock2_Tick(object sender, EventArgs e)
		{
			lblInfoMsg.Text = "Info: Ferdig...";
			btnDeploy.Text = "&Deploy File";
			btnDeploy.Enabled = true;
			tmrClock2.Stop();
		}
		
		private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs  e)
		{
            lblOption.Text = e.Node.Text;
            gbOptions.Controls.Clear();
            gbOptions.Controls.Add(this.btnSave);
            switch (lblOption.Text)
            {
            	case "Database":
            		gbOptions.Controls.Add(txtServerName);
            		gbOptions.Controls.Add(lblDbServer);
            		gbOptions.Controls.Add(txtDbName);
            		gbOptions.Controls.Add(lblDbName);
            		gbOptions.Controls.Add(txtDbUser);
            		gbOptions.Controls.Add(lblDbUser);
            		gbOptions.Controls.Add(txtDbPass);
            		gbOptions.Controls.Add(lblDbPass);
            		this.txtServerName.Text = GlobalFunction.GetConfigSettings("DBServer");
					this.txtDbName.Text = "sxtest_" + sxDBVersion.Replace(".", String.Empty); //GlobalFunction.GetConfigSettings("DBName");
					this.txtDbUser.Text = GlobalFunction.GetConfigSettings("DBUsername");
					this.txtDbPass.Text = GlobalFunction.Decrypt(GlobalFunction.GetConfigSettings("DBPassword"),true);
            		break;
            	case "Skripter":
            		string scriptFolder = (GlobalFunction.GetConfigSettings("ScriptsDir")).Replace("%DBName%", "v" + sxDBVersion); //GlobalFunction.GetConfigSettings("ScriptsDir");
					this.gbOptions.Controls.Add(this.txtScriptFolder);
					this.gbOptions.Controls.Add(this.lblScriptFolder);
					this.gbOptions.Controls.Add(this.btnBrowse);
					txtScriptFolder.Text = scriptFolder;
            		break;
            	case "Logger":
            		string limitSize = GlobalFunction.GetConfigSettings("LimitSize");
            		string programLog = GlobalFunction.GetConfigSettings("EnableErrorLog");
            		string openLog = GlobalFunction.GetConfigSettings("OpenLogAtWrite");
            		bool boolProgramLog = (programLog == "true");
            		bool boolOpenLog = (openLog == "true");
            		
            		gbOptions.Controls.Add(chkLogs);
            		gbOptions.Controls.Add(txtLimitSize);
            		gbOptions.Controls.Add(chkOpenLog);
            		gbOptions.Controls.Add(lblLimitSize);
            		txtLimitSize.Text = limitSize;
            		chkLogs.Text = "Aktivere program logging";
            		chkLogs.Location = new System.Drawing.Point(3, 14);
            		chkLogs.BackColor = Color.Transparent;
            		chkLogs.CheckedChanged += new EventHandler(chkLogs_CheckedChanged);
            		chkLogs.Checked = boolProgramLog;
            		chkOpenLog.Checked = boolOpenLog;
            		break;
            	case "Startmeny":
            		gbOptions.Controls.Add(rbButton5);
            		gbOptions.Controls.Add(rbButton4);
            		gbOptions.Controls.Add(rbButton3);
            		gbOptions.Controls.Add(rbButton2);
            		gbOptions.Controls.Add(rbButton1);
            		SetCheckedButton(startMenu);
            		break;
            	case "MSBuild": case "MySQL": case "AutoItBuild":
            		string txtValue = GlobalFunction.GetConfigSettings("MSBuild");
            		if (lblOption.Text == "MySQL")
            		{
            			txtValue = GlobalFunction.GetConfigSettings("MySQL");
            		}
            		else if (lblOption.Text == "AutoItBuild")
            		{
            			txtValue = GlobalFunction.GetConfigSettings("AutoItBuild");
            		}
            		lblPath = new Label();
            		txtPath = new TextBox();
            		lblPath.Text = lblOption.Text + "-banen";
            		lblPath.Location = new Point(1,34);
					lblPath.Size = new System.Drawing.Size(150, 20);
					txtScriptFolder.Text = txtValue;
            		gbOptions.Controls.Add(this.txtScriptFolder);
            		gbOptions.Controls.Add(lblPath);
            		gbOptions.Controls.Add(this.btnBrowse);
            		break;
            }
            
		}
		
		private void chkLogs_CheckedChanged(object sender, EventArgs e)
		{
			txtLimitSize.Enabled = chkLogs.Checked;
		}
		
		private void frmDlg_Keypress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			System.Windows.Forms.Form frmLogin = sender as System.Windows.Forms.Form;
			if (e.KeyChar == (char)Keys.Escape)
			{
				frmLogin.Close();
			}
		}
				
		private DialogResult UserLogin(ref string userName, ref string password, ref string database)
		{
		  System.Windows.Forms.Form frmLogin = new System.Windows.Forms.Form();
		  
		  Label lblUsername = new Label();
		  Label lblPassword = new Label();
		  Label lblDatabase = new Label();
		  TextBox txtUsername = new TextBox();
		  TextBox txtPassword = new TextBox();
		  cboDatabase = new System.Windows.Forms.ComboBox();
		  System.Windows.Forms.Button buttonOk = new System.Windows.Forms.Button();
		  System.Windows.Forms.Button buttonAbort = new System.Windows.Forms.Button();
		  
		  /*string[] databases = new string[]{"4.1", "5.1", "5.2"};
		  cboDatabase.Items.AddRange(databases);*/
		  
		  
		  string myConnection = "SERVER=" + GlobalFunction.GetConfigSettings("DBServer") + ";" +
	             "UID=" + GlobalFunction.GetConfigSettings("DBUsername") + ";" +
	              "PASSWORD=" + GlobalFunction.Decrypt(GlobalFunction.GetConfigSettings("DBPassword"),true) + ";" +
					"Connection TIMEOUT=5;" +
				 "Respect Binary Flags=false;";
		  
		  using (var connection = new MySqlConnection(myConnection))
    	  {
		  	connection.Open();
		  	string query = "SHOW DATABASES WHERE `Database` LIKE '%sxtest_%' AND `Database` NOT LIKE '%testdb%'";
		  	string databases;
		  	using (var command = new MySqlCommand(query, connection))
        	{
		  		using (var reader = command.ExecuteReader())
            	{
		  			while (reader.Read())
		  			{
		  				databases = reader.GetString("Database").Replace("sxtest_", string.Empty);
		  				string dbItem = databases.Substring(0, 1) + "." + databases.Substring(1, 1);
		  				cboDatabase.Items.Add(dbItem);
		  			}
		  			
		  		}
		  	}
		  }
		  
		  cboDatabase.DropDownStyle = ComboBoxStyle.DropDownList;
		
		  frmLogin.Text = "Auto X Tool | Logg inn";
		  lblUsername.Text = "Brukernavn:";
		  lblUsername.BackColor = System.Drawing.Color.Transparent;
		  lblPassword.Text = "Passord:";
		  lblPassword.BackColor = System.Drawing.Color.Transparent;
		  lblDatabase.Text = "Database:";
		  lblDatabase.BackColor = System.Drawing.Color.Transparent;
		
		  buttonOk.Text = "Logg inn";
		  buttonAbort.Text = "Avbryt";
		  buttonOk.DialogResult = DialogResult.OK;
		  buttonAbort.DialogResult = DialogResult.Abort;
		
		  lblUsername.SetBounds(15, 18, 72, 13);
		  txtUsername.SetBounds(90, 14, 272, 20);
		  txtUsername.Text = userName;
		  lblPassword.SetBounds(15, 43, 72, 13);
		  txtPassword.SetBounds(90, 39, 272, 20);
		  txtPassword.Text = password;
		  buttonOk.SetBounds(167, 102, 75, 23);
		  buttonAbort.SetBounds(250, 102, 75, 23);
		  
		  lblDatabase.SetBounds(15, 68, 72, 13);
		  cboDatabase.SetBounds(90, 64, 177, 23);
		  cboDatabase.Text = database;
		
		  lblPassword.TextAlign = ContentAlignment.MiddleRight;
		  lblUsername.TextAlign = ContentAlignment.MiddleRight;
		  lblDatabase.TextAlign = ContentAlignment.MiddleRight;
		  txtUsername.Anchor = txtUsername.Anchor | AnchorStyles.Right;
		  txtUsername.BorderStyle = BorderStyle.FixedSingle;
		  txtPassword.Anchor = txtUsername.Anchor | AnchorStyles.Right;
		  txtPassword.BorderStyle = BorderStyle.FixedSingle;
		  txtPassword.PasswordChar = '•';
		  buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		  buttonAbort.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		  
		  frmLogin.ClientSize = new Size(396, 137);
		  frmLogin.Controls.AddRange(new System.Windows.Forms.Control[] { lblPassword, lblUsername, lblDatabase, txtUsername, txtPassword, cboDatabase, buttonOk, buttonAbort });
		  
		  frmLogin.ClientSize = new Size(Math.Max(300, lblUsername.Right + 10), frmLogin.ClientSize.Height);
		  frmLogin.FormBorderStyle = FormBorderStyle.FixedDialog;
		  frmLogin.StartPosition = FormStartPosition.CenterScreen;
		  frmLogin.MinimizeBox = false;
		  frmLogin.MaximizeBox = false;
		  frmLogin.AcceptButton = buttonOk;
		  frmLogin.TopMost = true;
		  frmLogin.Icon = (Icon)resMgr.GetObject("Lock");
		  frmLogin.KeyPreview = true;
		  
		  frmLogin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(frmDlg_Keypress);
		  DialogResult dialogResult = frmLogin.ShowDialog();
		  
		  userName = txtUsername.Text;
		  password = txtPassword.Text;
		  database = cboDatabase.Text;
		  return dialogResult;
		}
		
		
		private void rbButton_CheckedChanged(object sender, EventArgs e)
		{
		    string rbButtonName = null;
		    foreach (System.Windows.Forms.Control control in this.gbOptions.Controls)
		    {
				if (control is System.Windows.Forms.RadioButton)
				{
				    System.Windows.Forms.RadioButton radio = control as System.Windows.Forms.RadioButton;
				    if (radio.Checked)
				    {
						rbButtonName = radio.Name;
				    }
				}
		    }
		   	lblTemp.Text = rbButtonName;
		}
		
		private void SetCheckedButton(string rbButtonName)
		{
		    foreach (System.Windows.Forms.Control control in this.gbOptions.Controls)
		    {
				if (control is System.Windows.Forms.RadioButton)
				{
				    System.Windows.Forms.RadioButton radio = control as System.Windows.Forms.RadioButton;
				    if (radio.Name == rbButtonName)
				    {
						radio.Checked = true;
						break;
				    }
				}
		    }
		}
		
		private void mnuUserlist_Click(object sender, System.EventArgs e)
		{
			ShowUsers();
		}
		
		private void ShowUsers()
		{
			lblInfoMsg.Text = "Info: Klar";
			lblInfoMsg.ForeColor = Color.Empty;
			string connectionString2 = "SERVER=" + GlobalFunction.GetConfigSettings("DBServer") + ";" +
	             "DATABASE=systemxtest_main;" +
	             "UID=" + GlobalFunction.GetConfigSettings("DBUsername") + ";" +
	              "PASSWORD=" + GlobalFunction.Decrypt(GlobalFunction.GetConfigSettings("DBPassword"),true) + ";" +
					"Connection TIMEOUT=5;" +
				 "Respect Binary Flags=false;";
			this.groupBox1.Controls.Clear();		 
			groupBox1.Text = "";
			DataGridView dataGridView1 = new DataGridView();
			MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter("SELECT LNAME as 'Etternavn', FNAME as 'Fornavn'  FROM designers WHERE USERNAME != 'admin' AND ACTIVE=1 ORDER BY LNAME ASC", connectionString2);
			DataSet DS = new DataSet();
			mySqlDataAdapter.Fill(DS);
			dataGridView1.DataSource = DS.Tables[0];
			dataGridView1.BorderStyle = BorderStyle.None;
			dataGridView1.Location = new Point(2,7);
			dataGridView1.Size = new Size(264,160);
			
			groupBox1.Controls.Add(dataGridView1);
			dataGridView1.Columns[0].Width = 111;
			dataGridView1.Columns[1].Width = 111;
			CheckLogs();
		}
		
		private void mnuScriptStatus_Click(object sender, System.EventArgs e)
		{
			ShowScriptStatus();
		}
		
		private void ShowScriptStatus()
		{
			lblInfoMsg.Text = "Info: Klar";
			lblInfoMsg.ForeColor = Color.Empty;
			string testsuiteCount = MySQLConnector.GetNumberOfTestsuites().ToString();
			string testcaseCount = MySQLConnector.GetNumberOfTestcases().ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("nb-NB"));
			string testcaseWriteCount = MySQLConnector.GetNumberOfTestcases(true).ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("nb-NB"));
			Label lblLabel = new Label();
			Label lblCount = new Label();
			lblLabel.Text = "Testsuiter\n\nTestcaser\n\nTestcaser skrevet";
			lblLabel.Size = new Size(100,100);
			lblLabel.TextAlign = ContentAlignment.TopLeft;
			lblLabel.Location = new Point(5,25);
			
			lblCount.Text = testsuiteCount + "\n\n" + testcaseCount + "\n\n" + testcaseWriteCount;
			lblCount.Size = new Size(100,100);
			lblCount.TextAlign = ContentAlignment.TopLeft;
			lblCount.Location = new Point(150,25);
			
			this.groupBox1.Controls.Clear();		 
			groupBox1.Text = "Skript status";
			groupBox1.Controls.Add(lblLabel);
			groupBox1.Controls.Add(lblCount);
			
			CheckLogs();
		}
		
		private void mnuBackup_Click(object sender, System.EventArgs e)
		{
			BackupDatabase();
		}
		
		private void BackupDatabase()
		{
			lblInfoMsg.Text = "Info: Klar";
			lblInfoMsg.ForeColor = Color.Empty;
			Label lblDatabaseName = new Label();
			Label lblBackupDir = new Label();
			TextBox txtDatabaseName = new TextBox();
			chkAutoBackup = new System.Windows.Forms.CheckBox();
			timePicker = new DateTimePicker();
			string autoBackup = GlobalFunction.GetConfigSettings("AutoBackup");
			System.Windows.Forms.Button btnBrowseDir = new System.Windows.Forms.Button();
			
			lblDatabaseName.Text = "Database navn";
			lblDatabaseName.Location = new Point(5,15);
			txtDatabaseName.Size = new Size(170, 20);
			txtDatabaseName.Location = new Point(7, 30);
			txtDatabaseName.BorderStyle = BorderStyle.FixedSingle;
			txtDatabaseName.Text = "sxtest_" + sxDBVersion.Replace(".", String.Empty); //GlobalFunction.GetConfigSettings("DBName");
			txtDatabaseName.ContextMenu = new System.Windows.Forms.ContextMenu();
			txtDatabaseName.KeyPress+= new System.Windows.Forms.KeyPressEventHandler(txtScriptFolder_Keypress);
			
			lblBackupDir.Text = "Eksport til";
			lblBackupDir.Location = new Point(5,60);
			txtBackupDir.Size = new Size(170, 20);
			txtBackupDir.Location = new Point(7, 75);
			txtBackupDir.BorderStyle = BorderStyle.FixedSingle;
			txtBackupDir.Text = (GlobalFunction.GetConfigSettings("DbBackupDir")).Replace("%DBName%", "v" + sxDBVersion); //GlobalFunction.GetConfigSettings("DbBackupDir");
			txtBackupDir.KeyPress+= new System.Windows.Forms.KeyPressEventHandler(txtScriptFolder_Keypress);
			txtBackupDir.ContextMenu = new System.Windows.Forms.ContextMenu();
			btnBrowseDir.Location = new System.Drawing.Point(185, 75);
			btnBrowseDir.Size = new System.Drawing.Size(25, 20);
			btnBrowseDir.Text = "...";
			btnBrowseDir.Font = new System.Drawing.Font(btnBrowse.Font, System.Drawing.FontStyle.Bold);
			btnBrowseDir.Click+= new EventHandler(browseFolder_Click);
			
			btnBackup.Location = new System.Drawing.Point(65, 134);
			btnBackup.Size = new System.Drawing.Size(110, 24);
			btnBackup.Text = "Backup";
			btnBackup.Click += new EventHandler(btnBackup_Click);
			
			chkAutoBackup.Location = new Point(7,105);
			chkAutoBackup.Size = new Size(180,20);
			chkAutoBackup.Text = "Auto-backup database hver kl.";
			chkAutoBackup.CheckedChanged += new EventHandler(chkAutoBackup_CheckedChanged);
			chkAutoBackup.Checked = false;
			
            timePicker.Format = DateTimePickerFormat.Custom;
            timePicker.CustomFormat = "HH:mm";
            timePicker.ShowUpDown = true;
            timePicker.Location = new Point(185, 105);
            timePicker.Width = 55;
            timePicker.Text = GlobalFunction.GetConfigSettings("BackupTime");
            timePicker.Enabled = false;
            
            if (autoBackup == "true")
			{
				chkAutoBackup.Checked = true;
				timePicker.Enabled = true;
			}
			
			groupBox1.Controls.Clear();
			groupBox1.Text = "Backup database";
			groupBox1.Controls.Add(txtDatabaseName);
			groupBox1.Controls.Add(lblDatabaseName);
			groupBox1.Controls.Add(txtBackupDir);
			groupBox1.Controls.Add(lblBackupDir);
			groupBox1.Controls.Add(btnBrowseDir);
			groupBox1.Controls.Add(btnBackup);
			groupBox1.Controls.Add(timePicker);
			groupBox1.Controls.Add(chkAutoBackup);
			
			CheckLogs();
		}
		
		private void btnBackup_Click(object sender, System.EventArgs e)
		{
			string dbSettingsFile = "SystemXAuto-Test.exe.config";
			// Backup directory
			string oldDir = (GlobalFunction.GetConfigSettings("DbBackupDir")).Replace("%DBName%", "v" + sxDBVersion); //GlobalFunction.GetConfigSettings("DbBackupDir");
			string newDir = txtBackupDir.Text;
			// Auto-backup On/Off
			string oldAutoBackup = GlobalFunction.GetConfigSettings("AutoBackup");
			string newAutoBackup = chkAutoBackup.Checked.ToString().ToLower();
			// Backup time
			string oldBackupTime = GlobalFunction.GetConfigSettings("BackupTime");
			string newBackupTime = timePicker.Text;
			
			string value1 = "key=\"DbBackupDir\" value=\"" + oldDir + "\"";
			string value2 = "key=\"DbBackupDir\" value=\"" + newDir + "\"";
			
			string valAutoBackup1 = "key=\"AutoBackup\" value=\"" + oldAutoBackup + "\"";
			string valAutoBackup2 = "key=\"AutoBackup\" value=\"" + newAutoBackup + "\"";
			
			MySQLConnector.DatabaseBackup(newDir);
			btnBackup.Text = "Vent...";
			lblInfoMsg.Text = "Sikkerhetskopiering av database...";
			btnBackup.Enabled = false;
			Delay.Milliseconds(3000);
			GlobalFunction.DbBackupCheck(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Backup-error.log");
			btnBackup.Enabled = true;
			btnBackup.Text = "Backup";
			lblInfoMsg.Text = "Info: Klar";
			//lblInfoMsg.BackColor = Color.Empty;
			//lblClock.BackColor = Color.Empty;
			
			if ((oldDir != newDir) || (oldAutoBackup != newAutoBackup) || (oldBackupTime != newBackupTime))
			{
				// Update changes
				File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(value1, value2));
				File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(valAutoBackup1, valAutoBackup2));
				File.WriteAllText(dbSettingsFile, System.IO.File.ReadAllText(dbSettingsFile).Replace(oldBackupTime, newBackupTime));
				
				System.Windows.Forms.DialogResult confirm = GlobalFunction.ConfirmMessage("Denne innstillingen er endret...\n\nØnsker du å starte programmet på nytt?");
				if (confirm == System.Windows.Forms.DialogResult.Yes)
				{
					Process.Start(Application.StartupPath + @"\SystemXAuto-Test.exe");
					Process.GetCurrentProcess().Kill();
				}
			}
		}
		
		private void chkAutoBackup_CheckedChanged(object sender, EventArgs e)
		{
			timePicker.Enabled = chkAutoBackup.Checked;
		}
		
		private bool IsCommit(string gitRepoDir)
        {
			string currentToolDirectory = Directory.GetCurrentDirectory().ToString();
            Directory.SetCurrentDirectory(gitRepoDir);
            //int result = GlobalFunction.RunCommandProcess("git diff-index --quiet HEAD");
            //int result = GlobalFunction.RunCommandProcess("git status --porcelain > %APPDATA%\\GitStatus.log");
            int result = GlobalFunction.RunCommandProcess("git status > %APPDATA%\\GitStatus.log");
            Directory.SetCurrentDirectory(currentToolDirectory);
            
            bool response = true;
            if (result == 0)
            {
            	response = GetGitStatus();
            	if (!response)
	            {
	            	lblInfoMsg.Text = "Info: Det er filer å commit";
					lblInfoMsg.ForeColor = Color.Firebrick;
	            }
            	else
	            {
	            	lblInfoMsg.Text = "Info: Ingen filer ble endret eller lagt til siden siste commit";
					lblInfoMsg.ForeColor = Color.Empty;
	            }
            }
            else if (result == 128)
            {
            	lblInfoMsg.Text = "Feil: " + gitRepoDir + " er ikke en git repository";
				lblInfoMsg.ForeColor = Color.Red;
            }
			/*
            if (result == 1)
            {
            	lblInfoMsg.Text = "Info: Det er filer å commit";
				lblInfoMsg.ForeColor = Color.Red;
				response = false;
            }
            else if (result == 128)
            {
            	lblInfoMsg.Text = "Feil: " + gitRepoDir + " er ikke en git repository";
				lblInfoMsg.ForeColor = Color.Red;
            }
            else
            {
            	lblInfoMsg.Text = "Info: Ingen filer ble endret eller lagt til siden siste commit";
				lblInfoMsg.ForeColor = Color.Empty;
            }
            */
            return response;
        }
		
		private void DeployFile()
		{
			string dbVersionGit = sxDBVersion;
			string gitRepoDir = (GlobalFunction.GetConfigSettings("ScriptsDir")).Replace("%DBName%", "v" + sxDBVersion);
			lblFileSource = new Label();
			Label lblFileDest = new Label();
			txtFileSource = new TextBox();
			txtGitRepo = new TextBox();
			btnDeployFile = new System.Windows.Forms.Button();
			System.Windows.Forms.Button btnBrowseDir = new System.Windows.Forms.Button();
			System.Windows.Forms.Button btnBrowseFileSource = new System.Windows.Forms.Button();
			btnDeploy = new System.Windows.Forms.Button();
			
			lblFileSource.Text = "&Kildefil";
			lblFileSource.Location = new Point(5,30);
			lblFileSource.AutoSize = true;
			lblFileSource.UseMnemonic = true;
			
			txtFileSource.Location = new Point(7, 50);
			txtFileSource.BorderStyle = BorderStyle.FixedSingle;
			txtFileSource.Size = new Size(200, 20);
			
			btnBrowseFileSource.Location = new System.Drawing.Point(212, 48);
			btnBrowseFileSource.Size = new System.Drawing.Size(50, 21);
			btnBrowseFileSource.Text = "&Browse";
			btnBrowseFileSource.Click += new EventHandler(btnBrowseFileSource_Click);
			btnBrowseFileSource.Enabled = true;
			
			lblFileDest.Text = "&Skript kilde";
			lblFileDest.Location = new Point(5,75);
			lblFileDest.AutoSize = true;
			lblFileDest.UseMnemonic = true;
			
			txtGitRepo.Location = new Point(7, 95);
			txtGitRepo.BorderStyle = BorderStyle.FixedSingle;
			txtGitRepo.Size = new Size(200, 20);
			txtGitRepo.Text = gitRepoDir;
			
			btnBrowseDir.Location = new System.Drawing.Point(212, 93);
			btnBrowseDir.Size = new System.Drawing.Size(50, 21);
			btnBrowseDir.Text = "B&rowse";
			btnBrowseDir.Click += new EventHandler(browseScriptSource_Click);
			btnBrowseDir.Enabled = true;
			
			btnDeploy.Location = new System.Drawing.Point(80, 134);
			btnDeploy.Size = new System.Drawing.Size(110, 24);
			btnDeploy.Text = "&Deploy File";
			btnDeploy.Click += new EventHandler(btnDeploy_Click);
			btnDeploy.Enabled = true;
			
			groupBox1.Controls.Clear();
			groupBox1.Text = "Deploy fil til SX " + dbVersionGit + " skripter";
			groupBox1.Controls.Add(lblFileSource);
			groupBox1.Controls.Add(txtFileSource);
			groupBox1.Controls.Add(btnBrowseFileSource);
			groupBox1.Controls.Add(lblFileDest);
			groupBox1.Controls.Add(txtGitRepo);
			groupBox1.Controls.Add(btnBrowseDir);
			groupBox1.Controls.Add(btnDeploy);
			
			CheckLogs();
		}
		
		private void CommitPush()
		{
			//string databaseVersion = GlobalFunction.GetConfigSettings("DBVersion");
			string dbVersionGit = sxDBVersion; //databaseVersion.Replace("v" , String.Empty);
			string gitRepoDir = (GlobalFunction.GetConfigSettings("ScriptsDir")).Replace("%DBName%", "v" + sxDBVersion);
			gitRepoDir = Directory.GetParent(gitRepoDir).ToString();
			
			Label lblCommitMessage = new Label();
			txtCommitMessage = new TextBox();
			
			Label lblGitRepo = new Label();
			txtGitRepo = new TextBox();
			
			btnCommit = new System.Windows.Forms.Button();
			System.Windows.Forms.Button btnBrowseDir = new System.Windows.Forms.Button();
			
			lblCommitMessage.Text = "Commit &message";
			lblCommitMessage.Location = new Point(5,30);
			lblCommitMessage.AutoSize = true;
			lblCommitMessage.UseMnemonic = true;
			
			CreateContextMenu();
			
			txtCommitMessage.Location = new Point(7, 50);
			txtCommitMessage.BorderStyle = BorderStyle.FixedSingle;
			txtCommitMessage.Size = new Size(250, 20);
			//txtCommitMessage.ContextMenu = conMenu;
			
			lblGitRepo.Text = "&Git repository";
			lblGitRepo.Location = new Point(5,75);
			lblGitRepo.AutoSize = true;
			lblGitRepo.UseMnemonic = true;
			
			txtGitRepo.Location = new Point(7, 95);
			txtGitRepo.BorderStyle = BorderStyle.FixedSingle;
			txtGitRepo.Size = new Size(200, 20);
			txtGitRepo.Text = gitRepoDir;
			
			btnCommit.Location = new System.Drawing.Point(80, 134);
			btnCommit.Size = new System.Drawing.Size(110, 24);
			btnCommit.Text = "&Commit && Push";
			btnCommit.Click += new EventHandler(btnCommitPush_Click);
			btnCommit.Enabled = true;
			
			btnBrowseDir.Location = new System.Drawing.Point(212, 93);
			btnBrowseDir.Size = new System.Drawing.Size(50, 21);
			btnBrowseDir.Text = "&Browse";
			btnBrowseDir.Click += new EventHandler(browseRepo_Click);
			btnBrowseDir.Enabled = true;
			
			if (IsCommit(gitRepoDir))
			{
				btnCommit.Enabled = false;
			}
            
			groupBox1.Controls.Clear();
			groupBox1.Text = "Git Commit til version_" + dbVersionGit;
			groupBox1.Controls.Add(lblCommitMessage);
			groupBox1.Controls.Add(txtCommitMessage);
			groupBox1.Controls.Add(lblGitRepo);
			groupBox1.Controls.Add(txtGitRepo);
			groupBox1.Controls.Add(btnCommit);
			groupBox1.Controls.Add(btnBrowseDir);
			//List<string> untrackedFiles = GetUntrackedFiles();
			txtCommitMessage.Focus();
			CheckLogs();
		}
		
		private void MenuItemNew_Click(Object sender, System.EventArgs e)
		{
			System.Windows.Forms.MenuItem mi = sender as System.Windows.Forms.MenuItem;
		    txtCommitMessage.Text = mi.Text;
		}
		
		private async void btnDeploy_Click(object sender, EventArgs e)
		{
			string sourceFile = txtFileSource.Text;
			//bool entryValid = false;
			if (sourceFile.IsEmpty())
			{
				GlobalFunction.ShowMessage("Du må velge kildefil!", MessageBoxIcon.Error);
			}
			else
			{
				if (!File.Exists(sourceFile))
				{
					GlobalFunction.ShowMessage("Kan ikke finne denne filen (" + sourceFile + ")", MessageBoxIcon.Error);
				}
				else
				{
		            FileInfo srcFile = new FileInfo(sourceFile);
		            if (!Directory.Exists(txtGitRepo.Text))
		            {
		            	GlobalFunction.ShowMessage("Kan ikke finne denne mappen (" + txtGitRepo.Text + ")", MessageBoxIcon.Error);
		            }
		            else
		            {
			            string [] dirs = Directory.GetDirectories(txtGitRepo.Text, "TS_*");
			            if (dirs.Length == 0)
			            {
			            	GlobalFunction.ShowMessage("Finner ikke skripter i denne mappen (" + txtGitRepo.Text + ")", MessageBoxIcon.Error);
			            }
			            else
			            {
			            	//this.tmrClock2.Tick+= new EventHandler(tmrClock2_Tick);
				            foreach (string dir in dirs) 
				            {
								string [] subdirs = Directory.GetDirectories(dir, "TC_*");
								if (subdirs.Length == 0)
					            {
					            	GlobalFunction.ShowMessage("Finner ikke skripter i denne mappen (" + txtGitRepo.Text + ")", MessageBoxIcon.Error);
					            	break;
					            }
								else
								{
									foreach (string subdir in subdirs)
									{
										btnDeploy.Text = "Vent...";
										btnDeploy.Enabled = false;
										lblInfoMsg.Text = "Info: Kopierer fil til " + subdir + "...";
										lblInfoMsg.ForeColor = Color.Empty;
										srcFile.CopyTo(subdir + @"\" + Path.GetFileName(sourceFile), true);
										await System.Threading.Tasks.Task.Delay(500);
										/*this.tmrClock2.Enabled = true;
										this.tmrClock2.Interval = 1000;
										this.tmrClock2.Start();*/
									}
									btnDeploy.Text = "&Deploy File";
									btnDeploy.Enabled = true;
									lblInfoMsg.Text = "Info: Ferdig...";
								}
								
				           	}
				           /* if (entryValid)
				            {
					            //Delay.Seconds(3);
					            // Timer: Delay Label
								lblInfoMsg.Text = "Info: Deploying " + Path.GetFileName(sourceFile) + "...";
								
				            }*/
			            }
		            }
				}
			}
		}
		
		private void btnCommitPush_Click(object sender, EventArgs e)
		{
			string gitRepoDir = txtGitRepo.Text;
			if (IsCommit(gitRepoDir))
			{
				btnCommit.Enabled = false;
			}
			else
			{
				if (txtCommitMessage.Text == String.Empty)
				{
					lblInfoMsg.Text = "Feil: Commit message må ikke være tomt.";
					txtCommitMessage.Focus();
				}
				else
				{
					btnCommit.Text = "Vent...";
					lblInfoMsg.Text = "Info: Commit/push skripter til Github... Vennligst vent!";
					lblInfoMsg.ForeColor = Color.Empty;
					this.btnCommit.Enabled = false;
					string currentToolDirectory = Directory.GetCurrentDirectory().ToString();
					string dbVersionGit = sxDBVersion;
					int exitCode = 0;
					string resMsg = "Info: Ferdig";
					Color colorDefault = Color.Empty;
					Directory.SetCurrentDirectory(txtGitRepo.Text);
					
					exitCode = GlobalFunction.RunCommandProcess("git checkout version_" + dbVersionGit);
					if (exitCode == 128)
					{
						resMsg = "Feil: Commit/Push encounters fatal error";
						colorDefault = Color.Firebrick;
					}
					else
					{
						exitCode = GlobalFunction.RunCommandProcess("git add .");
						exitCode = GlobalFunction.RunCommandProcess("git commit -m \"" + txtCommitMessage.Text + " (Git Commit Source: Tool)\"");
						exitCode = GlobalFunction.RunCommandProcess("git push --progress \"origin\" version_" + dbVersionGit + ":version_" + dbVersionGit);
						
						
						if (exitCode == 1)
						{
							resMsg = "Feil: Pathspec 'version_x" + dbVersionGit + "' did not match any file known to git.";
							colorDefault = Color.Firebrick;
						}
						
						Directory.SetCurrentDirectory(currentToolDirectory);
						LogGitMessage(txtCommitMessage.Text);
						CreateContextMenu();
						
						List<string> untrackedFiles = GetUntrackedFiles();
						if (untrackedFiles.Count > 0)
						{
							foreach (var list in untrackedFiles)
							{
								string[] arrList = list.Split(':');
								MySQLConnector.SetScriptCommit(arrList[0], arrList[1], this.txtCommitMessage.Text);
							}
						}
					}
					this.btnCommit.Enabled = true;
					btnCommit.Text = "&Commit && Push";
					lblInfoMsg.ForeColor = colorDefault;
					lblInfoMsg.Text = resMsg;
				}
			}
		}
		
		public static string GetAppInfo(string strInfo)
		{
			System.Reflection.Assembly assemblyInfo = System.Reflection.Assembly.GetExecutingAssembly();
			string resultInfo = "";
			switch (strInfo)
			{
				case "Version":
					FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assemblyInfo.Location);
					resultInfo = fileVersionInfo.FileVersion;
					break;
				case "BuildDate":
					FileInfo fileInfo = new FileInfo(assemblyInfo.Location);
					resultInfo = String.Format("{0:yyyy-MM-dd}", fileInfo.LastWriteTime);
					break;
			}
			
			return resultInfo;
		}
		
		private void btnBrowseFileSource_Click(object sender, System.EventArgs e)
		{
			ofdBrowseSource.Filter = "Filer | *.*";
			this.ofdBrowseSource.Title = "Velg kildefil:";
			ofdBrowseSource.InitialDirectory = Directory.GetCurrentDirectory();
			DialogResult result = ofdBrowseSource.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				txtFileSource.Text = ofdBrowseSource.FileName;
			}
		}
		
		private void browseRepo_Click(object sender, System.EventArgs e)
		{
			this.folderBrowserDialog1.Description = "Velg en GIT repo mappe:";
			folderBrowserDialog1.SelectedPath = txtGitRepo.Text;
			DialogResult result = folderBrowserDialog1.ShowDialog();
			btnCommit = new System.Windows.Forms.Button();
		    if (result == DialogResult.OK)
		    {
				txtGitRepo.Text = folderBrowserDialog1.SelectedPath;
				btnCommit.Enabled = true;
				if (IsCommit(txtGitRepo.Text))
				{
					btnCommit.Enabled = false;
				}
		    }
		}
		
		private void browseScriptSource_Click(object sender, System.EventArgs e)
		{
			this.folderBrowserDialog1.Description = "Velg skriptkilde mappen:";
			folderBrowserDialog1.SelectedPath = txtGitRepo.Text;
			DialogResult result = folderBrowserDialog1.ShowDialog();
			
		    if (result == DialogResult.OK)
		    {
				txtGitRepo.Text = folderBrowserDialog1.SelectedPath;
			
		    }
		}
		
		private void LogGitMessage(string gitMessage)
	    {
			int countItemList = File.ReadAllLines(menuItemsSource).Length;
			if (countItemList == 10)
			{
				RemoveOldMessage();
			}
			if (!IsGitMessageExist(gitMessage))
			{
				using (StreamWriter w = File.AppendText(menuItemsSource))
		        {
					w.WriteLine(gitMessage);
					w.Close();
		        }
			}
	    }
		
		private void RemoveOldMessage()
		{
			var file = new List<string>(System.IO.File.ReadAllLines(menuItemsSource));
	        file.RemoveAt(0);
	        File.WriteAllLines(menuItemsSource, file.ToArray());
		}
		
		private bool IsGitMessageExist(string gitMessage)
        {
        	var lists = System.IO.File.ReadAllLines(menuItemsSource);
            bool response = false;
            foreach (var list in lists) 
            {
            	if (gitMessage == list)
            	{
            		response = true;
            		break;
            	}
			}
            return response;
        }
		
		private void ClearList_Click(Object sender, System.EventArgs e)
		{
			File.WriteAllText(menuItemsSource, string.Empty);
			CreateContextMenu();
		}
		
		private void CreateContextMenu()
		{
			System.Windows.Forms.ContextMenu conMenu = new System.Windows.Forms.ContextMenu();
			string[] lines = new string[] {};
			try
			{
				lines = File.ReadAllLines(menuItemsSource);
				foreach (string line in lines)
				{
					conMenu.MenuItems.Add(new System.Windows.Forms.MenuItem(line, MenuItemNew_Click));
				}
			}
			catch
			{
				File.Create(menuItemsSource);
			}
			
			
			conMenu.MenuItems.Add("-");
			conMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("&Tøm nylig liste", ClearList_Click));
			txtCommitMessage.ContextMenu = conMenu;
		}
		
		private bool GetGitStatus()
        {
			string gitLog = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GitStatus.log";
        	/*var lists = System.IO.File.ReadAllLines(gitLog);
            bool response = true;
            foreach (var list in lists) 
            {
            	int contentLength = list.Length;
            	//if (list.Contains("tgitconfig??"))
            	if (contentLength > 14)
            	{
            		response = false;
            		break;
            	}
			}*/
        	string logContent = File.ReadAllText(gitLog);
        	return (logContent.Contains("working tree clean"));
        }
		
		private List<string> GetUntrackedFiles()
		{
			string gitLog = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GitStatus.log";
        	var lists = System.IO.File.ReadAllLines(gitLog);
        	List<string> scripts = new List<string>();
            foreach (var list in lists) 
            {
        		if (list.Contains("TS_"))
        		{
            		string[] arrList = list.Split('_');
            		int arrListPos = 0;
            		for (int x = 0; x<arrList.Length; x++)
            		{
            			if (arrList[x].Contains("/TC"))
            			{
            				arrListPos = x + 1;
            			}
            			else if (arrList[x].Contains("Masterscript"))
            			{
            				arrListPos = x;
            			}
            		}
            		if (!scripts.Contains(arrList[1] + ":" + arrList[arrListPos]))
            		{
            			if (arrList[arrListPos].Contains("Masterscript"))
            			{
            				arrList[arrListPos] = "XXX";
            			}
            			scripts.Add(arrList[1] + ":" + arrList[arrListPos]);
            		}
        		}
			}
            return scripts;
		}
		
		private void MainFormClosing(Object sender, FormClosingEventArgs e)
		{
			MySQLConnector.SetUserStatus(lastLoginID, true);
		}
	}
}
