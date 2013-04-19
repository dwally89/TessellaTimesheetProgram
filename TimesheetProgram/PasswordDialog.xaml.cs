namespace TimesheetProgramLogic
{
    using System.Security;
    using System.Windows;

    /// <summary>
    /// Interaction logic for PasswordDialog.xaml
    /// </summary>
    public partial class PasswordDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordDialog" /> class.
        /// </summary>
        public PasswordDialog()
        {
            InitializeComponent();
            txtPassword.Focus();
        }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public SecureString Password
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
            this.Password = txtPassword.SecurePassword;
            DialogResult = true;
        }
    }
}
