// -----------------------------------------------------------------------
// <copyright file="SubmitToNotes.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TimesheetProgramLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Domino;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SubmitToNotes
    {
        /// <summary>
        /// Sends the specified staff ID.
        /// </summary>
        /// <param name="staffID">The staff ID.</param>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <param name="fullFilename">The full filename.</param>
        /// <param name="emailAddress">The email address.</param>
        public static void Send(string staffID, string month, string year, string fullFilename, string emailAddress)
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
            string subject = staffID + " " + month + "/" + year;
            document.ReplaceItemValue("Subject", subject);

            NotesRichTextItem attachment = document.CreateRichTextItem("Attachment");
            attachment.EmbedObject(EMBED_TYPE.EMBED_ATTACHMENT, string.Empty, fullFilename, "Attachment");

            document.SaveMessageOnSend = true;
            document.ReplaceItemValue("PostedDate", DateTime.Now);
            document.Send(false, emailAddress);
        }
    }
}
