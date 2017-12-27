del %CD%\unpkgs\*.nupkg
dotnet pack -c Release -o %CD%\unpkgs
for /r . %%a in (unpkgs\*.nupkg) do (
 	nuget push "%%a" -source https://www.nuget.org/api/v2/package -apikey %1%
)
del %CD%\unpkgs\*.nupkgnupkg