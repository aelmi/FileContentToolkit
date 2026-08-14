# Changelog

All notable changes to CodeShuttle are recorded here. The format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and this project uses
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## Required before the first public release

These are **owner actions**, not code changes. The build is green without them,
but the release is not correct without them.

### 1. Rename the GitHub repository to `CodeShuttle` — REQUIRED

The repository is still named `FileContentToolkit`. Five places in the shipped
application already assume the new name:

| Location | Value |
|---|---|
| `UpdateChecker.Repo` | `CodeShuttle` |
| `AboutInfo.WebsiteUrl` | `https://github.com/aelmi/CodeShuttle` |
| `AboutInfo.DocsUrl` | `https://github.com/aelmi/CodeShuttle#readme` |
| `AboutInfo.ReleaseNotesUrl` | `https://github.com/aelmi/CodeShuttle/releases` |
| `AboutInfo.ReportBugUrl` | `https://github.com/aelmi/CodeShuttle/issues` |

Until the repository is renamed, all five 404. The update check degrades
safely — a 404 yields `null` and no notice is shown — but the four About links
open a GitHub 404 page in the user's browser, which looks broken.

**Rename the repository rather than reverting the code.** GitHub permanently
redirects the old name to the new one, so existing clones, links and the CI
workflow keep working. Reverting the code would ship the old product name in a
user-visible URL.

Settings → General → Repository name → `CodeShuttle` → Rename.

The release workflow does **not** depend on this: it uses `${{ github.repository }}`
and never hardcodes a name, so it succeeds under either name.

### 2. Replace the "MyCompany" placeholder

`MyCompany` is a placeholder pending ASIC registration. When the entity is
registered, the registered name must be set in **four** places, which must
agree:

- `Directory.Build.props` — `<Company>`, `<Authors>`, `<Copyright>`
- `Diagnostics\DiagnosticsReport.cs` — `AboutInfo.CopyrightHolder`
- `LICENSE.txt` — the Licensor, plus delete the owner-note block at the end
- `installer\CodeShuttle.iss` — `AppPublisher`

`AboutInfo.Edition` is likewise a placeholder (`"Standard"`) pending a decision
on whether more than one edition will exist.

### 3. Code signing

Deferred, blocked on business identity. The installer is built signing-**ready**:
`installer\CodeShuttle.iss` carries a commented, documented `SignTool`
directive, and the release workflow has a commented signing step in the correct
position (after publish, before packaging). Until then, SmartScreen shows
"Unknown publisher" on first run. This is expected and is a commercial problem,
not a technical one.

No `.pfx`, `.snk` or `.p12` is committed, and `.gitignore` blocks all three.

### 4. Manual QA still outstanding

Two acceptance criteria from earlier workstreams require a running GUI and were
never verified:

- Dual-monitor 100% / 150% DPI drag across all 16 forms.
- Keyboard-only walkthrough and a Narrator pass.

---

## [Unreleased] — 1.0.0

The first commercial release. CodeShuttle was previously an internal utility
called File Content Toolkit; this release is a rename plus a substantial
overhaul, and it is not a drop-in continuation of that tool.

### Added

- **Round-trip workflow.** Pack a codebase, paste it into any AI chat, then
  paste the answer back into CodeShuttle, review it as a diff, and apply it.
  "Paste AI response…" (Ctrl+Shift+V) and "Apply AI Changes…".
