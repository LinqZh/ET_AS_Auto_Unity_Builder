# https://devblogs.microsoft.com/scripting/use-powershell-to-work-with-any-ini-file/
function Get-IniContent ($filePath) {
    $ini = @{}
    switch -regex -file $filePath {
        "^\[(.+)\]" {
            $section = $matches[1]
            $ini[$section] = @{}
            $CommentCount = 0
        }
        "^(;.*)$" {
            $value = $matches[1]
            $CommentCount = $CommentCount + 1
            $name = "Comment" + $CommentCount
            $ini[$section][$name] = $value.Trim()
        }
        "(.+?)\s*=(.*)" {
            $name,$value = $matches[1..2]
            $ini[$section][$name] = $value.Trim()
        }
    }
    return $ini
}


# reg add HKEY_CURRENT_USER\Console /v QuickEdit /t REG_DWORD /d 00000000 /f
reg add HKEY_CURRENT_USER\Console\%SystemRoot%_SysWOW64_WindowsPowerShell_v1.0_powershell.exe /v QuickEdit /t REG_DWORD /d 00000000 /f
$group = Get-IniContent($args[0] + "\env.ini")
Set-Location $args[1]

# $gradlewPath = $args[0] + "\gradlew.bat"
# if (Test-Path $gradlewPath) {
#     gradle wrapper
# }
# Set-QuickEdit -DisableQuickEdit
$order = "gradlew " + $args[2] + " -Pandroid.injected.signing.store.file=" + $group["asBuild"]["file"] + " -Pandroid.injected.signing.store.password=" + $group["asBuild"]["password"] + " -Pandroid.injected.signing.key.alias=" + $group["asBuild"]["alias"] + " -Pandroid.injected.signing.key.password=" + $group["asBuild"]["key_password"]
cmd /c $order