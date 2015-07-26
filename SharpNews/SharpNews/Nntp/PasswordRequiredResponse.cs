using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents an NNTP response to AUTHINFO USER stating password command should be sent.
    /// </summary>
    public class PasswordRequiredResponse : Response
    {

        /// <summary>
        /// Response number identifying this response type.
        /// </summary>
        public static int ResType = 381;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="init">
        /// Raw bytes representing the response received from the NNTP server.
        /// </param>
        private PasswordRequiredResponse(byte[] init)
            : base(init, ResType, false)
        {
        }

        static PasswordRequiredResponse()
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
            return new PasswordRequiredResponse(init);
        }
    }
}
