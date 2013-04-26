namespace TimesheetProgramLogic
{
    /// <summary>
    /// dsgfdg dfgfdgd
    /// </summary>
    public class TimesheetPath : NonEmptyString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimesheetPath" /> class.
        /// </summary>
        public TimesheetPath()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimesheetPath" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public TimesheetPath(string value)
            : base(value, "Timesheet path")
        {
        }
    }
}
