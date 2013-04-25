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
        public LinqTimesheetManager(Timesheet timesheet)
        {
            this.Timesheet = timesheet;
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
        /// Gets all entries from project.
        /// </summary>
        /// <param name="projectNumber">The project number.</param>
        /// <returns>
        /// DFFD GDFGFD
        /// </returns>
        public override List<Entry> GetAllEntriesFromProject(int projectNumber)
        {
            return Timesheet.Entries.Where(entry => entry.ProjectNumber == projectNumber).ToList();
        }

        /// <summary>
        /// Gets all project numbers.
        /// </summary>
        /// <returns>
        /// A list of all project numbers
        /// </returns>
        public override List<int> GetAllProjectNumbers()
        {
            var x = (from entry in Timesheet.Entries select entry.ProjectNumber).Distinct().ToList();
            x.Sort();
            return x;
        }

        /// <summary>
        /// Edits the entry.
        /// </summary>
        /// <param name="editedEntry">The edited entry.</param>
        public override void PerformEditEntry(Entry editedEntry)
        {
            Timesheet.Entries.Remove(Timesheet.Entries.Where(e => e.ID == editedEntry.ID).ToList()[0]);            
            Timesheet.Entries.Add(editedEntry);            
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
        /// Determines whether [contains entry with start time] [the specified start time].
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        ///   <c>true</c> if [contains entry with start time] [the specified start time]; otherwise, <c>false</c>.
        /// </returns>
        public override bool VerifyEntryTime(Entry entry)
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
        public override TimeSpan GetLengthOfConflictingEntry(Entry newEntry)
        {
            Entry entry = GetConflictingEntries(newEntry)[0];
            return entry.FinishTime - entry.StartTime;
        }

        /// <summary>
        /// Inserts the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public override void InsertEntry(Entry entry)
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
        private List<Entry> GetConflictingEntries(Entry entry)
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
