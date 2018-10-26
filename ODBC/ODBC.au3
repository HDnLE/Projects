; List of the ODBC Drivers installed on this computer
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
	If $strODBC == "MySQL ODBC 3.51 Driver" Then
		MsgBox(0,"", $arrValueNames[$i])
	EndIf
Next
;Exit