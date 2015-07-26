using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a message to the NNTP server requesting the headers of a specific article.
    /// </summary>
    public class HeadMessage : Message
    {

        /// <summary>
        /// Constructor. For retrieveing current articles headers.
        /// </summary>
        public HeadMessage()
            : base(CreateMessageData("HEAD"), false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="postId">
        /// Angle bracketed post id of the article to retrieve.
        /// </param>
        public HeadMessage(string postId)
            : base(CreateMessageData("HEAD", postId), false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="postNumber">
        /// Number of the post to retrieve.
        /// </param>
        public HeadMessage(int postNumber)
            : base(CreateMessageData("HEAD", postNumber.ToString()), false)
        {
        }
    }
}
