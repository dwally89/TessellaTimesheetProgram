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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Timesheet : ISubmittable
    {
        /// <summary>
        /// The TIMESHEE t_ EMAI l_ ADDRESS
        /// </summary>
        public const string TIMESHEET_EMAIL_ADDRESS = "timesheet@tessella.com";

        /// <summary>
        /// The _projects
        /// </summary>
        private List<Project> _projects;

        /// <summary>
        /// Initializes a new instance of the <see cref="Timesheet" /> class.
        /// </summary>
        /// <param name="submitViaNotes">if set to <c>true</c> [submit via notes].</param>
        public Timesheet(bool submitViaNotes)
        {
            _projects = new List<Project>();
            UnsavedChanges = false;
            SubmitViaNotes = submitViaNotes;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [submit via notes].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [submit via notes]; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="System.Exception">Type of SubmitViaNotes not recognised</exception>
        public bool SubmitViaNotes
        {            
            get
            {
                Type notes = typeof(SubmitViaNotes);
                Type otherEmail = typeof(SubmitViaOtherEmail);
                if (notes.IsInstanceOfType(this.Submitter))
                {
                    return true;
                }
                else if (otherEmail.IsInstanceOfType(this.Submitter))
                {
                    return false;
                }
                else
                {
                    throw new Exception("Type of SubmitViaNotes not recognised");
                }
            }

            set
            {
                if (value)
                {
                    this.Submitter = new SubmitViaNotes(TIMESHEET_EMAIL_ADDRESS);
                }
                else
                {
                    this.Submitter = new SubmitViaOtherEmail(TIMESHEET_EMAIL_ADDRESS);
                }
            }
        }

        /// <summary>
        /// Gets or sets the submitter.
        /// </summary>
        /// <value>
        /// The submitter.
        /// </value>
        public ISubmitter Submitter
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
        /// Gets the projects.
        /// </summary>
        /// <value>
        /// The projects.
        /// </value>
        public List<Project> Projects
        {
            get
            {
                return _projects;
            }

            private set
            {
                _projects = value;
            }
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
        /// Gets the entries.
        /// </summary>
        /// <value>
        /// The entries.
        /// </value>
        public List<Entry> Entries
        {
            get
            {
                List<Entry> entries = new List<Entry>();
                foreach (Project project in Projects)
                {
                    foreach (Entry entry in project.Entries)
                    {
                        entries.Add(entry);
                    }
                }

                entries.Sort(new SortEntriesViaDateTime());
                return entries;
            }
        }

        /// <summary>
        /// Parses the timesheet.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void ParseTimesheet(string filename)
        {
            Projects.Clear();            
            List<string> fileContents = DataIO.ReadTextFile(filename);
            Project currentProject = null;
            int entryID = 0;
            foreach (string line in fileContents)
            {
                if (!line.StartsWith("*"))
                {
                    if (line.StartsWith("P"))
                    {
                        currentProject = new Project(int.Parse(line.Replace("P", string.Empty)));
                        Projects.Add(currentProject);
                        currentProject = Projects[Projects.Count - 1];
                    }
                    else if (line != "END")
                    {
                        currentProject.AddEntry(new Entry(entryID, currentProject.Number, line), Entries);
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
            if (NumberOfEntries() == 0)
            {
                Month = newEntry.Date.Month;
                Year = newEntry.Date.Year;
            }
                       
            if (VerifyEntryMonth(newEntry, true))
            {
                bool projectFound = false;
                foreach (Project project in Projects)
                {
                    if (project.Number == newEntry.ProjectNumber)
                    {                        
                        project.AddEntry(newEntry, Entries);
                        projectFound = true;
                        break;
                    }
                }

                if (!projectFound)
                {
                    Project project = new Project(newEntry.ProjectNumber);
                    project.AddEntry(newEntry, Entries);
                    Projects.Add(project);
                }

                UnsavedChanges = true;
            }            
        }

        /// <summary>
        /// Numbers the of entries.
        /// </summary>
        /// <returns>The number of entries in the timesheet</returns>
        public int NumberOfEntries()
        {
            int numberOfEntries = 0;
            foreach (Project project in Projects)
            {
                numberOfEntries += project.Entries.Count;
            }

            return numberOfEntries;
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
                Month = editedEntry.Date.Month;
                Year = editedEntry.Date.Year;
                bool entryFound = false;
                foreach (Project project in Projects)
                {
                    if (entryFound)
                    {
                        break;
                    }
                    else
                    {
                        foreach (Entry entry in project.Entries)
                        {
                            if (entry.ID == editedEntry.ID)
                            {
                                entryFound = true;
                                if (ContainsEntryWithStartTime(editedEntry.StartTime))
                                {
                                    throw new InvalidStartTimeException();
                                }

                                if (entry.ProjectNumber != editedEntry.ProjectNumber)
                                {
                                    project.Entries.Remove(entry);
                                    foreach (Project newProject in Projects)
                                    {
                                        if (newProject.Number == editedEntry.ProjectNumber)
                                        {
                                            newProject.AddEntry(editedEntry, Entries);                                            
                                            break;
                                        }
                                    }

                                    break;
                                }
                                else
                                {
                                    entry.Update(editedEntry);                                    
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!entryFound)
                {
                    throw new Exception("Entry could not be edited");
                }

                UnsavedChanges = true;
            }
        }

        /// <summary>
        /// News the timesheet.
        /// </summary>
        public void New()
        {
            Projects.Clear();            
            UnsavedChanges = false;
        }

        /// <summary>
        /// Reads the build.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void ReadBuild(string filename)
        {
            ParseTimesheet(filename);            

            Month = Projects[0].Entries[0].Date.Month;
            Year = Projects[0].Entries[0].Date.Year;
        }

        /// <summary>
        /// Reads the XML.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void ReadXML(string filename)
        {
            Projects = Serialization.DeserializeProjects(filename);

            Month = Projects[0].Entries[0].Date.Month;
            Year = Projects[0].Entries[0].Date.Year;
        }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="entryToDelete">The entry to delete.</param>
        public void DeleteEntry(Entry entryToDelete)
        {
            bool continueLoop = true;
            foreach (Project project in Projects)
            {
                foreach (Entry entry in project.Entries)
                {
                    if (entry == entryToDelete)
                    {
                        project.Entries.Remove(entryToDelete);
                        continueLoop = false;
                        break;
                    }
                }

                if (!continueLoop)
                {
                    break;
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

            foreach (Project project in Projects)
            {
                lines.Add("************************************************************************");
                lines.Add("*                                                                      *");
                lines.Add("* P" + project.Number.ToString("0000") + " - " + month + "                                                          *");
                lines.Add("* Timesheet for " + staffID + ", " + month + " " + year + "                                           *");
                lines.Add("* Generated by WALDMs Timesheet Program                                *");
                lines.Add("*                                                                      *");
                lines.Add("*                                                                      *");
                lines.Add("************************************************************************");
                lines.Add("*TASK     PHASE    DATE     PER HRS              DESCRIPTION            ");
                lines.Add("*-------- -------- -------- --- ---- -----------------------------------");
                lines.Add("P" + project.Number.ToString("0000"));

                foreach (Entry entry in project.Entries)
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
            if (Month != entry.Date.Month && check_month)
            {
                throw new EntriesNotInSameMonthException();
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Determines whether [contains entry with start time] [the specified start time].
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <returns>
        ///   <c>true</c> if [contains entry with start time] [the specified start time]; otherwise, <c>false</c>.
        /// </returns>
        private bool ContainsEntryWithStartTime(TimeSpan startTime)
        {
            foreach (Entry entry in Entries)
            {
                if (entry.StartTime == startTime)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
