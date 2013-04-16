namespace TimesheetProgramLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Exception thrown when the project is both billable and accountable
    /// </summary>
    public class ProjectCantBeBillableAndAccountableException : Exception
    {
    }
}
