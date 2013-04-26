// -----------------------------------------------------------------------
// <copyright file="Settings.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    using System.IO;
    using System.Security;
    using System.Xml.Serialization;

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
        public Settings(StaffID staffID, StaffNumber staffNumber, TCheckPath tCheckPath, TimesheetPath timesheetPath, bool submitViaNotes)
        {
            CommonConstructor(staffID, staffNumber, tCheckPath, timesheetPath, submitViaNotes);
            NotSubmitViaNotesConstructor(new SmtpServer(), new Port(), new EmailUsername(), false, new EmailAddress());            
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
        public Settings(StaffID staffID, StaffNumber staffNumber, TCheckPath tCheckPath, TimesheetPath timesheetPath, bool submitViaNotes, SmtpServer smtpServer, Port port, EmailUsername emailUsername, bool enableSSL, EmailAddress emailAddress)
        {
            CommonConstructor(staffID, staffNumber, tCheckPath, timesheetPath, submitViaNotes);
            NotSubmitViaNotesConstructor(smtpServer, port, emailUsername, enableSSL, emailAddress);
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
        public Settings(StaffID staffID, StaffNumber staffNumber, TCheckPath tCheckPath, TimesheetPath timesheetPath, bool submitViaNotes, SmtpServer smtpServer, Port port, EmailUsername emailUsername, bool enableSSL, EmailAddress emailAddress, SecureString password)
        {
            CommonConstructor(staffID, staffNumber, tCheckPath, timesheetPath, submitViaNotes);
            NotSubmitViaNotesConstructor(smtpServer, port, emailUsername, enableSSL, emailAddress);
            this.Password = Password;
        }

        /// <summary>
        /// Gets or sets the staff ID.
        /// </summary>
        /// <value>
        /// The staff ID.
        /// </value>
        public StaffID StaffID
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
        public StaffNumber StaffNumber
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
        public TCheckPath TCheckPath
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
        public TimesheetPath TimesheetPath
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
        public EmailAddress EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the email username.
        /// </summary>
        /// <value>
        /// The email username.
        /// </value>
        public EmailUsername EmailUsername { get; set; }

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
        public SmtpServer SmtpServer { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public Port Port { get; set; }

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
            return new Settings(new StaffID("WALDM"), new StaffNumber(699), new TCheckPath("K:\\Timesheets"), new TimesheetPath("C:\\Users\\WALDM\\Desktop"), true);
        }

        /// <summary>
        /// Reads the settings.
        /// </summary>
        public void Read()
        {
            Settings settings = Deserialize(SETTINGS_FILENAME);
            Update(settings);
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
        public void UpdateAndWrite(Settings newSettings)
        {
            Update(newSettings);
            Write();
        }

        /// <summary>
        /// Updates the specified new settings.
        /// </summary>
        /// <param name="newSettings">The new settings.</param>
        private void Update(Settings newSettings)
        {
            this.EmailAddress = newSettings.EmailAddress;
            this.EmailUsername = newSettings.EmailUsername;
            this.EnableSSL = newSettings.EnableSSL;
            this.Password = newSettings.Password;
            this.Port = newSettings.Port;
            this.SmtpServer = newSettings.SmtpServer;
            this.StaffID = newSettings.StaffID;
            this.StaffNumber = newSettings.StaffNumber;
            this.SubmitViaNotes = newSettings.SubmitViaNotes;
            this.TCheckPath = newSettings.TCheckPath;
            this.TimesheetPath = newSettings.TimesheetPath;
        }

        /// <summary>
        /// Deserializes the settings.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>The settings contained in the file</returns>
        private Settings Deserialize(string filename)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(Settings));
            try
            {
                using (TextReader reader = new StreamReader(filename))
                {
                    Settings settings = (Settings)deserializer.Deserialize(reader);
                    return settings;
                }
            }
            catch (FileNotFoundException)
            {
                Serialization.Serialize<Settings>(Settings.DefaultSettings(), filename);
                return Settings.DefaultSettings();
            }
        }

        /// <summary>
        /// Nots the submit via notes constructor.
        /// </summary>
        /// <param name="smtpServer">The SMTP server.</param>
        /// <param name="port">The port.</param>
        /// <param name="emailUsername">The email username.</param>
        /// <param name="enableSSL">if set to <c>true</c> [enable SSL].</param>
        /// <param name="emailAddress">The email address.</param>
        private void NotSubmitViaNotesConstructor(SmtpServer smtpServer, Port port, EmailUsername emailUsername, bool enableSSL, EmailAddress emailAddress)
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
        private void CommonConstructor(StaffID staffID, StaffNumber staffNumber, TCheckPath tCheckPath, TimesheetPath timesheetPath, bool submitViaNotes)
        {
            this.StaffID = staffID;
            this.StaffNumber = staffNumber;
            this.TCheckPath = tCheckPath;
            this.TimesheetPath = timesheetPath;
            this.SubmitViaNotes = submitViaNotes;
        }
    }
}
