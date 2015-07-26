using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a message to an NNTP server which requests the basic group details, plus a listing of the post numbers in that group.
    /// It also requests a change to that group.
    /// </summary>
    public class ListGroupMessage : Message
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="groupName">
        /// Group to change to and receive a full listing of its article numbers.
        /// </param>
        public ListGroupMessage(string groupName)
            : base(CreateMessageData("LISTGROUP", groupName), false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ListGroupMessage()
            : base(CreateMessageData("LISTGROUP"), false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="groupName">
        /// Group to change to and receive a full listing of its article numbers.
        /// </param>
        /// <param name="messageNumber">
        /// Message number to list if it exists.
        /// </param>
        public ListGroupMessage(string groupName, int messageNumber)
            : base(CreateMessage(groupName, messageNumber), false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="groupName">
        /// Group to change to and receive a full listing of its article numbers.
        /// </param>
        /// <param name="startRange">
        /// Message number for start of range to return existance details.
        /// </param>
        /// <param name="endRange">
        /// Message number for end of range to return existance details.
        /// Use int.MaxValue for unbounded.
        /// </param>
        public ListGroupMessage(string groupName, int startRange, int endRange)
            : base(CreateMessage(groupName, startRange, endRange), false)
        {
        }

        private static byte[] CreateMessage(string groupName, int messageNumber)
        {
            return CreateMessageData("LISTGROUP", groupName, messageNumber.ToString());
        }

        private static byte[] CreateMessage(string groupName, int startRange, int endRange)
        {
            if (endRange != int.MaxValue)
            {
                return CreateMessageData("LISTGROUP", groupName, startRange.ToString() + "-" + endRange.ToString());
            }
            else
            {
                return CreateMessageData("LISTGROUP", groupName, startRange.ToString() + "-");
            }
        }
    }
}
