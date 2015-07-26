using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a response from the NNTP server that the current article number is invalid.
    /// </summary>
    public class CurrentArticleNumberInvalidResponse : Response
    {
         /// <summary>
        /// Response number identifying this response type.
        /// </summary>
        public static int ResType = 420;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="init">
        /// Raw bytes representing the response received from the NNTP server.
        /// </param>
        private CurrentArticleNumberInvalidResponse(byte[] init)
            : base(init, ResType, false)
        {
        }

        static CurrentArticleNumberInvalidResponse()
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
            return new CurrentArticleNumberInvalidResponse(init);
        }
   }
}
