namespace TimesheetProgramWPF
{
    using System;
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
            SetupGUI(settings);
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
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            NewSettings = new Settings();

            try
            {
                NewSettings.StaffNumber = new StaffNumber(txtStaffNumber.Text);
                NewSettings.StaffID = new StaffID(txtStaffID.Text);
                NewSettings.TCheckPath = new TCheckPath(txtTCheckPath.Text);
                NewSettings.TimesheetPath = new TimesheetPath(txtTimesheetPath.Text);

                if (chkSubmitViaNotes.IsChecked == false)
                {
                    NewSettings.EmailAddress = new EmailAddress(txtEmailAddress.Text);
                    NewSettings.EmailUsername = new EmailUsername(txtEmailUsername.Text);
                    NewSettings.SmtpServer = new SmtpServer(txtSMTPServer.Text);
                    NewSettings.Port = new Port(txtPort.Text);
                }

                NewSettings.SubmitViaNotes = (bool)chkSubmitViaNotes.IsChecked;
                NewSettings.Password = null;
                NewSettings.EnableSSL = (bool)chkEnableSSL.IsChecked;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Invalid Setting", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            DialogResult = true;            
        }

        /// <summary>
        /// Folders the chooser.
        /// </summary>
        /// <param name="type">The type.</param>
        private void FolderChooser(FolderType type)
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
        private void BtnBrowseTCheckPath_Click(object sender, RoutedEventArgs e)
        {
            FolderChooser(FolderType.TCheck);
        }

        /// <summary>
        /// Handles the Click event of the btnBrowseTimesheetPath control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void BtnBrowseTimesheetPath_Click(object sender, RoutedEventArgs e)
        {
            FolderChooser(FolderType.Timesheet);
        }

        /// <summary>
        /// Handles the Checked event of the chkSubmitViaNotes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ChkSubmitViaNotes_Checked(object sender, RoutedEventArgs e)
        {
            ChkSubmitViaNotesChanged();
        }

        /// <summary>
        /// Handles the Unchecked event of the chkSubmitViaNotes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ChkSubmitViaNotes_Unchecked(object sender, RoutedEventArgs e)
        {
            ChkSubmitViaNotesChanged();
        }

        /// <summary>
        /// CHKs the submit via notes changed.
        /// </summary>
        private void ChkSubmitViaNotesChanged()
        {
            txtEmailAddress.IsEnabled = !(bool)chkSubmitViaNotes.IsChecked;
            txtEmailUsername.IsEnabled = !(bool)chkSubmitViaNotes.IsChecked;
            txtSMTPServer.IsEnabled = !(bool)chkSubmitViaNotes.IsChecked;
            txtPort.IsEnabled = !(bool)chkSubmitViaNotes.IsChecked;
            chkEnableSSL.IsEnabled = !(bool)chkSubmitViaNotes.IsChecked;
        }

        /// <summary>
        /// Setups the GUI.
        /// </summary>
        /// <param name="settings">The settings.</param>
        private void SetupGUI(Settings settings)
        {
            SetupGeneralTab(settings);
            SetupEmailTab(settings);
        }

        /// <summary>
        /// Setups the email tab.
        /// </summary>
        /// <param name="settings">The settings.</param>
        private void SetupEmailTab(Settings settings)
        {
            txtEmailAddress.Text = settings.EmailAddress.Value;
            txtEmailUsername.Text = settings.EmailUsername.Value;
            txtPort.Text = settings.Port.Value.ToString();
            txtSMTPServer.Text = settings.SmtpServer.Value;
            chkEnableSSL.IsChecked = settings.EnableSSL;

            chkSubmitViaNotes.IsChecked = settings.SubmitViaNotes;

            ChkSubmitViaNotesChanged();
        }

        /// <summary>
        /// Setups the general tab.
        /// </summary>
        /// <param name="settings">The settings.</param>
        private void SetupGeneralTab(Settings settings)
        {
            txtStaffID.Text = settings.StaffID.ID;
            txtStaffNumber.Text = settings.StaffNumber.ToString();
            txtTCheckPath.Text = settings.TCheckPath.Value;
            txtTimesheetPath.Text = settings.TimesheetPath.Value;
        }
    }
}
