using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a response from an NNTP server that the requested group does not exist.
    /// </summary>
    public class NoSuchGroupResponse : Response
    {
        /// <summary>
        /// Response number identifying this response type.
        /// </summary>
        public static int ResType = 411;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="init">
        /// Raw bytes representing the response received from the NNTP server.
        /// </param>
        private NoSuchGroupResponse(byte[] init)
            : base(init, ResType, false)
        {
        }

        static NoSuchGroupResponse()
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
            return new NoSuchGroupResponse(init);
        }
  }
}
