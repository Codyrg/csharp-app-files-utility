<#
.SYNOPSIS
    Script to increment project versions for beta, minor releases and builds.
.DESCRIPTION
    Script will increment either the patch or minor version for all projects under the current directory. Intelligently infers the next beta, release, or build version. 
.PARAMETER mode 
    Specify 'major', 'minor' or 'build'
.EXAMPLE 
    publish.ps1 release
.SEE
	https://gist.github.com/StephenRedd/d4f1471dbc205b9eb8484d6baa3bab85
#>

[cmdletbinding()]
param([string]$mode)



if($mode -ne "major" -and $mode -ne "minor" -and $mode -ne "build"){
    Write-Output "Syntax:  SemVer.ps1 [[-mode] <String>]"
    Write-Output "Please specify a mode; 'major', 'minor' or 'build'."
    Write-Output "Get-Help ./SemVer.ps1 for more info"
    exit 0;
}
Write-Output "mode = $mode"



$srcDir = Get-ChildItem .

$save = $true;
foreach ($folder in $srcDir) {
    $p = Join-Path -Path $folder.FullName -ChildPath '*.csproj';
    # only src project folders -> folders with a csproj file 
    if (Test-Path $p -PathType Leaf) {
        $projectFolders += $folder.FullName
        $projFile =  Get-ChildItem -Path $p | Select-Object -last 1
        $proj = [xml](get-content -Encoding UTF8 $projFile)
        $proj.GetElementsByTagName("SemVer") | ForEach-Object{
            $origVer = $_."#text"
            $verArray = $_."#text".Split(".")
            $majorInt = [convert]::ToInt32( $verArray[0], 10)
            $minorInt = [convert]::ToInt32( $verArray[1], 10)
            $buildInt = [convert]::ToInt32( $verArray[2], 10)
                      
            switch($mode) {
                "major"{			
					$majorInt = $majorInt + 1
					$minorInt = 0				
					$buildInt = 0
                }
                "minor"{
                    $minorInt = $minorInt + 1    			
					$buildInt = 0               
                }
                "build" {
                    $buildInt = $buildInt + 1
                }
            }
            
            $buildVersion =  $majorInt.ToString() + "." + $minorInt.ToString() + "." + $buildInt.ToString()
            $_."#text" = $buildVersion
            Write-Output "Incrementing version for: $($projFile.Name)"
            Write-Output "    $origVer --> $buildVersion"
        
            $proj.Save($projFile)            
        }
    }
}