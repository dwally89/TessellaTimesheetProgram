/*namespace TimesheetProgramLogic
{
    using System.Collections.Generic;
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
                using (SqlCommand deleteAllTimesheets = new SqlCommand("DELETE FROM Timesheet", con))
                {
                    deleteAllTimesheets.ExecuteNonQuery();
                }

                using (SqlCommand deleteAllStaff = new SqlCommand("DELETE FROM Staff", con))
                {
                    deleteAllStaff.ExecuteNonQuery();
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

        /// <summary>
        /// Deletes the staff member.
        /// </summary>
        /// <param name="staffNumber">The staff number.</param>
        public static void DeleteStaffMember(int staffNumber)
        {
            using (SqlConnection con = GetConnection())
            {
                con.Open();
                using (SqlCommand deleteStaffMember = new SqlCommand("DELETE FROM Staff WHERE StaffNumber=@StaffNumber", con))
                {
                    deleteStaffMember.Parameters.Add(new SqlParameter("StaffNumber", staffNumber));
                    deleteStaffMember.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets the temporary timesheet I ds for staff number.
        /// </summary>
        /// <param name="staffNumber">The staff number.</param>
        /// <returns>
        /// The timesheet IDs of the timesheets for temporary staff
        /// </returns>
        public static List<int> GetTemporaryTimesheetIDsForStaffNumber(int staffNumber)
        {
            List<int> timesheetIDs = new List<int>();
            using (SqlConnection con = GetConnection())
            {
                con.Open();
                using (SqlCommand getTemporaryTimesheetIDs = new SqlCommand("SELECT ID From Timesheet WHERE StaffNumber=@StaffNumber", con))
                {
                    getTemporaryTimesheetIDs.Parameters.Add(new SqlParameter("StaffNumber", staffNumber));
                    using (SqlDataReader timesheetIDReader = getTemporaryTimesheetIDs.ExecuteReader())
                    {
                        while (timesheetIDReader.Read())
                        {
                            timesheetIDs.Add(timesheetIDReader.GetInt32(0));
                        }
                    }
                }
            }

            return timesheetIDs;
        }

        /// <summary>
        /// Gets the temporary staff numbers.
        /// </summary>
        /// <returns>
        /// The staff numbers of the temporary staff
        /// </returns>
        public static List<int> GetTemporaryStaffNumbers()
        {
            List<int> staffNumbers = new List<int>();
            using (SqlConnection con = GetConnection())
            {
                con.Open();
                using (SqlCommand getTemporaryStaffNumbers = new SqlCommand("SELECT StaffNumber FROM Staff WHERE StaffID='TEMP'", con))
                {
                    using (SqlDataReader staffNumberReader = getTemporaryStaffNumbers.ExecuteReader())
                    {
                        while (staffNumberReader.Read())
                        {
                            staffNumbers.Add(staffNumberReader.GetInt32(0));
                        }
                    }
                }
            }

            return staffNumbers;
        }

        /// <summary>
        /// Deletes the unsaved timesheets.
        /// </summary>
        public static void DeleteUnsavedTimesheets()
        {
            using (SqlConnection con = GetConnection())
            {
                con.Open();
                using (SqlCommand findUnsavedTimesheetID = new SqlCommand("SELECT ID FROM Timesheet WHERE Month=0 AND Year=0", con))
                {
                    DeleteTimesheet((int)findUnsavedTimesheetID.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Numbers the of timesheets.
        /// </summary>
        /// <returns>The number of timesheets in the database</returns>
        public static int NumberOfTimesheets()
        {
            using (SqlConnection con = GetConnection())
            {
                con.Open();
                using (SqlCommand getNumberOfTimesheets = new SqlCommand("SELECT COUNT(ID) FROM Timesheet", con))
                {
                    return (int)getNumberOfTimesheets.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Deletes the timesheet.
        /// </summary>
        /// <param name="timesheetID">The timesheet ID.</param>
        public static void DeleteTimesheet(int timesheetID)
        {
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand deleteTimesheet = new SqlCommand("DELETE FROM Timesheet WHERE ID=@ID", con))
                {
                    deleteTimesheet.Parameters.Add(new SqlParameter("ID", timesheetID));
                    deleteTimesheet.ExecuteNonQuery();
                }
            }
        }
    }
}*/