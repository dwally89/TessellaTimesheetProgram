namespace TimesheetProgramLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// statistics stuff
    /// </summary>
    public class Statistics
    {
        /// <summary>
        /// The months with35and ahalf hour weeks
        /// </summary>
        private static readonly int[] MonthsWith35andAhalfHourWeeks = new int[] { 1, 2, 11, 12 };

        /// <summary>
        /// The months with37and ahalf hour weeks
        /// </summary>
        private static readonly int[] MonthsWith37andAhalfHourWeeks = new int[] { 3, 4, 5, 6, 7, 8, 9, 10 };

        /// <summary>
        /// Calculates the monthly summary.
        /// </summary>
        /// <param name="timesheet">The timesheet.</param>
        /// <returns>The monthly summary</returns>
        public static MonthlySummary CalculateMonthlySummary(Timesheet timesheet)
        {
            List<Entry> entries = timesheet.Entries;
            MonthlySummary summary = new MonthlySummary();                      
            
            summary.NumberOfDaysWorkedSoFar = NumberOfDaysWorkedSoFar(entries);
            summary.TotalHoursWorkedSoFar = TotalHoursWorkedInMonth(entries);                        

            if (MonthsWith37andAhalfHourWeeks.Contains(timesheet.Month))
            {
                summary.ExpectedHoursPerDay = 7.5;
            }
            else if (MonthsWith35andAhalfHourWeeks.Contains(timesheet.Month))
            {
                summary.ExpectedHoursPerDay = 7.1;
            }

            return summary;
        }

        /// <summary>
        /// Calculates the weekly summary.
        /// </summary>
        /// <param name="timesheet">The timesheet.</param>
        /// <returns>fdgdfgfd dfgdfgfd</returns>
        public static WeeklySummary CalculateWeeklySummary(Timesheet timesheet)
        {
            List<Entry> entries = timesheet.Entries;
            WeeklySummary summary = new WeeklySummary();
            summary.NumberOfDaysWorkedSoFar = NumberOfDaysWorkedSoFar(entries);            
            summary.TotalHoursWorkedSoFar = TotalHoursWorkedInMonth(entries);            
            if (MonthsWith37andAhalfHourWeeks.Contains(timesheet.Month))
            {
                summary.ExpectedHoursPerWeek = 37.5;
            }
            else if (MonthsWith35andAhalfHourWeeks.Contains(timesheet.Month))
            {
                summary.ExpectedHoursPerWeek = 35.5;
            }

            return summary;
        }

        /// <summary>
        /// Totals the hours worked in month.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <returns>the blah blah</returns>
        private static decimal TotalHoursWorkedInMonth(List<Entry> entries)
        {
            return (from entry in entries select entry.Time).Sum();
        }

        /// <summary>
        /// Numbers the of days worked so far.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <returns>ghfghfg fghfghgf</returns>
        private static int NumberOfDaysWorkedSoFar(List<Entry> entries)
        {
            return (from entry in entries select entry.Date).Distinct().ToList().Count;
        }
    }
}
