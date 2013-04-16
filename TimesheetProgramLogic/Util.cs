// -----------------------------------------------------------------------
// <copyright file="Util.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Util
    {
        /// <summary>
        /// Creates a string of spaces to make the given string be of the required length when appended.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <returns>A string of the appropriate number of spaces</returns>
        public static string AddSpaces(string section, int maxLength)
        {
            string spaces = string.Empty;
            for (int i = 0; i <= (maxLength - section.Length); i++)
            {
                spaces += " ";
            }

            return spaces;
        }
    }
}
