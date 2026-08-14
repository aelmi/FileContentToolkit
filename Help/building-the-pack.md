# Building the Pack

**GENERATE** reads every selected file and assembles them into one document in the right pane.

Each file is framed with its path, a line count and its encoding and line-ending style. The line
count is what lets CodeShuttle read the pack back later without a line of your source being
mistaken for a file header.

## Copy formats

The arrow beside **Copy** offers:

- **Plain text** — the pack exactly as shown.
- **Markdown** — fenced code blocks, one per file.
- **XML** — a `<documents>` wrapper, which Claude in particular follows closely.
- **JSON array** — for feeding another tool.
- **As prompt…** — wraps the pack in one of your prompt templates and lets you type the question
  to send with it.

## Prompt templates

Two are supplied, one tuned for Claude and one for ChatGPT. Both take your question and place it
after the files. Add your own under **Tools** > **Prompt templates…**, using `{files}` and
`{question}` wherever you want them.

## Token budget

The gauge under the output pane measures the pack against the context window of the model you
picked. Amber from 80%, red past 100%. **Breakdown…** lists the files by size and suggests which
to remove to fit.

The count is estimated from character count, not a model tokenizer, so treat it as approximate.

## Secrets

Before the pack can be copied or exported, it is scanned for credentials — AWS keys, private key
blocks, API-key assignments, connection-string passwords, JWTs, and high-entropy `.env` values.
Anything found is shown for review, with the value masked, and redacted by default. You can keep
individual matches if they are test fixtures.
