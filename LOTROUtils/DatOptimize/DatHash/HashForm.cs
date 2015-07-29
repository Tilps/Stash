using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DatOptimize;
using System.Security.Cryptography;
using System.Diagnostics;

namespace DatHash
{
    public partial class HashForm : Form
    {
        public HashForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            backgroundWorker1.RunWorkerAsync();
        }

        private static string[] GetAllDats(string basePath)
        {
            return GetAllDats(basePath, string.Empty);
        }
        private static string[] GetAllDats(string basePath, string extension)
        {
            string[] allstandarddats = Directory.GetFiles(basePath, "client_*.dat" + extension);
            string[] alldatxs = Directory.GetFiles(basePath, "client_*.datx" + extension);
            string[] alldats = new string[allstandarddats.Length + alldatxs.Length];
            Array.Copy(allstandarddats, alldats, allstandarddats.Length);
            Array.Copy(alldatxs, 0, alldats, allstandarddats.Length, alldatxs.Length);
            return alldats;
        }

        public string TargetDirectory { get; set; }

        public void HashDats()
        {
            backgroundWorker1.ReportProgress(0, "Starting");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            string[] curFiles = GetAllDats(TargetDirectory);
            Array.Sort(curFiles);
            SortedDictionary<string, byte[]> sectionHashes = new SortedDictionary<string,byte[]>();
            MD5 hasher = MD5.Create();
            MemoryStream stream = new MemoryStream();
            for (int i=0; i < curFiles.Length; i++)
            {
                if (curFiles[i].EndsWith(".dat"))
                {
                    int j = i + 1;
                    while (j < curFiles.Length && curFiles[j].StartsWith(curFiles[i].Substring(0, curFiles[i].Length - 4)))
                        j++;
                    Dictionary<string, DatFile2> files = new Dictionary<string, DatFile2>();
                    for (int k = i; k < j; k++)
                    {
                        DatFile2 datFile =  new DatFile2(curFiles[k]);
                        datFile.CheckJumpTables();
                        files.Add(curFiles[k], datFile);
                    }
                    byte[] result = HashDat(hasher, files);
                    sectionHashes.Add(curFiles[i], result);
                    stream.Write(result, 0, result.Length);
                    foreach (DatFile2 df2 in files.Values)
                        df2.Dispose();
                    backgroundWorker1.ReportProgress((i + 1) * 100 / curFiles.Length, "Progressing: " + (i + 1) * 100 / curFiles.Length + "%");
                }
            }
            stream.Flush();
            byte[] megaHash = hasher.ComputeHash(stream.ToArray());
            watch.Stop();
            StringBuilder display = new StringBuilder();
            display.AppendLine("Total time: " + watch.Elapsed.ToString()); 
            display.AppendLine("AllFileHash: " + Convert.ToBase64String(megaHash));
            foreach (KeyValuePair<string, byte[]> subHash in sectionHashes)
            {
                display.AppendLine(Path.GetFileName(subHash.Key) + ": " + Convert.ToBase64String(subHash.Value));
            }
            backgroundWorker1.ReportProgress(100, display.ToString());
        }

        private static void GetExtraFiles(OpenFileDialog dialog, Dictionary<string, DatFile2> dict1)
        {
            string fileName = Path.GetFileNameWithoutExtension(dialog.FileName);
            string[] files = Directory.GetFiles(Path.GetDirectoryName(dialog.FileName), fileName + "*.datx");
            foreach (string file in files)
            {
                DatFile2 df = new DatFile2(file);
                df.CheckJumpTables();
                dict1.Add(file, df);
            }
        }

        private static byte[] HashDat(MD5 hasher, Dictionary<string, DatFile2> files)
        {
            IEnumerable<KeyValuePair<int, string>> toProcess = MakeBatchFileEnumerator(files);
            MemoryStream fileData = new MemoryStream();
            foreach (KeyValuePair<int, string> entry in toProcess.OrderBy(a => a.Key))
            {
                byte[] dataBit = files[entry.Value].GetDataById(entry.Key);
                byte[] miniHash = hasher.ComputeHash(dataBit);
                fileData.Write(miniHash, 0, miniHash.Length);
            }
            byte[] result = hasher.ComputeHash(fileData.ToArray());
            return result;
        }

        private static IEnumerable<KeyValuePair<int, string>> MakeBatchFileEnumerator(Dictionary<string, DatFile2> files)
        {
            IEnumerable<KeyValuePair<int, string>> toProcess = new List<KeyValuePair<int, string>>();
            foreach (KeyValuePair<string, DatFile2> file in files)
            {
                string fileName = file.Key;
                // file id -65531 appears to be special, contains some data which is dependent on file hash history.
                // Ignore all negative files, I think they are all special.
                toProcess = toProcess.Concat(file.Value.GetFileIdEnumeration().Select(a => new KeyValuePair<int, string>(a, fileName)));
            }
            return toProcess;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            HashDats();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            textBox1.Text = (string)e.UserState;
        }
    }
}
