// -----------------------------------------------------------------------
// <copyright file="SubmitViaOtherEmail.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    using System.Security;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SubmitViaOtherEmail : ISubmitter
    {
        /// <summary>
        /// The email_address
        /// </summary>
        private string email_address;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitViaOtherEmail" /> class.
        /// </summary>
        /// <param name="email_address">The email_address.</param>
        public SubmitViaOtherEmail(string email_address)
        {
            this.email_address = email_address;
        }

        /// <summary>
        /// Sends the via notes.
        /// </summary>
        /// <param name="settings">Any nettings needed for creating emails</param>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <param name="fullFilename">The full filename.</param>
        /// <param name="password">The password required to submit</param>
        public void Send(Settings settings, string month, string year, string fullFilename, SecureString password = null)
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

            email.SendEmail(this.email_address, month, year, fullFilename);
        }
    }
}
