# psake v0.11
# Copyright c 2008 James Kovacs
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
#
# The above copyright notice and this permission notice shall be included in
# all copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
# THE SOFTWARE.

param(
[string]$buildFile = 'default.ps1',
[string[]]$taskList = @(),
[string]$framework = '3.5',
[switch]$debug = $false,
[switch]$help = $false,
[string]$ReleaseVersion = ''
)

if($help) {
@"
psake [buildFile] [tasks] [-framework ver] [-debug]
where buildFile is the name of the build file, (default: default.ps1)
tasks is a list of tasks to execute from the build file,
ver is the .NET Framework version to target - 1.0, 1.1, 2.0, 3.0, or 3.5
3.5 is the default
debug dumps information the tasks.
psake -help
Displays this message.
"@
return
}
$global:versionNum = $ReleaseVersion
$global:tasks = @{}
$global:properties = @()
$script:executedTasks = New-Object System.Collections.Stack
$script:callStack = New-Object System.Collections.Stack
$originalEnvPath = $env:path
$originalDirectory = Get-Location

$global:properties += {$ReleaseVersion}

function task([string]$name, [scriptblock]$action = $null, [string[]]$depends = @()) {
if($name -eq 'default' -and $action -ne $null) {
throw "Error: default task cannot specify an action"
}
$newTask = @{
Name = $name
DependsOn = $depends
Action = $action
}
if($global:tasks.$name -ne $null) { throw "Error: Task, $name, has already been defined." }
$global:tasks.$name = $newTask
}

function properties([scriptblock]$propertyBlock) {
$global:properties += $propertyBlock
}

function AssertNotCircular([string]$name) {
if($script:callStack.Contains($name)) {
throw "Circular reference found for task, $name"
}
}

function ExecuteTask([string]$name) {
if($script:executedTasks.Contains($name)) { return }
AssertNotCircular $name
$script:callStack.Push($name)

$task = $global:tasks.$name
foreach($childTask in $task.DependsOn) {
ExecuteTask $childTask
}
if($name -ne 'default') {
Write-Host "Executing task, $name..."
if($task.Action -ne $null) {
& $task.Action
}
Write-Host "`n"
}

$poppedTask = $script:callStack.Pop()
if($poppedTask -ne $name) {
throw "CallStack was corrupt. Expected $name, but got $poppedTask."
}
$script:executedTasks.Push($name)
}

function DumpTasks {
Write-Host 'Dumping tasks:'
foreach($key in $global:tasks.Keys) {
$task = $global:tasks.$key;
$task.Name + " depends on " + $task.DependsOn.Length + " other tasks: " + $task.DependsOn;
}
Write-Host "`n"
}

function DumpProperties {
Write-Host 'Dumping properties:'
$global:properties
}

function ConfigureEnvForBuild {
$version = $null
switch ($framework) {
'1.0' { $version = 'v1.0.3705' }
'1.1' { $version = 'v1.1.4322' }
'2.0' { $version = 'v2.0.50727' }
'3.0' { $version = 'v2.0.50727' } # .NET 3.0 uses the .NET 2.0 compilers
'3.5' { $version = 'v3.5' }
default { throw "Error: Unknown .NET Framework version, $framework" }
}
$frameworkDir = "$env:windir\Microsoft.NET\Framework\$version\"
if(!(test-path $frameworkDir)) {
throw "Error: No .NET Framework installation directory found at $frameworkDir"
}
$env:path = "$frameworkDir;$env:path"
}

function Cleanup {
$env:path = $originalEnvPath
$global:tasks = $null
Set-Location $originalDirectory
}

function RunBuild {
# Faking a finally block
trap {
Write-Host -foregroundcolor Red $_
Cleanup
break
}

# Execute the build file to set up the tasks and defaults
if(test-path $buildFile) {
$buildFile = resolve-path $buildFile
set-location (split-path $buildFile)
& $buildFile
} else {
throw "Error: Could not find the build file, $buildFile."
}

if($debug) {
DumpProperties
DumpTasks
}

ConfigureEnvForBuild

foreach($propertyBlock in $global:properties) {
# N.B. The initial dot (.) indicates that variables initialized/modified
# in the propertyBlock are available in the parent scope.
. $propertyBlock
}

# Execute the list of tasks or the default task
if($taskList.Length -ne 0) {
foreach($task in $taskList) {
ExecuteTask $task
}
} elseif ($global:tasks.default -ne $null) {
EXecuteTask default
}

# Clear out any global variables
Cleanup
}

# Avoids printing of error dump with line numbers
trap {
# continue
}

RunBuild

# SIG # Begin signature block
# MIINEQYJKoZIhvcNAQcCoIINAjCCDP4CAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUO6+rIJJfUt/WyTJBj0LdCM46
# 6SSgggpMMIIE0DCCA7igAwIBAgIQc1eMcW2zlVMTffMJcxir/jANBgkqhkiG9w0B
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
# DAYKKwYBBAGCNwIBFTAjBgkqhkiG9w0BCQQxFgQUWD2+xwg3pmhaxoa6avOttxEX
# bZwwDQYJKoZIhvcNAQEBBQAEggEAMD0pWLOQxa7ACRf4Ow8fESLQACneE4zL4qew
# q4blUdlCvHkmm2UXkp2lQdOZeEHMGFq3KfU0cEK3ss9PLJjod8THdcE0kK3kaJT1
# 41HisUm3oOl+8ZzDSXrk/NHirUUZHgYxe7RKUCF2xJetecsawkfPq43sHGm//Uam
# AiXW7zYkbRSKyPQMYA01O8PoDfiLI9suarv2KRhqr2e4UNQgDGyjDMOeiirgqMIr
# mZn6IFpfe7hzM9RRwcj85MCjw+iY/vjPbtiqhDIgGKfHxSShuzcA38r4S/aOJ7BZ
# qfhLsnoxWnWvzpQpgJvLVtvYtek5oSnR8QZMwRPBVSLkKELWeg==
# SIG # End signature block
