using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a message to the server, requesting a list of groups.
    /// </summary>
    public class ListMessage : Message
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ListMessage()
            : base(CreateMessageData("LIST"), true)
        {
        }
    }
}
