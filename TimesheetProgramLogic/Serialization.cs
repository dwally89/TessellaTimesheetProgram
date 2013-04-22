// -----------------------------------------------------------------------
// <copyright file="Serialization.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Serialization
    {
        /// <summary>
        /// Serializes the specified object to serialize.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize</typeparam>
        /// <param name="objectToSerialize">The object to serialize.</param>
        /// <param name="filename">The filename.</param>
        public static void Serialize<T>(T objectToSerialize, string filename)        
        {             
            XmlSerializer serializer = new XmlSerializer(objectToSerialize.GetType());
            using (TextWriter writer = new StreamWriter(filename))
            {                
                serializer.Serialize(writer, objectToSerialize);
            }          
        }

        /// <summary>
        /// Deserializes the entries.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>An observable collection containing the entries</returns>
        public static Timesheet DeserializeTimesheet(string filename)
        {            
            XmlSerializer deserializer = new XmlSerializer(typeof(Timesheet));
            using (TextReader reader = new StreamReader(filename))
            {
                return (Timesheet)deserializer.Deserialize(reader);                
            }                      
        }
    }
}
