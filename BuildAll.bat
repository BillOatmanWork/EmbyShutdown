dotnet publish -r win-x64 -c Release -p:PublishReadyToRun=true -p:PublishSingleFile=true --self-contained true -p:IncludeNativeLibrariesForSelfExtract=true
dotnet publish -r osx-x64 -c Release /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained
dotnet publish -r linux-arm -c Release /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained
dotnet publish -r linux-arm64 -c Release /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained
dotnet publish -r linux-x64 -c Release /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained

cd C:\repos\EmbyShutdown\Build

copy /Y "C:\repos\EmbyShutdown\bin\Release\net8.0\win-x64\publish\EmbyShutdown.exe" .
"C:\Program Files\7-Zip\7z" a -tzip EmbyShutdown-WIN.zip EmbyShutdown.exe

copy /Y "C:\repos\EmbyShutdown\bin\Release\net8.0\osx-x64\publish\EmbyShutdown" .
"C:\Program Files\7-Zip\7z" a -t7z EmbyShutdown-OSX.7z EmbyShutdown

copy /Y "C:\repos\EmbyShutdown\bin\Release\net8.0\linux-x64\publish\EmbyShutdown" .
"C:\Program Files\7-Zip\7z" a -t7z EmbyShutdown-LIN64.7z EmbyShutdown

copy /Y "C:\repos\EmbyShutdown\bin\Release\net8.0\linux-arm\publish\EmbyShutdown" .
"C:\Program Files\7-Zip\7z" a -t7z EmbyShutdown-RasPi.7z EmbyShutdown

copy /Y "C:\repos\EmbyShutdown\bin\Release\net8.0\linux-arm64\publish\EmbyShutdown" .
"C:\Program Files\7-Zip\7z" a -t7z EmbyShutdown-RasPi64.7z EmbyShutdown

