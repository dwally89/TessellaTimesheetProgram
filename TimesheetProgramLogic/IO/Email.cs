namespace TimesheetProgramLogic
{
    using System.Net.Mail;
    using System.Security;

    /// <summary>
    /// The class used for emails
    /// </summary>
    public class Email
    {
        /// <summary>
        /// The username
        /// </summary>
        private string username, smtpServer, staffID, fromEmailAddress;

        /// <summary>
        /// The password
        /// </summary>
        private SecureString password;
       
        /// <summary>
        /// The port
        /// </summary>
        private int port;
        
        /// <summary>
        /// The enable SSL
        /// </summary>
        private bool enableSSL;

        /// <summary>
        /// Initializes a new instance of the <see cref="Email" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public Email(Settings settings)
        {
            CommonConstructor(settings);
            this.password = settings.Password;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Email" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="password">The password.</param>
        public Email(Settings settings, SecureString password)
        {
            CommonConstructor(settings);
            this.password = password;
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <param name="fullFilename">The full filename.</param>
        public void SendEmail(string toEmailAddress, string month, string year, string fullFilename)
        {
            // Two digit month and four digit year
            
            // Method 2
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient(this.smtpServer);
            mail.From = new MailAddress(fromEmailAddress);
            mail.To.Add(toEmailAddress);
            mail.Subject = staffID + " " + month + "/" + year;            
                        
            mail.Attachments.Add(new System.Net.Mail.Attachment(fullFilename));

            smtpServer.Port = port;
            smtpServer.Credentials = new System.Net.NetworkCredential(username, password);
            smtpServer.EnableSsl = enableSSL;

            smtpServer.Send(mail);            
        }

        /// <summary>
        /// Commons the constructor.
        /// </summary>
        /// <param name="settings">The settings.</param>
        private void CommonConstructor(Settings settings)
        {
            this.smtpServer = settings.SmtpServer.Value;
            this.staffID = settings.StaffID.ID;
            this.port = int.Parse(settings.Port.Value);
            this.username = settings.EmailUsername.Value;
            this.enableSSL = settings.EnableSSL;
            this.fromEmailAddress = settings.EmailAddress.Value;
        }
    }
}