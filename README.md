# 📂 File Content Toolkit

A **Windows Forms application** that helps you **browse folders**, **filter files by extension**, **view file contents**, and **recreate files from structured output**.

This toolkit is ideal for developers, content reviewers, or automation workflows that involve auditing or restoring file contents from a structured list.

---

## 🔧 Features

- ✅ Browse and select folders to scan files
- ✅ Filter files by extension (e.g., `.cs`, `.txt`, `.config`)
- ✅ Include or exclude subfolders
- ✅ Manually add or remove specific files
- ✅ View and export file contents in a structured format
- ✅ Copy or edit the generated output
- ✅ Recreate files and folder structure directly from the output

---

## 🖥️ Interface Overview

The UI is divided into the following sections:

- **Left Panel**: Manage file extensions and selected files
- **Top Bar**: Folder selection and scanning controls
- **Right Panel**: Displays file contents in a readable, exportable format
- **Bottom Bar**: Generate file content output
- **Recreate Section**: Rebuild files from the exported or edited output

---

## 🚀 Getting Started

1. **Clone the repository**:
   ```bash
   git clone https://github.com/aelmi/FileContentToolkit.git
   ```

2. **Open the solution** in Visual Studio.

3. **Build and run** the application.

---

## 📤 Export Format

When you click **Generate**, the output text format is:
```
C:\MyProject\Program.cs:
using System;

class Program {
    static void Main() { }
}
```

Output can be edited or reused to **recreate files** using the **Recreate Files** feature.

---

## 🔁 File Recreation

Click the **Recreate Files** button and choose a base folder. The app will:
- Parse the output area
- Create files and folders based on their original relative paths
- Restore contents accurately

---

## 📅 Planned Enhancements

- [ ] Drag-and-drop support
- [ ] Extension profile presets (e.g., “C# files”, “Web config set”)
- [ ] Output as plain text or JSON zip package
- [ ] Dark mode toggle

---

## 📄 License

No license selected yet. You can add a license later under your GitHub repo's settings.

---

Let me know if you'd like me to generate a badge-rich version or include screenshots/diagrams for GitHub display!
