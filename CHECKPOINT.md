# CodeShuttle — working checkpoint

**Started:** 2026-08-14
**Repo:** `C:\Users\alelm\OneDrive\Projects\FileContentToolkit`
**Branch:** `master` · HEAD `d9e1320` · ~86 files uncommitted *before* this session's work

---

## Why this session started

Al reported that Encrypt / Decrypt / Encrypt-with-password / Decrypt-with-password had
disappeared from the main page, and asked for them back plus a full UX redesign.

## What was actually true

The feature was **never deleted**. It was **moved off the main surface into
`Tools ▸ Compression and encryption`**, and that move lives only in the **uncommitted working
tree** — no commit removed it.

| | |
|---|---|
| Added | `fc2997f` (2026-02-15), as four buttons in a `pnlCompressionTools` strip on `pnlOutput` |
| Still present at | `d9e1320` (HEAD) — the designer block is byte-identical from `fc2997f` to HEAD |
| Moved to Tools | uncommitted working tree only |
| Rationale on record | `MainForm.Designer.cs:1541`, `CHANGELOG.md:228`, `Help/reference.md:36` |

Crypto, handlers and `PasswordDialog` were all alive and **better** than the 2026-02 originals
(async, secret-gate, password confirmation, no longer destroys plaintext). So this was a
UI-surface job, not a resurrection. **Do not restore the old handler bodies from `d9e1320`** —
they are the regressed versions.

### Crypto spec (unchanged, for reference)
UTF-8 → GZip → Base64 → AES-GCM (256-bit, 16-byte tag) → Base64.
PBKDF2/SHA-256, 100,000 iterations, 16-byte salt, 12-byte nonce.
Layout: `magic "CSHT" (4) | version (1) | flags (1) | iterations (int32 LE) | salt (16) | nonce (12) | tag (16) | ciphertext`.
Legacy headerless blobs still decrypt. Suggested extension `*.cshtx`.

---

## Done this session

1. **The four actions are on the first page as buttons.** A **PROTECT** row (`pnlProtectTools`)
   docked directly above the output box, holding Edit, Compress, Decompress, 🔒 Encrypt… and
   🔓 Decrypt…, wired to the *existing* handlers. The Tools submenu still works and shares them.
   - It shipped first (2026-08-14) as a single `Protect ▾` split button in the pack header.
     **Superseded 2026-08-15 at Al's request** — he wanted the buttons visible, not behind a
     caret. `btnProtect` / `cmsProtect` and their two handlers are gone.
   - **Edit moved out of the pack header into this row**, so it is not duplicated. It belongs with
     the four because it acts on the text in the pane; Export, Copy and Generate act on the pack.
   - Enabling is per button, sniffed from the blob's magic prefix in `UpdateOutputPresence`:
     plain → Compress/Encrypt, compressed → Decompress, sealed → Decrypt. Encrypting twice would
     produce a blob needing two passwords in order, with neither recorded.
   - Button widths are **measured** (`TextRenderer.MeasureText`), not a multiple of `Font.Height`,
     because the labels differ in length and a fixed multiple clips the longer ones.
   - Files: `MainForm.Designer.cs`, `MainForm.Layout.cs` (`BuildProtectStrip`), `MainForm.cs`.
