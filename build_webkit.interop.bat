@rem Run this whenever the WebKit binaries are updated.
call "%VS100COMNTOOLS%..\..\VC\vcvarsall.bat" x86
if not exist "tools\TypeNormalizer.exe" csc /out:"tools\TypeNormalizer.exe" "tools\TypeNormalizer.cs"
set OUTPUT_DIR=webkit\bin
tlbimp "webkit\lib\webkit.tlb" /silent /keyfile:"WebKit.NET.snk" /namespace:WebKit.Interop /out:"%OUTPUT_DIR%\WebKit.Interop.dll"
ildasm "%OUTPUT_DIR%\WebKit.Interop.dll" /out="%OUTPUT_DIR%\temp_webkit_interop.il" /nobar
"tools\TypeNormalizer.exe" "%OUTPUT_DIR%\temp_webkit_interop.il"
ilasm "%OUTPUT_DIR%\temp_webkit_interop.il" /dll /output="%OUTPUT_DIR%\WebKit.Interop.dll" /key="WebKit.NET.snk"
del /F /Q "%OUTPUT_DIR%\temp_webkit_interop.*"
@pause