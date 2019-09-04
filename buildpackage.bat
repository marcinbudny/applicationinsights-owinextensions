gitversion /updateassemblyinfo 

"C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\MSBuild.exe" src\ApplicationInsights.OwinExtensions.sln /p:Configuration=Release

gitversion /exec _dopackage.bat

