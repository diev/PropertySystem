#region License
//------------------------------------------------------------------------------
// Copyright (c) Dmitrii Evdokimov
// Source https://github.com/diev/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CheckAuthors.Net48.Properties;

using Microsoft.WindowsAPICodePack.Shell;

using File = System.IO.File;

namespace CheckAuthors.Net48
{
    public partial class MainForm : Form
    {
        private const string _emptyStatus = "Перетащите нужные папки и файлы в окно программы";
        private const string _waitStatus = "Подождите...";
        private const string _doneStatus = "Выберите действие";
        private const string _exitStatus = "Программа завершается";
        private readonly bool _readonly = true;

        private readonly string[] _officeMasks = new string[] { "*.doc", "*.xls" }; // 3 letters include more as "docx", etc.

        private readonly HashSet<string> _onboardCollection = new HashSet<string>();
        private RecentManager _mru;

        private readonly ListViewColumnSorter _columnSorter = new ListViewColumnSorter();

        private bool _cancel = false;

        public MainForm()
        {
            InitializeComponent();
            var i = new AppInfo();
            Text = i.Title; // Application.ProductName;

            // Create an instance of a ListView column sorter and assign it
            // to the ListView control.
            ItemsList.ListViewItemSorter = _columnSorter;
        }

        private void ItemsList_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.All
                : DragDropEffects.None;
        }

        private void ItemsList_DragDrop(object sender, DragEventArgs e)
        {
            var drops = (string[])e.Data.GetData(DataFormats.FileDrop, true);
            DisplayProperties(drops);
        }

