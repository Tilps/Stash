using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a response from an NNTP server that it is temporarily unavailable, and the connection wil be shut down.
    /// </summary>
    public class ServiceTemporarilyUnavailableResponse : Response
    {
        /// <summary>
        /// Response number identifying this response type.
        /// </summary>
        public static int ResType = 400;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="init">
        /// Raw bytes representing the response received from the NNTP server.
        /// </param>
        private ServiceTemporarilyUnavailableResponse(byte[] init)
            : base(init, ResType, false)
        {
        }

        static ServiceTemporarilyUnavailableResponse()
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
            return new ServiceTemporarilyUnavailableResponse(init);
        }
    }
}
