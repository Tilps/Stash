using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using DatOptimize;

namespace DatHash
{
    public partial class CompareForm : Form
    {
        public CompareForm()
        {
            InitializeComponent();
        }

        public string TargetDirectory { get; set; }

        public string TargetFile { get; set; }

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

        public void HashDats()
        {
            backgroundWorker1.ReportProgress(0, "Starting...");
            StringBuilder finalDisplay = new StringBuilder();
            using (StreamReader input = File.OpenText(TargetFile))
            {
                string compareFileName = input.ReadLine();
                if (string.IsNullOrEmpty(compareFileName) || !compareFileName.EndsWith(".dat"))
                {
                    backgroundWorker1.ReportProgress(100, "Selected file is not a valid template.");
                    return;
                }
                string[] curFiles = GetAllDats(TargetDirectory);
                Array.Sort(curFiles);
                MD5 hasher = MD5.Create();
                for (int i = 0; i < curFiles.Length; i++)
                {
                    if (curFiles[i].EndsWith(".dat"))
                    {
                        string curFileName = Path.GetFileName(curFiles[i]);
                        while (curFileName.CompareTo(compareFileName) > 0)
                        {
                            finalDisplay.AppendLine(compareFileName + ": Not in install directory");
                            compareFileName = SkipToNext(input);
                        }
                        if (curFileName.CompareTo(compareFileName) < 0)
                        {
                            finalDisplay.AppendLine(curFileName + ": Not in template");
                            continue;
                        }
                        int j = i + 1;
                        while (j < curFiles.Length && curFiles[j].StartsWith(curFiles[i].Substring(0, curFiles[i].Length - 4)))
                            j++;
                        Dictionary<string, DatFile2> files = new Dictionary<string, DatFile2>();
                        for (int k = i; k < j; k++)
                        {
                            DatFile2 datFile = new DatFile2(curFiles[k]);
                            datFile.CheckJumpTables();
                            files.Add(curFiles[k], datFile);
                        }
                        IEnumerable<KeyValuePair<int, string>> toProcess = MakeBatchFileEnumerator(files);
                        HashSet<int> missingFromInstall = new HashSet<int>();
                        HashSet<int> missingFromTemplate = new HashSet<int>();
                        HashSet<int> correct = new HashSet<int>();
                        HashSet<int> wrongLength = new HashSet<int>();
                        HashSet<int> wrongHash = new HashSet<int>();
                        string templateRow = NextLine(input, ref compareFileName);
                        int templateId;
                        int templateLength;
                        byte[] templateHash;
                        ProcessRow(templateRow, out templateId, out templateLength, out templateHash);
                        foreach (KeyValuePair<int, string> entry in toProcess.OrderBy(a => a.Key))
                        {
                            while (templateId < entry.Key)
                            {
                                missingFromInstall.Add(templateId);
                                templateRow = NextLine(input, ref compareFileName);
                                ProcessRow(templateRow, out templateId, out templateLength, out templateHash);
                            }
                            if (templateId > entry.Key)
                            {
                                missingFromTemplate.Add(entry.Key);
                                continue;
                            }
                            byte[] dataBit = files[entry.Value].GetDataById(entry.Key);
                            if (dataBit.Length != templateLength)
                            {
                                wrongLength.Add(entry.Key);
                            }
                            else
                            {
                                byte[] miniHash = hasher.ComputeHash(dataBit);
                                bool success = true;
                                for (int l = 0; l < miniHash.Length; l++)
                                {
                                    if (miniHash[l] != templateHash[l])
                                    {
                                        wrongHash.Add(entry.Key);
                                        success = false;
                                        break;
                                    }
                                }
                                if (success)
                                    correct.Add(entry.Key);
                            }
                            templateRow = NextLine(input, ref compareFileName);
                            ProcessRow(templateRow, out templateId, out templateLength, out templateHash);
                        }
                        while (templateRow != null)
                        {
                            missingFromInstall.Add(templateId);
                            templateRow = NextLine(input, ref compareFileName);
                            ProcessRow(templateRow, out templateId, out templateLength, out templateHash);
                        }

                        foreach (DatFile2 df2 in files.Values)
                            df2.Dispose();
                        finalDisplay.AppendLine(string.Format("{0}: {1} correct, {2} extra, {3} missing, {4} wrong length, {5} wrong hash",
                            curFileName, correct.Count, missingFromTemplate.Count, missingFromInstall.Count, wrongLength.Count, wrongHash.Count));
                        backgroundWorker1.ReportProgress((i + 1) * 100 / curFiles.Length, "Progressing: " + (i + 1) * 100 / curFiles.Length + "%");
                    }
                }
            }
            backgroundWorker1.ReportProgress(100, finalDisplay.ToString());
        }

        private static void ProcessRow(string templateRow, out int templateId, out int templateLength, out byte[] templateHash)
        {
            if (templateRow == null)
            {
                templateId = int.MaxValue;
                templateLength = 0;
                templateHash = null;
            }
            else
            {
                string[] bits = templateRow.Split(':');
                templateId = int.Parse(bits[0]);
                templateLength = int.Parse(bits[1]);
                templateHash = Convert.FromBase64String(bits[2]);
            }
        }

        private string NextLine(StreamReader input, ref string compareFileName)
        {
            string value = input.ReadLine();
            if (value == null || value.EndsWith(".dat"))
            {
                compareFileName = value ?? "ZZZZZZZZZZZZZZZZ";
                return null;
            }
            return value;
        }

        private string SkipToNext(StreamReader input)
        {
            string value = input.ReadLine();
            while (!string.IsNullOrEmpty(value) && !value.EndsWith(".dat"))
                value = input.ReadLine();
            if (string.IsNullOrEmpty(value))
                return "ZZZZZZZZZZZZ";
            return value;
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
