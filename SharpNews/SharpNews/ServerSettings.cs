using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace SharpNews
{
    /// <summary>
    /// Represents settings pertaining to an NNTP server.
    /// </summary>
    public class ServerSettings
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ServerSettings()
        {
        }

        /// <summary>
        /// Load settings from the database.
        /// </summary>
        public void Load()
        {
            using (SqlConnection dbconnection = new SqlConnection(Properties.Settings.Default.ServerConnectionString))
            {
                dbconnection.Open();
                SqlCommand cmd = dbconnection.CreateCommand();
                cmd.CommandText = "SELECT * from Settings";
                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (reader.Read())
                    {
                        hostname = reader.GetString(1);
                        name = reader.GetString(2);
                        port = reader.GetInt32(3);
                        if (reader.IsDBNull(4))
                            username = null;
                        else
                            username = reader.GetString(4);
                        if (reader.IsDBNull(5))
                            password = null;
                        else
                            password = reader.GetString(5);
                        maxConnections = reader.GetInt32(6);
                    }
                }
            }
        }

        /// <summary>
        /// Saves settings to the database.
        /// </summary>
        public void Save()
        {
            using (SqlConnection dbconnection = new SqlConnection(Properties.Settings.Default.ServerConnectionString))
            {
                dbconnection.Open();
                using (SqlTransaction trans = dbconnection.BeginTransaction())
                {
                    SqlCommand cmd = dbconnection.CreateCommand();
                    cmd.Transaction = trans;

                    cmd.CommandText = "DELETE FROM Settings";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO Settings (Hostname, Name, Port, Username, Password, [Max Connections]) VALUES (@hostname, @name, @port, @username, @password, @maxconnections)";
                    cmd.Parameters.AddWithValue("hostname", hostname);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("port", port == -1 ? (object)DBNull.Value : (object)port);
                    cmd.Parameters.AddWithValue("username", username == null ? (object)DBNull.Value : (object)username);
                    cmd.Parameters.AddWithValue("password", password == null ? (object)DBNull.Value : (object)password);
                    cmd.Parameters.AddWithValue("maxconnections", maxConnections);

                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }

            }
        }


        /// <summary>
        /// Gets or sets the servers hostname.
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
        /// Gets or sets the servers friendly name.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        private string name;

        /// <summary>
        /// Gets or sets the servers port number.
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
        private int port = -1;

        /// <summary>
        /// Gets or sets the username to connect with.
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
        /// Gets or sets the password to authenticate using.
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

        /// <summary>
        /// Gets or sets the maximum number of connections supported.
        /// </summary>
        public int MaxConnections
        {
            get
            {
                return maxConnections;
            }
            set
            {
                maxConnections = value;
            }
        }
        private int maxConnections;
    }
}
