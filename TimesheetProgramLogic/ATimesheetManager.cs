namespace TimesheetProgramLogic
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// fgdssd sdfsdfsd
    /// </summary>
    public abstract class ATimesheetManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ATimesheetManager" /> class.
        /// </summary>
        /// <param name="timesheet">The timesheet.</param>
        public ATimesheetManager(Timesheet timesheet)
        {
            this.Timesheet = timesheet;
        }

        /// <summary>
        /// Gets or sets the timesheet.
        /// </summary>
        /// <value>
        /// The timesheet.
        /// </value>
        public Timesheet Timesheet
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether [unsaved changes].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [unsaved changes]; otherwise, <c>false</c>.
        /// </value>
        public bool UnsavedChanges
        {
            get;
            private set;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        public void New()
        {
            Timesheet = new Timesheet(Timesheet.StaffNumber, Timesheet.StaffID);
            UnsavedChanges = false;   
        }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="entryToDelete">The entry to delete.</param>
        public abstract void DeleteEntry(Entry entryToDelete);

        /// <summary>
        /// Gets the next unused entry ID.
        /// </summary>
        /// <returns>
        /// fdgdfg dfgfdgfd
        /// </returns>
        public abstract int GetNextUnusedEntryID();

        /// <summary>
        /// Reads the build.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void ReadBuild(string filename)
        {
            Parse(filename);
            DateTime date = Timesheet.GetADate();
            Timesheet.UpdateMonthAndYear(date.Month, date.Year);
            UnsavedChanges = false;
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
                    Timesheet.Month = entry.Date.Month;
                    Timesheet.Year = entry.Date.Year;
                    New();
                    isFirstEntry = false;
                }

                AddEntry(entry);
            }

            DateTime aDate = Timesheet.GetADate();
            Timesheet.Month = aDate.Month;
            Timesheet.Year = aDate.Year;
            UnsavedChanges = false;
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
            string month = filename.Split('.')[1];
            int year = Timesheet.Year;

            foreach (int projectNumber in GetAllProjectNumbers())
            {
                WriteProjectHeader(staffID, lines, month, year, projectNumber);

                foreach (Entry entry in GetAllEntriesFromProject(projectNumber))
                {
                    WriteEntry(staffNumber, lines, entry);
                }
            }

            lines.Add("END");

            System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
            foreach (string line in lines)
            {                
                file.WriteLine(line);
            }

            file.Close();
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
            if (Timesheet.Entries.Count == 0)
            {
                Timesheet.Month = newEntry.Date.Month;
                Timesheet.Year = newEntry.Date.Year;
                Timesheet.UpdateMonthAndYear(newEntry.Date.Month, newEntry.Date.Year);
            }

            VerifyEntry(newEntry, true);

            InsertEntry(newEntry);
            UnsavedChanges = true;
        }

        /// <summary>
        /// Edits the entry.
        /// </summary>
        /// <param name="editedEntry">The edited entry.</param>
        /// <exception cref="System.Exception">Entry could not be edited</exception>
        public void EditEntry(Entry editedEntry)
        {
            VerifyEntry(editedEntry, Timesheet.Entries.Count != 1);

            PerformEditEntry(editedEntry);
            UnsavedChanges = true;
        }

        /// <summary>
        /// Performs the edit entry.
        /// </summary>
        /// <param name="editedEntry">The edited entry.</param>
        protected abstract void PerformEditEntry(Entry editedEntry);
        
        /// <summary>
        /// Gets all entries from project.
        /// </summary>
        /// <param name="projectNumber">The project number.</param>
        /// <returns>
        /// fdgdfgfd dfgdfgd
        /// </returns>
        protected abstract List<Entry> GetAllEntriesFromProject(int projectNumber);

        /// <summary>
        /// Gets all project numbers.
        /// </summary>
        /// <returns>
        /// fdgdfg dfgfdgdf
        /// </returns>
        protected abstract List<int> GetAllProjectNumbers();        

        /// <summary>
        /// Gets the conflicting entries.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>dfgdfgd fdgdfgfd</returns>
        protected abstract List<Entry> GetConflictingEntries(Entry entry);

        /// <summary>
        /// Inserts the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        protected abstract void InsertEntry(Entry entry);               

        /// <summary>
        /// Determines whether the specified project number is billable.
        /// </summary>
        /// <param name="projectNumber">The project number.</param>
        /// <returns>
        ///   <c>true</c> if the specified project number is billable; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsBillable(int projectNumber);

        /// <summary>
        /// Determines whether the specified project number is accountable.
        /// </summary>
        /// <param name="projectNumber">The project number.</param>
        /// <returns>
        ///   <c>true</c> if the specified project number is accountable; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsAccountable(int projectNumber);

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
            if (Timesheet.Month != entry.Date.Month && Timesheet.Year != entry.Date.Year && check_month)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Verifies the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="checkMonth">if set to <c>true</c> [check month].</param>
        /// <exception cref="InvalidEntryTimeException">dfgdfgdf dfgdfgfd</exception>
        /// <exception cref="EntriesNotInSameMonthException">dfgdfgdf dfgfdgd</exception>
        /// <exception cref="ProjectCantBeBillableAndAccountableException">dfgdfg dfgdfgfd</exception>
        private void VerifyEntry(Entry entry, bool checkMonth)
        {
            if (!VerifyEntryTime(entry))
            {
                throw new InvalidEntryTimeException();
            }

            if (!VerifyEntryMonth(entry, checkMonth))
            {
                throw new EntriesNotInSameMonthException();
            }

            if ((IsBillable(entry.ProjectNumber) && entry.Billable.Equals("Accountable"))
                || (IsAccountable(entry.ProjectNumber) && entry.Billable.Equals("Yes")))
            {
                throw new ProjectCantBeBillableAndAccountableException();
            }
        }

        /// <summary>
        /// Determines whether [contains entry with start time] [the specified start time].
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        ///   <c>true</c> if [contains entry with start time] [the specified start time]; otherwise, <c>false</c>.
        /// </returns>
        private bool VerifyEntryTime(Entry entry)
        {
            /* If entry.starttime >= existing entry startime
             * and
             * entry.finishtime <= existing entry finishtime
               */
            return GetConflictingEntries(entry).Count == 0;
        }
        
        /// <summary>
        /// Gets the length of conflicting entry.
        /// </summary>
        /// <param name="newEntry">The new entry.</param>
        /// <returns>
        /// gfhgfhgf gfhfghfg
        /// </returns>
        private TimeSpan GetLengthOfConflictingEntry(Entry newEntry)
        {
            Entry entry = GetConflictingEntries(newEntry)[0];
            return entry.FinishTime - entry.StartTime;
        }

        /// <summary>
        /// Persists the add entry.
        /// </summary>
        /// <param name="newEntry">The new entry.</param>
        private void PersistAddEntry(Entry newEntry)
        {
            try
            {
                AddEntry(newEntry);
            }
            catch (InvalidEntryTimeException)
            {
                TimeSpan lengthOfConflictingEntry = GetLengthOfConflictingEntry(newEntry);
                newEntry.StartTime += lengthOfConflictingEntry;
                newEntry.FinishTime += lengthOfConflictingEntry;
                PersistAddEntry(newEntry);
            }
        }

        /// <summary>
        /// Parses the timesheet.
        /// </summary>
        /// <param name="filename">The filename.</param>
        private void Parse(string filename)
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
                            Timesheet.Month = entry.Date.Month;
                            Timesheet.Year = entry.Date.Year;
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
        /// Writes the entry.
        /// </summary>
        /// <param name="staffNumber">The staff number.</param>
        /// <param name="lines">The lines.</param>
        /// <param name="entry">The entry.</param>
        private void WriteEntry(int staffNumber, List<string> lines, Entry entry)
        {
            string sEntry = string.Empty;
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

        /// <summary>
        /// Writes the project header.
        /// </summary>
        /// <param name="staffID">The staff ID.</param>
        /// <param name="lines">The lines.</param>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <param name="projectNumber">The project number.</param>
        private void WriteProjectHeader(string staffID, List<string> lines, string month, int year, int projectNumber)
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
        }
    }
}
