using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SharpNews.Nntp
{
    /// <summary>
    /// Represents a single connection to an NNTP server.
    /// </summary>
    public class NntpConnection
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public NntpConnection()
        {
            blocking = 0;
            connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private Socket connection;

        private object threadLock = new object();
        private Thread sender;
        private Thread receiver;
        private int curBox = 1;

        /// <summary>
        /// Connects to the server.
        /// </summary>
        public void Connect()
        {
            blocking = 0;
            waiting.Clear();
            waitingForListGroupResponse.Clear();
            inboxes.Clear();
            startedBoxes.Clear();
            waiting.Add(0);
            waitingForListGroupResponse.Add(false);
            todo.Clear();
            curBox = 1;
            connection.NoDelay = true;
            connection.Connect(hostname, port);
            lock (threadLock)
            {
                sender = new Thread(new ThreadStart(SendLoop));
                sender.Start();
                receiver = new Thread(new ThreadStart(ReceiveLoop));
                receiver.Start();
            }
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        public void Disconnect()
        {
            connection.Disconnect(true);            
            lock (threadLock)
            {
                if (receiver != null)
                {
                    receiver.Abort();
                }
                if (sender != null)
                {
                    sender.Abort();
                }
                todo.Clear();
                waiting.Clear();
                waitingForListGroupResponse.Clear();
                blocking = -1;
                lock (inboxes)
                {
                    inboxes.Clear();
                    Monitor.PulseAll(inboxes);
                }
                lock (startedBoxes)
                {
                    startedBoxes.Clear();
                    Monitor.PulseAll(startedBoxes);
                }
            }
        }

        /// <summary>
        /// Performs username/password authentication on the connection.
        /// </summary>
        public void Authenticate()
        {
            AuthInfoUsernameMessage userMesg = new AuthInfoUsernameMessage(username);
            AuthInfoPasswordMessage passMesg = new AuthInfoPasswordMessage(password);
            int inbox = Send(userMesg);
            Response res = GetResponse(inbox);
            if (res.ResponseType != PasswordRequiredResponse.ResType)
                throw new Exception("Username authentication rejected.");
            int inbox2 = Send(passMesg);
            res = GetResponse(inbox2);
            if (res.ResponseType != AuthenticationAcceptedResponse.ResType) 
                throw new Exception("Username/password authentication rejected.");
        }


        /// <summary>
        /// Sends the given message.
        /// </summary>
        /// <param name="mesg">
        /// Message to send.
        /// </param>
        /// <returns>
        /// Inbox index to wait on.
        /// </returns>
        public int Send(Message mesg)
        {
            int res = 0;
            lock (sendLock)
            {
                res = curBox;
                outboxes[curBox] = mesg;
                todo.Add(curBox);
                curBox++;
                Monitor.PulseAll(sendLock);
            }
            return res;
        }

        /// <summary>
        /// Checks for any signs of connection loss.
        /// </summary>
        public void CheckConnect()
        {
            lock (threadLock)
            {
                if (receiver == null || sender == null)
                {
                    throw new Exception("Connection Lost");
                }
            }
        }

        /// <summary>
        /// Checks to see if a complete response is available.
        /// </summary>
        /// <param name="inbox">
        /// In box to check for a response.
        /// </param>
        /// <returns>
        /// The response if one has arrived, otherwise null.
        /// </returns>
        public Response CheckResponse(int inbox)
        {
            CheckConnect();
            lock (inboxes)
            {
                if (inboxes.ContainsKey(inbox))
                {
                    return inboxes[inbox];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets a complete response from the server.  Will wait until one is available.
        /// </summary>
        /// <param name="inbox">
        /// Inbox to retrieve response from.
        /// </param>
        /// <returns>
        /// The response, when it is received.
        /// </returns>
        public Response GetResponse(int inbox)
        {
            lock (inboxes)
            {
                while (!inboxes.ContainsKey(inbox))
                {
                    CheckConnect();
                    Monitor.Wait(inboxes);
                }
                return inboxes[inbox];
            }
        }

        /// <summary>
        /// Clears a response from the holding set so it doesn't take up memory.
        /// </summary>
        /// <param name="inbox">
        /// Inbox to clear.
        /// </param>
        public void ClearResponse(int inbox)
        {
            lock (inboxes)
            {
                inboxes.Remove(inbox);
            }
            lock (startedBoxes)
            {
                startedBoxes.Remove(inbox);
            }
        }

        /// <summary>
        /// Checks if a response has arrived which may or may not have all of its body.
        /// </summary>
        /// <param name="inbox">
        /// In box to check.
        /// </param>
        /// <returns>
        /// A response, potentially incomplete, if one is available.  Otherwise null.
        /// </returns>
        public Response CheckReceiving(int inbox)
        {
            CheckConnect();
            lock (startedBoxes)
            {
                if (startedBoxes.ContainsKey(inbox))
                {
                    return startedBoxes[inbox];
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets a response which may or may not have all of its body.  Waits until one is available before returning.
        /// </summary>
        /// <param name="inbox">
        /// The inbox to check for a response.
        /// </param>
        /// <returns>
        /// A response which may or may not be complete, as soon as it is available.
        /// </returns>
        public Response GetReceiving(int inbox)
        {
            lock (startedBoxes)
            {
                while (!startedBoxes.ContainsKey(inbox))
                {
                    CheckConnect();
                    Monitor.Wait(startedBoxes);
                }
                return startedBoxes[inbox];
            }
        }

        private Dictionary<int, Response> inboxes = new Dictionary<int, Response>();
        private Dictionary<int, Response> startedBoxes = new Dictionary<int, Response>();
        private Dictionary<int, Message> outboxes = new Dictionary<int, Message>();

        private List<int> todo = new List<int>();
        private List<int> waiting = new List<int>();
        private List<bool> waitingForListGroupResponse = new List<bool>();

        private object sendLock = new object();

        private void SendLoop()
        {
            try
            {
                lock (sendLock)
                {
                    while (true)
                    {
                        idle = blocking == -1 && todo.Count == 0;
                        if (blocking != -1 || todo.Count == 0)
                            Monitor.Wait(sendLock);
                        else
                        {
                            int outbox = todo[0];
                            Message mesg = outboxes[outbox];
                            if (mesg.Blocking)
                                blocking = outbox;
                            else
                                blocking = -1;
                            byte[] packet = mesg.MessageBytes;
                            IAsyncResult res = connection.BeginSend(packet, 0, packet.Length, SocketFlags.None, new AsyncCallback(SendCompleted), null);
                            outboxes.Remove(outbox);
                            lock (waiting)
                            {
                                waiting.Add(outbox);
                                waitingForListGroupResponse.Add(mesg is ListGroupMessage);
                            }
                            todo.RemoveAt(0);
                            Monitor.Wait(sendLock);

                        }
                    }
                }
            }
            catch
            {
                // Shutup the debugger.
            }
            finally
            {
                lock (threadLock)
                {
                    sender = null;
                    if (receiver != null) {
                        receiver.Abort();
                    }
                }
            }
        }

        private void SendCompleted(IAsyncResult res)
        {
            lock (sendLock)
            {
                connection.EndSend(res);
                Monitor.PulseAll(sendLock);
            }

        }

        private List<byte> buffer = new List<byte>(10000);

        private void ReceiveLoop()
        {
            try
            {
                byte[] chunk = new byte[10000];
                Response res = null;
                int inbox = 0;
                while (true)
                {
                    int length = connection.Receive(chunk);
                    if (length == 0)
                        throw new Exception("Connection Closed.");
                    for (int i = 0; i < length; i++)
                    {
                        buffer.Add(chunk[i]);
                    }
                    int firstLine = 0;
                    int lastLine = -1;
                    while (true)
                    {
                        firstLine = buffer.IndexOf((byte)'\n', firstLine, buffer.Count - firstLine);
                        if (firstLine == -1)
                            break;
                        // This construction is not wasteful, as the responses own these arrays eventually.
                        byte[] line = new byte[firstLine - lastLine];
                        buffer.CopyTo(lastLine + 1, line, 0, line.Length);
                        if (res == null)
                        {
                            res = Response.Construct(line);
                            // Res now owns line, we can't use it any more.
                            bool isListGroupResponse;
                            lock (waiting)
                            {
                                inbox = waiting[0];
                                waiting.RemoveAt(0);
                                isListGroupResponse = waitingForListGroupResponse[0];
                                waitingForListGroupResponse.RemoveAt(0);
                            }
                            if (res is GroupDetailsResponse && isListGroupResponse)
                            {
                                ((GroupDetailsResponse)res).IsListGroupResponse = true;
                            }
                            if (!res.MultiLine)
                            {
                                lock (inboxes)
                                {
                                    inboxes[inbox] = res;
                                    Monitor.PulseAll(inboxes);
                                }
                                lock (startedBoxes)
                                {
                                    startedBoxes[inbox] = res;
                                    Monitor.PulseAll(startedBoxes);
                                }

                                lock (sendLock)
                                {
                                    if (blocking == inbox)
                                    {
                                        blocking = -1;
                                        Monitor.PulseAll(sendLock);
                                    }
                                }
                                res = null;
                            }
                            else
                            {
                                lock (startedBoxes)
                                {
                                    startedBoxes[inbox] = res;
                                    Monitor.PulseAll(startedBoxes);
                                }
                            }
                        }
                        else if (res != null && res.MultiLine)
                        {
                            res.Add(line);
                            // Res now owns line, we can't use it any more.
                            if (res.Complete)
                            {
                                lock (inboxes)
                                {
                                    inboxes[inbox] = res;
                                    Monitor.PulseAll(inboxes);
                                }

                                lock (sendLock)
                                {
                                    if (blocking == inbox)
                                    {
                                        blocking = -1;
                                        Monitor.PulseAll(sendLock);
                                    }
                                }
                                res = null;
                            }
                        }
                        else
                        {
                            res = null;
                        }
                        lastLine = firstLine;
                        firstLine++;

                    }
                    if (lastLine >= 0)
                    {
                        buffer.RemoveRange(0, lastLine + 1);
                    }
                }
            }
            catch
            {
                // Shutup the debugger.
            }
            finally
            {
                lock (threadLock)
                {
                    receiver = null;
                    if (sender != null) {
                        sender.Abort();
                    }
                }
            }
        }

        /// <summary>
        /// Gets whether the connection is idle.
        /// </summary>
        public bool Idle
        {
            get
            {
                return idle;
            }
        }
        private bool idle;

        private int blocking;

        /// <summary>
        /// Gets or sets the hostname to connect to for this connection.
        /// </summary>
        public string Hostname
        {
            get
            {
                return hostname;
            }
            set
            {
                hostname = value;
            }
        }
        private string hostname;

        /// <summary>
        /// Gets or sets the port number to connect to on the server for this connection.
        /// </summary>
        public int Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
            }
        }
        private int port;

        /// <summary>
        /// Gets or sets the username to login as during authentication.
        /// </summary>
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
            }
        }
        private string username;

        /// <summary>
        /// Gets or sets the password to login with during authentication.
        /// </summary>
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
            }
        }
        private string password;
    }
}
