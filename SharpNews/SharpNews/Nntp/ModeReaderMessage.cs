using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a request to an NNTP server to switch to reading mode if the server is a mode switching server.
    /// </summary>
    public class ModeReaderMessage : Message
    {
         /// <summary>
        /// Constructor.
        /// </summary>
        public ModeReaderMessage()
            : base(CreateMessageData("MODE READER"), true)
        {
        }
   }
}
