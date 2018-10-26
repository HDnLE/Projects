#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Icon=hovemedical.ico
#AutoIt3Wrapper_Change2CUI=y
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#include <MsgBoxConstants.au3>

Local $iFreeSpace = DriveSpaceFree("C:\")
$iFreeSpace = Round($iFreeSpace/1024,1)

Local $limitDiskSpace = 5

If $CmdLine[0] == 1 Then
	ConsoleWrite("Syntax error. (Argument expects a value)" & @LF & "[Syntax: DiskSpaceChecker.exe -LIMIT <disk_space_limit>]")
	Exit(1)
EndIf

If $CmdLine[0] == 2 Then
	If StringUpper($CmdLine[1]) <> "-LIMIT" Then
		ConsoleWrite("Syntax error. (Invalid argument name)" & @LF & "[Syntax: DiskSpaceChecker.exe -LIMIT <disk_space_limit>]")
		Exit(1)
	EndIf

	If Number($CmdLine[2]) == 0 Then
		ConsoleWrite("Syntax error. (Invalid limit value)" & @LF & "[Syntax: DiskSpaceChecker.exe -LIMIT <disk_space_limit>]")
		Exit(1)
	EndIf

	$limitDiskSpace = $CmdLine[2]
EndIf


If $iFreeSpace < $limitDiskSpace Then
	ConsoleWrite("ERROR: You are running very low on disk space (" & $iFreeSpace & " Gb) on Local Disk (C:)." & @LF)
	ConsoleWrite("This test needs more disk space as it backs up the database after complete. Free up some space and run this job again.")
	Exit(1)
EndIf

If ProcessExists("Systemx.exe") Then
	ProcessClose("Systemx.exe")
EndIf

If ProcessExists("IBConsole.exe") Then
	ProcessClose("IBConsole.exe")
EndIf

ConsoleWrite("Checking available disk space... OK")