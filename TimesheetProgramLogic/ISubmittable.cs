// -----------------------------------------------------------------------
// <copyright file="ISubmittable.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ISubmittable
    {
        /// <summary>
        /// Gets or sets the submitter.
        /// </summary>
        /// <value>
        /// The submitter.
        /// </value>
        ISubmitter Submitter { get; set; }
    }
}