        private void DisplayProperties(string[] paths)
        {
            var timer = Stopwatch.StartNew();
            StatusLabel.Text = _waitStatus;
            UseWaitCursor = true;
            ProgressBar.Style = ProgressBarStyle.Marquee;
            ProgressBar.Visible = true;
            _cancel = false;
            int total = ItemsList.Items.Count;
            int count = 0;

            foreach (var path in paths)
            {
                if (_onboardCollection.Contains(path))
                {
                    continue;
                }

                _onboardCollection.Add(path);
                _mru.Add(path);

                if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                {
                    const int batchSize = 100;

                    var files = OfficeMenuItem.Checked
                        ? _officeMasks.SelectMany(x => Directory.EnumerateFiles(path, x, SearchOption.AllDirectories))
                        : Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);

                    int batch = 0;

                    foreach (var file in files)
                    {
                        batch++;
                        TryGetProperties(file);

                        if (batch == batchSize)
                        {
                            Application.DoEvents();
                            count += batch;

                            if (_cancel)
                            {
                                UseWaitCursor = false;
                                timer.Stop();

                                if (MessageBox.Show($"Проверено файлов: {count}\nВремя выполнения: {timer.Elapsed}\n\nХотите прервать?",
                                    Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                                {
                                    goto canceled; // break
                                }

                                timer.Start();
                                UseWaitCursor = true;
                                _cancel = false;
                            }

                            batch = 0;
                        }
                    }
                }
                else
                {
                    count++;
                    TryGetProperties(path);
                }
            }

        canceled:

            foreach (ColumnHeader column in ItemsList.Columns)
            {
                column.Width = -2;
            }

            ProgressBar.Visible = false;
            UseWaitCursor = false;
            StatusLabel.Text = _doneStatus;
            SetButtons();

            int added = ItemsList.Items.Count - total;
            timer.Stop();
            MessageBox.Show($"Проверено: {count}\nДобавлено: {added}\n\nВремя выполнения: {timer.Elapsed}", 
                Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TryGetProperties(string path)
        {
            using (var shell = ShellFile.FromFilePath(path))
            {
                var prop = shell.Properties.System;

                try
                {
                    if (prop.Author.Value == null &&
                        prop.Category.Value == null &&
                        prop.Comment.Value == null &&
                        prop.Company.Value == null &&
                        prop.ContentStatus.Value == null &&
                        prop.Document.LastAuthor.Value == null &&
                        prop.Document.Manager.Value == null &&
                        prop.Keywords.Value == null &&
                        prop.Subject.Value == null &&
                        prop.Title.Value == null)
                    {
                        return;
                    }
                }
                catch
                {
                    AddItem(path, "Непонятная ошибка");
                    return;
                }

                var sb = new StringBuilder();

                if (prop.Author.Value != null)
                    sb.Append($" [Author: {string.Join("; ", prop.Author.Value)}]");

                if (prop.Category.Value != null)
                    sb.Append($" [Category: {string.Join("; ", prop.Category.Value)}]");

                if (prop.Comment.Value != null)
                    sb.Append($" [Comment: {prop.Comment.Value}]");

                if (prop.Company.Value != null)
                    sb.Append($" [Company: {prop.Company.Value}]");

                if (prop.ContentStatus.Value != null)
                    sb.Append($" [ContentStatus: {prop.ContentStatus.Value}]");

                if (prop.Document.LastAuthor.Value != null)
                    sb.Append($" [LastAuthor: {prop.Document.LastAuthor.Value}]");

                if (prop.Document.Manager.Value != null)
                    sb.Append($" [Manager: {prop.Document.Manager.Value}]");

                if (prop.Keywords.Value != null)
                    sb.Append($" [Keywords: {string.Join("; ", prop.Keywords.Value)}]");

                if (prop.Subject.Value != null)
                    sb.Append($" [Subject: {prop.Subject.Value}]");

                if (prop.Title.Value != null)
                    sb.Append($" [Title: {prop.Title.Value}]");

                AddItem(path, sb.ToString().Trim());
            }
        }

        private void AddItem(string path, string properties)
        {
            var fi = new FileInfo(path);
            string groupName = fi.DirectoryName;
            var group = ItemsList.Groups[groupName];

            if (group is null)
            {
                group = new ListViewGroup()
                {
                    Header = groupName,
                    HeaderAlignment = HorizontalAlignment.Center,
                    Name = groupName
                };
                ItemsList.Groups.Add(group);
            }

            int index = ImageList.Images.IndexOfKey(fi.Extension);

            if (index < 0)
            {
                ImageList.Images.Add(fi.Extension, Icon.ExtractAssociatedIcon(path));
                index = ImageList.Images.Count - 1;
            }

            var item = new ListViewItem(fi.Name)
            {
                ImageIndex = index,
                Tag = fi,
                ToolTipText = path,
                Group = group,
                Checked = true
            };

            item.SubItems.Add(properties);

            ItemsList.Items.Add(item);
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            ResetMenuItem_Click(sender, e);
        }

        private void CleanButton_Click(object sender, EventArgs e)
        {
            int count = ItemsList.CheckedItems.Count;

            if (MessageBox.Show($"Свойства у {count} файлов будут очищены безвозвратно!",
                Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                CleanMenuItem_Click(sender, e);
            }
        }

        private void CleanProperties()
        {
            const int batchSize = 20;

            var timer = Stopwatch.StartNew();
            int count = ItemsList.CheckedItems.Count;

            StatusLabel.Text = _waitStatus;
            UseWaitCursor = true;
            int batch = 0, clean = 0, errors = 0;

            ProgressBar.Style = ProgressBarStyle.Continuous;
            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = count;
            ProgressBar.Value = 0;
            ProgressBar.Visible = true;

            _cancel = false;

            foreach (ListViewItem item in ItemsList.CheckedItems)
            {
                batch++;
                ProgressBar.Value++;

                if (batch == batchSize)
                {
                    Application.DoEvents();

                    if (_cancel)
                    {
                        UseWaitCursor = false;
                        timer.Stop();

                        if (MessageBox.Show($"Проверено файлов: {count}\nВремя выполнения: {timer.Elapsed}\n\nХотите прервать?",
                            Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            goto canceled; // break
                        }

                        timer.Start();
                        UseWaitCursor = true;
                        _cancel = false;
                    }

                    batch = 0;
                }

                var fi = (FileInfo)item.Tag;
                TryCleanProperties(ref clean, ref errors, item, fi);
            }

        canceled:

            ProgressBar.Visible = false;
            UseWaitCursor = false;
            SetButtons();

            timer.Stop();
            MessageBox.Show($"Очищено: {clean}\nОшибок: {errors}\n\nВремя выполнения: {timer.Elapsed}", 
                Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TryCleanProperties(ref int clean, ref int errors, ListViewItem item, FileInfo fi)
        {
            var sb = new StringBuilder();

            bool modified = false;
            bool excepted = false;
            var lastWritten = fi.LastWriteTime;
            var readOnly = fi.IsReadOnly;

            if (readOnly)
            {
                if (_readonly)
                {
                    fi.IsReadOnly = false;
                }
                else
                {
                    sb.Append(" ReadOnly!");
                }
            }

            using (var shell = ShellFile.FromFilePath(fi.FullName))
            {
                var prop = shell.Properties.System;

                try
                {
                    if (prop.Author.Value != null)
                    {
                        prop.Author.ClearValue();

                        if (prop.Author.Value == null)
                        {
                            modified |= true;
                        }
                        else
                        {
                            sb.Append($" [Author: {string.Join("; ", prop.Author.Value)}]");
                        }
                    }

                    if (prop.Category.Value != null)
                    {
                        prop.Category.ClearValue();

                        if (prop.Category.Value == null)
                        {
                            modified |= true;
                        }
                        else
                        {
                            sb.Append($" [Category: {string.Join("; ", prop.Category.Value)}]");
                        }
                    }

                    if (prop.Comment.Value != null)
                    {
                        prop.Comment.ClearValue();

                        if (prop.Comment.Value == null)
                        {
                            modified |= true;
                        }
                        else
                        {
                            sb.Append($" [Comment: {prop.Comment.Value}]");
                        }
                    }

                    if (prop.Company.Value != null)
                    {
                        prop.Company.ClearValue();

                        if (prop.Company.Value == null)
                        {
                            modified |= true;
                        }
                        else
                        {
                            sb.Append($" [Company: {prop.Company.Value}]");
                        }
                    }

                    if (prop.ContentStatus.Value != null)
                    {
                        prop.ContentStatus.ClearValue();

                        if (prop.ContentStatus.Value == null)
                        {
                            modified |= true;
                        }
                        else
                        {
                            sb.Append($" [ContentStatus: {prop.ContentStatus.Value}]");
                        }
                    }

                    if (prop.Document.LastAuthor.Value != null)
                    {
                        prop.Document.LastAuthor.ClearValue();

                        if (prop.Document.LastAuthor.Value == null)
                        {
                            modified |= true;
                        }
                        else
                        {
                            sb.Append($" [LastAuthor: {prop.Document.LastAuthor.Value}]");
                        }
                    }

                    if (prop.Document.Manager.Value != null)
                    {
                        prop.Document.Manager.ClearValue();

                        if (prop.Document.Manager.Value == null)
                        {
                            modified |= true;
                        }
                        else
                        {
                            sb.Append($" [Manager: {prop.Document.Manager.Value}]");
                        }
                    }

                    if (prop.Keywords.Value != null)
                    {
                        prop.Keywords.ClearValue();

                        if (prop.Keywords.Value == null)
                        {
                            modified |= true;
                        }
                        else
                        {
                            sb.Append($" [Keywords: {string.Join("; ", prop.Keywords.Value)}]");
                        }
                    }

                    if (prop.Subject.Value != null)
                    {
                        prop.Subject.ClearValue();

                        if (prop.Subject.Value == null)
                        {
                            modified |= true;
                        }
                        else
                        {
                            sb.Append($" [Subject: {prop.Subject.Value}]");
                        }
                    }

                    if (prop.Title.Value != null)
                    {
                        prop.Title.ClearValue();

                        if (prop.Title.Value == null)
                        {
                            modified |= true;
                        }
                        else
                        {
                            sb.Append($" [Title: {prop.Title.Value}]");
                        }
                    }
                }
                catch
                {
                    errors++;
                    //sb.Append(" !Unwritable.");
                    modified = true;
                    excepted = true;
                }

                if (modified)
                {
                    try
                    {
                        fi.LastWriteTime = lastWritten;
                        clean++;
                    }
                    catch
                    {
                        if (excepted)
                        {
                            sb.Append(" In use!");
                        }
                        else
                        {
                            errors++;
                            sb.Append(" Unwritable!");
                        }
                    }
                }

                if (readOnly && _readonly)
                {
                    fi.IsReadOnly = true;
                }

                string properties = sb.ToString().Trim();

                if (properties.Length > 0)
                {
                    item.ForeColor = Color.Red;
                    item.SubItems[1].Text = properties;
                }
                else
                {
                    item.Checked = false;
                    item.ForeColor = Color.DarkGreen;
                    item.SubItems[1].Text = "OK";
                }
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            CloseMenuItem_Click(sender, e);
        }

        private void ItemsList_Resize(object sender, EventArgs e)
        {
            //AlignColumns();
        }

        private void AlignColumns()
        {
            int width = ItemsList.ClientSize.Width / ItemsList.Columns.Count;

            foreach (ColumnHeader column in ItemsList.Columns)
            {
                column.Width = width;
            }
        }

        private void SetButtons()
        {
            int iData = ItemsList.Items.Count;
            int iSelect = ItemsList.SelectedItems.Count;
            int iCheck = ItemsList.CheckedItems.Count;

            bool bData = iData > 0;
            bool bSelect = iSelect > 0;
            bool bCheck = iCheck > 0;

            ResetMenuItem.Enabled = bData;
            SelectAllMenuItem.Enabled = iSelect < iData;

            DelMenuItem.Enabled = bSelect;
            CheckMenuItem.Enabled = bSelect && iCheck < iData;
            UncheckMenuItem.Enabled = bSelect && bCheck;
            CleanMenuItem.Enabled = bCheck;

            ResetButton.Enabled = ResetMenuItem.Enabled;
            CleanButton.Enabled = CleanMenuItem.Enabled;

            StatusItems.Text = bData ? $"Всего: {iData}" : string.Empty;
            StatusSelected.Text = bSelect ? $"Выбрано: {iSelect}" : string.Empty;
            StatusChecked.Text = bCheck ? $"Помечено: {iCheck}!" : string.Empty;

            if (!bData)
            {
                _onboardCollection.Clear();
            }

            if (!bData && !bSelect && !bCheck)
            {
                StatusLabel.Text = _emptyStatus;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            PersistentWindow.Restore(this);
            var p = Settings.Default;
            ItemsList.Font = p.Font;
            _mru = new RecentManager(RecentMenuItem, Recent_Click, p.MRU);

            AlignColumns();

            //foreach (ColumnHeader column in ItemsList.Columns)
            //{
            //    column.Width = -1;
            //}

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                DisplayProperties(args.Skip(1).ToArray());
            }

            SetButtons();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var p = Settings.Default;
            p.Font = ItemsList.Font;
            p.MRU = _mru.Items;
            PersistentWindow.Save(this);
        }

        private void ResetMenuItem_Click(object sender, EventArgs e)
        {
            _onboardCollection.Clear();
            ItemsList.Items.Clear();
            ItemsList.Groups.Clear();
            SetButtons();
        }

        private void FolderMenuItem_Click(object sender, EventArgs e)
        {
            if (FolderDialog.ShowDialog(this) == DialogResult.OK)
            {
                DisplayProperties(new string[]{ FolderDialog.SelectedPath });
            }
        }

        private void FileMenuItem_Click(object sender, EventArgs e)
        {
            FileDialog.InitialDirectory = FolderDialog.SelectedPath; //TODO

            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                DisplayProperties(FileDialog.FileNames);
            }
        }

        private void CleanMenuItem_Click(object sender, EventArgs e)
        {
            CleanProperties();
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            StatusLabel.Text = _exitStatus;
            Close();
        }

        private void FontMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog.Font = ItemsList.Font;

            if (FontDialog.ShowDialog() == DialogResult.OK)
            {
                ItemsList.Font = FontDialog.Font;
            }
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().Show();
        }

        private void DelMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in ItemsList.SelectedItems)
            {
                item.Remove();
            }

            SetButtons();
        }

        private void ItemsList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            SetButtons();
        }

        private void ItemsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons();
        }

        private void ItemsList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == _columnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (_columnSorter.Order == SortOrder.Ascending)
                {
                    _columnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    _columnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                _columnSorter.SortColumn = e.Column;
                _columnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            ItemsList.Sort();
        }

        private void ProgressBar_Click(object sender, EventArgs e)
        {
            _cancel = true;
        }

        private void MyDocsMenuItem_Click(object sender, EventArgs e)
        {
            DisplayProperties(new string[] { Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) });
        }

        private void DesktopMenuItem_Click(object sender, EventArgs e)
        {
            DisplayProperties(new string[] { Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) });
        }

        private void Recent_Click(object sender, EventArgs e)
        {
            var recent = sender as ToolStripMenuItem;
            DisplayProperties(new string[] { recent.Text });
        }

        private void SelectAllMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in ItemsList.Items)
            {
                item.Selected = true;
            }
        }

        private void CheckMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in ItemsList.SelectedItems)
            {
                item.Checked = true;
            }
        }

        private void UncheckMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in ItemsList.SelectedItems)
            {
                item.Checked = false;
            }
        }
    }
}
