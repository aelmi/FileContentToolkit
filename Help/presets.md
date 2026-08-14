# Presets

A preset stores a folder, its extension list, its ignore rules and the include-subfolders
setting, under a name you choose.

- **Save preset** captures the current state.
- **Presets** (Ctrl+P) lists what you have saved.
- **Manage presets…** renames and deletes.

Loading a preset always rescans, even when the folder has not changed — otherwise the file list
would still be showing the previous preset's results against the new extension set.

## Language presets

**Add** > **Language presets** is a different thing: sixteen ready-made extension bundles for
common stacks. They set extensions only, and do not touch the folder or your rules.

Note that the shell bundle includes `.env`. That is deliberate — you often do want your
configuration in the pack — and it is exactly why the secret scan runs before anything can be
copied.
