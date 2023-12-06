namespace CheckAuthors.Net48
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.ResetButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.CleanButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.CloseButton = new System.Windows.Forms.ToolStripButton();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusItems = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusSelected = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusChecked = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.DesktopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MyDocsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FolderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RecentMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.CleanMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.CloseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ListMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ResetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SelectAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CheckMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UncheckMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.OfficeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.FontMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.FileDialog = new System.Windows.Forms.OpenFileDialog();
            this.FontDialog = new System.Windows.Forms.FontDialog();
            this.ImageList = new System.Windows.Forms.ImageList(this.components);
            this.ItemsList = new System.Windows.Forms.ListView();
            this.ItemHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ProperiesHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ToolStrip.SuspendLayout();
            this.StatusStrip.SuspendLayout();
            this.MenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ToolStrip
            // 
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ResetButton,
            this.toolStripSeparator1,
            this.CleanButton,
            this.toolStripSeparator2,
            this.CloseButton});
            this.ToolStrip.Location = new System.Drawing.Point(0, 24);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.Size = new System.Drawing.Size(800, 25);
            this.ToolStrip.TabIndex = 1;
            this.ToolStrip.Text = "toolStrip1";
            // 
            // ResetButton
            // 
            this.ResetButton.Enabled = false;
            this.ResetButton.Image = ((System.Drawing.Image)(resources.GetObject("ResetButton.Image")));
            this.ResetButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(121, 22);
            this.ResetButton.Text = "Очистить список";
            this.ResetButton.ToolTipText = "Очистка списка папок и файлов";
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // CleanButton
            // 
            this.CleanButton.Enabled = false;
            this.CleanButton.Image = ((System.Drawing.Image)(resources.GetObject("CleanButton.Image")));
            this.CleanButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CleanButton.Name = "CleanButton";
            this.CleanButton.Size = new System.Drawing.Size(126, 22);
            this.CleanButton.Text = "Удалить свойства!";
            this.CleanButton.ToolTipText = "Удаление свойств файлов (осторожно!)";
            this.CleanButton.Click += new System.EventHandler(this.CleanButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // CloseButton
            // 
            this.CloseButton.Image = ((System.Drawing.Image)(resources.GetObject("CloseButton.Image")));
            this.CloseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(62, 22);
            this.CloseButton.Text = "Выход";
            this.CloseButton.ToolTipText = "Закрыть программу";
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // StatusStrip
            // 
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusItems,
            this.StatusSelected,
            this.StatusChecked,
            this.StatusLabel,
            this.ProgressBar});
            this.StatusStrip.Location = new System.Drawing.Point(0, 428);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.ShowItemToolTips = true;
            this.StatusStrip.Size = new System.Drawing.Size(800, 22);
            this.StatusStrip.TabIndex = 2;
            this.StatusStrip.Text = "statusStrip1";
            // 
            // StatusItems
            // 
            this.StatusItems.Name = "StatusItems";
            this.StatusItems.Size = new System.Drawing.Size(41, 17);
            this.StatusItems.Text = "Всего:";
            this.StatusItems.ToolTipText = "Всего файлов в списке";
            // 
            // StatusSelected
            // 
            this.StatusSelected.Name = "StatusSelected";
            this.StatusSelected.Size = new System.Drawing.Size(65, 17);
            this.StatusSelected.Text = "Выделено:";
            this.StatusSelected.ToolTipText = "Выделеные файлы можно удалить из списка";
            // 
            // StatusChecked
            // 
            this.StatusChecked.Name = "StatusChecked";
            this.StatusChecked.Size = new System.Drawing.Size(68, 17);
            this.StatusChecked.Text = "Помечено:";
            this.StatusChecked.ToolTipText = "У помеченных файлов будут удалены свойства (осторожно!)";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(611, 17);
            this.StatusLabel.Spring = true;
            this.StatusLabel.Text = "Загрузка...";
            this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.StatusLabel.ToolTipText = "Информация";
            // 
            // ProgressBar
            // 
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.ProgressBar.Size = new System.Drawing.Size(110, 16);
            this.ProgressBar.ToolTipText = "Кликните, чтобы прервать";
            this.ProgressBar.Visible = false;
            this.ProgressBar.Click += new System.EventHandler(this.ProgressBar_Click);
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenu,
            this.ListMenu,
            this.ViewMenu,
            this.HelpMenu});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(800, 24);
            this.MenuStrip.TabIndex = 3;
            this.MenuStrip.Text = "menuStrip1";
            // 
            // FileMenu
            // 
            this.FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DesktopMenuItem,
            this.MyDocsMenuItem,
            this.FolderMenuItem,
            this.FileMenuItem,
            this.RecentMenuItem,
            this.toolStripMenuItem2,
            this.CleanMenuItem,
            this.toolStripMenuItem3,
            this.CloseMenuItem});
            this.FileMenu.Name = "FileMenu";
            this.FileMenu.Size = new System.Drawing.Size(48, 20);
            this.FileMenu.Text = "&Файл";
            // 
            // DesktopMenuItem
            // 
            this.DesktopMenuItem.Name = "DesktopMenuItem";
            this.DesktopMenuItem.Size = new System.Drawing.Size(230, 22);
            this.DesktopMenuItem.Text = "&Рабочий стол";
            this.DesktopMenuItem.Click += new System.EventHandler(this.DesktopMenuItem_Click);
            // 
            // MyDocsMenuItem
            // 
            this.MyDocsMenuItem.Name = "MyDocsMenuItem";
            this.MyDocsMenuItem.Size = new System.Drawing.Size(230, 22);
            this.MyDocsMenuItem.Text = "&Мои документы";
            this.MyDocsMenuItem.Click += new System.EventHandler(this.MyDocsMenuItem_Click);
            // 
            // FolderMenuItem
            // 
            this.FolderMenuItem.Name = "FolderMenuItem";
            this.FolderMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.FolderMenuItem.Size = new System.Drawing.Size(230, 22);
            this.FolderMenuItem.Text = "Добавить &папку...";
            this.FolderMenuItem.Click += new System.EventHandler(this.FolderMenuItem_Click);
            // 
            // FileMenuItem
            // 
            this.FileMenuItem.Name = "FileMenuItem";
            this.FileMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.FileMenuItem.Size = new System.Drawing.Size(230, 22);
            this.FileMenuItem.Text = "Добавить &файл...";
            this.FileMenuItem.Click += new System.EventHandler(this.FileMenuItem_Click);
            // 
            // RecentMenuItem
            // 
            this.RecentMenuItem.Enabled = false;
            this.RecentMenuItem.Name = "RecentMenuItem";
            this.RecentMenuItem.Size = new System.Drawing.Size(230, 22);
            this.RecentMenuItem.Text = "П&оследние";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(227, 6);
            // 
            // CleanMenuItem
            // 
            this.CleanMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("CleanMenuItem.Image")));
            this.CleanMenuItem.Name = "CleanMenuItem";
            this.CleanMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F8)));
            this.CleanMenuItem.Size = new System.Drawing.Size(230, 22);
            this.CleanMenuItem.Text = "Удалить &свойства!";
            this.CleanMenuItem.Click += new System.EventHandler(this.CleanMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(227, 6);
            // 
            // CloseMenuItem
            // 
            this.CloseMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("CloseMenuItem.Image")));
            this.CloseMenuItem.Name = "CloseMenuItem";
            this.CloseMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.CloseMenuItem.Size = new System.Drawing.Size(230, 22);
            this.CloseMenuItem.Text = "В&ыход";
            this.CloseMenuItem.Click += new System.EventHandler(this.CloseMenuItem_Click);
            // 
            // ListMenu
            // 
            this.ListMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ResetMenuItem,
            this.DelMenuItem,
            this.SelectAllMenuItem,
            this.CheckMenuItem,
            this.UncheckMenuItem});
            this.ListMenu.Name = "ListMenu";
            this.ListMenu.Size = new System.Drawing.Size(60, 20);
            this.ListMenu.Text = "&Список";
            // 
            // ResetMenuItem
            // 
            this.ResetMenuItem.Enabled = false;
            this.ResetMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("ResetMenuItem.Image")));
            this.ResetMenuItem.Name = "ResetMenuItem";
            this.ResetMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.ResetMenuItem.Size = new System.Drawing.Size(219, 22);
            this.ResetMenuItem.Text = "&Очистить список";
            this.ResetMenuItem.Click += new System.EventHandler(this.ResetMenuItem_Click);
            // 
            // DelMenuItem
            // 
            this.DelMenuItem.Enabled = false;
            this.DelMenuItem.Name = "DelMenuItem";
            this.DelMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.DelMenuItem.Size = new System.Drawing.Size(219, 22);
            this.DelMenuItem.Text = "&Удалить из списка";
            // 
            // SelectAllMenuItem
            // 
            this.SelectAllMenuItem.Enabled = false;
            this.SelectAllMenuItem.Name = "SelectAllMenuItem";
            this.SelectAllMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.SelectAllMenuItem.Size = new System.Drawing.Size(219, 22);
            this.SelectAllMenuItem.Text = "&Выделить все";
            this.SelectAllMenuItem.Click += new System.EventHandler(this.SelectAllMenuItem_Click);
            // 
            // CheckMenuItem
            // 
            this.CheckMenuItem.Enabled = false;
            this.CheckMenuItem.Name = "CheckMenuItem";
            this.CheckMenuItem.Size = new System.Drawing.Size(219, 22);
            this.CheckMenuItem.Text = "&Пометить";
            this.CheckMenuItem.Click += new System.EventHandler(this.CheckMenuItem_Click);
            // 
            // UncheckMenuItem
            // 
            this.UncheckMenuItem.Enabled = false;
            this.UncheckMenuItem.Name = "UncheckMenuItem";
            this.UncheckMenuItem.Size = new System.Drawing.Size(219, 22);
            this.UncheckMenuItem.Text = "&Снять отметки";
            this.UncheckMenuItem.Click += new System.EventHandler(this.UncheckMenuItem_Click);
            // 
            // ViewMenu
            // 
            this.ViewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OfficeMenuItem,
            this.toolStripSeparator4,
            this.FontMenuItem});
            this.ViewMenu.Name = "ViewMenu";
            this.ViewMenu.Size = new System.Drawing.Size(39, 20);
            this.ViewMenu.Text = "&Вид";
            // 
            // OfficeMenuItem
            // 
            this.OfficeMenuItem.Checked = true;
            this.OfficeMenuItem.CheckOnClick = true;
            this.OfficeMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.OfficeMenuItem.Name = "OfficeMenuItem";
            this.OfficeMenuItem.Size = new System.Drawing.Size(180, 22);
            this.OfficeMenuItem.Text = "&Только doc, xls";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(153, 6);
            // 
            // FontMenuItem
            // 
            this.FontMenuItem.Name = "FontMenuItem";
            this.FontMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.FontMenuItem.Size = new System.Drawing.Size(156, 22);
            this.FontMenuItem.Text = "&Шрифт...";
            this.FontMenuItem.Click += new System.EventHandler(this.FontMenuItem_Click);
            // 
            // HelpMenu
            // 
            this.HelpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AboutMenuItem});
            this.HelpMenu.Name = "HelpMenu";
            this.HelpMenu.Size = new System.Drawing.Size(65, 20);
            this.HelpMenu.Text = "С&правка";
            // 
            // AboutMenuItem
            // 
            this.AboutMenuItem.Name = "AboutMenuItem";
            this.AboutMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.AboutMenuItem.Size = new System.Drawing.Size(177, 22);
            this.AboutMenuItem.Text = "&О программе...";
            this.AboutMenuItem.Click += new System.EventHandler(this.AboutMenuItem_Click);
            // 
            // FolderDialog
            // 
            this.FolderDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.FolderDialog.ShowNewFolderButton = false;
            // 
            // FileDialog
            // 
            this.FileDialog.Filter = "Документы|*.doc*;*.xls*|Все файлы|*.*";
            this.FileDialog.InitialDirectory = "C:\\";
            this.FileDialog.Multiselect = true;
            this.FileDialog.SupportMultiDottedExtensions = true;
            this.FileDialog.Title = "Обзор файлов";
            // 
            // FontDialog
            // 
            this.FontDialog.AllowVerticalFonts = false;
            this.FontDialog.FontMustExist = true;
            this.FontDialog.MaxSize = 16;
            this.FontDialog.MinSize = 8;
            this.FontDialog.ShowEffects = false;
            // 
            // ImageList
            // 
            this.ImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.ImageList.TransparentColor = System.Drawing.SystemColors.Window;
            // 
            // ItemsList
            // 
            this.ItemsList.AllowDrop = true;
            this.ItemsList.CheckBoxes = true;
            this.ItemsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ItemHeader,
            this.ProperiesHeader});
            this.ItemsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ItemsList.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ItemsList.FullRowSelect = true;
            this.ItemsList.GridLines = true;
            this.ItemsList.HideSelection = false;
            this.ItemsList.LargeImageList = this.ImageList;
            this.ItemsList.Location = new System.Drawing.Point(0, 49);
            this.ItemsList.Name = "ItemsList";
            this.ItemsList.ShowItemToolTips = true;
            this.ItemsList.Size = new System.Drawing.Size(800, 379);
            this.ItemsList.SmallImageList = this.ImageList;
            this.ItemsList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.ItemsList.TabIndex = 0;
            this.ItemsList.UseCompatibleStateImageBehavior = false;
            this.ItemsList.View = System.Windows.Forms.View.Details;
            this.ItemsList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ItemsList_ColumnClick);
            this.ItemsList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ItemsList_ItemChecked);
            this.ItemsList.SelectedIndexChanged += new System.EventHandler(this.ItemsList_SelectedIndexChanged);
            this.ItemsList.DragDrop += new System.Windows.Forms.DragEventHandler(this.ItemsList_DragDrop);
            this.ItemsList.DragEnter += new System.Windows.Forms.DragEventHandler(this.ItemsList_DragEnter);
            this.ItemsList.Resize += new System.EventHandler(this.ItemsList_Resize);
            // 
            // ItemHeader
            // 
            this.ItemHeader.Text = "Папки и файлы";
            // 
            // ProperiesHeader
            // 
            this.ProperiesHeader.Text = "Свойства";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ItemsList);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.ToolStrip);
            this.Controls.Add(this.MenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuStrip;
            this.Name = "MainForm";
            this.Text = "Check Authors";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView ItemsList;
        private System.Windows.Forms.ColumnHeader ItemHeader;
        private System.Windows.Forms.ColumnHeader ProperiesHeader;
        private System.Windows.Forms.ToolStrip ToolStrip;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripButton CleanButton;
        private System.Windows.Forms.ToolStripButton ResetButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton CloseButton;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem FileMenu;
        private System.Windows.Forms.ToolStripMenuItem FolderMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem CleanMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem CloseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewMenu;
        private System.Windows.Forms.ToolStripMenuItem FontMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpMenu;
        private System.Windows.Forms.ToolStripMenuItem AboutMenuItem;
        private System.Windows.Forms.FolderBrowserDialog FolderDialog;
        private System.Windows.Forms.OpenFileDialog FileDialog;
        private System.Windows.Forms.FontDialog FontDialog;
        private System.Windows.Forms.ImageList ImageList;
        private System.Windows.Forms.ToolStripStatusLabel StatusItems;
        private System.Windows.Forms.ToolStripStatusLabel StatusSelected;
        private System.Windows.Forms.ToolStripStatusLabel StatusChecked;
        private System.Windows.Forms.ToolStripProgressBar ProgressBar;
        private System.Windows.Forms.ToolStripMenuItem OfficeMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem MyDocsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DesktopMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RecentMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ListMenu;
        private System.Windows.Forms.ToolStripMenuItem ResetMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DelMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SelectAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CheckMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UncheckMenuItem;
    }
}

