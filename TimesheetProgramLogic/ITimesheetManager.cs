namespace TimesheetProgramLogic
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// dfgdfgfd dfsgfdgfdg
    /// </summary>
    public interface ITimesheetManager
    {
        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="entryToDelete">The entry to delete.</param>
        /// <returns>
        /// dfgdfgfd gdfgfdgfd
        /// </returns>
        List<Entry> DeleteEntry(List<Entry> entries, Entry entryToDelete);

        /// <summary>
        /// Edits the entry.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="editedEntry">The edited entry.</param>
        /// <returns>
        /// gfdgfd dfgdfgfd
        /// </returns>
        List<Entry> EditEntry(List<Entry> entries, Entry editedEntry);

        /// <summary>
        /// Gets all entries from project.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="projectNumber">The project number.</param>
        /// <returns>
        /// fdgdfgfd dfgdfgd
        /// </returns>
        List<Entry> GetAllEntriesFromProject(List<Entry> entries, int projectNumber);

        /// <summary>
        /// Gets all project numbers.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <returns>
        /// fdgdfg dfgfdgdf
        /// </returns>
        List<int> GetAllProjectNumbers(List<Entry> entries);

        /// <summary>
        /// Gets the next unused entry ID.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <returns>
        /// fdgdfg dfgfdgfd
        /// </returns>
        int GetNextUnusedEntryID(List<Entry> entries);

        /// <summary>
        /// Verifies the entry time.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>
        /// bfdbfd sdfgdgfd
        /// </returns>
        bool VerifyEntryTime(List<Entry> entries, Entry entry);

        /// <summary>
        /// Inserts the entry.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>dfgfdgdf dfgdfgfd</returns>
        List<Entry> InsertEntry(List<Entry> entries, Entry entry);

        /// <summary>
        /// Gets the length of conflicting entry.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="newEntry">The new entry.</param>
        /// <returns>gfhgfhgf fghfhgf</returns>
        TimeSpan GetLengthOfConflictingEntry(List<Entry> entries, Entry newEntry);
    }
}
