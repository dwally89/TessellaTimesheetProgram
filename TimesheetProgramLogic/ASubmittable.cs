// -----------------------------------------------------------------------
// <copyright file="ASubmittable.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public abstract class ASubmittable
    {
        /// <summary>
        /// Gets or sets the submitter.
        /// </summary>
        /// <value>
        /// The submitter.
        /// </value>
        public ISubmitter Submitter { get; set; }
    }
}
