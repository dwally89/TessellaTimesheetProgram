namespace TimesheetProgramLogic
{
    /// <summary>
    /// fgdgd dfgdfgdf
    /// </summary>
    public class EmailUsername : NonEmptyString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailUsername" /> class.
        /// </summary>
        public EmailUsername()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailUsername" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public EmailUsername(string value)
            : base(value, "Email username")
        {
        }
    }
}
