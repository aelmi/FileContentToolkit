# Settings

**Options** (Ctrl+,) covers filtering, encoding, trust and the token budget.

## Filters

- **Maximum file size** — anything larger is skipped and reported.
- **Skip binary files** — content sniffing, not extension guessing.
- **Auto-detect encoding** — BOM first, then UTF-8. With this off, the encoding dropdown is used
  and a file that does not match it raises an error rather than silently substituting `?` for
  every character it cannot represent.
- **.gitignore** and **.dockerignore** — separate toggles. See *Selecting Files*.

## Trust

- **Redact detected secrets** — replaces credential values with `[REDACTED: kind]` before the
  pack leaves the application.
- **Warn before copying secrets** — shows the review dialog rather than redacting silently.

Both default on.

## Token budget

Pick the model whose context window the gauge should measure against, or choose Custom and enter
a figure.

## Import and export

**Tools** > **Export settings…** writes presets, prompt templates, filters and appearance to one
file. **Import settings…** reads it back. Window position and the first-run flag are deliberately
not included — they describe one machine's monitors.

Settings live in `%APPDATA%\CodeShuttle\settings.json`. If that file is ever unreadable it is kept
as `settings.json.bad` rather than overwritten, and you are told, because it may be the only copy
of your presets.
