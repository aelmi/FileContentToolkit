# CodeShuttle

**Send your code to AI. Bring the answers back.**

CodeShuttle is a Windows desktop app that packs a codebase into a single block
of text you can paste into any AI chat — and then takes the AI's reply, diffs it
against your files, and applies it.

That second half is the point. Every other tool in this space is one-way: they
pack your code and stop. Getting the answer back is left to you, copying file
by file out of a chat window and hoping you did not paste into the wrong place.

> Target: **.NET 8 (Windows)** · WinForms · Windows 10 1809 or later, x64

---

## The round trip

```
   your repo                                            your repo
       |                                                    ^
       |  1. Pack          2. Paste        3. Paste back    |
       v      ---------->      ---------->      ---------->  |
   CodeShuttle           any AI chat          CodeShuttle ---+
                       (Claude, ChatGPT,       4. Review the diff
                        Gemini, local)         5. Apply what you accept
```

1. **Pack.** Point CodeShuttle at a folder. Filter by extension, `.gitignore`,
   ignore rules or by hand. Generate.
2. **Paste it into whatever AI you use.** No API key, no account, no per-token
   billing. CodeShuttle never talks to an AI service — you do, in the chat you
   already pay for.
3. **Paste the answer back.** Ctrl+Shift+V. CodeShuttle parses the reply,
   whether it came from a full pack or the model wrote files from scratch.
4. **Review the diff.** Per file, line by line, before anything is written.
5. **Apply.** Every file that will be overwritten is backed up first.

You never have to trust the model. You approve every file.

---

## Why not one of the CLI tools

`repomix`, `code2prompt`, `gitingest` and `ai-digest` all pack a repo well and
all stop there. The one comparable round-trip tool, **Repo Prompt, is macOS
only**. If you work on Windows, this is the option.

---

## It will not let you leak a credential

Before a pack leaves your machine, CodeShuttle scans it for things that look
like secrets and warns you:

- AWS access key IDs
- PEM private key blocks
- JWTs
- connection strings with an embedded password
- `.env`-style assignments whose value has high entropy

You can redact matches from the output with one toggle. The scan runs entirely
locally — nothing detected is transmitted, logged or written to disk. Your
original files are never modified by it.

This matters because the tool's default job is to take everything in a folder
and put it in a chat window.

---

## Everything else

**Selecting files**
- Browse, paste a path, or drag files in
- Filter by extension; a checkbox tree picker for hand-picking
- `.gitignore` support with correct anchoring and case sensitivity;
  `.dockerignore` as a separate opt-in
- Comma-separated ignore rules with a dedicated rule editor
- Max file size, skip-binary detection, per-file encoding detection
- A clickable "N files skipped" status item that tells you exactly what was
  excluded and why — a pack that quietly omits files is worse than no pack
- Recent folders and saved presets, both persisted

**Output**
- Plain text, Markdown, XML or JSON
- Editable in place, exportable, copyable
- Find and replace with regex, case and whole-word toggles
- Prompt templates, with Claude and ChatGPT variants built in

**Token budget**
- A gauge showing how much of your model's context the pack will use, and a
  per-file breakdown of where it went
- The number is a heuristic estimate, not a real tokenizer count, and is
  labelled as such wherever it appears

**Applying changes back**
- A real diff view before anything is written
- Encoding, line endings and trailing-newline state are preserved per file — an
  LF repository stays LF, a UTF-16 file stays UTF-16
