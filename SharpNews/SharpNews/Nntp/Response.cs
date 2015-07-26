using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents any NNTP response received by the client from the server.
    /// </summary>
    public class Response
    {

        /// <summary>
        /// Static constructor to ensure that all static constructors for individual responses are initialized to register.
        /// </summary>
        static Response()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in types)
            {
                if (type.BaseType != null && type.BaseType.Equals(typeof(Response)))
                {
                    RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="init">
        /// Raw byte array representing the initial line of the response.
        /// </param>
        private Response(byte[] init)
        {
            this.init = init;
            this.initString = Convert(init);
            this.responseType = -1;
            this.multiLine = false;
            complete = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="init">
        /// Raw byte array represneting the initial line of the response.
        /// </param>
        /// <param name="responseType">
        /// Response number of this response.
        /// </param>
        /// <param name="multiLine">
        /// If true, the initial line is followed by an unknown number of lines.
        /// </param>
        protected Response(byte[] init, int responseType, bool multiLine)
        {
            this.init = init;
            this.initString = Convert(init);
            this.responseType = responseType;
            this.multiLine = multiLine;
            complete = !multiLine;
        }

        /// <summary>
        /// Gets whether this response has been completed or is still receiving.
        /// </summary>
        public bool Complete
        {
            get
            {
                return complete;
            }
            protected set
            {
                complete = value;
            }
        }
        private volatile bool complete;

        /// <summary>
        /// Gets whether this response has a response body of unknown length.
        /// </summary>
        public bool MultiLine
        {
            get
            {
                return multiLine;
            }
            protected set
            {
                multiLine = value;
            }
        }
        private bool multiLine;

        /// <summary>
        /// Gets a cached copy of the space based spliting of the initial response line.
        /// </summary>
        public string[] InitSplits
        {
            get
            {
                if (initSplits == null)
                {
                    lock (initSplitsLock)
                    {
                        if (initSplits == null)
                        {
                            initSplits = initString.Split(' ');
                        }
                    }
                }
                return initSplits;
            }
        }
        private volatile string[] initSplits;
        private object initSplitsLock = new object();

        /// <summary>
        /// Gets the initial response line as a string, decoded using utf-8.
        /// </summary>
        public string InitString
        {
            get
            {
                return initString;
            }
        }
        private string initString;

        /// <summary>
        /// Gets the raw bytes of the initial response line.
        /// </summary>
        public byte[] Init
        {
            get
            {
                return init;
            }
        }
        private byte[] init;

        /// <summary>
        /// Gets the lines of the body, decoded using utf-8 if possible.
        /// </summary>
        public List<string> BodyLines
        {
            get
            {
                if (bodyLines == null)
                {
                    lock (bodyLinesLock)
                    {
                        if (bodyLines == null)
                        {
                            List<string> temp = new List<string>();
                            lock (body)
                            {
                                foreach (byte[] array in body)
                                {
                                    temp.Add(Convert(array));
                                }
                            }
                            bodyLines = temp;
                        }
                    }
                }
                return bodyLines;
            }
        }
        private volatile List<string> bodyLines;

        /// <summary>
        /// Gets a lock object for body lines.
        /// </summary>
        public object BodyLinesLock
        {
            get
            {
                return bodyLinesLock;
            }
        }
        private object bodyLinesLock = new object();

        /// <summary>
        /// Gets the raw bytes of the body of the response.
        /// </summary>
        public List<byte[]> Body
        {
            get
            {
                return body;
            }
        }
        private List<byte[]> body = new List<byte[]>();

        /// <summary>
        /// Adds a line to the body.
        /// </summary>
        /// <param name="bodyLine">
        /// Raw bytes of the body.
        /// </param>
        internal void Add(byte[] bodyLine)
        {
            lock (bodyLinesLock)
            {
                lock (body)
                {
                    body.Add(bodyLine);
                }
                if (bodyLines != null) 
                    bodyLines.Add(Convert(bodyLine));
            }
            if (bodyLine != null && bodyLine.Length > 1 && bodyLine[0] == (byte)'.' && bodyLine[1] != (byte)'.')
                complete = true;
        }

        /// <summary>
        /// Gets the integer response type indicator.
        /// </summary>
        public int ResponseType
        {
            get
            {
                return responseType;
            }
        }
        private int responseType;

        /// <summary>
        /// Gets the response args as a single string.
        /// </summary>
        public string ResponseArgs
        {
            get
            {
                if (initString != null && initString.Length > 3)
                    return initString.Substring(3).Trim();
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Registers a response type with a delegate used for constructing the response matching that type.
        /// </summary>
        /// <param name="type">
        /// Number identifying the response type to register.
        /// </param>
        /// <param name="tryConstructor">
        /// Constructor delegate which can construct a response of the appropriate type given the response type number.
        /// </param>
        protected static void Register(int type, TryConstructDelegate tryConstructor)
        {
            Response.constructors[type] = tryConstructor;
        }
        private static Dictionary<int, TryConstructDelegate> constructors = new Dictionary<int, TryConstructDelegate>();

        /// <summary>
        /// Converts a raw byte array into a string.
        /// </summary>
        /// <param name="init">
        /// Raw bytes representing some data.
        /// </param>
        /// <returns>
        /// A UTF-8 based conversion of the raw data into a string, if possible.
        /// Otherwise returns 'non-utf8 data.'
        /// </returns>
        protected static string Convert(byte[] init)
        {
            try
            {
                string res = Encoding.UTF8.GetString(init);
                return res;
            }
            catch
            {
                return "non-utf8 data.";
            }
        }

        /// <summary>
        /// Constructs a response given a byte array representing the initial line.  Will attempt to construct a stongly
        /// typed representation if one is known.
        /// </summary>
        /// <param name="flat">
        /// Array containing raw bytes of the initial line.
        /// </param>
        /// <returns>
        /// A response constructed from the initial raw data, potentially strongly typed.
        /// </returns>
        internal static Response Construct(byte[] flat)
        {
            if (flat.Length < 3)
            {
                return new Response(flat);
            }
            string resType = Encoding.ASCII.GetString(flat, 0, 3);
            int type = 0;
            bool valid = int.TryParse(resType, out type);
            if (!valid)
            {
                return new Response(flat);
            }
            else
            {
                if (!constructors.ContainsKey(type))
                {
                    return new Response(flat);
                }
                return constructors[type](flat);
            }
        }

        /// <summary>
        /// Delegate for constructing responses.
        /// </summary>
        /// <param name="init">
        /// Initial data to pass to constructor.
        /// </param>
        /// <returns>
        /// A response constructed using the initial data passed in.
        /// </returns>
        protected delegate Response TryConstructDelegate(byte[] init);
    }
}
