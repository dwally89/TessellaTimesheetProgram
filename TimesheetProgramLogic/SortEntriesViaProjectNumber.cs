// -----------------------------------------------------------------------
// <copyright file="SortEntriesViaProjectNumber.cs" company="Microsoft">
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
    public class SortEntriesViaProjectNumber : IComparer<Entry>
    {
        /// <summary>
        /// Compares the specified e1.
        /// </summary>
        /// <param name="e1">The e1.</param>
        /// <param name="e2">The e2.</param>
        /// <returns>The result of the comparison</returns>
        public int Compare(Entry e1, Entry e2)
        {
            if (e1.ProjectNumber == e2.ProjectNumber)
            {
                return 0;
            }
            else if (e1.ProjectNumber > e2.ProjectNumber)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
