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
    public class Timesheet : ISubmittable
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
        public Timesheet()
        {
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

            if (controller.Month != newEntry.Date.Month)
            {
                throw new EntriesNotInSameMonthException();
            }
            else
            {
                if (isProjectBillable(controller, newEntry.ProjectNumber) && newEntry.Billable.Equals("Accountable"))
                {
                    throw new ProjectCantBeBillableAndAccountableException();
                }
                else if (isProjectAccountable(controller, newEntry.ProjectNumber) && newEntry.Billable.Equals("Yes"))
                {
                    throw new ProjectCantBeBillableAndAccountableException();
                }
                else
                {
                    controller.Entries.Add(newEntry);
                    controller.UnsavedChanges = true;
                }
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
            controller.Entries[controller.Entries.IndexOf(entryToEdit)] = editedEntry;
            controller.UnsavedChanges = true;
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
        /// Sends the timesheet.
        /// </summary>
        /// <param name="staffID">The staff ID.</param>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <param name="fullFilename">The full filename.</param>
        public void SendViaNotes(string staffID, string month, string year, string fullFilename)
        {
            SubmitToNotes.Send(staffID, month, year, fullFilename, TIMESHEET_EMAIL_ADDRESS);
        }

        /// <summary>
        /// Sends the via other email.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="sMonth">The s month.</param>
        /// <param name="year">The year.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="password">The password.</param>
        public void SendViaOtherEmail(Settings settings, string sMonth, string year, string filename, SecureString password = null)
        {
            Email email;
            if (password == null)
            {
                email = new Email(settings);
            }
            else
            {
                email = new Email(settings, password);
            }
            
            email.SendEmail(TIMESHEET_EMAIL_ADDRESS, sMonth, year, filename);
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        public void Build()
        {
            List<Project> projects = buildProjectList();

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
        /// Determines whether [is project billable] [the specified project number].
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="projectNumber">The project number.</param>
        /// <returns>
        ///   <c>true</c> if [is project billable] [the specified project number]; otherwise, <c>false</c>.
        /// </returns>
        private static bool isProjectBillable(Controller controller, int projectNumber)
        {
            bool billable = false;
            foreach (Entry entry in controller.Entries)
            {
                if (entry.Billable == "Yes" && entry.ProjectNumber == projectNumber)
                {
                    billable = true;
                    break;
                }
            }

            return billable;
        }

        /// <summary>
        /// Determines whether [is project accountable] [the specified project number].
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="projectNumber">The project number.</param>
        /// <returns>
        ///   <c>true</c> if [is project accountable] [the specified project number]; otherwise, <c>false</c>.
        /// </returns>
        private static bool isProjectAccountable(Controller controller, int projectNumber)
        {
            bool accountable = false;
            foreach (Entry entry in controller.Entries)
            {
                if (entry.Billable == "Accountable" && entry.ProjectNumber == projectNumber)
                {
                    accountable = true;
                    break;
                }
            }

            return accountable;
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

        /// <summary>
        /// Builds the project list.
        /// </summary>
        /// <returns>A list of projects</returns>
        private List<Project> buildProjectList()
        {
            entries.Sort(new SortEntriesViaProjectNumber());
            List<Project> projects = new List<Project>();
            bool projectExists;
            foreach (Entry entry in entries)
            {
                projectExists = false;
                foreach (Project project in projects)
                {
                    if (entry.ProjectNumber == project.Number)
                    {
                        project.AddEntry(entry);
                        projectExists = true;
                    }
                }

                if (!projectExists)
                {
                    Project project = new Project(entry.ProjectNumber);
                    project.AddEntry(entry);
                    projects.Add(project);
                }
            }

            return projects;
        }
    }
}
