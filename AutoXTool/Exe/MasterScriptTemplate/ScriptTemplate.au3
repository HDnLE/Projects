#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Change2CUI=y
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#cs ----------------------------------------------------------------------------

 AutoIt Version: 3.3.14.1
 Author:         Rommel A. Lamanilao (rommel@hovemedical.no)

 Script Function:
	%ScriptTemplate%

#ce ----------------------------------------------------------------------------

#include <File.au3>
#include <Date.au3>
#include <MySQL.au3>

Global $masterReportFile = @ScriptDir & "\Reports\TestSuite%TestID%SummaryReport.htm"
Global $iHours = 0, $iMins = 0, $iSecs = 0, $totalScriptCount = %ScriptCount%

Func _RunScript($testsuite, $script, $fullScriptName)
	Local $hTimer = TimerInit()
	Local $result = 0
	FileChangeDir(@ScriptDir)
	FileChangeDir("..\")
	FileCreateShortcut(@WorkingDir & "\" & $script & "\" & $script & ".exe", @TempDir & "\" & $script, @WorkingDir & "\" & $script & "\")
	RunWait(@ComSpec & " /C " & @TempDir & "\" & $script & ".lnk")
	Local $fDiff = TimerDiff($hTimer)
	_TicksToTime($fDiff, $iHours, $iMins, $iSecs)
	Local $duration = StringFormat("%02d:%02d:%02d", $iHours, $iMins, $iSecs)
	$result = _WriteScriptStatus($fullScriptName, $script, $duration)
	Return $result
EndFunc

Func _WriteReportHeaderInfo()
	;Copy master report template
	FileCopy(@ScriptDir & "\Reports\master_report_template.htm", $masterReportFile,1)

	;Write date and time the script has started
	_UpdateHeaderInfo("%ExecutionTime%", _Now())

	;Write OS version
	_UpdateHeaderInfo("%OperatingSystem%", _OSVersion())

	;Write Computer name
	_UpdateHeaderInfo("%ComputerName%", @ComputerName)

	;Write System X Version
	_UpdateHeaderInfo("%SXVersion%", "%SysXVersion%")
EndFunc

Func _UpdateHeaderInfo($info, $value)
	_ReplaceStringInFile($masterReportFile,$info,$value)
EndFunc

Func _OSVersion()
	Local $regPath = "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion"
	Local $productName = RegRead($regPath, "ProductName")
	Local $csdVersion = RegRead($regPath, "CSDVersion")
	Local $osArch = (@OSArch == "X86") ? "32-Bit" : "64-Bit"
	Return $productName & " " & $csdVersion & " " & $osArch
EndFunc

Func _WriteScriptStatus($fullScriptName, $scriptName, $duration)
	Local $hFileOpen = FileOpen($masterReportFile, $FO_APPEND)
	Local $errorCount = _ReadTextFromLog(@AppDataDir & "\script_error.log")
	Local $sLogfile =  _ReadTextFromLog(@AppDataDir & "\logfilename.log")
	Local $result = "Feil"
	Local $comments = "Vennligst sjekk individuelle loggrapporten for detaljer"
	Local $passed_script_count = 0

	If ($errorCount == "0") Then
		$result = "OK"
		$comments = ""
		$passed_script_count = 1
	EndIf

	FileWriteLine($hFileOpen, @TAB & @TAB & @TAB & @TAB & @TAB & "<tr>")
	FileWriteLine($hFileOpen, @TAB & @TAB & @TAB & @TAB & @TAB & @TAB & '<td><a href="..\..\' & $scriptName & "\" & $sLogfile & '" target="_blank">' & $fullScriptName & '</a></td>')
	FileWriteLine($hFileOpen, @TAB & @TAB & @TAB & @TAB & @TAB & @TAB & "<td>" & $duration & "</td>")
	FileWriteLine($hFileOpen, @TAB & @TAB & @TAB & @TAB & @TAB & @TAB & "<td class='bold " & StringLower($result) & "'>" & $result & "</td>")
	FileWriteLine($hFileOpen, @TAB & @TAB & @TAB & @TAB & @TAB & @TAB & "<td>" & $comments & "</td>")
	FileWriteLine($hFileOpen, @TAB & @TAB & @TAB & @TAB & @TAB & "</tr>")
	FileClose($hFileOpen)

	Return $passed_script_count
EndFunc

Func _WriteToFile($file, $content)
	Local $hFileOpen = FileOpen($file, 2)
	FileWrite($hFileOpen, $content)
	FileClose($hFileOpen)
EndFunc

Func _ReadTextFromLog($logfileName)
	Local $logFile = FileOpen($logfileName, $FO_READ)
	Local $content = FileRead($logFile)
	FileClose($logFile)
	Return $content
EndFunc

Func _WriteEndTag()
	Local $hFileOpen = FileOpen($masterReportFile, $FO_APPEND)
	FileWriteLine($hFileOpen, @TAB & @TAB & @TAB & @TAB & "</tbody>")
	FileWriteLine($hFileOpen, @TAB & @TAB & @TAB & "</table>")
	FileWriteLine($hFileOpen, @TAB & @TAB & "</div>")
	FileWriteLine($hFileOpen, @TAB & "</body>")
	FileWriteLine($hFileOpen, "</html>")
	FileClose($hFileOpen)
EndFunc

Local $scriptCount = 0

;Write test report header info
_WriteReportHeaderInfo()

;Write Testsuite ID in the header
_UpdateHeaderInfo("%TSID%", "%TestsuiteID%")

;Write Testsuite Title in the header
_UpdateHeaderInfo("%TSTitle%", "%TestsuiteTitle%")

;Write Script designer in the header
_UpdateHeaderInfo("%Designer%", "%ScriptDesigner%")

Local $sTimer = TimerInit()

