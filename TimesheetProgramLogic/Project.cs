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
        /// Initializes a new instance of the <see cref="Project" /> class.
        /// </summary>
        /// <param name="number">The number.</param>
        public Project(int number)
        {
            this.Number = number;
            this.Entries = new List<Entry>();
        }

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
        /// <param name="new_entry">The entry.</param>
        public void AddEntry(Entry new_entry)
        {
            if (Entries.Count == 0)
            {
                _billable = new_entry.Billable;                
            }

            if (IsBillable() && new_entry.Billable.Equals("Accountable"))
            {
                throw new ProjectCantBeBillableAndAccountableException();
            }
            else if (IsAccountable() && new_entry.Billable.Equals("Yes"))
            {
                throw new ProjectCantBeBillableAndAccountableException();
            }

            bool conflicts_with_existing_entry = true;
            while (conflicts_with_existing_entry)
            {
                conflicts_with_existing_entry = false;
                foreach (Entry entry in Entries)
                {
                    if (new_entry.Date == entry.Date && new_entry.StartTime == entry.StartTime)
                    {
                        TimeSpan existing_entry_length = entry.FinishTime - entry.StartTime;
                        new_entry.StartTime = new_entry.StartTime + existing_entry_length;
                        new_entry.FinishTime = new_entry.FinishTime + existing_entry_length;
                        conflicts_with_existing_entry = true;
                    }
                }
            }

            Entries.Add(new_entry);
        }
    }
}
