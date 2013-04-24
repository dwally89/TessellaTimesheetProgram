namespace TimesheetProgramLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// manages the timesheet
    /// </summary>
    public class LinqTimesheetManager : TimesheetProgramLogic.ITimesheetManager
    {
        /// <summary>
        /// Gets the next unused entry ID.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <returns>
        /// fdgdfg dfgfdgfd
        /// </returns>
        public int GetNextUnusedEntryID(List<Entry> entries)
        {
            if (entries.Count == 0)
            {
                return 0;
            }
            else
            {
                return entries.Max(entry => entry.ID) + 1;
            }
        }

        /// <summary>
        /// Gets all entries from project.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="projectNumber">The project number.</param>
        /// <returns>
        /// DFFD GDFGFD
        /// </returns>
        public List<Entry> GetAllEntriesFromProject(List<Entry> entries, int projectNumber)
        {
            return entries.Where(entry => entry.ProjectNumber == projectNumber).ToList();
        }

        /// <summary>
        /// Gets all project numbers.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <returns>
        /// A list of all project numbers
        /// </returns>
        public List<int> GetAllProjectNumbers(List<Entry> entries)
        {
            var x = (from entry in entries select entry.ProjectNumber).Distinct().ToList();
            x.Sort();
            return x;
        }

        /// <summary>
        /// Edits the entry.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="editedEntry">The edited entry.</param>
        /// <returns>
        /// gfdgfd dfgdfgfd
        /// </returns>
        public List<Entry> EditEntry(List<Entry> entries, Entry editedEntry)
        {
            List<Entry> filteredEntries = entries.Where(e => e.ID != editedEntry.ID).ToList();
            filteredEntries.Add(editedEntry);
            return filteredEntries;
        }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="entryToDelete">The entry to delete.</param>
        /// <returns>
        /// dfgdfgfd gdfgfdgfd
        /// </returns>
        public List<Entry> DeleteEntry(List<Entry> entries, Entry entryToDelete)
        {
            return entries.Where(entry => entry.ID != entryToDelete.ID).ToList();            
        }

        /// <summary>
        /// Determines whether [contains entry with start time] [the specified start time].
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>
        ///   <c>true</c> if [contains entry with start time] [the specified start time]; otherwise, <c>false</c>.
        /// </returns>
        public bool VerifyEntryTime(List<Entry> entries, Entry entry)
        {
            /* If entry.starttime >= existing entry startime
             * and
             * entry.finishtime <= existing entry finishtime
               */
            return GetConflictingEntries(entries, entry).Count == 0;            
        }

        /// <summary>
        /// Gets the length of conflicting entry.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="newEntry">The new entry.</param>
        /// <returns>gfhgfhgf gfhfghfg</returns>
        public TimeSpan GetLengthOfConflictingEntry(List<Entry> entries, Entry newEntry)
        {
            Entry entry = GetConflictingEntries(entries, newEntry)[0];
            return entry.FinishTime - entry.StartTime;
        }

        /// <summary>
        /// Inserts the entry.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>dfgdfg dfgfdgfd</returns>
        public List<Entry> InsertEntry(List<Entry> entries, Entry entry)
        {
            entries.Add(entry);
            return entries;
        }

        /// <summary>
        /// Gets the conflicting entries.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>fghfghgf fghfhgfhgf</returns>
        private List<Entry> GetConflictingEntries(List<Entry> entries, Entry entry)
        {
            entries = entries.Where(e => e.ID != entry.ID).ToList();
            List<Entry> existingStartTimeOverlaps = entries.Where(e => e.StartTime >= entry.StartTime && e.StartTime < entry.FinishTime).ToList();
            List<Entry> existingFinishTimeOverlaps = entries.Where(e => e.FinishTime > entry.StartTime && e.FinishTime <= entry.FinishTime).ToList();

            entries = new List<Entry>();
            entries.AddRange(existingStartTimeOverlaps);
            entries.AddRange(existingFinishTimeOverlaps);
                        
            return entries.Where(e => e.Date == entry.Date).ToList();
        }
    }
}
