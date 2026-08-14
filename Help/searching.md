# Searching

## Searching file contents

The search box above the file list finds files whose *contents* match a term. Matching files are
highlighted in the list and the count appears beside the box.

- **Aa** — match case
- **Word** — whole word only
- **.\*** — treat the term as a regular expression

The arrow lists your last 15 search terms.

Regular expressions run with a match timeout, so a pattern that would otherwise backtrack forever
fails with a message instead of freezing the window.

## Searching the output

Ctrl+F opens Find and Replace against the output pane, from anywhere in the window. Ctrl+H opens
the same dialog. F3 and Shift+F3 step through matches.

Replace edits the output pane only. It does not touch any file on disk — applying changes to
files is what *Apply AI Changes* is for.
