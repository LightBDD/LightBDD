@echo off
dotnet test --logger:"console;verbosity=detailed" %*
