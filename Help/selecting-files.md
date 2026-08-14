# Selecting Files

## Extensions

Nothing is collected until at least one extension is listed. Type one into the box and press
Enter, or use **Add** > **Language presets** for a whole stack at once.

## Ignore rules

**Edit rules…** opens the exclusion rule editor: one rule per row, with a live count of how many
of the current candidates each rule removes, and a box to test a single path against the whole
set.

Rules are globs, matched against the path relative to the scan root:

- `*.tmp` — every file with that extension, at any depth
- `bin/` — a directory and everything under it
- `docs/notes.md` — one specific file, anchored to the root
- `**/generated/*` — a directory at any depth

A rule containing a slash anywhere other than at the end is anchored to the scan root, the same
way Git treats `.gitignore` patterns. Matching is case-sensitive, again as Git does it.

## .gitignore and .dockerignore

Enable them in **Options**. They are separate settings on purpose: an idiomatic `.dockerignore`
starts by excluding everything and then re-including a few paths, so merging it into the
`.gitignore` rules used to reduce an ordinary repository to zero files with no explanation.

## Skipped files

The status bar reports anything left out — binary content, files over the size limit, permission
failures, and files excluded by a rule. Click it to see the list. A pack that is quietly missing
files is worse than one that tells you.
