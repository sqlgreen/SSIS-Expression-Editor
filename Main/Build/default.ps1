properties{
    $newVersion = $ReleaseVersion

    # Digital Signature SHA1
    $sha1 = "0e1003c6a75db08e6a50ef777635563b809cdd13"
}

properties{#directories
    $framework_dir_3_5 = "$env:systemroot\microsoft.net\framework\v3.5"
    $framework_dir_4 = "$env:systemroot\microsoft.net\framework\v4.0.30319"
    $base_dir = [System.IO.Directory]::GetParent("$pwd")
    $build_dir = "$base_dir\build"
}

properties { #solution file
    $sln_file_2005 = "$base_dir\ExpressionTester2005.sln"
    $sln_file_2008 = "$base_dir\ExpressionTester2008.sln"
    $sln_file_2012 = "$base_dir\ExpressionTester2012.sln"
    #$sql_ver = "2005"
}


properties { #utility locations
    $msbuild = "msbuild.exe"  
    $tf = "$env:ProgramFiles\Microsoft Visual Studio 9.0\Common7\IDE\tf.exe"
    $zip = "$env:ProgramFiles\7-zip\7z.exe"
    $sign = "$env:ProgramFiles\Microsoft SDKs\Windows\v6.0A\Bin\SignTool.exe"
    $ilmerge = "Z:\Program Files (x86)\Microsoft\ILMerge\ILMerge.exe"
}


task default -depends MakeZips

