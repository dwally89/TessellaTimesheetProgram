namespace TimesheetProgramLogic
{
    /// <summary>
    /// Monthly sumamry
    /// </summary>
    public class MonthlySummary
    {
        /// <summary>
        /// Gets or sets the number of days worked so far.
        /// </summary>
        /// <value>
        /// The number of days worked so far.
        /// </value>
        public int NumberOfDaysWorkedSoFar { get; set; }

        /// <summary>
        /// Gets or sets the total hours worked so far.
        /// </summary>
        /// <value>
        /// The total hours worked so far.
        /// </value>
        public decimal TotalHoursWorkedSoFar { get; set; }

        /// <summary>
        /// Gets the average hours per day.
        /// </summary>
        /// <value>
        /// The average hours per day.
        /// </value>
        public decimal AverageHoursPerDay
        {
            get
            {
                return TotalHoursWorkedSoFar / NumberOfDaysWorkedSoFar;
            }
        }

        /// <summary>
        /// Gets or sets the expected hours per day.
        /// </summary>
        /// <value>
        /// The expected hours per day.
        /// </value>
        public double ExpectedHoursPerDay { get; set; }
    }
}
