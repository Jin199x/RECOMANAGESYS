using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace RECOMANAGESYS
{
    public static class NotificationManager
    {
        public static event Action NotificationsUpdated;

        private static List<Notification> notifications = new List<Notification>();
        public static IReadOnlyList<Notification> Notifications => notifications.AsReadOnly();

        private static string saveFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RECOMANAGESYS",
            "notif_status.json"
        );

        private static HashSet<string> readNotifications = new HashSet<string>();
        public static void Reload()
        {
            LoadReadStatus();
            notifications.Clear();
            DateTime now = DateTime.Now;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // Announcements 
                string queryAnn = @"
            SELECT Id, Title, IsImportant, DatePosted
            FROM Announcements
            WHERE ExpirationDate IS NULL OR ExpirationDate >= CAST(GETDATE() AS DATE)";
                using (SqlCommand cmd = new SqlCommand(queryAnn, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int normalAnnouncementsPostedToday = 0;
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        bool isImportant = reader["IsImportant"] != DBNull.Value && Convert.ToBoolean(reader["IsImportant"]);
                        string title = reader["Title"].ToString();
                        DateTime datePosted = Convert.ToDateTime(reader["DatePosted"]);

                        if (isImportant)
                        {
                            var newNotif = new Notification($"⚠️ {title} (Important)", "Announcement", id, datePosted);
                            newNotif.IsUnread = !readNotifications.Contains(newNotif.Key);
                            notifications.Add(newNotif);
                        }
                        else
                        {
                            var tempKey = $"Announcement_{id}";
                            if (datePosted.Date == DateTime.Today && !readNotifications.Contains(tempKey))
                            {
                                normalAnnouncementsPostedToday++;
                            }
                        }
                    }
                    if (normalAnnouncementsPostedToday > 0)
                    {
                        string message = $"{normalAnnouncementsPostedToday} new announcement(s) posted today";
                        var newNotif = new Notification(message, "Announcement", null, DateTime.Today);
                        newNotif.IsUnread = !readNotifications.Contains(newNotif.Key);
                        notifications.Add(newNotif);
                    }
                }

                // Events 
                string queryEvents = @"
            SELECT EventId, EventName, StartDateTime
            FROM Events
            WHERE CAST(StartDateTime AS DATE) IN (CAST(GETDATE() AS DATE), DATEADD(DAY,1,CAST(GETDATE() AS DATE)))";
                using (SqlCommand cmd = new SqlCommand(queryEvents, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["EventId"]);
                        string title = reader["EventName"].ToString();
                        DateTime start = Convert.ToDateTime(reader["StartDateTime"]);
                        string text = start.Date == DateTime.Today ? $"Event today: {title}" : $"Event tomorrow: {title}";
                        var newNotif = new Notification(text, "Event", id, start);
                        newNotif.IsUnread = !readNotifications.Contains(newNotif.Key);
                        notifications.Add(newNotif);
                    }
                }

                // Garbage 
                string queryGarbage = @"SELECT CollectionDay FROM GarbageCollectionSchedules WHERE Status = 1";
                using (SqlCommand cmd = new SqlCommand(queryGarbage, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string day = reader["CollectionDay"].ToString();
                        DayOfWeek scheduleDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), day);
                        if (scheduleDay == now.DayOfWeek)
                        {
                            var newNotif = new Notification("Garbage collection today!", "Garbage", null, now);
                            newNotif.IsUnread = !readNotifications.Contains(newNotif.Key);
                            notifications.Add(newNotif);
                        }
                    }
                }
            }

            notifications = notifications.OrderByDescending(n => n.SortDate).ToList();
            NotificationsUpdated?.Invoke();
        }
        
        public static void MarkAsRead(Notification notif)
        {
            if (!notif.IsUnread) return;
            if (notif.id == null && notif.type == "Announcement")
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT Id FROM Announcements 
                             WHERE IsImportant = 0 AND CAST(DatePosted AS DATE) = CAST(GETDATE() AS DATE)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["Id"]);
                            readNotifications.Add($"Announcement_{id}");
                        }
                    }
                }
                readNotifications.Add(notif.Key);
            }
            else
            {
                readNotifications.Add(notif.Key);
            }

            notif.IsUnread = false; 
            SaveReadStatus();
            NotificationsUpdated?.Invoke();
        }
        

        private static void LoadReadStatus()
        {
            try
            {
                if (File.Exists(saveFile))
                {
                    var json = File.ReadAllText(saveFile);
                    readNotifications = JsonConvert.DeserializeObject<HashSet<string>>(json) ?? new HashSet<string>();
                }
                else
                {
                    readNotifications = new HashSet<string>();
                }
            }
            catch
            {
                readNotifications = new HashSet<string>();
            }
        }

        private static void SaveReadStatus()
        {
            try
            {
                string dir = Path.GetDirectoryName(saveFile);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                var json = JsonConvert.SerializeObject(readNotifications, Formatting.Indented);
                File.WriteAllText(saveFile, json);
            }
            catch { }
        }
    }

    public class Notification
    {
        public string message;
        public string type;
        public int? id;
        public bool IsUnread;
        public DateTime SortDate;

        public string Key
        {
            get
            {
                if (id.HasValue) 
                {
                    return $"{type}_{id.Value}";
                }
                else 
                {
                    return $"{type}_Summary_{SortDate:yyyy-MM-dd}";
                }
            }
        }

        public Notification(string message, string type, int? id, DateTime sortDate, bool isUnread = true)
        {
            this.message = message;
            this.type = type;
            this.id = id;
            this.SortDate = sortDate;
            this.IsUnread = isUnread;
        }
    }
}
