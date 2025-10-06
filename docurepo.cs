using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class docurepo : UserControl
    {
        public docurepo()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            panelDesktop.AutoScroll = true;
            SetupFilterButtons();

            flowBreadcrumb.WrapContents = false;
            flowBreadcrumb.AutoSize = false;
            flowBreadcrumb.Height = 25; 
            flowBreadcrumb.AutoScroll = false;

            LoadDesktopItems(); 
        }
        private ToolStripMenuItem activeTypeMenuItem = null; //for Type
        private ToolStripMenuItem activeDateMenuItem = null;//for date added
        private ToolStripMenuItem activeModifiedMenuItem = null;//for modified

        // RefreshAllData fetches directly from DB
        public void RefreshAllData()
        {
            // Clear and reload desktopItems from DB without changing layout
            desktopItems.Clear();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM DesktopItems", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DesktopItem item = new DesktopItem
                    {
                        ItemId = (int)reader["ItemId"],
                        Name = (string)reader["Name"],
                        IsFolder = (bool)reader["IsFolder"],
                        FilePath = reader["FilePath"] == DBNull.Value ? null : (string)reader["FilePath"],
                        ParentId = reader["ParentId"] == DBNull.Value ? null : (int?)reader["ParentId"],
                        CreatedAt = (DateTime)reader["CreatedAt"],
                        ModifiedAt = (DateTime)reader["ModifiedAt"]
                    };
                    desktopItems.Add(item);
                }
            }

            foreach (var item in desktopItems)
            {
                if (item.ParentId.HasValue)
                {
                    var parent = desktopItems.FirstOrDefault(d => d.ItemId == item.ParentId.Value);
                    if (parent != null)
                    {
                        item.Parent = parent;
                        parent.Children.Add(item);
                    }
                }
            }

            // Redisplay current folder or root
            DisplayItems(currentFolder == null
                ? desktopItems.Where(d => d.Parent == null).ToList()
                : currentFolder.Children);
        }
        private void SetupFilterButtons()
        {
            //TYPE FILTER BUTTON
            ContextMenuStrip typeMenu = new ContextMenuStrip();
            var typeMappings = new Dictionary<string, string[]>
    {
        { "All Types", null },
        { "Folders", null },
        { "Images", new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" } },
        { "Word Documents", new[] { ".doc", ".docx" } },
        { "Excel Sheets", new[] { ".xls", ".xlsx" } },
        { "PowerPoint", new[] { ".ppt", ".pptx" } },
        { "PDF", new[] { ".pdf" } },
        { "Text Files", new[] { ".txt" } },
        { "Music", new[] { ".mp3", ".wav", ".flac", ".aac", ".ogg" } },
        { "Videos", new[] { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv" } },
        { "Other Files", new string[] { } }
    };

            foreach (var kvp in typeMappings)
            {
                typeMenu.Items.Add(CreateTypeMenuItem(kvp.Key, kvp.Value, typeMappings));
            }

            btnType.Click += (s, e) => typeMenu.Show(btnType, 0, btnType.Height);

            // LAST ADDED FILTER BUTTON
            ContextMenuStrip addedMenu = new ContextMenuStrip();
            string[] dateFilters = { "Today", "Last 7 Days", "Last 30 Days", "This Year", "Last Year", "Older" };
            foreach (var filter in dateFilters)
            {
                var menuItem = new ToolStripMenuItem(filter);
                menuItem.Click += (s, e) =>
                {
                    FilterByDate(filter);
                    SetActiveMenuItem(menuItem, ref activeDateMenuItem);
                };
                addedMenu.Items.Add(menuItem);
            }
            btnDate.Click += (s, e) => addedMenu.Show(btnDate, 0, btnDate.Height);

            // LAST MODIFIED FILTER BUTTON 
            ContextMenuStrip modifiedMenu = new ContextMenuStrip();
            foreach (var filter in dateFilters)
            {
                var menuItem = new ToolStripMenuItem(filter);
                menuItem.Click += (s, e) =>
                {
                    FilterByModifiedDate(filter); 
                    SetActiveMenuItem(menuItem, ref activeModifiedMenuItem);
                };
                modifiedMenu.Items.Add(menuItem);
            }
            btnModified.Click += (s, e) => modifiedMenu.Show(btnModified, 0, btnModified.Height);

            // RESET FILTER
            lblResetFilter.Visible = false;
            lblResetFilter.Text = "Reset Filter";
            lblResetFilter.Click += (s, e) =>
            {
                DisplayItems(currentFolder == null ? desktopItems.Where(d => d.Parent == null).ToList() : currentFolder.Children);
                lblResetFilter.Visible = false;

                btnType.BackColor = SystemColors.Control;
                btnDate.BackColor = SystemColors.Control;
                btnModified.BackColor = SystemColors.Control;

                if (activeTypeMenuItem != null) ResetMenuItem(activeTypeMenuItem);
                if (activeDateMenuItem != null) ResetMenuItem(activeDateMenuItem);
                if (activeModifiedMenuItem != null) ResetMenuItem(activeModifiedMenuItem);

                activeTypeMenuItem = null;
                activeDateMenuItem = null;
                activeModifiedMenuItem = null;
            };
        }

        // HELPER FUNCTIONS
        private ToolStripMenuItem CreateTypeMenuItem(string displayName, string[] extensions, Dictionary<string, string[]> typeMappings)
        {
            var item = new ToolStripMenuItem(displayName);
            item.Click += (s, e) =>
            {
                FilterByType(displayName, extensions, typeMappings);
                SetActiveMenuItem(item, ref activeTypeMenuItem);
            };
            return item;
        }

        private void SetActiveMenuItem(ToolStripMenuItem clicked, ref ToolStripMenuItem activeItem)
        {
            if (activeItem != null) ResetMenuItem(activeItem);

            activeItem = clicked;
            activeItem.BackColor = Color.DodgerBlue;
            activeItem.ForeColor = Color.White;

            lblResetFilter.Visible = true;
        }

        private void ResetMenuItem(ToolStripMenuItem item)
        {
            item.BackColor = SystemColors.Control;
            item.ForeColor = Color.Black;
        }

        //FILTER BY TYPE
        private void FilterByType(string displayName, string[] extensions, Dictionary<string, string[]> typeMappings)
        {
            List<DesktopItem> itemsToShow = currentFolder == null
                ? desktopItems.Where(d => d.Parent == null).ToList()
                : currentFolder.Children;

            if (displayName == "All Types") { }
            else if (displayName == "Folders") itemsToShow = itemsToShow.Where(d => d.IsFolder).ToList();
            else if (extensions != null && extensions.Length > 0)
                itemsToShow = itemsToShow.Where(d => !d.IsFolder && d.FilePath != null && extensions.Contains(System.IO.Path.GetExtension(d.FilePath).ToLower())).ToList();
            else // Other files
                itemsToShow = itemsToShow.Where(d => !d.IsFolder && (d.FilePath == null || !typeMappings.Values.Where(v => v != null).Any(exts => exts.Contains(System.IO.Path.GetExtension(d.FilePath).ToLower())))).ToList();

            lblResetFilter.Visible = true;
            DisplayItems(itemsToShow);
        }

        // FILTER BY DATE
        private void FilterByDate(string option)
        {
            List<DesktopItem> itemsToShow = currentFolder == null
                ? desktopItems.Where(d => d.Parent == null).ToList()
                : currentFolder.Children;

            DateTime now = DateTime.Now;

            switch (option)
            {
                case "Today": itemsToShow = itemsToShow.Where(d => d.CreatedAt.Date == now.Date).ToList(); break;
                case "Last 7 Days": itemsToShow = itemsToShow.Where(d => d.CreatedAt >= now.AddDays(-7)).ToList(); break;
                case "Last 30 Days": itemsToShow = itemsToShow.Where(d => d.CreatedAt >= now.AddDays(-30)).ToList(); break;
                case "This Year": itemsToShow = itemsToShow.Where(d => d.CreatedAt.Year == now.Year).ToList(); break;
                case "Last Year":
                    DateTime startOfLastYear = new DateTime(now.Year - 1, 1, 1);
                    DateTime endOfLastYear = new DateTime(now.Year - 1, 12, 31, 23, 59, 59);
                    itemsToShow = itemsToShow.Where(d => d.CreatedAt >= startOfLastYear && d.CreatedAt <= endOfLastYear).ToList();
                    break;
                case "Older":
                    DateTime startOfThisYear = new DateTime(now.Year, 1, 1);
                    itemsToShow = itemsToShow.Where(d => d.CreatedAt < startOfThisYear).ToList();
                    break;
            }

            lblResetFilter.Visible = true;
            DisplayItems(itemsToShow);
        }

        //FILTER BY MODIFIED DATE 
        private void FilterByModifiedDate(string option)
        {
            List<DesktopItem> itemsToShow = currentFolder == null
                ? desktopItems.Where(d => d.Parent == null).ToList()
                : currentFolder.Children;

            DateTime now = DateTime.Now;

            switch (option)
            {
                case "Today": itemsToShow = itemsToShow.Where(d => d.ModifiedAt.Date == now.Date).ToList(); break;
                case "Last 7 Days": itemsToShow = itemsToShow.Where(d => d.ModifiedAt >= now.AddDays(-7)).ToList(); break;
                case "Last 30 Days": itemsToShow = itemsToShow.Where(d => d.ModifiedAt >= now.AddDays(-30)).ToList(); break;
                case "This Year": itemsToShow = itemsToShow.Where(d => d.ModifiedAt.Year == now.Year).ToList(); break;
                case "Last Year":
                    DateTime startOfLastYear = new DateTime(now.Year - 1, 1, 1);
                    DateTime endOfLastYear = new DateTime(now.Year - 1, 12, 31, 23, 59, 59);
                    itemsToShow = itemsToShow.Where(d => d.ModifiedAt >= startOfLastYear && d.ModifiedAt <= endOfLastYear).ToList();
                    break;
                case "Older":
                    DateTime startOfThisYear = new DateTime(now.Year, 1, 1);
                    itemsToShow = itemsToShow.Where(d => d.ModifiedAt < startOfThisYear).ToList();
                    break;
            }

            lblResetFilter.Visible = true;
            DisplayItems(itemsToShow);
        }

        private void button2_Click(object sender, EventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }

        // Panel layout settings
        int nextX = 10;
        int nextY = 10;
        int itemWidth = 80;
        int itemHeight = 100;
        int padding = 10;

        // In-memory tracking
        List<DesktopItem> desktopItems = new List<DesktopItem>();
        DesktopItem currentFolder = null; // null = root

        // Context menu for right-click
        ContextMenuStrip contextMenu = new ContextMenuStrip();

        // DesktopItem class
        class DesktopItem
        {
            public int ItemId { get; set; }
            public string Name { get; set; }
            public bool IsFolder { get; set; }
            public string FilePath { get; set; } = null; // for real files
            public List<DesktopItem> Children { get; set; } = new List<DesktopItem>();
            public DesktopItem Parent { get; set; } = null;
            public int? ParentId { get; set; } = null;
            public DateTime CreatedAt { get; set; }
            public DateTime ModifiedAt { get; set; }
        }

        // Add Folder button
        private void buttonAddFolder_Click(object sender, EventArgs e)
        {
            string baseName = "Folder";
            string newName = baseName;
            int counter = 1;

            // Get existing folder names in current folder
            List<string> existingNames = (currentFolder == null
                ? desktopItems.Where(d => d.Parent == null && d.IsFolder).Select(d => d.Name).ToList()
                : currentFolder.Children.Where(d => d.IsFolder).Select(d => d.Name).ToList());

            while (existingNames.Contains(newName))
            {
                counter++;
                newName = $"{baseName} {counter}";
            }
            AddItemToDatabase(newName, true, null);
        }


        // Add File button
        private void buttonAddFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select a file to add";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = ofd.FileName;
                    string fileName = System.IO.Path.GetFileName(filePath);

                    AddItemToDatabase(fileName, false, filePath);
                }
            }
        }

        // Add to database + memory + panel
        private void AddItemToDatabase(string name, bool isFolder, string filePath)
        {
            int? parentId = currentFolder?.ItemId;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                "INSERT INTO DesktopItems (Name, IsFolder, ParentId, IconType, FilePath, CreatedAt, ModifiedAt) OUTPUT INSERTED.ItemId VALUES (@name, @isFolder, @parentId, @icon, @filePath, @created, @modified)",
                conn);

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@isFolder", isFolder);
                cmd.Parameters.AddWithValue("@parentId", parentId.HasValue ? (object)parentId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@icon", isFolder ? "folder" : "file");
                cmd.Parameters.AddWithValue("@filePath", string.IsNullOrEmpty(filePath) ? DBNull.Value : (object)filePath);
                cmd.Parameters.AddWithValue("@created", DateTime.Now);
                cmd.Parameters.AddWithValue("@modified", DateTime.Now);

                int newId = (int)cmd.ExecuteScalar();

                DesktopItem newItem = new DesktopItem
                {
                    ItemId = newId,
                    Name = name,
                    IsFolder = isFolder,
                    FilePath = filePath,
                    Parent = currentFolder,
                    ParentId = parentId
                };

                if (currentFolder == null)
                    desktopItems.Add(newItem);
                else
                    currentFolder.Children.Add(newItem);

                AddItemPanel(newItem);
            }
        }

        // Load all items from DB
        private void LoadDesktopItems()
        {
            desktopItems.Clear();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM DesktopItems", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DesktopItem item = new DesktopItem
                    {
                        ItemId = (int)reader["ItemId"],
                        Name = (string)reader["Name"],
                        IsFolder = (bool)reader["IsFolder"],
                        FilePath = reader["FilePath"] == DBNull.Value ? null : (string)reader["FilePath"],
                        ParentId = reader["ParentId"] == DBNull.Value ? null : (int?)reader["ParentId"],
                        CreatedAt = (DateTime)reader["CreatedAt"],
                        ModifiedAt = (DateTime)reader["ModifiedAt"]
                    };
                    desktopItems.Add(item);
                }
            }

            // Build parent-child relationships
            foreach (var item in desktopItems)
            {
                if (item.ParentId.HasValue)
                {
                    var parent = desktopItems.FirstOrDefault(d => d.ItemId == item.ParentId.Value);
                    if (parent != null)
                    {
                        item.Parent = parent;
                        parent.Children.Add(item);
                    }
                }
            }

            DisplayItems(desktopItems.Where(d => d.Parent == null).ToList());
        }

        // Display a list of items in the panel
        private void DisplayItems(List<DesktopItem> items)
        {
            panelDesktop.Controls.Clear();
            nextX = 10;
            nextY = 10;

            if (items.Count == 0)
            {
                // Show the No Results picture
                PictureBox noResults = new PictureBox();
                noResults.Image = Properties.Resources.noResult; 
                noResults.SizeMode = PictureBoxSizeMode.Zoom; 
                noResults.Size = new Size(200, 200); 
                noResults.Anchor = AnchorStyles.None; 
                panelDesktop.Controls.Add(noResults);
                noResults.Left = (panelDesktop.Width - noResults.Width) / 2;
                noResults.Top = (panelDesktop.Height - noResults.Height) / 2;
                return; 
            }

            // Add each item panel
            foreach (var item in items)
            {
                AddItemPanel(item);
            }

            UpdateBreadcrumb();
        }
        // Create panel for a single DesktopItem
        private void AddItemPanel(DesktopItem item)
        {
            Panel itemPanel = new Panel();
            itemPanel.Width = itemWidth;
            itemPanel.Height = itemHeight;
            itemPanel.Tag = item;

            PictureBox icon = new PictureBox();
            icon.Image = GetItemIcon(item);
            icon.SizeMode = PictureBoxSizeMode.StretchImage;
            icon.Width = 60;
            icon.Height = 60;
            icon.Top = 0;
            icon.Left = (itemWidth - icon.Width) / 2;

            Label nameLabel = new Label();
            nameLabel.Text = item.Name;
            nameLabel.Top = 65;
            nameLabel.Width = itemWidth;
            nameLabel.TextAlign = ContentAlignment.MiddleCenter;

            itemPanel.Controls.Add(icon);
            itemPanel.Controls.Add(nameLabel);

            itemPanel.Left = nextX;
            itemPanel.Top = nextY;
            panelDesktop.Controls.Add(itemPanel);

            nextX += itemWidth + padding;
            if (nextX + itemWidth > panelDesktop.Width)
            {
                nextX = 10;
                nextY += itemHeight + padding;
            }

            // Attach events
            itemPanel.MouseDoubleClick += ItemPanel_MouseDoubleClick;
            itemPanel.MouseUp += ItemPanel_MouseUp;

            // Forward events from icon/label
            icon.MouseDoubleClick += (s, e) => ItemPanel_MouseDoubleClick(itemPanel, null);
            nameLabel.MouseDoubleClick += (s, e) => ItemPanel_MouseDoubleClick(itemPanel, null);
            icon.MouseUp += (s, e) => ItemPanel_MouseUp(itemPanel, e);
            nameLabel.MouseUp += (s, e) => ItemPanel_MouseUp(itemPanel, e);
        }

        private Image GetItemIcon(DesktopItem item)
        {
            if (item.IsFolder)
                return Properties.Resources.folderIcon;

            if (!string.IsNullOrEmpty(item.FilePath))
            {
                string ext = System.IO.Path.GetExtension(item.FilePath).ToLower();

                switch (ext)
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                    case ".gif":
                    case ".bmp":
                        return Properties.Resources.imageIcon;

                    case ".doc":
                    case ".docx":
                        return Properties.Resources.wordIcon;

                    case ".xls":
                    case ".xlsx":
                        return Properties.Resources.excelIcon;

                    case ".ppt":
                    case ".pptx":
                        return Properties.Resources.pptIcon;

                    case ".pdf":
                        return Properties.Resources.pdfIcon;

                    case ".txt":
                        return Properties.Resources.textIcon;

                    case ".mp3":
                    case ".wav":
                    case ".flac":
                    case ".aac":
                    case ".ogg":
                        return Properties.Resources.musicIcon;

                    case ".mp4":
                    case ".avi":
                    case ".mkv":
                    case ".mov":
                    case ".wmv":
                    case ".flv":
                        return Properties.Resources.videoIcon;
                }
            }

            return Properties.Resources.fileIcon;
        }

        private void ItemPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel?.Tag is DesktopItem item)
            {
                if (item.IsFolder)
                {
                    currentFolder = item;
                    DisplayItems(item.Children);
                    UpdateBreadcrumb();
                }
                else if (!string.IsNullOrEmpty(item.FilePath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(item.FilePath) { UseShellExecute = true });
                }
            }
        }

        private void ItemPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Panel panel = sender as Panel;
                if (panel != null)
                {
                    contextMenu.Tag = panel;
                    contextMenu.Items.Clear();
                    contextMenu.Items.Add("Rename");
                    contextMenu.Items.Add("Delete");
                    contextMenu.Items.Add("Details");
                    contextMenu.ItemClicked -= ContextMenu_ItemClicked;
                    contextMenu.ItemClicked += ContextMenu_ItemClicked;
                    contextMenu.Show(panel, e.Location);
                }
            }
        }

        private void ContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (contextMenu.Tag is Panel panel && panel.Tag is DesktopItem item)
            {
                if (e.ClickedItem.Text == "Delete")
                {
                    DeleteItem(item);
                    panelDesktop.Controls.Remove(panel);
                }
                else if (e.ClickedItem.Text == "Rename")
                {
                    string newName = Microsoft.VisualBasic.Interaction.InputBox(
                        "Enter new name:", "Rename Item", item.Name);

                    if (!string.IsNullOrWhiteSpace(newName))
                    {
                        // Check if name already exists in the same folder (folders and files)
                        if (IsNameExistsInCurrentFolder(newName, item))
                        {
                            MessageBox.Show("An item with this name already exists in this folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        RenameItem(item, newName);

                        Label lbl = panel.Controls.OfType<Label>().FirstOrDefault();
                        if (lbl != null) lbl.Text = newName;
                    }
                }
                else if (e.ClickedItem.Text == "Details")
                {
                    contextMenu.Close();

                    DesktopItem detailsItem = (contextMenu.Tag as Panel)?.Tag as DesktopItem;
                    if (detailsItem == null) return;

                    string typeText;
                    if (detailsItem.IsFolder)
                    {
                        typeText = "Folder";
                    }
                    else if (!string.IsNullOrEmpty(detailsItem.FilePath))
                    {
                        typeText = System.IO.Path.GetExtension(detailsItem.FilePath).TrimStart('.').ToUpper(); 
                    }
                    else
                    {
                        typeText = "File";
                    }

                    string fileSizeText = "N/A";
                    if (!detailsItem.IsFolder && !string.IsNullOrEmpty(detailsItem.FilePath) && System.IO.File.Exists(detailsItem.FilePath))
                    {
                        long bytes = new System.IO.FileInfo(detailsItem.FilePath).Length;
                        double size = bytes;
                        string[] units = { "B", "KB", "MB", "GB", "TB" };
                        int unitIndex = 0;
                        while (size >= 1024 && unitIndex < units.Length - 1)
                        {
                            size /= 1024;
                            unitIndex++;
                        }
                        fileSizeText = $"{size:0.##} {units[unitIndex]}";
                    }

                    string details = $"Name: {detailsItem.Name}\n" +
                                     $"Type: {typeText}\n" +
                                     $"Full Path: {(detailsItem.FilePath ?? "N/A")}\n" +
                                     $"File Size: {fileSizeText}\n" +
                                     $"Created At: {detailsItem.CreatedAt}\n" +
                                     $"Modified At: {detailsItem.ModifiedAt}";

                    MessageBox.Show(details, "Item Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }
    
        // Helper to check if the name already exists in the same folder
        private bool IsNameExistsInCurrentFolder(string name, DesktopItem currentItem)
        {
            var siblings = currentItem.Parent == null
                ? desktopItems.Where(d => d.Parent == null)
                : currentItem.Parent.Children;

            return siblings.Any(d => d.ItemId != currentItem.ItemId && d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }


        private void RenameItem(DesktopItem item, string newName)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE DesktopItems SET Name = @name, ModifiedAt = @modified WHERE ItemId = @id",
                    conn
                );
                cmd.Parameters.AddWithValue("@name", newName);
                cmd.Parameters.AddWithValue("@id", item.ItemId);
                cmd.Parameters.AddWithValue("@modified", DateTime.Now);
                cmd.ExecuteNonQuery();
            }

            item.Name = newName;
        }

        private void DeleteItem(DesktopItem item)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string query = @"
            WITH RecursiveCTE AS (
                SELECT ItemId
                FROM DesktopItems
                WHERE ItemId = @id
                UNION ALL
                SELECT d.ItemId
                FROM DesktopItems d
                INNER JOIN RecursiveCTE r ON d.ParentId = r.ItemId
            )
            DELETE FROM DesktopItems
            WHERE ItemId IN (SELECT ItemId FROM RecursiveCTE);";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", item.ItemId);
                cmd.ExecuteNonQuery();
            }

            // Update UI
            if (item.Parent == null)
                desktopItems.Remove(item);
            else
                item.Parent.Children.Remove(item);
        }

        private void searchbtn_Click(object sender, EventArgs e)
        {
            string query = searchDocu.Text.Trim();
            SearchItemsInDatabase(query);
        }

        private void SearchItemsInDatabase(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                DisplayItems(currentFolder == null
                    ? desktopItems.Where(d => d.Parent == null).ToList()
                    : currentFolder.Children);
                return;
            }

            List<DesktopItem> results = new List<DesktopItem>();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = @"
                SELECT * FROM DesktopItems
                WHERE Name LIKE @query
                AND (ParentId = @parentId OR @parentId IS NULL)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@query", "%" + query + "%");
                    cmd.Parameters.AddWithValue("@parentId", currentFolder?.ItemId ?? (object)DBNull.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DesktopItem item = new DesktopItem
                            {
                                ItemId = (int)reader["ItemId"],
                                Name = (string)reader["Name"],
                                IsFolder = (bool)reader["IsFolder"],
                                FilePath = reader["FilePath"] == DBNull.Value ? null : (string)reader["FilePath"],
                                ParentId = reader["ParentId"] == DBNull.Value ? null : (int?)reader["ParentId"]
                            };
                            results.Add(item);
                        }
                    }
                }
            }

            foreach (var item in results)
            {
                if (item.ParentId.HasValue)
                    item.Parent = desktopItems.FirstOrDefault(d => d.ItemId == item.ParentId.Value);
            }

            DisplayItems(results);
        }

        private void searchDocu_TextChanged(object sender, EventArgs e)
        {
            string query = searchDocu.Text.Trim();

            if (string.IsNullOrEmpty(query))
            {
                DisplayItems(currentFolder == null
                    ? desktopItems.Where(d => d.Parent == null).ToList()
                    : currentFolder.Children);
            }
        }

        private void btnSafeguard_Click(object sender, EventArgs e)
        {
            BackupRestoreManager backupRestoreManager = new BackupRestoreManager(this);
            backupRestoreManager.Show();
        }
        private void UpdateBreadcrumb()
        {
            flowBreadcrumb.Controls.Clear();

            // Build full path stack
            List<DesktopItem> path = new List<DesktopItem>();
            DesktopItem folder = currentFolder;
            while (folder != null)
            {
                path.Insert(0, folder);
                folder = folder.Parent;
            }

            // Always show Root first
            LinkLabel rootLink = new LinkLabel();
            rootLink.Text = "Root";
            rootLink.AutoSize = true;
            rootLink.LinkClicked += (s, e) =>
            {
                currentFolder = null;
                DisplayItems(desktopItems.Where(d => d.Parent == null).ToList());
                UpdateBreadcrumb();
            };
            flowBreadcrumb.Controls.Add(rootLink);

            // If too many levels, compress
            int maxVisible = 3; // show last 3 folders max
            if (path.Count > maxVisible)
            {
                AddSeparator();

                Label ellipsis = new Label();
                ellipsis.Text = "...";
                ellipsis.AutoSize = true;
                flowBreadcrumb.Controls.Add(ellipsis);

                // Only take last N
                path = path.Skip(path.Count - maxVisible).ToList();
            }

            // Render remaining path
            foreach (var f in path)
            {
                AddSeparator();

                LinkLabel link = new LinkLabel();
                link.Text = f.Name;
                link.AutoSize = true;
                link.LinkClicked += (s, e) =>
                {
                    currentFolder = f;
                    DisplayItems(f.Children);
                    UpdateBreadcrumb();
                };
                flowBreadcrumb.Controls.Add(link);
            }
        }

        private void AddSeparator()
        {
            Label sep = new Label();
            sep.Text = " > ";
            sep.AutoSize = true;
            flowBreadcrumb.Controls.Add(sep);
        }
    }
}
