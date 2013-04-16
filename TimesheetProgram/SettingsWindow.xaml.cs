namespace TimesheetProgramWPF
{
    using System.Windows;
    using TimesheetProgramLogic;

    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindow" /> class.
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindow" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public SettingsWindow(Settings settings)
        {
            InitializeComponent();
            txtStaffID.Text = settings.StaffID;
            txtStaffNumber.Text = settings.StaffNumber;
            txtTCheckPath.Text = settings.TCheckPath;
            txtTimesheetPath.Text = settings.TimesheetPath;

            txtEmailAddress.Text = settings.EmailAddress;
            txtEmailUsername.Text = settings.EmailUsername;
            txtPort.Text = settings.Port.ToString();
            txtSMTPServer.Text = settings.SmtpServer;
            chkEnableSSL.IsChecked = settings.EnableSSL;

            chkSubmitViaNotes.IsChecked = settings.SubmitViaNotes;

            chkSubmitViaNotesChanged();
        }

        /// <summary>
        /// The type of the folder
        /// </summary>
        private enum FolderType
        {
            /// <summary>
            /// The T check
            /// </summary>
            TCheck,
           
            /// <summary>
            /// The timesheet
            /// </summary>
            Timesheet
        }

        /// <summary>
        /// Gets the new settings.
        /// </summary>
        /// <value>
        /// The new settings.
        /// </value>
        public Settings NewSettings
        {
            get;
            private set;
        }

        /// <summary>
        /// Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            int staffNumber;
            string staffID = txtStaffID.Text;
            if (int.TryParse(txtStaffNumber.Text, out staffNumber))
            {
                if (staffNumber >= 1000 || staffNumber < 1)
                {
                    MessageBox.Show("Staff number must be less than 1000", "Invalid Staff Number", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (staffID.Length >= 6 || staffID.Equals(string.Empty))
                {
                    MessageBox.Show("Staff ID must be less than 6 characters", "Invalid Staff ID", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtTCheckPath.Text.Equals(string.Empty))
                {
                    MessageBox.Show("TCheck path cannot be empty", "Invalid TCheck Path", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtTimesheetPath.Text.Equals(string.Empty))
                {
                    MessageBox.Show("Timesheet path cannot be empty", "Invalid Timesheet Path", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (chkSubmitViaNotes.IsChecked == false)
                    {
                        if (txtEmailAddress.Text.Equals(string.Empty))
                        {
                            MessageBox.Show("Email address must be specified", "Invalid Email Address", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (txtEmailUsername.Text.Equals(string.Empty))
                        {
                            MessageBox.Show("Email username must be specified", "Invalid Email Username", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (txtSMTPServer.Text.Equals(string.Empty))
                        {
                            MessageBox.Show("SMTP server must be specified", "Invalid SMTP Server", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (txtPort.Text.Equals(string.Empty))
                        {
                            MessageBox.Show("Port must be specified", "Invalid Port", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else
                        {
                            DialogResult = true;
                        }
                    }
                    else
                    {
                        DialogResult = true;
                    }
                }                
            }
            else
            {
                MessageBox.Show("Invalid staff number", "Invalid Staff Number", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            if (DialogResult == true)            
            {
                NewSettings = new Settings();
                NewSettings.StaffID = txtStaffID.Text;
                NewSettings.StaffNumber = txtStaffNumber.Text;
                NewSettings.TCheckPath = txtTCheckPath.Text;
                NewSettings.TimesheetPath = txtTimesheetPath.Text;
                NewSettings.SubmitViaNotes = (bool)chkSubmitViaNotes.IsChecked;
                NewSettings.SmtpServer = txtSMTPServer.Text;
                NewSettings.Port = int.Parse(txtPort.Text);
                NewSettings.EmailUsername = txtEmailUsername.Text;
                NewSettings.EmailAddress = txtEmailAddress.Text;
                NewSettings.Password = null;
                NewSettings.EnableSSL = (bool)chkEnableSSL.IsChecked;
            }
        }

        /// <summary>
        /// Folders the chooser.
        /// </summary>
        /// <param name="type">The type.</param>
        private void folderChooser(FolderType type)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (type == FolderType.Timesheet)
                {
                    txtTimesheetPath.Text = folderBrowser.SelectedPath;
                }
                else if (type == FolderType.TCheck)
                {
                    txtTCheckPath.Text = folderBrowser.SelectedPath;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the btnBrowseTCheckPath control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnBrowseTCheckPath_Click(object sender, RoutedEventArgs e)
        {
            folderChooser(FolderType.TCheck);
        }

        /// <summary>
        /// Handles the Click event of the btnBrowseTimesheetPath control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnBrowseTimesheetPath_Click(object sender, RoutedEventArgs e)
        {
            folderChooser(FolderType.Timesheet);
        }

        /// <summary>
        /// Handles the Checked event of the chkSubmitViaNotes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void chkSubmitViaNotes_Checked(object sender, RoutedEventArgs e)
        {
            chkSubmitViaNotesChanged();
        }

        /// <summary>
        /// Handles the Unchecked event of the chkSubmitViaNotes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void chkSubmitViaNotes_Unchecked(object sender, RoutedEventArgs e)
        {
            chkSubmitViaNotesChanged();
        }

        /// <summary>
        /// CHKs the submit via notes changed.
        /// </summary>
        private void chkSubmitViaNotesChanged()
        {
            txtEmailAddress.IsEnabled = !(bool)chkSubmitViaNotes.IsChecked;
            txtEmailUsername.IsEnabled = !(bool)chkSubmitViaNotes.IsChecked;
            txtSMTPServer.IsEnabled = !(bool)chkSubmitViaNotes.IsChecked;
            txtPort.IsEnabled = !(bool)chkSubmitViaNotes.IsChecked;
            chkEnableSSL.IsEnabled = !(bool)chkSubmitViaNotes.IsChecked;
        }
    }
}
