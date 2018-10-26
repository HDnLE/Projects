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

namespace Splash
{	
	public class WinSplash: System.Windows.Forms.Form
	{
		private System.Windows.Forms.Timer tmrClock;
		private PictureBox splashImg;
		private PictureBox loadingImg;
		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.Label lblVersion;
		
		public WinSplash()
		{
			tmrClock = new Timer();
			tmrClock.Interval = 3000;
			tmrClock.Start();
     		tmrClock.Tick += tmr_Tick;
     		
     		ResourceManager resMgr = new ResourceManager("SystemXTest.Images", Assembly.GetExecutingAssembly());
     		
     		this.splashImg = new PictureBox();
     		this.splashImg.Location = new System.Drawing.Point(0, 0);
     		this.splashImg.Image = (Image)resMgr.GetObject("Splash");
			this.splashImg.Size = new System.Drawing.Size(250,120);
			
			this.loadingImg = new PictureBox();
     		this.loadingImg.Location = new System.Drawing.Point(60, 90);
     		this.loadingImg.Image = (Image)resMgr.GetObject("Loading");
			this.loadingImg.Size = new System.Drawing.Size(128,10);
			
			this.lblInfo = new System.Windows.Forms.Label();
			this.lblInfo.Location = new System.Drawing.Point(35, 75);
			this.lblInfo.AutoSize = true;
			this.lblInfo.Font = new System.Drawing.Font(lblInfo.Font, System.Drawing.FontStyle.Bold);
			this.lblInfo.TabIndex = 0;
			this.lblInfo.Text = "Sjekker configfil. Vennligst vent...";
			this.lblInfo.BackColor = System.Drawing.Color.FromArgb(186,202,224);
			this.lblInfo.ForeColor = System.Drawing.Color.OrangeRed;
			
			this.lblVersion = new System.Windows.Forms.Label();
			this.lblVersion.Location = new System.Drawing.Point(90, 48);
			this.lblVersion.AutoSize = true;;
			this.lblVersion.TabIndex = 0;
			this.lblVersion.Text = "Build: " + SystemXTest.WinComponent.GetAppInfo("Version");
			//this.lblVersion.BackColor = System.Drawing.Color.White;
			this.lblVersion.BackColor = System.Drawing.Color.FromArgb(39,151,233);
			this.lblVersion.ForeColor = System.Drawing.Color.Gold;
			this.lblVersion.TextAlign = ContentAlignment.BottomRight;
     		
		    this.StartPosition = FormStartPosition.CenterScreen;
		    this.ClientSize = new System.Drawing.Size(250, 120);
			this.FormBorderStyle = FormBorderStyle.None;
			this.Icon = (Icon)resMgr.GetObject("HMS");
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
			                       	this.loadingImg,
			                       	this.lblInfo,
			                       	this.lblVersion,
			                       	this.splashImg
			                       });
		}
		
		
		private void tmr_Tick(object sender, EventArgs e)
		{
		    tmrClock.Stop();
	     	this.Close();
		 }
		
	}
}
