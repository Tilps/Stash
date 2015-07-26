using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a response from the NNTP server that a request to move to the next article was made while on the last article.
    /// </summary>
    public class NoNextArticleResponse : Response
    {

        /// <summary>
        /// Response number identifying this response type.
        /// </summary>
        public static int ResType = 421;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="init">
        /// Raw bytes representing the response received from the NNTP server.
        /// </param>
        private NoNextArticleResponse(byte[] init)
            : base(init, ResType, false)
        {
        }

        static NoNextArticleResponse()
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
            return new NoNextArticleResponse(init);
        }
    }
}
