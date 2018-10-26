#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Icon=reminder.ico
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#include <WindowsConstants.au3>
#include <guiconstants.au3>
#include <MsgBoxConstants.au3>
#include <FontConstants.au3>

Local $iniFile = @ScriptDir & "\TimesheetReminder.ini"
Local $alarmTime = IniRead($iniFile, "Alarm", "Time", "17:30")
Global $SnoozeDuration = IniRead($iniFile, "Alarm", "SnoozeDuration", "3")
Local $message = IniRead($iniFile, "Display", "DisplayMessage", "Time to go...")
Local $isShow = IniRead($iniFile, "Display", "ShowWaitingTime", "0")
Local $backColor = IniRead($iniFile, "Display", "GUIBackColor", "0x000000")
Local $guiFontName = IniRead($iniFile, "Display", "GUIFontName", "Impact")
Local $guiFontColor = IniRead($iniFile, "Display", "GUIFontColor", "0xFE2E64")
Local $xLocation = IniRead($iniFile, "Display", "XLocation", "-304")
Local $yLocation = IniRead($iniFile, "Display", "YLocation", "-92")
Local $splitTime = StringSplit($alarmTime, ":")
Local $hour = $splitTime[1]
Local $minute = $splitTime[2]

Global $SEC = 99
Local $Show = 0
Global $timer = 0;

Global $popup = GUICreate('Task Reminder', 300, 50, @DesktopWidth + ($xLocation), @DesktopHeight + ($yLocation), $WS_POPUP, BitOR($WS_EX_TOPMOST, $WS_EX_TOOLWINDOW))
GUISetOnEvent($GUI_EVENT_CLOSE, "_Close")
GUISetBkColor($backColor)
Global $lblMessage = GUICtrlCreateLabel('', -1, -1, 300, 50, BitOR($SS_CENTER, $BS_CENTER))
GUICtrlSetFont($lblMessage, 17, 0, 0, $guiFontName)
GUICtrlSetColor($lblMessage, $guiFontColor)
WinSetTrans($popup, "", 190)

Local $alarmReach = 0

While 1
    Switch GUIGetMsg()
        Case -3
            GUIDelete()
            ExitLoop
	EndSwitch

    If $SEC = @SEC Then ContinueLoop
    $SEC = @SEC

	If (@HOUR = $hour And @MIN = $minute) Then $alarmReach = 1

	If $alarmReach = 1 Then
		_ShowMessage($message)
		ExitLoop
	EndIf

	Local $current = (@HOUR*60) + @MIN
	Local $minutes = ($hour*60) + $minute

	If ($minutes < $current) Then
		_ShowMessage("Time already expires!")
	Else
		If $isShow = 1 Then
			GUISetState(@SW_SHOW)
		EndIf
		Local $minDiff = $minute - @MIN
		Local $hrDiff = ($hour - @HOUR)
		If $minDiff < 0 Then
			$minDiff = 60 + $minDiff
			$hrDiff = $hrDiff - 1
		EndIf
		Local $secDiff = 59 - @SEC
		$hrDiff = ($hrDiff < 10) ? "0" & $hrDiff : $hrDiff
		$minDiff = $minDiff-1
		$minDiff = ($minDiff < 10) ? "0" & $minDiff : $minDiff
		$secDiff = ($secDiff < 10) ? "0" & $secDiff :$secDiff
		GUICtrlSetData($lblMessage, "Alarm sets in " & $hrDiff & ":" & $minDiff & ":" & $secDiff)
	EndIf
WEnd

Func _ShowMessage($displayMessage)
	While 1
		If $SEC = @SEC Then ContinueLoop
		$SEC = @SEC
		GUISetState(@SW_SHOW)
		If $Show Then
			GUICtrlSetColor($lblMessage,$backColor)
		Else
			GUICtrlSetColor($lblMessage, $guiFontColor)
		EndIf

		$Show = Not $Show
		GUICtrlSetData($lblMessage, $displayMessage)
		$timer = $timer + 1
		If ($timer = ($SnoozeDuration)*60) Then
			GUIDelete()
			ExitLoop
		EndIf
		HotKeySet("{ESC}", "_Close")
	WEnd
EndFunc

Func _Close()
  GUIDelete()
  Exit
EndFunc