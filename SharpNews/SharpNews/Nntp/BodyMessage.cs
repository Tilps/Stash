using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a message to the NNTP server that requests the body of a specific article.
    /// </summary>
    public class BodyMessage : Message
    {
 
        /// <summary>
        /// Constructor. For retrieveing current articles body.
        /// </summary>
        public BodyMessage()
            : base(CreateMessageData("BODY"), false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="postId">
        /// Angle bracketed post id of the article to retrieve.
        /// </param>
        public BodyMessage(string postId)
            : base(CreateMessageData("BODY", postId), false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="postNumber">
        /// Number of the post to retrieve.
        /// </param>
        public BodyMessage(int postNumber)
            : base(CreateMessageData("BODY", postNumber.ToString()), false)
        {
        }
   }
}
