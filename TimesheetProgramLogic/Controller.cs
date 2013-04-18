// -----------------------------------------------------------------------
// <copyright file="Controller.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Security;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Controller
    {        
        /// <summary>
        /// The filename
        /// </summary>
        private string filename = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller" /> class.
        /// </summary>
        public Controller()
        {
            Entries = new ObservableCollection<Entry>();
            Settings = new Settings();
            Settings.Read();
            UnsavedChanges = false;
        }

        /// <summary>
        /// Gets the entries.
        /// </summary>
        /// <value>
        /// The entries.
        /// </value>
        public ObservableCollection<Entry> Entries
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public Settings Settings
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the month.
        /// </summary>
        /// <value>
        /// The month.
        /// </value>
        public int Month
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        /// <value>
        /// The year.
        /// </value>
        public int Year
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [unsaved changes].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [unsaved changes]; otherwise, <c>false</c>.
        /// </value>
        public bool UnsavedChanges
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the staff number.
        /// </summary>
        /// <value>
        /// The staff number.
        /// </value>
        public string StaffNumber
        {
            get
            {
                return Settings.StaffNumber;
            }
        }

        /// <summary>
        /// Gets the staff ID.
        /// </summary>
        /// <value>
        /// The staff ID.
        /// </value>
        public string StaffID
        {
            get
            {
                return Settings.StaffID;
            }
        }

        /// <summary>
        /// Runs the T check.
        /// </summary>
        public void RunTCheck()
        {
            TCheck.Run(Settings, Month.ToString("00"), (Year - 2000).ToString());
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="newEntry">The new entry.</param>
        public void AddEntry(Entry newEntry)
        {
            Timesheet.AddEntry(this, newEntry);
        }

        /// <summary>
        /// Edits the entry.
        /// </summary>
        /// <param name="entryToEdit">The entry to edit.</param>
        /// <param name="editedEntry">The edited entry.</param>
        public void EditEntry(Entry entryToEdit, Entry editedEntry)
        {
            Timesheet.EditEntry(this, entryToEdit, editedEntry);
        }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public void DeleteEntry(Entry entry)
        {
            Timesheet.DeleteEntry(this, entry);
        }

        /// <summary>
        /// Submits the T check.
        /// </summary>
        /// <param name="submitable">The submitable.</param>
        /// <param name="password">The password.</param>
        /// <exception cref="UnableToSubmitEmailException">Occurs if unable to submit an email</exception>        
        public void Submit(ASubmittable submitable, SecureString password = null)
        {
            string sMonth = Month.ToString("00");

            // Don't want to submit XML file, want to submit build
            if (isXmlFilename(filename))
            {
                submitable.Submitter.Send(Settings, StaffID, sMonth, Year.ToString(), filename, password);                
            }
            else
            {
                throw new UnableToSubmitEmailException();
            }
        }

        /// <summary>
        /// Saves as.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void SaveAs(string filename)
        {
            this.filename = filename;
            Save();
        }

        /// <summary>
        /// Builds the timesheet.
        /// </summary>
        /// <param name="buildFilename">The build filename.</param>
        public void BuildTimesheet(string buildFilename)
        {
            new Timesheet(Entries, buildFilename, StaffID, StaffNumber).Build();
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <exception cref="FilenameUnsetException">Occurs when the filename is not set</exception>
        public void Save()
        {
            if (filename.Equals(string.Empty))
            {
                throw new FilenameUnsetException();
            }
            else
            {                
                if (!isXmlFilename(filename))
                {
                    filename = filename + "X";
                }

                Serialization.Serialize(Entries, filename);
            }
        }

        /// <summary>
        /// News the timesheet.
        /// </summary>
        public void NewTimesheet()
        {
            Timesheet.New(this);
        }

        /// <summary>
        /// Opens the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Open(string filename)
        {
            this.filename = filename;
            Entries.Clear();  
            if (isXmlFilename(filename))
            {
                Timesheet.ReadXML(this, filename);
            }
            else
            {
                Timesheet.ReadBuild(this, filename);
            }

            UnsavedChanges = false;
            Month = Entries[0].Date.Month;
            Year = Entries[0].Date.Year;
        }

        /// <summary>
        /// Determines whether [is XML filename] [the specified filename].
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>
        ///   <c>true</c> if [is XML filename] [the specified filename]; otherwise, <c>false</c>.
        /// </returns>
        private bool isXmlFilename(string filename)
        {
            if (filename.EndsWith("X"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
