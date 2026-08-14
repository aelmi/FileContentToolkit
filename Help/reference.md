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

Only the buttons that apply to what the pane currently holds are enabled, read from the blob's own
header: plain text can be compressed or encrypted, a compressed pack can only be decompressed, and
a sealed one can only be decrypted. That is why **Encrypt** greys out once a pack is already
sealed — encrypting twice would produce a blob needing two passwords in the right order, and
nothing records either of them.

**Edit** sits in the same row because it also acts on the text in the pane, unlike Export, Copy and
Generate, which act on the pack as a whole.

## Token estimation

Roughly 3.3 characters per token, applied uniformly. The commonly quoted 4.0 holds for prose;
code sits nearer 3.0–3.5 because of punctuation and identifiers. Since the number exists to
answer "will this fit", the estimate errs toward over-counting.
