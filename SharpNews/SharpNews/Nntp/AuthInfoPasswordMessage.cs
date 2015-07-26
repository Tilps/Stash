using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents an authentication message carrying the password for a user in plain text to the NNTP server.
    /// </summary>
    public class AuthInfoPasswordMessage : Message
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="password">
        /// Password to send to the server.
        /// </param>
        public AuthInfoPasswordMessage(string password)
            : base(CreateMessageData("AUTHINFO PASS", password), true)
        {
        }
    }
}

