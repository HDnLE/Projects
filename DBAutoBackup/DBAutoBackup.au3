#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Icon=hovemedical.ico
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****

;Backup all databases
RunWait(@ComSpec & " /c " & '"C:\Program Files\MySQL\MySQL Server 5.6\bin\mysqldump.exe" -h 192.168.10.61 -u ta_admin -pdhocc648 --all-databases > C:\Scripts\AutomaticTesting\Database\AutoX-DB.sql', "", @SW_HIDE);

;Commit and push to repository
FileChangeDir("C:\Scripts\AutomaticTesting\Database")
RunWait(@ComSpec & " /c " & 'git add .', "", @SW_HIDE)
RunWait(@ComSpec & " /c " & 'git commit -m "TE-8: Updated data -- Backup triggered by Windows Task Scheduler"', "", @SW_HIDE)
RunWait(@ComSpec & " /c " & 'git push origin master', "", @SW_HIDE)

;Backup single database (SX 4.1)
_SQLBackup("4.1", "3")

;Backup single database (SX 5.1)
_SQLBackup("5.1", "22")

;Backup single database (SX 5.2)
_SQLBackup("5.2", "29")

MsgBox(64, "Database Auto-Backup", "Auto-backup complete...", 5)

Func _SQLBackup($sxVersion, $jiraID)
	Local $dbName = StringReplace($sxVersion, ".", "")

	;Backup database
	Run(@ComSpec & " /c " & '"C:\Program Files\MySQL\MySQL Server 5.6\bin\mysqldump.exe" -h 192.168.10.61 -u ta_admin -pdhocc648 sxtest_' & $dbName & ' > C:\Scripts\v' & $sxVersion & '\AutomaticTesting\Database\ScriptsDB-Backup.sql', "", @SW_HIDE);

	;Change working directory
	FileChangeDir("C:\Scripts\v" & $sxVersion & "\AutomaticTesting\Database")

	;Commit and push to repository
	RunWait(@ComSpec & " /c " & 'git checkout version_' & $sxVersion, "", @SW_HIDE)
	RunWait(@ComSpec & " /c " & 'git add .', "", @SW_HIDE)
	RunWait(@ComSpec & " /c " & 'git commit -m "TE-' & $jiraID & ': Updated data -- Backup triggered by Windows Task Scheduler"', "", @SW_HIDE)
	RunWait(@ComSpec & " /c " & 'git push origin version_' & $sxVersion & ':version_' & $sxVersion, "", @SW_HIDE);
EndFunc