// -----------------------------------------------------------------------
// <copyright file="Controller.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Security;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Controller
    {        
        /// <summary>
        /// The filename
        /// </summary>
        private string filename = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller" /> class.
        /// </summary>
        public Controller()
        {            
            Settings = new Settings();
            Settings.Read();
            Timesheet = new Timesheet(Settings.StaffNumber, Settings.StaffID);            
            try
            {
                Timesheet.New();
            }
            catch (SqlException)
            {                 
                DeleteAllUnsavedAndTemporaryTimesheets();
                Timesheet.New();
            }
        }

        /// <summary>
        /// Gets the timesheet.
        /// </summary>
        /// <value>
        /// The timesheet.
        /// </value>
        public Timesheet Timesheet
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public Settings Settings
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the submitter.
        /// </summary>
        /// <value>
        /// The submitter.
        /// </value>
        public ISubmitter Submitter
        {
            get
            {
                if (Settings.SubmitViaNotes)
                {
                    return new SubmitViaNotes();
                }
                else
                {
                    return new SubmitViaOtherEmail();
                }
            }
        }

        /// <summary>
        /// Runs the T check.
        /// </summary>
        public void RunTCheck()
        {
            TCheck.Run(Settings, Timesheet.Month.ToString("00"), (Timesheet.Year - 2000).ToString());
        }

        /// <summary>
        /// Submits the T check.
        /// </summary>
        /// <param name="submitable">The submitable.</param>
        /// <param name="password">The password.</param>
        /// <exception cref="UnableToSubmitEmailException">Occurs if unable to submit an email</exception>        
        public void Submit(ASubmittable submitable, SecureString password = null)
        {
            string sMonth = Timesheet.Month.ToString("00");

            // Don't want to submit XML file, want to submit build
            if (!Util.IsXmlFilename(filename))
            {
                Submitter.Send(Settings, sMonth, Timesheet.Year.ToString(), filename, submitable.EmailAddress, password);                
            }
            else
            {
                throw new UnableToSubmitEmailException();
            }
        }

        /// <summary>
        /// Saves as.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void SaveAs(string filename)
        {
            this.filename = filename;
            Save();
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <exception cref="FilenameUnsetException">Occurs when the filename is not set</exception>
        public void Save()
        {
            if (filename.Equals(string.Empty))
            {
                throw new FilenameUnsetException();
            }
            else
            {                
                if (!Util.IsXmlFilename(filename))
                {
                    filename = filename + "X";
                }

                Serialization.Serialize(Timesheet, filename);
            }
        }

        /// <summary>
        /// News the timesheet.
        /// </summary>
        public void NewTimesheet()
        {
            Timesheet.New();
        }

        /// <summary>
        /// Opens the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Open(string filename)
        {
            this.filename = filename;            
            if (Util.IsXmlFilename(filename))
            {
                Timesheet.ReadXML(filename);
            }
            else
            {
                Timesheet.ReadBuild(filename);
            }            

            Timesheet.UnsavedChanges = false;
        }

        /// <summary>
        /// Clears the database.
        /// </summary>
        public void ClearDatabase()
        {
            Database.Clear();
        }

        /// <summary>
        /// Deletes all unsaved timesheets.
        /// </summary>
        public void DeleteAllUnsavedAndTemporaryTimesheets()
        {
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand findUnsavedTimesheetID = new SqlCommand("SELECT ID FROM Timesheet WHERE Month=0 AND Year=0", con))
                {
                    DeleteTimesheet((int)findUnsavedTimesheetID.ExecuteScalar());
                }

                List<int> staffNumbers = new List<int>();                        
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

                foreach (int staffNumber in staffNumbers)
                {
                    List<int> timesheetIDs = new List<int>();
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
                    
                    foreach (int timesheetID in timesheetIDs)
                    {
                        using (SqlCommand deleteEntries = new SqlCommand("DELETE FROM Entry WHERE TimesheetID=@TimesheetID", con))
                        {
                            deleteEntries.Parameters.Add(new SqlParameter("TimesheetID", timesheetID));
                            deleteEntries.ExecuteNonQuery();
                        }

                        using (SqlCommand deleteTimesheet = new SqlCommand("DELETE FROM Timesheet WHERE ID=@TimesheetID", con))
                        {
                            deleteTimesheet.Parameters.Add(new SqlParameter("TimesheetID", timesheetID));
                            deleteTimesheet.ExecuteNonQuery();
                        }                                        

                        using (SqlCommand deleteStaffMember = new SqlCommand("DELETE FROM Staff WHERE StaffNumber=@StaffNumber", con))
                        {
                            deleteStaffMember.Parameters.Add(new SqlParameter("StaffNumber", staffNumber));
                            deleteStaffMember.ExecuteNonQuery();
                        }                        
                    }                    
                }
            }           
        }

        /// <summary>
        /// Deletes the timesheet.
        /// </summary>
        /// <param name="timesheetID">The timesheet ID.</param>
        public void DeleteTimesheet(int timesheetID)
        {
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand deleteTimesheet = new SqlCommand("DELETE FROM Entry WHERE TimesheetID=@ID", con))
                {
                    deleteTimesheet.Parameters.Add(new SqlParameter("ID", timesheetID));
                    deleteTimesheet.ExecuteNonQuery();
                }

                using (SqlCommand deleteTimesheet = new SqlCommand("DELETE FROM Timesheet WHERE ID=@ID", con))
                {
                    deleteTimesheet.Parameters.Add(new SqlParameter("ID", timesheetID));
                    deleteTimesheet.ExecuteNonQuery();
                }
            }            
        }

        /// <summary>
        /// Opens the timesheet under temporary staff ID.
        /// </summary>
        /// <param name="staffNumber">The staff number.</param>
        /// <param name="filename">The filename.</param>
        public void OpenTimesheetUnderTemporaryStaffID(int staffNumber, string filename)
        {
            Settings.StaffID = "TEMP";
            Settings.StaffNumber = staffNumber;
            Timesheet.UpdateStaffDetails(staffNumber, "TEMP");
            try
            {
                Open(filename);
            }
            catch (SqlException)
            {
                OpenTimesheetUnderTemporaryStaffID(staffNumber - 1, filename);
            }
        }
    }
}
