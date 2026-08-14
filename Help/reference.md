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

**Protect ▾** in the pack header, and also under **Tools ▸ Compression and encryption**. GZip+Base64
compresses the output pane; the encrypting variants additionally seal it with AES-GCM behind a
password you confirm twice.

There is no password recovery. A blob whose password is lost is not decryptable — that is what
authenticated encryption means, not a limitation to be worked around.

The four actions live behind one button rather than four, because none of them is the primary thing
you do with a pack — Generate is. Clicking anywhere on **Protect** opens the menu; nothing runs
until you pick it by name, since three of the four rewrite the pane and one asks for a password you
cannot recover. Items you cannot use are greyed: the menu will not offer to decrypt plain text, or
to encrypt something already sealed.

## Token estimation

Roughly 3.3 characters per token, applied uniformly. The commonly quoted 4.0 holds for prose;
code sits nearer 3.0–3.5 because of punctuation and identifiers. Since the number exists to
answer "will this fit", the estimate errs toward over-counting.
