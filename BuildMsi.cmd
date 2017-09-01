@echo off

del installer\*.wixobj
del installer\*.wixpdb
del installer\*.msi
del installer\attachrfiles.wxs
pushd installer
..\packages\ParaffinBundler\paraffin.exe -dir ..\AttachR\bin\release -groupName AttachRFiles attachrfiles.wxs -ext pdb -ext xml -ext exe -regexexclude .*\.vshost\..* -norootdirectory
..\packages\WiX.Toolset\tools\wix\candle.exe *.wxs
..\packages\WiX.Toolset\tools\wix\light.exe -ext WixNetFxExtension -ext WixUIExtension *.wixobj -o AttachR.msi
popd