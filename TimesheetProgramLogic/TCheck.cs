// -----------------------------------------------------------------------
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
    public class TCheck : ISubmittable
    {
        /// <summary>
        /// The TCHEC k_ EMAI l_ ADDRESS
        /// </summary>
        private const string TCHECK_EMAIL_ADDRESS = "tcheck@tessella.com";

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

        /// <summary>
        /// Sends to T check.
        /// </summary>
        /// <param name="staffID">The staff ID.</param>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <param name="fullFilename">The full filename.</param>
        public void SendViaNotes(string staffID, string month, string year, string fullFilename)
        {
            SubmitToNotes.Send(staffID, month, year, fullFilename, TCHECK_EMAIL_ADDRESS);
        }

        /// <summary>
        /// Sends the via other email.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="sMonth">The s month.</param>
        /// <param name="year">The year.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="password">The password.</param>
        public void SendViaOtherEmail(Settings settings, string sMonth, string year, string filename, SecureString password = null)
        {
            Email email;
            if (password == null)
            {
                email = new Email(settings);
            }
            else
            {
                email = new Email(settings, password);
            }

            email.SendEmail(TCHECK_EMAIL_ADDRESS, sMonth, year, filename);
        }
    }
}
