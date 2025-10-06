using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class scheduling : UserControl
    {
        private int currentEventId = 0;
        public scheduling()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Load += scheduling_Load;

        }
        private void scheduling_Load(object sender, EventArgs e)
        {

            DTPEventTime.Format = DateTimePickerFormat.Custom;
            DTPEventTime.CustomFormat = "hh:mm tt";
            DTPEventTime.ShowUpDown = true;

            DTPEndTime.Format = DateTimePickerFormat.Custom;
            DTPEndTime.CustomFormat = "hh:mm tt";
            DTPEndTime.ShowUpDown = true;


            DGVEvents.DataError -= DGVEvents_DataError;
            DGVEvents.DataError += DGVEvents_DataError;

            LoadVenues();
            LoadEventsData();
            LoadApprovedBy();
            ClearEventForm();

            InitializeGarbageScheduling();
        }

        private void Tab1Events_Click(object sender, EventArgs e)
        {
            LoadEventsData();
            ClearEventForm();
        }
        private void LoadVenues()
        {
            Venuecmb.Items.Clear();
            Venuecmb.Items.AddRange(new string[] {
                "BasketBall court 1",
                "Club house 1",
                "BasketBall court 2",
                "Club house 2"
            });
        }
        private void LoadEventsData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT EventId, EventName, Venue, StartDateTime, EndDateTime, ApprovedBy 
                                 FROM Events 
                                 ORDER BY StartDateTime DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    da.Fill(dt);
                }
            }


            if (!dt.Columns.Contains("ScheduleDisplay"))
                dt.Columns.Add("ScheduleDisplay", typeof(string));

            foreach (DataRow r in dt.Rows)
            {
                DateTime start = Convert.ToDateTime(r["StartDateTime"]);
                DateTime end = Convert.ToDateTime(r["EndDateTime"]);
                r["ScheduleDisplay"] = $"Start: {start:MMM dd, yyyy hh:mm tt}\nEnd:   {end:MMM dd, yyyy hh:mm tt}";
            }

            DGVEvents.DataSource = dt;
            StyleDataGridView();
        }
        private void StyleDataGridView()
        {
            try
            {
                if (DGVEvents.Columns.Count == 0) return;


                if (DGVEvents.Columns["EventId"] != null)
                    DGVEvents.Columns["EventId"].Visible = false;

                if (DGVEvents.Columns["StartDateTime"] != null)
                    DGVEvents.Columns["StartDateTime"].Visible = false;

                if (DGVEvents.Columns["EndDateTime"] != null)
                    DGVEvents.Columns["EndDateTime"].Visible = false;


                if (DGVEvents.Columns["EventName"] != null)
                {
                    DGVEvents.Columns["EventName"].HeaderText = "Event Name";
                    DGVEvents.Columns["EventName"].Width = 200;
                    DGVEvents.Columns["EventName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }

                if (DGVEvents.Columns["Venue"] != null)
                {
                    DGVEvents.Columns["Venue"].HeaderText = "Venue";
                    DGVEvents.Columns["Venue"].Width = 180;
                    DGVEvents.Columns["Venue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }

                if (DGVEvents.Columns["ScheduleDisplay"] != null)
                {
                    DGVEvents.Columns["ScheduleDisplay"].HeaderText = "Schedule";
                    DGVEvents.Columns["ScheduleDisplay"].Width = 400;
                    DGVEvents.Columns["ScheduleDisplay"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    DGVEvents.Columns["ScheduleDisplay"].DefaultCellStyle.WrapMode = DataGridViewTriState.True; 
                }

                if (DGVEvents.Columns["ApprovedBy"] != null)
                {
                    DGVEvents.Columns["ApprovedBy"].HeaderText = "Approved By";
                    DGVEvents.Columns["ApprovedBy"].Width = 150;
                    DGVEvents.Columns["ApprovedBy"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }


                DGVEvents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                DGVEvents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                DGVEvents.ReadOnly = true;
                DGVEvents.AllowUserToAddRows = false;
                DGVEvents.AllowUserToDeleteRows = false;
                DGVEvents.AllowUserToResizeRows = false;
                DGVEvents.AllowUserToResizeColumns = true;
                DGVEvents.MultiSelect = false;


                DGVEvents.ScrollBars = ScrollBars.Both;
                DGVEvents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                DGVEvents.RowTemplate.Height = 50;

                DGVEvents.EnableHeadersVisualStyles = false;
                DGVEvents.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 130, 180);
                DGVEvents.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                DGVEvents.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                DGVEvents.ColumnHeadersHeight = 35;
                DGVEvents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

                DGVEvents.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                DGVEvents.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 149, 237);
                DGVEvents.DefaultCellStyle.SelectionForeColor = Color.White;
                DGVEvents.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
                DGVEvents.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                DGVEvents.Dock = DockStyle.None;
                DGVEvents.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting up columns: {ex.Message}", "Column Setup Error",
                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void EventSave_Click(object sender, EventArgs e)
        {
            string eventName = EventNametxt.Text.Trim();
            string venue = Venuecmb.Text.Trim();
            string approvedBy = cmbApprovedBy.Text.Trim();

            if (string.IsNullOrWhiteSpace(eventName) || string.IsNullOrWhiteSpace(venue))
            {
                MessageBox.Show("Please fill in Event Name and select a Venue.",
                                 "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(approvedBy))
            {
                MessageBox.Show("Please select who approved this event.",
                                 "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime start = DTPEventDate.Value.Date + DTPEventTime.Value.TimeOfDay;
            DateTime end = DTPEndDate.Value.Date + DTPEndTime.Value.TimeOfDay;

            if (end <= start)
            {
                MessageBox.Show("End date/time must be later than start date/time.",
                                 "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string conflictMessage = CheckVenueConflict(currentEventId, venue, start, end);
            if (!string.IsNullOrEmpty(conflictMessage))
            {
                MessageBox.Show(conflictMessage, "Venue Not Available",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        if (currentEventId == 0)
                        {
                            cmd.CommandText = @"
                                INSERT INTO Events (EventName, Venue, StartDateTime, EndDateTime, EventDate, EventTime, ApprovedBy)
                                VALUES (@name, @venue, @start, @end, @evDate, @evTime, @approvedBy)";
                        }
                        else
                        {
                            cmd.CommandText = @"
                                UPDATE Events SET
                                    EventName = @name,
                                    Venue = @venue,
                                    StartDateTime = @start,
                                    EndDateTime = @end,
                                    EventDate = @evDate,
                                    EventTime = @evTime,
                                    ApprovedBy = @approvedBy
                                WHERE EventId = @id";
                            cmd.Parameters.AddWithValue("@id", currentEventId);
                        }

                        cmd.Parameters.AddWithValue("@name", eventName);
                        cmd.Parameters.AddWithValue("@venue", venue);
                        cmd.Parameters.AddWithValue("@start", start);
                        cmd.Parameters.AddWithValue("@end", end);
                        cmd.Parameters.AddWithValue("@evDate", start.Date);
                        cmd.Parameters.AddWithValue("@evTime", start.TimeOfDay);
                        cmd.Parameters.AddWithValue("@approvedBy", approvedBy);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(currentEventId == 0 ? "Event created successfully!" : "Event updated successfully!",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearEventForm();
                LoadEventsData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving event: {ex.Message}", "Database Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private string CheckVenueConflict(int eventId, string venue, DateTime start, DateTime end)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT EventName, StartDateTime, EndDateTime 
                                 FROM Events
                                 WHERE Venue = @venue
                                   AND EventId != @id
                                   AND (
                                       (StartDateTime < @end AND EndDateTime > @start)
                                   )";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@venue", venue);
                    cmd.Parameters.AddWithValue("@id", eventId);
                    cmd.Parameters.AddWithValue("@start", start);
                    cmd.Parameters.AddWithValue("@end", end);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string conflictEventName = reader["EventName"].ToString();
                            DateTime conflictStart = Convert.ToDateTime(reader["StartDateTime"]);
                            DateTime conflictEnd = Convert.ToDateTime(reader["EndDateTime"]);

                            return $"The venue is not available at that time.\n\n" +
                                   $"Conflict with: {conflictEventName}\n" +
                                   $"From: {conflictStart:MMM dd, yyyy hh:mm tt}\n" +
                                   $"To: {conflictEnd:MMM dd, yyyy hh:mm tt}";
                        }
                    }
                }
            }
            return string.Empty;
        }
        private void EditEvent_Click(object sender, EventArgs e)
        {
            if (DGVEvents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an event to edit.",
                                "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow row = DGVEvents.SelectedRows[0];
            currentEventId = Convert.ToInt32(row.Cells["EventId"].Value);

            EventNametxt.Text = row.Cells["EventName"].Value.ToString();
            Venuecmb.Text = row.Cells["Venue"].Value.ToString();
            if (row.Cells["ApprovedBy"].Value != null && row.Cells["ApprovedBy"].Value != DBNull.Value)
            {
                cmbApprovedBy.Text = row.Cells["ApprovedBy"].Value.ToString();
            }
            DateTime start = Convert.ToDateTime(row.Cells["StartDateTime"].Value);
            DateTime end = Convert.ToDateTime(row.Cells["EndDateTime"].Value);

            DTPEventDate.Value = start.Date;
            DTPEventTime.Value = start;
            DTPEndDate.Value = end.Date;
            DTPEndTime.Value = end;

            EventSave.Text = "Update";
        }

        private void DeleteEventbtn_Click(object sender, EventArgs e)
        {

            if (DGVEvents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an event to delete.",
                                "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow row = DGVEvents.SelectedRows[0];
            int eventId = Convert.ToInt32(row.Cells["EventId"].Value);
            string eventName = row.Cells["EventName"].Value.ToString();

            DialogResult dr = MessageBox.Show($"Are you sure you want to delete '{eventName}'?",
                                              "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Events WHERE EventId = @id", conn))
                        {
                            cmd.Parameters.AddWithValue("@id", eventId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Event deleted successfully.", "Deleted",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ClearEventForm();
                    LoadEventsData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting event: {ex.Message}", "Database Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void ClearEventForm()
        {
            currentEventId = 0;
            EventNametxt.Clear();
            Venuecmb.SelectedIndex = -1;
            cmbApprovedBy.SelectedIndex = -1;

            DTPEventDate.Value = DateTime.Now.Date;
            DTPEventTime.Value = DateTime.Now;
            DTPEndDate.Value = DateTime.Now.Date;
            DTPEndTime.Value = DateTime.Now.AddHours(1);

            EventSave.Text = "Save";
        }
        private void Venuecmb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(Venuecmb.Text) &&
                !Venuecmb.Items.Contains(Venuecmb.Text))
            {
                Venuecmb.Items.Add(Venuecmb.Text);
            }
        }

        private void DGVEvents_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void DGVEvents_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Debug.WriteLine($"DataGridView DataError at Row:{e.RowIndex}, Col:{e.ColumnIndex} - {e.Exception?.Message}");
            e.ThrowException = false;
        }
        private void LoadApprovedBy()
        {
            cmbApprovedBy.Items.Clear();

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT DISTINCT Lastname 
                             FROM Users 
                             WHERE Lastname != 'account' 
                             ORDER BY Lastname";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["Lastname"] != DBNull.Value)
                                {
                                    cmbApprovedBy.Items.Add(reader["Lastname"].ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Database Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }

        // GARBAGE SCHEDD Part

        private int currentScheduleID = 0;
        private void InitializeGarbageScheduling()
        {

            LoadCollectionDays();
            LoadGarbageStatus();
            LoadGarbageSchedules();

            DTPGarbageSchedTime.Format = DateTimePickerFormat.Custom;
            DTPGarbageSchedTime.CustomFormat = "hh:mm tt";
            DTPGarbageSchedTime.ShowUpDown = true;
            DTPGarbageSchedTime.Value = DateTime.Now;

            DGVGarbageSched.CellClick -= DGVGarbageSched_CellClick;
            DGVGarbageSched.CellClick += DGVGarbageSched_CellClick;
        }

        private void LoadCollectionDays()
        {
            cmbDay.Items.Clear();
            cmbDay.Items.AddRange(new string[]
            {
        "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"
            });
        }
        private void LoadGarbageStatus()
        {
            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new string[] { "Active", "Inactive" });
            cmbStatus.SelectedIndex = 0;
        }

        private void LoadGarbageSchedules()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    ScheduleID,
                    TruckCompany,
                    CollectionDay,
                    FORMAT(CAST(CollectionTime AS datetime), 'hh:mm tt') AS CollectionTime,
                    CASE WHEN Status = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
                    CreatedDate
                FROM GarbageCollectionSchedules
                ORDER BY 
                    CASE CollectionDay
                        WHEN 'Monday' THEN 1
                        WHEN 'Tuesday' THEN 2
                        WHEN 'Wednesday' THEN 3
                        WHEN 'Thursday' THEN 4
                        WHEN 'Friday' THEN 5
                        WHEN 'Saturday' THEN 6
                        WHEN 'Sunday' THEN 7
                        ELSE 8
                    END,
                    Status DESC,
                    CreatedDate DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    DGVGarbageSched.DataSource = dt;

                    StyleGarbageDataGridView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading garbage schedules: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void StyleGarbageDataGridView()
        {
            var dgv = DGVGarbageSched;
            if (dgv.Columns.Count == 0) return;

            if (dgv.Columns["ScheduleID"] != null) dgv.Columns["ScheduleID"].Visible = false;
            if (dgv.Columns["TruckCompany"] != null) dgv.Columns["TruckCompany"].HeaderText = "Truck Company";
            if (dgv.Columns["CollectionDay"] != null) dgv.Columns["CollectionDay"].HeaderText = "Collection Day";
            if (dgv.Columns["CollectionTime"] != null) dgv.Columns["CollectionTime"].HeaderText = "Collection Time";
            if (dgv.Columns["Status"] != null) dgv.Columns["Status"].HeaderText = "Status";
            if (dgv.Columns["CreatedDate"] != null)
            {
                dgv.Columns["CreatedDate"].HeaderText = "Date Created";
                dgv.Columns["CreatedDate"].DefaultCellStyle.Format = "MM/dd/yyyy hh:mm tt";
            }

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            dgv.ScrollBars = ScrollBars.Both;  

            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.MultiSelect = false;

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 130, 180);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 149, 237);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.ColumnHeadersHeight = 35;
            dgv.RowTemplate.Height = 50;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;


            dgv.Dock = DockStyle.None;
            dgv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        }
        private void DGVGarbageSched_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = DGVGarbageSched.Rows[e.RowIndex];
            if (row.Cells["ScheduleID"].Value == null) return;

            currentScheduleID = Convert.ToInt32(row.Cells["ScheduleID"].Value);
            txtTruckCompany.Text = row.Cells["TruckCompany"].Value.ToString();
            cmbDay.SelectedItem = row.Cells["CollectionDay"].Value.ToString();
            cmbStatus.SelectedItem = row.Cells["Status"].Value.ToString();

            if (DateTime.TryParse(row.Cells["CollectionTime"].Value?.ToString(), out DateTime parsedTime))
                DTPGarbageSchedTime.Value = DateTime.Today.Add(parsedTime.TimeOfDay);
        }

        private void Tab2GarbageSched_Click(object sender, EventArgs e)
        {
              LoadGarbageSchedules();
            ClearGarbageForm();
        }

        private void DGVGarbageSched_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (currentScheduleID == 0)
            {
                MessageBox.Show("Please select a schedule to update.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateGarbageInputs()) return;

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                UPDATE GarbageCollectionSchedules
                SET 
                    TruckCompany = @TruckCompany,
                    CollectionDay = @CollectionDay,
                    CollectionTime = @CollectionTime,
                    Status = @Status,
                    LastModified = GETDATE()
                WHERE ScheduleID = @ScheduleID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = currentScheduleID;
                        cmd.Parameters.Add("@TruckCompany", SqlDbType.NVarChar, 100).Value = txtTruckCompany.Text.Trim();
                        cmd.Parameters.Add("@CollectionDay", SqlDbType.NVarChar, 50).Value = cmbDay.SelectedItem?.ToString() ?? "";
                        cmd.Parameters.Add("@CollectionTime", SqlDbType.Time).Value = DTPGarbageSchedTime.Value.TimeOfDay;
                        cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = (cmbStatus.SelectedItem?.ToString() == "Active");

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                            MessageBox.Show("Garbage schedule updated successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show("No record updated. Please reselect a row.", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                LoadGarbageSchedules();
                ClearGarbageForm();
                currentScheduleID = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating garbage schedule: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool ValidateGarbageInputs()
        {
            if (string.IsNullOrWhiteSpace(txtTruckCompany.Text))
            {
                MessageBox.Show("Please enter Truck Company name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTruckCompany.Focus();
                return false;
            }

            if (cmbDay.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Collection Day.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbDay.Focus();
                return false;
            }

            if (cmbStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Status.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbStatus.Focus();
                return false;
            }

            return true;
        }
        private void ClearGarbageForm()
        {
            txtTruckCompany.Clear();
            cmbDay.SelectedIndex = -1;
            cmbStatus.SelectedIndex = 0;
            DTPGarbageSchedTime.Value = DateTime.Now;
            currentScheduleID = 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateGarbageInputs()) return;

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query;

                    if (currentScheduleID == 0)
                    {
                        query = @"
                    INSERT INTO GarbageCollectionSchedules 
                        (TruckCompany, CollectionDay, CollectionTime, Status, CreatedDate)
                    VALUES (@TruckCompany, @CollectionDay, @CollectionTime, @Status, GETDATE())";
                    }
                    else
                    {
                        query = @"
                    UPDATE GarbageCollectionSchedules
                    SET 
                        TruckCompany = @TruckCompany,
                        CollectionDay = @CollectionDay,
                        CollectionTime = @CollectionTime,
                        Status = @Status,
                        LastModified = GETDATE()
                    WHERE ScheduleID = @ScheduleID";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TruckCompany", txtTruckCompany.Text.Trim());
                        cmd.Parameters.AddWithValue("@CollectionDay", cmbDay.SelectedItem?.ToString() ?? "");
                        cmd.Parameters.AddWithValue("@CollectionTime", DTPGarbageSchedTime.Value.TimeOfDay);
                        cmd.Parameters.AddWithValue("@Status", (cmbStatus.SelectedItem?.ToString() == "Active"));

                        if (currentScheduleID != 0)
                            cmd.Parameters.AddWithValue("@ScheduleID", currentScheduleID);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    currentScheduleID == 0
                        ? "Garbage schedule added successfully!"
                        : "Garbage schedule updated successfully!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearGarbageForm();
                LoadGarbageSchedules();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving schedule: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

