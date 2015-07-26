using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a message to an NNTP server informing it of the username.
    /// </summary>
    public class AuthInfoUsernameMessage : Message
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="username">
        /// Username to pass to server.
        /// </param>
        public AuthInfoUsernameMessage(string username)
            : base(CreateMessageData("AUTHINFO USER", username), true)
        {
        }
    }
}
