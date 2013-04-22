// -----------------------------------------------------------------------
// <copyright file="Project.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// The _billable
        /// </summary>
        private string _billable;

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
        /// Gets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        public int Number
        {            
            get;
            private set;
        }

        /// <summary>
        /// Determines whether [is project billable] [the specified project number].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is project billable] [the specified project number]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsBillable()
        {
            return _billable == "Yes";            
        }

        /// <summary>
        /// Determines whether [is project accountable] [the specified project number].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is project accountable] [the specified project number]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAccountable()
        {
            return _billable == "Accountable";
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="newEntry">The entry.</param>
        /// <param name="allEntries">All entries.</param>
        /// <exception cref="TimesheetProgramLogic.ProjectCantBeBillableAndAccountableException">blahy blah</exception>
        public void AddEntry(Entry newEntry, List<Entry> allEntries)
        {
            if (Entries.Count == 0)
            {
                _billable = newEntry.Billable;                
            }

            if (IsBillable() && newEntry.Billable.Equals("Accountable"))
            {
                throw new ProjectCantBeBillableAndAccountableException();
            }
            else if (IsAccountable() && newEntry.Billable.Equals("Yes"))
            {
                throw new ProjectCantBeBillableAndAccountableException();
            }

            bool conflicts_with_existing_entry = true;
            while (conflicts_with_existing_entry)
            {
                conflicts_with_existing_entry = false;
                foreach (Entry entry in allEntries)
                {
                    if (newEntry.Date == entry.Date && newEntry.StartTime == entry.StartTime)
                    {
                        TimeSpan existing_entry_length = entry.FinishTime - entry.StartTime;
                        newEntry.StartTime = newEntry.StartTime + existing_entry_length;
                        newEntry.FinishTime = newEntry.FinishTime + existing_entry_length;
                        conflicts_with_existing_entry = true;
                    }
                }
            }

            Entries.Add(newEntry);
        }
    }
}
