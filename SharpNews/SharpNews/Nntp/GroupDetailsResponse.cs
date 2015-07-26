using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a response from an NNTP server regarding basic details of a group, 
    /// potentially including a listing of post numbers.
    /// After receiving this response, the NNTP server will internally be assocating 
    /// this connection with the group details were returned for.
    /// </summary>
    public class GroupDetailsResponse : Response
    {
        /// <summary>
        /// Response number identifying this response type.
        /// </summary>
        public static int ResType = 211;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="init">
        /// Raw bytes representing the response received from the NNTP server.
        /// </param>
        private GroupDetailsResponse(byte[] init)
            : base(init, ResType, false)
        {
        }

        /// <summary>
        /// Gets or sets whether this response is to a LISTGROUP message.
        /// </summary>
        public bool IsListGroupResponse
        {
            get
            {
                return MultiLine;
            }
            internal set
            {
                MultiLine = value;
                Complete = !value;

            }
        }

        static GroupDetailsResponse()
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
            return new GroupDetailsResponse(init);
        }

        /// <summary>
        /// Gets the estimated number of articles in the group
        /// </summary>
        public int EstimatedArticleCount
        {
            get
            {
                return int.Parse(this.InitSplits[1]);
            }
        }

        /// <summary>
        /// Gets the article number of the lowest article in the group.
        /// </summary>
        public int LowArticleMark
        {
            get
            {
                return int.Parse(this.InitSplits[2]);
            }
        }

        /// <summary>
        /// Gets the article number of the top of the group.
        /// </summary>
        public int HighArticleMark
        {
            get
            {
                return int.Parse(this.InitSplits[3]);
            }
        }

        /// <summary>
        /// Gets the name of the news group the details are for.
        /// </summary>
        public string GroupName
        {
            get
            {
                return this.InitSplits[4];
            }
        }

   }
}
