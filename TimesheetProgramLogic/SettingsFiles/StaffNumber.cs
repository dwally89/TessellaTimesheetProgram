namespace TimesheetProgramLogic
{
    using System;

    /// <summary>
    /// fsdgdgfd dfgdfgfd
    /// </summary>
    public class StaffNumber
    {
        /// <summary>
        /// The _value
        /// </summary>
        private int _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaffNumber" /> class.
        /// </summary>
        public StaffNumber() 
        {
            this._value = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaffNumber" /> class.
        /// </summary>
        /// <param name="sStaffNumber">The s staff number.</param>
        /// <exception cref="System.Exception">Invalid staff number</exception>
        public StaffNumber(string sStaffNumber)
        {
            int staffNumber;

            if (!int.TryParse(sStaffNumber, out staffNumber))
            {
                throw new ArgumentException("Invalid staff number");                
            }

            this.Number = staffNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaffNumber" /> class.
        /// </summary>
        /// <param name="staffNumber">The staff number.</param>
        public StaffNumber(int staffNumber)
        {            
            this.Number = staffNumber;
        }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        /// <exception cref="System.ArgumentException">Staff number must be less than 1000</exception>
        public int Number
        {
            get
            {
                return this._value;                
            }

            set
            {
                if (value >= 1000 || value < 1)
                {
                    throw new ArgumentException("Staff number must be less than 1000");
                }

                this._value = value;
            }
        }                
    }
}
