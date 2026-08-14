# Troubleshooting

**The scan found nothing.**
Check the status bar for a skipped-files count and click it. The usual causes are no extension
listed, or an ignore rule matching more than intended — the rule editor shows a per-rule count of
what each one removes.

**Every file shows as Modified when I know nothing changed.**
That should no longer happen: comparison is done on normalised content, so a difference of line
ending or trailing newline alone reports Unchanged. If you still see it, the encoding declared in
the pack and the encoding on disk genuinely differ.

**A file in the response was rejected.**
Its path resolved outside the folder you chose, or used an absolute path, a `..` segment, an
alternate data stream, or a reserved device name. The reason is shown beside the entry. This is a
refusal, not a failure.

**The apply stopped part-way.**
Nothing is left half-written. Files already written stay written; the rest are untouched. The
previous contents of everything written are in `%APPDATA%\CodeShuttle\backups\<timestamp>\`.

**Text is clipped after I raised the Windows text size.**
The layout baseline is pinned to 9pt so that it is the same on every machine. Very large text
settings can therefore overflow a control. Resize the window; the panes are resizable and the
splitter can be dragged.

**The application crashed.**
Logs are in `%APPDATA%\CodeShuttle\logs\`. Paths below your scan root are redacted before they
are written. **Help** > **About** > **Copy diagnostics** gathers the environment details worth
attaching to a report; it never includes a scanned path or any file content.
