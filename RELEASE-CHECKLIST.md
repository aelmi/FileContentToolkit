# CodeShuttle — Release Checklist

Status as of the v1.0.0 overhaul: **code complete, QA gate passed (SHIP)**.
Build is clean cold with analyzers and zero suppressions; 275 tests pass.

Everything below is **outstanding**. Nothing here is a code defect — it is human,
business, or hardware-dependent work that no automated pass could complete.

---

## 1. Owner actions — business / accounts

- [ ] **Rename the GitHub repo `FileContentToolkit` → `CodeShuttle`.**
      Five user-visible URLs 404 until this happens: `UpdateChecker.Repo`
      (`UpdateChecker.cs:34`) and the four About-dialog links
      (`Diagnostics\DiagnosticsReport.cs:118-121`).
      **Rename the repo rather than reverting the code** — GitHub keeps redirects from
      the old name, so nothing pointing at the old URL breaks.
      The update check degrades safely (404 → `null` → no notice), but the four About
      links open a 404 page in the browser, which looks broken to a paying customer.

- [ ] **Replace the `MyCompany` placeholder in four places that must agree:**
      `Directory.Build.props:6-8` · `Diagnostics\DiagnosticsReport.cs:112` ·
      `LICENSE.txt:4,13` · `installer\CodeShuttle.iss:21`.
      Blocked on ASIC registration.

- [ ] **Decide `AboutInfo.Edition`** (currently `"Standard"`) — placeholder pending a
      decision on whether there will be more than one edition.

- [ ] **Delete the owner-note block at the end of `LICENSE.txt`** (from the
      `NOTE FOR THE PROJECT OWNER` divider) **and have the EULA reviewed by a lawyer
      before money changes hands.** It carries an Australian Consumer Law clause and
      Queensland governing law — a reasonable starting point, not legal advice.

- [ ] **`PRIVACY.md:92`** needs the registered entity name and a contact address.

- [ ] **Code signing.** Start procurement early — identity validation takes days to
      weeks and is the long pole. Hooks are already in place and commented at
      `build\publish.ps1:113-129`, `installer\CodeShuttle.iss:68-79`, and in the CI
      workflow. No `.pfx`/`.snk`/`.p12` is committed; `.gitignore:384-386` blocks all three.
      Until signed, SmartScreen shows **"Unknown publisher"** with Run hidden behind
      "More info" — a commercial problem, not a technical one.
      Note: Azure Trusted Signing (the cheapest CI-compatible option) requires a verified
      business identity with 3+ years of history, which the pending registration cannot
      satisfy. Budget for a commercial OV/EV certificate with a cloud HSM instead — a
      physical USB token cannot be used from GitHub Actions.

- [ ] **Make the first commit.** Everything from this overhaul lives in an uncommitted
      working tree against `d9e1320`. Until it is committed, the fresh-clone build check
      cannot run and none of the work is recoverable.

---

## 2. Two design decisions that need a human

- [ ] **Does `<ApplicationDefaultFont>` stay?** (`CodeShuttle.csproj:39`)
      It pins the layout baseline, which makes geometry deterministic — but means the app
      does not honour the Windows "Make text bigger" setting for layout, only for text.
      Removing it restores that responsiveness and reopens the geometry problem WS3 solved.
      **Pass 1's "Make text bigger" step below gives you the evidence to decide.**

- [ ] **Is a deployment scanner reading DPI awareness from `app.manifest` a requirement?**
      Some enterprise checks look for it there. It is deliberately absent: `WFAC010` fires
      on its mere presence, and removing it was the only suppression-free resolution.
      PerMonitorV2 is live via `<ApplicationHighDpiMode>`.
      If a scanner requires it, the only route back is reinstating the `WFAC010`
      suppression — make that an explicit decision, not a quiet revert.

---

## 3. Manual GUI verification — could not be automated

No implementation agent was permitted to launch the GUI, so these remain **UNVERIFIED**.
One session covers all of it. **Run the installed build, not a debug run** — the
`Environment.ProcessPath` icon fix is invisible in debug.

### Setup
Two monitors: primary **100%**, secondary **150%**; a third pass at **200%**.
Narrator available (Win+Ctrl+Enter). A real repo (≥500 files, mixed extensions), plus a
folder containing a `.env` with `AKIAIOSFODNN7EXAMPLE`.

### Pass 1 — DPI, all 16 forms
Open each, **drag to the 150% monitor, drag back**. Check: no blurring, no clipped text,
no control outside its parent, no overlap, no new scrollbars.

