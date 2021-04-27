echo "-----------------------------------";
#If the file does not exist, create it.
if (-not(Test-Path -Path "./bin/nuget.exe" -PathType Leaf)) {
     try {
			echo "Downloading nuget executable 5.2.0"
			New-Item -ItemType Directory -Force -Path "./bin/"
			Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/v5.2.0/nuget.exe" -OutFile "./bin/nuget.exe"
     }
     catch {
         throw $_.Exception.Message
     }
 }
# If the file already exists, show the message and do nothing.
 else {
     echo "Found nuget.exe. Please make sure you have version 5.2.0 installed!"
 }

echo "Done"
echo "-----------------------------------";
echo "Packing IdeaStatiCa.OpenModel";
&"bin/nuget.exe" "pack" "./src/IdeaRS.OpenModel/IdeaRS.OpenModel.csproj" "-properties" "Configuration=Release" "-IncludeReferencedProjects" | Write-Host;
echo "Finished packing nuget IdeaStatiCa.OpenModel";
echo "-----------------------------------";
echo "Packing IdeaStatiCa.Plugin";
&"bin/nuget.exe" "pack" "./src/IdeaStatiCa.Plugin/IdeaStatiCa.Plugin.csproj" "-properties" "Configuration=Release" "-IncludeReferencedProjects" "-Exclude" "**\*.x86.*;**\*.x64.*" | Write-Host;
echo "Finished packing nuget IdeaStatiCa.Plugin";
echo "-----------------------------------";
echo "Done packing!"