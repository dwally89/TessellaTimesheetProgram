namespace TimesheetProgramLogic
{
    using System.Data.SqlClient;

    /// <summary>
    /// Database stuff
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns>FGDFG DFG</returns>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(TimesheetProgramLogic.Properties.Settings.Default.ConnectionString);
        }

        /// <summary>
        /// Clears the database.
        /// </summary>
        public static void Clear()
        {
            using (SqlConnection con = GetConnection())
            {
                con.Open();
                using (SqlCommand deleteAllEntries = new SqlCommand("DELETE FROM Entry", con))
                {
                    deleteAllEntries.ExecuteNonQuery();
                }

                using (SqlCommand deleteAllTimesheets = new SqlCommand("DELETE FROM Timesheet", con))
                {
                    deleteAllTimesheets.ExecuteNonQuery();
                }
            }
        }
    }
}
