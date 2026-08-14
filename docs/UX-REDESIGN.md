# CodeShuttle — UX redesign specification

**Status:** proposal. Nothing here is implemented except the blue palette and the `Protect ▾` button.
**Date:** 2026-08-14
**Constraint:** .NET 8 · WinForms · Win10 1809+ · **zero NuGet packages**. Everything below is
stock WinForms plus `OnPaint`. Anything that isn't buildable is marked as such rather than drawn
and left for someone else to discover.

> **Provenance.** Codex/OpenAI was rate-limited when this was written, so this is not an OpenAI
> design. It was produced by two independent design passes — one on information architecture, one
> on the visual system — synthesised here. Every contrast number was re-measured against the
> shipped palette before being written down; every claimed code defect was confirmed in the working
> tree. Worth an adversarial Codex review before implementation.

---

## 0. The one-paragraph version

CodeShuttle's differentiator is the **return trip** — no competitor brings the AI's answer back.
Today that differentiator is the bottom-most strip of the window, above the status bar, because
WinForms docks in reverse z-order and it was added last. The redesign makes the round trip the
**top** of the window: a two-stage journey switch with a deliberately inert legend in the middle
naming the step that happens in someone else's product. Underneath, the two-pane shape survives,
re-ranked so Generate terminates the rail it consumes and the token gauge sits beside the lever
that moves it. Visually, the app moves off Unicode dingbats and 90° flat rectangles onto a glyph
font, a 4/8px radius system, a Semibold-not-Bold type scale, and a set of tokens that fix five
measured WCAG failures in the current build.

---

## Part I — Information architecture

### 1. What's wrong today

Eleven findings. The first six are structural; the rest are placement.

#### 1.1 The differentiator is architecturally last

`BuildPane()` adds children in the order `outputHost → pnlBudget → lblOutputStats →
pnlRecreateInfo → pnlOutputHeader`. WinForms docks from the end of `Controls` backwards, so the
real vertical stack is:

```
PACK header
output box
token budget strip
stats line
round-trip strip     ← the entire competitive moat
status bar
```

A comment in the file records that the round trip was promoted out of a Tools menu because it "is
half the product". It was promoted to the least valuable location on screen. In a comparison
screenshot against repomix or gitingest — which is how this gets sold — CodeShuttle looks like
another file concatenator.

#### 1.2 `Apply AI Changes` is a trap

`btnApplyAiChanges` applies **whatever is currently in the output pane** to a folder chosen from a
`FolderBrowserDialog`. In the overwhelmingly common state, the output pane holds the user's own
freshly generated pack. So the button writes your code back over your code — a no-op at best, and
a confusing failure at worst. It sits beside `btnPasteResponse`, which opens a real inbound dialog,
and the two read as synonyms. The button most likely to be clicked by a confused user does the
least useful thing.

#### 1.3 The IA contradicts itself in shipped strings

`ReportNoFileEntries` tells the user to "Run **Tools ▸ Decompress output** first". The same four
commands now also live behind `Protect ▾` in the pack header. Four commands, two homes, and the
error text points at the home the redesign demoted.

#### 1.4 Generate is on the wrong side of the splitter

Generate consumes the **left rail's** contents but lives in the **right pane's** header, at the end
of a cluster of five siblings that are all disabled at rest. A first-run window shows five dead
controls with the one live one hiding behind them. Disabled-cluster-with-one-live-button is the
signature of a toolbar that was never ranked.

#### 1.5 The rail's priority order is inverted

`SOURCE` is a titled section holding two checkboxes that **does not contain the source** — the
folder chip is up in the command bar. It occupies the most valuable slot in the rail. Meanwhile
`FILES`, the only region holding the user's actual data, is third and gets whatever height survives
two `AutoSize` sections above it. And *Watch folder* — background behaviour with real consequences
— sits one tab stop from *Include subfolders*, a benign scope toggle, at identical visual weight.

#### 1.6 Presets are buried and duplicated

Presets are the fastest path from "I opened this app" to "I have a correct pack" — the answer to
the first-timer's actual question. They are filed as a small action link on a section header
(`hdrFilters.ActionText = "Presets ▾"`), and the language presets **also** appear inside the chip
list's add-menu. Two homes, neither prominent.

#### 1.7 The token gauge is adjacent to the artifact and remote from the lever

`UpdateTokenBudget()` reads `rtbOutput.Text`, so the gauge is meaningless until after Generate. But
the decision it informs — *is this too big, which files do I drop* — is made **before** generating,
against the **file list**, which is in the other pane. Post-generate it says "over budget" and the
remedy is 700px away in a different column.

#### 1.8 The safety story is invisible until it fires

The README leads with "**It will not let you leak a credential**." In the resting UI, `SecretGuard`
has zero pixels — it surfaces only as a modal, on Generate, only on a hit. A user with a clean repo
will use CodeShuttle for a year, never learn the feature exists, and never pay for it.

#### 1.9 Load-bearing information is exiled to the chrome

The clickable **"N files skipped"** item is one of the genuinely good ideas here — a pack that
quietly omits files is worse than no pack. It is a small status label in the bottom-left corner, as
far from the file list it describes as the window permits.

#### 1.10 Advanced plumbing is promoted to the toolbar

`cmbEncoding` is right-docked in the command bar beside Browse and Tree. Encoding is auto-detected
per file; it is a fallback for a failure case. In the top toolbar it reads as a decision the user
must make before proceeding. It buys hesitation and sells nothing.

#### 1.11 Nothing acknowledges that step 2 happens elsewhere

The moment of maximum confusion is immediately after Copy: the pack is on the clipboard and the app
says nothing about what happens next. The window never mentions the middle step except in one
sentence in the bottom strip.

---

### 2. The new organising principle

> **The window's top-level structure is the round trip — and the round trip is a loop with two
> entry points, not a line.**

Two facts constrain every candidate structure:

1. `Ctrl+Shift+V` is documented as *"Bring an AI's reply back in… **No Generate needed first.**"*
   and `PasteResponseForm` takes its own target folder. **Bringing a reply back is an independent
   entry point.** Any structure that gates it behind packing deletes a working capability.
2. The loop is **iterated**, not completed — filter, generate, copy, paste, re-filter, five times a
   session. Any structure implying "done" is wrong.

#### Candidates scored

| | Wizard spine | Two-mode switch | Re-ranked two-pane | **Trip bar** |
|---|---|---|---|---|
| First-run clarity | 5 | 3 | 2 | **5** |
| Expert speed | 1 | 4 | 5 | **5** |
| WinForms risk *(5 = lowest)* | 2 | 4 | 5 | **4** |
| Showcases the round trip | 4 | 5 | 1 | **5** |
| Honours 2 entry points | 1 | 5 | 3 | **5** |
| Honours iteration | 1 | 5 | 5 | **5** |
| **Total / 30** | 14 | 26 | 21 | **29** |

