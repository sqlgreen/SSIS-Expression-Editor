properties{
    $newVersion = $ReleaseVersion

    # Digital Signature SHA1
    $sha1 = "53df6a03e3dd74ccf55fac7491f7aadadea4a5bc"
}

properties{#directories
    #$framework_dir_2 = "$env:systemroot\microsoft.net\framework\v2.0.50727"
    #$framework_dir_3_5 = "$env:systemroot\microsoft.net\framework\v3.5"
    $base_dir = [System.IO.Directory]::GetParent("$pwd")
    $build_dir = "$base_dir\build"
}

properties { #solution file
    $sln_file_2005 = "$base_dir\ExpressionTester2005.sln"
    $sln_file_2008 = "$base_dir\ExpressionTester2008.sln"
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
    write-Host "Path: $msbuild"
    &($msbuild) $sln_file_2005 /t:Rebuild /p:Configuration=Release /v:q /p:DefineConstants="YUKON" 

    &($sign) sign /sha1 $sha1 /d "Expression Tester" /du http://www.konesans.com /t http://timestamp.comodoca.com/authenticode "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe"
}


task Copy2005 -depends Compile2005 {
    copy-item "$base_dir\ExpressionEditor\bin\Release\ExpressionEditor.dll" "$base_dir\Releases\ExpressionEditor2005\"
    copy-item "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe" "$base_dir\Releases\ExpressionEditor2005\"
}

task Clean2008 -depends Copy2005 { 
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
    write-Host "Path:  $msbuild"
    &($msbuild) $sln_file_2008 /t:Rebuild /p:Configuration=Release /v:q /p:DefineConstants="KATMAI" 

    &($sign) sign /sha1 $sha1 /d "Expression Tester" /du http://www.konesans.com /t http://timestamp.comodoca.com/authenticode "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe"
}

task Copy2008 -depends Compile2008 {
    copy-item "$base_dir\ExpressionEditor\bin\Release\ExpressionEditor.dll" "$base_dir\Releases\ExpressionEditor2008\"
    copy-item "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe" "$base_dir\Releases\ExpressionEditor2008\"
}

