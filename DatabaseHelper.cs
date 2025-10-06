using System;
using System.Data;
using System.Data.SqlClient;

public static class DatabaseHelper
{
    // ✅ Put your connection string here only once
    private static string connectionString =
               "Data Source=LAPTOP-FT905FTC\\SQLEXPRESS;Initial Catalog=RecordManagement;Integrated Security=True;";

    public static string ConnectionString { get; internal set; }

    public static SqlConnection GetConnection()
    {
        return new SqlConnection(connectionString);
    }
    public static DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

    }

    public static int ExecuteNonQuery(string query, params SqlParameter[] parameters)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteNonQuery();
            }
        }
    }
}
