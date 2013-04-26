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
    public class Entry
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
        /// Initializes a new instance of the <see cref="Entry" /> class.
        /// DO NOT USE. ONLY HERE FOR SERIALIZATION.
        /// </summary>
        public Entry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entry" /> class.
        /// </summary>
        /// <param name="projectNumber">The project number.</param>
        /// <param name="line">The line.</param>
        /// <param name="id">The id.</param>
        public Entry(int projectNumber, string line, int id)
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
                        ParseTaskCode(ref taskCode, ref billable, part);
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
                        ParseTime(ref time, ref hours, ref minutes, part);
                    }
                    else
                    {
                        ParseDescription(ref description, ref billable, ref overhead, part);
                    }
                }
            }

            FullConstructor(id, date, projectNumber, new TimeSpan(9, 0, 0), new TimeSpan(9 + hours, minutes, 0), taskCode, phaseCode, overhead, billable, description, true);            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entry" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
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
        public Entry(int id, DateTime date, int projectNumber, TimeSpan startTime, TimeSpan finishTime, string taskCode, string phaseCode, bool overhead, string billable, string description, bool isReadFromBuild)
        {
            FullConstructor(id, date, projectNumber, startTime, finishTime, taskCode, phaseCode, overhead, billable, description, isReadFromBuild);
        }

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
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the project number.
        /// </summary>
        /// <value>
        /// The project number.
        /// </value>
        [DisplayName("Proj")]
        public int ProjectNumber { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public int ID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        [DisplayName("Start")]
        [XmlIgnore]
        public TimeSpan StartTime { get; set; }

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
                return StartTime.Ticks;
            }

            set
            {
                StartTime = new TimeSpan(value);
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
        public TimeSpan FinishTime { get; set; }

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
                return FinishTime.Ticks;
            }

            set
            {
                FinishTime = new TimeSpan(value);
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
                return Convert.ToDecimal((FinishTime - StartTime).TotalHours);
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
        public string TaskCode { get; set; }

        /// <summary>
        /// Gets or sets the phase code.
        /// </summary>
        /// <value>
        /// The phase code.
        /// </value>
        [DisplayName("Phase Code")]
        public string PhaseCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Entry" /> is overhead.
        /// </summary>
        /// <value>
        ///   <c>true</c> if overhead; otherwise, <c>false</c>.
        /// </value>
        [DisplayName("O/H")]
        public bool Overhead { get; set; }

        /// <summary>
        /// Gets or sets the billable.
        /// </summary>
        /// <value>
        /// The billable.
        /// </value>
        [DisplayName("Billable")]
        public string Billable { get; set; }
            
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [DisplayName("Description")]
        public string Description { get; set; }

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
        /// Full_constructors the specified date.
        /// </summary>
        /// <param name="id">The id.</param>
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
        /// <exception cref="InvalidProjectNumberException">FDG FDGDF</exception>
        /// <exception cref="InvalidPhaseCodeException">DFGD FGFD</exception>
        /// <exception cref="InvalidBillableException">DFGDF GFD</exception>
        /// <exception cref="TimesheetProgramLogic.InvalidProjectNumberException">blah blah blah</exception>
        /// <exception cref="TimesheetProgramLogic.InvalidBillableException">blah blah blah</exception>
        private void FullConstructor(int id, DateTime date, int projectNumber, TimeSpan startTime, TimeSpan finishTime, string taskCode, string phaseCode, bool overhead, string billable, string description, bool readFromBuild)
        {            
            this.ID = id;
            this.Date = date;
            if (projectNumber < 0 || projectNumber > 9999)
            {
                throw new InvalidProjectNumberException();
            }
            else
            {
                this.ProjectNumber = projectNumber;
            }

            if (PhaseCodes.Contains(phaseCode))
            {
                this.PhaseCode = phaseCode;
            }
            else if (readFromBuild)
            {
                this.PhaseCode = string.Empty;
            }
            else
            {
                throw new InvalidPhaseCodeException();
            }
            
            if (Billables.Contains(billable.ToUpper()))
            {
                this.Billable = billable;
            }
            else
            {
                throw new InvalidBillableException();
            }

            this.StartTime = startTime;
            this.FinishTime = finishTime;
            this.TaskCode = taskCode;
            this.Overhead = overhead;            
            this.Description = description;
            IsReadFromBuild = false;
        }

        /// <summary>
        /// Parses the description.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="billable">The billable.</param>
        /// <param name="overhead">if set to <c>true</c> [overhead].</param>
        /// <param name="part">The part.</param>
        private void ParseDescription(ref string description, ref string billable, ref bool overhead, string part)
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

        /// <summary>
        /// Parses the time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="hours">The hours.</param>
        /// <param name="minutes">The minutes.</param>
        /// <param name="part">The part.</param>
        private void ParseTime(ref double time, ref int hours, ref int minutes, string part)
        {
            time = double.Parse(part);
            hours = (int)time;
            minutes = (int)((time - hours) * 60);
        }

        /// <summary>
        /// Parses the task code.
        /// </summary>
        /// <param name="taskCode">The task code.</param>
        /// <param name="billable">The billable.</param>
        /// <param name="part">The part.</param>
        private void ParseTaskCode(ref string taskCode, ref string billable, string part)
        {
            if (part.StartsWith("-"))
            {
                billable = "No";
            }

            taskCode = part.Replace("-", string.Empty);
        }
    }
}