The wizard teaches perfectly and destroys the expert — and it violates both constraints, gating the
reply entry point and implying completion. The plain re-rank serves the expert and teaches nothing;
it cannot fix §1.1, so the screenshot still looks like repomix. Two-mode does both but never
explains *why* there are two modes.

#### Recommended: the trip bar

```
┏━━━━━━━━━━━━━━━━━━━━┓                                    ┌────────────────────┐
┃  ①  PACK CODE      ┃ ──▶  paste into your AI chat  ──▶  │  ②  APPLY REPLY    │
┗━━━━━━━━━━━━━━━━━━━━┛      Claude · ChatGPT · Gemini     └────────────────────┘
      stage switch            legend — NOT a control            stage switch
```

Both segments always enabled, including on a cold start with no folder — which is exactly what the
code already supports and what a first-run user needs to *see*.

**The middle is deliberately inert.** A segment labelled "Send" would imply transmission and
undercut the product's central privacy claim: *CodeShuttle never talks to an AI service.* The legend
states that the AI step is yours, in your chat, and the app does not touch it. It costs one
owner-drawn label and it is the only element that explains why there are two stages.

This is the only candidate where **the top row of the window is the sales pitch, the navigation and
the mental model simultaneously**, while the surfaces underneath stay as fast as the plain re-rank.

#### The differentiator is taught three times

1. **The trip bar** — always visible. *Awareness.*
2. **The next-step hint** — appears in the pack pane the instant Copy succeeds: "Copied. Paste it
   into your AI chat, then come back to **② Apply reply**." Dismissible, persisted. *Teaching, at
   the exact moment of confusion (§1.11).*
3. **Stage ② itself.** *Doing.*

The permanent bottom strip is **deleted**. It was compensating for the absence of 1 and 2.

---

### 3. Region map

```
A  Menu bar        File / Edit / View / Tools / Help
B  TRIP BAR        ① Pack code · legend · ② Apply reply    (both always enabled)
C  CONTEXT BAR     ①: source folder + Browse/Tree/↻    ②: "Apply to" target + Browse
D  LEFT RAIL       ①: INCLUDE + FILES + gauge + Generate
                   ②: CHANGED FILES, collapsed until a plan parses
E  WORK PANE       ①: the pack, or its empty state
                   ②: paste box → diff for the selected file
F  RELEASE STRIP   ① only, post-generate: "before it leaves this machine"
G  STATUS BAR      counts · skipped · progress · ambient indicators
```

### 4. What moves, what dies

**Dies**

- The bottom round-trip strip — replaced by the trip bar plus the next-step hint.
- `btnApplyAiChanges` as a top-level control (§1.2). Its one legitimate case — the user hand-edited
  the pack pane — becomes a link inside stage ②: *"use what's in the pack pane."*
- `Protect ▾` — see §6. (It ships today; the redesign supersedes it.)
- `SOURCE` as a rail section; `cmbEncoding` from the command bar.

**Moves**

| What | From | To | Why |
|---|---|---|---|
| **Generate** | pack-pane header | bottom of the left rail, full-width primary | It consumes the rail, so it should terminate the rail — and point left-to-right into the pane where its result lands (§1.4) |
| **Token gauge** | pane bottom | rail bottom, under the file list, above Generate | The gauge and the lever that moves it become adjacent (§1.7) |
| **Presets** | header action link | first control in INCLUDE: `Preset [ C# project ▾ ]` | One home instead of two; the fastest path to a correct pack (§1.6) |
| **"N files skipped"** | status bar | inline chip beside the FILES count, mirrored in the status bar | Its home should be next to the list it describes (§1.9) |
| **Include subfolders** | rail SOURCE | context bar, beside the folder | It is a property of the folder |
| **Watch folder** | rail checkbox | Tools menu toggle + status-bar `Watching` indicator | Ambient behaviour belongs where ambient state lives (§1.5) |
| **Copy** | one of five | the pane's primary once a pack exists | Exactly one filled button on screen; which one changes with state |

**The gauge also becomes predictive** — estimated from selected-file bytes pre-generate, labelled
*"estimate, before packing"* so the post-generate shift reads as expected rather than as a defect.

---

### 5. Wireframes

#### 5.1 First run — no folder, no pack

```
┌──────────────────────────────────────────────────────────────────────────────────────────────┐
│ File   Edit   View   Tools   Help                                                 [_][□][X]  │ A
├──────────────────────────────────────────────────────────────────────────────────────────────┤
│   ┏━━━━━━━━━━━━━━━━━━━━━━┓                                    ┌──────────────────────┐       │ B
│   ┃  ①  PACK CODE        ┃ ──▶  paste into your AI chat  ──▶  │  ②  APPLY REPLY      │       │
│   ┗━━━━━━━━━━━━━━━━━━━━━━┛      Claude · ChatGPT · Gemini     └──────────────────────┘       │
├──────────────────────────────────────────────────────────────────────────────────────────────┤
│ ▤  no folder chosen — Browse, or drop a folder here     [ ] subfolders  [Browse…][Tree][ ↻ ] │ C
├────────────────────────────────┬─────────────────────────────────────────────────────────────┤
│ INCLUDE                        │            ┌───────────────────────────────────┐            │ D / E
│  Preset  [ pick a preset  ▾ ]  │            │  ①  Pack                          │            │
│  ┌──────────────────────────┐  │            │      your files → one block       │            │
│  │  No extensions yet.      │  │            │              │                    │            │
│  │  Pick a preset above,    │  │            │              ▼                    │            │
│  │  or add one by hand.     │  │            │  ②  Your AI chat                  │            │
│  │        [ + add ]         │  │            │      you paste it in — CodeShuttle│            │
│  └──────────────────────────┘  │            │      never talks to a service     │            │
│  ▸ Advanced                    │            │              │                    │            │
├────────────────────────────────┤            │              ▼                    │            │
│ FILES  0                Sort ▾ │            │  ③  Apply                         │            │
│  [ search files…            ]  │            │      review the diff, back up,    │            │
│  ┌──────────────────────────┐  │            │      write only what you approve  │            │
│  │   Nothing here yet.      │  │            └───────────────────────────────────┘            │
│  │   Choose a folder to     │  │                                                             │
│  │   start.                 │  │                  [    Choose a folder…    ]                 │
│  └──────────────────────────┘  │                                                             │
│  [+Files][+Folder]   ▲ ▼  Rm   │        already have a reply from an AI?  →  ② Apply reply   │
├────────────────────────────────┤                                                             │
│  Fits in  [ Claude ▾ ]         │                                                             │ D
│  ░░░░░░░░░░░░░░░░░░░░░░░░░░░   │                                                             │
│  no files selected             │                                                             │
├────────────────────────────────┤                                                             │
│  [ ▓▓▓  Generate pack  ▓▓▓ ]   │  ← disabled                                                 │
├────────────────────────────────┴─────────────────────────────────────────────────────────────┤
│ 0 files · 0 KB                                                              Ready            │ G
└──────────────────────────────────────────────────────────────────────────────────────────────┘
```

