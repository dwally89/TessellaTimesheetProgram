// -----------------------------------------------------------------------
// <copyright file="Settings.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    using System.Security;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// The SETTING s_ FILENAME
        /// </summary>
        private const string SETTINGS_FILENAME = "Settings.settings";

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings" /> class.
        /// </summary>
        /// <param name="staffID">The staff ID.</param>
        /// <param name="staffNumber">The staff number.</param>
        /// <param name="tCheckPath">The t check path.</param>
        /// <param name="timesheetPath">The timesheet path.</param>
        /// <param name="submitViaNotes">if set to <c>true</c> [submit via notes].</param>
        public Settings(string staffID, string staffNumber, string tCheckPath, string timesheetPath, bool submitViaNotes)
        {
            commonConstructor(staffID, staffNumber, tCheckPath, timesheetPath, submitViaNotes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings" /> class.
        /// </summary>
        public Settings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings" /> class.
        /// </summary>
        /// <param name="staffID">The staff ID.</param>
        /// <param name="staffNumber">The staff number.</param>
        /// <param name="tCheckPath">The t check path.</param>
        /// <param name="timesheetPath">The timesheet path.</param>
        /// <param name="submitViaNotes">if set to <c>true</c> [submit via notes].</param>
        /// <param name="smtpServer">The SMTP server.</param>
        /// <param name="port">The port.</param>
        /// <param name="emailUsername">The email username.</param>
        /// <param name="enableSSL">if set to <c>true</c> [enable SSL].</param>
        /// <param name="emailAddress">The email address.</param>
        public Settings(string staffID, string staffNumber, string tCheckPath, string timesheetPath, bool submitViaNotes, string smtpServer, int port, string emailUsername, bool enableSSL, string emailAddress)
        {
            commonConstructor(staffID, staffNumber, tCheckPath, timesheetPath, submitViaNotes);
            notSubmitViaNotesConstructor(smtpServer, port, emailUsername, enableSSL, emailAddress);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings" /> class.
        /// </summary>
        /// <param name="staffID">The staff ID.</param>
        /// <param name="staffNumber">The staff number.</param>
        /// <param name="tCheckPath">The t check path.</param>
        /// <param name="timesheetPath">The timesheet path.</param>
        /// <param name="submitViaNotes">if set to <c>true</c> [submit via notes].</param>
        /// <param name="smtpServer">The SMTP server.</param>
        /// <param name="port">The port.</param>
        /// <param name="emailUsername">The email username.</param>
        /// <param name="enableSSL">if set to <c>true</c> [enable SSL].</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="password">The password.</param>
        public Settings(string staffID, string staffNumber, string tCheckPath, string timesheetPath, bool submitViaNotes, string smtpServer, int port, string emailUsername, bool enableSSL, string emailAddress, string password)
        {
            commonConstructor(staffID, staffNumber, tCheckPath, timesheetPath, submitViaNotes);
            notSubmitViaNotesConstructor(smtpServer, port, emailUsername, enableSSL, emailAddress);
            this.Password = Password;
        }

        /// <summary>
        /// Gets or sets the staff ID.
        /// </summary>
        /// <value>
        /// The staff ID.
        /// </value>
        public string StaffID
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
        public string StaffNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the T check path.
        /// </summary>
        /// <value>
        /// The T check path.
        /// </value>
        public string TCheckPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the timesheet path.
        /// </summary>
        /// <value>
        /// The timesheet path.
        /// </value>
        public string TimesheetPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [submit via notes].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [submit via notes]; otherwise, <c>false</c>.
        /// </value>
        public bool SubmitViaNotes { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the email username.
        /// </summary>
        /// <value>
        /// The email username.
        /// </value>
        public string EmailUsername { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public SecureString Password { get; set; }

        /// <summary>
        /// Gets or sets the SMTP server.
        /// </summary>
        /// <value>
        /// The SMTP server.
        /// </value>
        public string SmtpServer { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable SSL].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable SSL]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableSSL { get; set; }
                
        /// <summary>
        /// Defaults the settings.
        /// </summary>
        /// <returns>The default settings</returns>
        public static Settings DefaultSettings()
        {
            return new Settings("WALDM", "699", "K:\\Timesheets", "C:\\Users\\WALDM\\Desktop", true);
        }

        /// <summary>
        /// Reads the settings.
        /// </summary>
        public void Read()
        {
            Settings settings = Serialization.DeserializeSettings(SETTINGS_FILENAME);
            this.EmailAddress = settings.EmailAddress;
            this.EmailUsername = settings.EmailUsername;
            this.EnableSSL = settings.EnableSSL;
            this.Password = settings.Password;
            this.Port = settings.Port;
            this.SmtpServer = settings.SmtpServer;
            this.StaffID = settings.StaffID;
            this.StaffNumber = settings.StaffNumber;
            this.SubmitViaNotes = settings.SubmitViaNotes;
            this.TCheckPath = settings.TCheckPath;
            this.TimesheetPath = settings.TimesheetPath;
        }

        /// <summary>
        /// Writes the settings.
        /// </summary>
        public void Write()
        {
            Serialization.Serialize(this, SETTINGS_FILENAME);
        }

        /// <summary>
        /// Updates the settings.
        /// </summary>
        /// <param name="newSettings">The new settings.</param>
        public void Update(Settings newSettings)
        {
            this.StaffID = newSettings.StaffID;
            this.StaffNumber = newSettings.StaffNumber;
            this.TCheckPath = newSettings.TCheckPath;
            this.TimesheetPath = newSettings.TimesheetPath;
            this.SubmitViaNotes = newSettings.SubmitViaNotes;
            this.SmtpServer = newSettings.SmtpServer;
            this.Port = newSettings.Port;
            this.EmailUsername = newSettings.EmailUsername;
            this.EmailAddress = newSettings.EmailAddress;
            this.Password = newSettings.Password;
            this.EnableSSL = newSettings.EnableSSL;
            Write();
        }

        /// <summary>
        /// Nots the submit via notes constructor.
        /// </summary>
        /// <param name="smtpServer">The SMTP server.</param>
        /// <param name="port">The port.</param>
        /// <param name="emailUsername">The email username.</param>
        /// <param name="enableSSL">if set to <c>true</c> [enable SSL].</param>
        /// <param name="emailAddress">The email address.</param>
        private void notSubmitViaNotesConstructor(string smtpServer, int port, string emailUsername, bool enableSSL, string emailAddress)
        {
            this.SmtpServer = smtpServer;
            this.Port = port;
            this.EmailUsername = emailUsername;
            this.EnableSSL = enableSSL;
            this.EmailAddress = emailAddress;
        }

        /// <summary>
        /// Commons the constructor.
        /// </summary>
        /// <param name="staffID">The staff ID.</param>
        /// <param name="staffNumber">The staff number.</param>
        /// <param name="tCheckPath">The t check path.</param>
        /// <param name="timesheetPath">The timesheet path.</param>
        /// <param name="submitViaNotes">if set to <c>true</c> [submit via notes].</param>
        private void commonConstructor(string staffID, string staffNumber, string tCheckPath, string timesheetPath, bool submitViaNotes)
        {
            this.StaffID = staffID;
            this.StaffNumber = staffNumber;
            this.TCheckPath = tCheckPath;
            this.TimesheetPath = timesheetPath;
            this.SubmitViaNotes = submitViaNotes;
        }
    }
}
