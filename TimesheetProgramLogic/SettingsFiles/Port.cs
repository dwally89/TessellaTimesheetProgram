namespace TimesheetProgramLogic
{
    using System;

    /// <summary>
    /// fgfdgd dfgfdg
    /// </summary>
    public class Port
    {
        /// <summary>
        /// The _value
        /// </summary>
        private string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Port" /> class.
        /// </summary>
        public Port() 
        {
            this._value = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Port" /> class.
        /// </summary>
        /// <param name="sValue">The s value.</param>
        /// <exception cref="System.Exception">Port must be a valid number</exception>
        public Port(string sValue)
        {
            this.Value = sValue;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        /// <exception cref="System.ArgumentException">Port must be a valid number</exception>
        public string Value
        {
            get
            {
                return this._value;
            }

            set
            {
                int iValue;
                if (!int.TryParse(value, out iValue))
                {
                    throw new ArgumentException("Port must be a valid number");
                }

                this._value = value;
            }
        }
    }
}
