
call "%VS100COMNTOOLS%\vsvars32.bat"

msbuild i4o.sln /p:Configuration=Debug /verbosity:normal
if %errorlevel% neq 0 exit /b %errorlevel%

.\packages\StatLight.1.3.3981\tools\StatLight.exe -x=.\i4o.Silverlight.Tests\Bin\Debug\i4o.Silverlight.Tests.xap
if %errorlevel% neq 0 exit /b %errorlevel%

.\packages\NUnit.2.5.7.10213\Tools\nunit-console.exe .\i4o.Tests\bin\Debug\i4o2.Tests.dll
if %errorlevel% neq 0 exit /b %errorlevel%