# Applying Answers Back

This is the half no one-way tool has.

When the AI replies with edited files, you do not have to copy them out one at a time.

## Paste the response

**Apply AI Changes…** > **Paste AI response…**, or Ctrl+Shift+V.

Paste the reply into the box, choose the folder it applies to, and press **Review changes**.
You do not need to have generated anything first — a pack from yesterday, or from a colleague,
works the same way.

## What happens next

Every entry in the response is resolved against the folder you chose and classified:

- **New** — no such file exists yet.
- **Modified** — the content differs from what is on disk.
- **Unchanged** — byte-identical once encoding and line endings are accounted for. Ticked off by
  default, because there is nothing to do.
- **Rejected** — the path failed containment validation. See below.

The diff viewer shows each file line by line. Tick the ones you accept. Nothing is written until
you press Write, and Write is never the default action — Enter will not overwrite your source.

## Rejected paths

A file header that resolves outside the folder you chose is rejected and shown with the reason,
never silently dropped or silently written. This matters: a header of the form
`..\..\AppData\Roaming\...\Startup\evil.bat` would otherwise place a file in your startup folder
while the dialog displayed a harmless-looking relative path.

Absolute paths, `..` segments, alternate data streams and reserved device names are all refused.
If a whole bundle is refused, nothing in it is applied.

## Backups

Before anything is overwritten, the current contents of every affected file are copied to
`%APPDATA%\CodeShuttle\backups\<timestamp>\` with a manifest. The apply summary tells you where.

Each file is written through a temporary file in the destination directory and then moved into
place, so an apply that fails part-way cannot leave a half-written file.
