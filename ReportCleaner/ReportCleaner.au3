#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Icon=disk_cleaner_o5i_icon.ico
#AutoIt3Wrapper_Res_Fileversion=1.0.1.12
#AutoIt3Wrapper_Res_Fileversion_AutoIncrement=y
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#cs ----------------------------------------------------------------------------

 AutoIt Version: 3.3.14.1
 Author:         Rommel A. Lamanilao (rommel@hovemdical.no)

 Script Function:
	To delete all report logs.

#ce ----------------------------------------------------------------------------

; Script Start - Add your code below here
#include <MsgBoxConstants.au3>
#include <GUIConstantsEx.au3>
#include <Array.au3>
#include <ButtonConstants.au3>
#include <ColorConstants.au3>
#include <WindowsConstants.au3>
#include <guiconstants.au3>
#include <Date.au3>
#include <FileConstants.au3>

;#Include "_Dbug.au3"

#AutoIt3Wrapper_Res_Field = Timestamp|%date% %time%
If @Compiled Then
	$rcVersion = FileGetVersion(@ScriptFullPath, "FileVersion")
	$buildDate =  FileGetVersion(@ScriptFullPath, "Timestamp")
Else
	$rcVersion = "1.0.1.10"
	$buildDate = "16.10.2018 16:36:00"
EndIf

FileInstall("cleanmgr.jpg", @TempDir & "\temp.jpg")

Local $hGUI = GUICreate("Report Cleaner", 300, 175, -1, -1, BitXOR($GUI_SS_DEFAULT_GUI, $WS_MINIMIZEBOX))

Local $lblLabel = GUICtrlCreateLabel("Clean report logs from?",10,10,185,25)
Local $rbVersion41 = GUICtrlCreateRadio("System X &4.1",10,30,80,25)
Local $rbVersion51 = GUICtrlCreateRadio("System X 5.&1",10,50,80,25)
Local $rbVersion52 = GUICtrlCreateRadio("System X 5.&2",10,70,80,25)
Local $rbVersion52TR = GUICtrlCreateRadio("System X 5.&2 (TestRail)",10,90,130,25)
Local $btnCancel = GUICtrlCreateButton("Cancel", 150, 125, 85, 25)
Local $btnClean = GUICtrlCreateButton("&Clean", 60, 125, 85, 25, $BS_DEFPUSHBUTTON)
Local $lblInfo = GUICtrlCreateLabel(" Ready",1,154,190,20, BitOR($SS_SUNKEN , $SS_LEFT, $BS_CENTER))
Local $picMain = GUICtrlCreatePic(@TempDir & "\temp.jpg", 170, 3, 130, 90)
Local $clockLabel = GUICtrlCreateLabel('', 188, 154, 110, 20, BitOR($SS_SUNKEN , $BS_CENTER, $SS_CENTER))
Local $lblVersion = GUICtrlCreateLabel("v" & $rcVersion ,252,80,185,25)
Local $lblBuildDate = GUICtrlCreateLabel("Build date: " & $buildDate ,145,92,185,25)
GUICtrlSetBkColor($lblBuildDate, $GUI_BKCOLOR_TRANSPARENT)
GUICtrlSetBkColor($lblVersion, $GUI_BKCOLOR_TRANSPARENT)
GUICtrlSetColor($lblBuildDate, 0x777777)
GUICtrlSetColor($lblVersion, 0x777777)
If Not FileExists(@TempDir & "\ReportCleaner") Then
	DirCreate(@TempDir & "\ReportCleaner")
EndIf

Local $configFile = @TempDir & "\ReportCleaner\Config.INI"
If Not FileExists($configFile) Then
	IniWrite($configFile, "Settings", "LogSource", "4.1")
EndIf
Local $logSource = IniRead($configFile, "Settings", "LogSource", "4.1")
Local $rbOptionSelected

Switch $logSource
	Case "4.1"
		$rbOptionSelected = $rbVersion41
	Case "5.1"
		$rbOptionSelected = $rbVersion51
	Case "5.2"
		$rbOptionSelected = $rbVersion52
	Case "5.2_testrail"
		$rbOptionSelected = $rbVersion52TR
EndSwitch

GUICtrlSetState($rbOptionSelected, $GUI_CHECKED)
GUICtrlSetColor($lblInfo, 0x848484)
GUICtrlSetColor($clockLabel, 0x848484)
GUISetState(@SW_SHOW, $hGUI)