task Clean2005{ 
    if (!(Test-Path "$base_dir\Releases\ExpressionEditor2005"))
    {
        new-item "$base_dir\Releases\ExpressionEditor2005" -type directory
    }

    if (test-path "$base_dir\ExpressionEditor\bin\Release\")
    {
        remove-item "$base_dir\ExpressionEditor\bin\Release\" -recurse
    }

    if (test-path "$base_dir\ExpressionTester\bin\Release\")
    {
        remove-item "$base_dir\ExpressionTester\bin\Release\" -recurse
    }
}


task Compile2005 -depends Clean2005 {
    $frameworkbuild = ($framework_dir_3_5 + "\" + $msbuild)
    write-Host "Path: $frameworkbuild"
    &($frameworkbuild) $sln_file_2005 /t:Rebuild /p:Configuration=Release /v:q /p:DefineConstants="YUKON" 

    &($sign) sign /sha1 $sha1 /d "Expression Tester" /du http://www.konesans.com /t http://timestamp.comodoca.com/authenticode "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe"
}


task Copy2005 -depends Compile2005 {
    copy-item "$base_dir\ExpressionEditor\bin\Release\ExpressionEditor.dll" "$base_dir\Releases\ExpressionEditor2005\"
    copy-item "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe" "$base_dir\Releases\ExpressionEditor2005\"
}

task Clean2008 -depends Copy2005 { 
    if (!(Test-Path "$base_dir\Releases\ExpressionEditor2008"))
    {
        new-item "$base_dir\Releases\ExpressionEditor2008" -type directory
    }

    if (test-path "$base_dir\ExpressionEditor\bin\Release\")
    {
        remove-item "$base_dir\ExpressionEditor\bin\Release\" -recurse
    }

    if (test-path "$base_dir\ExpressionTester\bin\Release\")
    {
        remove-item "$base_dir\ExpressionTester\bin\Release\" -recurse
    }
}

task Compile2008 -depends Clean2008 {
    $frameworkbuild = ($framework_dir_3_5 + "\" + $msbuild)
    write-Host "Path: $frameworkbuild"
    &($frameworkbuild) $sln_file_2008 /t:Rebuild /p:Configuration=Release /v:q /p:DefineConstants="KATMAI" 

    &($sign) sign /sha1 $sha1 /d "Expression Tester" /du http://www.konesans.com /t http://timestamp.comodoca.com/authenticode "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe"
}

task Copy2008 -depends Compile2008 {
    copy-item "$base_dir\ExpressionEditor\bin\Release\ExpressionEditor.dll" "$base_dir\Releases\ExpressionEditor2008\"
    copy-item "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe" "$base_dir\Releases\ExpressionEditor2008\"
}


task Clean2012 -depends Copy2008 { 
    if (!(Test-Path "$base_dir\Releases\ExpressionEditorDenali"))
    {
        new-item "$base_dir\Releases\ExpressionEditorDenali" -type directory
    }

    if (test-path "$base_dir\ExpressionEditor\bin\Release\")
    {
        remove-item "$base_dir\ExpressionEditor\bin\Release\" -recurse
    }

    if (test-path "$base_dir\ExpressionTester\bin\Release\")
    {
        remove-item "$base_dir\ExpressionTester\bin\Release\" -recurse
    }
}

task Compile2012 -depends Clean2012 {
    $frameworkbuild = ($framework_dir_4 + "\" + $msbuild)
    write-Host "Path: $frameworkbuild"
    &($frameworkbuild) $sln_file_2012 /t:Rebuild /p:Configuration=Release /v:q /p:DefineConstants="DENALI" 

    &($sign) sign /sha1 $sha1 /d "Expression Tester" /du http://www.konesans.com /t http://timestamp.comodoca.com/authenticode "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe"
}

task Copy2012 -depends Compile2012 {
    copy-item "$base_dir\ExpressionEditor\bin\Release\ExpressionEditor.dll" "$base_dir\Releases\ExpressionEditorDenali\"
    copy-item "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe" "$base_dir\Releases\ExpressionEditorDenali\"
}

task MakeZips -depends Copy2005, Copy2008, Copy2012 {
    write-Host "Starting Zip $base_dir"

    &($zip) a -tzip "$base_dir\Releases\ExpressionEditor.zip" "$base_dir\Releases\ExpressionEditor2005\"
    &($zip) a -tzip "$base_dir\Releases\ExpressionEditor.zip" "$base_dir\Releases\ExpressionEditor2008\"
    &($zip) a -tzip "$base_dir\Releases\ExpressionEditor.zip" "$base_dir\Releases\ExpressionEditorDenali\"
}

# SIG # Begin signature block
# MIINEQYJKoZIhvcNAQcCoIINAjCCDP4CAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUmsZKXndELkX4/r9fiY1udo88
# Z1ygggpMMIIE0DCCA7igAwIBAgIQc1eMcW2zlVMTffMJcxir/jANBgkqhkiG9w0B
# AQUFADCBlTELMAkGA1UEBhMCVVMxCzAJBgNVBAgTAlVUMRcwFQYDVQQHEw5TYWx0
# IExha2UgQ2l0eTEeMBwGA1UEChMVVGhlIFVTRVJUUlVTVCBOZXR3b3JrMSEwHwYD
# VQQLExhodHRwOi8vd3d3LnVzZXJ0cnVzdC5jb20xHTAbBgNVBAMTFFVUTi1VU0VS
# Rmlyc3QtT2JqZWN0MB4XDTExMDQyNzAwMDAwMFoXDTIwMDUzMDEwNDgzOFoweTEL
# MAkGA1UEBhMCR0IxGzAZBgNVBAgTEkdyZWF0ZXIgTWFuY2hlc3RlcjEQMA4GA1UE
# BxMHU2FsZm9yZDEaMBgGA1UEChMRQ09NT0RPIENBIExpbWl0ZWQxHzAdBgNVBAMT
# FkNPTU9ETyBDb2RlIFNpZ25pbmcgQ0EwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAw
# ggEKAoIBAQCV4zPrBjKZoyL0U0BrUF9eO8AqDn6Uubmb+uFe5wqcdD7WjF9+OKJH
# a6AeC8D9B+8QvEzbK9uWJtE8Ug4+K47eEtJmmoxIilY4y11wbrNoj5vnl7Bgvbuc
# R30A2L9IJGF+E4ThagbcaXQtZbJclsI5301QcYhQWyFnjXSbcwUot4YfBSbCgA4Y
# R7ENXX4Zy0K/qbbDNlv4XNfP60KwRMxKin2osM+DL66I3N5eRE68n9NcMYR29n6e
# m3e844GkXt1A4OivnEJb+cmOZaWR6u4nHN+Yt7BKWDWV/rA6c0Z+VkWx1cCclu3A
# bviUnLvvJsHLbd0eZ5F6ecl8CGZ22EW5AgMBAAGjggE1MIIBMTAfBgNVHSMEGDAW
# gBTa7WR0FJwUPKvdmam9WyhNizzJ2DAdBgNVHQ4EFgQUICUjFxG/0A2g5yu8ntWa
# QglPKsUwDgYDVR0PAQH/BAQDAgEGMBIGA1UdEwEB/wQIMAYBAf8CAQAwEQYDVR0g
# BAowCDAGBgRVHSAAMEIGA1UdHwQ7MDkwN6A1oDOGMWh0dHA6Ly9jcmwudXNlcnRy
# dXN0LmNvbS9VVE4tVVNFUkZpcnN0LU9iamVjdC5jcmwwdAYIKwYBBQUHAQEEaDBm
# MD0GCCsGAQUFBzAChjFodHRwOi8vY3J0LnVzZXJ0cnVzdC5jb20vVVROQWRkVHJ1
# c3RPYmplY3RfQ0EuY3J0MCUGCCsGAQUFBzABhhlodHRwOi8vb2NzcC51c2VydHJ1
# c3QuY29tMA0GCSqGSIb3DQEBBQUAA4IBAQCzlfJAw9PEpH9SnmCuiMPhTsV/zWV+
# jO8VLl8w8LsYM3gyRZ4DFX+daIu8QWfgAyufAzy1K9ZOV3wgRXOXTKGwB+lP042M
# JeoaiDUw/mKUASq6HNXYFgQWqTTcDuMFyJKcQcSC5vFmrD08HdDxLuSWihL9fgNK
# 5vRZoG6Z0unEDWLWqztra8IcztXYZo/kbj8Bdcs3jG1KWZXK6GiejZwowTnBaQf9
# j6+chJ0LTvLSmfXUqBcJFsYXqGfaUBoh6nXPJT67vIEiFATpO7EukTqsnCO8hzse
# zq4ls6GxgwDvLsPe28VxxS8v+bCFv2DUdfhpL2t+o69azpTybNDgJNbGMIIFdDCC
# BFygAwIBAgIQUHMbx0YlnPi/uovPlvtONTANBgkqhkiG9w0BAQUFADB5MQswCQYD
# VQQGEwJHQjEbMBkGA1UECBMSR3JlYXRlciBNYW5jaGVzdGVyMRAwDgYDVQQHEwdT
# YWxmb3JkMRowGAYDVQQKExFDT01PRE8gQ0EgTGltaXRlZDEfMB0GA1UEAxMWQ09N
# T0RPIENvZGUgU2lnbmluZyBDQTAeFw0xMTA4MTIwMDAwMDBaFw0xNDA4MTEyMzU5
# NTlaMIHGMQswCQYDVQQGEwJHQjERMA8GA1UEEQwIUEUxMiA4TEgxFTATBgNVBAgM
# DExpbmNvbG5zaGlyZTERMA8GA1UEBwwIU3BhbGRpbmcxFjAUBgNVBAkMDUZsZWV0
# IEhhcmdhdGUxFjAUBgNVBAkMDU9sZCBNYWluIFJvYWQxFDASBgNVBAkMC1RoZSBM
# YXVyZWxzMRkwFwYDVQQKDBBLb25lc2FucyBMaW1pdGVkMRkwFwYDVQQDDBBLb25l
# c2FucyBMaW1pdGVkMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAyrvY
# hz3EtFSu/OvN83X2jP+8YIHa3BzsEM1D0O3njVgualsh0EvMRtgiCw0Ff5S7/OZV
# 6YL1vqk0yimhxmZL7dLdDdbGLbD4nePB10sd1aVU6quEMQ+BzTtTKFY4IudBV11O
# o4LfuQeRoI9dJ4WsSg4/kxBadrcAtvqaKJqkjW/Av9mkCloiCj3R9gR6Fh1aPeh2
# 7eQJwGoYHYIrDyTosP50Xm99rjlm7vc6wo/l58v1ferJ38Trw5Y0sBu9DvHT++9A
# 4Ssd8j03hVrzeZIAwel15ulPNC4tus00MXaCYJwVftDLhXZYrhtVMpNadgM9XtsG
# fYVAajp5YIdqIyZVlwIDAQABo4IBqDCCAaQwHwYDVR0jBBgwFoAUICUjFxG/0A2g
# 5yu8ntWaQglPKsUwHQYDVR0OBBYEFEXlXyrtzesR6pmyxPs6kNEi/ileMA4GA1Ud
# DwEB/wQEAwIHgDAMBgNVHRMBAf8EAjAAMBMGA1UdJQQMMAoGCCsGAQUFBwMDMBEG
# CWCGSAGG+EIBAQQEAwIEEDBGBgNVHSAEPzA9MDsGDCsGAQQBsjEBAgEDAjArMCkG
# CCsGAQUFBwIBFh1odHRwczovL3NlY3VyZS5jb21vZG8ubmV0L0NQUzBABgNVHR8E
# OTA3MDWgM6Axhi9odHRwOi8vY3JsLmNvbW9kb2NhLmNvbS9DT01PRE9Db2RlU2ln
# bmluZ0NBLmNybDBxBggrBgEFBQcBAQRlMGMwOwYIKwYBBQUHMAKGL2h0dHA6Ly9j
# cnQuY29tb2RvY2EuY29tL0NPTU9ET0NvZGVTaWduaW5nQ0EuY3J0MCQGCCsGAQUF
# BzABhhhodHRwOi8vb2NzcC5jb21vZG9jYS5jb20wHwYDVR0RBBgwFoEUc3VwcG9y
# dEBrb25lc2Fucy5jb20wDQYJKoZIhvcNAQEFBQADggEBAGSFl4006tj66sBR6ybN
# NGriefZ56TVsWuptT0rHkgLZkwEIbTFNsfOXA1THudx6Ur5x97EwuHyH5f2GU7ul
# QFcZBD/YMiTH8W/ZYzEr3AnwuY+FRYYEoR4tdUJxnDeiUYHCHHLKzZhPbhOZrdVX
# 4V/gJOaaVwLiqNQSQ6OjblTxv/j1W4KeNHjakfAH2rwS58SV8idn/soWRXHetzfC
# gJi6Mols1DguHaKdsFa2v7iggzVIEC0IOnVC26k1bGDEsi6ZgBrmaMiuCzZonPLj
# DVh8OmergKa9W5RZ1EAOg66O0EMzX4vIEivnvVdQR6MOb5JA1QdYla2zHF7Y8XpV
# 1n4xggIvMIICKwIBATCBjTB5MQswCQYDVQQGEwJHQjEbMBkGA1UECBMSR3JlYXRl
# ciBNYW5jaGVzdGVyMRAwDgYDVQQHEwdTYWxmb3JkMRowGAYDVQQKExFDT01PRE8g
# Q0EgTGltaXRlZDEfMB0GA1UEAxMWQ09NT0RPIENvZGUgU2lnbmluZyBDQQIQUHMb
# x0YlnPi/uovPlvtONTAJBgUrDgMCGgUAoHgwGAYKKwYBBAGCNwIBDDEKMAigAoAA
# oQKAADAZBgkqhkiG9w0BCQMxDAYKKwYBBAGCNwIBBDAcBgorBgEEAYI3AgELMQ4w
# DAYKKwYBBAGCNwIBFTAjBgkqhkiG9w0BCQQxFgQUXyDH6jRxEjAoBliNQlV8YsrI
# v2AwDQYJKoZIhvcNAQEBBQAEggEAGxp3VRK/WSSBk4F4FEyDDouvrCIEkAwLxAtG
# kKiBNjMSCPR2F0Lj6Nb4hDed6IUbuwROZV0p8PHdNLeOAMN9ELxyGtdrdz8kBTfA
# ZqvU00EbebSkg2Q6e54grntKjTrPszkOc6+avSU2c6zEnIPnwKaF9yz9LkO33u5U
# wdBVrvXIXsrSZ5mDzetKpQNTU8umOm8C+Va3EERY0t+v09g0aGOj3GAFR6dxfaXz
# FXoRMAJwT7awGEhyI2dNGtXFmzYhivLs0x4+I3BJqNtwmpIHB4qKCC+C2+/r/eki
# eMU6J3eAiz1g0dXKIy1Ar8HYkhEDNNhg3NbVEu0J9d8A2v+HQw==
# SIG # End signature block
