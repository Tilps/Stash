using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a response from an NNTP server that you should not have connected and the connection will be terminated.
    /// If in response to 'mode reader' this means reading will never be available to you, but connection is still terminated.
    /// </summary>
    public class ServicePermanentlyUnavailableResponse : Response
    {
        /// <summary>
        /// Response number identifying this response type.
        /// </summary>
        public static int ResType = 502;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="init">
        /// Raw bytes representing the response received from the NNTP server.
        /// </param>
        private ServicePermanentlyUnavailableResponse(byte[] init)
            : base(init, ResType, false)
        {
        }

        static ServicePermanentlyUnavailableResponse()
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
            return new ServicePermanentlyUnavailableResponse(init);
        }
    }
}
