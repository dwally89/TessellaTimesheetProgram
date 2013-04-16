// -----------------------------------------------------------------------
// <copyright file="Serialization.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
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
        public static ObservableCollection<Entry> DeserializeEntries(string filename)
        {            
            XmlSerializer deserializer = new XmlSerializer(typeof(ObservableCollection<Entry>));
            using (TextReader reader = new StreamReader(filename))
            {
                ObservableCollection<Entry> entries = (ObservableCollection<Entry>)deserializer.Deserialize(reader);
                return entries;
            }                      
        }

        /// <summary>
        /// Deserializes the settings.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>The settings contained in the file</returns>
        public static Settings DeserializeSettings(string filename)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(Settings));
            try
            {
                using (TextReader reader = new StreamReader(filename))
                {
                    Settings settings = (Settings)deserializer.Deserialize(reader);
                    return settings;
                }
            }
            catch (FileNotFoundException)
            {
                Serialize<Settings>(Settings.DefaultSettings(), filename);
                return Settings.DefaultSettings();
            }
        }
    }
}
