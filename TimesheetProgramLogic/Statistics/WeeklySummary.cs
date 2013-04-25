namespace TimesheetProgramLogic
{
    /// <summary>
    /// fdgdfgd dfghfdgdf
    /// </summary>
    public class WeeklySummary
    {
        /// <summary>
        /// Gets or sets the number of days worked so far.
        /// </summary>
        /// <value>
        /// The number of days worked so far.
        /// </value>
        public int NumberOfDaysWorkedSoFar { get; set; }

        /// <summary>
        /// Gets the number of weeks worked so far.
        /// </summary>
        /// <value>
        /// The number of weeks worked so far.
        /// </value>
        public double NumberOfWeeksWorkedSoFar
        {
            get
            {
                return NumberOfDaysWorkedSoFar / 5.0;
            }
        }

        /// <summary>
        /// Gets or sets the total hours worked so far.
        /// </summary>
        /// <value>
        /// The total hours worked so far.
        /// </value>
        public decimal TotalHoursWorkedSoFar { get; set; }

        /// <summary>
        /// Gets the number of hours per week.
        /// </summary>
        /// <value>
        /// The number of hours per week.
        /// </value>
        public double NumberOfHoursPerWeek
        {
            get
            {
                return double.Parse(TotalHoursWorkedSoFar.ToString()) / NumberOfWeeksWorkedSoFar;
            }
        }

        /// <summary>
        /// Gets or sets the expected hours per week.
        /// </summary>
        /// <value>
        /// The expected hours per week.
        /// </value>
        public double ExpectedHoursPerWeek { get; set; }
    }
}
