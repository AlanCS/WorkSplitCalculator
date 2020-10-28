Set-StrictMode -Version latest
$ErrorActionPreference = "Stop"

$folder = "temp"
Remove-Item $folder -Recurse -ErrorAction Ignore

$newFolder = New-Item -ItemType directory -Path $folder

dotnet build -c Release src > ($folder + "/BuildResults.log")

dotnet publish --no-build -c Release src/web -o ($folder + "/Web")   > ($folder + "/PublishResults.log") 
dotnet publish --no-build -c Release src/Client -o ($folder + "/Client")   > ($folder + "/PublishResults.log") 

dotnet run -p src/CdkStack