The onboarding card **is** the empty state — it occupies space that has nothing else to do on first
run, so it costs zero friction and there is nothing to dismiss. It teaches the loop and the privacy
fact in one object, and introduces the second entry point at second zero without a dialog. Once a
folder is chosen it collapses to `No pack yet · 128 files are ready · [ Generate pack ]`.

#### 5.2 Loaded, pre-generate

```
┌──────────────────────────────────────────────────────────────────────────────────────────────┐
│   ┏━━━━━━━━━━━━━━━━━━━━━━┓                                    ┌──────────────────────┐       │ B
│   ┃  ①  PACK CODE        ┃ ──▶  paste into your AI chat  ──▶  │  ②  APPLY REPLY      │       │
│   ┗━━━━━━━━━━━━━━━━━━━━━━┛                                    └──────────────────────┘       │
├──────────────────────────────────────────────────────────────────────────────────────────────┤
│ ▤ C:\Projects\MigratePro                    ▾  [x] subfolders  [Browse…][Tree][ ↻ ]          │ C
├────────────────────────────────┬─────────────────────────────────────────────────────────────┤
│ INCLUDE                        │  Pack                                                    ⋯  │ D / E
│  Preset [ C# project      ▾ ]  │            ┌───────────────────────────────────┐            │
│  (.cs)(.csproj)(.json)(.xml)   │            │        No pack yet                │            │
│  (.md)             [ + add ]   │            │                                   │            │
│  ▾ Advanced                    │            │  A pack is your selected files    │            │
│    Ignore [bin, obj, .vs] edit │            │  flattened into one block of      │            │
│    [x] use .gitignore          │            │  text, ready to paste into any    │            │
│    Max file size  [ 512 KB ]   │            │  AI chat.                         │            │
├────────────────────────────────┤            │                                   │            │
│ FILES  128   ⚠ 9 skipped Sort ▾│            │  128 files are ready.             │            │
│  [ search files…            ]  │            │                                   │            │
│  ┌──────────────────────────┐  │            │     [  Generate pack   Ctrl+G ]   │            │
│  │ ▸ src\Program.cs     4 KB│  │            └───────────────────────────────────┘            │
│  │ ▸ src\MainForm.cs   98 KB│  │                                                             │
│  │ ▸ tests\Diff.cs      6 KB│  │                                                             │
│  └──────────────────────────┘  │                                                             │
│  [+Files][+Folder]   ▲ ▼  Rm   │                                                             │
├────────────────────────────────┤                                                             │
│  Fits in [ Claude 200k ▾ ]     │                                                             │ D
│  ████████████░░░░░░░░░░░░░░░   │                                                             │
│  ≈ 96,000 of 200,000 · 48%     │                                                             │
│  estimate, before packing      │                                                             │
├────────────────────────────────┤                                                             │
│  [ ▓▓▓  Generate pack  ▓▓▓ ]   │  ← enabled, primary                                         │
├────────────────────────────────┴─────────────────────────────────────────────────────────────┤
│ 128 files · 3.4 MB · ⚠ 9 skipped                       Scanned in 0.4s          Watching     │ G
└──────────────────────────────────────────────────────────────────────────────────────────────┘
```

#### 5.3 Post-generate

```
├────────────────────────────────┬─────────────────────────────────────────────────────────────┤
│ INCLUDE                        │ Pack · 128 files · plain text ▾     ⋯   [ ▓ Copy ▾ ▓ ]      │ E header
│  Preset [ C# project      ▾ ]  ├─────────────────────────────────────────────────────────────┤
│  (.cs)(.csproj)(.json)(.xml)   │ >>>> CodeShuttle bundle v1                                  │ E body
│  (.md)             [ + add ]   │ >>>> file: src\Program.cs                                   │
│  ▸ Advanced                    │ >>>> meta: lines=3; encoding=utf-8; eol=lf                  │
├────────────────────────────────┤ using System;                                               │
│ FILES  128   ⚠ 9 skipped Sort ▾│ class Program { }                                           │
│  [ search files…            ]  │ <<<< end file                                               │
│  ┌──────────────────────────┐  │ >>>> file: src\MainForm.cs                                  │
│  │ ▸ src\Program.cs     4 KB│  │ …                                                           │
│  │ ▸ src\MainForm.cs   98 KB│  │                                                             │
│  └──────────────────────────┘  │                                                             │
│  [+Files][+Folder]   ▲ ▼  Rm   ├─────────────────────────────────────────────────────────────┤
├────────────────────────────────┤ BEFORE IT LEAVES THIS MACHINE                               │ F
│  Fits in [ Claude 200k ▾ ]     │  ✓ no secrets found      unprotected ▾      Breakdown       │
│  ██████████████░░░░░░░░░░░░    │                                                             │
│  ~103,400 of 200,000 · 52%     ├─────────────────────────────────────────────────────────────┤
├────────────────────────────────┤ ✓ Copied. Paste it into your AI chat, then come back to     │ E
│  [    Regenerate    Ctrl+G  ]  │   ② Apply reply.                              [dismiss]     │
├────────────────────────────────┴─────────────────────────────────────────────────────────────┤
```

The rail's primary demotes to **Regenerate**; the pane's primary becomes **Copy ▾**. Exactly one
filled button in the window at any time — *which* one changes with state.

#### 5.4 Stage ② — pre-parse

Reachable from a cold start, with no folder and no pack.

```
├──────────────────────────────────────────────────────────────────────────────────────────────┤
│ Apply to  ▤ C:\Projects\MigratePro                              ▾        [ Browse… ]         │ C
├──────────────────────────────────────────────────────────────────────────────────────────────┤
│  Paste the AI's reply                                     [ Paste  Ctrl+V ]  [ Clear ]       │ E
│ ┌──────────────────────────────────────────────────────────────────────────────────────────┐ │
│ │      Paste the whole reply here — CodeShuttle finds the file entries in it.               │ │
│ │                                                                                          │ │
│ │      The reply needs the pack's file headers. If the AI returned only a fragment,         │ │
│ │      ask it to return complete files in the format you sent.                              │ │
│ │                                                                                          │ │
│ │                                          or  use what's in the pack pane                 │ │
│ └──────────────────────────────────────────────────────────────────────────────────────────┘ │
│  nothing pasted yet                                              [  Review changes  ]        │
└──────────────────────────────────────────────────────────────────────────────────────────────┘
```

