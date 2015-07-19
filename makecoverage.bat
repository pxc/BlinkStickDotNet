@echo off
SETLOCAL

REM REM Run this to regenerate the unit test coverage statistics for use by Sonarqube

SET root=BlinkStickDotNet
SET cc="C:\Program Files (x86)\Microsoft Visual Studio 11.0\Team Tools\Dynamic Code Coverage Tools\CodeCoverage.exe"
SET nunit="C:\Program Files (x86)\NUnit 2.6.4\bin\nunit-console.exe"
SET testtarget=.\BlinkStickDotNetTest\bin\Debug\BlinkStickDotNetTest.exe

REM Delete old files, otherwise it whines
DEL %root%.coverage
DEL %root%.coverage.xml

REM Generate %root%.converage
%cc% collect /output:%root%.coverage %nunit% %testtarget%

REM Generate %root%.converage.xml
%cc% analyze /output:%root%.coverage.xml %root%.coverage

ECHO.
ECHO Now load it into Sonarqube with this command:
ECHO "c:\Program Files (x86)\sonar-runner-2.4\bin\sonar-runner"
ECHO.
ECHO Then open http://localhost:9000

