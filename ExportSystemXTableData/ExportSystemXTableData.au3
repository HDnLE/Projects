#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Icon=hovemedical.ico
#AutoIt3Wrapper_Change2CUI=y
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
;Syntax: ExportSystemXTableData -T <tableName> -S <jobSet>

#include <Date.au3>
#include <File.au3>

Local $strTableName = $CmdLine[2]
Local $strJobSet = $CmdLine[4]
Local $date = StringReplace(_NowDate(), ".", "")
Local $exportFilename = @WorkingDir & "\Scripts\Reports\" & $CmdLine[2] & "-" & $date & "-Set-" & $CmdLine[4] & ".txt";
Local $queryFile = @AppDataDir & "\QUERY.SQL"
Local $hFileOpen = FileOpen(@AppDataDir & "\QUERY.SQL", $FO_APPEND)
Local $iSQL = '"C:\Program Files\Embarcadero\InterBase\bin\isql.exe"'
Local $mdbFile = "C:\hmsdata\Database\Auto-Test\DBHMS.IB"
Local $userName = "SYSDBA"
Local $userPassword = "dhocc648"

FileDelete(@AppDataDir & "\QUERY.SQL")
FileDelete($exportFilename)
FileWriteLine($hFileOpen, "SELECT * FROM " & $CmdLine[2] & ";")
FileWriteLine($hFileOpen, "EXIT;")
FileClose($hFileOpen)

RunWait(@ComSpec & " /c " & $iSQL & " " & $mdbFile & " -u " & $userName & " -p " & $userPassword & " -i " & $queryFile & " -o " & $exportFilename)