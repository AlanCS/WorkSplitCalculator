Set-StrictMode -Version latest
$ErrorActionPreference = "Stop"

$folder = "published"
Remove-Item $folder -Recurse -ErrorAction Ignore

$newFolder = New-Item -ItemType directory -Path $folder

# take care of the .net stuff
dotnet build -c Release src > ($folder + "/BuildResults.log")
dotnet publish --no-build -c Release src/WorkSplitCalculator/Web -o ($folder + "/WorkSplitCalculator/Web")   > ($folder + "/PublishResults.log") 
dotnet publish --no-build -c Release src/WorkSplitCalculator/Client -o ($folder + "/WorkSplitCalculator/Client")   > ($folder + "/PublishResults.log") 

# static website
Copy-item -Force -Recurse src/LandingWebsite -Destination $($folder + "/LandingWebsite") 

dotnet run -p src/CloudConfiguration