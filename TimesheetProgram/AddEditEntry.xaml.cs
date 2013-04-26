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
        private Controller controller;

        /// <summary>
        /// The entry ID
        /// </summary>
        private int entryID = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddEditEntry" /> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public AddEditEntry(Controller controller)
        {
            InitializeComponent();
            CommonConstructor(controller);
            cboStartTime.SelectedIndex = 0;
            cboFinishTime.SelectedIndex = 0;
            cboPhaseCode.SelectedIndex = 0;
            datePicker.SelectedDate = DateTime.Today;
            this.Title = "Add Entry";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddEditEntry" /> class.
        /// </summary>
        /// <param name="entryToEdit">The entry to edit.</param>
        /// <param name="controller">The controller.</param>
        public AddEditEntry(Entry entryToEdit, Controller controller)
        {
            InitializeComponent();
            CommonConstructor(controller);
            SetupGUI(entryToEdit);
            this.Title = "Edit Entry";
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
        /// <param name="controller">The controller.</param>
        private void CommonConstructor(Controller controller)
        {
            PopulateComboBoxes();
            PopulateTaskCodes();
            PopulatePhaseCodes();

            this.Entry = null;
            this.controller = controller;            
        }

        /// <summary>
        /// Populates the phase codes.
        /// </summary>
        private void PopulatePhaseCodes()
        {
            foreach (string phaseCode in Entry.PhaseCodes)
            {
                cboPhaseCode.Items.Add(phaseCode);
            }
        }

        /// <summary>
        /// Populates the task codes.
        /// </summary>
        private void PopulateTaskCodes()
        {
            foreach (string taskCode in Entry.TaskCodes)
            {
                cboTaskCode.Items.Add(taskCode);
            }
        }

        /// <summary>
        /// Populates the combo boxes.
        /// </summary>
        private void PopulateComboBoxes()
        {
            for (int hour = 8; hour <= 18; hour++)
            {
                for (int minute = 0; minute < 60; minute = minute + 15)
                {
                    cboStartTime.Items.Add(hour.ToString("00") + ":" + minute.ToString("00"));
                    cboFinishTime.Items.Add(hour.ToString("00") + ":" + minute.ToString("00"));
                }
            }
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
                return;
            }
            
            if (txtDescription.Text.Length > Entry.MAX_DESCRIPTION_LENGTH || txtDescription.Text.Length < 1)
            {
                MessageBox.Show("Description must be less than " + Entry.MAX_DESCRIPTION_LENGTH + " characters and non-empty.", "Invalid Description", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            
            if (txtDescription.Text.StartsWith("*") || txtDescription.Text.StartsWith("#"))
            {
                MessageBox.Show("Descriptions can not begin with a * or #.", "Invalid Description", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            
            if (!int.TryParse(txtProjectNumber.Text, out projectNumber))
            {
                MessageBox.Show("Project number must be a four digit number", "Invalid Project Number", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }   

            if ((projectNumber > 9999 || projectNumber < 1000) && projectNumber != 0)
            {
                MessageBox.Show("Project number must be a four digit number", "Invalid Project Number", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
                
            if (cboTaskCode.Text.Length > 8 || cboTaskCode.Text.Length < 2)
            {
                MessageBox.Show("Task code must be between 2 and 8 characters", "Invalid Task Code", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            bool overhead = !cboOverhead.Text.Equals("No");            

            if (overhead && !cboBillable.Text.Equals("Yes"))
            {
                MessageBox.Show("Only billable tasks can have overheads", "Can't Have Overheads", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }                    
                                            
            if (entryID == -1)
            {
                entryID = controller.Manager.GetNextUnusedEntryID();                     
            }                        

            Entry = new Entry(entryID, (DateTime)datePicker.SelectedDate, projectNumber, TimeSpan.Parse(cboStartTime.Text), TimeSpan.Parse(cboFinishTime.Text), cboTaskCode.Text, cboPhaseCode.Text, overhead, cboBillable.Text, txtDescription.Text, false);
            DialogResult = true;                                             
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

        /// <summary>
        /// Setups the GUI.
        /// </summary>
        /// <param name="entry">The entry.</param>
        private void SetupGUI(Entry entry)
        {
            datePicker.SelectedDate = entry.Date;
            txtProjectNumber.Text = entry.ProjectNumber.ToString();
            cboStartTime.Text = entry.StartTime.ToString("hh\\:mm");
            cboFinishTime.Text = entry.FinishTime.ToString("hh\\:mm");
            cboTaskCode.Text = entry.TaskCode;
            cboPhaseCode.Text = entry.PhaseCode;
            if (entry.Overhead)
            {
                cboOverhead.Text = "Yes";
            }
            else
            {
                cboOverhead.Text = "No";
            }

            cboBillable.Text = entry.Billable;
            txtDescription.Text = entry.Description;
            entryID = entry.ID;
        }
    }
}
