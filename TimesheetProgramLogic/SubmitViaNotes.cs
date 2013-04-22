// -----------------------------------------------------------------------
// <copyright file="SubmitViaNotes.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
namespace TimesheetProgramLogic
{
    using System;
    using System.Security;
    using Domino;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SubmitViaNotes : ISubmitter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitViaNotes" /> class.
        /// </summary>
        public SubmitViaNotes()
        {            
        }

        /// <summary>
        /// Sends the via notes.
        /// </summary>
        /// <param name="settings">Any settings needed</param>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <param name="fullFilename">The full filename.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="password">The password required to submit</param>
        public void Send(Settings settings, string month, string year, string fullFilename, string emailAddress, SecureString password = null)
        {
            // Two digit month and four digit year
            NotesSession session = new NotesSession();
            session.Initialize();
            NotesDatabase database = session.GetDatabase("Pride/Tessella", "mail\\waldm.nsf");

            if (!database.IsOpen)
            {
                database.Open();
            }

            NotesDocument document = database.CreateDocument();
            document.ReplaceItemValue("Form", "Memo");
            document.ReplaceItemValue("Sendto", emailAddress);
            string subject = settings.StaffID + " " + month + "/" + year;
            document.ReplaceItemValue("Subject", subject);

            NotesRichTextItem attachment = document.CreateRichTextItem("Attachment");
            attachment.EmbedObject(EMBED_TYPE.EMBED_ATTACHMENT, string.Empty, fullFilename, "Attachment");

            document.SaveMessageOnSend = true;
            document.ReplaceItemValue("PostedDate", DateTime.Now);
            document.Send(false, emailAddress);
        }
    }
}
