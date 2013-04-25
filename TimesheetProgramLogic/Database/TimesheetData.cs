namespace TimesheetProgramLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// BLAH FDGDFGFD
    /// </summary>
    public class TimesheetData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimesheetData" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="staffNumber">The staff number.</param>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        public TimesheetData(int id, int staffNumber, int month, int year)
        {
            this.ID = id;            
            this.StaffNumber = staffNumber;
            this.Month = month;
            this.Year = year;
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public int ID { get; private set; }

        /// <summary>
        /// Gets the staff number.
        /// </summary>
        /// <value>
        /// The staff number.
        /// </value>
        public int StaffNumber { get; private set; }

        /// <summary>
        /// Gets the month.
        /// </summary>
        /// <value>
        /// The month.
        /// </value>
        public int Month { get; private set; }

        /// <summary>
        /// Gets the year.
        /// </summary>
        /// <value>
        /// The year.
        /// </value>
        public int Year { get; private set; }
    }
}
