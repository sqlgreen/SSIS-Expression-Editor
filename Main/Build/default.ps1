properties{
    $newVersion = $ReleaseVersion
}



properties{#directories
    $framework_dir_3_5 = "$env:systemroot\microsoft.net\framework\v3.5"
    $framework_dir_4 = "$env:systemroot\microsoft.net\framework\v4.0.30319"
    $base_dir = [System.IO.Directory]::GetParent("$pwd")
    $build_dir = "$base_dir\build"
}

properties { #solution file
    $sln_file_2012 = "$base_dir\ExpressionTester2012.sln"
    $sln_file_2014 = "$base_dir\ExpressionTester2014.sln"
	$sln_file_2016 = "$base_dir\ExpressionTester2016.sln"
	$sln_file_2017 = "$base_dir\ExpressionTester2017.sln"
}


properties { #utility locations
    $msbuild = "msbuild.exe"  
    $tf = "$env:ProgramFiles\Microsoft Visual Studio 14.0\Common7\IDE\tf.exe"
    # Install 7-Zip - 7-Zip for 64-bit Windows x64 (Intel 64 or AMD64)
    $zip = "C:\Program Files\7-zip\7z.exe"
    $sign = "C:\Program Files (x86)\Windows Kits\10\bin\x86\signtool.exe"
}

task default -depends MakeZips

task Clean2012 { 
    if (!(Test-Path "$base_dir\Releases\ExpressionEditor2012"))
    {
        new-item "$base_dir\Releases\ExpressionEditor2012" -type directory
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
    &($frameworkbuild) $sln_file_2012 /t:Rebuild /p:Configuration="Release" /v:q /p:DefineConstants="DENALI" 

    &($sign) sign /a /d "Expression Tester" /du http://www.konesans.com /t http://timestamp.comodoca.com/authenticode "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe"
}

task Copy2012 -depends Compile2012 {
    copy-item "$base_dir\ExpressionEditor\bin\Release\ExpressionEditor.dll" "$base_dir\Releases\ExpressionEditor2012\"
    copy-item "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe" "$base_dir\Releases\ExpressionEditor2012\"
}


task Clean2014 -depends Copy2012 { 
    if (!(Test-Path "$base_dir\Releases\ExpressionEditor2014"))
    {
        new-item "$base_dir\Releases\ExpressionEditor2014"-type directory
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

task Compile2014 -depends Clean2014 {
    $frameworkbuild = ($framework_dir_4 + "\" + $msbuild)
    write-Host "Path: $frameworkbuild"
	Write-Host $sln_file_2014
    &($frameworkbuild) $sln_file_2014 /t:Rebuild /p:Configuration=Release /v:q /p:DefineConstants="SQL2014" 

    &($sign) sign /a /d "Expression Tester" /du http://www.konesans.com /t http://timestamp.comodoca.com/authenticode "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe"
}

task Copy2014 -depends Compile2014 {
    copy-item "$base_dir\ExpressionEditor\bin\Release\ExpressionEditor.dll" "$base_dir\Releases\ExpressionEditor2014\"
    copy-item "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe" "$base_dir\Releases\ExpressionEditor2014\"
}

task Clean2016 -depends Copy2014 { 
    if (!(Test-Path "$base_dir\Releases\ExpressionEditor2016"))
    {
        new-item "$base_dir\Releases\ExpressionEditor2016"-type directory
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

task Compile2016 -depends Clean2016 {
    $frameworkbuild = ($framework_dir_4 + "\" + $msbuild)
    write-Host "Path: $frameworkbuild"
	Write-Host $sln_file_2016
    &($frameworkbuild) $sln_file_2016 /t:Rebuild /p:Configuration=Release /v:q /p:DefineConstants="SQL2016" 

    &($sign) sign /a /d "Expression Tester" /du http://www.konesans.com /t http://timestamp.comodoca.com/authenticode "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe"
}

task Copy2016 -depends Compile2016 {
    copy-item "$base_dir\ExpressionEditor\bin\Release\ExpressionEditor.dll" "$base_dir\Releases\ExpressionEditor2016\"
    copy-item "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe" "$base_dir\Releases\ExpressionEditor2016\"
}


task Clean2017 -depends Copy2016 { 
    if (!(Test-Path "$base_dir\Releases\ExpressionEditor2017"))
    {
        new-item "$base_dir\Releases\ExpressionEditor2017"-type directory
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

task Compile2017 -depends Clean2017 {
    $frameworkbuild = ($framework_dir_4 + "\" + $msbuild)
    write-Host "Path: $frameworkbuild"
	Write-Host $sln_file_2017
    &($frameworkbuild) $sln_file_2017 /t:Rebuild /p:Configuration=Release /v:q /p:DefineConstants="SQL2017" 

    &($sign) sign /a /d "Expression Tester" /du http://www.konesans.com /t http://timestamp.comodoca.com/authenticode "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe"
}

task Copy2017 -depends Compile2017 {
    copy-item "$base_dir\ExpressionEditor\bin\Release\ExpressionEditor.dll" "$base_dir\Releases\ExpressionEditor2017\"
    copy-item "$base_dir\ExpressionTester\bin\Release\ExpressionTester.exe" "$base_dir\Releases\ExpressionEditor2017\"
}

task MakeZips -depends Copy2012, Copy2014, Copy2016, Copy2017 {
    write-Host "Starting Zip $base_dir"

    &($zip) a -tzip "$base_dir\Releases\ExpressionEditor.zip" "$base_dir\Releases\ExpressionEditor2012\"
    &($zip) a -tzip "$base_dir\Releases\ExpressionEditor.zip" "$base_dir\Releases\ExpressionEditor2014\"
	&($zip) a -tzip "$base_dir\Releases\ExpressionEditor.zip" "$base_dir\Releases\ExpressionEditor2016\"
	&($zip) a -tzip "$base_dir\Releases\ExpressionEditor.zip" "$base_dir\Releases\ExpressionEditor2017\"
}

