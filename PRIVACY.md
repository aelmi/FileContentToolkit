# CodeShuttle Privacy Statement

Last updated: 2026-07-19 · Applies to CodeShuttle 1.0.0

## The short version

**CodeShuttle does not send your code anywhere.** It is a desktop application.
Your files are read from your disk, assembled in memory, and shown to you. What
happens next is your decision — you copy the result and paste it wherever you
choose.

There is no account, no sign-in, no telemetry, no analytics, no crash
reporting service, and no server operated by us that your content is ever sent
to.

## What CodeShuttle stores, and where

Everything CodeShuttle persists lives under `%APPDATA%\CodeShuttle\` on your
own machine. Nothing is uploaded.

| Location | Contents |
|---|---|
| `%APPDATA%\CodeShuttle\settings.json` | Your options, window position, recent folders, recent searches, presets and prompt templates. Recent folders and searches are literal paths and search terms you typed. |
| `%APPDATA%\CodeShuttle\backups\` | Copies of files taken immediately before CodeShuttle overwrites them, so a bad apply can be undone. These are copies of **your source files** and are never deleted automatically. |
| `%APPDATA%\CodeShuttle\logs\` | Crash reports, written only when the application actually crashes. |

You can delete any of these at any time. Uninstalling CodeShuttle
**deliberately leaves this folder in place** so that a reinstall keeps your
presets and your backups; if you want it gone, delete it yourself.

## Crash logs

If CodeShuttle crashes it writes a plain-text report to
`%APPDATA%\CodeShuttle\logs\`. It contains the exception and stack trace, the
application version, the OS version and the .NET version.

Crash logs are **never transmitted**. They exist so that you can attach one to a
bug report if you choose to. Because a user may do exactly that, file paths
below the folder being scanned are redacted before the report is written. Read
the file before you send it — it is yours, and it is plain text.

## The only network request

CodeShuttle makes exactly one kind of outbound request: an update check against
the public GitHub Releases API.

- Endpoint: `https://api.github.com/repos/aelmi/CodeShuttle/releases/latest`
- Sent: a User-Agent header identifying CodeShuttle and its version. Nothing
  else. No file names, no paths, no content, no identifier.
- Received: the latest published release tag.
- Failure is silent — no network means no update notice, and nothing is retried
  or queued.

GitHub will observe your IP address, as it would for any web request. GitHub's
handling of that is covered by their own privacy statement.

If you open a link from the About dialog or the update notice, that opens in
your normal browser and is subject to your browser's settings.

## Secret detection

CodeShuttle scans the content you are about to pack for things that look like
credentials — API keys, private key blocks, connection strings, high-entropy
`.env` values — and warns you before you send them to an AI service.

This scan runs **entirely on your machine**. Nothing detected is transmitted,
logged, or written to disk. The redaction feature replaces matches in the
generated output only; your original files are never modified by it.

## Encryption

The compress-and-encrypt feature derives a key from the password you type using
PBKDF2 and encrypts with AES-GCM. The password is held in memory only for as
long as the operation takes. **It is never stored, never written to disk and
never logged.** There is consequently no recovery mechanism: if you lose the
password, the data is gone.

## Children

CodeShuttle is a developer tool and is not directed at children.

## Changes

Material changes to this statement will be noted in `CHANGELOG.md` and the
"Last updated" date above will change.

## Contact

Questions about this statement, or a privacy concern, can be raised at
<https://github.com/aelmi/CodeShuttle/issues>.

> **Owner action before release:** this statement names "MyCompany" nowhere by
> design, because the entity is not yet registered. Once ASIC registration
> completes, add the registered entity name and a contact address, and confirm
> the GitHub URLs resolve (see the repository-rename note in `CHANGELOG.md`).
