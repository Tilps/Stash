using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a friendly message to the server saying we are disconnecting.
    /// </summary>
    public class QuitMessage : Message
    {
          /// <summary>
        /// Constructor.
        /// </summary>
        public QuitMessage()
            : base(CreateMessageData("QUIT"), true)
        {
        }
   }
}
