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

1. **Protect ▾ restored to the first page** — a `SplitButton` in `pnlOutputHeader`, between Edit
   and Export, opening a four-item menu: Encrypt with password / Decrypt with password /
   Compress (no password) / Decompress. Wired to the *existing* handlers; the Tools submenu still
   works and shares them.
   - The whole face opens the menu (`BtnProtect_Click`), not just the caret. There is no safe
     primary action: three of the four rewrite the pane, one asks for an unrecoverable password.
   - Item enabling is on `cmsProtect.Opening`, **not** on the button's `Click` — a caret click
     opens the drop-down from `SplitButton.OnMouseDown` and never raises `Click`, so gating on
     `Click` would leave half the ways in ungated.
   - Files: `MainForm.Designer.cs`, `MainForm.Layout.cs`, `MainForm.cs` (`BtnProtect_Click`,
     `CmsProtect_Opening`).
2. **Palette moved to blue** (Al's call, mid-session): bright cobalt `#0B57D0` accent fill, azure
   `#007FFF` focus ring / `#0A4FBF` on-surface text accent, light sky blue `#E1EDFD` selection
   wash. Dark mode swaps sky and cobalt. `Theming/ThemePalettes.cs`.
3. **Two pre-existing build blockers fixed** — CA1875 (`Regex.Matches(..).Count` → `Regex.Count`)
   in `FindReplaceForm.cs:223` and `MainForm.cs:~1427`. `TreatWarningsAsErrors=true`, so the
   project did not compile at all before this.
4. **Docs corrected** — `Help/reference.md`, `README.md`, `CHANGELOG.md` all claimed the actions
   were deliberately *not* on the main surface.

### Verification
- `dotnet build` — 0 warnings, 0 errors.
- `dotnet test` — **299/299 pass**, including `ThemeContrastTests` on both palettes.
- Real app launched and screenshotted: Protect ▾ renders in the pack header and is correctly
  disabled with no pack. Blue palette confirmed live.

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

### Also done
5. **Progress bars are blue** — `NativeTheming.cs` stripped the visual style in dark mode only, so
   light mode rendered the stock Windows green (comctl32 ignores `ForeColor` while a style is
   active). Now stripped in both. Same edit fixed combo drop-downs staying light in dark mode
   (`DarkMode_CFD`, not `DarkMode_Explorer`).
6. **8 new tests** in `tests/CodeShuttle.Tests/ProtectButtonTests.cs` pin Protect to the main
   surface and pin the enable-gating matrix (plain / encrypted / compressed / empty). **307/307.**

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
- [ ] Nothing committed yet this session; the 86 pre-existing uncommitted files are still
      uncommitted. **Ask Al before committing or pushing** (GitHub remote `aelmi/FileContentToolkit`).
- [ ] Codex/OpenAI was rate-limited until 2026-08-20, so the redesign is my work, not OpenAI's.
      Worth a Codex adversarial review once quota returns.
