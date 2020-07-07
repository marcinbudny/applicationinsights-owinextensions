gitversion /updateassemblyinfo 

"C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe" src\ApplicationInsights.OwinExtensions.sln /p:Configuration=Release

gitversion /exec _dopackage.bat

