namespace TimesheetProgramLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// manages the timesheet
    /// </summary>
    public class LinqTimesheetManager : ATimesheetManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinqTimesheetManager" /> class.
        /// </summary>
        /// <param name="timesheet">The timesheet.</param>
        public LinqTimesheetManager(Timesheet timesheet) : base(timesheet)
        { 
        }

        /// <summary>
        /// Gets the next unused entry ID.
        /// </summary>
        /// <returns>
        /// fdgdfg dfgfdgfd
        /// </returns>
        public override int GetNextUnusedEntryID()
        {
            if (Timesheet.Entries.Count == 0)
            {
                return 0;
            }
            else
            {
                return Timesheet.Entries.Max(entry => entry.ID) + 1;
            }
        }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="entryToDelete">The entry to delete.</param>
        public override void DeleteEntry(Entry entryToDelete)
        {
            Timesheet.Entries.Remove(Timesheet.Entries.Where(entry => entry.ID == entryToDelete.ID).ToList()[0]);
        }

        /// <summary>
        /// Determines whether the specified project number is billable.
        /// </summary>
        /// <param name="projectNumber">The project number.</param>
        /// <returns>
        ///   <c>true</c> if the specified project number is billable; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsBillable(int projectNumber)
        {
            return Timesheet.Entries.Where(entry => entry.Billable.Equals("Yes")).ToList().Count > 0;
        }

        /// <summary>
        /// Determines whether the specified project number is accountable.
        /// </summary>
        /// <param name="projectNumber">The project number.</param>
        /// <returns>
        ///   <c>true</c> if the specified project number is accountable; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsAccountable(int projectNumber)
        {
            return Timesheet.Entries.Where(entry => entry.Billable.Equals("Accountable")).ToList().Count > 0;
        }

        /// <summary>
        /// Gets all entries from project.
        /// </summary>
        /// <param name="projectNumber">The project number.</param>
        /// <returns>
        /// DFFD GDFGFD
        /// </returns>
        protected override List<Entry> GetAllEntriesFromProject(int projectNumber)
        {
            return Timesheet.Entries.Where(entry => entry.ProjectNumber == projectNumber).ToList();
        }

        /// <summary>
        /// Gets all project numbers.
        /// </summary>
        /// <returns>
        /// A list of all project numbers
        /// </returns>
        protected override List<int> GetAllProjectNumbers()
        {
            var x = (from entry in Timesheet.Entries select entry.ProjectNumber).Distinct().ToList();
            x.Sort();
            return x;
        }

        /// <summary>
        /// Edits the entry.
        /// </summary>
        /// <param name="editedEntry">The edited entry.</param>
        protected override void PerformEditEntry(Entry editedEntry)
        {
            Timesheet.Entries.Remove(Timesheet.Entries.Where(e => e.ID == editedEntry.ID).ToList()[0]);            
            Timesheet.Entries.Add(editedEntry);            
        }

        /// <summary>
        /// Inserts the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        protected override void InsertEntry(Entry entry)
        {
            Timesheet.Entries.Add(entry);            
        }

        /// <summary>
        /// Gets the conflicting entries.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        /// fghfghgf fghfhgfhgf
        /// </returns>
        protected override List<Entry> GetConflictingEntries(Entry entry)
        {
            List<Entry> entries = Timesheet.Entries.Where(e => e.ID != entry.ID && e.Date == entry.Date).ToList();
            List<Entry> existingStartTimeOverlaps = entries.Where(e => e.StartTime >= entry.StartTime && e.StartTime < entry.FinishTime).ToList();
            List<Entry> existingFinishTimeOverlaps = entries.Where(e => e.FinishTime > entry.StartTime && e.FinishTime <= entry.FinishTime).ToList();

            entries = new List<Entry>();
            entries.AddRange(existingStartTimeOverlaps);
            entries.AddRange(existingFinishTimeOverlaps);
                        
            return entries.ToList();
        }
    }
}
