using System;
using System.Collections.Generic;
using System.Text;
using SharpNews.Nntp;

namespace SharpNews
{
    /// <summary>
    /// Represents a mechanism for caching NNTP connections, and creating them as needed.
    /// </summary>
    public class NntpWorkhorse
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        private NntpWorkhorse()
        {
            settings.Load();
        }

        /// <summary>
        /// Ensure settings are uptodate.
        /// </summary>
        public void UpdateSettings()
        {
            settings.Load();
        }
        private ServerSettings settings = new ServerSettings();

        private Dictionary<Guid, NntpConnection> given = new Dictionary<Guid, NntpConnection>();
        private List<NntpConnection> available = new List<NntpConnection>();

        /// <summary>
        /// Gets an NntpConnection instance, if one is available or can be made.
        /// </summary>
        /// <param name="shareIfRequired">
        /// If true, will steal an existing connection from someone else, generally bad.
        /// </param>
        /// <param name="ownerHandle">
        /// Out parameter to receive the owning handle to use to return the connection.
        /// </param>
        /// <returns>
        /// An NntpConnection instance if one is available, otherwise null.
        /// </returns>
        public NntpConnection GetConnection(bool shareIfRequired, out Guid ownerHandle)
        {
            ownerHandle = Guid.Empty;
            lock (available)
            {
                if (available.Count > 0)
                {
                    NntpConnection connection = available[available.Count - 1];
                    available.RemoveAt(available.Count - 1);
                    ownerHandle = Guid.NewGuid();
                    lock (given)
                    {
                        given.Add(ownerHandle, connection);
                    }
                    return connection;
                }
            }
            lock (given)
            {
                if (given.Count < settings.MaxConnections)
                {
                    ownerHandle = Guid.NewGuid();
                    NntpConnection newConnection = new NntpConnection();
                    newConnection.Hostname = settings.Hostname;
                    newConnection.Username = settings.Username;
                    newConnection.Password = settings.Password;
                    newConnection.Port = settings.Port;
                    newConnection.Connect();
                    newConnection.Authenticate();
                    given.Add(ownerHandle, newConnection);
                    return newConnection;
                }
                else
                {
                    if (shareIfRequired)
                    {
                        NntpConnection res = null;
                        foreach (NntpConnection con in given.Values)
                        {
                            if (res == null)
                                res = con;
                            else if (con.Idle)
                                res = con;
                        }
                        return res;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a connection retrieved by GetConnection.
        /// </summary>
        /// <param name="ownerGuid">
        /// Handle guid given out when connection was retrieved.
        /// </param>
        public void ReturnConnection(Guid ownerGuid)
        {
            lock (available)
            {
                lock (given)
                {
                    if (given.ContainsKey(ownerGuid))
                    {
                        available.Add(given[ownerGuid]);
                        given.Remove(ownerGuid);
                    }
                }
            }
        }

        /// <summary>
        /// Returns and disconnects a connection retrieved by GetConnection.
        /// </summary>
        /// <param name="ownerGuid"></param>
        public void ReturnConnectionAndClose(Guid ownerGuid)
        {
            lock (given)
            {
                if (given.ContainsKey(ownerGuid))
                {
                    given[ownerGuid].Disconnect();
                    given.Remove(ownerGuid);
                }
            }
        }

        /// <summary>
        /// Gets a singleton instance of the NntpWorkhorse.
        /// </summary>
        public static NntpWorkhorse Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new NntpWorkhorse();
                    }
                    return instance;
                }
            }
        }
        private static object instanceLock = new object();
        private static NntpWorkhorse instance;
    }
}
