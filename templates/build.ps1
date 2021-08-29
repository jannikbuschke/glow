Remove-Item .\content\.vs -Recurse -Force
Get-ChildItem .\content\ -include bin,obj,node_modules,.vs -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
nuget pack
