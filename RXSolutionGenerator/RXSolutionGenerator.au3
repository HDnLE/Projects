#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Icon=hovemedical.ico
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****

#cs ----------------------------------------------------------------------------

 AutoIt Version: 3.3.14.1
 Author:         Rommel A. Lamanilao (rommel@hovemdical.no)

 Script Function:
	To rename TestTemplate files and its content.

#ce ----------------------------------------------------------------------------

#include <File.au3>
#include <Array.au3>
#include <FileConstants.au3>
#include <GUIConstantsEx.au3>
#include <EditConstants.au3>
#include <ButtonConstants.au3>
#include <WindowsConstants.au3>
#include <ColorConstants.au3>
#include <guiconstants.au3>
#include <MySQL.au3>
#include <Date.au3>

Global Const $strTemplate = "TestTemplate"
FileInstall("RXLogo.jpg", @TempDir & "\temp_r.jpg")

Global Const $mainTitle = "RX Solution Generator"

; Complete path of the application config file
Global $iniFile = @ScriptDir & "\RXSolutionGenerator.ini"

; Get the current database name from the config file
$dbName = IniRead($iniFile, "Database", "DBName", "sxtest_52")

; Get the current database hostname from the config file
$dbHost = IniRead($iniFile, "Database", "DBHost", "UTV-SXTEST1")

; Main form
Local $hGUI = GUICreate($mainTitle, 450, 190, -1, -1, BitXOR($GUI_SS_DEFAULT_GUI, $WS_MINIMIZEBOX))

; RX solution path
Local $lblPath = GUICtrlCreateLabel("Complete path of the TestTemplate RX solution",10,10,285,25)
Local $txtPath = GUICtrlCreateInput("", 10, 26, 270, 20)
GUICtrlSetTip($txtPath, "Press Ctrl+B to select path of the TestTemplate RX solution")
Local $btnBrowse = GUICtrlCreateButton("&Browse", 280, 25, 40, 22)
GUICtrlSetTip($btnBrowse, "Select path of the TestTemplate RX solution")

; Testsuite ID
Local $lblTSID = GUICtrlCreateLabel("Testsuite ID",10,65,100,25)
Local $txtTSID = GUICtrlCreateInput("", 80, 57, 140, 20, $ES_NUMBER)
GUICtrlSetTip($txtTSID, "Type TS-ID and press Ctrl+ENTER to retrieve the testsuite title")
Local $iENTER = GUICtrlCreateDummy()
Dim $AccelKeys[2][2] = [["^{ENTER}", $iENTER],["^b", $btnBrowse]]

; Testsuite title
Local $lblTitle = GUICtrlCreateLabel("Testsuite Title",10,95,100,25)
Local $txtTitle = GUICtrlCreateInput("", 80, 87, 240, 20, $ES_READONLY)

; Database source
Local $lblDbSource = GUICtrlCreateLabel("DB Host/Name Source: " & $dbHost & "/" & $dbName,10,115,285,25)

; Confirm and Cancel buttons
Global $btnConfirm = GUICtrlCreateButton("&Confirm", 125, 130, 100, 35)
GUICtrlSetState ($btnConfirm,$GUI_DISABLE)
Local $btnCancel = GUICtrlCreateButton("E&xit", 235, 130, 100, 35)

Local $clockFakeLabel = GUICtrlCreateInput('', 338, 169, 110, 20, BitOR($SS_SUNKEN , $BS_CENTER, $SS_CENTER, $ES_READONLY))
Local $clockLabel = GUICtrlCreateLabel('', 338, 169, 110, 20, BitOR($SS_SUNKEN , $BS_CENTER, $SS_CENTER))
; Ranorex logo
Local $picMain = GUICtrlCreatePic(@TempDir & "\temp_r.jpg", 330, 18, 140, 100)

Local $lblInfo = GUICtrlCreateLabel(" Ready",1,169,335,20, BitOR($SS_SUNKEN , $SS_LEFT, $BS_CENTER))

GUICtrlSetColor($clockLabel, 0x848484)
GUICtrlSetColor($lblInfo, 0x848484)
Local Const $sMessage = "Select a folder"

GUISetAccelerators($AccelKeys)
GUICtrlSetState ($clockFakeLabel,$GUI_DISABLE)
GUISetState(@SW_SHOW, $hGUI)
Local $SEC = 99

