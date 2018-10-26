#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Icon=hovemedical.ico
#AutoIt3Wrapper_Change2CUI=y
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****

#cs ----------------------------------------------------------------------------

 AutoIt Version: 3.3.14.1
 Author:         Rommel A. Lamanilao (rommel@hovemdical.no)

 Script Function:
	To restart SystemXBackend service

#ce ----------------------------------------------------------------------------

#include <File.au3>
RunWait(@ComSpec & " /c " & 'C:\Windows\System32\NET STOP SystemXBackend', "", @SW_HIDE)
Local $winStatus = _GetServiceStatus()
Do
	Sleep(1000)
	$winStatus = _GetServiceStatus()
Until $winStatus == "STOPPED"

While $winStatus == "STOPPED"
	Sleep(5000)
	RunWait(@ComSpec & " /c " & 'C:\Windows\System32\NET START SystemXBackend', "", @SW_HIDE)
	$winStatus = _GetServiceStatus()
WEnd

Func _GetServiceStatus()
	Sleep(5000)
	RunWait(@ComSpec & " /c " & 'C:\Windows\System32\SC query SystemXBackend > ' & @AppDataDir & '\ServiceStatus.log', "", @SW_HIDE)
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