| # | Form | How to reach it |
|---|---|---|
| 1 | MainForm | launch |
| 2 | FolderTreePickerForm | Browse (Ctrl+O) → tree picker |
| 3 | ExtensionCountsForm | extensions panel → Refresh/summary |
| 4 | ExclusionRuleEditorForm | **Edit rules** beside the ignore-patterns box |
| 5 | OptionsForm | Ctrl+, |
| 6 | PromptComposerForm | Options → "Edit prompt templates…" *and* Tools ▸ Prompt templates |
| 7 | PresetManagerForm | Ctrl+P |
| 8 | PromptDialog | Save preset |
| 9 | FindReplaceForm | Ctrl+F |
| 10 | TokenBreakdownForm | budget strip → Breakdown |
| 11 | SecretWarningForm | scan the `.env` folder → Generate → Copy |
| 12 | PasteResponseForm | Ctrl+Shift+V |
| 13 | DiffViewerForm | Apply AI Changes… (or Review from #12) |
| 14 | PasswordDialog | Tools ▸ Compression ▸ Compress + Encrypt |
| 15 | HelpForm | Shift+F1 |
| 16 | AboutForm | Help ▸ About |

Specific checks:
- [ ] **PasswordDialog at 100/150/200%** — `chkShowPassword` fully inside its parent, both
      password rows visible, form *shrinks* when the confirm row collapses on the decrypt
      path. This is the form the entire DPI workstream was built around.
- [ ] **Every dialog shows the app icon** in its title bar and Alt+Tab (single-file build only).
- [ ] **MainForm at 200%** can be positioned and resized freely (`MinimumSize` is now 760×520).
- [ ] **Windows "Make text bigger" at 150%** — open MainForm, Options, About. Text grows,
      boxes do not. Note any *clipping*; this is the evidence for decision §2.1.
- [ ] **Toggle Dark mode with several dialogs open**, then reopen each. No white flash,
      title bars darken, PresetManager's details pane stays distinct from the body.
- [ ] **About dialog link hover/press states in dark mode** — the active-link colour was
      repointed during the QA fix-up; perceptual distance from the resting colour is reduced
      (ΔE 51 → 11, still above threshold). Confirm it reads correctly.

### Pass 2 — keyboard only
**Unplug the mouse.** Ctrl+O pick folder → add extension → Tab the file list → F5 →
Ctrl+G generate → Ctrl+F find → Esc → Ctrl+Shift+C copy-as → pick format → Ctrl+E export →
Ctrl+, options → Esc → Ctrl+P presets → Esc.
- [ ] **SplitButton dropdowns** (Copy as…, Recent ▾, Presets ▾) open on **Alt+Down** and **F4**.
- [ ] **Escape closes all 16 forms.** ExtensionCountsForm is the historical failure.
- [ ] **Enter in DiffViewerForm does NOT write to disk** — `AcceptButton` is deliberately null.
- [ ] **Enter in PresetManagerForm loads** the selection.
- [ ] Tab order reads left-to-right / top-to-bottom in every container.
- [ ] **Focus ring visible on every focused control**, including on saturated accent fills.
- [ ] **F1 is contextual** — file list → *Selecting Files*; output pane → *Building the Pack*;
      search box → *Searching*. Shift+F1 always opens *Getting Started*.

### Pass 3 — Narrator
Repeat Pass 2 with Narrator on, low speed. Each must announce a **name**, not a glyph:
`btnBrowse`, `btnMoveUp`/`btnMoveDown`, search recents `▾`, `chkCase`/`chkWord`/`chkRegex`,
`lstFiles`, `lstExtensions`, `rtbOutput`, `gridCounts`, the checkbox `tree`, `lstPresets`,
`lstFilePlans`, `rtbDiff`, `txtPassword`, `txtConfirm`.
- [ ] `txtPassword`/`txtConfirm` announce as **password fields with a label**.
- [ ] `PromptDialog`'s input announces its prompt (it is the generic text-input primitive).
- [ ] `PresetManagerForm`'s details pane announces on selection change.
- [ ] Status and progress changes are announced.

### Pass 4 — packaging
- [ ] `git clone` after the first commit → `dotnet build -c Release --no-incremental` → 0/0.
- [ ] `CodeShuttle.exe` launches on a machine with **no .NET Desktop Runtime installed**.
- [ ] Install → launch → uninstall → confirm `%APPDATA%\CodeShuttle` **survives**
      (presets, templates, backup sets).
- [ ] About ▸ Third-party notices renders the full `THIRD-PARTY-NOTICES.txt`.
- [ ] Capture the `assets\` screenshots (see `assets\README.md`) and link them from the README.

---

## 4. Known open items (deliberately not fixed — P3, out of scope)

Recorded so they are decisions rather than oversights:

- Search results can highlight wrong rows if a watcher rescan lands inside the search's
  `await` (bounds-checked, silent, not a crash) — `MainForm.cs:1047,1071,1084-1088`
- `Border` token is 1.39–1.89:1 against surfaces, below WCAG 1.4.11's 3:1 — arguable, since
  fills also differentiate — `ThemePalettes.cs:39,80`
- `ShowCrashNotice` can raise a modal on the finalizer thread — `Program.cs:28`, `CrashLogger.cs:61-65`
- `SplitButton` keeps dead `SystemColors` fallbacks that `ThemeApplier` always overwrites — `SplitButton.cs:26,29`
- Cancelling during the *backup* phase reports nothing; cancelling during *write* goes modal — `MainForm.cs:921-927`
- Copy on an empty output pane is a silent no-op — `MainForm.cs:719`
- `ExecuteAsync` takes `IEnumerable<FilePlan>` so it cannot check `CanProceed` itself; both
  callers check correctly, but the invariant lives in comments rather than types — `FileRecreator.cs:273`
- `FolderTreePickerForm` duplicates enumeration policy instead of using `BuildEnumerationOptions()`,
  and is already drifting — `FolderTreePickerForm.cs:102-115`
- `IsTrustedReleaseUrl` accepts `*.github.com`, wider than specified — `MainForm.Extra.cs:195-196`
- `BtnCompressEnc_Click` has no secret gate (defensible — the blob is AES-GCM and unreadable
  to an AI — but the asymmetry with plain compress is now a decision) — `MainForm.cs:392-418`
- Redundant statistics pass after Generate (direct call + debounce both fire) — `MainForm.cs:1478,1497`

---

## 5. Deferred to v2 (scope decision, not defects)

`src/` + `CodeShuttle.Core` classlib extraction · full MVVM service-layer extraction ·
CLI mode · WebView2 Markdown help with topic tree and search · licensing/activation ·
Myers O(ND) diff · nested `.gitignore` support · high-contrast theme · system-theme
following · localization · PBKDF2 100k→600k and key zeroing (the versioned header now
stores the iteration count, so raising it is a one-line back-compatible change) ·
icon set to replace text-only labels · win-arm64 / MSIX.
