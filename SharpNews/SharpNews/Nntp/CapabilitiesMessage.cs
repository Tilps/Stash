using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a message to an NNTP server to request information about what features the server supports.
    /// </summary>
    public class CapabilitiesMessage : Message
    {
         /// <summary>
        /// Constructor.
        /// </summary>
        public CapabilitiesMessage()
            : base(CreateMessageData("CAPABILITIES"), true)
        {
        }
  }
}
