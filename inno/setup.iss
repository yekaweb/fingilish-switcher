; Inno Setup Script for Fingilish Switcher Industrial Edition v1.2.0
#define MyAppName "Fingilish Switcher"
#define MyAppVersion "1.2.0"
#define MyAppPublisher "Fingilish"
#define MyAppExeName "FingilishSwitcher.exe"

[Setup]
AppId={{5E2D32F7-4182-4B6A-B68C-98A6F9E131BC}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=d:\Fingilish\inno\license.txt
InfoBeforeFile=d:\Fingilish\inno\info.txt
OutputDir=d:\Fingilish\installer
OutputBaseFilename=FingilishSwitcher_Setup_v1.2.0
SetupIconFile=d:\Fingilish\src\FingilishSwitcher\app.ico
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
PrivilegesRequiredOverridesAllowed=commandline dialog
UninstallDisplayIcon={app}\{#MyAppExeName}
UninstallDisplayName={#MyAppName} Industrial Edition (نسخه ۱.۲.۰)
CloseApplications=force

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"
Name: "startwithwindows"; Description: "اجرای خودکار هنگام بالا آمدن ویندوز (Launch on Windows Startup)"; GroupDescription: "تنظیمات سیستمی (System Tasks)"

[Files]
Source: "d:\Fingilish\dist\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\پیشخوان صنعتی {#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "FingilishSwitcher"; ValueData: """{app}\{#MyAppExeName}"""; Tasks: startwithwindows; Flags: uninsdeletevalue

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallRun]
Filename: "taskkill"; Parameters: "/F /IM {#MyAppExeName}"; Flags: runhidden; RunOnceId: "StopFingilish"
