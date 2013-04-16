// -----------------------------------------------------------------------
// <copyright file="ISubmittable.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    using System.Security;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ISubmittable
    {
        /// <summary>
        /// Sends the via notes.
        /// </summary>
        /// <param name="staffID">The staff ID.</param>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <param name="fullFilename">The full filename.</param>
        void SendViaNotes(string staffID, string month, string year, string fullFilename);

        /// <summary>
        /// Sends the via other email.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="sMonth">The s month.</param>
        /// <param name="year">The year.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="password">The password.</param>
        void SendViaOtherEmail(Settings settings, string sMonth, string year, string filename, SecureString password = null);
    }
}
