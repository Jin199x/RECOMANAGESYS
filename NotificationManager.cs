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
                        string key = $"Announcement_{id}";
                        DateTime datePosted = Convert.ToDateTime(reader["DatePosted"]);

                        if (isImportant)
                        {
                            notifications.Add(new Notification($"⚠️ {title} (Important Announcement)", "Announcement", id, datePosted, !readNotifications.Contains(key)));
                        }
                        else
                        {
                            if (datePosted.Date == DateTime.Today)
                            {
                                normalAnnouncementsPostedToday++;
                            }
                        }
                    }
                    if (normalAnnouncementsPostedToday > 0)
                    {
                        string key = $"Announcement_Summary_{DateTime.Today:yyyy-MM-dd}";
                        string message = $"{normalAnnouncementsPostedToday} new announcement(s) posted today";
                        notifications.Add(new Notification(message, "Announcement", null, DateTime.Today, !readNotifications.Contains(key)));
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
                        string key = $"Event_{id}";
                        string text = start.Date == DateTime.Today ? $"Event today: {title}" : $"Event tomorrow: {title}";
                        notifications.Add(new Notification(text, "Event", id, start, !readNotifications.Contains(key)));
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
                            string key = $"Garbage_Today_{now:yyyy-MM-dd}";
                            // --- GEMINI: For "today" items, we use the current time to make them appear at the very top ---
                            notifications.Add(new Notification("Garbage collection today!", "Garbage", null, now, !readNotifications.Contains(key)));
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

            notif.IsUnread = false;
            string key = notif.type + "_" + (notif.id.HasValue ? notif.id.Value.ToString() : notif.message.GetHashCode().ToString());
            readNotifications.Add(key);
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