**Protected-bundle detection replaces the current apology.** `LooksLikeEncryptedBase64` and
`LooksLikeCompressedBase64` already exist and are used only to write a better error message. Turn
the detector into an affordance:

```
│ ┌──────────────────────────────────────────────────────────────────────────────────────────┐ │
│ │ 🔒 This looks like an encrypted CodeShuttle bundle.              [  Unlock…  ]            │ │
│ ├──────────────────────────────────────────────────────────────────────────────────────────┤ │
│ │ Q1NIVAEAAABpAAAA4Kp3Yk1lZ2Fic2VjcmV0YmxvYmJhc2U2NGRhdGFoZXJl…                            │ │
```

Zero new detection code. Two of the four security commands leave the visible surface and become
automatic.

#### 5.5 Stage ② — diff review

```
├────────────────────────────────┬─────────────────────────────────────────────────────────────┤
│ CHANGED FILES        7 of 9    │ src\Services\TrustLedger.cs        modified  +18 −4          │ D / E
│  [x] all                       ├─────────────────────────────────────────────────────────────┤
│  ┌──────────────────────────┐  │   42    public void Post(decimal amount)                    │
│  │[x] ~ TrustLedger.cs +18-4│  │   43    {                                                   │
│  │[x] ~ MainForm.cs   +2 -2 │  │ − 44        _balance += amount;                             │
│  │[x] + Overpayment.cs  new │  │ + 44        if (amount <= 0)                                │
│  │[ ] ~ Theme.cs      +9 -9 │  │ + 45            throw new ArgumentOutOfRangeException(…);   │
│  │[x] + LedgerTests.cs  new │  │ + 46        if (_balance + amount > _cap)                   │
│  │ ⚠  ..\..\evil.txt reject │  │ + 47            throw new InvalidOperationException(…);     │
│  └──────────────────────────┘  │   48    }                                                   │
│  ⚠ 1 entry rejected: resolves  │                                                             │
│    outside the target folder   │                                                             │
├────────────────────────────────┤                                                             │
│  utf-8 · lf · trailing newline │  ← per-file fidelity, preserved                             │
├────────────────────────────────┤                                                             │
│  Every file that will be       │                                                             │
│  overwritten is copied to      │                                                             │
│  %APPDATA%\CodeShuttle\backups │                                                             │
│  first.                        │                                                             │
├────────────────────────────────┤                                                             │
│ [ ▓ Back up and apply 7 files ▓]│  ← primary, states the count                               │
└────────────────────────────────┴─────────────────────────────────────────────────────────────┘
```

**The primary button counts** — "apply 7 files", not "Apply". Rejected entries appear *in the list*
rather than only in an error dialog: `PathSafety` refusing a traversal is a feature demonstration,
and the README already sells it. The fidelity line and the backup promise sit permanently above the
button, because "you never have to trust the model" is the reason someone buys this and it should
be legible at the moment of commitment.

---

### 6. Where the security actions belong

#### What's wrong with `Protect ▾` (as shipped today)

The four commands are grouped by **implementation** — "things that transform the pane text" — not
by **intent**:

1. **Two directions in one menu.** Compress/Encrypt are outbound; Decompress/Decrypt are inbound.
   `CmsProtect_Opening` has to sniff the pane's state before it can offer anything. When a menu must
   detect what mode you're in, the menu is doing thinking the IA should have done.
2. **The four are not independent peers.** `CompressAndEncryptToBase64Async` already couples
   compression to encryption — the user never composes "compress then encrypt".
3. **The face does nothing.** The implementation comment concedes it: there is no safe primary. A
   split button whose primary action had to be neutered shouldn't be a split button.

#### Proposal: split by direction

**Outbound → the release strip, as one decision:**

```
┌─ Protect this pack ─────────────────────────────────────────────┐
│  Nothing leaves this machine unless you copy or export it.      │
│  These options protect it once it does.                         │
│                                                                 │
│  [x] Compress it (gzip)                                         │
│      Smaller to paste. Not readable as text any more —          │
│      the AI can't read it either. Use for storage, not chat.    │
│                                                                 │
│  [x] Encrypt it with a password (AES-GCM)                       │
│      Compresses too. If you lose the password, the pack is      │
│      gone — there is no recovery.                               │
│      Password  [ ················ ]  Confirm [ ··············]  │
│                                                                 │
│                                     [ Cancel ]  [   Apply   ]   │
└─────────────────────────────────────────────────────────────────┘
```

Encrypt forces and disables Compress, matching what the code already does. The strip then reads
`gzip + AES-GCM 🔒 ▾` instead of `unprotected ▾`, so the state is legible at rest rather than
inferrable from the pane looking like base64.

Critically, the sheet names **the trade-off the current menu never states**: a compressed pack
cannot be read by an AI. Today the only way to learn that is to compress, paste into Claude, and
get nonsense back. That is not a security feature; it is a trap with a padlock on it.

**Inbound → automatic in stage ②** (§5.4), detected on paste.

**Manual escape hatch → `Tools ▸ Bundle`**, the single non-duplicated home, for decoding a blob a
colleague sent without applying it. This ends the duplication in §1.3.

#### The framing that makes it a capability

Not "compression and encryption utilities". This:

> ### Before it leaves this machine

| Concern | Feature | Where it shows |
|---|---|---|
| Don't leak *content* | secret scanner | `✓ no secrets found` — **at rest**, in the release strip |
| Don't leak *in transit* | AES-GCM encryption | `unprotected ▾` → the Protect sheet |
| Fit + obscure | gzip | same sheet, same decision |
| Don't leak *at all* | "never talks to an AI service" | the trip bar's inert legend |

The scanner and the encryption were built as separate features and are the same promise. One strip
under one heading turns two utilities into a capability a buyer can name — and it fixes §1.8 for
free, because the strip states `✓ no secrets found` in the happy path, which is the 95% case the
user currently never sees.

---

### 7. Progressive disclosure

**The governing rule: hide the control, never hide the consequence.** Anything collapsed must leave
a visible trace of its non-default state. A collapsed "Advanced" silently excluding 400 files is how
you ship a tool that lies.

