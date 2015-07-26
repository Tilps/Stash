using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a response from the NNTP server that a specified article has been found.
    /// </summary>
    public class ArticleFoundResponse : Response
    {
        /// <summary>
        /// Response number identifying this response type.
        /// </summary>
        public static int ResType = 223;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="init">
        /// Raw bytes representing the response received from the NNTP server.
        /// </param>
        private ArticleFoundResponse(byte[] init)
            : base(init, ResType, false)
        {
        }

        static ArticleFoundResponse()
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
            return new ArticleFoundResponse(init);
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
