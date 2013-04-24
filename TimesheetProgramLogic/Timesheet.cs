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
            New();
            this.Manager = new LinqTimesheetManager();            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Timesheet" /> class.
        /// DO NOT USE. ONLY PRESENT FOR SERIALIZATION.
        /// </summary>
        public Timesheet()
        {            
        }

        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <value>
        /// The manager.
        /// </value>
        public ITimesheetManager Manager { get; private set; }

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
            get;
            private set;
        }

        /// <summary>
        /// Parses the timesheet.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Parse(string filename)
        {                   
            List<string> fileContents = DataIO.ReadTextFile(filename);
            int projectNumber = -1;
            int entryID = 0;
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

                        PersistAddEntry(entry);                        

                        entryID++;
                    }
                }
            }            
        }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public void DeleteEntry(Entry entry)
        {
            Entries = Manager.DeleteEntry(Entries, entry);
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
            if (Entries.Count == 0)
            {
                Month = newEntry.Date.Month;
                Year = newEntry.Date.Year;                
                UpdateMonthAndYear(newEntry.Date.Month, newEntry.Date.Year);                
            }

            if (!Manager.VerifyEntryTime(Entries, newEntry))
            {
                throw new InvalidEntryTimeException();
            }
                       
            if (VerifyEntryMonth(newEntry, true))
            {          
                Entries = Manager.InsertEntry(Entries, newEntry);                
                UnsavedChanges = true;
            }            
        }

        /// <summary>
        /// Persists the add entry.
        /// </summary>
        /// <param name="newEntry">The new entry.</param>
        public void PersistAddEntry(Entry newEntry)
        {
            try
            {
                AddEntry(newEntry);
            }
            catch (InvalidEntryTimeException)
            {
                TimeSpan lengthOfConflictingEntry = Manager.GetLengthOfConflictingEntry(Entries, newEntry);
                newEntry.StartTime += lengthOfConflictingEntry;
                newEntry.FinishTime += lengthOfConflictingEntry;
                PersistAddEntry(newEntry);
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
            if (Entries.Count == 1)
            {
                checkMonth = false;
            }

            if (!Manager.VerifyEntryTime(Entries, editedEntry))
            {
                throw new InvalidEntryTimeException();
            }

            if (VerifyEntryMonth(editedEntry, checkMonth))
            {
                Entries = Manager.EditEntry(Entries, editedEntry);
                UnsavedChanges = true;
            }
        }

        /// <summary>
        /// News the timesheet.
        /// </summary>
        public void New()
        {
            Entries = new List<Entry>();
            UnsavedChanges = false;            
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
            Parse(filename);
            UpdateMonthAndYear(Month, Year);
        }

        /// <summary>
        /// Reads the XML.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void ReadXML(string filename)
        {            
            Timesheet timesheet = Serialization.DeserializeTimesheet(filename);                                   
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

                AddEntry(entry);            
            }                             

            DateTime aDate = GetADate();
            Month = aDate.Month;
            Year = aDate.Year;
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

            foreach (int projectNumber in Manager.GetAllProjectNumbers(Entries))
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

                foreach (Entry entry in Manager.GetAllEntriesFromProject(Entries, projectNumber))
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
        private void UpdateMonthAndYear(int month, int year)
        {
            Month = month;
            Year = year;            
        }

        /// <summary>
        /// Gets the A date.
        /// </summary>
        /// <returns>FDGDF GFDGFD</returns>
        private DateTime GetADate()
        {
            return Entries[0].Date;            
        }
    }
}