While 1
	If @SEC <> $SEC Then
		GUICtrlSetData($clockFakeLabel, _NowDate() & " " & @HOUR & ":" & @MIN & ":" & @SEC & " ")
		$SEC = @SEC
	EndIf
	$msg = GUIGetMsg()
	Local $errorCodeDir = 0
	Global $strPath = GUICtrlRead($txtPath)
	Global $tsID = GUICtrlRead($txtTSID)
	Global $tsTitle = GUICtrlRead($txtTitle)

	If Not _IsODBCConnectorInstalled() Then
		_ShowStatusMessage(64, " ERROR: MySQL ODBC Connector is not installed!")
		GUICtrlSetState ($txtTSID,$GUI_DISABLE)
		GUICtrlSetState ($txtPath,$GUI_DISABLE)
		GUICtrlSetState ($btnBrowse,$GUI_DISABLE)
	EndIf

	Switch $msg
		Case $GUI_EVENT_CLOSE, $btnCancel
			ExitLoop
		Case $btnBrowse
			Local $defaultFolder = (FileExists($strPath)) ? $strPath : @WorkingDir
			Local $sFileSelectFolder = FileSelectFolder($sMessage, $defaultFolder)
			Local $arrScriptFiles = _FileListToArray($sFileSelectFolder, $strTemplate & "*.rxsln", 1)

			$errorCodeDir = @error
			If ($errorCodeDir == 4 And $sFileSelectFolder <> "") Then
				_ShowStatusMessage(64, " ERROR: Selected folder does not contain template Ranorex solution!")
				GUICtrlSetData($txtPath, "")
			ElseIf ($errorCodeDir <> 4) Then
				GUICtrlSetState($txtTSID, $GUI_FOCUS)
				GUICtrlSetData($txtPath, $sFileSelectFolder)
				_ShowStatusMessage(0, " Ready")
				If ($tsID <> "" And $tsTitle <> "") Then
					GUICtrlSetState ($btnConfirm,$GUI_ENABLE)
				EndIf
			EndIf
		Case $iENTER
			Local $tsID = GUICtrlRead($txtTSID)
			Local $testsuiteTitle = _GetTestsuiteTitle($tsID)
			GUICtrlSetData($txtTitle, $testsuiteTitle)
			If ($testsuiteTitle <> "" And GUICtrlRead($txtPath) <> "") Then
				GUICtrlSetState ($btnConfirm,$GUI_ENABLE)
				GUICtrlSetState($btnConfirm, $GUI_FOCUS)
			Else

			EndIf
		Case $btnConfirm
			Local $newName = "TS_" & $tsID & "_" & StringReplace($tsTitle, " ", "_")
			$newName = StringReplace($newName, "-", "_")
			Local $arrScriptFiles = _FileListToArray($strPath, $strTemplate & "*.rxsln", 1)
			$errorCodeDir = @error

			$tsTitle = _GetTestsuiteTitle($tsID)
			If ($tsTitle == "") Then
				$errorCodeDir = 99
				GUICtrlSetData($txtTitle, "")
			EndIf
			Local $checkDirStatus = _DirectoryStatus($errorCodeDir)
			If ($checkDirStatus == "OK") Then
				Local $newDirectory = StringLeft($strPath, StringInStr($strPath, '\', 0, -1) - 1)
				Global $scriptPath = $newDirectory & "\" & $newName

				$ok = True

				;Copy TestTemplate folder
				If (FileExists($scriptPath)) Then
					$overwrite = MsgBox(68, $mainTitle, "Script already exist! Overwrite?")
					$ok = ($overwrite == 6)
				EndIf

				If ($ok) Then
					_ShowStatusMessage(0, " Generating... Please wait!")
					DirRemove($scriptPath, 1)
					DirCopy($strPath, $scriptPath, 1)

					;Rename TestTemplate file
					Local $arrFiles = _FileListToArray($scriptPath, $strTemplate & "*", 1)
					_RenameFile($arrFiles, $newName)

					;Replace contents of file $BS_DEFPUSHBUTTON
					_ReplaceTemplateContent(_FileListToArray($scriptPath, "*", 1), $newName)

					;Notify user once it is complete
					;MsgBox(64, $mainTitle, "SUCCESS: All files and its content have been successfully modified.")
					;GUICtrlSetData($lblInfo, " SUCCESS: All files and its content have been successfully modified.")
					_ShowStatusMessage(16, " SUCCESS: All files and its content have been successfully modified.")
				EndIf
			EndIf
	EndSwitch

WEnd

GUIDelete($hGUI)


Func _DirectoryStatus($errorCode)
	Local $status = "OK"
	If ($errorCode == 1 Or Not FileExists($strPath)) And $strPath <> "" Then
		;MsgBox(16, $mainTitle, "ERROR: Path was invalid.")
		$status = " ERROR: Path was invalid!"
	EndIf
	If ($errorCode == 4 And $strPath <> "") Then
		;MsgBox(16, $mainTitle, "ERROR: This folder does not contain template Ranorex solution!")
		$status = " ERROR: Selected folder does not contain template Ranorex solution!"
	EndIf
	If ($errorCode == 99) Then
		$status = " ERROR: Testsuite ID not found!"
		GUICtrlSetState ($btnConfirm,$GUI_DISABLE)
	EndIf
;~ 	If ($tsID == "" Or $tsTitle == "" Or $strPath == "") And $errorFlag Then
;~ 		;MsgBox(16, $mainTitle, "ERROR: All fields are required!")
;~ 		$status = " ERROR: All fields are required!"
;~ 	EndIf
	Return $status
EndFunc

Func _RenameFile($arrFiles, $newName)
	For $i = 1 To UBound($arrFiles) - 1
		FileMove($scriptPath & "\" & $arrFiles[$i], $scriptPath & "\" & StringReplace($arrFiles[$i], $strTemplate, $newName))
	Next
EndFunc

Func _ReplaceTemplateContent($arrTemplateFiles, $strReplacement)
	For $i = 1 To UBound($arrTemplateFiles) - 1
		_ReplaceStringInFile($scriptPath & "\" & $arrTemplateFiles[$i], $strTemplate, $strReplacement)
	Next
EndFunc

Func _GetTestsuiteTitle($tsID)
	$dbUsername = IniRead($iniFile, "Database", "DBUsername", "ta_admin")
	$dbPassword = IniRead($iniFile, "Database", "DBPassword", "dhocc648")
	$dbName = IniRead($iniFile, "Database", "DBName", "sxtest_52")
	$dbHost = IniRead($iniFile, "Database", "DBHost", "192.168.10.61")
	$sql = _MySQLConnect($dbUsername,$dbPassword,$dbName,$dbHost)
	$var = _Query($sql,"SELECT TITLE FROM testsuites WHERE TS_ID = '" & $tsID & "'")
	$result = ""
	With $var
		While NOT .EOF
			$result = .Fields("TITLE").value
			.MoveNext
		WEnd
	EndWith
	If ($result == "") Then
		_ShowStatusMessage(64, " ERROR: Testsuite ID not found!")
		GUICtrlSetState ($btnConfirm,$GUI_DISABLE)
	Else
		_ShowStatusMessage(0, " Ready")
	EndIf
	_MySQLEnd($sql)
	Return $result
EndFunc

Func _ShowStatusMessage($flag, $statusMsg)
	$color = ($flag == 64) ? 0xB22222 : 0x848484
	$color = 0x848484
	If ($flag == 64) Then
		$color = 0xB22222
	ElseIf ($flag == 16) Then
		$color = 0x556B2F
	EndIf
	GUICtrlSetColor($lblInfo, $color)
	GUICtrlSetData($lblInfo, $statusMsg)
EndFunc

Func _IsODBCConnectorInstalled()
	Local $HKEY_LOCAL_MACHINE = 0x80000002
	Local $strComputer = "."
	Local $objRegistry = ObjGet("winmgmts:\\" & $strComputer & "\root\default:StdRegProv")
	Local $strKeyPath = "SOFTWARE\ODBC\ODBCINST.INI\ODBC Drivers"
	Local $arrValueNames, $arrValueTypes, $strValue
	$objRegistry.EnumValues($HKEY_LOCAL_MACHINE, $strKeyPath, $arrValueNames, $arrValueTypes)

	For $i = 0 to UBound($arrValueNames) - 1
		$strValueName = $arrValueNames[$i]
		$objRegistry.GetStringValue($HKEY_LOCAL_MACHINE,$strKeyPath,$strValueName,$strValue)
		Local $strODBC = $arrValueNames[$i]
		Local $response = False
		If $strODBC == "MySQL ODBC 3.51 Driver" Then
			$response = True
			ExitLoop
		EndIf
	Next
	Return $response
EndFunc