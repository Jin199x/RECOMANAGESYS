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
            panelDesktop.AutoScroll = true;

            // Back button
            buttonBack.Text = "Back";
            buttonBack.Click += buttonBack_Click;

            LoadDesktopItems(); // load items from DB on start
        }

        // --- UPDATED: RefreshAllData fetches directly from DB ---
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
                        ParentId = reader["ParentId"] == DBNull.Value ? null : (int?)reader["ParentId"]
                    };
                    desktopItems.Add(item);
                }
            }

            // Rebuild parent-child relationships
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
        }

        // Add Folder button
        private void buttonAddFolder_Click(object sender, EventArgs e)
        {
            AddItemToDatabase("Folder1", true, null);
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
                    "INSERT INTO DesktopItems (Name, IsFolder, ParentId, IconType, FilePath) OUTPUT INSERTED.ItemId VALUES (@name, @isFolder, @parentId, @icon, @filePath)",
                    conn
                );
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@isFolder", isFolder);
                cmd.Parameters.AddWithValue("@parentId", parentId.HasValue ? (object)parentId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@icon", isFolder ? "folder" : "file");
                cmd.Parameters.AddWithValue("@filePath", string.IsNullOrEmpty(filePath) ? DBNull.Value : (object)filePath);

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
                        ParentId = reader["ParentId"] == DBNull.Value ? null : (int?)reader["ParentId"]
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

            foreach (var item in items)
            {
                AddItemPanel(item);
            }

            buttonBack.Enabled = currentFolder != null;
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
                        RenameItem(item, newName);
                        Label lbl = panel.Controls.OfType<Label>().FirstOrDefault();
                        if (lbl != null) lbl.Text = newName;
                    }
                }
            }
        }

        private void RenameItem(DesktopItem item, string newName)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE DesktopItems SET Name = @name WHERE ItemId = @id", conn);
                cmd.Parameters.AddWithValue("@name", newName);
                cmd.Parameters.AddWithValue("@id", item.ItemId);
                cmd.ExecuteNonQuery();
            }

            item.Name = newName;
        }

        private void DeleteItem(DesktopItem item)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM DesktopItems WHERE ItemId = @id OR ParentId = @id", conn);
                cmd.Parameters.AddWithValue("@id", item.ItemId);
                cmd.ExecuteNonQuery();
            }

            if (item.Parent == null)
                desktopItems.Remove(item);
            else
                item.Parent.Children.Remove(item);
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (currentFolder != null)
            {
                currentFolder = currentFolder.Parent;
                DisplayItems(currentFolder == null
                    ? desktopItems.Where(d => d.Parent == null).ToList()
                    : currentFolder.Children);
            }
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
    }
}
