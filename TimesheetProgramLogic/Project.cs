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
        /// Adds the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public void AddEntry(Entry entry)
        {
            Entries.Add(entry);
        }
    }
}