| Item | Today | Proposed | Why |
|---|---|---|---|
| **Presets** | header action link + duplicated in the chip menu | **Promote.** First control in INCLUDE; its dropdown holds the project catalogue, saved slots and "Save current…" | Fastest path to a correct pack; one home, not two |
| **Ignore rules** | always-visible inline row | **Demote** into `▸ Advanced`, collapsed — but the header reads `▸ Advanced · 3 ignore rules` and FILES shows `⚠ 9 skipped` | Most users never touch these; consequence stays visible |
| **Encoding** | combo in the top command bar | **Remove** → Options, plus contextual: `2 files couldn't be decoded — [ set encoding ]` | Auto-detected already; in the toolbar it reads as a prerequisite (§1.10) |
| **Watch folder** | rail checkbox | **Demote** → `Tools ▸ Watch this folder` + a `Watching` status indicator | Ambient behaviour belongs where ambient state lives |
| **Token budget** | pane bottom, post-hoc | **Keep, move to rail bottom, make predictive** | It informs a *selection* decision (§1.7) |
| **Extension filters** | chips + `+ add` | **Keep**, but design the zero-state | Correctly placed; only its empty state is undesigned |

**Also demoted:** Find / Replace / Edit → the pane's `⋯` overflow (`Ctrl+F` / `Ctrl+H` keep working
globally). Export → into `Copy ▾` as "Export to file…" — same intent, different destination.
**Promoted:** Copy-with-a-prompt to the *second* item in `Copy ▾`; it attaches the question and is
arguably the better default, and it is currently three levels deep.

---

### 8. Keyboard model

> **Tab follows the trip; F6 crosses regions; every stage has a stated default button.**

Tab order today is z-order, which after `BuildLayout()`'s `Controls.Clear()`-and-rebuild is
effectively arbitrary.

| Chord | Action |
|---|---|
| `Ctrl+1` / `Ctrl+2` | Stage ① / ② — never disabled |
| `F6` / `Shift+F6` | Cycle regions (Windows-standard; currently absent) |
| `Ctrl+L` | Focus the folder / target field |
| `Ctrl+Enter` | Commit the stage — ① Generate, ② Back up and apply |
| `Space` | Toggle the selected change (stage ②) |
| `Ctrl+A` / `Ctrl+Shift+A` | Tick / untick all changes |

`Ctrl+Shift+V` gains one behaviour: it **switches to stage ②** and focuses the paste box rather
than opening a dialog. Same chord, same intent, now consistent with the structure.

**No chord for Protect.** It is a per-pack decision, and a chord that pops a password dialog is a
chord you press by accident once.

**Rules:** containers are one tab stop (the chip list is one stop; arrows move between chips — else
adding a language preset makes the rail unusable from the keyboard). The trip bar is one stop. Tab
skips an all-disabled region as a unit. The default button is set explicitly per stage, not
inherited. Esc is layered: cancel a running op → clear the search box → collapse a disclosure →
confirm before discarding pasted text → nothing. **Esc must never close the main window.**

#### The expert claim — and the product demo

```
Ctrl+O          browse for folder
Ctrl+P          presets → arrows → Enter
Ctrl+G          generate
Ctrl+C          copy the pack
Alt+Tab         → your AI chat, Ctrl+V, ask, copy the reply
Alt+Tab         → back
Ctrl+Shift+V    stage ② opens, paste box focused
Ctrl+V          paste
Enter           review → the change list appears
↑↓ / Space      read each diff, untick what you don't want
Ctrl+Enter      back up and apply
```

Thirteen keystrokes and one alt-tab from nothing to a reviewed, backed-up, applied change. **That
sequence is the demo** — it belongs on the landing page and as Help topic one, and every keyboard
decision above exists to make it true.

---

## Part II — Visual system

### 9. Measured defects in the current build

Six of these are not taste. All were re-verified against the shipped palette.

| # | Defect | Measured | Required |
|---|---|---|---|
| 9.1 | `Border` is the *only* boundary on every secondary button and every input | **1.37:1** light / **1.41:1** dark | 3:1 (WCAG 1.4.11) |
| 9.2 | The focus ring vanishes on the primary button | **1.67:1** light / **2.62:1** dark | 3:1 |
| 9.3 | `TextDisabled` — the ≥3:1 *chrome* token — is used as real text by 5 controls | **3.96:1** / **3.84:1** | 4.5:1 |
| 9.4 | Remove is illegible in dark mode (`Danger`, a *fill*, used as a foreground) | **2.39:1** | 4.5:1 |
| 9.5 | The filled primary button's own outer edge, dark mode | **2.95:1** | 3:1 |
| 9.6 | Progress bars render **Windows green** in light mode — the one piece of live data, in the one colour the product doesn't use | — | — |

**9.2 is the sharpest.** A keyboard user tabbing onto **Generate** — the one action the window
exists for — sees nothing at all. The XML doc on `FocusRing` claims it fixes exactly this.

**Live code bugs, all confirmed in the working tree:**

| Bug | Location |
|---|---|
| Chip hover is a dead ternary: `hot ? t.Selection : t.Selection` | `Controls/ChipList.cs:218` |
| Chip ✕ hit target is 15×15 px | `Controls/ChipList.cs:124` — WCAG 2.2 SC 2.5.8 requires 24×24 |
| Outline buttons **permanently change colour after first hover** — `ApplyOutlineButtons` runs after `AttachHover` and overwrites `BackColor`, so `MouseLeave` restores the wrong value | `MainForm.Layout.cs` |
| Pressed and hover are the same colour — nothing acknowledges a click | `ThemeApplier.StyleButton` |
| "Presets ▾" / "Sort ▾" are keyboard-unreachable despite the doc comment claiming otherwise | `Controls/SectionHeader.cs:42` |
| Combo drop-down lists stay light in dark mode (`DarkMode_Explorer` darkens only the edit portion) | `Theming/NativeTheming.cs:66` |

> **Fixed already:** 9.6 and the combo class, in this session.

**Two more, on taste rather than measurement.** Iconography is Unicode dingbats — `⌕` U+2315, `▤`
standing in for a folder, `↻`, `⚙`, `▾`, `▲▼`, a hand-drawn `✕` — resolving from three or four
fallback fonts at three optical weights, and U+2699 will render as a **colour emoji** wherever it
resolves through Segoe UI Emoji. That is the loudest "not a commercial product" signal in the UI.
And the type scale has seven roles but three distinguishable sizes: the gap from `Small` to `Body`
is **0.5pt**, invisible at 96 DPI, costing a cached GDI handle for nothing.

---

### 10. Type

**Family.** Segoe UI Variable is Windows 11 only and its honest payoff at 9–11pt is small (GDI has
no fractional positioning). **The high-value change is Semibold**, which ships as its own GDI family
on Win10+. `FontStyle` has no Semibold member — it is reachable *only* by family name.

Bold (700) at 9pt is shouty and is why the app reads as a form rather than a document; Windows 11's
own control emphasis is Semibold (600).

```
Text ≤11pt : "Segoe UI Variable Small" → "Segoe UI Variable Text" → "Segoe UI" → Base
Semibold   : "<family> Semibold" → "Segoe UI Semibold" → Base + FontStyle.Bold
Mono       : unchanged (Cascadia Mono → Consolas → Courier New)
```

