using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a request to the NNTP server to move to the message after the current one.
    /// </summary>
    public class NextMessage : Message
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public NextMessage()
            : base(CreateMessageData("NEXT"), false)
        {
        }
    }
}
