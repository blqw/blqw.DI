del %CD%\unpkgs\*.nupkg
dotnet build -c release
dotnet pack --no-build -o %CD%\unpkgs
for /r . %%a in (unpkgs\*.nupkg) do (
 	dotnet nuget push "%%a" -s https://api.nuget.org/v3/index.json -k %1%
)
del %CD%\unpkgs\*.nupkg