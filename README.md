# 📂 File Content Toolkit

A **Windows Forms application** that **scans folders**, **filters by extension or rule**, **bundles file contents** into a single output, **searches**, **encrypts**, and **recreates** files from that output.

Useful for developers preparing context for LLMs, content reviewers, automation workflows, code-review prep, or anyone who needs to audit / package / restore a tree of files.

> Target: **.NET 8 (Windows)** · WinForms

---

## 🔧 Features

### File selection & scanning
- Browse folders or paste a path; subfolder scan is on by default
- Filter by extension (`.cs`, `.txt`, `.json`, …) — multiple at once
- **Tree-view picker**: lazy-loaded `TreeView` with checkboxes for hand-picking files/folders
- **Recent folders dropdown** (LRU, last 15) — survives restarts
- **Ignore patterns** (comma-separated globs: `*.tmp, bin/`)
- **.gitignore / .dockerignore** parsing from the folder root (negation, anchors, `**`, `?`)
- **Max file size filter** to skip giant blobs
- **Skip-binary detection** via null-byte heuristic on first 8 KB
- **Auto-detect encoding** per file (BOM-first, then UTF-8 validation, then user-selected fallback)
- **Drag-and-drop** files into the list

### Background scanning
- Fully async scan (`RefreshFilesAsync`) on a worker thread; cancellable
- In-flight scans are cancelled when the folder, extensions, or ignore rules change
- TextChanged on path / ignore patterns is **debounced 400 ms** so a scan only kicks off when typing pauses
- Progress reported only on integer-percent changes (no UI marshalling storm)

### Folder watching
- **`FolderWatcher`** wraps `FileSystemWatcher` with a 600 ms debounce — bursts of file events collapse into a single refresh
- Off by default; toggle the **Watch folder** checkbox in the top toolbar

### Output generation
- One-click **Generate** assembles every selected file into the right-hand output pane
- File reads + string assembly run off-thread; output is assigned once and styled under suspended redraw (one repaint instead of N)
- Per-file headers are colored & bold; output stats (chars / lines / bytes) shown below

### Search
- Search across selected files with **regex**, **match case**, **whole word** toggles
- Total match count + file count reported in the UI
- **Recent searches** dropdown (LRU, last 15)

### Find & Replace in the output
- Modeless `FindReplaceForm` opens via **Ctrl+F** or **Ctrl+H** anywhere in the output
- Regex / case / whole-word toggles, **F3 / Shift+F3** for next / previous, live match count
- Full Replace / Replace All (output must be in Edit mode)

### Presets
- **Save preset** snapshots the current folder + extensions + ignore patterns + IncludeSubfolders
- **Presets ▾** lists saved presets and includes a **Manage presets…** dialog (load, rename, delete)
- Persisted in JSON at `%APPDATA%\FileContentToolkit\settings.json`

### Output toolbox
- **Copy / Edit / Export** the output
- **Compress / Decompress** GZip + Base64
- **Compress+Encrypt / Decompress+Decrypt** with AES-GCM (password-protected)

### Recreate files
- Click **Recreate Files**, pick a base folder, and the app reconstructs every file from the output, preserving relative paths

### Help & About
- **Help** menu:
  - **Keyboard Shortcuts…** (F1) — themed reference with shortcuts + feature overview
  - **About…** — version (from assembly), copyright, link to the settings folder

---

## ⌨️ Keyboard Shortcuts

| Shortcut | Action |
|---|---|
| `Ctrl + F` / `Ctrl + H` | Open Find & Replace in the output pane |
| `F3` / `Shift + F3` | (in Find dialog) next / previous match |
| `Esc` | (in Find dialog) close |
| `Delete` | Remove selected file(s) from the list |
| `Enter` | (in extension box) add the typed extension |
| `F1` | Open the Help dialog |

---

## 🖥️ Interface

