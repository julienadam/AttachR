@echo off

del installer\*.wixobj
del installer\*.wixpdb
del installer\*.msi
del installer\attachrfiles.wxs
pushd installer
..\packages\ParaffinBundler.3.61\paraffin.exe -dir ..\AttachR\bin\release -groupName AttachRFiles attachrfiles.wxs -ext pdb -ext xml -ext exe -regexexclude .*\.vshost\..* -norootdirectory
..\packages\WiX.Toolset.3.9.1208.0\tools\wix\candle.exe *.wxs
..\packages\WiX.Toolset.3.9.1208.0\tools\wix\light.exe -ext WixNetFxExtension -ext WixUIExtension *.wixobj -o AttachR.msi
popd