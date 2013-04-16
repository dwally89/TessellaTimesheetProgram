namespace TimesheetProgramWPF
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Net.Mail;
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
            dataGrid.ItemsSource = controller.Entries;                                     
            Title = Title + " - " + controller.StaffNumber + ": " + controller.StaffID;            
            enableControls();
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
        private void mnuBuildTimesheet_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = saveAsType(FileType.Build);
            if (dialog.ShowDialog() == true)
            {
                controller.BuildTimesheet(dialog.FileName);
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuAddEntry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void mnuAddEntry_Click(object sender, RoutedEventArgs e)
        {
            AddEditEntry newEntry = new AddEditEntry();
            
            if (newEntry.ShowDialog() == true)
            {
                controller.AddEntry(newEntry.Entry);
                enableControls();
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuOpen control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void mnuOpen_Click(object sender, RoutedEventArgs e)
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

                enableControls();            
            }                       
        }

        /// <summary>
        /// Enables the controls.
        /// </summary>
        private void enableControls()
        {           
            mnuDeleteEntry.IsEnabled = controller.Entries.Count != 0;
            mnuEditEntry.IsEnabled = controller.Entries.Count != 0;
            mnuSave.IsEnabled = controller.Entries.Count != 0;
            mnuSaveAs.IsEnabled = controller.Entries.Count != 0;
            mnuBuildTimesheet.IsEnabled = controller.Entries.Count != 0;
            mnuTCheck.IsEnabled = controller.Entries.Count != 0;
            if (controller.Entries.Count != 0)
            {
                dataGrid.SelectedIndex = 0;
            }

            mnuSubmitTimesheet.IsEnabled = controller.Entries.Count != 0;
        }

        /// <summary>
        /// Handles the Click event of the mnuSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void mnuSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(controller.Settings);
            if (settingsWindow.ShowDialog() == true)
            {
                controller.Settings.Update(settingsWindow.NewSettings);
                Title = "Timesheet Program - " + controller.StaffNumber + ": " + controller.StaffID;
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuEditEntry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void mnuEditEntry_Click(object sender, RoutedEventArgs e)
        {
             AddEditEntry newEntry = new AddEditEntry((Entry)dataGrid.SelectedItem);

             if (newEntry.ShowDialog() == true)
             {
                 controller.EditEntry((Entry)dataGrid.SelectedItem, newEntry.Entry);
             }
        }

        /// <summary>
        /// Handles the Click event of the mnuDeleteEntry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void mnuDeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure that you want to delete the selected entry?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                controller.DeleteEntry((Entry)dataGrid.SelectedItem);                
                enableControls();
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuNewTimesheet control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void mnuNewTimesheet_Click(object sender, RoutedEventArgs e)
        {
            bool createTimesheet = true;
            if (controller.UnsavedChanges)
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
                controller.NewTimesheet();
                enableControls();
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void mnuSave_Click(object sender, RoutedEventArgs e)
        {
            controller.Save();
        }

        /// <summary>
        /// Handles the Click event of the mnuSaveAs control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void mnuSaveAs_Click(object sender, RoutedEventArgs e)
        {            
            SaveFileDialog saveAs = saveAsType(FileType.XML);
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
        private SaveFileDialog saveAsType(FileType type)
        {
            SaveFileDialog saveAs = new SaveFileDialog();
            string fileExtension = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(controller.Month).ToUpper();
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
            saveAs.FileName = controller.StaffID;

            return saveAs;
        }

        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs" /> instance containing the event data.</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (controller.UnsavedChanges)
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
        private void mnuRunTCheck_Click(object sender, RoutedEventArgs e)
        {
            controller.RunTCheck();
        }

        /// <summary>
        /// Handles the Click event of the mnuSubmitTCheck control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void mnuSubmitTCheck_Click(object sender, RoutedEventArgs e)
        {
            if (controller.Settings.SubmitViaNotes)
            {
                controller.Submit(new TCheck());
            }
            else if (controller.Settings.Password == null)
            {
                PasswordDialog passwordDialog = new PasswordDialog();
                if (passwordDialog.ShowDialog() == true)
                {
                    try
                    {
                        controller.Submit(new TCheck(), passwordDialog.Password);
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

                        MessageBox.Show(message, "Error Submitting TCheck", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuSubmitTimesheet control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void mnuSubmitTimesheet_Click(object sender, RoutedEventArgs e)
        {
            if (controller.Settings.Password == null && !controller.Settings.SubmitViaNotes)
            {
                PasswordDialog passwordDialog = new PasswordDialog();
                if (passwordDialog.ShowDialog() == true)
                {
                    try
                    {
                        controller.Submit(new Timesheet(), passwordDialog.Password);
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
            else
            {
                controller.Submit(new Timesheet());
            }
        }
    }
}
