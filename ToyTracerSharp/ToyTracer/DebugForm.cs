using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ToyTracerLibrary;

namespace ToyTracer
{
    public partial class DebugForm : Form
    {
        public DebugForm()
        {
            InitializeComponent();
        }
#if DEBUG
        public void SetGather(GatherInfo info)
        {
            this.info = info;
            this.treeView1.Nodes.Add(BuildTree(info, "eye"));
        }

        private TreeNode BuildTree(GatherInfo gathered, string name)
        {
            TreeNode node = new TreeNode();
            node.Tag = gathered;
            node.Checked = false;
            node.Text = name + " " + gathered.Weight.ToString();
            foreach (GatherInfo child in gathered.LightChildren)
            {
                node.Nodes.Add(BuildTree(child, "shadow")); 
            }
            foreach (Photon ph in gathered.GatheredPhotons)
            {
                node.Nodes.Add(BuildTree(ph));
            }
            foreach (GatherInfo child in gathered.Children)
            {
                node.Nodes.Add(BuildTree(child, "reflect/refract"));
            }
            return node;
        }

        private TreeNode BuildTree(Photon ph)
        {
            TreeNode node = new TreeNode();
            node.Tag = ph;
            node.Checked = false;
            node.Text = "photon " + ((ph.PhotonColorPower.R+ph.PhotonColorPower.G+ph.PhotonColorPower.B)/3).ToString();
            if (ph.parent != null)
            {
                node.Nodes.Add(BuildTree(ph.parent));
            }
            return node;
        }

        private GatherInfo info;
        public void SetTracer(ToyTracerLibrary.ToyTracer tracer)
        {
            this.tracer = tracer;
        }
        private ToyTracerLibrary.ToyTracer tracer;
        public void SetImage(Bitmap img)
        {
            main = new Bitmap(img);
            backup = img;
            pictureBox1.Image = main;

        }
        private Bitmap main;
        private Bitmap backup;
#endif

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
#if DEBUG
            if (e.Node.Tag is GatherInfo)
            {
                GatherInfo info = (GatherInfo)e.Node.Tag;
                int x;
                int y;
                tracer.Project(info.RealInfo.Normal.Start, out x, out y);
                if (e.Node.Checked)
                {
                    main.SetPixel(x, y, Color.White);
                }
                else
                {
                    main.SetPixel(x, y, backup.GetPixel(x, y));
                }
            }
            else if (e.Node.Tag is Photon)
            {
                Photon ph = (Photon)e.Node.Tag;
                ToyTracerLibrary.Point p = new ToyTracerLibrary.Point();
                p.X = ph.HitPos.X;
                p.Y = ph.HitPos.Y;
                p.Z = ph.HitPos.Z;
                int x;
                int y;
                tracer.Project(p, out x, out y);
                if (e.Node.Checked)
                {
                    main.SetPixel(x, y, Color.White);
                }
                else
                {
                    main.SetPixel(x, y, backup.GetPixel(x, y));
                }
            }
            if (e.Node.Checked && e.Node.Parent != null && !e.Node.Parent.Checked)
                e.Node.Parent.Checked = true;
            else if (!e.Node.Checked && e.Node.Nodes.Count > 0)
            {
                bool foundOne = false;
                foreach (TreeNode node in e.Node.Nodes)
                {
                    if (node.Checked)
                    {
                        foundOne = true;
                        node.Checked = false;
                    }
                }
                if (!foundOne)
                    pictureBox1.Refresh();
            }
            else
                pictureBox1.Refresh();

#endif
        }
    }
}