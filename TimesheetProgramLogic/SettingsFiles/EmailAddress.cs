namespace TimesheetProgramLogic
{
    /// <summary>
    /// sdgdf dfgfdg
    /// </summary>
    public class EmailAddress : NonEmptyString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddress" /> class.
        /// </summary>
        public EmailAddress() : base()
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddress" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public EmailAddress(string value)
            : base(value, "Email address")
        {
        }
    }
}
