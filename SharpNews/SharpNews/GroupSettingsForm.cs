using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using SharpNews.Nntp;

namespace SharpNews
{
    /// <summary>
    /// Provides a form for selecting groups
    /// </summary>
    public partial class GroupSettingsForm : Form
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GroupSettingsForm()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        Thread downloader = null;
        object downloaderLock = new object();

        private void startDownloadButton_Click(object sender, EventArgs e)
        {
            lock (downloaderLock)
            {
                if (downloader != null)
                    return;
                downloader = new Thread(new ThreadStart(DownloadGroups));
                downloader.Start();
                startDownloadButton.Enabled = false;
                stopDownloadButton.Enabled = true;
            }
        }

        private void RevertEnabled()
        {
            startDownloadButton.Enabled = true;
            stopDownloadButton.Enabled = false;
        }

        List<string> allLines = new List<string>();
        delegate void DisplayDelegate(string[] toAdd);

        private void Display(string[] toAdd)
        {
            allLines.AddRange(toAdd);
            dataGridViewGroups.RowCount = allLines.Count;
        }

        private void DownloadGroups()
        {
            allLines.Clear();
            NntpConnection con = null;
            Guid handle = Guid.Empty;
            try
            {
                con = NntpWorkhorse.Instance.GetConnection(true, out handle);
                ListMessage mesg = new ListMessage();
                int inbox = con.Send(mesg);
                Response res = con.GetReceiving(inbox);
                if (res.ResponseType != InformationFollowsResponse.ResType)
                {
                    // Invoke display error.
                }
                else
                {
                    InformationFollowsResponse info = (InformationFollowsResponse)res;
                    List<string> toAdd = new List<string>();
                    int index = 0;
                    while (true)
                    {
                        bool complete = info.Complete;
                        int max = 0;
                        lock (info.BodyLinesLock)
                        {
                            max = info.BodyLines.Count;
                        }
                        for (; index < max; index++)
                        {
                            lock (info.BodyLinesLock)
                            {
                                toAdd.Add(info.BodyLines[index]);
                            }
                        }
                        this.Invoke(new DisplayDelegate(Display), (object)toAdd.ToArray());
                        toAdd = new List<string>();
                        if (complete)
                            break;
                        else
                            Thread.Sleep(300);
                    }
                    this.Invoke(new DisplayDelegate(Display), (object)toAdd.ToArray());
                }
            }
            finally
            {
                if (con != null)
                {
                    NntpWorkhorse.Instance.ReturnConnectionAndClose(handle);
                }
                lock (downloaderLock)
                {
                    if (downloader != null)
                    {
                        downloader = null;
                        this.Invoke(new ThreadStart(RevertEnabled));
                    }
                }
            }
        }

        private void stopDownloadButton_Click(object sender, EventArgs e)
        {
            lock (downloaderLock)
            {
                if (downloader != null)
                {
                    downloader.Abort();
                    downloader = null;
                    RevertEnabled();
                }
            }
        }

        private void dataGridViewGroups_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < allLines.Count)
                e.Value = allLines[e.RowIndex];
        }
    }
}