// -----------------------------------------------------------------------
// <copyright file="ISubmitter.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    using System.Security;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ISubmitter
    {
        /// <summary>
        /// Sends the via notes.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <param name="fullFilename">The full filename.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="password">The password.</param>
        void Send(Settings settings, string month, string year, string fullFilename, string emailAddress, SecureString password = null);
    }
}
