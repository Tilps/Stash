using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Resources;
using System.Reflection;
using System.Security.Cryptography;
using System.Linq;

namespace DatOptimize
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Text = "Dat Optimize: " + this.GetType().Assembly.GetName().Version.ToString();
            toolTip1.SetToolTip(button6, "Make Selection Higher Priority");
            toolTip1.SetToolTip(button7, "Make Selection Lower Priority");
            toolTip1.SetToolTip(button5, "Adds a file use profile(s) to the list");
            toolTip1.SetToolTip(button8, "Removes a file use profile from the list");
            toolTip1.SetToolTip(button1, "Create an event profile from Process Monitor data");
            toolTip1.SetToolTip(button2, "Optimize selected install using the profiles");
            toolTip1.SetToolTip(button9, "Navigate to the location of your LOTRO installation");
#if DEBUG
            button4.Visible = true;
            button3.Visible = true;
            button12.Visible = true;
#else
            button4.Visible = false;
            button3.Visible = false;
            button12.Visible = false;
#endif
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select Process Monitor output to process.";
            dialog.Filter = "CSV Files|*.csv";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                backgroundWorker1.RunWorkerAsync(new KeyValuePair<string, int>(dialog.FileName, 1));
                SetUIEnable(false, false);
            }

        }

        private bool ParseResult(string result, out long offset, out long length, out bool hitDisk)
        {
            offset = 0;
            length = 0;
            hitDisk = false;
            if (!result.StartsWith("Offset: "))
                return false;
            result = result.Substring("Offset: ".Length);
            int splitPoint = result.IndexOf(", Length: ");
            offset = long.Parse(result.Substring(0, splitPoint).Replace(",", ""));
            result = result.Substring(splitPoint + ", Length: ".Length);
            int sp2 = result.IndexOf(", I/O Flags: ");
            if (sp2 >= 0)
            {
                length = long.Parse(result.Substring(0, sp2).Replace(",", ""));
                hitDisk = true;
            }
            else
            {
                int sp3 = result.IndexOf(", Priority: ");
                if (sp3 >= 0)
                {
                    length = long.Parse(result.Substring(0, sp3).Replace(",", ""));
                }
                else
                {
                    length = long.Parse(result.Replace(",", ""));
                }
            }
            return true;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            string[] filenames = new string[listBox1.Items.Count];
            for (int i = 0; i < filenames.Length; i++)
            {
                filenames[i] = ((DatProfileEntry)listBox1.Items[i]).Filename;
            }
            string files = string.Join("|", filenames);
            if (string.IsNullOrEmpty(files))
            {
                MessageBox.Show("No profiles selected, no optimization steps to perform.");
                return;
            }
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Installation folder must be selected.");
                return;
            }
            backgroundWorker1.RunWorkerAsync(new KeyValuePair<string, int>(textBox1.Text + "|" + files, 2));
            SetUIEnable(false, false);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "DAT Files|client_*.dat|DAT Files|client_*.dat.orig|DATX Files|client_*.datx";
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string parameter = string.Join("|", dialog.FileNames);
                backgroundWorker1.RunWorkerAsync(new KeyValuePair<string, int>(parameter, 3));
                SetUIEnable(false,false);
            }

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            KeyValuePair<string, int> arg = (KeyValuePair<string, int>)e.Argument;
            int option = arg.Value;
            string inputFileName = arg.Key;
            if (option == 1)
            {
                Dictionary<string, DatFile2> files = new Dictionary<string, DatFile2>();
                try
                {
                    Dictionary<string, List<int>> ids = new Dictionary<string, List<int>>();
                    Dictionary<string, bool> seen = new Dictionary<string, bool>();
                    using (CSV csv = new CSV(inputFileName))
                    {
                        csv.GetRow();
                        while (true)
                        {
                            string[] row = csv.GetRow();
                            if (row == null)
                                break;
                            string path = row[5];
                            string fileName = Path.GetFileName(path);
                            if (!fileName.StartsWith("client_") || (!fileName.EndsWith(".dat") && !fileName.EndsWith(".datx")))
                                continue;
                            string collapsedFile = Collapse(fileName);
                            DatFile2 file;
                            if (!files.TryGetValue(path, out file))
                            {
                                try
                                {
                                    file = new DatFile2(path);
                                    files.Add(path, file);
                                    file.CheckJumpTables();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(string.Format("Error processing {0}, Exception: {1}", path, ex.ToString()));
                                    return;
                                }
                                ids[collapsedFile] = new List<int>();
                            }
                            string result = row[7];
                            long offset;
                            long length;
                            bool hitDisk;
                            if (ParseResult(result, out offset, out length, out hitDisk))
                            {
                                if (!hitDisk)
                                {
                                    int id;
                                    if (file.TryMapToId(offset, out id))
                                    {
                                        string check = fileName + ":" + id.ToString();
                                        if (!seen.ContainsKey(check))
                                        {
                                            ids[collapsedFile].Add(id);
                                            seen.Add(check, true);
                                        }
                                    }
                                }
                            }
                        }
                        e.Result = ids;
                    }
                }
                catch
                {
                    MessageBox.Show("An unexpected error occured while creating a profile.");
                    return;
                }
                finally
                {
                    foreach (DatFile2 file in files.Values)
                    {
                        file.Dispose();
                    }
                }
            }
            if (option == 2)
            {
                Dictionary<string, DatFile2> files = new Dictionary<string, DatFile2>();
                Dictionary<string, DatFile2.DatFileRewriter> writers = new Dictionary<string, DatFile2.DatFileRewriter>();
                bool abort = false;
                try
                {
                    string[] args = inputFileName.Split('|');
                    string basePath = args[0];
                    for (int i = 1; i < args.Length; i++)
                    {
                        string datProfileFile = args[i];
                        using (StreamReader reader = File.OpenText(datProfileFile))
                        {
                            // Skip the description section.
                            string version = reader.ReadLine();
                            if (string.IsNullOrEmpty(version) || version != "1")
                            {
                                MessageBox.Show(string.Format("{0} is not a valid profile.", datProfileFile));
                                abort = true;
                                return;
                            }
                            string description = reader.ReadLine();
                            if (description == null)
                            {
                                MessageBox.Show(string.Format("{0} is not a valid profile.", datProfileFile));
                                abort = true;
                                return;
                            }
                            string graphicsLevel = reader.ReadLine();
                            if (graphicsLevel == null)
                            {
                                MessageBox.Show(string.Format("{0} is not a valid profile.", datProfileFile));
                                abort = true;
                                return;
                            }

                            string currentFile = null;
                            while (true)
                            {
                                string next = reader.ReadLine();
                                if (string.IsNullOrEmpty(next))
                                    break;
                                if (char.IsLetter(next[0]))
                                {
                                    currentFile = next;
                                    continue;
                                }
                                if (currentFile == null)
                                {
                                    MessageBox.Show(string.Format("{0} is not a valid profile.", datProfileFile));
                                    abort = true;
                                    return;
                                }
                                int value;
                                if (!int.TryParse(next, out value))
                                {
                                    MessageBox.Show(string.Format("{0} is not a valid profile.", datProfileFile));
                                    abort = true;
                                    return;
                                }
                                int level = 0;
                                while (true)
                                {
                                    string fullPath = Path.Combine(basePath, currentFile);
                                    if (level > 0)
                                    {
                                        fullPath =  Path.Combine(basePath, Path.GetFileNameWithoutExtension(fullPath) + "_aux_" + level + ".datx");
                                    }
                                    if (!File.Exists(fullPath))
                                        break;
                                    DatFile2 file;
                                    if (!files.TryGetValue(fullPath, out file))
                                    {
                                        try
                                        {
                                            file = new DatFile2(fullPath);
                                            files.Add(fullPath, file);
                                            file.CheckJumpTables();
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(string.Format("Error processing {0}, Exception: {1}", fullPath, ex.ToString()));
                                            abort = true;
                                            return;
                                        }
                                    }
                                    DatFile2.DatFileRewriter datWriter;
                                    if (!writers.TryGetValue(fullPath, out datWriter))
                                    {
                                        try
                                        {
                                            datWriter = file.GetRewriter(fullPath + ".optimized");
                                            writers.Add(fullPath, datWriter);
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(string.Format("Error creating optimized output file {0}, Exception: {1}", fullPath + ".optimized", ex.ToString()));
                                            abort = true;
                                            return;
                                        }
                                    }
                                    try
                                    {
                                        if (datWriter.ForceById(value))
                                            break;
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(string.Format("Error writing to optimized output file {0}, Exception: {1}", datWriter.Filename, ex.ToString()));
                                        abort = true;
                                        return;
                                    }
                                    level++;
                                }
                            }
                            string[] alldats = GetAllDats(basePath);
                            foreach (string fullPath in alldats)
                            {
                                DatFile2 file;
                                if (!files.TryGetValue(fullPath, out file))
                                {
                                    try
                                    {
                                        file = new DatFile2(fullPath);
                                        files.Add(fullPath, file);
                                        file.CheckJumpTables();
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(string.Format("Error processing {0}, Exception: {1}", fullPath, ex.ToString()));
                                        abort = true;
                                        return;
                                    }
                                }
                                DatFile2.DatFileRewriter datWriter;
                                if (!writers.TryGetValue(fullPath, out datWriter))
                                {
                                    try
                                    {
                                        datWriter = file.GetRewriter(fullPath + ".optimized");
                                        writers.Add(fullPath, datWriter);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(string.Format("Error creating optimized output file {0}, Exception: {1}", fullPath + ".optimized", ex.ToString()));
                                        abort = true;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    foreach (DatFile2.DatFileRewriter rewriter in writers.Values)
                    {
                        try
                        {
                            rewriter.Finish();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("Error writing to optimized output file {0}, Exception: {1}", rewriter.Filename, ex.ToString()));
                            abort = true;
                            return;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Unexpected error while optimizing.");
                    abort = true;
                    return;
                }
                finally
                {
                    foreach (DatFile2.DatFileRewriter rewriter in writers.Values)
                    {
                        try
                        {
                            if (abort)
                                rewriter.Cancel();
                            else
                                rewriter.CancelIfNotFinished();
                        }
                        catch
                        {
                        }
                    }
                    foreach (DatFile2 file in files.Values)
                    {
                        file.Dispose();
                    }
                }
            }
            if (option == 3)
            {
                foreach (string fileName in inputFileName.Split('|'))
                {
                    using (DatFile2 file = new DatFile2(fileName))
                    {
                        try
                        {
                            file.CheckJumpTables();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("Error processing {0}, Exception: {1}", fileName, ex.ToString()));
                            return;
                        }
                        using (DatFile2.DatFileRewriter writer = file.GetRewriter(fileName + ".optimized"))
                        {

                        }
                    }
                }
            }
            if (option == 4)
            {
                foreach (string fileName in inputFileName.Split('|'))
                {
                    File.Copy(fileName, fileName + ".compress");
                    using (DatFile2 file = new DatFile2(fileName + ".compress"))
                    {
                        try
                        {
                            file.CheckJumpTables(false);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("Error processing {0}, Exception: {1}", fileName, ex.ToString()));
                            return;
                        }
                        file.DebugFileContent();
                    }
                }
            }
        }

        private string Collapse(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return fileName;
            if (fileName.EndsWith(".datx"))
            {
                return fileName.Substring(0, fileName.IndexOf("_aux_")) + ".dat";
            }
            return fileName;
        }

        private static string[] GetAllDats(string basePath)
        {
            return GetAllDats(basePath, string.Empty);
        }
        private static string[] GetAllDats(string basePath, string extension)
        {
            string[] allstandarddats = Directory.GetFiles(basePath, "client_*.dat"+extension);
            string[] alldatxs = Directory.GetFiles(basePath, "client_*.datx"+extension);
            string[] alldats = new string[allstandarddats.Length + alldatxs.Length];
            Array.Copy(allstandarddats, alldats, allstandarddats.Length);
            Array.Copy(alldatxs, 0, alldats, allstandarddats.Length, alldatxs.Length);
            return alldats;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                Dictionary<string, List<int>> mapping = (Dictionary<string, List<int>>)e.Result;
                DatProfileSave saveDialog = new DatProfileSave();
                saveDialog.Mapping = mapping;
                saveDialog.ShowDialog();
            }
            SetUIEnable(true, false);
            UpdateStatus();
        }

        private void SetUIEnable(bool status, bool collecting)
        {
            button1.Enabled = status;
            button2.Enabled = status;
            button3.Enabled = status;
            button5.Enabled = status;
            button8.Enabled = status;
            button9.Enabled = status;
            button6.Enabled = status;
            button7.Enabled = status;
            button10.Enabled = status;
            button11.Enabled = status || collecting;
            textBox1.Enabled = status;
            listBox1.Enabled = status;
            if (!status)
            {
                timer1.Start();
                panel1.Top = (this.Height - panel1.Height) / 2;
                panel1.Left = (this.Width - panel1.Width) / 2;
                panel1.Visible = true;
                label3.Text = "Working";
                counter = 0;
            }
            else
            {
                timer1.Stop();
                panel1.Visible = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
#if DEBUG
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "CSV|*.csv";
            if (dialog.ShowDialog() == DialogResult.OK)
            {

                Dictionary<string, DatFile2> files = new Dictionary<string, DatFile2>();
                try
                {
                    Dictionary<string, List<int>> ids = new Dictionary<string, List<int>>();
                    List<string> fullPattern = new List<string>();
                    using (CSV csv = new CSV(dialog.FileName))
                    {
                        csv.GetRow();
                        while (true)
                        {
                            string[] row = csv.GetRow();
                            if (row == null)
                                break;
                            string path = row[5];
                            string fileName = Path.GetFileName(path);
                            if (!fileName.StartsWith("client_") || (!fileName.EndsWith(".dat") && !fileName.EndsWith(".datx")))
                                continue;
                            DatFile2 file;
                            if (!files.TryGetValue(path, out file))
                            {
                                try
                                {
                                    file = new DatFile2(path);
                                    files.Add(path, file);
                                    file.CheckJumpTables();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(string.Format("Error processing {0}, Exception: {1}", path, ex.ToString()));
                                    return;
                                }
                                ids[fileName] = new List<int>();
                            }
                            string result = row[7];
                            long offset;
                            long length;
                            bool hitDisk;
                            if (ParseResult(result, out offset, out length, out hitDisk))
                            {
                                if (!hitDisk)
                                {
                                    int id;
                                    if (file.TryMapToId(offset, out id))
                                    {
                                        string check = fileName + ":" + id.ToString();
                                        ids[fileName].Add(id);
                                        fullPattern.Add(check);
                                    }
                                }
                            }
                        }
                    }
                    PatternFinder<string> patterns = new PatternFinder<string>(fullPattern, EqualityComparer<string>.Default);
                    PatternFinder<string>.PatternTree tree = patterns.FindPatterns();
                }
                finally
                {
                    foreach (DatFile2 file in files.Values)
                    {
                        file.Dispose();
                    }
                }
            }
            Viewer view = new Viewer();
            view.Show(this);
#endif
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "Dat Profiles|*.datprofile";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string filename in dialog.FileNames)
                {
                    listBox1.Items.Add(new DatProfileEntry(filename));
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index < 0)
                return;
            listBox1.Items.RemoveAt(index);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index < 1)
                return;
            object temp = listBox1.Items[index];
            listBox1.Items[index] = listBox1.Items[index - 1];
            listBox1.Items[index - 1] = temp;
            listBox1.SelectedIndex = index - 1;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index < 0 || index >= listBox1.Items.Count - 1)
                return;
            object temp = listBox1.Items[index];
            listBox1.Items[index] = listBox1.Items[index + 1];
            listBox1.Items[index + 1] = temp;
            listBox1.SelectedIndex = index + 1;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select LOTRO Installation folder to optimize.";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dialog.SelectedPath;
            }

        }

        private int counter = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            StringBuilder message = new StringBuilder();
            message.Append(new string('.', counter % 10));
            switch (counter / 60)
            {
                case 0:
                    message.Append("Working");
                    break;
                case 1:
                    message.Append("Still Working");
                    break;
                case 2:
                    message.Append("Working some more");
                    break;
                case 3:
                    message.Append("Working ... right???");
                    break;
                case 4:
                    message.Append("Maybe you should come back later?");
                    break;
                case 5:
                    message.Append("It'll finish one day, really");
                    break;
                case 6:
                    message.Append("Better than the hourglass anyway");
                    break;
                case 7:
                    message.Append("Hasn't crashed yet!");
                    break;
                case 8:
                    message.Append("Working, not the same message as before");
                    break;
                default:
                    message.Append("Working, *really*");
                    break;
            }
            message.Append(new string('.', counter % 10));
            label3.Text = message.ToString();
            label3.Refresh();
            counter++;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        private void UpdateStatus()
        {

            if (string.IsNullOrEmpty(textBox1.Text))
            {
                button10.Visible = false;
                label4.Text = string.Empty;
            }
            else
            {
                try
                {
                    if (Directory.Exists(textBox1.Text))
                    {
                        string[] curFiles = GetAllDats(textBox1.Text);
                        if (curFiles.Length == 0)
                        {
                            label4.Text = "Selected location is not a valid LOTRO installation.";
                            button10.Visible = false;
                        }
                        else
                        {
                            string[] origFiles = GetAllDats(textBox1.Text, ".orig");
                            string[] optFiles = GetAllDats(textBox1.Text, ".optimized");
                            if (origFiles.Length > 0 && optFiles.Length > 0)
                            {
                                label4.Text = "LOTRO file status is unclear.";
                                if (optFiles.Length != curFiles.Length)
                                {
                                    label4.Text = label4.Text + " (And not all files have optimized versions.)";
                                }
                                button10.Visible = true;
                                button10.Text = "Use optimized";
                            }
                            else if (origFiles.Length > 0)
                            {
                                label4.Text = "LOTRO file status is optimized.";
                                button10.Visible = true;
                                button10.Text = "Use original";
                            }
                            else if (optFiles.Length > 0)
                            {
                                label4.Text = "LOTRO file status is not optimized.";
                                if (optFiles.Length != curFiles.Length)
                                {
                                    label4.Text = label4.Text + " (And not all files have optimized versions.)";
                                }
                                button10.Visible = true;
                                button10.Text = "Use optimized";
                            }
                            else
                            {
                                button10.Visible = false;
                                label4.Text = "LOTRO file status is unclear (may have never run dat optimize).";
                            }
                        }
                    }
                    else
                    {
                        button10.Visible = false;
                        label4.Text = "Selected location does not exist.";
                    }
                }
                catch
                {
                    button10.Visible = false;
                    label4.Text = "Selected location is not valid.";
                }
            }
            button10.Left = label4.Left + label4.Width + 10;
        }

        private void button10_Click(object sender, EventArgs e)
        {

            try
            {
                if (!Directory.Exists(textBox1.Text))
                    return;
                string[] curFiles = GetAllDats(textBox1.Text);
                string[] origFiles = GetAllDats(textBox1.Text, ".orig"); 
                string[] optFiles = GetAllDats(textBox1.Text, ".optimized"); 
                if (origFiles.Length > 0 && optFiles.Length > 0)
                {
                    if (MessageBox.Show("If you continue existing files will be renamed to '.old' if '.orig' already exists.", ".orig files already exist", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        foreach (string file in curFiles)
                        {
                            if (File.Exists(file + ".optimized"))
                            {
                                if (!File.Exists(file + ".orig"))
                                {
                                    File.Move(file, file + ".orig");
                                    File.Move(file + ".optimized", file);
                                }
                                else
                                {
                                    string targetName = file + ".old";
                                    if (File.Exists(targetName))
                                    {
                                        int counter = 1;
                                        targetName = file + ".old" + counter.ToString();
                                        while (File.Exists(targetName))
                                        {
                                            counter++;
                                            targetName = file + ".old" + counter.ToString();
                                        }
                                    }
                                    File.Move(file, targetName);
                                    File.Move(file + ".optimized", file);
                                }
                            }
                        }
                    }
                }
                else if (origFiles.Length > 0)
                {
                    foreach (string file in curFiles)
                    {
                        if (File.Exists(file + ".orig"))
                        {
                            File.Move(file, file + ".optimized");
                            File.Move(file + ".orig", file);
                        }
                    }
                }
                else if (optFiles.Length > 0)
                {
                    foreach (string file in curFiles)
                    {
                        if (File.Exists(file + ".optimized"))
                        {
                            File.Move(file, file + ".orig");
                            File.Move(file + ".optimized", file);
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("An error occured while moving files.  This is most likely because you do not have permission.");
            }
            UpdateStatus();
        }

        bool collecting = false;

        private void button11_Click(object sender, EventArgs e)
        {
            if (!collecting)
            {
                if (!File.Exists("procmon.exe"))
                {
                    MessageBox.Show("This feature only works if Process Monitor is copied into the same directory.");
                    return;
                }
                if (!File.Exists("LOTROGather.pmc") || (File.GetLastWriteTimeUtc("LOTROGather.pmc") < DateTime.Parse("2011-03-26") && File.GetCreationTimeUtc("LOTROGather.pmc") < DateTime.Parse("2011-03-26")))
                {
                    Assembly currentAssembly = Assembly.GetExecutingAssembly();
                    Stream configStream = currentAssembly.GetManifestResourceStream("DatOptimize.LOTROGather.pmc");
                    byte[] configData = new byte[configStream.Length];
                    configStream.Read(configData, 0, configData.Length);
                    File.WriteAllBytes("LOTROGather.pmc", configData);
                }
                if (File.Exists("LOTROBackingFile.pml"))
                {
                    File.Delete("LOTROBackingFile.pml");
                }
                Process newProc = new Process();
                newProc.StartInfo.FileName = "procmon.exe";
                newProc.StartInfo.Arguments = "/LoadConfig LOTROGather.pmc /AcceptEula /BackingFile LOTROBackingFile.pml /Quiet /Minimized";
                newProc.Start();
                button11.Text = "Stop";
                collecting = true;
                SetUIEnable(false, true);
            }
            else
            {
                Process newProc2 = new Process();
                newProc2.StartInfo.FileName = "procmon.exe";
                newProc2.StartInfo.Arguments = "/Terminate";
                newProc2.Start();
                newProc2.WaitForExit();
                Process newProc3 = new Process();
                newProc3.StartInfo.FileName = "procmon.exe";
                newProc3.StartInfo.Arguments = "/OpenLog LOTROBackingFile.pml /SaveAs ToProcess.csv";
                newProc3.Start();
                newProc3.WaitForExit();
                button11.Text = "Collect Profile";
                collecting = false;
                backgroundWorker1.RunWorkerAsync(new KeyValuePair<string, int>("ToProcess.csv", 1));
                SetUIEnable(false, false);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "DAT Files|client_*.dat|DAT Files|client_*.dat.orig|DATX Files|client_*.datx";
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string parameter = string.Join("|", dialog.FileNames);
                backgroundWorker1.RunWorkerAsync(new KeyValuePair<string, int>(parameter, 4));
                SetUIEnable(false, false);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
#if DEBUG
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "DAT Files|client_*.dat|DAT Files|client_*.dat.orig|DATX Files|client_*.datx";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OpenFileDialog dialog2 = new OpenFileDialog();
                dialog2.Filter = "DAT Files|client_*.dat|DAT Files|client_*.dat.orig|DATX Files|client_*.datx";
                if (dialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (DatFile2 file1 = new DatFile2(dialog.FileName))
                    using (DatFile2 file2 = new DatFile2(dialog2.FileName))
                    {
                        file1.CheckJumpTables();
                        file2.CheckJumpTables();
                        Dictionary<string, DatFile2> dict1 = new Dictionary<string, DatFile2>();
                        dict1.Add(dialog.FileName, file1);
                        GetExtraFiles(dialog, dict1);
                        Dictionary<string, DatFile2> dict2 = new Dictionary<string, DatFile2>();
                        dict2.Add(dialog2.FileName, file2);
                        GetExtraFiles(dialog2, dict2);

                        IEnumerable<KeyValuePair<int, string>> enum1 = MakeBatchFileEnumerator(dict1);
                        IEnumerable<KeyValuePair<int, string>> enum2 = MakeBatchFileEnumerator(dict2);
                        HashSet<int> set1 = new HashSet<int>(enum1.Select(b=>b.Key));
                        if (set1.SetEquals(enum2.Select(b=>b.Key)))
                        {
                            Dictionary<int, string> lookup = new Dictionary<int, string>();
                            foreach (KeyValuePair<int, string> kvp in enum2)
                            {
                                if (lookup.ContainsKey(kvp.Key))
                                    MessageBox.Show(kvp.Key.ToString());
                                else
                                    lookup.Add(kvp.Key, kvp.Value);
                            }
                            foreach (KeyValuePair<int, string> kvp in enum1)
                            {
                                byte[] data1 = dict1[kvp.Value].GetDataById(kvp.Key);
                                byte[] data2 = dict2[lookup[kvp.Key]].GetDataById(kvp.Key);
                                if (data1.Length != data2.Length)
                                {
                                    MessageBox.Show("Different data lengths.");
                                    return;
                                }
                                else
                                {
                                    for (int i=0; i < data1.Length; i++)
                                        if (data1[i] != data2[i])
                                        {
                                            MessageBox.Show("Different data contents.");
                                            return;
                                        }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Different id list.");
                        }
                        MessageBox.Show(Convert.ToBase64String(HashDat(MD5.Create(), dict1)) + "\n\r" +
                            Convert.ToBase64String(HashDat(MD5.Create(), dict2)));
                    }
                }

            }
            return;
#endif
            if (!Directory.Exists(textBox1.Text))
            {
                MessageBox.Show("Must specify an install location.");
                return;
            }
            string[] curFiles = GetAllDats(textBox1.Text);
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
                }
            }
            stream.Flush();
            byte[] megaHash = hasher.ComputeHash(stream.ToArray());
            StringBuilder display = new StringBuilder();
            display.AppendLine("AllFileHash: " + Convert.ToBase64String(megaHash));
            foreach (KeyValuePair<string, byte[]> subHash in sectionHashes)
            {
                display.AppendLine(Path.GetFileName(subHash.Key) + ": " + Convert.ToBase64String(subHash.Value));
            }
            MessageBox.Show(display.ToString());
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
    }


}
