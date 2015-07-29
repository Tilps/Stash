using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;

namespace LOTROChat
{

    class LOTROGateway
    {
        public LOTROGateway(string cserver, string server, string username, string password, string expectedUser, string confirmMsg, bool tls)
        {
            connection = new XmppClientConnection(server);
            connection.ConnectServer = cserver;
            connection.UseStartTLS = tls;
            connection.OnMessage += new XmppClientConnection.MessageHandler(connection_OnMessage);
            connection.OnLogin += new ObjectHandler(connection_OnLogin);
            connection.OnError += new ErrorHandler(connection_OnError);
            connection.OnClose += new ObjectHandler(connection_OnClose);
            connection.OnPresence += new XmppClientConnection.PresenceHandler(connection_OnPresence);
            connection.OnSocketError += new ErrorHandler(connection_OnSocketError);
            this.username = username;
            this.password = password;
            connection.KeepAlive = true;
            connection.Open(username, password);
            this.expectedUser = expectedUser;
            this.confirmMsg = confirmMsg;
        }
        private string username;
        private string password;
        object mudconslock = new object();
        agsXMPP.XmppClientConnection connection;
        bool open = false;
        string expectedUser;
        string confirmMsg;
        
        void connection_OnSocketError(object sender, Exception ex)
        {
        }

        void connection_OnPresence(object sender, agsXMPP.protocol.client.Presence pres)
        {
            if (pres.Type == agsXMPP.protocol.client.PresenceType.subscribe)
            {
                if (pres.From.Bare == expectedUser)
                {
                    connection.PresenceManager.ApproveSubscriptionRequest(pres.From);
                }
            }
            else if (pres.Type == agsXMPP.protocol.client.PresenceType.unavailable)
            {
                if (pres.From.Bare == expectedUser)
                {
                    open = false;
                }
            }
        }

        void connection_OnClose(object sender)
        {
            connection.Open();
        }

        public void Send(string message, bool urgent)
        {
            if (open || urgent)
            {
                if (!open)
                {
                    message = "Sent As Urgent: " + message;
                }
                agsXMPP.protocol.client.Message mesg = new agsXMPP.protocol.client.Message(new Jid(expectedUser), agsXMPP.protocol.client.MessageType.chat, message);
                connection.Send(mesg);
            }
        }

        void connection_OnError(object sender, Exception ex)
        {
        }

        void connection_OnLogin(object sender)
        {
            connection.SendMyPresence();
            connection.RequestRoster();
        }


        void connection_OnMessage(object sender, agsXMPP.protocol.client.Message msg)
        {
            if (msg.Type == agsXMPP.protocol.client.MessageType.chat)
            {
                if (msg.From.Bare != expectedUser)
                    return;
                if (!open)
                {
                    if (msg.Body == confirmMsg)
                    {
                        open = true;
                    }
                }
                else
                {
                    KeySender.SendString(msg.Body + "\n");
                }
            }
            else if (msg.Type == agsXMPP.protocol.client.MessageType.normal)
            {
            }
        }

        void con_ConnectionShutdown(object sender, EventArgs e)
        {
            open = false;
        }

    }
}
