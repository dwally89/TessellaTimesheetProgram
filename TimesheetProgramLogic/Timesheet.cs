// -----------------------------------------------------------------------
// <copyright file="Timesheet.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.SqlClient;    

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Timesheet : ASubmittable
    {
        /// <summary>
        /// The TIMESHEE t_ EMAI l_ ADDRESS
        /// </summary>
        public const string TIMESHEET_EMAIL_ADDRESS = "timesheet@tessella.com";

        /// <summary>
        /// Initializes a new instance of the <see cref="Timesheet" /> class.
        /// </summary>
        /// <param name="staffNumber">The staff number.</param>
        /// <param name="staffID">The staff ID.</param>
        public Timesheet(StaffNumber staffNumber, StaffID staffID)
        {
            Entries = new List<Entry>();
            this.StaffNumber = staffNumber;
            this.StaffID = staffID;            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Timesheet" /> class.
        /// DO NOT USE. ONLY PRESENT FOR SERIALIZATION.
        /// </summary>
        public Timesheet()
        {            
        }

        /// <summary>
        /// Gets the email address.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        public override string EmailAddress
        {
            get { return TIMESHEET_EMAIL_ADDRESS; }
        }

        /// <summary>
        /// Gets or sets the month.
        /// </summary>
        /// <value>
        /// The month.
        /// </value>
        public int Month
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        /// <value>
        /// The year.
        /// </value>
        public int Year
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the staff number.
        /// </summary>
        /// <value>
        /// The staff number.
        /// </value>
        public StaffNumber StaffNumber { get; set; }

        /// <summary>
        /// Gets or sets the staff ID.
        /// </summary>
        /// <value>
        /// The staff ID.
        /// </value>
        public StaffID StaffID { get; set; }

        /// <summary>
        /// Gets or sets the entries.
        /// </summary>
        /// <value>
        /// The entries.
        /// </value>
        public List<Entry> Entries
        {
            get;
            set;
        }

        /// <summary>
        /// Updates the staff details.
        /// </summary>
        /// <param name="staffNumber">The staff number.</param>
        /// <param name="staffID">The staff ID.</param>
        public void UpdateStaffDetails(StaffNumber staffNumber, StaffID staffID)
        {
            this.StaffID = staffID;
            this.StaffNumber = staffNumber;
        }
       
        /// <summary>
        /// Updates the timesheet month and year.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        public void UpdateMonthAndYear(int month, int year)
        {
            Month = month;
            Year = year;            
        }

        /// <summary>
        /// Gets the A date.
        /// </summary>
        /// <returns>FDGDF GFDGFD</returns>
        public DateTime GetADate()
        {
            return Entries[0].Date;            
        }
    }
}