Verify by comparing `Font.Name` to the requested name — **GDI+ silently substitutes Microsoft Sans
Serif rather than throwing.** The codebase already does this correctly for mono; copy that.

| Role | pt @ base 9 | weight | change |
|---|---|---|---|
| `Small` / `SmallBold` | **8.0** | Reg / Semibold | was 9 — creates a real step below body |
| **`Overline`** *(new)* | **8.0** | Semibold, +0.08em, UPPERCASE | section headers, "PACK" |
| `Body` / `BodyBold` | **9.0** | Reg / Semibold | was 9.5 — 0.5pt is not a scale step |
| `Medium` / `MediumBold` | 10.0 | Reg / Semibold | unchanged |
| `Heading` | 11.0 | Semibold | was Bold |
| `Title` | **14.0** | Semibold | was 12 Bold — barely a title next to 11pt |
| `TitleLarge` | **18.0** | Semibold | was 14 |
| `Mono` / `MonoSmall` | **9.0** / **8.0** | Reg | were 10 / 9 — mono runs visually large |
| **`Icon`** / **`IconSmall`** *(new)* | 16 / 12 **device px** | — | glyph font, `GraphicsUnit.Pixel` |

**Honest limits.** You cannot set line-height on a `Label` or `TextRenderer` call — GDI takes it
from the font metrics. Where 1.45/1.50 matters, draw line-by-line; do this in exactly four places
(`EmptyStateView` body, `Toast` body, banner body, custom tooltip) and accept natural leading
everywhere else. **GDI+ has no letter-spacing** — the per-glyph loop `SectionHeader` already has is
the only route; keep it, apply it only to `Overline`, and *cache the measured width* instead of
re-measuring every glyph on every paint. **Tabular figures are unreachable** — no OpenType feature
access from `System.Drawing`. Segoe UI's default figures are already tabular-width, so don't add a
`Numeric` role.

---

### 11. Space, geometry, elevation

`Unit = ThemeFonts.Base.Height` = **15 device px** at 9pt/96dpi. The codebase already sizes from
`Font.Height`; this just formalises it and replaces the ad-hoc `+14 / +12 / ×4 / ×5 / ×6 / ×9 / ×16`
constants.

| token | px | | token | px |
|---|---|---|---|---|
| `s1` | 4 | | `H.Small` (chip, toggle) | 24 |
| `s2` | 6 | | `H.Control` (button, input) | 32 |
| `s3` | 8 | | `H.Large` (CTA) | 40 |
| `s4` | 12 | | `H.Row` (list row) | 30 |
| `s5` | 15 | | `H.MenuItem` | 28 |
| `s6` | 20 | | `H.Meter` | 6 |
| `s7` | 28 | | | |
| `s8` | 40 | | | |

**Minimum hit target 24×24 device px.** Current violation: the chip ✕ at 15×15.

**Button widths must stop being font-height multiples.** `btnProtect.Width = unit * 6` is guesswork
that clips on a longer string. Measure: `TextRenderer.MeasureText(label) + glyph + s4 × 2`, clamped
to a `MinWidth` of `Unit × 5`.

**Radii** (always through `LogicalToDeviceUnits` — a 4px radius must become 8px at 200%, or the app
looks *sharper* on a 4K panel than on a laptop):

| class | radius |
|---|---|
| chip, pill, count badge, segmented thumb, meter | fully rounded (`h/2`) |
| button, input, combo, split button, toggle | **4** |
| card, section, banner, empty state, dialog | **8** |
| menu, dropdown, flyout, tooltip, toast | **8** (DWM-rounded where possible) |
| focus ring | component radius **+3** |
| form window | leave to DWM — never round the form yourself |

**Two border tokens, not one.** `Border` becomes *decorative only* — separators, rules, the divider
inside a split button. **`BorderStrong`** (new) is the boundary of anything clickable or typeable.

**Elevation — what GDI+ can and cannot do.** No blur filter exists in `System.Drawing`; a child
control cannot paint outside its own bounds; `BackColor` alpha is ignored. In order of preference:

1. **OS shadow for anything with its own HWND** — menus, dropdowns, tooltips. Add Win11 rounding
   with `DwmSetWindowAttribute(hwnd, 33, 2, 4)`, the exact pattern `NativeTheming.ApplyTitleBar`
   already uses; it no-ops harmlessly on Win10. **BUILDABLE.**
2. **Stacked-stroke soft shadow** — concentric rounded-rect strokes with quadratic alpha falloff,
   `alpha = base × (1 − (i−1)/spread)²`. At spread ≤ 8 this is indistinguishable from a real blur
   and costs ≤ 8 `DrawPath` calls. Above ~10px it bands. **BUILDABLE to spread 8.**
3. **Value-step elevation, no shadow.** In dark mode a lighter surface reads as raised more
   convincingly than any shadow can. **This is the recommended default.**

> ⚠ **Never round a Panel's children by setting `Control.Region`** from a `GraphicsPath` — regions
> are hard-edged and aliased, and the corners will look chewed. Keep `s5` padding so nothing
> reaches a corner instead. **NOT-WORTH-IT.**

#### Nine new tokens — all additive, no existing value changes

| token | Light | Dark | guarantee |
|---|---|---|---|
| `BorderStrong` | `#7788A6` | `#6A7A94` | ≥3:1 on every surface, both palettes — **fixes 9.1** |
| `SurfaceOverlay` | `#FFFFFF` | `#222A36` | dark: TextPrimary 11.95:1 |
| `SelectionHover` | `#CFE1FB` | `#17304C` | AccentOnSurface 5.48:1 / 6.63:1 |
| `AccentPressed` | `#083A8E` | `#113F87` | white **10.47:1** / **10.07:1** |
| `DangerOnSurface` | `#B42318` | `#F0827A` | 6.57:1 / 6.80:1 — **fixes 9.4** |
| `SuccessOnSurface` | `#1B7A43` | `#5FC98A` | 5.37:1 / 8.49:1 |
| `Track` | `#DDE4F0` | `#2A3341` | meter container |
| `MeterFill` | `#0B57D0` | `#6FBEF6` | 5.00:1 / 6.29:1 on Track |
| `Scrim` | `argb(102,15,26,43)` | `argb(140,0,0,0)` | in-window overlays only |

Plus one constant: **the focus ring's inner stroke is white** (`AccentText`) — 6.39:1 on Accent,
6.57:1 on Danger, 8.07:1 on Neutral. That is what makes a ring survive on a saturated fill and
**fixes 9.2**.

