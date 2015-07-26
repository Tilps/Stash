using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using SharpNews.Nntp;
using System.Xml.XPath;

namespace SharpNews
{
    /// <summary>
    /// Provides the main form for the news reader program.
    /// </summary>
    public partial class Main : Form
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Main()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Accessor for the group details panel.
        /// </summary>
        public SplitterPanel GroupDetailsPanel
        {
            get
            {
                return groupActiveSplit.Panel1;
            }
        }

        /// <summary>
        /// Accessor for the active panel.
        /// </summary>
        public SplitterPanel ActivePanel
        {
            get
            {
                return groupActiveSplit.Panel2;
            }
        }

        /// <summary>
        /// Accessor for the group details split.
        /// </summary>
        public SplitContainer GroupDetailsSplit
        {
            get
            {
                return (SplitContainer)GroupDetailsPanel.Controls["groupsDetailsSplit"];
            }
        }

        /// <summary>
        /// Accessor for the group tree panel.
        /// </summary>
        public SplitterPanel GroupTreePanel
        {
            get
            {
                return GroupDetailsSplit.Panel1;
            }
        }

        /// <summary>
        /// Accessor for the details panel.
        /// </summary>
        public SplitterPanel DetailsPanel
        {
            get
            {
                return GroupDetailsSplit.Panel2;
            }
        }

        private void serverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerSettingsForm form = new ServerSettingsForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                NntpWorkhorse.Instance.UpdateSettings();
            }
        }

        private void groupMenuItem_Click(object sender, EventArgs e)
        {
            GroupSettingsForm form = new GroupSettingsForm();
            form.ShowDialog();
        }

        private void nZBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                XPathDocument doc = new XPathDocument(dialog.FileName);
                XPathNavigator nav = doc.CreateNavigator();
                nav.MoveToFirstChild();
                do
                {
                    nav.MoveToNext();
                } while (nav.NodeType != XPathNodeType.Element);
                string nspace = nav.NamespaceURI;
                XmlNamespaceManager manager = new XmlNamespaceManager(nav.NameTable);
                manager.AddNamespace("x", nspace);
                foreach (XPathNavigator node in nav.Select("//x:file", manager))
                {
                    string subj = node.SelectSingleNode("@subject", manager).Value;
                    string[] bits = subj.Split('\x22');
                    string name = subj;
                    // assume post follows standards...
                    if (bits.Length >= 3)
                    {
                        name = bits[bits.Length - 2].Trim();
                    }
                    else
                    {
                        // otherwise lets see what we can do ...
                        string trimmed = subj.Trim();
                        if (trimmed.EndsWith(")"))
                        {
                            int endpos = trimmed.LastIndexOf('(');
                            if (endpos >= 0)
                            {
                                trimmed = trimmed.Substring(0, endpos).Trim();
                            }                            
                        }
                        if (trimmed.ToLower().EndsWith("yenc"))
                        {
                            trimmed = trimmed.Substring(0, trimmed.Length - 4).Trim();
                        }
                        string[] bits2 = trimmed.Split(new string[] { " - " }, StringSplitOptions.None);
                        if (bits2.Length >= 2)
                            name = bits2[bits2.Length - 1].Trim();
                        else
                            name = trimmed;
                    }
                    name = SanitizeFileName(name);
                    List<int> segIndexes = new List<int> ();
                    List<string> segs = new List<string>();
                    foreach (XPathNavigator seg in node.SelectSingleNode("x:segments", manager).Select("x:segment", manager))
                    {
                        segs.Add(seg.Value);
                        segIndexes.Add(int.Parse(seg.SelectSingleNode("@number", manager).Value));
                    }
                    int[] indexes = segIndexes.ToArray();
                    string[] segArts = segs.ToArray();
                    Array.Sort(indexes, segArts);
                    Guid connectionHandle;
                    NntpConnection con = NntpWorkhorse.Instance.GetConnection(false, out connectionHandle);
                    using (FileStream stream = File.Create(name))
                    {
                        Encoder.YEnc.YEncDecoder dec = new SharpNews.Encoder.YEnc.YEncDecoder(stream);
                        if (con != null)
                        {
                            foreach (string segArt in segArts)
                            {
                                try
                                {
                                    con.CheckConnect();
                                }
                                catch
                                {
                                    con.Connect();
                                    con.Authenticate();
                                }
                                int mailBox = con.Send(new ArticleMessage("<" + segArt + ">"));
                                ArticleFollowsResponse res = con.GetResponse(mailBox) as ArticleFollowsResponse;
                                dec.AddPost(res.Body);
                            }
                            NntpWorkhorse.Instance.ReturnConnection(connectionHandle);
                        }
                    }
                }
            }
        }

        private string SanitizeFileName(string name)
        {
            char[] invalids = Path.GetInvalidFileNameChars();
            StringBuilder b = new StringBuilder(name);
            for (int i = 0; i < b.Length; i++)
            {
                if (Array.IndexOf(invalids, b[i]) != -1)
                {
                    b[i] = '_';
                }
            }
            string fixedname = b.ToString();
            name = fixedname;
            int counter = 0;
            while (File.Exists(name))
            {
                name = fixedname + "." + counter.ToString();
                counter++;
            }
            return name;
        }
    }
}