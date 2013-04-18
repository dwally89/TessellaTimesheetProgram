// -----------------------------------------------------------------------
// <copyright file="SubmitViaNotes.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
namespace TimesheetProgramLogic
{
    using System.Security;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SubmitViaNotes : ISubmitter
    {
        /// <summary>
        /// The email_address
        /// </summary>
        private string email_address;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitViaNotes" /> class.
        /// </summary>
        /// <param name="email_address">The email_address.</param>
        public SubmitViaNotes(string email_address)
        {
            this.email_address = email_address;
        }

        /// <summary>
        /// Sends the via notes.
        /// </summary>
        /// <param name="settings">Any settings needed</param>
        /// <param name="staffID">The staff ID.</param>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <param name="fullFilename">The full filename.</param>
        /// <param name="password">The password required to submit</param>
        public void Send(Settings settings, string staffID, string month, string year, string fullFilename, SecureString password = null)
        {
            SubmitToNotes.Send(staffID, month, year, fullFilename, email_address);
        }
    }
}