For **9.5**, rather than disturb a palette whose white-text headroom is already tight: give the
filled accent button a **1px `BorderStrong` outline in dark mode only** (4.01:1). Zero palette
churn, no test disturbance, and it is what Fluent does with `ColorStrokeAccentDefault`.

**Contrast assertions to add** to `ThemeContrastTests`: `BorderStrong` vs all four surfaces ≥3;
`MeterFill` vs `Track` ≥3; `AccentText` vs `AccentPressed` ≥4.5; `AccentText` vs each fill ≥3 (the
inner ring); `AccentOnSurface`/`TextPrimary` vs `SelectionHover` ≥4.5; text vs `SurfaceOverlay`
≥4.5; `DangerOnSurface`/`SuccessOnSurface` vs all surfaces ≥4.5.

---

### 12. Iconography

**Glyph font with a verified fallback chain, plus four hand-drawn paths.**

```
"Segoe Fluent Icons" (Win11) → "Segoe MDL2 Assets" (Win10 1607+, the safe floor) → drawn set
```

Verification is **mandatory** — `new Font("Segoe Fluent Icons", …)` on Win10 silently returns
Microsoft Sans Serif and every icon becomes an empty rectangle. **Never fall back to Segoe UI
Emoji**; you get colour emoji. Embedded SVG is out (`System.Drawing` has no rasteriser and packages
are forbidden); PNG is out (never crisp at 125%/150%).

**Rules:** `TextRenderer.DrawText` with `NoPadding` (without it GDI adds 3px side bearing and the
glyph sits off-centre); size in **pixels** not points; **never mix a glyph run and a text run in one
`DrawText`** — the whole string takes the icon font and the label becomes tofu; every icon-only
control needs an `AccessibleName`, because a PUA glyph is announced as silence.

| purpose | codepoint | | purpose | codepoint |
|---|---|---|---|---|
| folder | `\uE8B7` | | copy | `\uE8C8` |
| browse | `\uE838` | | export | `\uEDE1` |
| refresh | `\uE72C` | | generate / send | `\uE724` |
| settings | `\uE713` | | apply / check | `\uE73E` |
| search | `\uE721` | | paste | `\uE77F` |
| lock / unlock | `\uE72E` / `\uE785` | | warning / error / info | `\uE7BA` / `\uEA39` / `\uE946` |
| chevron down / up | `\uE70D` / `\uE70E` | | add / remove / close | `\uE710` / `\uE738` / `\uE711` |
| up / down | `\uE74A` / `\uE74B` | | overflow / filter | `\uE712` / `\uE71C` |

**Draw these four, don't pick a codepoint:**

- **diff** — MDL2 has no unambiguous diff glyph, and **a missing PUA glyph renders as nothing at
  all, not as a visible error**, so a guessed codepoint will pass review and ship broken. Draw two
  6px rounded bars, one `SuccessOnSurface`, one `DangerOnSurface`. ~8 lines, and it doubles as the
  diff-view brand mark.
- **sun** and **moon** — `\uE706` and `\uEC46` exist but are optically unmatched to each other.
  Drawing both guarantees they match, which two glyphs from two different eras will not.
- **chip ✕ at 12px** — keep the drawn cross, move it into a 24×24 target.

> ⚠ **Pick one icon family per process at startup and never mix.** Fluent and MDL2 share codepoints
> but differ in optical weight.

---

### 13. Components

**Foundation first: `ThemedButton : Button`.** Today a button is styled by four fighting
mechanisms — `ThemeApplier.StyleButton`, `Theme.AttachHover`, `FocusRing`, and `ApplyOutlineButtons`
overwriting after the fact. That is where the permanent-colour-change hover bug lives and why there
is no pressed state. One owner-drawn control with a `Variant` property replaces all four and is
**~200 lines net negative**.

| Component | Key change | Verdict |
|---|---|---|
| **Primary button** | `AccentPressed` fill + 1px shape shrink on press; **never move the label**; disabled is `SurfaceAlt`/`TextDisabled`, never a desaturated accent (that reads as "loading") | BUILDABLE |
| **Secondary / outline** | 1px **`BorderStrong`** — the 1.37:1 fix; hover border `AccentOnSurface` | BUILDABLE |
| **Ghost** | label `TextPrimary`, **not `TextDisabled`** — the search toggles currently paint an *enabled* control in the disabled colour. Danger ghost uses `DangerOnSurface` | BUILDABLE |
| **Split button** | the two halves highlight **independently** — that's what makes it legible as two things; caret stays pressed while the menu is open | BUILDABLE |
| **Segmented control** *(new)* | track + inset thumb; radio-group keyboard semantics | BUILDABLE (static thumb) |
| **Card** *(new)* | r=8, `Surface`/`SurfaceOverlay`, 1px `Border`, `s5` padding | BUILDABLE |
| **Section header** | title `TextDisabled` → **`TextSecondary`** (3.96 → 6.30); the trailing action becomes a real button, **fixing a keyboard-unreachable control**; cache the tracking width | BUILDABLE |
| **Chip** | fix the dead hover ternary; 24×24 ✕ target; `+N more` overflow so the rail stops growing unbounded | BUILDABLE |
| **Search / text input** | one `FieldFrame` replacing three near-identical hand-rolled borders; **focus must not change geometry** (the current 1f→1.6f pen swap makes fields breathe) | BUILDABLE |
| **Combo box** | owner-draw the face and items; **the drop-down list window is native and cannot be rounded or shadowed**. Use the menu-button pattern for ≤8 items | BUILDABLE / list NOT-WORTH-IT |
| **List row** | `PathEllipsis` not `EndEllipsis` (a path with the *end* removed is useless); **3px accent leading bar on selection** — cheapest single change that makes a list look designed. ⚠ `ListBox` does not double-buffer; eat `WM_ERASEBKGND` or it flickers | BUILDABLE-WITH-EFFORT |
| **Meter bar** *(replaces `ProgressBar`)* | the only way to get a blue meter. ⚠ when the fill is narrower than its own diameter, clip a plain rect — a degenerate rounded rect inverts into a bowtie | BUILDABLE |
| **Status bar** | **add a 1px top rule** — `StatusStrip` draws no top edge, so it currently floats. One renderer override, disproportionate payoff | BUILDABLE |
| **Toast** | move off the solid coloured fill (a 2014 Bootstrap alert) to neutral surface + coloured glyph; **pause the timer on hover**; allow 3 stacked | BUILDABLE-WITH-EFFORT |
| **Menu** | `SurfaceOverlay` + `BorderStrong`; **rounded r=4 inset selection fill** instead of full-bleed square — this one override is what makes a WinForms menu look like a Win11 menu; **draw no image margin** (the gutter is the most dated thing about a WinForms menu) | BUILDABLE |
| **Focus ring** | **two-tone**: 2px `BorderFocus` outer + 1px white inner. Reserve a 3px inset so the ring sits outside the shape. Gate on `ShowFocusCues` — one line, removes "why is there a blue box after I clicked" | BUILDABLE |
| **Sun/moon toggle** *(new)* | two-cell segment, not an iOS switch — both destinations visible at once. Keep the menu item as a keyboard duplicate | BUILDABLE |
| **Scrollbars** | **NOT-WORTH-IT.** Non-client area; `WM_NCPAINT` interception flickers and breaks specifically on `RichTextBox`. `SetWindowTheme` is the correct and complete answer, and the code already does it. *Stated here so nobody spends a week on it.* | NOT-WORTH-IT |

