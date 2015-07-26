using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents an NNTP message used for retrieving whole articles.
    /// </summary>
    public class ArticleMessage : Message
    {

        /// <summary>
        /// Constructor. For retrieveing current article.
        /// </summary>
        public ArticleMessage()
            : base(CreateMessageData("ARTICLE"), false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="postId">
        /// Angle bracketed post id of the article to retrieve.
        /// </param>
        public ArticleMessage(string postId)
            : base(CreateMessageData("ARTICLE", postId), false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="postNumber">
        /// Number of the post to retrieve.
        /// </param>
        public ArticleMessage(int postNumber)
            : base(CreateMessageData("ARTICLE", postNumber.ToString()), false)
        {
        }
    }
}
