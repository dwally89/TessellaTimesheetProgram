// -----------------------------------------------------------------------
// <copyright file="Entry.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Entry : INotifyPropertyChanged
    {
        /// <summary>
        /// The MA x_ TASKCOD e_ LENGTH
        /// </summary>
        public const int MAX_TASKCODE_LENGTH = 8;

        /// <summary>
        /// The MA x_ PHASECOD e_ LENGTH
        /// </summary>
        public const int MAX_PHASECODE_LENGTH = 8;

        /// <summary>
        /// The MA x_ DAT e_ LENGTH
        /// </summary>
        public const int MAX_DATE_LENGTH = 11;

        /// <summary>
        /// The MA x_ STAF f_ NUMBE r_ LENGTH
        /// </summary>
        public const int MAX_STAFF_NUMBER_LENGTH = 4;

        /// <summary>
        /// The MA x_ TIM e_ LENGTH
        /// </summary>
        public const int MAX_TIME_LENGTH = 2;

        /// <summary>
        /// The MA x_ DESCRIPTIO n_ LENGTH
        /// </summary>
        public const int MAX_DESCRIPTION_LENGTH = 35;

        /// <summary>
        /// The task codes
        /// </summary>
        public static readonly string[] TaskCodes = new string[]
        {
            "ADMIN",
            "HOLIDAY",
            "MANAGE",
            "MARKET",
            "QUALITY",
            "RECRUIT",
            "SICK",
            "TOIL",
            "TRAIN",
            "TRAINOWN",
            "BANKHOL",
            "BEREAVE",
            "DEPEND",
            "JURY",
            "MATERN",
            "OFFJOB",
            "THERA",
            "PARENT",
            "PATERN",
            "PUBHOL",
            "SALES",
            "TARMY",
            "XMAS"
        };

        /// <summary>
        /// The phase codes
        /// </summary>
        public static readonly string[] PhaseCodes = new string[]
        {
            "TESSELLA",
            "CONSULT",
            "DESIGN",
            "DEVELOP",
            "FSTUDY",
            "INSTALL",
            "REQUIRE",
            "SUPPORT",
            "TEST",
            "TRAVEL"
        };

        /// <summary>
        /// The billables
        /// </summary>
        public static readonly string[] Billables = new string[]
        {
            "YES",
            "NO",
            "ACCOUNTABLE"
        };        

        /// <summary>
        /// The _start time
        /// </summary>
        private TimeSpan _startTime;

        /// <summary>
        /// The _finish time
        /// </summary>
        private TimeSpan _finishTime;

        /// <summary>
        /// The _date
        /// </summary>
        private DateTime _date;

        /// <summary>
        /// The _billable
        /// </summary>
        private string _billable;

        /// <summary>
        /// The _task code
        /// </summary>
        private string _taskCode;

        /// <summary>
        /// The _project number
        /// </summary>
        private int _projectNumber;

        /// <summary>
        /// The _phase code
        /// </summary>
        private string _phaseCode;

        /// <summary>
        /// The _overhead
        /// </summary>
        private bool _overhead;

        /// <summary>
        /// The _description
        /// </summary>
        private string _description;

        /// <summary>
        /// Initializes a new instance of the <see cref="Entry" /> class.
        /// </summary>
        /// <param name="iD">The i D.</param>
        /// <param name="projectNumber">The project number.</param>
        /// <param name="line">The line.</param>
        public Entry(int iD, int projectNumber, string line)
        {
            string[] splitLine = line.Split(' ');
            string taskCode = string.Empty;
            string phaseCode = string.Empty;
            DateTime date = DateTime.MinValue;
            int staffID = -1;
            double time = -1;
            int hours = -1;
            int minutes = -1;
            string description = string.Empty;
            string billable = "Yes";
            bool overhead = false;

            foreach (string part in splitLine)
            {
                if (!(part.Equals(" ") || part.Equals(string.Empty)))
                {
                    if (taskCode == string.Empty)
                    {
                        if (part.StartsWith("-"))
                        {
                            billable = "No";
                        }

                        taskCode = part.Replace("-", string.Empty);
                    }
                    else if (phaseCode == string.Empty)
                    {
                        phaseCode = part;
                    }
                    else if (date == DateTime.MinValue)
                    {
                        date = DateTime.Parse(part);
                    }
                    else if (staffID == -1)
                    {
                        staffID = int.Parse(part);
                    }
                    else if (time == -1)
                    {
                        time = double.Parse(part);
                        hours = (int)time;
                        minutes = (int)((time - hours) * 60);
                    }
                    else
                    {
                        if (description == string.Empty)
                        {
                            if (part.StartsWith("#"))
                            {
                                billable = "Accountable";
                            }
                            else if (part.StartsWith("*"))
                            {
                                overhead = true;
                            }

                            description += part.Replace("#", string.Empty).Replace("*", string.Empty);
                        }
                        else
                        {
                            description += " " + part;
                        }
                    }
                }
            }

            FullConstructor(iD, date, projectNumber, new TimeSpan(9, 0, 0), new TimeSpan(9 + hours, minutes, 0), taskCode, phaseCode, overhead, billable, description, true);            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entry" /> class.
        /// </summary>
        /// <param name="iD">The i D.</param>
        /// <param name="date">The date.</param>
        /// <param name="projectNumber">The project number.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="finishTime">The finish time.</param>
        /// <param name="taskCode">The task code.</param>
        /// <param name="phaseCode">The phase code.</param>
        /// <param name="overhead">if set to <c>true</c> [overhead].</param>
        /// <param name="billable">The billable.</param>
        /// <param name="description">The description.</param>
        /// <param name="isReadFromBuild">if set to <c>true</c> [is read from build].</param>
        public Entry(int iD, DateTime date, int projectNumber, TimeSpan startTime, TimeSpan finishTime, string taskCode, string phaseCode, bool overhead, string billable, string description, bool isReadFromBuild)
        {
            FullConstructor(iD, date, projectNumber, startTime, finishTime, taskCode, phaseCode, overhead, billable, description, isReadFromBuild);
        }

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public int ID { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read from build.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read from build; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadFromBuild
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        [DisplayName("Date")]
        public DateTime Date
        {
            get
            {
                return _date;
            }

            set
            {
                _date = value;
                OnPropertyChanged("Date");
            }
        }

        /// <summary>
        /// Gets or sets the project number.
        /// </summary>
        /// <value>
        /// The project number.
        /// </value>
        [DisplayName("Proj")]
        public int ProjectNumber
        {
            get
            {
                return _projectNumber;
            }

            set
            {
                _projectNumber = value;
                OnPropertyChanged("ProjectNumber");
            }
        }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        [DisplayName("Start")]
        [XmlIgnore]
        public TimeSpan StartTime
        {            
            get
            {
                return _startTime;
            }

            set
            {
                _startTime = value;
                OnPropertyChanged("StartTime");
            }
        }

        /// <summary>
        /// Gets or sets the start time ticks.
        /// </summary>
        /// <value>
        /// The start time ticks.
        /// </value>
        public long StartTimeTicks
        {
            get
            {
                return _startTime.Ticks;
            }

            set
            {
                _startTime = new TimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the finish time.
        /// </summary>
        /// <value>
        /// The finish time.
        /// </value>
        [DisplayName("Finish")]
        [XmlIgnore]
        public TimeSpan FinishTime
        {
            get
            {
                return _finishTime;
            }

            set
            {
                _finishTime = value;
                OnPropertyChanged("FinishTime");
            }
        }

        /// <summary>
        /// Gets or sets the finish time ticks.
        /// </summary>
        /// <value>
        /// The finish time ticks.
        /// </value>
        public long FinishTimeTicks
        {
            get
            {
                return _finishTime.Ticks;
            }

            set
            {
                _finishTime = new TimeSpan(value);
            }
        }

        /// <summary>
        /// Gets the time.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        [DisplayName("Time")]
        public decimal Time
        {
            get
            {                             
                return Convert.ToDecimal((_finishTime - _startTime).TotalHours);
            }
        }

        /// <summary>
        /// Gets the date time.
        /// </summary>
        /// <value>
        /// The date time.
        /// </value>
        public DateTime DateTime
        {
            get
            {
                return new DateTime(Date.Year, Date.Month, Date.Day, StartTime.Hours, StartTime.Minutes, 0);
            }
        }

        /// <summary>
        /// Gets or sets the task code.
        /// </summary>
        /// <value>
        /// The task code.
        /// </value>
        [DisplayName("Task Code")]
        public string TaskCode
        {
            get
            {
                return _taskCode;
            }

            set
            {
                _taskCode = value;
                OnPropertyChanged("TaskCode");
            }
        }

        /// <summary>
        /// Gets or sets the phase code.
        /// </summary>
        /// <value>
        /// The phase code.
        /// </value>
        [DisplayName("Phase Code")]
        public string PhaseCode
        {
            get
            {
                return _phaseCode;
            }

            set
            {
                _phaseCode = value;
                OnPropertyChanged("PhaseCode");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Entry" /> is overhead.
        /// </summary>
        /// <value>
        ///   <c>true</c> if overhead; otherwise, <c>false</c>.
        /// </value>
        [DisplayName("O/H")]
        public bool Overhead
        {
            get
            {
                return _overhead;
            }

            set
            {
                _overhead = value;
                OnPropertyChanged("Overhead");
            }
        }

        /// <summary>
        /// Gets or sets the billable.
        /// </summary>
        /// <value>
        /// The billable.
        /// </value>
        [DisplayName("Billable")]
        public string Billable
        {
            get
            {
                return _billable;
            }

            set
            {
                _billable = value;
                OnPropertyChanged("Billable");
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [DisplayName("Description")]
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        /// <summary>
        /// Mains this instance.
        /// </summary>
        public static void Main()
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Date.ToString() + " " +
                ProjectNumber + " " +
                StartTime + " " +
                FinishTime + " " +
                TaskCode + " " +
                PhaseCode + " " +
                Overhead + " " +
                Billable + " " + 
                Description;
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Updates the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public void Update(Entry entry)
        {
            this.Billable = entry.Billable;
            this.Date = entry.Date;
            this.Description = entry.Description;
            this.FinishTime = entry.FinishTime;
            this.IsReadFromBuild = entry.IsReadFromBuild;
            this.Overhead = entry.Overhead;
            this.PhaseCode = entry.PhaseCode;
            this.ProjectNumber = entry.ProjectNumber;
            this.StartTime = entry.StartTime;
            this.TaskCode = entry.TaskCode;            
        }

        /// <summary>
        /// Full_constructors the specified date.
        /// </summary>
        /// <param name="iD">The i D.</param>
        /// <param name="date">The date.</param>
        /// <param name="projectNumber">The project number.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="finishTime">The finish time.</param>
        /// <param name="taskCode">The task code.</param>
        /// <param name="phaseCode">The phase code.</param>
        /// <param name="overhead">if set to <c>true</c> [overhead].</param>
        /// <param name="billable">The billable.</param>
        /// <param name="description">The description.</param>
        /// <param name="readFromBuild">if set to <c>true</c> [read from build].</param>
        /// <exception cref="TimesheetProgramLogic.InvalidProjectNumberException">blah blah blah</exception>
        /// <exception cref="TimesheetProgramLogic.InvalidBillableException">blah blah blah</exception>
        private void FullConstructor(int iD, DateTime date, int projectNumber, TimeSpan startTime, TimeSpan finishTime, string taskCode, string phaseCode, bool overhead, string billable, string description, bool readFromBuild)
        {            
            // _date = new DateTime();
            // _date.SelectedDate = date;
            this.ID = iD;
            _date = date;
            if (projectNumber < 0 || projectNumber > 9999)
            {
                throw new InvalidProjectNumberException();
            }
            else
            {
                _projectNumber = projectNumber;
            }

            _startTime = startTime;
            _finishTime = finishTime;
            _taskCode = taskCode;
            if (PhaseCodes.Contains(phaseCode))
            {
                _phaseCode = phaseCode;
            }
            else
            {
                if (readFromBuild)
                {
                    _phaseCode = string.Empty;
                }
                else
                {
                    throw new InvalidPhaseCodeException();
                }               
            }

            _overhead = overhead;

            if (Billables.Contains(billable.ToUpper()))
            {
                _billable = billable;
            }
            else
            {
                throw new InvalidBillableException();
            }

            _description = description;
            IsReadFromBuild = false;
        }
    }
}