**On the amber.** `WarningText #8A4B0A` is a brown paragraph — technically inside the blue-only rule
as a semantic status colour, but conspicuous. Preferred fix: keep the warm wash, set the **body text
to `TextPrimary`** (15.95:1), and let the colour live **only in the 16×16 glyph**. The brown shrinks
from a paragraph to an icon and the tested pair stays available.

---

### 14. Motion

**One shared animation clock**, not a timer per control — twenty hovers with twenty `WM_TIMER`s is a
storm; one 16ms timer ticking a tween list is not. ~80 lines.

**Reduced motion is mandatory, not optional** (WCAG 2.3.3): one guard on
`SystemInformation.UIEffectsEnabled` at the top of `Animator.Start`; when false every tween jumps to
its end state. Never `Thread.Sleep` + `Application.DoEvents()`.

| Interaction | Duration | Verdict |
|---|---|---|
| Button hover fill | 120ms linear | BUILDABLE |
| Button press | **0ms in**, 90ms out | BUILDABLE — presses must be instant *in* |
| **Meter count-up** | 300ms easeOutCubic | **BUILDABLE — highest payoff in the app.** A budget bar that counts up to 48,200 is the difference between a widget and a product |
| Busy shimmer over the output pane | 1200ms loop | BUILDABLE — the one place a long operation currently shows nothing but a status-bar bar |
| Segmented / theme thumb slide | 180 / 160ms | BUILDABLE-WITH-EFFORT |
| Toast enter / exit | 220 / 160ms | BUILDABLE-WITH-EFFORT — `Control` has no `Opacity`; blend toward the parent's known flat background |
| Focus ring appear | — | **Snap.** A ring that fades feels laggy under fast tabbing |
| List row hover | — | **Snap.** A fade per row under a moving cursor is jankier than a snap |
| Rail section expand | 200ms | BUILDABLE-WITH-EFFORT — a `PerformLayout` per frame; otherwise snap |
| **Theme cross-fade** | — | **NOT-WORTH-IT.** Native children can't be captured; it will tear. **A WinForms theme switch will flash** — minimise with `SuspendLayout`, don't fight it |
| Splitter-drag smoothing | — | NOT-WORTH-IT — dominated by `RichTextBox` reflow |

---

## Part III — Migration

### 15. The good news

The codebase is **far more redesign-friendly than its file sizes suggest**:

- **Zero hardcoded fonts** in `MainForm.Designer.cs`. **Zero hardcoded colours** in layout code.
- **`MainForm.resx` is boilerplate only** — no images, no strings. Restructure freely.
- **No NuGet packages**, so nothing external constrains the UI.
- **Sizing is already entirely font-derived.** Nothing anchors to a pixel constant.
- **`MainForm.Layout.cs` is the only file that needs rewriting for structure.** The 99KB of
  `MainForm.cs` is handler logic and never touches layout. `BuildLayout()` already does
  `Controls.Clear()` and rebuilds programmatically, so **there is no Designer surgery anywhere in
  this restructure.**

### 16. The landmines

1. 🔴 **The 102KB Designer file describes a window that no longer runs.** 61 `new Point` and 202
   `new Size` calls of dead geometry. **Do not open MainForm in the Visual Studio designer** — a
   regeneration would wipe the hand-maintained `ThemeRoles.Set(...)` block.
2. 🔴 **~11 orphaned controls**, two still load-bearing: `lstExtensions` (10 refs in `MainForm.cs`)
   and `cmbExtension` (an off-screen text buffer for the add-extension flow). Deleting them breaks
   that flow. **Untangle before structural work, not during.**
3. 🟡 **The RichTextBox teardown contract is non-negotiable** — `_tearingDown`, `OutputReadable`,
   and overlaying `emptyOutput` rather than hiding `rtbOutput`. Breaking it resurrects
   `"Dispose() cannot be called while doing CreateHandle()"` on every exit after generating.
4. 🟡 **Dock order is add-fill-child-first, edge-child-last.** Get it wrong and controls stack
   silently wrong with no error.
5. 🟡 **`ApplyLayoutMetrics()` must be called from `ApplyTheme()`** — sizes derive from
   `Font.Height` and the theme sets the fonts.
6. 🟡 **`TreatWarningsAsErrors=true`** with latest analyzers. Sloppy code will not compile.
7. 🟡 **`ThemeContrastTests` fails the build** on any palette regression.

### 17. Recommended order

Ranked by visible change per unit of effort. **Steps 1–4 are worth doing even if the IA redesign
never happens.**

| # | Step | Why first |
|---|---|---|
| 1 | `BorderStrong` + two-tone focus ring + `DangerOnSurface` + `TextDisabled`→`TextSecondary` | **Four token-level changes closing five measured WCAG failures.** The app immediately stops looking like it has invisible controls |
| 2 | `MeterBar` + the count-up tween | The green bar was the most off-brand pixel in the window *(the green is already fixed; the meter is the upgrade)* |
| 3 | Glyph font + the four drawn icons | Removes the dingbats — the loudest "not commercial" signal |
| 4 | Type scale: `Body` 9.5→9, `Small` 9→8, Bold→Semibold, `Title` 12→14 | Cheap, and it stops the app reading as a form |
| 5 | `ThemedButton` | Collapses four mechanisms into one; fixes the hover bug, the missing pressed state, the invisible outline |
| 6 | Menu renderer overrides + `SurfaceOverlay` | Four overrides; a WinForms menu starts looking like a Win11 menu |
| 7 | Owner-drawn list rows + the 3px accent selection bar | Cheapest change that makes a list look designed |
| 8 | Radii, spacing scale, `SectionHeader`, `ChipList`, toast, sun/moon toggle | Polish |
| 9 | **The IA restructure** — trip bar, rail re-rank, release strip, stage ② | The big one. Phase it: v1 embeds `PasteResponseForm`'s content in stage ② and keeps `DiffViewerForm` modal; v2 extracts the diff renderer into the pane |

The only medium-risk item in the whole plan is the inline diff, and it phases cleanly.
