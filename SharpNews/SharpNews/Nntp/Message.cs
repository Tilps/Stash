using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents any kind of NNTP message from client to server
    /// </summary>
    public class Message
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">
        /// Raw bytes of message to send to server.
        /// </param>
        /// <param name="blocking">
        /// Whether this message blocks pipelining.
        /// </param>
        public Message(byte[] message, bool blocking)
        {
            messageBytes = message;
            this.blocking = blocking;
        }

        /// <summary>
        /// Creates a message data from the parts of the command.
        /// </summary>
        /// <param name="parameters">
        /// String parameters forming parts of the message.
        /// </param>
        /// <returns>
        /// A utf-8 encoded byte array of the mesage constructed from the parts.
        /// </returns>
        protected static byte[] CreateMessageData(params string[] parameters)
        {
            return Encoding.UTF8.GetBytes(string.Join(" ", parameters) + "\r\n");
        }

        /// <summary>
        /// Gets the raw message bytes of this message.
        /// </summary>
        public byte[] MessageBytes
        {
            get
            {
                return messageBytes;
            }
        }
        private byte[] messageBytes;

        /// <summary>
        /// Gets whether this message blocks pipelining.
        /// </summary>
        public bool Blocking
        {
            get
            {
                return blocking;
            }
        }
        private bool blocking;
    }
}
