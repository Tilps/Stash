using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a response from the NNTP server that the server is available, and that posting is not available.
    /// </summary>
    public class ServiceAvailableNoPostingResponse : Response
    {
        /// <summary>
        /// Response number identifying this response type.
        /// </summary>
        public static int ResType = 201;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="init">
        /// Raw bytes representing the response received from the NNTP server.
        /// </param>
        private ServiceAvailableNoPostingResponse(byte[] init)
            : base(init, ResType, false)
        {
        }

        static ServiceAvailableNoPostingResponse()
        {
            Register(ResType, new TryConstructDelegate(Build));
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
            return new ServiceAvailableNoPostingResponse(init);
        }
    }
}
