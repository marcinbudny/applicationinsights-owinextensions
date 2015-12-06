gitversion /updateassemblyinfo 

"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" src\ApplicationInsights.OwinExtensions.sln /p:Configuration=Release

gitversion /exec _dopackage.bat

