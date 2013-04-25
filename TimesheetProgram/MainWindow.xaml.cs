namespace TimesheetProgramWPF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Net.Mail;
    using System.Security;
    using System.Windows;
    using Microsoft.Win32;
    using TimesheetProgramLogic;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The controller
        /// </summary>
        private Controller controller;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {            
            InitializeComponent();
            controller = new Controller();                        
            UpdateGUI();
        }

        /// <summary>
        /// The type of the file
        /// </summary>
        private enum FileType
        {
            /// <summary>
            /// The XML
            /// </summary>
            XML,
            
            /// <summary>
            /// The build
            /// </summary>
            Build
        }

        /// <summary>
        /// Handles the Click event of the mnuBuildTimesheet control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MnuBuildTimesheet_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = SaveAsType(FileType.Build);
            if (dialog.ShowDialog() == true)
            {                
                controller.Manager.Build(dialog.FileName, controller.Settings.StaffID, controller.Settings.StaffNumber);
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuAddEntry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MnuAddEntry_Click(object sender, RoutedEventArgs e)
        {
            AddEntry();
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        private void AddEntry(Entry entry = null)
        {
            AddEditEntry newEntry;

            if (entry == null)
            {
                newEntry = new AddEditEntry(controller);
            }
            else
            {
                newEntry = new AddEditEntry(entry, controller);
            }

            if (newEntry.ShowDialog() == true)
            {
                try
                {
                    controller.Manager.AddEntry(newEntry.Entry);
                }
                catch (InvalidEntryTimeException)
                {
                    MessageBox.Show("There is already an entry that occurs at that time", "Invalid Time", MessageBoxButton.OK, MessageBoxImage.Error);
                    AddEntry(newEntry.Entry);
                    return;
                }
                catch (EntriesNotInSameMonthException)
                {
                    MessageBox.Show("All entries must be in the same month", "Invalid month", MessageBoxButton.OK, MessageBoxImage.Error);
                    AddEntry(newEntry.Entry);
                    return;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message, "Invalid Start Time", MessageBoxButton.OK, MessageBoxImage.Error);
                    AddEntry(newEntry.Entry);
                    return;
                }

                UpdateGUI();
            }
        }

        /// <summary>
        /// Update_datagrids this instance.
        /// </summary>
        private void UpdateDatagrid()
        {
            dataGrid.Items.Clear();
            List<Entry> entries = controller.Manager.Timesheet.Entries;
            entries.Sort(new SortEntriesViaDateTime());
            foreach (Entry entry in entries)
            {
                dataGrid.Items.Add(entry);
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuOpen control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MnuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == true)
            {
                try
                {
                    controller.Open(openFile.FileName);
                }
                catch (NullReferenceException)
                {
                    MessageBox.Show("Unable to open file", "Invalid File", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                UpdateGUI();
            }                       
        }

        /// <summary>
        /// Updates the GUI.
        /// </summary>
        private void UpdateGUI()
        {
            UpdateDatagrid();
            EnableControls();
            Title = "Timesheet Program - " + controller.Settings.StaffNumber + ": " + controller.Settings.StaffID + " - " + controller.Manager.Timesheet.Month.ToString("00") + "/" + controller.Manager.Timesheet.Year.ToString();
        }

        /// <summary>
        /// Enables the controls.
        /// </summary>
        private void EnableControls()
        {            
            bool timesheetContainsEntries = controller.Manager.Timesheet.Entries.Count != 0;
            mnuDeleteEntry.IsEnabled = timesheetContainsEntries;
            mnuEditEntry.IsEnabled = timesheetContainsEntries;
            mnuSave.IsEnabled = timesheetContainsEntries;
            mnuSaveAs.IsEnabled = timesheetContainsEntries;
            mnuBuildTimesheet.IsEnabled = timesheetContainsEntries;
            mnuTCheck.IsEnabled = timesheetContainsEntries;
            mnuSubmitTimesheet.IsEnabled = timesheetContainsEntries;
            mnuStatistics.IsEnabled = timesheetContainsEntries;

            if (timesheetContainsEntries)
            {
                dataGrid.SelectedIndex = 0;
            }            
        }

        /// <summary>
        /// Handles the Click event of the mnuSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MnuSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(controller.Settings);
            if (settingsWindow.ShowDialog() == true)
            {
                controller.Settings.UpdateAndWrite(settingsWindow.NewSettings);
                controller.Manager.Timesheet.UpdateStaffDetails(controller.Settings.StaffNumber, controller.Settings.StaffID);
                UpdateGUI();                
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuEditEntry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MnuEditEntry_Click(object sender, RoutedEventArgs e)
        {
            AddEditEntry editEntry = new AddEditEntry((Entry)dataGrid.SelectedItem, controller);                   

            try
            {
                if (editEntry.ShowDialog() == false)
                {
                    return;
                }
            }
            catch (InvalidPhaseCodeException)
            {
                MessageBox.Show("Invalid phase code entered", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                MnuEditEntry_Click(sender, e);
                return;
            }
             
            try
            {
                controller.Manager.EditEntry(editEntry.Entry);
            }
            catch (EntriesNotInSameMonthException)
            {
                MessageBox.Show("All entries must be in the same month", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                MnuEditEntry_Click(sender, e);
                return;
            }
            catch (InvalidEntryTimeException)
            {
                MessageBox.Show("There is already an entry with that start time", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                MnuEditEntry_Click(sender, e);
                return;
            }

            UpdateGUI();             
        }

        /// <summary>
        /// Handles the Click event of the mnuDeleteEntry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MnuDeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure that you want to delete the selected entry?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                controller.DeleteEntry((Entry)dataGrid.SelectedItem);
                UpdateGUI();
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuNewTimesheet control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MnuNewTimesheet_Click(object sender, RoutedEventArgs e)
        {
            bool createTimesheet = true;
            if (controller.Manager.UnsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save the current timesheet?", "Do you want to save?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    controller.Save();                    
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    createTimesheet = false;
                }
            }

            if (createTimesheet)            
            {
                controller.Manager.New();
                UpdateGUI();
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MnuSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                controller.Save();
            }
            catch (FilenameUnsetException)
            {
                MnuSaveAs_Click(sender, e);
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuSaveAs control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MnuSaveAs_Click(object sender, RoutedEventArgs e)
        {            
            SaveFileDialog saveAs = SaveAsType(FileType.XML);
            if (saveAs.ShowDialog() == true)
            {
                controller.SaveAs(saveAs.FileName);                
            }
        }

        /// <summary>
        /// Saves as type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A save file dialog</returns>
        private SaveFileDialog SaveAsType(FileType type)
        {
            SaveFileDialog saveAs = new SaveFileDialog();
            int month = controller.Manager.Timesheet.Month;
            if (month == 0)
            {
                month = DateTime.Today.Month;
            }            

            string fileExtension = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month).ToUpper();
            if (type == FileType.XML)
            {
                fileExtension += "X";
                saveAs.Filter = "XML Timesheets | *." + fileExtension;
            }
            else if (type == FileType.Build)
            {
                saveAs.Filter = "Timesheet Builds | *." + fileExtension;
            }

            saveAs.DefaultExt = fileExtension;
            saveAs.FileName = controller.Settings.StaffID;

            return saveAs;
        }

        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs" /> instance containing the event data.</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (controller.Manager.UnsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save the current timesheet?", "Do you want to save?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    controller.Save();                    
                }   
            }           
        }

        /// <summary>
        /// Handles the Click event of the mnuRunTCheck control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MnuRunTCheck_Click(object sender, RoutedEventArgs e)
        {
            controller.RunTCheck();
        }

        /// <summary>
        /// Handles the Click event of the mnuSubmitTCheck control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MnuSubmitTCheck_Click(object sender, RoutedEventArgs e)
        {
            Submit(new TCheck());
        }

        /// <summary>
        /// Gets the password if required.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>Whether or not the email should be submitted</returns>
        private bool GetPasswordIfRequired(out SecureString password)
        {
            password = null;
            if (controller.Settings.SubmitViaNotes)
            {
                return true;
            }
            else if (controller.Settings.Password != null)
            {
                password = controller.Settings.Password;
                return true;
            }
            else            
            {
                PasswordDialog passwordDialog = new PasswordDialog();
                if (passwordDialog.ShowDialog() == true)
                {
                    password = passwordDialog.Password;
                    return true;
                }
                else
                {
                    return false;
                }
            }            
        }

        /// <summary>
        /// Submits the specified submittable.
        /// </summary>
        /// <param name="submittable">The submittable.</param>
        private void Submit(ASubmittable submittable)
        {            
            SecureString password = null;            
            if (GetPasswordIfRequired(out password))
            {
                try
                {
                    controller.Submit(submittable, password);
                }
                catch (SmtpException ex)
                {
                    string message = string.Empty;
                    Exception lEx = ex;

                    while (lEx.InnerException != null)
                    {
                        message += lEx.InnerException.Message + "\n";
                        lEx = lEx.InnerException;
                    }

                    MessageBox.Show(message, "Error Submitting Timesheet", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuSubmitTimesheet control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MnuSubmitTimesheet_Click(object sender, RoutedEventArgs e)
        {
            Submit(controller.Manager.Timesheet);
        }

        /// <summary>
        /// Handles the Click event of the MnuMonthlySummary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MnuMonthlySummary_Click(object sender, RoutedEventArgs e)
        {
            MonthlySummary summary = Statistics.CalculateMonthlySummary(controller.Manager.Timesheet);
            MessageBox.Show("Number of days worked so far: " + summary.NumberOfDaysWorkedSoFar + "\nTotal hours so far: " + summary.TotalHoursWorkedSoFar + "\nAverage hours per day: " + summary.AverageHoursPerDay + "\nExpected hours per day: " + summary.ExpectedHoursPerDay, "Monthly Summary", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Handles the Click event of the MnuWeeklySummary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MnuWeeklySummary_Click(object sender, RoutedEventArgs e)
        {
            WeeklySummary summary = Statistics.CalculateWeeklySummary(controller.Manager.Timesheet);
            MessageBox.Show("Number of days worked so far: " + summary.NumberOfDaysWorkedSoFar + "\nTotal hours so far: " + summary.TotalHoursWorkedSoFar + "\nNumber of weeks worked so far: " + summary.NumberOfWeeksWorkedSoFar + "\nNumber of hours per week: " + summary.NumberOfHoursPerWeek + "\nExpected hours per week: " + summary.ExpectedHoursPerWeek, "Weekly Summary", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
