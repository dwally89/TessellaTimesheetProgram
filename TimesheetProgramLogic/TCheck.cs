﻿// -----------------------------------------------------------------------
// <copyright file="TCheck.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Security;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TCheck : ASubmittable
    {
        /// <summary>
        /// The TCHEC k_ EMAI l_ ADDRESS
        /// </summary>
        private const string TCHECK_EMAIL_ADDRESS = "tcheck@tessella.com";

        /// <summary>
        /// Initializes a new instance of the <see cref="TCheck" /> class.
        /// </summary>
        /// <param name="submit_via_notes">if set to <c>true</c> [submit_via_notes].</param>
        public TCheck(bool submit_via_notes)
        {
            if (submit_via_notes)
            {
                this.Submitter = new SubmitViaNotes(TCHECK_EMAIL_ADDRESS);
            }
            else
            {
                this.Submitter = new SubmitViaOtherEmail(TCHECK_EMAIL_ADDRESS);
            }
        }

        /// <summary>
        /// Runs the specified month.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        public static void Run(Settings settings, string month, string year)
        {            
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = settings.TCheckPath + "\\tcheck.exe";
            startInfo.Arguments = string.Empty;
            startInfo.Arguments += " -m " + month + " -y " + year;
            string emailFileName = settings.StaffID + "." + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(int.Parse(month));
            startInfo.Arguments += " -f " + "\"" + settings.TimesheetPath + "\\" + emailFileName + "\"";
            string mstrLogFile = settings.TimesheetPath + "\\" + settings.StaffID + ".log";
            startInfo.Arguments += " -l " + "\"" + mstrLogFile + "\"";

            Process.Start(startInfo);
        }
    }
}
