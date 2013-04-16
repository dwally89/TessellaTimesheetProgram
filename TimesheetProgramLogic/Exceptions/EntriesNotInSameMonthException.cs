namespace TimesheetProgramLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Exception thrown when all entries are not in the same month
    /// </summary>
    public class EntriesNotInSameMonthException : Exception
    {
    }
}
