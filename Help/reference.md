# Reference

## Bundle format

A generated pack is framed. Each entry is:

```
>>>> CodeShuttle bundle v1
>>>> file: <path>
>>>> meta: lines=N; encoding=…; eol=…; eofNewline=…
<exactly N lines>
<<<< end file
```

The reader takes exactly N lines and then verifies the closing sentinel. It never scans inside an
entry, so a line of your source that happens to look like a header cannot split a file in two.

Packs produced by older builds, which used a bare `path:` header, are still read.

## Where things are kept

| What | Where |
|---|---|
| Settings | `%APPDATA%\CodeShuttle\settings.json` |
| Quarantined settings | `%APPDATA%\CodeShuttle\settings.json.bad` |
| Crash logs | `%APPDATA%\CodeShuttle\logs\` |
| Apply backups | `%APPDATA%\CodeShuttle\backups\<timestamp>\` |

## Compression and encryption

The **PROTECT** row sits directly above the output box, and the same four actions are also under
**Tools ▸ Compression and encryption**. GZip+Base64 compresses the output pane; the encrypting
variants additionally seal it with AES-GCM behind a password you confirm twice.

| Button | What it does |
|---|---|
| **Compress** | GZip + Base64. Smaller to paste — but an AI cannot read a compressed pack, so this is for storage, not for chat. |
| **Decompress** | Turns a compressed pack back into readable text. |
| **🔒 Encrypt…** | Compresses *and* seals with AES-GCM behind a password. |
| **🔓 Decrypt…** | Unseals a password-protected pack. |

There is no password recovery. A blob whose password is lost is not decryptable — that is what
authenticated encryption means, not a limitation to be worked around.

All four are available as soon as there is anything in the pane. If an action does not apply, the
app says so in a sentence rather than greying the button out — **Decrypt** on plain text tells you
the pack is not encrypted, and points you at **Decompress** if that is what it actually is.

**Encrypt** refuses on a pack that is already sealed. Encrypting twice produces a blob needing both
passwords in the right order to open, and nothing records either of them; decrypt it first if you
want to change the password.

**Edit** unlocks the pane for typing, and works on an empty pane too — that is how you paste in a
pack somebody sent you and decrypt it without generating anything first.

**Edit** sits in the same row because it also acts on the text in the pane, unlike Export, Copy and
Generate, which act on the pack as a whole.

## Token estimation

Roughly 3.3 characters per token, applied uniformly. The commonly quoted 4.0 holds for prose;
code sits nearer 3.0–3.5 because of punctuation and identifiers. Since the number exists to
answer "will this fit", the estimate errs toward over-counting.
