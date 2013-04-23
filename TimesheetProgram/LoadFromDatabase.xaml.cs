namespace TimesheetProgramLogic
{
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Interaction logic for LoadFromDatabase.xaml
    /// </summary>
    public partial class LoadDeleteFromDatabase : Window
    {
        /// <summary>
        /// The timesheet data
        /// </summary>
        private List<TimesheetData> timesheetData;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadDeleteFromDatabase" /> class.
        /// </summary>
        /// <param name="formType">Type of the form.</param>
        public LoadDeleteFromDatabase(FormType formType)
        {
            InitializeComponent();

            if (formType == FormType.Delete)
            {
                btnLoad.Content = "Delete";
                Title = "Delete From Database";
            }

            timesheetData = Timesheet.GetAllTimesheetData();
            foreach (TimesheetData data in timesheetData)
            {
                cboTimesheetID.Items.Add(data.StaffNumber.ToString() + ": " + data.Month.ToString() + "/" + data.Year.ToString());
            }

            if (timesheetData.Count != 0)
            {
                cboTimesheetID.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// fdgfdg sdfgdgfd
        /// </summary>
        public enum FormType
        {
            /// <summary>
            /// The load
            /// </summary>
            Load,

            /// <summary>
            /// The delete
            /// </summary>
            Delete
        }

        /// <summary>
        /// Gets the timesheet ID.
        /// </summary>
        /// <value>
        /// The timesheet ID.
        /// </value>
        public int TimesheetID
        {
            get;
            private set;
        }

        /// <summary>
        /// Handles the Click event of the BtnLoad control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            TimesheetID = timesheetData[cboTimesheetID.SelectedIndex].ID;
            DialogResult = true;
        }
    }
}
