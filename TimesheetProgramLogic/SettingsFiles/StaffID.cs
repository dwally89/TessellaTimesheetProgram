namespace TimesheetProgramLogic
{
    using System;

    /// <summary>
    /// dfdg dfgfdgfd
    /// </summary>
    public class StaffID
    {
        /// <summary>
        /// The _value
        /// </summary>
        private string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaffID" /> class.
        /// </summary>
        public StaffID() 
        {
            this._value = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaffID" /> class.
        /// </summary>
        /// <param name="staffID">The staff ID.</param>
        /// <exception cref="System.Exception">Staff ID must be less than 6 characters</exception>
        public StaffID(string staffID)
        {            
            this.ID = staffID;            
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        /// <exception cref="System.ArgumentException">Staff ID must be less than 6 characters</exception>
        public string ID
        {
            get
            {
                return _value;
            }

            set
            {
                if (value.Length >= 6 || value.Equals(string.Empty))
                {
                    throw new ArgumentException("Staff ID must be less than 6 characters");
                }

                this._value = value;
            }
        }
    }
}
