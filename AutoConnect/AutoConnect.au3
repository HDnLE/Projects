#cs ----------------------------------------------------------------------------

 AutoIt Version: 3.3.14.1
 Author:         myName

 Script Function:
	Template AutoIt script.

#ce ----------------------------------------------------------------------------

; Script Start - Add your code below here
RunWait(@ComSpec & " /c " & '"C:\Program Files\uvnc bvba\UltraVNC\vncviewer.exe" -connect host ci-2012r2-02 -password Ranorex1', "", @SW_HIDE)