using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RECOMANAGESYS
{
        public static class NotificationManager
        {
            public static event Action NotificationsUpdated;

            private static List<(string message, string type)> notifications = new List<(string, string)>();
            public static IReadOnlyList<(string message, string type)> Notifications => notifications.AsReadOnly();

            public static void Reload()
            {
                notifications.Clear();
                int count = 0;
                DateTime now = DateTime.Now;

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Announcements
                    string queryAnn = @"
                SELECT Title, IsImportant
                FROM Announcements
                WHERE ExpirationDate IS NULL OR ExpirationDate >= CAST(GETDATE() AS DATE)";
                    using (SqlCommand cmd = new SqlCommand(queryAnn, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int normalCount = 0;
                        while (reader.Read())
                        {
                            bool isImportant = reader["IsImportant"] != DBNull.Value && Convert.ToBoolean(reader["IsImportant"]);
                            string title = reader["Title"].ToString();

                            if (isImportant)
                            {
                                notifications.Add(($"⚠️ {title} (Important Announcement)", "Announcement"));
                                count++;
                            }
                            else normalCount++;
                        }
                        if (normalCount > 0)
                        {
                            notifications.Add(($"{normalCount} announcement(s) posted today", "Announcement"));
                            count++;
                        }
                    }

                    // Events
                    string queryEvents = @"
                SELECT EventName, StartDateTime
                FROM Events
                WHERE CAST(StartDateTime AS DATE) IN (CAST(GETDATE() AS DATE), DATEADD(DAY,1,CAST(GETDATE() AS DATE)))";
                    using (SqlCommand cmd = new SqlCommand(queryEvents, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string title = reader["EventName"].ToString();
                            DateTime start = Convert.ToDateTime(reader["StartDateTime"]);
                            if (start.Date == DateTime.Today)
                            {
                                notifications.Add(($"📅 Event today: {title}", "Event"));
                                count++;
                            }
                            else notifications.Add(($"📅 Event tomorrow: {title}", "Event"));
                        }
                    }

                    // Garbage schedules
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
                                notifications.Add(("🗑️ Garbage collection today!", "Garbage"));
                                count++;
                            }
                        }
                    }
                }

                NotificationsUpdated?.Invoke();
            }
        }

    }
