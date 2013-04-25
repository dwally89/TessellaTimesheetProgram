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
            this.Manager = new LinqTimesheetManager(new Timesheet(Settings.StaffNumber, Settings.StaffID));            
        }

        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <value>
        /// The manager.
        /// </value>
        public ATimesheetManager Manager { get; private set; }

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
            TCheck.Run(Settings, Manager.Timesheet.Month.ToString("00"), (Manager.Timesheet.Year - 2000).ToString());
        }

        /// <summary>
        /// Submits the T check.
        /// </summary>
        /// <param name="submitable">The submitable.</param>
        /// <param name="password">The password.</param>
        /// <exception cref="UnableToSubmitEmailException">Occurs if unable to submit an email</exception>        
        public void Submit(ASubmittable submitable, SecureString password = null)
        {
            string sMonth = Manager.Timesheet.Month.ToString("00");

            // Don't want to submit XML file, want to submit build
            if (!Util.IsXmlFilename(filename))
            {
                Submitter.Send(Settings, sMonth, Manager.Timesheet.Year.ToString(), filename, submitable.EmailAddress, password);                
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

                Serialization.Serialize(Manager.Timesheet, filename);
            }
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
                Manager.ReadXML(filename);
            }
            else
            {
                Manager.ReadBuild(filename);
            }                       
        }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public void DeleteEntry(Entry entry)
        {
            Manager.DeleteEntry(entry);
        }
    }
}
