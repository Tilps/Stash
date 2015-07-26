using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a message to the NNTP server requesting to know whether a specific article exists.
    /// </summary>
    public class StatMessage : Message
    {
        /// <summary>
        /// Constructor.  Determines whether the currently selected message exists.
        /// </summary>
        public StatMessage()
            : base(CreateMessageData("STAT"), false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="postId">
        /// Angle bracketed post id of the article to retrieve.
        /// </param>
        public StatMessage(string postId)
            : base(CreateMessageData("STAT", postId), false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="postNumber">
        /// Number of the post to retrieve.
        /// </param>
        public StatMessage(int postNumber)
            : base(CreateMessageData("STAT", postNumber.ToString()), false)
        {
        }
    }
}
