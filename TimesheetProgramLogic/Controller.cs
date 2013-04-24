// -----------------------------------------------------------------------
// <copyright file="Controller.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
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
            Settings = new Settings();
            Settings.Read();
            Timesheet = new Timesheet(Settings.StaffNumber, Settings.StaffID);                        
        }

        /// <summary>
        /// Gets the timesheet.
        /// </summary>
        /// <value>
        /// The timesheet.
        /// </value>
        public Timesheet Timesheet
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
        /// Gets the submitter.
        /// </summary>
        /// <value>
        /// The submitter.
        /// </value>
        public ISubmitter Submitter
        {
            get
            {
                if (Settings.SubmitViaNotes)
                {
                    return new SubmitViaNotes();
                }
                else
                {
                    return new SubmitViaOtherEmail();
                }
            }
        }

        /// <summary>
        /// Runs the T check.
        /// </summary>
        public void RunTCheck()
        {
            TCheck.Run(Settings, Timesheet.Month.ToString("00"), (Timesheet.Year - 2000).ToString());
        }

        /// <summary>
        /// Submits the T check.
        /// </summary>
        /// <param name="submitable">The submitable.</param>
        /// <param name="password">The password.</param>
        /// <exception cref="UnableToSubmitEmailException">Occurs if unable to submit an email</exception>        
        public void Submit(ASubmittable submitable, SecureString password = null)
        {
            string sMonth = Timesheet.Month.ToString("00");

            // Don't want to submit XML file, want to submit build
            if (!Util.IsXmlFilename(filename))
            {
                Submitter.Send(Settings, sMonth, Timesheet.Year.ToString(), filename, submitable.EmailAddress, password);                
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
                if (!Util.IsXmlFilename(filename))
                {
                    filename = filename + "X";
                }

                Serialization.Serialize(Timesheet, filename);
            }
        }

        /// <summary>
        /// News the timesheet.
        /// </summary>
        public void NewTimesheet()
        {
            Timesheet.New();
        }

        /// <summary>
        /// Opens the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Open(string filename)
        {
            this.filename = filename;            
            if (Util.IsXmlFilename(filename))
            {
                Timesheet.ReadXML(filename);
            }
            else
            {
                Timesheet.ReadBuild(filename);
            }            

            Timesheet.UnsavedChanges = false;
        }

        /// <summary>
        /// Opens the timesheet under temporary staff ID.
        /// </summary>
        /// <param name="staffNumber">The staff number.</param>
        /// <param name="filename">The filename.</param>
        public void OpenTimesheetUnderTemporaryStaffID(int staffNumber, string filename)
        {
            Settings.StaffID = "TEMP";
            Settings.StaffNumber = staffNumber;
            Timesheet.UpdateStaffDetails(staffNumber, "TEMP");
            try
            {
                Open(filename);
            }
            catch (SqlException)
            {
                OpenTimesheetUnderTemporaryStaffID(staffNumber - 1, filename);
            }
        }

        /// <summary>
        /// Deletes the timesheet.
        /// </summary>
        /// <param name="timesheetID">The timesheet ID.</param>
        /// <exception cref="TimesheetProgramLogic.CannotDeleteDefaultBlankTimesheetException">dfgdfgdfgdf sdfgfdgfd</exception>
        public void DeleteTimesheet(int timesheetID)
        {            
            if (Timesheet.Month == 0 && Timesheet.Year == 0)
            {
                throw new CannotDeleteDefaultBlankTimesheetException();
            }            
        }
    }
}
