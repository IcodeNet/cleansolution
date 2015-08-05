param($installPath, $toolsPath, $package, $project) 

$assembly = [reflection.assembly]::LoadFrom($toolsPath + "\Microsoft.Activities.NuGet.dll")

Import-Module -Assembly $assembly

Remove-ToolboxTab  -Category "Microsoft.Activities"	
Remove-ToolboxTab  -Category "Dictionary"			
