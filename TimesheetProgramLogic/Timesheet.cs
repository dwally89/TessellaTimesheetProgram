// -----------------------------------------------------------------------
// <copyright file="Timesheet.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.SqlClient;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Timesheet : ASubmittable
    {
        /// <summary>
        /// The TIMESHEE t_ EMAI l_ ADDRESS
        /// </summary>
        public const string TIMESHEET_EMAIL_ADDRESS = "timesheet@tessella.com";

        /// <summary>
        /// Initializes a new instance of the <see cref="Timesheet" /> class.
        /// </summary>
        /// <param name="staffNumber">The staff number.</param>
        /// <param name="staffID">The staff ID.</param>
        public Timesheet(int staffNumber, string staffID)
        {
            UnsavedChanges = false;
            this.StaffNumber = staffNumber;
            this.StaffID = staffID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Timesheet" /> class.
        /// DO NOT USE. ONLY PRESENT FOR SERIALIZATION.
        /// </summary>
        public Timesheet()
        {            
        }

        /// <summary>
        /// Gets the email address.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        public override string EmailAddress
        {
            get { return TIMESHEET_EMAIL_ADDRESS; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [unsaved changes].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [unsaved changes]; otherwise, <c>false</c>.
        /// </value>
        public bool UnsavedChanges
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the month.
        /// </summary>
        /// <value>
        /// The month.
        /// </value>
        public int Month
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        /// <value>
        /// The year.
        /// </value>
        public int Year
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public int ID { get; set; }

        /// <summary>
        /// Gets the staff number.
        /// </summary>
        /// <value>
        /// The staff number.
        /// </value>
        public int StaffNumber { get; private set; }

        /// <summary>
        /// Gets the staff ID.
        /// </summary>
        /// <value>
        /// The staff ID.
        /// </value>
        public string StaffID { get; private set; }

        /// <summary>
        /// Gets the entries.
        /// </summary>
        /// <value>
        /// The entries.
        /// </value>
        public List<Entry> Entries
        {
            get
            {
                return ReadTimesheetViaID(ID);
            }
        }

        /// <summary>
        /// Gets all timesheet data.
        /// </summary>
        /// <returns>DFGDFG DFGFD</returns>
        public static List<TimesheetData> GetAllTimesheetData()
        {
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand getAllTimesheetIDs = new SqlCommand("SELECT * FROM Timesheet", con))
                {
                    List<TimesheetData> timesheetData = new List<TimesheetData>();
                    using (SqlDataReader reader = getAllTimesheetIDs.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            timesheetData.Add(new TimesheetData(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3)));
                        }
                    }

                    return timesheetData;
                }
            }
        }

        /// <summary>
        /// Reads the timesheet via ID.
        /// </summary>
        /// <param name="timesheetID">The timesheet ID.</param>
        /// <returns>DFGD FGDFGFD</returns>
        public List<Entry> ReadTimesheetViaID(int timesheetID)
        {
            List<Entry> entries = new List<Entry>();
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand getAllEntries = new SqlCommand("SELECT * FROM Entry WHERE TimesheetID=@ID", con))
                {
                    getAllEntries.Parameters.Add(new SqlParameter("ID", timesheetID));
                    using (SqlDataReader reader = getAllEntries.ExecuteReader())
                    {
                        int id, projectNumber;
                        DateTime date;
                        TimeSpan startTime, finishTime;
                        string taskCode, phaseCode, billable, description;
                        bool overhead;

                        while (reader.Read())
                        {
                            id = reader.GetInt32(0);
                            date = reader.GetDateTime(1);
                            projectNumber = reader.GetInt32(2);
                            startTime = reader.GetTimeSpan(3);
                            finishTime = reader.GetTimeSpan(4);
                            taskCode = reader.GetString(5);
                            phaseCode = reader.GetString(6);
                            overhead = reader.GetBoolean(7);
                            billable = reader.GetString(8);
                            description = reader.GetString(9);
                            entries.Add(new Entry(id, date, projectNumber, startTime, finishTime, taskCode, phaseCode, overhead, billable, description, true));
                        }
                    }
                }
            }

            entries.Sort(new SortEntriesViaDateTime());
            return entries;
        }

        /// <summary>
        /// Parses the timesheet.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void ParseTimesheet(string filename)
        {                   
            List<string> fileContents = DataIO.ReadTextFile(filename);
            int projectNumber = -1;            
            int entryID = GetNextUnusedID("Entry");
            bool isFirstEntry = true;
            foreach (string line in fileContents)
            {
                if (!line.StartsWith("*"))
                {
                    if (line.StartsWith("P"))
                    {
                        projectNumber = int.Parse(line.Replace("P", string.Empty));                        
                    }
                    else if (line != "END")
                    {                        
                        Entry entry = new Entry(projectNumber, line, entryID);
                        if (isFirstEntry)
                        {
                            Month = entry.Date.Month;
                            Year = entry.Date.Year;
                            New();
                            isFirstEntry = false;
                        }

                        try
                        {
                            InsertEntry(entry, this.ID);
                        }
                        catch (SqlException)
                        {
                            PersistInsertEntry(entry);
                        }

                        entryID++;
                    }
                }
            }            
        }

        /// <summary>
        /// Gets the next unused ID.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>DFGD FGDF</returns>
        public int GetNextUnusedID(string table)
        {
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand getMaxID = new SqlCommand("SELECT MAX(ID) FROM " + table, con))
                {
                    var maxID = getMaxID.ExecuteScalar();
                    if (maxID.GetType() == typeof(System.DBNull))
                    {
                        return 0;
                    }
                    else
                    {
                        return (int)maxID + 1;
                    }
                }
            }
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="newEntry">The new entry.</param>
        /// <exception cref="TimesheetProgramLogic.EntriesNotInSameMonthException">blah blah blah</exception>
        /// <exception cref="TimesheetProgramLogic.ProjectCantBeBillableAndAccountableException">blha blha blha</exception>
        /// <exception cref="EntriesNotInSameMonthException">Occurs when all entries aren't in the same month</exception>
        /// <exception cref="ProjectCantBeBillableAndAccountableException">Occurs when the project is both billable and accountable</exception>
        public void AddEntry(Entry newEntry)
        {
            if (NumberOfEntries() == 0)
            {
                Month = newEntry.Date.Month;
                Year = newEntry.Date.Year;                
                UpdateTimesheetMonthAndYear(newEntry.Date.Month, newEntry.Date.Year);                
            }

            if (ContainsEntryWithStartTime(newEntry.StartTime, newEntry.Date))
            {
                throw new InvalidStartTimeException();
            }
                       
            if (VerifyEntryMonth(newEntry, true))
            {                   
                InsertEntry(newEntry, ID);                
                UnsavedChanges = true;
            }            
        }

        /// <summary>
        /// Loads the timesheet via ID.
        /// </summary>
        /// <param name="timesheetID">The timesheet ID.</param>
        public void LoadTimesheetViaID(int timesheetID)
        {
            ID = timesheetID;
            UnsavedChanges = false;
        }

        /// <summary>
        /// Numbers the of entries.
        /// </summary>
        /// <returns>The number of entries in the timesheet</returns>
        public int NumberOfEntries()
        {
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand getNumberOfEntries = new SqlCommand("SELECT COUNT(*) FROM Entry WHERE TimesheetID=@ID", con))
                {
                    getNumberOfEntries.Parameters.Add(new SqlParameter("ID", ID));
                    return (int)getNumberOfEntries.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Edits the entry.
        /// </summary>
        /// <param name="editedEntry">The edited entry.</param>
        /// <exception cref="System.Exception">Entry could not be edited</exception>
        public void EditEntry(Entry editedEntry)
        {            
            bool checkMonth = true;
            if (NumberOfEntries() == 1)
            {
                checkMonth = false;
            }

            if (VerifyEntryMonth(editedEntry, checkMonth))
            {                  
                if (ContainsEntryWithStartTime(editedEntry.StartTime, editedEntry.Date))
                {
                    throw new InvalidStartTimeException();
                }

                using (SqlConnection con = Database.GetConnection())
                {
                    con.Open();
                    using (SqlCommand updateTimesheet = new SqlCommand("UPDATE Entry SET Date=@Date, ProjectNumber=@ProjectNumber, StartTime=@StartTime, FinishTime=@FinishTime, TaskCode=@TaskCode, PhaseCode=@PhaseCode, Overhead=@Overhead, Billable=@Billable, Description=@Description WHERE ID=@ID AND TimesheetID=@TimesheetID", con))
                    {
                        updateTimesheet.Parameters.Add(new SqlParameter("Date", editedEntry.Date));
                        updateTimesheet.Parameters.Add(new SqlParameter("ProjectNumber", editedEntry.ProjectNumber));
                        updateTimesheet.Parameters.Add(new SqlParameter("StartTime", editedEntry.StartTime));
                        updateTimesheet.Parameters.Add(new SqlParameter("FinishTime", editedEntry.FinishTime));
                        updateTimesheet.Parameters.Add(new SqlParameter("TaskCode", editedEntry.TaskCode));
                        updateTimesheet.Parameters.Add(new SqlParameter("PhaseCode", editedEntry.PhaseCode));
                        updateTimesheet.Parameters.Add(new SqlParameter("Overhead", editedEntry.Overhead));
                        updateTimesheet.Parameters.Add(new SqlParameter("Billable", editedEntry.Billable));
                        updateTimesheet.Parameters.Add(new SqlParameter("Description", editedEntry.Description));
                        updateTimesheet.Parameters.Add(new SqlParameter("ID", editedEntry.ID));
                        updateTimesheet.Parameters.Add(new SqlParameter("TimesheetID", this.ID));                                
                        updateTimesheet.ExecuteNonQuery();
                    }
                }                          

                UnsavedChanges = true;
            }
        }

        /// <summary>
        /// News the timesheet.
        /// </summary>
        public void New()
        {                   
            UnsavedChanges = false;
            if (!Database.ContainsStaffNumber(StaffNumber))
            {
                Database.AddStaffMember(StaffNumber, StaffID);
            }

            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand newTimesheet = new SqlCommand("INSERT INTO Timesheet VALUES(@StaffNumber, @Month, @Year)", con))
                {                    
                    newTimesheet.Parameters.Add(new SqlParameter("StaffNumber", StaffNumber));
                    newTimesheet.Parameters.Add(new SqlParameter("Month", Month));
                    newTimesheet.Parameters.Add(new SqlParameter("Year", Year));                    
                    newTimesheet.ExecuteNonQuery();
                }
            }

            ID = GetNextUnusedID("Timesheet") - 1;
        }

        /// <summary>
        /// Updates the staff details.
        /// </summary>
        /// <param name="staffNumber">The staff number.</param>
        /// <param name="staffID">The staff ID.</param>
        public void UpdateStaffDetails(int staffNumber, string staffID)
        {
            this.StaffID = staffID;
            this.StaffNumber = staffNumber;
        }

        /// <summary>
        /// Reads the build.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void ReadBuild(string filename)
        {
            ParseTimesheet(filename);
            UpdateTimesheetMonthAndYear(Month, Year);
        }

        /// <summary>
        /// Reads the XML.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void ReadXML(string filename)
        {            
            Timesheet timesheet = Serialization.DeserializeTimesheet(filename);                       
                        
            ID = GetNextUnusedID("Timesheet");
            bool isFirstEntry = true;
            foreach (Entry entry in timesheet.Entries)
            {
                if (isFirstEntry)
                {
                    Month = entry.Date.Month;
                    Year = entry.Date.Year;
                    New();
                    isFirstEntry = false;
                }

                InsertEntry(entry, this.ID);            
            }                             

            DateTime aDate = GetADate();
            Month = aDate.Month;
            Year = aDate.Year;
        }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="entryToDelete">The entry to delete.</param>
        public void DeleteEntry(Entry entryToDelete)
        {
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand deleteEntry = new SqlCommand("DELETE FROM Entry WHERE TimesheetID=@ID AND ProjectNumber=@ProjectNumber AND Date=@Date AND StartTime=@StartTime", con))
                {
                    deleteEntry.Parameters.Add(new SqlParameter("ID", ID));
                    deleteEntry.Parameters.Add(new SqlParameter("ProjectNumber", entryToDelete.ProjectNumber));
                    deleteEntry.Parameters.Add(new SqlParameter("Date", entryToDelete.Date));
                    deleteEntry.Parameters.Add(new SqlParameter("StartTime", entryToDelete.StartTime));                   

                    deleteEntry.ExecuteNonQuery();                    
                }
            }      

            UnsavedChanges = true;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="staffID">The staff ID.</param>
        /// <param name="staffNumber">The staff number.</param>
        public void Build(string filename, string staffID, int staffNumber)
        {            
            List<string> lines = new List<string>();
            string sEntry = string.Empty;
            string month = filename.Split('.')[1];
            int year = Year;

            foreach (int projectNumber in GetAllProjectNumbers())
            {
                lines.Add("************************************************************************");
                lines.Add("*                                                                      *");
                lines.Add("* P" + projectNumber.ToString("0000") + " - " + month + "                                                          *");
                lines.Add("* Timesheet for " + staffID + ", " + month + " " + year + "                                           *");
                lines.Add("* Generated by WALDMs Timesheet Program                                *");
                lines.Add("*                                                                      *");
                lines.Add("*                                                                      *");
                lines.Add("************************************************************************");
                lines.Add("*TASK     PHASE    DATE     PER HRS              DESCRIPTION            ");
                lines.Add("*-------- -------- -------- --- ---- -----------------------------------");
                lines.Add("P" + projectNumber.ToString("0000"));

                foreach (Entry entry in GetAllEntriesFromProject(projectNumber))
                {
                    sEntry = string.Empty;
                    if (entry.Billable == "YES")
                    {
                        sEntry += " ";
                    }
                    else
                    {
                        sEntry += "-";
                    }

                    sEntry += entry.TaskCode;
                    sEntry += Util.AddSpaces(entry.TaskCode, Entry.MAX_TASKCODE_LENGTH);
                    sEntry += entry.PhaseCode;
                    sEntry += Util.AddSpaces(entry.PhaseCode, Entry.MAX_PHASECODE_LENGTH);
                    sEntry += entry.Date.ToString("dd-MMM-yyyy");
                    sEntry += Util.AddSpaces(entry.Date.ToString("dd-MMM-yyyy"), Entry.MAX_DATE_LENGTH);
                    sEntry += staffNumber;
                    sEntry += Util.AddSpaces(staffNumber.ToString(), Entry.MAX_STAFF_NUMBER_LENGTH);
                    sEntry += entry.Time.ToString("0.0");
                    sEntry += Util.AddSpaces(entry.Time.ToString("0.0"), Entry.MAX_TIME_LENGTH);
                    if (entry.Overhead == true)
                    {
                        sEntry += "*";
                    }

                    if (entry.Billable == "ACCOUNTABLE")
                    {
                        sEntry += "#";
                    }

                    sEntry += " " + entry.Description;
                    lines.Add(sEntry);
                }
            }

            lines.Add("END");

            System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
            foreach (string line in lines)
            {
                Console.WriteLine(line);
                file.WriteLine(line);
            }

            file.Close();
        }

        /// <summary>
        /// Verifies the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="check_month">if set to <c>true</c> [check_month].</param>
        /// <returns>
        /// If the entry passed verification
        /// </returns>
        /// <exception cref="EntriesNotInSameMonthException">If the new entry isn't in the same month as current entries</exception>
        /// <exception cref="ProjectCantBeBillableAndAccountableException">blah blah blah</exception>
        private bool VerifyEntryMonth(Entry entry, bool check_month)
        {
            if (Month != entry.Date.Month && Year != entry.Date.Year && check_month)
            {
                throw new EntriesNotInSameMonthException();
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Updates the timesheet month and year.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        private void UpdateTimesheetMonthAndYear(int month, int year)
        {
            Month = month;
            Year = year;
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand updateTimesheet = new SqlCommand("UPDATE Timesheet SET Month=@Month, Year=@Year WHERE ID=@ID", con))
                {
                    updateTimesheet.Parameters.Add(new SqlParameter("ID", ID));
                    updateTimesheet.Parameters.Add(new SqlParameter("Month", month));
                    updateTimesheet.Parameters.Add(new SqlParameter("Year", year));                                 

                    updateTimesheet.ExecuteNonQuery();
                }
            }      
        }

        /// <summary>
        /// Determines whether [contains entry with start time] [the specified start time].
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="date">The date.</param>
        /// <returns>
        ///   <c>true</c> if [contains entry with start time] [the specified start time]; otherwise, <c>false</c>.
        /// </returns>
        private bool ContainsEntryWithStartTime(TimeSpan startTime, DateTime date)
        {
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand getEntryWithStartTime = new SqlCommand("SELECT ID FROM Entry WHERE StartTime=@StartTime AND Date=@Date", con))
                {
                    getEntryWithStartTime.Parameters.Add(new SqlParameter("StartTime", startTime));
                    getEntryWithStartTime.Parameters.Add(new SqlParameter("Date", date));                    
                    var x = getEntryWithStartTime.ExecuteScalar();
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
        /// Gets the A date.
        /// </summary>
        /// <returns>FDGDF GFDGFD</returns>
        private DateTime GetADate()
        {
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand getADate = new SqlCommand("SELECT Date FROM Entry WHERE TimesheetID=@ID", con))
                {
                    getADate.Parameters.Add(new SqlParameter("ID", ID));

                    using (SqlDataReader reader = getADate.ExecuteReader())
                    {
                        reader.Read();
                        return reader.GetDateTime(0);
                    }
                }
            }
        }

        /// <summary>
        /// Inserts the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="timesheetID">The timesheet ID.</param>
        private void InsertEntry(Entry entry, int timesheetID)
        {
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand insertEntry = new SqlCommand("INSERT INTO Entry VALUES(@Date, @ProjectNumber, @StartTime, @FinishTime, @TaskCode, @PhaseCode, @Overhead, @Billable, @Description, @TimesheetID)", con))
                {
                    insertEntry.Parameters.Add(new SqlParameter("Date", entry.Date));
                    insertEntry.Parameters.Add(new SqlParameter("ProjectNumber", entry.ProjectNumber));
                    insertEntry.Parameters.Add(new SqlParameter("StartTime", entry.StartTime));
                    insertEntry.Parameters.Add(new SqlParameter("FinishTime", entry.FinishTime));
                    insertEntry.Parameters.Add(new SqlParameter("TaskCode", entry.TaskCode));
                    insertEntry.Parameters.Add(new SqlParameter("PhaseCode", entry.PhaseCode));
                    insertEntry.Parameters.Add(new SqlParameter("Overhead", entry.Overhead));
                    insertEntry.Parameters.Add(new SqlParameter("Billable", entry.Billable));
                    insertEntry.Parameters.Add(new SqlParameter("Description", entry.Description));
                    insertEntry.Parameters.Add(new SqlParameter("TimesheetID", timesheetID));
                    insertEntry.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets all entries from project.
        /// </summary>
        /// <param name="projectNumber">The project number.</param>
        /// <returns>DFFD GDFGFD</returns>
        private List<Entry> GetAllEntriesFromProject(int projectNumber)
        {
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand getAllEntriesFromProject = new SqlCommand("SELECT * FROM Entry WHERE TimesheetID=@ID AND ProjectNumber=@ProjectNumber", con))
                {
                    getAllEntriesFromProject.Parameters.Add(new SqlParameter("ID", ID));
                    getAllEntriesFromProject.Parameters.Add(new SqlParameter("ProjectNumber", projectNumber));
                    List<Entry> entries = new List<Entry>();
                    using (SqlDataReader reader = getAllEntriesFromProject.ExecuteReader())
                    {
                        reader.Read();
                        entries.Sort(new SortEntriesViaDateTime());
                        return entries;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all project numbers.
        /// </summary>
        /// <returns>A list of all project numbers</returns>
        private List<int> GetAllProjectNumbers()
        {
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand getAllProjectNumbers = new SqlCommand("SELECT DISTINCT ProjectNumber FROM Entry WHERE TimesheetID=@ID", con))
                {
                    getAllProjectNumbers.Parameters.Add(new SqlParameter("ID", ID));
                    List<int> projectNumbers = new List<int>();
                    using (SqlDataReader reader = getAllProjectNumbers.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            projectNumbers.Add(reader.GetInt32(0));
                        }

                        projectNumbers.Sort();
                        return projectNumbers;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the length of entry with start time.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="date">The date.</param>
        /// <returns>
        /// The length of the entry with the given start time
        /// </returns>
        private TimeSpan GetLengthOfEntryWithStartTime(TimeSpan startTime, DateTime date)
        {
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand getFinishTimeOfEntryWithStartTime = new SqlCommand("SELECT FinishTime FROM Entry WHERE StartTime=@StartTime AND Date=@Date AND TimesheetID=@TimesheetID", con))
                {
                    getFinishTimeOfEntryWithStartTime.Parameters.Add(new SqlParameter("StartTime", startTime));
                    getFinishTimeOfEntryWithStartTime.Parameters.Add(new SqlParameter("Date", date));
                    getFinishTimeOfEntryWithStartTime.Parameters.Add(new SqlParameter("TimesheetID", ID));
                    TimeSpan finishTime = (TimeSpan)getFinishTimeOfEntryWithStartTime.ExecuteScalar();
                    return finishTime - startTime;
                }
            }
        }

        /// <summary>
        /// Persists the insert entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        private void PersistInsertEntry(Entry entry)
        {
            TimeSpan conflictingEntryLength = GetLengthOfEntryWithStartTime(entry.StartTime, entry.Date);
            entry.StartTime += conflictingEntryLength;
            entry.FinishTime += conflictingEntryLength;
            try
            {
                InsertEntry(entry, this.ID);
            }
            catch (SqlException)
            {
                PersistInsertEntry(entry);
            }
        }
    }
}
