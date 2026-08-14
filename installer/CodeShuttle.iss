; CodeShuttle installer — Inno Setup 6
;
; Build with:   ISCC /DAppVersion=1.0.0 /Oartifacts installer\CodeShuttle.iss
; or, easier:   .\build\publish.ps1 -Version 1.0.0 -Installer
;
; Expects publish\win-x64\CodeShuttle.exe to already exist. Run publish.ps1
; first; this script does not build anything.
;
; Inno Setup was chosen over WiX (XML, steep, buys nothing for a single-exe
; utility) and over MSIX. MSIX is the one that matters: its sandbox forbids
; machine-wide registry writes, so it cannot set LongPathsEnabled, and would
; therefore silently defeat the longPathAware manifest the application ships
; with. A packaging format that quietly breaks a shipped capability is not a
; packaging format we can use.

#ifndef AppVersion
  #define AppVersion "1.0.0"
#endif

#define AppName        "CodeShuttle"
#define AppPublisher   "MyCompany"
#define AppTagline     "Send your code to AI. Bring the answers back."
#define AppUrl         "https://github.com/aelmi/CodeShuttle"
#define AppExeName     "CodeShuttle.exe"
#define SourceDir      "..\publish\win-x64"

[Setup]
; AppId is the existing project GUID from CodeShuttle.csproj / the .sln.
; DO NOT CHANGE IT. Inno keys upgrade detection on this value; a new GUID makes
; every future release install alongside the old one instead of over it.
AppId={{11358B60-344C-4CDB-98C3-13F2A47701D9}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppUrl}
AppSupportURL={#AppUrl}/issues
AppUpdatesURL={#AppUrl}/releases
VersionInfoVersion={#AppVersion}
VersionInfoDescription={#AppTagline}

DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
LicenseFile=..\LICENSE.txt
InfoAfterFile=..\CHANGELOG.md

; Let the user choose per-user or all-users rather than forcing elevation.
; A per-user install is a legitimate outcome — see the [Registry] note below.
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog

; x64 only. There is no 32-bit story in 2026, and win-arm64 waits for demand.
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

MinVersion=10.0.17763
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
OutputBaseFilename={#AppName}-{#AppVersion}-setup
SetupIconFile=..\CodeShuttle.ico
UninstallDisplayIcon={app}\{#AppExeName}
UninstallDisplayName={#AppName}

; --- Code signing (deferred) -------------------------------------------
; Signing is blocked on business identity, not on tooling; see CHANGELOG.md.
; When a certificate is available, define the "signtool" tool in the Inno Setup
; IDE (Tools > Configure Sign Tools) or pass it on the ISCC command line, then
; uncomment the two directives below. The exe inside is signed separately, by
; build\publish.ps1, before this script runs.
;
; The timestamp URL is not optional: without /tr the signature stops validating
; the day the certificate expires.
;
;   signtool sign /fd SHA256 /tr http://timestamp.digicert.com /td SHA256 \
;                 /f cert.pfx /p <password> $f
;
; SignTool=signtool
; SignedUninstaller=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop shortcut"; GroupDescription: "Additional shortcuts:"; Flags: unchecked

[Files]
Source: "{#SourceDir}\{#AppExeName}";       DestDir: "{app}"; Flags: ignoreversion
Source: "..\LICENSE.txt";                   DestDir: "{app}"; Flags: ignoreversion
Source: "..\THIRD-PARTY-NOTICES.txt";       DestDir: "{app}"; Flags: ignoreversion
Source: "..\PRIVACY.md";                    DestDir: "{app}"; Flags: ignoreversion
Source: "..\README.md";                     DestDir: "{app}"; Flags: ignoreversion
Source: "..\CHANGELOG.md";                  DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}";        Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}";  Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Registry]
; Long path support. The application's manifest declares longPathAware, but
; that flag alone does nothing — Windows also requires this machine-wide
; registry value, which is why MSIX was rejected. The code still handles
; PathTooLongException, so a per-user install degrades rather than breaking.
;
; The Check is mandatory. Without it a per-user install attempts an HKLM write
; it has no permission for and the installer throws.
Root: HKLM; Subkey: "SYSTEM\CurrentControlSet\Control\FileSystem"; \
    ValueType: dword; ValueName: "LongPathsEnabled"; ValueData: 1; \
    Check: IsAdminInstallMode

[Run]
Filename: "{app}\{#AppExeName}"; Description: "Launch {#AppName}"; \
    Flags: nowait postinstall skipifsilent

[UninstallDelete]
; Nothing. This section is deliberately empty.
;
; %APPDATA%\CodeShuttle is NOT removed on uninstall. It holds the user's
; presets, prompt templates and — critically — the backup copies CodeShuttle
; takes before overwriting their source files. Deleting a user's only copy of
; their own data as a side effect of uninstalling the tool would be
; indefensible. The uninstaller says so, below.

[Messages]
ConfirmUninstall=Are you sure you want to remove %1?%n%nYour settings, presets and file backups in %%APPDATA%%\CodeShuttle will be kept, so a reinstall picks up where you left off. Delete that folder by hand if you want them gone.
