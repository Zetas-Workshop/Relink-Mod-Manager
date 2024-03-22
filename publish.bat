
dotnet publish "Relink Mod Manager/Relink Mod Manager.csproj" -r win-x64 -c Release -o ./publish /p:PublishSingleFile=true /p:PublishTrimmed=true /p:TrimMode=Link /p:IncludeAllContentForSelfExtract=true --self-contained true
move "publish\Relink Mod Manager.exe" "publish\Relink Mod Manager.exe"