```
┌──────────────────────────────────────────────────────────────────────┐
│  Help                                                                 │  Menu
├──────────────────────────────────────────────────────────────────────┤
│  Folder Path:  [.........................................] [...]    │
│  [Tree][Recent ▾][Options][Save preset][Presets ▾]  ☐ Watch folder   │  Toolbar
├──────────────────────────────────┬────────────────────────────────────┤
│ File Extensions                  │ Output                             │
│ Selected Files                   │  ┌──────────────────────────────┐ │
│  Search  ☐Aa ☐Word ☐.* [F/R]     │  │ (concatenated text)          │ │
│  [ list of files ]               │  │ ...                          │ │
│ Files: N   [Add][Remove][▲][▼]   │  └──────────────────────────────┘ │
├──────────────────────────────────┴────────────────────────────────────┤
│                          [ ▶  GENERATE ]                              │
└───────────────────────────────────────────────────────────────────────┘
```

---

## ⚙️ Options

Available from the **Options** button in the top toolbar:

| Setting | Default |
|---|---|
| Max file size (KB, 0 = unlimited) | 0 |
| Skip binary files | On |
| Auto-detect encoding (BOM + UTF-8 fallback) | On |
| Apply `.gitignore` / `.dockerignore` from folder root | On |
| Watch folder for changes and auto-refresh | Off |

All settings, recent folders, recent searches, and presets persist to JSON in `%APPDATA%\FileContentToolkit\settings.json`.

---

## 🏗️ Project Structure

| Namespace | Files |
|---|---|
| `FileContentToolkit` (root) | `MainForm.{cs,Designer.cs,resx}`, `MainForm.Extra.cs` (plumbing only), `FileContentService.cs`, `FileRecreator.cs`, `CompressionUtils.cs`, `ExtensionCountsForm.*` |
| `FileContentToolkit.Dialogs` | `OptionsForm.{cs,Designer.cs,resx}`, `PresetManagerForm.cs`, `FolderTreePickerForm.cs`, `FindReplaceForm.cs`, `AboutForm.{cs,Designer.cs,resx}`, `HelpForm.{cs,Designer.cs,resx}`, `PasswordDialog.*` |
| `FileContentToolkit.Filters` | `GitIgnoreParser.cs`, `BinaryFileDetector.cs`, `EncodingDetector.cs` |
| `FileContentToolkit.Settings` | `AppSettings.cs` (also defines `Preset`) |
| `FileContentToolkit.Watcher` | `FolderWatcher.cs` |
| `FileContentToolkit.UI` | `Theme.cs` (palette + button factories + AppIcon), `ThemedPrompt.cs`, `SplitButton.cs` |

The main form is **100 % designer-backed** — every visible control is declared and configured in `MainForm.Designer.cs`. `MainForm.Extra.cs` carries only cross-cutting plumbing (settings load/save, folder watcher lifecycle, event handlers wired by the designer).

---

## 🎨 Theme

Single source of truth in `Theme.cs`:

| Token | Value | Used for |
|---|---|---|
| Header    | `#0066CC` | Page-header strip |
| Primary   | `#3375B7` | Primary buttons |
| Action    | `#0D6EFD` | Accent / Find&Replace |
| Success   | `#28A745` | Add / Recreate / Presets |
| Danger    | `#DC3545` | Remove |
| Secondary | `#6C757D` | Neutral / Cancel |
| FormBg    | `#F5F7FA` | Form background |

`Theme.ApplyForm(f)` applies background + font + icon in one call. `Theme.AppIcon` is lazy-extracted from the running executable so every dialog shares the main app icon.

---

## 🚀 Getting Started

```bash
git clone https://github.com/aelmi/FileContentToolkit.git
cd FileContentToolkit
dotnet build
dotnet run
```

Or open the solution in **Visual Studio 2022+** and press F5.

---

## 📤 Output Format

When you click **Generate**, each file is emitted as:

```
C:\MyProject\Program.cs:
using System;

class Program {
    static void Main() { }
}
```

Files are separated by blank lines. The output is plain text, can be edited in place, and can be **Recreated** back into a folder while preserving relative paths.

---

## 🔁 File Recreation

Click **Recreate Files**, choose a target folder, and the parser:
- Walks the output looking for `path:` headers
- Creates each file, plus any intermediate directories
- Preserves the original relative-path structure

Works on the original output **and** on output that's been re-imported after editing or after decryption.

---

## 📄 License

Not specified yet — add a license under the repo's GitHub settings.
