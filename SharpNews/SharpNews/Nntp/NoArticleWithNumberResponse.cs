using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a response from the NNTP server to a request for a specific article by number, when such number is not
    /// available in the current group.
    /// </summary>
    public class NoArticleWithNumberResponse : Response
    {
        /// <summary>
        /// Response number identifying this response type.
        /// </summary>
        public static int ResType = 423;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="init">
        /// Raw bytes representing the response received from the NNTP server.
        /// </param>
        private NoArticleWithNumberResponse(byte[] init)
            : base(init, ResType, false)
        {
        }

        static NoArticleWithNumberResponse()
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
            return new NoArticleWithNumberResponse(init);
        }
    }
}
