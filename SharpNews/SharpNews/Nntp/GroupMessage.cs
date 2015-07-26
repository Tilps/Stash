using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a message to an NNTP server requesting basic details of a group, and to change to that group.
    /// </summary>
    public class GroupMessage : Message
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="groupName">
        /// Group to request basic statistics for and to change to.
        /// </param>
        public GroupMessage(string groupName)
            : base(CreateMessageData("GROUP", groupName), false)
        {
        }
    }
}
