param($installPath, $toolsPath, $package, $project) 

#remove the designer
$ref = $project.Object.References.Find("Microsoft.Activities.UnitTesting.Design")
if ($ref) {	$ref.Remove() }

$assembly = [reflection.assembly]::LoadFrom($toolsPath + "\Microsoft.Activities.NuGet.dll")

Import-Module -Assembly $assembly

Add-ActivityToolbox -Project $project.Object -Activity "Microsoft.Activities.UnitTesting.Activities.DiagnosticTrace" -Category "Unit Testing" -ActivityAssembly "Microsoft.Activities.UnitTesting" -BitmapPath "$toolspath\DiagTrace.bmp"
Add-ActivityToolbox -Project $project.Object -Activity "Microsoft.Activities.UnitTesting.Activities.TestBookmark``1" -DisplayName "TestBookmark<T>" -Category "Unit Testing" -ActivityAssembly "Microsoft.Activities.UnitTesting" -BitmapPath "$toolspath\Bookmark.bmp"
Add-ActivityToolbox -Project $project.Object -Activity "Microsoft.Activities.UnitTesting.Activities.TestAsync" -Category "Unit Testing" -ActivityAssembly "Microsoft.Activities.UnitTesting" -BitmapPath "$toolspath\Activity.bmp"
