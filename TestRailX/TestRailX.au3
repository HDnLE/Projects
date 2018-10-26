#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Icon=hovemedical.ico
#AutoIt3Wrapper_Change2CUI=y
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#cs ----------------------------------------------------------------------------

 AutoIt Version: 3.3.14.1
 Author:         myName

 Script Function:
	Template AutoIt script.

#ce ----------------------------------------------------------------------------
#include <File.au3>
#include <Array.au3>
#include <Date.au3>

Local $startTime = _NowCalc()
Global $masterReportFile = "Scripts\Utility\Report\TestAutomationReport_" & StringReplace(StringReplace(StringReplace($startTime, "/", "")," ","-"),":","") & ".htm"
Global $testsuiteTitle = ""

;Check if the parameter provided is correct
_CheckParameter()

;Setup daddress.ini file
_DAddressModifier()

;Delete System X logs
_DeleteSystemXLogs()

;Write test report header info
_WriteReportHeaderInfo()

;Delete available report logs
_CleanReportsDirectory()

Local $entryList = _GetEntryList($CmdLine[2])

Local $hTimer = TimerInit()
Local $iHours = 0, $iMins = 0, $iSecs = 0, $scriptCount = 0, $scriptErrorCount = 0
Local $start = (UBound($entryList) == 1) ? 0 : 1
ConsoleWrite(" --- Start of tests" & @LF)

Local $validList[0] = []

For $i = 0 To UBound($entryList) - 1
	Local $arrDirectories = SearchFolder("Scripts\TR_" & $entryList[$i] & "*")
	If (UBound($arrDirectories) > 0) Then
		_ArrayAdd($validList,$entryList[$i])
	Else
		ConsoleWrite(@CRLF & "Warning: No test suite found for this run ID (" & $entryList[$i] & ")")
	EndIf
Next

