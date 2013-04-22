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
        /// Initializes a new instance of the <see cref="SubmitViaOtherEmail" /> class.
        /// </summary>
        public SubmitViaOtherEmail()
        {            
        }

        /// <summary>
        /// Sends the via notes.
        /// </summary>
        /// <param name="settings">Any nettings needed for creating emails</param>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <param name="fullFilename">The full filename.</param>
        /// <param name="emailAddress">fdgd fgdf</param>
        /// <param name="password">The password required to submit</param>
        public void Send(Settings settings, string month, string year, string fullFilename, string emailAddress, SecureString password = null)
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

            email.SendEmail(emailAddress, month, year, fullFilename);
        }
    }
}
