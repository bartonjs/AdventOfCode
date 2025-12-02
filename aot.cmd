@echo off
dotnet publish -c Release csharp\Runner\AdventOfCode.csproj
csharp\Runner\bin\Release\net10.0\win-x64\publish\AdventOfCode.exe %*
