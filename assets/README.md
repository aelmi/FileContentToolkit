# assets

Branding and marketing material that is **not** compiled into the application.

| Wanted | Notes |
|---|---|
| `screenshot-main.png` | The main window, light theme, at 100% scaling, with a real repo loaded. Goes at the top of the root README. |
| `screenshot-diff.png` | The diff view before an apply. This is the differentiator; it should be the second image anyone sees. |
| `screenshot-dark.png` | Same main window, dark theme. |
| `logo.svg` / `logo-256.png` | Source logo. |
| `banner.png` | 1280x640, for the GitHub social preview card. |

Screenshots are not checked in yet: they require running the GUI, which the
build workstream was not permitted to do. Capture them during the manual QA
pass — the same session that covers the outstanding dual-monitor DPI and
keyboard/Narrator criteria — and link them from the root README.

**The application icon is not here.** `CodeShuttle.ico` lives at the repository
root because `CodeShuttle.csproj` references it as `ApplicationIcon` and
`installer\CodeShuttle.iss` references it as `SetupIconFile`. Moving it would
mean editing both; it is left where the build expects it.
