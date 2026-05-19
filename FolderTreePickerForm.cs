using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FileContentToolkit.UI;

namespace FileContentToolkit.Dialogs
{
    public partial class FolderTreePickerForm : Form
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

            if (Theme.AppIcon != null) Icon = Theme.AppIcon;
            Theme.AttachHover(btnOk, btnOk.BackColor);
            Theme.AttachHover(btnCancel, btnCancel.BackColor);

            chkExtFilter.Checked = _extensionFilter != null;
            chkExtFilter.Enabled = _extensionFilter != null;

            ReloadTree();
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
                root.Expand();
            }
            finally { tree.EndUpdate(); }
        }

        private TreeNode MakeFolderNode(string folderPath)
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

        private TreeNode MakeFileNode(string filePath)
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

        private void LoadChildren(TreeNode node, FolderTag tag)
        {
            node.Nodes.Clear();
            try
            {
                var dirs = Directory.EnumerateDirectories(tag.Path)
                                    .OrderBy(d => d, StringComparer.OrdinalIgnoreCase);
                foreach (var d in dirs)
                {
                    var child = MakeFolderNode(d);
                    if (node.Checked) child.Checked = true;
                    node.Nodes.Add(child);
                }

                IEnumerable<string> files = Directory.EnumerateFiles(tag.Path);
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
                node.Nodes.Add(new TreeNode("(access denied)") { ForeColor = Theme.SubtleText, Tag = Sentinel });
            }
            catch (Exception ex)
            {
                node.Nodes.Add(new TreeNode($"(error: {ex.Message})") { ForeColor = Theme.SubtleText, Tag = Sentinel });
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
                var opts = new EnumerationOptions
                {
                    IgnoreInaccessible = true,
                    RecurseSubdirectories = true
                };
                foreach (var f in Directory.EnumerateFiles(folder, "*", opts))
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
