namespace TimesheetProgramLogic
{
    using System;

    /// <summary>
    /// sgdfgfd dfgfd
    /// </summary>
    public class NonEmptyString
    {
        /// <summary>
        /// The _name
        /// </summary>
        private string _name;

        /// <summary>
        /// The _value
        /// </summary>
        private string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonEmptyString" /> class.
        /// </summary>
        public NonEmptyString()
        {
            this._value = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonEmptyString" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="System.Exception">dfgdfgdf dfgdfgfd</exception>
        public NonEmptyString(string value, string name)
        {
            this._name = name;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value
        {
            get
            {
                return this._value;
            }

            set
            {
                if (value.Equals(string.Empty))
                {
                    throw new ArgumentException(_name + " cannot be empty");
                }

                this._value = value;
            }
        }
    }
}
