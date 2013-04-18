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
    using System.IO;
    using System.Security;

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
        /// The entries
        /// </summary>
        private List<Entry> entries;

        /// <summary>
        /// The filename
        /// </summary>
        private string filename;
        
        /// <summary>
        /// The staff ID
        /// </summary>
        private string staffID;
        
        /// <summary>
        /// The staff number
        /// </summary>
        private string staffNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="Timesheet" /> class.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="staffID">The staff ID.</param>
        /// <param name="staffNumber">The staff number.</param>
        public Timesheet(ObservableCollection<Entry> entries, string filename, string staffID, string staffNumber)
        {
            this.entries = new List<Entry>(entries);
            this.filename = filename;
            this.staffID = staffID;
            this.staffNumber = staffNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Timesheet" /> class.
        /// </summary>
        /// <param name="submit_via_notes">if set to <c>true</c> [submit_via_notes].</param>
        public Timesheet(bool submit_via_notes)
        {
            if (submit_via_notes)
            {
                this.Submitter = new SubmitViaNotes(TIMESHEET_EMAIL_ADDRESS);
            }
            else
            {
                this.Submitter = new SubmitViaOtherEmail(TIMESHEET_EMAIL_ADDRESS);
            }
        }

        /// <summary>
        /// Parses the timesheet.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>A list of projects</returns>
        public static List<Project> ParseTimesheet(string filename)
        {
            List<Project> timesheet = new List<Project>();
            List<string> fileContents = readFromFile(filename);
            Project currentProject = null;

            foreach (string line in fileContents)
            {
                if (!line.StartsWith("*"))
                {
                    if (line.StartsWith("P"))
                    {
                        if (currentProject != null)
                        {
                            timesheet.Add(currentProject);
                        }

                        currentProject = new Project(int.Parse(line.Replace("P", string.Empty)));
                    }
                    else if (line != "END")
                    {
                        currentProject.AddEntry(Entry.Parse(currentProject.Number, line));
                    }
                }
            }

            timesheet.Add(currentProject);
            return timesheet;
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="newEntry">The new entry.</param>
        /// <exception cref="TimesheetProgramLogic.EntriesNotInSameMonthException">blah blah blah</exception>
        /// <exception cref="TimesheetProgramLogic.ProjectCantBeBillableAndAccountableException">blha blha blha</exception>
        /// <exception cref="EntriesNotInSameMonthException">Occurs when all entries aren't in the same month</exception>
        /// <exception cref="ProjectCantBeBillableAndAccountableException">Occurs when the project is both billable and accountable</exception>
        public static void AddEntry(Controller controller, Entry newEntry)
        {
            if (controller.Entries.Count == 0)
            {
                controller.Month = newEntry.Date.Month;
                controller.Year = newEntry.Date.Year;
            }

            if (Entry.Verify(controller, newEntry, true))
            {
                controller.Entries.Add(newEntry);
                controller.UnsavedChanges = true;
            }            
        }

        /// <summary>
        /// Edits the entry.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="entryToEdit">The entry to edit.</param>
        /// <param name="editedEntry">The edited entry.</param>
        public static void EditEntry(Controller controller, Entry entryToEdit, Entry editedEntry)
        {
            bool checkMonth = true;
            if (controller.Entries.Count == 1)
            {
                checkMonth = false;
            }

            if (Entry.Verify(controller, editedEntry, checkMonth))
            {
                controller.Month = editedEntry.Date.Month;
                controller.Year = editedEntry.Date.Year;
                controller.Entries[controller.Entries.IndexOf(entryToEdit)] = editedEntry;
                controller.UnsavedChanges = true;
            }
        }

        /// <summary>
        /// News the timesheet.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public static void New(Controller controller)
        {
            controller.Entries.Clear();
            controller.UnsavedChanges = false;
        }

        /// <summary>
        /// Reads the build.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="filename">The filename.</param>
        public static void ReadBuild(Controller controller, string filename)
        {
            List<Project> timesheet = Timesheet.ParseTimesheet(filename);
            foreach (Project project in timesheet)
            {
                foreach (Entry entry in project.Entries)
                {
                    controller.Entries.Add(entry);
                }
            }
        }

        /// <summary>
        /// Reads the XML.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="filename">The filename.</param>
        public static void ReadXML(Controller controller, string filename)
        {
            ObservableCollection<Entry> file = Serialization.DeserializeEntries(filename);
            foreach (Entry entry in file)
            {
                controller.Entries.Add(entry);
            }
        }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="entryToDelete">The entry to delete.</param>
        public static void DeleteEntry(Controller controller, Entry entryToDelete)
        {
            controller.Entries.Remove(entryToDelete);
            controller.UnsavedChanges = true;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        public void Build()
        {
            List<Project> projects = Project.BuildProjectList(this.entries);

            List<string> lines = new List<string>();
            string sEntry = string.Empty;
            string month = filename.Split('.')[1];
            int year = entries[0].Date.Year;

            foreach (Project project in projects)
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
        /// Reads from file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>The contents of the file as a list of strings</returns>
        private static List<string> readFromFile(string filename)
        {
            List<string> fileContents = new List<string>();
            string line;
            using (StreamReader sr = new StreamReader(filename))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    fileContents.Add(line);
                }
            }

            return fileContents;
        }
    }
}
