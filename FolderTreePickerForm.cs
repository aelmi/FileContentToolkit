using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FileContentToolkit.UI;

namespace FileContentToolkit.Dialogs
{
    public class FolderTreePickerForm : Form
    {
        private readonly string _rootPath;
        private readonly HashSet<string>? _extensionFilter;

        private readonly TreeView _tree;
        private readonly Button _ok;
        private readonly Button _cancel;
        private readonly CheckBox _chkExtFilter;

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

            Text = "Select files and folders";
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = true;
            ClientSize = new Size(760, 720);
            MinimumSize = new Size(540, 480);
            Theme.ApplyForm(this);

            Controls.Add(Theme.BuildHeader("Select files and folders",
                "Tip: checking a folder selects every file inside it."));

            // Body panel
            var body = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.White,
                Padding = new Padding(16, 12, 16, 12)
            };

            // Host the filter check inside a fixed-height strip so its descenders aren't clipped.
            var filterStrip = new Panel
            {
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = Theme.White,
                Padding = new Padding(0, 6, 0, 6)
            };
            _chkExtFilter = new CheckBox
            {
                Text = "Filter by configured extensions",
                Left = 2,
                Top = 4,
                AutoSize = true,
                Checked = _extensionFilter != null,
                Enabled = _extensionFilter != null,
                ForeColor = Theme.BodyText,
                Font = Theme.BodyFont
            };
            _chkExtFilter.CheckedChanged += (s, e) => ReloadTree();
            filterStrip.Controls.Add(_chkExtFilter);

            _tree = new TreeView
            {
                Dock = DockStyle.Fill,
                CheckBoxes = true,
                ShowLines = true,
                ShowPlusMinus = true,
                ShowRootLines = true,
                HideSelection = false,
                BorderStyle = BorderStyle.FixedSingle,
                Font = Theme.BodyFont,
                BackColor = Color.White,
                ForeColor = Theme.BodyText
            };
            _tree.BeforeExpand += OnBeforeExpand;
            _tree.AfterCheck += OnAfterCheck;

            body.Controls.Add(_tree);
            body.Controls.Add(filterStrip);

            // Bottom action bar
            var bottom = Theme.BuildBottomBar();
            _ok = Theme.PrimaryButton("OK");
            _ok.Size = new Size(90, 36);
            _ok.DialogResult = DialogResult.OK;
            _cancel = Theme.SecondaryButton("Cancel");
            _cancel.Size = new Size(90, 36);
            _cancel.DialogResult = DialogResult.Cancel;
            bottom.Resize += (s, e) =>
            {
                _ok.Left = bottom.ClientSize.Width - 20 - _ok.Width;
                _ok.Top = (bottom.ClientSize.Height - _ok.Height) / 2;
                _cancel.Left = _ok.Left - _cancel.Width - 8;
                _cancel.Top = _ok.Top;
            };
            bottom.Controls.Add(_ok);
            bottom.Controls.Add(_cancel);

            Controls.Add(bottom);
            Controls.Add(body);
            body.BringToFront();

            AcceptButton = _ok;
            CancelButton = _cancel;
            _ok.Click += OnOkClick;

            ReloadTree();
        }

        private void ReloadTree()
        {
            _tree.BeginUpdate();
            try
            {
                _tree.Nodes.Clear();
                if (!Directory.Exists(_rootPath))
                {
                    _tree.Nodes.Add(new TreeNode("(folder not found)"));
                    return;
                }
                var root = MakeFolderNode(_rootPath);
                _tree.Nodes.Add(root);
                root.Expand();
            }
            finally { _tree.EndUpdate(); }
        }

        private TreeNode MakeFolderNode(string folderPath)
        {
            var node = new TreeNode(Path.GetFileName(folderPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)) ?? folderPath)
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

        private void OnBeforeExpand(object? sender, TreeViewCancelEventArgs e)
        {
            if (e.Node == null) return;
            if (e.Node.Tag is FolderTag tag && !tag.Loaded)
            {
                LoadChildren(e.Node, tag);
            }
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
                if (_chkExtFilter.Checked && _extensionFilter != null)
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

        private void OnAfterCheck(object? sender, TreeViewEventArgs e)
        {
            if (_suppressAfterCheck || e.Node == null) return;
            _suppressAfterCheck = true;
            try
            {
                if (e.Node.Tag is FolderTag tag && !tag.Loaded)
                {
                    LoadChildren(e.Node, tag);
                }
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

        private void OnOkClick(object? sender, EventArgs e)
        {
            var collected = new List<string>();
            CollectCheckedFiles(_tree.Nodes, collected);
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
                    if (_chkExtFilter.Checked && _extensionFilter != null &&
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
