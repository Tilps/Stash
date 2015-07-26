using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a response from the server to an attempt to move backwards through the group when
    /// you are already at the start of the group.
    /// </summary>
    public class NoPreviousArticleResponse : Response
    {

        /// <summary>
        /// Response number identifying this response type.
        /// </summary>
        public static int ResType = 422;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="init">
        /// Raw bytes representing the response received from the NNTP server.
        /// </param>
        private NoPreviousArticleResponse(byte[] init)
            : base(init, ResType, false)
        {
        }

        static NoPreviousArticleResponse()
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
            return new NoPreviousArticleResponse(init);
        }
    }
}