task MakeZips -depends Copy2005, Copy2008 {
    write-Host "Starting Zip $base_dir"

    &($zip) a -tzip "$base_dir\Releases\ExpressionEditor.zip" "$base_dir\Releases\ExpressionEditor2005\"
    &($zip) a -tzip "$base_dir\Releases\ExpressionEditor.zip" "$base_dir\Releases\ExpressionEditor2008\"
}
# SIG # Begin signature block
# MIIH5AYJKoZIhvcNAQcCoIIH1TCCB9ECAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUPPOOW00enHNvjPgpRxIwbz9K
# +1OgggUBMIIE/TCCA+WgAwIBAgIRANyqRi2Ra8RiVEYUO2AKG5AwDQYJKoZIhvcN
# AQEFBQAwgZUxCzAJBgNVBAYTAlVTMQswCQYDVQQIEwJVVDEXMBUGA1UEBxMOU2Fs
# dCBMYWtlIENpdHkxHjAcBgNVBAoTFVRoZSBVU0VSVFJVU1QgTmV0d29yazEhMB8G
# A1UECxMYaHR0cDovL3d3dy51c2VydHJ1c3QuY29tMR0wGwYDVQQDExRVVE4tVVNF
# UkZpcnN0LU9iamVjdDAeFw0wODA0MTYwMDAwMDBaFw0xMTA0MTYyMzU5NTlaMIGi
# MQswCQYDVQQGEwJHQjEQMA4GA1UEEQwHR0wyIDVETDEYMBYGA1UECAwPR2xvdWNl
# c3RlcnNoaXJlMRMwEQYDVQQHDApHbG91Y2VzdGVyMRwwGgYDVQQJDBMyMCBTb3Jl
# biBMYXJzZW4gV2F5MRkwFwYDVQQKDBBLb25lc2FucyBMaW1pdGVkMRkwFwYDVQQD
# DBBLb25lc2FucyBMaW1pdGVkMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKC
# AQEAjvFmm+v0V63NUf0TjeFsDYz+jtPIlUlEJsTppWIeJDc2k5dHN7FAWvX4GJbW
# BApXmIe+Za3EfygHJUdUvtB2/Q9jbwpMt0oyFvWsB/RGrEvxpUqtzcjP1ozGVJSF
# 5hyCBiW7yaJBmkmeIWW20GdWdSApV2KQMZ9XG+4XjOnB+kOaTrh7tj7UyG82EXyj
# XytnFutlFDidz0BJ3SMX65KHppJbmVuu39i356C+ScC7raniHQ1DA4Trqf3Stmm+
# pQueYJafB6SDUGPjEIb795AjtSfrXs65uV6nw9rUZh//iH5uW/4a4tGZQb/KtWIK
# ll0BeLXkLVsT8jEt1C7dUMGzGQIDAQABo4IBNzCCATMwHwYDVR0jBBgwFoAU2u1k
# dBScFDyr3ZmpvVsoTYs8ydgwHQYDVR0OBBYEFHHHBgrFEaWvrNnH2zij9gmDHtlX
# MA4GA1UdDwEB/wQEAwIHgDAMBgNVHRMBAf8EAjAAMBMGA1UdJQQMMAoGCCsGAQUF
# BwMDMBEGCWCGSAGG+EIBAQQEAwIEEDBGBgNVHSAEPzA9MDsGDCsGAQQBsjEBAgED
# AjArMCkGCCsGAQUFBwIBFh1odHRwczovL3NlY3VyZS5jb21vZG8ubmV0L0NQUzBC
# BgNVHR8EOzA5MDegNaAzhjFodHRwOi8vY3JsLnVzZXJ0cnVzdC5jb20vVVROLVVT
# RVJGaXJzdC1PYmplY3QuY3JsMB8GA1UdEQQYMBaBFHN1cHBvcnRAa29uZXNhbnMu
# Y29tMA0GCSqGSIb3DQEBBQUAA4IBAQC1gKNsttr7gIFpJRuEAvWmtJsspEq2IkLe
# KklaVlkz9K1n5ZSk+GnKJsVI4ehjvo9bvKcFi1+89kHRyRCAkN3+22nUMRHPT50y
# O8RmgVn4DGsh6iyJYkF3pvXnCykNvtHoQRsXySn1P8ZvbZHYIxfZ6b9Qayn/mpAh
# L9+BThodMAfHJc0t00dbYI6du04UQcjuvoVhFX9hKrLwlALlawiHgQaO9+P64HAl
# YfxItcRQyKL3lfDQ/fWih2odue5XcSJlTcIbQkwfcbswcEc1KRnyKnQZ0o5Kzl6l
# QKzi/c989S0rTNDNtp3XrwmhrjkgOOSBQpw3igUgIP7nzFdSNX/KMYICTTCCAkkC
# AQEwgaswgZUxCzAJBgNVBAYTAlVTMQswCQYDVQQIEwJVVDEXMBUGA1UEBxMOU2Fs
# dCBMYWtlIENpdHkxHjAcBgNVBAoTFVRoZSBVU0VSVFJVU1QgTmV0d29yazEhMB8G
# A1UECxMYaHR0cDovL3d3dy51c2VydHJ1c3QuY29tMR0wGwYDVQQDExRVVE4tVVNF
# UkZpcnN0LU9iamVjdAIRANyqRi2Ra8RiVEYUO2AKG5AwCQYFKw4DAhoFAKB4MBgG
# CisGAQQBgjcCAQwxCjAIoAKAAKECgAAwGQYJKoZIhvcNAQkDMQwGCisGAQQBgjcC
# AQQwHAYKKwYBBAGCNwIBCzEOMAwGCisGAQQBgjcCARUwIwYJKoZIhvcNAQkEMRYE
# FF/us0fGJRBLm3D2hBVRxwnTw/y9MA0GCSqGSIb3DQEBAQUABIIBAAbHemZB6gzc
# iNKQt3E8143BkejaiZpFE3scH8NRi+Y/C1ilwJIYtyHv4/0hHebBu/vYkfTARMYC
# z6h/aRsjDFU84PriBtWTZgTI5UpJcMRFlsshjM3RzmQJ88lkommYxKrQ+b12CTnz
# WbqluqK4yQh75B9y5j45TkgS8jGKBJdY095YDdCtx4Vawf3TYKTPeuHZKO0erwu2
# si79yoRT6bXTv02EEPzkRdMWgfp7CcUmHoL0gfsDJMKtpXiVRsvL98bRNpv1YREr
# 2KW+Bf+na9bxz5lIm0jdAvL8qY8j8kQz6Pvszu+npXRVFQksfK1mPNomD8+d1I9z
# KWpVrUDJm2U=
# SIG # End signature block
