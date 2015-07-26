using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a request to the NNTP server to move to the message before the current one.
    /// </summary>
    public class LastMessage : Message
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public LastMessage()
            : base(CreateMessageData("LAST"), false)
        {
        }
    }
}