2. **Palette moved to blue** (Al's call, mid-session): bright cobalt `#0B57D0` accent fill, azure
   `#007FFF` focus ring / `#0A4FBF` on-surface text accent, light sky blue `#E1EDFD` selection
   wash. Dark mode swaps sky and cobalt. `Theming/ThemePalettes.cs`.
3. **Progress bars are blue.** `NativeTheming.cs` stripped the visual style in dark mode only, so
   light mode rendered the stock Windows green (comctl32 ignores `ForeColor` while a style is
   active). Now stripped in both. The same edit fixed combo drop-downs staying light in dark mode
   (`DarkMode_CFD`, not `DarkMode_Explorer`).
4. **Two pre-existing build blockers fixed** — CA1875 (`Regex.Matches(..).Count` → `Regex.Count`)
   in `FindReplaceForm.cs` and `MainForm.cs`. `TreatWarningsAsErrors=true`, so the project did not
   compile at all before this.
5. **Docs corrected twice** — `Help/reference.md`, `README.md`, `CHANGELOG.md` first claimed the
   actions were deliberately *not* on the main surface, then described the split button that the
   PROTECT row replaced.
6. **`tests/CodeShuttle.Tests/ProtectButtonTests.cs`** pins the row to the main surface, pins Edit
   into it and out of the header, pins the strip's position between header and output box, and
   pins the enable-gating matrix. One test clicks the real Compress button and asserts the pane
   becomes a real compressed blob — it needs a `PumpUntil` helper because the handlers are
   `async void` and no test pumps the message loop.

### Verification
- `dotnet build` — 0 warnings, 0 errors.
- `dotnet test` — **309/309 pass**, including `ThemeContrastTests` on both palettes.
- Real app launched and screenshotted: the PROTECT row renders above the output box with all five
  buttons, correctly disabled with no pack. Blue palette confirmed live.
- ⚠ **Not** driven end-to-end live. Every route to selecting files needs either the shell file
  dialog (whose filename box is not exposed to UI Automation) or the chip "+ add" popup (which
  closes when the driving script exits). Forcing foreground activation to work around that was
  blocked by Defender/AMSI as a foreground-hijack pattern — correctly, and not worked around.
  The click-through is covered by test instead.

---
## Landmines (found by survey — read before any layout work)

- **`MainForm.Designer.cs` does not describe the window you see.** `BuildLayout()` in
  `MainForm.Layout.cs:68` calls `Controls.Clear()` and rebuilds everything. Layout.cs wins,
  unconditionally. The Designer's 61 `new Point` / 202 `new Size` calls are dead geometry.
- **Do not open MainForm in the Visual Studio designer.** Regeneration would wipe the
  hand-written `ThemeRoles.Set(...)` block at `MainForm.Designer.cs:~2240`.
- **~11 orphaned controls** are created, themed, then dropped by `DetachOldContainers()`. Two are
  still load-bearing: `lstExtensions` (10 refs in `MainForm.cs`) and `cmbExtension` (used as an
  off-screen text buffer by `mnuAddCustom`). Deleting them breaks the add-extension flow.
- **RichTextBox teardown contract is non-negotiable** — `_tearingDown`, `OutputReadable`, and
  overlaying `emptyOutput` instead of hiding `rtbOutput`. See `MainForm.Layout.cs:677-732`.
  Breaking it resurrects "Dispose() cannot be called while doing CreateHandle()" on every exit.
- **Dock order is add-fill-child-first, edge-child-last.** Silent breakage otherwise.
- **`ApplyLayoutMetrics()` must be called from `ApplyTheme()`** — sizes derive from `Font.Height`
  and the theme sets the fonts. New controls need an entry there or they won't resize.
- `TreatWarningsAsErrors=true` + latest analyzers. Sloppy code will not compile.

---

## Open / next

- [ ] UX redesign — full spec in `docs/UX-REDESIGN.md`, also published at
      https://claude.ai/code/artifact/9c24a646-74a2-4af9-a3ba-22a2224cf0e0 . Not implemented.
- [ ] **Verified but unfixed defects** found during the survey, all confirmed in the working tree:
      focus ring 1.67:1 on the primary button (a keyboard user tabbing onto Generate sees nothing);
      `Border` at 1.37:1 is the only boundary on every secondary button and input; `TextDisabled`
      used as real text by 5 controls at 3.96:1; Remove illegible in dark mode at 2.39:1; chip
      hover is a dead ternary (`ChipList.cs:218`); chip ✕ target is 15×15 vs the 24×24 minimum;
      outline buttons permanently change colour after first hover; "Presets ▾"/"Sort ▾" are
      keyboard-unreachable. Steps 1–4 of the spec's migration order fix most of these and are worth
      doing even if the IA redesign never happens.
- [x] Committed and pushed 2026-08-14 as `972eff6` (146 files) at Al's instruction — that commit
      bundles ~80 files of pre-existing uncommitted work with this session's, because the two are
      interleaved in the same files and could not be split honestly after the fact. The PROTECT
      row rework is a second commit on top.
- [ ] Codex/OpenAI was rate-limited until 2026-08-20, so the redesign is my work, not OpenAI's.
      Worth a Codex adversarial review once quota returns.
