namespace TimesheetProgramWPF
{
    using System;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using TimesheetProgramLogic;

    /// <summary>
    /// Interaction logic for AddEditEntry.xaml
    /// </summary>
    public partial class AddEditEntry : Window
    {
        /// <summary>
        /// The timesheet
        /// </summary>
        private Timesheet timesheet;

        /// <summary>
        /// The entry ID
        /// </summary>
        private int entryID = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddEditEntry" /> class.
        /// </summary>
        /// <param name="timesheet">The timesheet.</param>
        public AddEditEntry(Timesheet timesheet)
        {
            InitializeComponent();
            CommonConstructor(timesheet);
            cboStartTime.SelectedIndex = 0;
            cboFinishTime.SelectedIndex = 0;
            cboPhaseCode.SelectedIndex = 0;
            datePicker.SelectedDate = DateTime.Today;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddEditEntry" /> class.
        /// </summary>
        /// <param name="entryToEdit">The entry to edit.</param>
        /// <param name="timesheet">The timesheet.</param>
        public AddEditEntry(Entry entryToEdit, Timesheet timesheet)
        {
            InitializeComponent();
            CommonConstructor(timesheet);
            datePicker.SelectedDate = entryToEdit.Date;
            txtProjectNumber.Text = entryToEdit.ProjectNumber.ToString();
            cboStartTime.Text = entryToEdit.StartTime.ToString("hh\\:mm");
            cboFinishTime.Text = entryToEdit.FinishTime.ToString("hh\\:mm");
            cboTaskCode.Text = entryToEdit.TaskCode;
            cboPhaseCode.Text = entryToEdit.PhaseCode;
            if (entryToEdit.Overhead)
            {
                cboOverhead.Text = "Yes";
            }
            else
            {
                cboOverhead.Text = "No";
            }

            cboBillable.Text = entryToEdit.Billable;
            txtDescription.Text = entryToEdit.Description;
            entryID = entryToEdit.ID;
        }

        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <value>
        /// The entry.
        /// </value>
        public Entry Entry
        {
            get;
            private set;
        }

        /// <summary>
        /// Commons the constructor.
        /// </summary>
        /// <param name="timesheet">The timesheet.</param>
        private void CommonConstructor(Timesheet timesheet)
        {
            for (int hour = 8; hour <= 18; hour++)
            {
                for (int minute = 0; minute < 60; minute = minute + 15)
                {
                    cboStartTime.Items.Add(hour.ToString("00") + ":" + minute.ToString("00"));
                    cboFinishTime.Items.Add(hour.ToString("00") + ":" + minute.ToString("00"));
                }
            }            

            foreach (string taskCode in Entry.TaskCodes)
            {
                cboTaskCode.Items.Add(taskCode);
            }

            foreach (string phaseCode in Entry.PhaseCodes)
            {
                cboPhaseCode.Items.Add(phaseCode);
            }

            this.Entry = null;
            this.timesheet = timesheet;
        }

        /// <summary>
        /// Calculates the total time.
        /// </summary>
        private void CalculateTotalTime()
        {
            bool validTime = true;            
            Match startMatch = Regex.Match(cboStartTime.Text, "^([0-1]\\d|2[0-3]|\\d):(00|15|30|45)$");
            Match finishMatch = Regex.Match(cboFinishTime.Text, "^([0-1]\\d|2[0-3]|\\d):(00|15|30|45)$");
            if (startMatch.Success && finishMatch.Success)
            {
                string startHours = startMatch.Groups[1].Value;
                string startMinutes = startMatch.Groups[2].Value;

                string finishHours = finishMatch.Groups[1].Value;
                string finishMinutes = finishMatch.Groups[2].Value;

                TimeSpan startTime = new TimeSpan(int.Parse(startHours), int.Parse(startMinutes), 0);
                TimeSpan finishTime = new TimeSpan(int.Parse(finishHours), int.Parse(finishMinutes), 0);

                if (startTime >= finishTime)
                {
                    validTime = false;
                }
                else
                {
                    TimeSpan totalTime = finishTime - startTime;
                    if (totalTime.Minutes == 0 || totalTime.Minutes == 30)
                    {                        
                        double totalTimeToDisplay = (double)totalTime.Hours + ((double)totalTime.Minutes / 60.0);
                        lblTotalTime.Content = totalTimeToDisplay.ToString("0.0");
                    }
                    else
                    {
                        validTime = false;
                    }
                }
            }
            else
            {
                validTime = false;
            }

            if (!validTime)
            {
                lblTotalTime.Content = "Invalid time";
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the cboStartTime control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void CboStartTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotalTime();
        }

        /// <summary>
        /// Handles the TextChanged event of the cboFinishTime control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void CboFinishTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotalTime();
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            int projectNumber;
            if (lblTotalTime.Content.Equals("Invalid time"))
            {
                MessageBox.Show("Invalid time", "Invalid Time", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (txtDescription.Text.Length > Entry.MAX_DESCRIPTION_LENGTH || txtDescription.Text.Length < 1)
            {
                MessageBox.Show("Description must be less than " + Entry.MAX_DESCRIPTION_LENGTH + " characters and non-empty.", "Invalid Description", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (txtDescription.Text.StartsWith("*") || txtDescription.Text.StartsWith("#"))
            {
                MessageBox.Show("Descriptions can not begin with a * or #.", "Invalid Description", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (int.TryParse(txtProjectNumber.Text, out projectNumber))
            {
                if ((projectNumber > 9999 || projectNumber < 1000) && projectNumber != 0)
                {
                    MessageBox.Show("Project number must be a four digit number", "Invalid Project Number", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cboTaskCode.Text.Length > 8 || cboTaskCode.Text.Length < 2)
                {
                    MessageBox.Show("Task code must be between 2 and 8 characters", "Invalid Task Code", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    bool overhead;
                    if (cboOverhead.Text.Equals("No"))
                    {
                        overhead = false;
                    }
                    else
                    {
                        overhead = true;
                    }

                    if (overhead && !cboBillable.Text.Equals("Yes"))
                    {
                        MessageBox.Show("Only billable tasks can have overheads", "Can't Have Overheads", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        if (entryID == -1)
                        {                            
                            foreach (Project project in timesheet.Projects)
                            {
                                foreach (Entry entry in project.Entries)
                                {
                                    if (entry.ID > entryID)
                                    {
                                        entryID = entry.ID;
                                    }
                                }
                            }

                            entryID++;
                        }

                        Entry = new Entry(entryID, (DateTime)datePicker.SelectedDate, projectNumber, TimeSpan.Parse(cboStartTime.Text), TimeSpan.Parse(cboFinishTime.Text), cboTaskCode.Text, cboPhaseCode.Text, overhead, cboBillable.Text, txtDescription.Text);
                        DialogResult = true;
                    }
                }
            }
            else
            {
                MessageBox.Show("Project number must be a four digit number", "Invalid Project Number", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }            
        }

        /// <summary>
        /// Handles the LostFocus event of the txtProjectNumber control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void TxtProjectNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtProjectNumber.Text.Equals("0") || txtProjectNumber.Text.Equals("00") || txtProjectNumber.Text.Equals("000"))
            {
                txtProjectNumber.Text = "0000";
            }
        }
    }
}
