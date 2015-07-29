using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Turbine.DatFileTools;
using System.Runtime.InteropServices;

namespace DatView
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DatFileInitParams parms = new DatFileInitParams();
                parms.FileName = dialog.FileName;
                parms.IsReadOnly = true;
                if (current != null)
                {
                    current.Dispose();
                    current = null;
                }
                current = DatFile.OpenExisting(parms);
                listBox1.BeginUpdate();
                listBox1.Items.Clear();

                foreach (Subfile file in current.Subfiles)
                {
                    listBox1.Items.Add(new FileWrap { File = file });
                }
                listBox1.EndUpdate();
            }
        }
        DatFile current;

        private class FileWrap
        {
            public Subfile File;
            public override string ToString()
            {
                return "" + File.DataID + " " + File.IsCompressed.ToString() + " " + File.Version + " " + File.Iteration + " " + File.Size;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FileWrap wrap = (FileWrap)listBox1.SelectedItem;
            if (!wrap.File.IsCompressed && wrap.File.Size < 10000)
            {
                byte[] data = new byte[wrap.File.Size];
                Marshal.Copy(wrap.File.Data, data, 0, wrap.File.Size);
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == 0)
                    {
                        data[i] = (byte)' ';
                    }
                }
                textBox1.Text = Encoding.ASCII.GetString(data);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in dialog.FileNames)
                {
                    DatFileInitParams parms = new DatFileInitParams();
                    parms.FileName = fileName;
                    parms.IsReadOnly = true;
                    using (DatFile file1 = DatFile.OpenExisting(parms))
                    {
                        HashSet<int> ids = new HashSet<int>();
                        foreach (Subfile part in file1.Subfiles)
                        {
                            ids.Add(part.DataID);
                        }
                        DatFileInitParams parms2 = new DatFileInitParams();
                        parms2.FileName = fileName + ".optimized";
                        parms2.IsReadOnly = true;
                        using (DatFile file2 = DatFile.OpenExisting(parms2))
                        {
                            HashSet<int> ids2 = new HashSet<int>();
                            foreach (Subfile part in file2.Subfiles)
                            {
                                ids2.Add(part.DataID);
                            }
                            if (ids.Count != ids2.Count)
                            {
                                MessageBox.Show(string.Format("{0} does not have correct filecount after optimization.", fileName));
                                continue;
                            }
                            ids.ExceptWith(ids2);
                            if (ids.Count != 0)
                            {
                                MessageBox.Show(string.Format("{0} does not have same file list after optimization.", fileName));
                                continue;
                            }
                            foreach (var filePair in Zip<Subfile, Subfile>(file1.Subfiles, file2.Subfiles))
                            {
                                if (filePair.Key.IsCompressed != filePair.Value.IsCompressed)
                                {
                                    MessageBox.Show(string.Format("{0} has compression not matched for {1}.", fileName, filePair.Key.DataID));
                                    continue;
                                }
                                if (filePair.Key.Version != filePair.Value.Version)
                                {
                                    MessageBox.Show(string.Format("{0} has version not matched for {1}.", fileName, filePair.Key.DataID));
                                    continue;
                                }
                                if (filePair.Key.Size != filePair.Value.Size)
                                {
                                    MessageBox.Show(string.Format("{0} has size not matched for {1}.", fileName, filePair.Key.DataID));
                                    continue;
                                }
                                if (filePair.Key.Iteration != filePair.Value.Iteration)
                                {
                                    MessageBox.Show(string.Format("{0} has iteration not matched for {1}.", fileName, filePair.Key.DataID));
                                    continue;
                                }
                                byte[] origData = new byte[filePair.Key.Size];
                                byte[] newData = new byte[filePair.Key.Size];
                                IntPtr data1 = filePair.Key.Data;
                                IntPtr data2 = filePair.Value.Data;
                                Marshal.Copy(data1, origData, 0, origData.Length);
                                Marshal.Copy(data2, newData, 0, newData.Length);
                                bool same = true;
                                for (int i = 0; i < origData.Length; i++)
                                {
                                    if (origData[i] != newData[i])
                                    {
                                        same = false;
                                        break;
                                    }
                                }
                                if (!same)
                                {
                                    MessageBox.Show(string.Format("{0} has data not matched for {1}.", fileName, filePair.Key.DataID));
                                }
                                filePair.Key.Dispose();
                                filePair.Value.Dispose();
                            }
                        }

                    }
                }
            }

        }
        public static IEnumerable<KeyValuePair<T1, T2>> Zip<T1, T2>(
               IEnumerable<T1> source1,
               IEnumerable<T2> source2)
        {
            using (var iter1 = source1.GetEnumerator())
            using (var iter2 = source2.GetEnumerator())
            {
                while (iter1.MoveNext() && iter2.MoveNext())
                {
                    yield return new KeyValuePair<T1, T2>(iter1.Current, iter2.Current);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DatFileInitParams parms = new DatFileInitParams();
                parms.FileName = dialog.FileName;
                parms.IsReadOnly = true;
                using (DatFile file1 = DatFile.OpenExisting(parms))
                {
                    DatFileInitParams params2 = new DatFileInitParams();
                    params2.FileName = dialog.FileName + ".scrambled";
                    params2.IsReadOnly = false;
                    using (DatFile file2 = DatFile.CreateNew(params2, file1))
                    {
                        Random rnd = new Random();
                        HashSet<int> done = new HashSet<int>();
                        List<Subfile> subFiles = file1.Subfiles.ToList();
                        for (int i = 0; i < subFiles.Count - 1; i++)
                        {
                            int swap = rnd.Next(i, subFiles.Count);
                            if (swap != i)
                            {
                                Subfile temp = subFiles[swap];
                                subFiles[swap] = subFiles[i];
                                subFiles[i] = temp;
                            }
                        }
                        foreach (Subfile part in subFiles)
                        {
                            file2.AddSubfile(part);
                            part.Dispose();
                        }
                    }
                }
            }

        }
    }
}
