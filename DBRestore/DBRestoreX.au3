#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Icon=hovemedical.ico
#AutoIt3Wrapper_Change2CUI=y
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
; ================================================================================
; Title 			: 	DBRestore.au3
; AutoIt Version 	: 	3.3.14.1
; Language 			: 	English
; Description 		: 	To restore a backup database
; Application Type	:	Console
; Syntax			: 	DBRestore.exe -GBK <backupFile> -MDB <mdbFile>
; Author			: 	Rommel A. Lamanilao (rommel@hovemedical.no)
; ================================================================================

#include <File.au3>
#RequireAdmin

#cs
Global $backupFile = "C:\HMSDATA\Database\Backup\DBHMS.gbk"
Global $dbFile = "C:\HMSDATA\Database\Auto-Test\DBHMS.IB"

If Not _CheckFileExists($backupFile) Then
	Exit(1)
EndIf

If Not _CheckArguments() Then
	Exit(1)
EndIf

If $CmdLine[0] = 4 Then
	$backupFile = $CmdLine[2]
	$dbFile = $CmdLine[4]
EndIf
#ce

Local $sourceDB = "C:\HMSDATA\Database\Auto-Test\DBHMS-BASELINE-TRAINING-TESTS.IB"
Local $targetDB = "C:\HMSDATA\Database\Auto-Test\DBHMS.IB"

If Not _CheckFileExists($sourceDB) Then
	ConsoleWrite(@CRLF & "ERROR: Unable to locate source database (" & $sourceDB & "...)" & @CRLF)
	Exit(1)
EndIf

_StopService("IBS_gds_db")
FileDelete($targetDB)
ConsoleWrite(@CRLF & "Restoring baseline database. This will take some time depending on the size of the database..." & @CRLF)
Sleep(5000)

;Local $iReturn = RunWait(@ComSpec & " /c " & '"C:\Program Files\Embarcadero\Interbase\bin\gbak.exe" -c -r -user SYSDBA -password dhocc648 ' & $backupFile & ' ' & @ComputerName & ':' & $dbFile, "", @SW_HIDE)
FileCopy($sourceDB, $targetDB, 1)
Sleep(5000)
_StartService("IBS_gds_db")
#cs
While ($iReturn = 1 And $counter <= 10)
	Sleep(60000)
	$iReturn = RunWait(@ComSpec & " /c " & '"C:\Program Files\Embarcadero\Interbase\bin\gbak.exe" -c -r -user SYSDBA -password dhocc648 ' & $backupFile & ' ' & @ComputerName & ':' & $dbFile, "", @SW_HIDE)
	$counter = $counter + 1
WEnd
If ($iReturn = 1) Then
	ConsoleWrite("ERROR: Could not drop database " & $dbFile & " (Database might be in use) after 10 attempts...")
	Exit(1)
EndIf
Sleep(15000)
#ce

ConsoleWrite("Database restored successfully...")

; ===============================  START: USER-DEFINED FUNCTIONS =================================================
Func _CheckFileExists($pathFile)
	Local $result = True
	If Not FileExists($pathFile) Then
		ConsoleWrite("ERROR: Unable to locate this file (" & $pathFile & ")... Test will be ABORTED!" & @CRLF)
		$result = False
	EndIf
	Return $result
EndFunc

Func _CheckArguments()
	Local $result = True
	If $CmdLine[0] > 0 Then
		If $CmdLine[0] < 4 Then
			ConsoleWrite("ERROR: Missing arguments... Test will be ABORTED!")
			$result = False
		EndIf
		If $CmdLine[0] == 4 Then
			If $CmdLine[1] <> "-GBK" Then
				ConsoleWrite("ERROR: Invalid argument... Test will be ABORTED!")
				$result = False
			EndIf
			If $CmdLine[3] <> "-MDB" Then
				ConsoleWrite("ERROR: Invalid argument... Test will be ABORTED!")
				$result = False
			EndIf
			If $result Then
				$result = _CheckFileExists($CmdLine[2])
			EndIf
		EndIf
	EndIf
	Return $result
EndFunc

Func _StopService($serviceName)
	RunWait(@ComSpec & " /c " & 'C:\Windows\System32\NET STOP ' & $serviceName, "", @SW_HIDE)
	Local $winStatus = _GetServiceStatus($serviceName)
	Do
		Sleep(1000)
		$winStatus = _GetServiceStatus($serviceName)
	Until $winStatus == "STOPPED"
EndFunc

Func _StartService($serviceName)
	RunWait(@ComSpec & " /c " & 'C:\Windows\System32\NET START ' & $serviceName, "", @SW_HIDE)
	Do
		Sleep(2000)
		$winStatus = _GetServiceStatus($serviceName)
	Until $winStatus == "RUNNING"
EndFunc

Func _RestartService($serviceName)
	RunWait(@ComSpec & " /c " & 'C:\Windows\System32\NET STOP ' & $serviceName, "", @SW_HIDE)
	Local $winStatus = _GetServiceStatus($serviceName)
	Do
		Sleep(1000)
		$winStatus = _GetServiceStatus($serviceName)
	Until $winStatus == "STOPPED"
	;ProcessClose("SystemXBackend.exe")
	Sleep(5000)
	RunWait(@ComSpec & " /c " & 'C:\Windows\System32\NET START ' & $serviceName, "", @SW_HIDE)
	Do
		Sleep(2000)
		$winStatus = _GetServiceStatus($serviceName)
	Until $winStatus == "RUNNING"
	#cs
	While $winStatus == "STOPPED"
		Sleep(5000)
		RunWait(@ComSpec & " /c " & 'C:\Windows\System32\NET START SystemXBackend', "", @SW_HIDE)
		$winStatus = _GetServiceStatus()
	WEnd
	#CE
EndFunc

Func _GetServiceStatus($serviceName)
	Sleep(5000)
	RunWait(@ComSpec & " /c " & 'C:\Windows\System32\SC query ' & $serviceName & ' > ' & @AppDataDir & '\ServiceStatus.log', "", @SW_HIDE)
	Local $text = ""
	Dim $array
	$file = @AppDataDir & "\ServiceStatus.log"
	_FileReadToArray($file, $array)
	Local $searchText = "STATE"
	For $i = 1 To UBound($array) - 1
		If StringInStr($array[$i],$searchText) Then
			$text = $array[$i]
			ExitLoop
		EndIf
	Next
	Return _GetStatusText($text)
EndFunc

Func _GetStatusText($text)
	Local $arrText1 = StringSplit($text, ":")
	Local $arrText2 = StringSplit($arrText1[2], " ")
	Return $arrText2[4]
EndFunc
; ===============================  END: USER-DEFINED FUNCTIONS   =================================================