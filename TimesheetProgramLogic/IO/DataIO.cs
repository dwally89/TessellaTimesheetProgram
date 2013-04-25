// -----------------------------------------------------------------------
// <copyright file="DataIO.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DataIO
    {
        /// <summary>
        /// Reads from file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>The contents of the file as a list of strings</returns>
        public static List<string> ReadTextFile(string filename)
        {
            List<string> fileContents = new List<string>();
            string line;
            using (StreamReader sr = new StreamReader(filename))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    fileContents.Add(line);
                }
            }

            return fileContents;
        }
    }
}
