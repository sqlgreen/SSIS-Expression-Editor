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