- **Backups before every apply.** Every file that is about to be overwritten is
  copied to `%APPDATA%\CodeShuttle\backups\<timestamp>\` with a manifest, before
  the first write.
- **Secret detection.** AWS key IDs, PEM private key blocks, JWTs, connection
  strings with embedded passwords, and high-entropy `.env` values are detected
  and flagged before they leave the machine. Optional redaction in output.
- **Token budgeting.** A per-model gauge and a per-file breakdown, so a pack
  that will not fit is visible before it is sent. The number is an estimate, not
  a real tokenizer count, and is labelled as such everywhere it appears.
- **Prompt templates**, editable, with Claude and ChatGPT presets wired up.
- **Token-based theme system** with a real dark mode across all 16 forms, plus a
  dark title bar and dark native scrollbars.
- **Per-monitor V2 DPI awareness.**
- **Full keyboard shortcut set** with a single source of truth, accessible names
  throughout, focus rings, and Accept/Cancel buttons on every dialog.
- **F1 contextual help** — 9 embedded topics, opening on the topic for the
  focused pane. Shift+F1 for contents.
- **About dialog** with version, copyright, third-party notices rendered from
  the real notice file, and a "Copy diagnostics" support blob that is
  structurally incapable of carrying a scanned path.
- **Window and splitter position** persist across sessions.
- **Skipped-file reporting** — a clickable status item explains exactly what was
  excluded and why, instead of silently producing an incomplete bundle.
- **Cancellation** on scan, generate and apply.
- `LICENSE.txt`, `THIRD-PARTY-NOTICES.txt`, `PRIVACY.md`, this changelog.
- Inno Setup installer, publish script, and a GitHub Actions release workflow.
- **270 unit tests**, where there were none.

### Fixed — binary files reaching the pack

- **A dropped or picked binary was added and packed.** The folder scan has always
  classified its candidates and reported what it left out, but the two explicit
  routes — dropping files on the list, and the Add files dialog — bypassed it and
  added whatever they were handed. Dropping an `.ico` produced a bundle entry
  whose body was a .NET decoder error (`Unable to translate bytes [ED]…`).
- **That error string was written into the bundle where the file's source
  belongs.** A bundle is designed to be pasted to an AI and applied back to disk,
  so the message sat one round trip away from being written into the user's real
  file. A file that cannot be read now contributes no entry at all.
- Both explicit routes screen with the same `BinaryFileDetector` the scan uses,
  and report through the same clickable "N files skipped" status indicator.
  An explicitly chosen file still outranks the configured extension filters —
  naming a file is a clearer statement of intent than a filter list — but being
  unreadable is refused either way.

### Fixed — Generate

- **Generate appeared to do nothing on a large pack.** Styling the file headers
  ran a Select / SelectionColor / SelectionFont triple per header, and the RTF
  engine reflows from the selection point on each one, at a cost that grows with
  the size of the document. On a 1.1 MB pack of 124 files the loop ran for over
  a minute *after* the text was already in the pane, so the window sat there
  looking frozen with an apparently dead button. Suppressing redraw did not help
  — the cost is in the engine, not the painting. Header highlighting is now
  skipped above 200,000 characters and the status bar says so. Measured on the
  same pack: **70s → 0.9s**.
- **The generated pack was invisible.** The empty state that covers the output
  pane was refreshed only from the file and extension model, which Generate does
  not touch, so a pack landed behind "No pack yet". It was counted in the
  statistics and could be copied; the window simply never showed it.
- **Closing the window after generating threw a .NET crash dialog.** Closing
  destroys the form's handle, which cascades to every child; a RichTextBox keeps
  its document in the native window and re-creates its handle to preserve it, so
  the dispose walk reached a control in the middle of CreateHandle and
  `Control.Dispose` threw. Closing mid-generate hit the same fault through the
  async continuation. Both paths are now gated on a teardown flag set at
  FormClosing, and the output pane is never hidden — hiding it left the control
  without a handle at all, which was the original trigger.

### Changed — project-type presets

- **Presets ▸ Project type** replaces the flat list of sixteen languages. A
  language is not what anyone has — they have a WinForms app, a Django site or a
  Next.js front end — and the extensions those need differ sharply within one
  language. The old "C# project" entry offered `.razor` and `.cshtml` to someone
  building a desktop app and no `.xaml` at all to someone building WPF.
- Thirty-one entries across .NET, Python, Web, Mobile, JVM, Systems, Other and
  Content, reachable from both the Presets menu and the extension chips'
  "+ add" menu, built from one catalogue so the two cannot drift.
- **Each preset also carries the build output worth excluding** for that stack:
  `bin/ obj/` for .NET, `node_modules/ dist/` for Node, `__pycache__/ .venv/`
  for Python, `target/` for Rust. A pack that sweeps in `node_modules/` will not
  fit in any context window ever built.
- Selecting a project type **replaces** the extension list — it is a statement
  about what the project is — but **merges** the ignore list, which is usually
  hand-tuned and must not be silently discarded. The status bar reports both.

### Changed — the main window

The window is a pipeline — pick a source, filter it, pack it, ship it and apply
the reply — and the previous arrangement expressed none of that. The stages were
spread across an accent-filled header band, two group boxes whose titles named
controls rather than steps, a Generate button floating alone at the bottom
centre, and three menus. The pipeline is now the layout.

- **The accent header band is gone.** It was the loudest element in the window
  and carried no information. The palette is a cool-slate neutral with a single
  deep viridian accent; colour that is not the accent is semantic (destructive,
  caution) and nothing else is coloured. Every contrast pair still clears
  WCAG AA in both palettes, with the tightest at 4.79:1.
- **One button hierarchy.** Exactly one filled accent button on screen —
  Generate. Second-tier actions are outlined, third-tier are ghosts, and red
  appears only on destructive actions. Previously red Remove, green Save preset,
  green Add files, blue Add, blue Refresh and blue Find/Replace all competed, so
  nothing read as the primary action.
- **Extensions are chips.** A list box showing five at a time plus three stacked
  buttons became removable pills with an inline "+ add". All filters are visible
  at once and removal is a click on the thing being removed. Custom extensions
  are typed through "+ add ▸ Custom extension…", which routes to the same
  validation as before.
- **The rail reads Source → Filters → Files**, with counts on the section
  headers.
- **The search field carries its own modifiers.** Match case, whole word and
  regex moved inside the field. The caption they sat under is gone, and the
  access key it carried is replaced by Ctrl+Shift+F, which reaches the box from
  anywhere in the window.
- **A real empty state.** `EmptyStateView` existed in the codebase and had never
  been wired into the main window; the output pane was a blank rectangle. It now
  explains what a pack is and offers the action that produces one.
- **Round-trip is permanent.** Paste-reply and apply-changes were behind a
  dismissible banner shown only once output existed — invisible to anyone who
  had not generated a pack, and permanently gone for anyone who had once clicked
  hide. Pasting a reply does not need a pack, so gating its entry point on one
  was backwards.
- **The budget strip is one line** under the output it describes, instead of a
  full-width stripe of five controls.
- The window is assembled in `MainForm.Layout.cs` after `InitializeComponent`.
  Every control keeps the field name, event wiring, accessible name and tooltip
  the designer gave it, which is what let a restructure of this size leave the
  handler logic untouched.

### Changed

- **Renamed from File Content Toolkit to CodeShuttle.** Settings move from
  `%APPDATA%\FileContentToolkit\` to `%APPDATA%\CodeShuttle\`; existing settings
  are migrated automatically on first run, so presets and recent folders
  survive.
- **The bundle format is now framed** — each entry declares its line count,
  encoding, line-ending style and trailing-newline state, terminated by a
  sentinel. Bundles written by the old format still parse; the legacy reader is
  retained.
- The compression/encryption actions moved from the main surface into
  **Tools ▸ Compression and encryption**, and are now back on the main surface
  as a **PROTECT** row directly above the output box: Compress, Decompress,
  🔒 Encrypt… and 🔓 Decrypt…, as buttons. They were four buttons originally,
  were moved out because they competed with Generate, and returned briefly as
  one split button; they are buttons again because this is the capability that
  lets a pack leave the machine safely, and a capability behind a caret is one
  nobody finds. **Edit** joined that row and left the pack header, because it
  acts on the text in the pane like the other four rather than on the pack like
  Export, Copy and Generate. The Tools submenu still works and shares the same
  handlers.
- Each protect button is enabled against what the output pane actually holds,
  read from the blob's own magic header. Plain text can be compressed or
  encrypted; a compressed pack can only be decompressed; a sealed one can only
  be decrypted. Encrypting an already-encrypted pane would have produced a blob
  requiring two passwords in the right order, with neither recorded anywhere.
- **The accent is now blue.** The palette moved off the deep viridian to a
  three-blue family: bright cobalt for the one filled accent fill, azure for
  the focus ring and (deepened to carry as text) for on-surface links and
  chips, and light sky blue for selection washes — swapping roles with cobalt
  in dark mode, where a fill dark enough to carry white text is too dark to
  read against the ground. The neutral ramp is sky-tinted rather than grey.
  Warning stays amber on purpose: a caution banner in the accent hue is not
  read as a caution. Every pair asserted in `ThemeContrastTests` still clears
  4.5:1 for text and 3:1 for chrome in both palettes.
- "Recreate Files" is now "Apply AI Changes…".
- Routine success messages became transient toasts. Modal dialogs are now
  reserved for genuine errors and destructive confirmations.
- Encrypted blobs carry a magic header (`CSHT`) with a version, flags and the
  **stored KDF iteration count**, so the parameters can be raised later without
  breaking existing blobs. Legacy headerless blobs still decrypt.
- Update checks now ignore prereleases, so publishing a beta no longer offers
  that beta to users on the stable channel.
- `.dockerignore` is now an explicit opt-in, separate from `.gitignore`.

### Fixed — security and data loss

- **Path traversal on apply.** A crafted bundle header could write outside the
  chosen folder — for example into the Startup directory. Every target is now
  resolved and containment-checked; rejected entries are shown in the diff with
  a reason rather than silently dropped.
- **UNC bundles flattened and overwrote each other.** Three files at different
  paths could resolve to one target, destroying two of them silently. Root
  computation is fixed, and `Plan` now refuses outright if two entries resolve
  to the same target.
- **Recreation destroyed encoding and line endings.** Files were written as
  UTF-8 without BOM with CRLF line endings regardless of what they had been.
  UTF-16 files became unreadable; LF repositories showed every line as changed;
  shell scripts broke. The knock-on was worse: because the comparison saw its
  own damage, almost every file was reported as *Modified* even when identical,
  so users accepted a wall of spurious diffs and rewrote their tree mangled.
- **Decompression bomb.** An unbounded gzip expansion ran synchronously on the
  UI thread; roughly 2 MB of input could reach 2 GB of output. Now capped,
  cancellable, and off-thread.
- **No password confirmation on encryption.** A single typo produced a
  permanently undecryptable blob, and the plaintext was destroyed in the same
  action by overwriting the output pane. There is now a confirm field, an
  enforced minimum length, and the output pane is no longer overwritten.
- **Encoding override corrupted files silently.** Reading a UTF-8 file as ASCII
  replaced every non-ASCII byte with `?`, and that corrupted text was written
  back over the user's real files.
- Content could forge a bundle header, splitting one file into two and writing
  the tail to a fabricated path.

### Fixed — crashes and hangs

- Out-of-memory opening the diff viewer on large files (a 20k-line pair
  allocated 1.6 GB in one block).
- GDI handle exhaustion in the diff viewer and the split button — one `Font`
  leaked per rendered line, and at ~10,000 handles the whole application stops
  drawing and then dies.
- A single access-denied folder aborted an entire scan.
- Symlink and junction loops recursed until failure.
- The folder watcher died permanently on buffer overflow — routine during a
  build — and never recovered, with the checkbox still ticked.
- A superseded scan could overwrite a newer scan's results.
- Regex search could hang the UI permanently on a catastrophic-backtracking
  pattern.
- Settings could be silently reset to defaults by a truncated write; writes are
  now atomic and a corrupt file is quarantined with a message rather than
  discarded.

### Fixed — correctness

- `.gitignore` anchoring, case sensitivity, and per-scan regex recompilation.
- User ignore patterns were substring matches (`bin` excluded `Robinson.cs`) and
  the documented `dir/` form never matched anything on Windows.
- Version display: About, the update check and crash reports all read a version
  pinned at `1.0.0.0`, which made the update check claim an update was available
  on every launch for every user.
- Reordered files lost their selection; dropped directories were added as files.
- XML output did not escape `"` in a path attribute.
- Token estimates were optimistic in the one direction that matters.
- Failures were reported through the 3.2-second toast, which dismissed itself
  before it could be read and painted its text rather than hosting it, so the
  message could not be copied. Every blocked action now opens `MessageDialog`:
  themed, modal, selectable, with Ctrl+C and a Copy button. Toast is retained
  for confirmations only ("copied", "cancelled", "up to date").
- "Apply AI Changes" on a compressed or encrypted output pane reported "No file
  entries found in the output. Generate first, then try again." — advice for a
  different problem, which sent the user off to regenerate a pack they already
  had. The blob is now recognised and the message names the actual next step
  (Tools ▸ Decompress output).

### Developer

- `TreatWarningsAsErrors` plus .NET analyzers at `AnalysisMode=Recommended`,
  with `EnforceCodeStyleInBuild`. **Zero suppressions** — no `NoWarn` entries and
  no `#pragma warning disable` anywhere in the project.
- Deterministic builds; `ContinuousIntegrationBuild` conditioned on CI so
  step-into debugging still works locally.

### Deferred to a later release

CLI mode · WebView2 help with search · licensing and activation · Myers diff ·
nested `.gitignore` files · high-contrast theme and system-theme following ·
localization · raising PBKDF2 iterations to 600k (the format already stores the
count, so this is a one-line change with back-compat built in) · an icon set to
replace the remaining emoji · win-arm64 and MSIX packaging.
