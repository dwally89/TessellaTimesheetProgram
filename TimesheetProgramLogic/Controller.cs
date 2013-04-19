﻿// -----------------------------------------------------------------------
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
            Settings = new Settings();
            Settings.Read();
            Timesheet = new Timesheet(Settings.SubmitViaNotes);
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
        public void Submit(ISubmittable submitable, SecureString password = null)
        {
            string sMonth = Timesheet.Month.ToString("00");

            // Don't want to submit XML file, want to submit build
            if (!IsXmlFilename(filename))
            {
                submitable.Submitter.Send(Settings, sMonth, Timesheet.Year.ToString(), filename, password);                
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
                if (!IsXmlFilename(filename))
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
            if (IsXmlFilename(filename))
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
        /// Determines whether [is XML filename] [the specified filename].
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>
        ///   <c>true</c> if [is XML filename] [the specified filename]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsXmlFilename(string filename)
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
