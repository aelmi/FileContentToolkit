using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeShuttle.Theming;
using CodeShuttle.UI;

namespace CodeShuttle.Dialogs
{
    public partial class FolderTreePickerForm : ThemedForm
    {
        private readonly string _rootPath;
        private readonly HashSet<string>? _extensionFilter;
        private bool _suppressAfterCheck;

        public List<string> SelectedFiles { get; private set; } = new();

        public FolderTreePickerForm(string rootPath, IEnumerable<string>? extensionsFilter = null)
        {
            _rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            if (extensionsFilter != null)
            {
                _extensionFilter = new HashSet<string>(extensionsFilter, StringComparer.OrdinalIgnoreCase);
                if (_extensionFilter.Count == 0) _extensionFilter = null;
            }

            InitializeComponent();


            chkExtFilter.Checked = _extensionFilter != null;
            chkExtFilter.Enabled = _extensionFilter != null;

            tree.AccessibleName = "Folders and files";
            tree.AccessibleDescription = "Tick the files and folders to add. Expand a folder to see its contents.";

            // Expanding in the constructor enumerated the first level synchronously before the
            // window had been shown, so on a cold network share the user was left looking at a
            // frozen main window and no dialog at all.
            Shown += (s, e) => ReloadTree();
        }

        private void ReloadTree()
        {
            tree.BeginUpdate();
            try
            {
                tree.Nodes.Clear();
                if (!Directory.Exists(_rootPath))
                {
                    tree.Nodes.Add(new TreeNode("(folder not found)"));
                    return;
                }
                var root = MakeFolderNode(_rootPath);
                tree.Nodes.Add(root);
            }
            finally { tree.EndUpdate(); }

            // Expanded after the dialog is on screen and painted, under a wait cursor, so a slow
            // first level reads as "loading" rather than as a hang.
            UseWaitCursor = true;
            try
            {
                Update();
                tree.Nodes[0].Expand();
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private static TreeNode MakeFolderNode(string folderPath)
        {
            var node = new TreeNode(
                Path.GetFileName(folderPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)) ?? folderPath)
            {
                Tag = new FolderTag(folderPath)
            };
            if (string.IsNullOrEmpty(node.Text)) node.Text = folderPath;
            node.Nodes.Add(new TreeNode("…") { Tag = Sentinel });
            return node;
        }

        private static TreeNode MakeFileNode(string filePath)
        {
            return new TreeNode(Path.GetFileName(filePath)) { Tag = new FileTag(filePath) };
        }

        private void ChkExtFilter_CheckedChanged(object? sender, EventArgs e) => ReloadTree();

        private void Tree_BeforeExpand(object? sender, TreeViewCancelEventArgs e)
        {
            if (e.Node == null) return;
            if (e.Node.Tag is FolderTag tag && !tag.Loaded)
                LoadChildren(e.Node, tag);
        }

        // Reparse points are skipped: .NET does not detect reparse cycles, so a junction pointing
        // at one of its own ancestors recurses without bound until PathTooLongException.
        private static readonly EnumerationOptions TopLevelOptions = new()
        {
            IgnoreInaccessible = true,
            RecurseSubdirectories = false,
            AttributesToSkip = FileAttributes.ReparsePoint | FileAttributes.Hidden | FileAttributes.System
        };

        private static readonly EnumerationOptions RecursiveOptions = new()
        {
            IgnoreInaccessible = true,
            RecurseSubdirectories = true,
            AttributesToSkip = FileAttributes.ReparsePoint | FileAttributes.Hidden | FileAttributes.System,
            MaxRecursionDepth = 64
        };

        private void LoadChildren(TreeNode node, FolderTag tag)
        {
            node.Nodes.Clear();
            try
            {
                var dirs = Directory.EnumerateDirectories(tag.Path, "*", TopLevelOptions)
                                    .OrderBy(d => d, StringComparer.OrdinalIgnoreCase);
                foreach (var d in dirs)
                {
                    var child = MakeFolderNode(d);
                    if (node.Checked) child.Checked = true;
                    node.Nodes.Add(child);
                }

                IEnumerable<string> files = Directory.EnumerateFiles(tag.Path, "*", TopLevelOptions);
                if (chkExtFilter.Checked && _extensionFilter != null)
                    files = files.Where(f => _extensionFilter.Contains(Path.GetExtension(f)));

                foreach (var f in files.OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
                {
                    var child = MakeFileNode(f);
                    if (node.Checked) child.Checked = true;
                    node.Nodes.Add(child);
                }
            }
            catch (UnauthorizedAccessException)
            {
                node.Nodes.Add(new TreeNode("(access denied)") { ForeColor = ThemeManager.Tokens.TextSecondary, Tag = Sentinel });
            }
            catch (Exception ex)
            {
                node.Nodes.Add(new TreeNode($"(error: {ex.Message})") { ForeColor = ThemeManager.Tokens.TextSecondary, Tag = Sentinel });
            }
            tag.Loaded = true;
        }

        private void Tree_AfterCheck(object? sender, TreeViewEventArgs e)
        {
            if (_suppressAfterCheck || e.Node == null) return;
            _suppressAfterCheck = true;
            try
            {
                if (e.Node.Tag is FolderTag tag && !tag.Loaded)
                    LoadChildren(e.Node, tag);
                PropagateCheck(e.Node, e.Node.Checked);
            }
            finally { _suppressAfterCheck = false; }
        }

        private static void PropagateCheck(TreeNode parent, bool check)
        {
            foreach (TreeNode child in parent.Nodes)
            {
                if (ReferenceEquals(child.Tag, Sentinel)) continue;
                child.Checked = check;
                if (child.Nodes.Count > 0) PropagateCheck(child, check);
            }
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            var collected = new List<string>();
            CollectCheckedFiles(tree.Nodes, collected);
            SelectedFiles = collected.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        private void CollectCheckedFiles(TreeNodeCollection nodes, List<string> sink)
        {
            foreach (TreeNode n in nodes)
            {
                switch (n.Tag)
                {
                    case FileTag ft when n.Checked:
                        sink.Add(ft.Path);
                        break;
                    case FolderTag folder when n.Checked && folder.Loaded:
                        CollectCheckedFiles(n.Nodes, sink);
                        break;
                    case FolderTag folder when n.Checked && !folder.Loaded:
                        AddAllFilesFromDisk(folder.Path, sink);
                        break;
                    default:
                        if (n.Nodes.Count > 0) CollectCheckedFiles(n.Nodes, sink);
                        break;
                }
            }
        }

        private void AddAllFilesFromDisk(string folder, List<string> sink)
        {
            try
            {
                foreach (var f in Directory.EnumerateFiles(folder, "*", RecursiveOptions))
                {
                    if (chkExtFilter.Checked && _extensionFilter != null &&
                        !_extensionFilter.Contains(Path.GetExtension(f))) continue;
                    sink.Add(f);
                }
            }
            catch { /* ignore */ }
        }

        private static readonly object Sentinel = new();

        private sealed class FolderTag
        {
            public string Path { get; }
            public bool Loaded { get; set; }
            public FolderTag(string p) { Path = p; }
        }

        private sealed class FileTag
        {
            public string Path { get; }
            public FileTag(string p) { Path = p; }
        }
    }
}