- Every overwritten file is copied to
  `%APPDATA%\CodeShuttle\backups\<timestamp>\` with a manifest, before the first
  write
- Writes are staged through a temp file in the destination directory, so a
  failure cannot leave a half-written file
- Any bundle entry that would resolve outside the folder you chose is rejected
  and shown to you with the reason

**Other**
- Dark mode across every window, including the title bar and scrollbars
- Per-monitor V2 DPI awareness
- F1 opens help for whichever pane has focus; Shift+F1 opens contents
- Gzip + AES-GCM compression and encryption, from the **PROTECT** row above the output

---

## Requirements

- Windows 10 version 1809 or later, or Windows 11
- x64
- **No .NET runtime needed.** The installer ships a self-contained build.

Building from source needs the .NET 8 SDK.

---

## Install

Download the installer from the
[Releases page](https://github.com/aelmi/CodeShuttle/releases) and run it.

The installer offers a per-user or an all-users install. An all-users install
also enables long path support in Windows
(`HKLM\SYSTEM\CurrentControlSet\Control\FileSystem\LongPathsEnabled`), which
CodeShuttle needs to reach deeply nested files; a per-user install cannot set
this and skips it.

> **First run will show a SmartScreen warning.** CodeShuttle is not yet code
> signed — see `CHANGELOG.md`. Choose "More info" then "Run anyway". This will
> stop once signing is in place.

Uninstalling **deliberately leaves `%APPDATA%\CodeShuttle` in place** so your
presets, prompt templates and backups survive a reinstall. Delete that folder
by hand if you want it gone.

---

## Quick start

1. Ctrl+O and pick a folder.
2. Add the extensions you care about, or load a language preset.
3. Ctrl+G to generate. Check the token gauge, and the skipped-file count.
4. Ctrl+C, then paste into your AI chat with whatever you want done.
5. Copy the reply. Back in CodeShuttle, Ctrl+Shift+V.
6. Pick the target folder, read the diff, untick anything you do not want.
7. Apply.

---

## Keyboard shortcuts

| Shortcut | Action |
|---|---|
| `Ctrl+O` | Browse for folder |
| `Ctrl+Shift+O` | Add another folder |
| `F5` / `Ctrl+R` | Refresh file list |
| `Ctrl+G` / `F9` | Generate output |
| `Ctrl+C` | Copy output |
| `Ctrl+Shift+C` | Copy output as… (Markdown, XML, JSON) |
| `Ctrl+E` | Export output to a file |
| `Ctrl+Shift+V` | Paste AI response |
| `Ctrl+F` / `Ctrl+H` | Find and replace in output |
| `Ctrl+,` | Options |
| `Ctrl+P` | Presets |
| `Del` | Remove selected files or extensions |
| `Esc` | Cancel the running scan, generate or apply |
| `F1` | Help for the focused pane |
| `Shift+F1` | Help contents |

This table is generated from the same `Shortcuts.All` table the application
reads at runtime, so the in-app list and this one cannot disagree.

---

## Where your data lives

Everything is local, under `%APPDATA%\CodeShuttle\`:

| Path | Contents |
|---|---|
| `settings.json` | Options, presets, recent folders and searches, window position |
| `backups\` | Pre-overwrite copies taken before each apply |
| `logs\` | Crash reports, written only on a crash, never transmitted |

CodeShuttle makes exactly one network request: an update check against the
public GitHub Releases API. It sends a User-Agent and nothing else. See
[PRIVACY.md](PRIVACY.md).

---

## Building from source

```bash
git clone https://github.com/aelmi/CodeShuttle.git
cd CodeShuttle
dotnet build -c Release
dotnet test tests/CodeShuttle.Tests
```

The build runs with `TreatWarningsAsErrors` and .NET analyzers at
`AnalysisMode=Recommended`, and the project has **zero suppressions** — no
`NoWarn` entries and no `#pragma warning disable`. A warning is a broken build.

> **Verify with `dotnet build -c Release --no-incremental`.** A warm `obj/`
> reports "0 Warning(s)" whether or not warnings exist, because nothing
> recompiles.

To produce a release build and installer locally:

```powershell
.\build\publish.ps1 -Version 1.0.0 -Installer
```

Trimming and AOT are **not** available: `PublishTrimmed=true` fails outright with
`NETSDK1175` for Windows Forms, and this app would break specifically anyway —
settings and update parsing use reflection-based `JsonSerializer.Deserialize<T>`,
which trimming reduces to silently default-valued objects.

---

## Repository layout

The project is deliberately flat: a single WinForms project at the root, with
the test project under `tests\`.

| Namespace | Covers |
|---|---|
| `CodeShuttle` | `MainForm` and its partials, `FileContentService`, `FileRecreator`, `BundleFormat`, `PathSafety`, `TokenBudget` |
| `CodeShuttle.Dialogs` | Every dialog, including `PasteResponseForm` and `DiffViewerForm` |
| `CodeShuttle.Theming` | Token theme system: `ThemeTokens`, `ThemePalettes`, `ThemeApplier`, `ThemedForm` |
| `CodeShuttle.Controls` | `Toast`, `SearchBox`, `EmptyStateView`, `FocusRing` |
| `CodeShuttle.Filters` | `GitIgnoreParser`, `BinaryFileDetector`, `EncodingDetector`, `SecretScanner` |
| `CodeShuttle.Diagnostics` | `CrashLogger`, `UpdateChecker`, `DiagnosticsReport`, `AboutInfo` |
| `CodeShuttle.Settings` | `AppSettings` |

Colours are never written into a Designer file. Controls are tagged with a
`ThemeRole` and the applier resolves the palette, so adding a control and doing
nothing is correct rather than broken.

---

## The bundle format

```
>>>> CodeShuttle bundle v1
>>>> file: src\Program.cs
>>>> meta: lines=3; encoding=utf-8; eol=lf; eofNewline=true
using System;

class Program { }
<<<< end file
```

The parser reads exactly the declared number of lines and then verifies the
sentinel — it never scans inside an entry, so file content cannot forge a
header. Bundles written in the older `path:` format still parse.

---

## Documentation

- [CHANGELOG.md](CHANGELOG.md) — what changed, and what the owner must do
  before release
- [RELEASE-CHECKLIST.md](RELEASE-CHECKLIST.md) — everything outstanding before
  v1.0.0 ships: owner actions, two open design decisions, and the manual GUI
  verification passes that could not be automated
- [PRIVACY.md](PRIVACY.md) — what is stored and what leaves the machine
- [LICENSE.txt](LICENSE.txt) — end user licence agreement
- [THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt) — third-party attributions
- In-app help: F1

---

## License

Proprietary. See [LICENSE.txt](LICENSE.txt).

© 2026 MyCompany. `MyCompany` is a placeholder pending business registration —
see `CHANGELOG.md`.