Local $srcDir = "C:\Development\Ranorex"
Local $version = ""
FileDelete(@TempDir & "\temp.jpg")
Local $SEC = 99
While 1
	Local $processCancel = True
	Switch GUIGetMsg()
		Case $GUI_EVENT_CLOSE, $btnCancel
                ExitLoop
		Case $btnClean
			If (GUICtrlRead($rbVersion41) == 1) Then
				$version = "4.1"
			EndIf
			If (GUICtrlRead($rbVersion51) == 1) Then
				$version = "5.1"
			EndIf
			If (GUICtrlRead($rbVersion52) == 1) Then
				$version = "5.2"
			EndIf
			If (GUICtrlRead($rbVersion52TR) == 1) Then
				$version = "5.2_testrail"
			EndIf
			$processCancel = False
			GUICtrlSetState($btnCancel, 128)
			GUICtrlSetState($btnClean, 128)
			GUICtrlSetState($rbVersion41, 128)
			GUICtrlSetState($rbVersion51, 128)
			GUICtrlSetState($rbVersion52, 128)

			Local $scriptDir = $srcDir & "\version_" & $version & "\Scripts"
			Local $runMasterscript = $scriptDir & "\RunMasterscripts"
			Local $ranorexLog = $srcDir & "\version_" & $version
			Local $utilityLog = $srcDir & "\version_" & $version & "\Scripts\Utility\Report"

			GUICtrlSetData($lblInfo, " Deleting report logs. Please wait...")
			IniWrite($configFile, "Settings", "LogSource", $version)

			CleanDirectory($scriptDir, 0)
			CleanDirectory($runMasterscript, 1)
			CleanDirectory($ranorexLog, 2)
			CleanDirectory($utilityLog, 3)

			Sleep(5000)
			;MsgBox(64, "Report Cleaner", "Report logs have been successfully deleted...")
			;GUICtrlSetColor($lblInfo, $COLOR_GREEN)
			GUICtrlSetData($lblInfo, " Done...")
			GUICtrlSetData($btnCancel, "Done")
			GUICtrlSetState($btnCancel, 64)
			GUICtrlSetState($btnClean, 64)
			GUICtrlSetState($rbVersion41, 64)
			GUICtrlSetState($rbVersion51, 64)
			GUICtrlSetState($rbVersion52, 64)
			GUICtrlSetState($btnCancel, 256)
	EndSwitch
	If $SEC = @SEC Then ContinueLoop
    $SEC = @SEC
	GUICtrlSetData($clockLabel, _NowDate() & " " & @HOUR & ":" & @MIN & ":" & @SEC & " ")
WEnd

GUIDelete($hGUI)


Func CleanDirectory($dirName, $type)
	Switch $type
		Case 0
			Local $arrDirectories = SearchFolder($dirName & "\TS*.")
			Local $iResult = 0
			If (UBound($arrDirectories) > 0) Then
				For $i = 0 To UBound($arrDirectories) - 1
					Local $arrMasterscripts = SearchFolder($dirName & "\" & $arrDirectories[$i] & "\Master*.")

					If (UBound($arrMasterscripts) > 0) Then
						FileDelete($dirName & "\" & $arrDirectories[$i] & "\" & $arrMasterscripts[0] & "\Reports\T*.htm")
					EndIf

					Local $arrScriptReports = SearchFolder($dirName & "\" & $arrDirectories[$i] & "\TC*.")

					If (UBound($arrScriptReports) > 0) Then
						For $m = 0 To UBound($arrScriptReports) - 1
							FileDelete($dirName & "\" & $arrDirectories[$i] & "\" & $arrScriptReports[$m] & "\Reports\T*.htm")
						Next
					EndIf
				Next
			EndIf
		Case 1
			FileDelete($dirName & "\Report\T*.*")
			FileDelete($dirName & "\Report\SystemX_ERRORLOG_EVENTS_2*.*")
			FileDelete($dirName & "\Report\SystemX_LOGFEIL_2*.*")
			FileDelete($dirName & "\RanorexReport.*")
			FileDelete($dirName & "\ReportsR*.*")
			;DirRemove($dirName & "\Report",1)
		Case 2
			FileDelete($dirName & "\R*.*")
		Case 3
			FileDelete($dirName & "\TestAutomationReport_2*.*")
			FileDelete($dirName & "\SystemX_ERRORLOG_EVENTS_2*.*")
			FileDelete($dirName & "\SystemX_LOGFEIL_2*.*")
	EndSwitch
EndFunc


Func SearchFolder($directory)
	Local $result[0] = []

	Local $hSearch = FileFindFirstFile($directory)

    ; Check if the search was successful, if not display a message and return False.
    If $hSearch = -1 Then
        ;MsgBox(4096, "", "Error: No files/directories matched the search pattern.")
        Return False
    EndIf

    ; Assign a Local variable the empty string which will contain the files names found.
    Local $sFileName = "", $iResult = 0

    While 1
        $sFileName = FileFindNextFile($hSearch)
        ; If there is no more file matching the search.
        If @error Then ExitLoop

        ; Display the file name.
        ;$iResult = MsgBox(BitOR(4096, 1), "", "File: " & $sFileName)
		_ArrayAdd($result,$sFileName)
        ;If $iResult <> $IDOK Then ExitLoop ; If the user clicks on the cancel/close button.
    WEnd

    ; Close the search handle.
    FileClose($hSearch)

	Return $result
EndFunc

Func addVerticalSeparator($x, $y, $h)
    GUICtrlCreateLabel("", $x, $y, 1, $h)
    GUICtrlSetBkColor(-1, 0x999999)
    GUICtrlCreateLabel("", $x + 1, $y, 2, 1)
    GUICtrlSetBkColor(-1, 0x999999)
    GUICtrlCreateLabel("", $x + 1, $y + 1, 2, $h)
    GUICtrlSetBkColor(-1, 0xffffff)

EndFunc