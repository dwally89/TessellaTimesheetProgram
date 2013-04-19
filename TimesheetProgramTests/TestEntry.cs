using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimesheetProgramLogic;
using System;

namespace TimesheetProgramTests
{
    [TestClass]
    public class TestEntry
    {
        [TestMethod]
        public void CanCreateANewEntry()
        {            
            Entry entry = new Entry(0, new DateTime(), 0, new TimeSpan(), new TimeSpan() , "taskCode", "TESSELLA", false, "YES", "description", false);                
        }

        [TestMethod]        
        [ExpectedException(typeof(InvalidProjectNumberException))]
        public void ShouldThrowErrorWithNegativeProjectNumber()
        {
            Entry entry = new Entry(0, new DateTime(), -1, new TimeSpan(), new TimeSpan(), "taskCode", "TESSELLA", false, "YES", "description", false);                
        }
    }
}
