﻿namespace TimesheetProgramLogic
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// fgdssd sdfsdfsd
    /// </summary>
    public abstract class ATimesheetManager
    {
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
        /// News this instance.
        /// </summary>
        public void New()
        {
            Timesheet = new Timesheet(Timesheet.StaffNumber, Timesheet.StaffID);
            UnsavedChanges = false;   
        }

        /// <summary>
        /// Performs the edit entry.
        /// </summary>
        /// <param name="editedEntry">The edited entry.</param>
        public abstract void PerformEditEntry(Entry editedEntry);

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="entryToDelete">The entry to delete.</param>
        public abstract void DeleteEntry(Entry entryToDelete);

        /// <summary>
        /// Gets all entries from project.
        /// </summary>
        /// <param name="projectNumber">The project number.</param>
        /// <returns>
        /// fdgdfgfd dfgdfgd
        /// </returns>
        public abstract List<Entry> GetAllEntriesFromProject(int projectNumber);

        /// <summary>
        /// Gets all project numbers.
        /// </summary>
        /// <returns>
        /// fdgdfg dfgfdgdf
        /// </returns>
        public abstract List<int> GetAllProjectNumbers();

        /// <summary>
        /// Gets the next unused entry ID.
        /// </summary>
        /// <returns>
        /// fdgdfg dfgfdgfd
        /// </returns>
        public abstract int GetNextUnusedEntryID();

        /// <summary>
        /// Verifies the entry time.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        /// bfdbfd sdfgdgfd
        /// </returns>
        public abstract bool VerifyEntryTime(Entry entry);

        /// <summary>
        /// Inserts the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public abstract void InsertEntry(Entry entry);

        /// <summary>
        /// Gets the length of conflicting entry.
        /// </summary>
        /// <param name="newEntry">The new entry.</param>
        /// <returns>
        /// gfhgfhgf fghfhgf
        /// </returns>
        public abstract TimeSpan GetLengthOfConflictingEntry(Entry newEntry);

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

            if (!VerifyEntryTime(newEntry))
            {
                throw new InvalidEntryTimeException();
            }

            if (VerifyEntryMonth(newEntry, true))
            {
                InsertEntry(newEntry);
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
                TimeSpan lengthOfConflictingEntry = GetLengthOfConflictingEntry(newEntry);
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
            if (Timesheet.Entries.Count == 1)
            {
                checkMonth = false;
            }

            if (!VerifyEntryTime(editedEntry))
            {
                throw new InvalidEntryTimeException();
            }

            if (VerifyEntryMonth(editedEntry, checkMonth))
            {
                PerformEditEntry(editedEntry);
                UnsavedChanges = true;
            }
        }

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
            string sEntry = string.Empty;
            string month = filename.Split('.')[1];
            int year = Timesheet.Year;

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
            if (Timesheet.Month != entry.Date.Month && Timesheet.Year != entry.Date.Year && check_month)
            {
                throw new EntriesNotInSameMonthException();
            }
            else
            {
                return true;
            }
        }
    }
}
