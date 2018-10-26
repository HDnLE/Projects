#cs ----------------------------------------------------------------------------

 AutoIt Version: 3.3.14.1
 Author:         Rommel Lamanilao

 Script Function:
	Patch Auto-Uninstall

#ce ----------------------------------------------------------------------------

; Script Start - Add your code below here

#include <File.au3>
#include <Array.au3>
#include <GUIConstantsEx.au3>

Local $arrPatches[0] = []

Local $hGUI = GUICreate("Patch Auto-Uninstall", 300, 145)
Local $lblLabel = GUICtrlCreateLabel("Select patches to be uninstalled:",10,10,185,25)
Local $chkPatch1 = GUICtrlCreateCheckbox("WebOffice", 10, 30, 85, 25)
GUICtrlSetState(-1, $GUI_CHECKED)
Local $lblLabel = GUICtrlCreateLabel("Version:",120,35,40,25)
Local $txtPatch1 = GUICtrlCreateInput("",160,30,125,20)
Local $chkPatch2 = GUICtrlCreateCheckbox("Platform", 10, 55, 105, 25)
GUICtrlSetState(-1, $GUI_CHECKED)
Local $lblLabel2 = GUICtrlCreateLabel("Version:",120,60,40,25)
Local $txtPatch2 = GUICtrlCreateInput("",160,55,125,20)
Local $chkPatch3 = GUICtrlCreateCheckbox("Application", 10, 80, 105, 25)
GUICtrlSetState(-1, $GUI_CHECKED)
Local $lblLabel3 = GUICtrlCreateLabel("Version:",120,85,40,25)
Local $txtPatch3 = GUICtrlCreateInput("",160,80,125,20)
Local $btnUninstall = GUICtrlCreateButton("Uninstall", 60, 110, 85, 25)
Local $btnCancel = GUICtrlCreateButton("Cancel", 150, 110, 85, 25)

GUISetState(@SW_SHOW, $hGUI)

While 1
	Switch GUIGetMsg()
		Case $GUI_EVENT_CLOSE, $idClose
                ExitLoop
		Case $GUI_EVENT_CLOSE, $btnCancel
                ExitLoop
		Case $btnUninstall
			If (GUICtrlRead($chkPatch1) = 1) Then
				_ArrayAdd($arrPatches,"WebOffice." & GUICtrlRead($txtPatch1))
			EndIf
			If (GUICtrlRead($chkPatch2) = 1) Then
				_ArrayAdd($arrPatches,"Platform." & GUICtrlRead($txtPatch2))
			EndIf
			If (GUICtrlRead($chkPatch3) = 1) Then
				_ArrayAdd($arrPatches,"Application." & GUICtrlRead($txtPatch3))
			EndIf
			If (UBound($arrPatches) = 0) Then
				MsgBox(48,"Patch Auto-Uninstall", "No patch selected!")
			Else
				ExitLoop
			EndIf
	EndSwitch
WEnd

GUIDelete($hGUI)


;Make a loop for each patch uninstallation

For $x = 0 To UBound($arrPatches) -1
	UninstallPatch($arrPatches[$x]);
;~ 	$patchFilename = "Package History";													;Name of the logfile (Patches WebOffice and Platform use this logfile - You should include the full path, example: C:\Temp\Package History)
;~ 	If ($arrPatches[$x] = "Application") Then
;~ 		$patchFilename = "Update History";												;Name of the other logfile (Application patch uses this logfile - You should include the full path, example: C:\Temp\Update History)
;~ 	EndIf

;~ 	$patchVersion = GetPatchVersion($patchFilename, $arrPatches[$x]);					;Call the function that will fetch the latest patch installed

;~ 	If ($patchVersion = "") Then														;If defined patch is not installed
;~ 		MsgBox(48,"Error", "This patch (" & $arrPatches[$x] & ") is not installed!");
;~ 	Else
;~ 		$patchFullName = $arrPatches[$x] & "." & $patchVersion;							;Full patch name
;~ 		UninstallPatch($patchFullName);													;Call the function that will uninstal the patch
;~ 	EndIf
Next

;Function that fetches for the latest patch installed
;Parameter 1 - Name of the logfile
;Parameter 2 - The keyword to search which is the name of the patch to be uninstalled
Func GetPatchVersion($patchFile, $patchName)
	FileOpen($patchFile, 0);
	$lines = _FileCountLines($patchFile);												;Get the total number of lines in the logfile

	For $i = $lines To 1 Step -1
		$response = "";
		$line  = FileReadLine($patchFile,$i);							;Read the contents of the logfile line by line starting from the bottom (Latest log info is located at the bottom)
		If StringInStr($line, $patchName) Then							;Search for line where it matches the keyword (Patch name)
			$arrLines = StringSplit($line," ");							;Extract the searched text by space (" ")
			$response = $arrLines[1];									;This is now the value that you are looking
			ExitLoop;													;Exit the loop when keyword is found
		EndIf
	Next
	FileClose($patchFile);

	Return $response;
EndFunc

Func UninstallPatch($patch)
	;Run(@ComSpec & " /c " & 'ACSHelper.exe /c:"cscript C:\ACS\Patch\' & $patch & '\install.js -u"', "", @SW_HIDE);		;Uncomment this line to execute uninstall
	MsgBox(0,"Windows Command",'ACSHelper.exe /c:"cscript C:\ACS\Patch\' & $patch & '\install.js -u"');					;Comment this out
EndFunc