For $i = 0 To UBound($validList) - 1
	_RunTestRailX($validList[$i])
	$scriptCount = _ReadTextFromLog(@AppDataDir & "\script_count.log") + $scriptCount
	$scriptErrorCount = _ReadTextFromLog(@AppDataDir & "\script_error_count.log") + $scriptErrorCount
	; Write testsuite result to console
	If (_ReadTextFromLog(@AppDataDir & "\script_error_count.log") > 0) Then
		ConsoleWrite(" --- TR-" & $validList[$i] & ":" & $testsuiteTitle & "...FEIL (Vennligst sjekk vedlagte testlogger i Byggeartifakter for mer informasjon)" & @LF)
	Else
		ConsoleWrite(" --- TR-" & $validList[$i] & ":" & $testsuiteTitle & "...OK" & @LF)
	EndIf

	_WriteScriptStatus("TR-" & $validList[$i] & ": " & $testsuiteTitle, _ReadTextFromLog(@AppDataDir & "\rxlogfile.log") & ".html")
	$rxCompressedLogFilename = "TR-" & $validList[$i] & ":" & $testsuiteTitle
	$rxCompressedLogFilename = StringReplace($rxCompressedLogFilename, "-", "_")
	$rxCompressedLogFilename = StringReplace($rxCompressedLogFilename, " ", "_")
	$rxCompressedLogFilename = StringReplace($rxCompressedLogFilename, ".", "_")
	$rxCompressedLogFilename = StringReplace($rxCompressedLogFilename, ":", "_")
	FileCopy("Reports\" & _ReadTextFromLog(@AppDataDir & "\rxlogfile.log") & ".rxzlog", "Scripts\Reports\" & $rxCompressedLogFilename & ".rxzlog", 1)
Next

ConsoleWrite(" --- End of tests" & @LF)

;Write test duration
Local $fDiff = TimerDiff($hTimer)
_TicksToTime($fDiff, $iHours, $iMins, $iSecs)
Local $duration = StringFormat("%02d:%02d:%02d", $iHours, $iMins, $iSecs)
_UpdateHeaderInfo("%Duration%", $duration)

;Write total no. of failed scripts
_UpdateHeaderInfo("%Fail%", $scriptErrorCount)

;Write total no. of passed scripts
_UpdateHeaderInfo("%Pass%", $scriptCount - $scriptErrorCount)

;Write total no. of scripts
_UpdateHeaderInfo("%Total%", $scriptCount)

;Write total no. of testsuites/masterscripts
_UpdateHeaderInfo("%Tests%", UBound($validList))

_WriteEndTag()

FileCopy($masterReportFile, "Scripts\Reports\TestAutomationReport-MAIN.htm", 1)

_WriteSystemXLogs()

_ExportSystemXTableData("ERRORLOG_EVENTS", $CmdLine[4])
_ExportSystemXTableData("LOGFEIL", $CmdLine[4])

 If $scriptErrorCount > 0 Then
 	Exit (1)
 EndIf


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

 Func _CheckParameter()
	If $CmdLine[0] <= 1 Then		; Check if there is a parameter
		ConsoleWrite("ERROR: Parameter is missing! Please check ReadMe file for syntax")
		Exit (1)
	EndIf

	If $CmdLine[1] <> "/ID" Then	; Check if parameter name is correct
		ConsoleWrite("ERROR: Invalid parameter name! Please check ReadMe file for syntax")
		Exit (1)
	EndIf

	If $CmdLine[3] <> "/SET" Then	; Check if parameter name is correct
		ConsoleWrite("ERROR: Invalid parameter name! Please check ReadMe file for syntax")
		Exit (1)
	EndIf

	Local $validate = _CheckParameterValue($CmdLine[2])		; Check if the parameter value is correct
	If $validate <> "OK" Then
		ConsoleWrite($validate)
		Exit (1)
	EndIf
 EndFunc

 Func _CheckParameterValue($paramVal)
	Local $boolResult = True
	Local $strInfo = "OK"
	Local $entryList[0] = []
	If _StringContains($paramVal,",") Then
		$entryList = StringSplit($paramVal,",")
		For $i = 1 To UBound($entryList) - 1
			If $entryList[$i] = "" Then
				$boolResult = False
				ExitLoop
			EndIf
			If _StringContains($entryList[$i],"-") Then
				Local $entryList2 = StringSplit($entryList[$i],"-")
				If UBound($entryList2) > 3 Then
					$boolResult = False
					ExitLoop
					Else
						If $entryList2[1] == "" Or $entryList2[2] == "" Then
							$boolResult = False
							ExitLoop
							Else
								If Int($entryList2[1]) > Int($entryList2[2]) Then
									$boolResult = False
									ExitLoop
								EndIf
						EndIf
				EndIf
			EndIf
		Next
	ElseIf (StringInStr($paramVal,"-") > 0 And StringInStr($paramVal,",") = 0) Then
		$entryList = StringSplit($paramVal,"-")
		If UBound($entryList) == 3 Then
			For $x = 1 To UBound($entryList) - 1
				If $entryList[$x] = "" Then
					$boolResult = False
					ExitLoop
				EndIf
			Next
			If $boolResult Then
				If Int($entryList[1]) > Int($entryList[2]) Then
					$boolResult = False
				EndIf
			EndIf
		Else
			$boolResult = False
		EndIf
	EndIf
	If Not $boolResult And $paramVal <> "" Then
		$strInfo = "ERROR: Invalid parameter value! Please check ReadMe file for syntax"
	EndIf
	Return $strInfo
 EndFunc

 Func _StringContains($string, $containString)
	Local $intStringContain = StringInStr($string, $containString)
	Return ($intStringContain > 0)
 EndFunc

 Func _GetEntryList($strEntry)
	Local $result[0] = []
	Local $rangeList[0] = []


	; Entry example: 1000,1001
	If _StringContains($strEntry,",") Then
		$rangeList = StringSplit($strEntry,",")

 		For $x = 1 To UBound($rangeList) - 1
			Local $list = $rangeList[$x]

			If _StringContains($list,"-") Then
				Local $rangeList2 = StringSplit($list,"-")
				Local $start = Int($rangeList2[1])
				Local $end = Int($rangeList2[2])
				For $i = $start To $end
					_ArrayAdd($result, $i)
				Next
				Else
					_ArrayAdd($result, $list)
			EndIf
			#cs
			If _StringContains($list,"-") Then
				$rangeList = StringSplit($list,"-")
				If _StringContains($rangeList[0],",") Then
					Local $rangeList2 = StringSplit($rangeList[0],",")
					$rangeList[0] = $rangeList2[1]
				EndIf
				If _StringContains($rangeList[1],",") Then
					Local $rangeList2 = StringSplit($rangeList[1],",")
					$rangeList[1] = $rangeList2[0]
				EndIf
				Local $start = Int($rangeList[1])
				Local $end = Int($rangeList[2])
				For $x = $start To $end
					_ArrayAdd($result, $x)
				Next
			Else
				_ArrayAdd($result, $list)
			EndIf
			#ce
		Next
		ElseIf _StringContains($strEntry,"-") Then
			; Entry example: 1000-1001
			$rangeList2 = StringSplit($strEntry,"-")
			Local $start = Int($rangeList2[1])
			Local $end = Int($rangeList2[2])
			For $x = $start To $end
				_ArrayAdd($result, $x)
			Next
		Else
			_ArrayAdd($result, $strEntry)
	EndIf

	Return $result
 EndFunc

 Func _DAddressModifier()
	Local $iniFile = "C:\HMSCLIENT\daddress.ini"
	Local $sxUserFile = "C:\HMSCLIENT\SXUser.ini"

	;[fs.appserver]
	_LineTextModifier($iniFile, "fs.appserver", "text=CS")

	;[fs.gdbaddresse]
	_LineTextModifier($iniFile, "fs.gdbaddresse", "text=" & @ComputerName & ":C:\HMSDATA\Database\Auto-Test\DBHMS.IB")

	;[fs.dadr]
	_LineTextModifier($iniFile, "fs.dadr", "text=c:\hmsdata\")

	;[fs.ServerName]
	_LineTextModifier($iniFile, "fs.ServerName", "text=" & @ComputerName)

	;[fs.ServerLabel]
	_LineTextModifier($iniFile, "fs.ServerLabel", "Text=CI Auto Test")

	;[fs.dadr]
	_LineTextModifier($sxUserFile, "fs.dadr", "text=")
 EndFunc

 Func _DeleteSystemXLogs()
	FileCreateShortcut(@WorkingDir & "\Scripts\Utility\DeleteSystemXLogs\DeleteSystemXLogs.exe", @TempDir & "\DeleteSystemXLogs", @WorkingDir & "\Scripts\Utility\DeleteSystemXLogs\")
	RunWait(@ComSpec & " /c " & @TempDir & "\DeleteSystemXLogs.lnk")
 EndFunc

 Func _WriteReportHeaderInfo()
	;Copy master report template
	FileCopy("Scripts\Utility\Report\master_report_template.htm", $masterReportFile,1)

	;Write date and time the script has started
	_UpdateHeaderInfo("%ExecutionTime%", _Now())

	;Write OS version
	_UpdateHeaderInfo("%OperatingSystem%", _OSVersion())

	;Write Computer name
	_UpdateHeaderInfo("%ComputerName%", @ComputerName)
EndFunc

Func _UpdateHeaderInfo($info, $value)
	_ReplaceStringInFile($masterReportFile, $info, $value)
EndFunc

Func _OSVersion()
	Local $regPath = "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion"
	Local $productName = RegRead($regPath, "ProductName")
	Local $csdVersion = RegRead($regPath, "CSDVersion")
	Local $osArch = (@OSArch == "X86") ? "32-Bit" : "64-Bit"
	Return $productName & " " & $csdVersion & " " & $osArch
EndFunc

Func _CleanReportsDirectory()
	FileDelete("Scripts\Reports\*.*")
	FileDelete(@AppDataDir & "\*.log")
EndFunc

Func _LineTextModifier($fileINI, $oldText, $newText)
	$lineNumber = _GetTextLineNumber($fileINI, $oldText)
	_FileWriteToLine($fileINI, $lineNumber, $newText, True)
EndFunc

Func _GetTextLineNumber($fileINI, $searchText)
	$file = FileOpen($fileINI, 0)
	$lineNumber = 2

	While 1
		$line = FileReadLine($file)
		If @error = -1 Then ExitLoop
		If StringInStr($line,$searchText) Then
			ExitLoop
		EndIf
		$lineNumber+= 1
	WEnd
	FileClose($file)
	Return $lineNumber
EndFunc

Func _RunTestRailX($testId)
	Local $arrDirectories = SearchFolder("Scripts\TR_" & $testId & "*")
	Local $exeFile = StringReplace($arrDirectories[0], "TR_" & $testId & "_", "")
	Local $masterscript = ".\Scripts\" & $arrDirectories[0] & "\Masterscript\" & $exeFile & ".exe -ts=" & $exeFile & ".rxtst -testrail -truser=rommel@hovemedical.no -trpass=D8gAdDawnubs/OapYk02-XvXTDkP5fsaDN7.h4vx1 -trrunid=" & $testId
	$testsuiteTitle = StringReplace(StringReplace($arrDirectories[0], "_", " "),"TR " & $testId, "")
	$arrTestsuiteTitle = StringSplit($testsuiteTitle, " ")
	$testsuiteTitle = StringReplace($testsuiteTitle, $arrTestsuiteTitle[2] & " " & $arrTestsuiteTitle[3], $arrTestsuiteTitle[2] & "." & $arrTestsuiteTitle[3])

	RunWait(@ComSpec & " /C " & $masterscript)
EndFunc

Func _ReadTextFromLog($logfileName)
	Local $logFile = FileOpen($logfileName, $FO_READ)
	Local $content = FileRead($logFile)
	FileClose($logFile)
	Return $content
EndFunc

Func _WriteScriptStatus($fullSuiteTitle, $htmlFile)
	Local $hFileOpen = FileOpen($masterReportFile, $FO_APPEND)
	Local $errorCount = _ReadTextFromLog(@AppDataDir & "\script_error_count.log")
	Local $scriptCount =  _ReadTextFromLog(@AppDataDir & "\script_count.log")
	Local $timeScriptExecute =  _ReadTextFromLog(@AppDataDir & "\script_duration.log")
	Local $result = "Feil"
	;Local $comments = "Vennligst sjekk vedlagte test logg inn Byggeartifakter for flere feil detaljer"

	If ($errorCount == "0") Then
		$result = "OK"
		;ConsoleWrite($fullSuiteTitle & "...OK")
	Else
		;ConsoleWrite($fullSuiteTitle & "...FEIL (Vennligst sjekk vedlagte test logg inn Byggeartifakter for flere feil detaljer)")
	EndIf
	;Local $logPath = StringReplace(StringReplace(StringReplace($scriptName,"-","_"),":","")," ", "_")
	FileWriteLine($hFileOpen, @TAB & @TAB & @TAB & @TAB & @TAB & "<tr>")
	FileWriteLine($hFileOpen, @TAB & @TAB & @TAB & @TAB & @TAB & @TAB & '<td><a href="' & $htmlFile & '" target="_blank">' & $fullSuiteTitle & '</a></td>')
	FileWriteLine($hFileOpen, @TAB & @TAB & @TAB & @TAB & @TAB & @TAB & "<td>" & $timeScriptExecute & "</td>")
	FileWriteLine($hFileOpen, @TAB & @TAB & @TAB & @TAB & @TAB & @TAB & "<td class='bold " & StringLower($result) & "'>" & $result & "</td>")
	FileWriteLine($hFileOpen, @TAB & @TAB & @TAB & @TAB & @TAB & @TAB & "<td>" & $scriptCount & "</td>")
	FileWriteLine($hFileOpen, @TAB & @TAB & @TAB & @TAB & @TAB & "</tr>")
	;FileWriteLine($hFileOpen, @TAB & @TAB & @TAB & @TAB & "</tbody>")
	;FileWriteLine($hFileOpen, @TAB & @TAB & @TAB & "</table>")
	FileClose($hFileOpen)
EndFunc

Func _ExportSystemXTableData($tableName, $jobSet)
	Local $date = StringReplace(_NowDate(), ".", "")
	Local $exportFilename = @WorkingDir & "\Scripts\Reports\" & $tableName & "-" & $date & "-Set-" & $jobSet & ".txt";

	Local $queryFile = @AppDataDir & "\QUERY.SQL"
	FileDelete($queryFile)

	Local $hFileOpen = FileOpen($queryFile, $FO_APPEND)
	Const $iSQL = '"C:\Program Files\Embarcadero\InterBase\bin\isql.exe"'
	Const $mdbFile = "C:\hmsdata\Database\Auto-Test\DBHMS.IB"
	Const $userName = "SYSDBA"
	Const $userPassword = "dhocc648"


	FileDelete($exportFilename)
	FileWriteLine($hFileOpen, "SELECT * FROM " & $tableName & ";")
	FileWriteLine($hFileOpen, "EXIT;")
	FileClose($hFileOpen)

	RunWait(@ComSpec & " /c " & $iSQL & " " & $mdbFile & " -u " & $userName & " -p " & $userPassword & " -i " & $queryFile & " -o " & $exportFilename, "", @SW_HIDE)
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

Func _WriteSystemXLogs()
	FileCreateShortcut(@WorkingDir & "\Scripts\Utility\WriteSystemXLogs\WriteSystemXLogs.exe", @TempDir & "\WriteSystemXLogs", @WorkingDir & "\Scripts\Utility\WriteSystemXLogs\")
	RunWait(@ComSpec & " /c " & @TempDir & "\WriteSystemXLogs.lnk")
EndFunc