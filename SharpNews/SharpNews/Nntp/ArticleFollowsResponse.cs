using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a response from the NNTP server containing the full content of an article.
    /// </summary>
    public class ArticleFollowsResponse : Response
    {
        /// <summary>
        /// Response number identifying this response type.
        /// </summary>
        public static int ResType = 220;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="init">
        /// Raw bytes representing the response received from the NNTP server.
        /// </param>
        private ArticleFollowsResponse(byte[] init)
            : base(init, ResType, true)
        {
        }

        static ArticleFollowsResponse()
        {
            Register(ResType, Build);
        }

        /// <summary>
        /// Helper method to perform construction which can be passed as a delegate.
        /// </summary>
        /// <param name="init">
        /// Raw bytes representing the response received from the NNTP server.
        /// </param>
        /// <returns>
        /// A constructed response for this type using the given byte array.
        /// </returns>
        private static Response Build(byte[] init)
        {
            return new ArticleFollowsResponse(init);
        }

        /// <summary>
        /// Gets the article number of the article found.
        /// </summary>
        public int ArticleNumber
        {
            get
            {
                return int.Parse(this.InitSplits[1]);
            }
        }

        /// <summary>
        /// Gets the angle bracketed article message-id of the article found.
        /// </summary>
        public string ArticleId
        {
            get
            {
                return this.InitSplits[2];
            }
        }
    }
}
