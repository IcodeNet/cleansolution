param($installPath, $toolsPath, $package, $project) 

#remove the designer
$ref = $project.Object.References.Find("Microsoft.Activities.Design")
if ($ref) {	$ref.Remove() }

$assembly = [reflection.assembly]::LoadFrom($toolsPath + "\Microsoft.Activities.NuGet.dll")

Import-Module -Assembly $assembly

Add-ActivityToolbox -Project $project.Object -Activity "Microsoft.Activities.Statements.DelayUntilTime" -Category "Microsoft.Activities" -ActivityAssembly "Microsoft.Activities" -BitmapPath "$toolspath\DelayCheck.bmp"
Add-ActivityToolbox -Project $project.Object -Activity "Microsoft.Activities.Statements.DelayUntilDateTime" -Category "Microsoft.Activities" -ActivityAssembly "Microsoft.Activities" -BitmapPath "$toolspath\DelayCheck.bmp"
Add-ActivityToolbox -Project $project.Object -Activity "Microsoft.Activities.Statements.LoadActivity" -Category "Microsoft.Activities" -ActivityAssembly "Microsoft.Activities" -BitmapPath "$toolspath\Activity.bmp"
Add-ActivityToolbox -Project $project.Object -Activity "Microsoft.Activities.Statements.InvokeWorkflow" -Category "Microsoft.Activities" -ActivityAssembly "Microsoft.Activities" -BitmapPath "$toolspath\InvokeWorkflow.bmp"
Add-ActivityToolbox -Project $project.Object -Activity "Microsoft.Activities.Statements.LoadAndInvokeWorkflow" -Category "Microsoft.Activities" -ActivityAssembly "Microsoft.Activities" -BitmapPath "$toolspath\LoadAndInvoke.bmp"
Add-ActivityToolbox -Project $project.Object -Activity "Microsoft.Activities.Statements.AddToDictionary``2" -DisplayName "AddToDictionary<TKey, TValue>"  -Category "Dictionary" -ActivityAssembly "Microsoft.Activities" -BitmapPath "$toolspath\AddToDict.bmp"
Add-ActivityToolbox -Project $project.Object -Activity "Microsoft.Activities.Statements.ClearDictionary``2" -DisplayName "ClearDictionary<TKey, TValue>" -Category "Dictionary" -ActivityAssembly "Microsoft.Activities" -BitmapPath "$toolspath\ClearDict.bmp"
Add-ActivityToolbox -Project $project.Object -Activity "Microsoft.Activities.Statements.GetFromDictionary``2" -DisplayName "GetFromDictionary<TKey, TValue>" -Category "Dictionary" -ActivityAssembly "Microsoft.Activities" -BitmapPath "$toolspath\DictGet.bmp"
Add-ActivityToolbox -Project $project.Object -Activity "Microsoft.Activities.Statements.KeyExistsInDictionary``2" -DisplayName "KeyExistsInDictionary<TKey, TValue>" -Category "Dictionary" -ActivityAssembly "Microsoft.Activities" -BitmapPath "$toolspath\ExistsDict.bmp"
Add-ActivityToolbox -Project $project.Object -Activity "Microsoft.Activities.Statements.ValueExistsInDictionary``2" -DisplayName "ValueExistsInDictionary<TKey, TValue>" -Category "Dictionary" -ActivityAssembly "Microsoft.Activities" -BitmapPath "$toolspath\ValExistsDict.bmp"
Add-ActivityToolbox -Project $project.Object -Activity "Microsoft.Activities.Statements.RemoveFromDictionary``2" -DisplayName "RemoveFromDictionary<TKey, TValue>" -Category "Dictionary" -ActivityAssembly "Microsoft.Activities" -BitmapPath "$toolspath\RemoveDict.bmp"
