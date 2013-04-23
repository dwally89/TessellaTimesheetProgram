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
            return new SqlConnection(TimesheetProgramLogic.Properties.Settings.Default.TimesheetDatabaseConnectionString);
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

        /// <summary>
        /// Determines whether [contains staff number] [the specified staff number].
        /// </summary>
        /// <param name="staffNumber">The staff number.</param>
        /// <returns>
        ///   <c>true</c> if [contains staff number] [the specified staff number]; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsStaffNumber(int staffNumber)
        {
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand isStaffNumberInDatabase = new SqlCommand("SELECT StaffNumber FROM Staff WHERE StaffNumber=@StaffNumber", con))
                {
                    isStaffNumberInDatabase.Parameters.Add(new SqlParameter("StaffNumber", staffNumber));
                    var x = isStaffNumberInDatabase.ExecuteScalar();
                    if (x == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// Adds the staff member.
        /// </summary>
        /// <param name="staffNumber">The staff number.</param>
        /// <param name="staffID">The staff ID.</param>
        public static void AddStaffMember(int staffNumber, string staffID)
        {
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand addStaffMember = new SqlCommand("INSERT INTO Staff VALUES(@StaffNumber, @StaffID)", con))
                {
                    addStaffMember.Parameters.Add(new SqlParameter("StaffNumber", staffNumber));
                    addStaffMember.Parameters.Add(new SqlParameter("StaffID", staffID));
                    addStaffMember.ExecuteNonQuery();
                }
            }
        }
    }
}
