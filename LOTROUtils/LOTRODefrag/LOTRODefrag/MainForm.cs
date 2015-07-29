using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Turbine.DatFileTools;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;

namespace LOTRODefrag
{
    public partial class MainForm : Form
    {
        private const int externalFragsThreshold = 3;

        public MainForm()
        {
            InitializeComponent();
            dataGridViewFiles.Rows.Clear();
            try
            {
                foreach (string str in Directory.GetFiles(Directory.GetCurrentDirectory(), "client_*.dat"))
                {
                    dataGridViewFiles.Rows.Add(false, Path.GetFileName(str), string.Empty, string.Empty, false, string.Empty, false, string.Empty);
                }
            }
            catch
            {
                // GetFiles may throw exception if user has no permissions...
            }
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (backgroundWorkerProcessing.IsBusy)
            {
                backgroundWorkerProcessing.CancelAsync();
                buttonAnalyze.Text = "Stopping";
            }
            else
            {
                Analyze();
            }
        }

        bool analyzedOnce = false;

        private void Analyze()
        {
            buttonAnalyze.Text = "Stop";
            buttonDefrag.Enabled = false;
            backgroundWorkerProcessing.RunWorkerAsync(new AnalyzeArgument(dataGridViewFiles.Rows));
        }

        private long CalcExternalFrags(string str)
        {
            return (long)DefragHelper.GetFileMap(str).Count;
        }

        [DllImport("DatExport.dll")]
        internal static extern int CalculateFragmentation(int handle, out int numFiles, out int numFrags, out byte hint);


        private int InternalHandle(DatFile file)
        {
            return (int)typeof(DatFile).GetProperty("Handle", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(file, null);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is AnalyzeArgument)
            {
                AnalyzeArgument arg = (AnalyzeArgument)e.Argument;
                foreach (string str in arg.Files)
                {
                    if (backgroundWorkerProcessing.CancellationPending)
                        return;
                    AnalyzeFile(str);
                }
                analyzedOnce = true;
            }
            else if (e.Argument is DefragArgument)
            {
                DefragArgument arg = (DefragArgument)e.Argument;
                foreach (string str in arg.Files)
                {
                    if (CheckCancelled(str))
                        return;
                    if (arg.InternalDefrag)
                    {
                        DatDefragFile(str);
                    }
                    else
                    {
                        FileDefrag(str, string.Empty);
                        if (File.Exists(Path.GetFileNameWithoutExtension(str) + "_aux_1.datx"))
                            FileDefrag(str, "_aux_1.datx");
                    }
                }
            }
        }

        private void AnalyzeFile(string fileName)
        {
            if (CheckCancelled(fileName))
                return;
            DatFileInitParams openParams = new DatFileInitParams();
            openParams.FileName = fileName;
            openParams.IsReadOnly = true;
            try
            {
                using (DatFile file = DatFile.OpenExisting(openParams))
                {
                    if (CheckCancelled(fileName))
                        return;
                    int numFiles = 0;
                    int numFrags = 0;
                    byte hint = 0;
                    ReportStatus(fileName, "Analyzing");
                    CalculateFragmentation(InternalHandle(file), out numFiles, out numFrags, out hint);
                    if (CheckCancelled(fileName))
                        return;
                    bool shouldDefrag = ((1 == hint) || ((numFrags - numFiles) > 100));
                    long count = CalcExternalFrags(Path.Combine(Directory.GetCurrentDirectory(), fileName));
                    bool shouldExtDefrag = count > externalFragsThreshold;
                    AnalyzeProgress progress = new AnalyzeProgress(shouldDefrag, fileName, numFrags, numFiles, shouldDefrag, count, shouldExtDefrag, "Analyzed");
                    backgroundWorkerProcessing.ReportProgress(1, progress);
                }
            }
            catch
            {
                ReportStatus(fileName, "Unexpected error while analyzing file.  May not have permissions to access it.");
            }
        }

        private bool CheckCancelled(string fileName)
        {
            if (backgroundWorkerProcessing.CancellationPending)
            {
                ReportStatus(fileName, "Cancelled");
                return true;
            }
            return false;
        }

        private void ReportStatus(string fileName, string status)
        {
            backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, status));
        }

        private void FileDefrag(string fileName, string postfix)
        {
            string targetFile = fileName;
            if (!string.IsNullOrEmpty(postfix))
            {
                targetFile = Path.GetFileNameWithoutExtension(fileName) + postfix;
            }
            string fullPath = Path.GetFullPath(targetFile);
            try
            {
                int retries = 3;
                do
                {
                    if (CheckCancelled(fileName))
                        return;

                    try
                    {
                        backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Finding file locations on disk."));
                        var fileMap = DefragHelper.GetFileMap(fullPath);
                        backgroundWorkerProcessing.ReportProgress(1, new ExtFragsProgress(fileName, fileMap.Count));
                        if (fileMap.Count == 1)
                        {
                            backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Already defragmented."));
                            return;
                        }
                        if (CheckCancelled(fileName))
                            return;
                        string device = GetDeviceForPath(fullPath);
                        backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Finding free space locations on disk."));
                        BitArray driveMap = DefragHelper.GetVolumeMap(device.ToLower());
                        if (CheckCancelled(fileName))
                            return;
                        long fileSize = fileMap[fileMap.Count - 1].Key;
                        if (!TrySimpleFileDefrag(fileName, fullPath, fileMap, device, driveMap, fileSize))
                            if (!FullDefrag(device, fileName, fullPath, fileMap, fileSize, driveMap))
                                return;
                        UpdateExtFragmentationStats(fileName, fullPath);
                        return;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        retries--;
                        if (retries > 0)
                        {
                            backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Free space locations changed while working, restarting."));
                        }
                        else
                        {
                            backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Free space locations changed while working, giving up."));
                            try
                            {
                                // Some changes may have occured, update the analysis.
                                UpdateExtFragmentationStats(fileName, fullPath);
                            }
                            catch
                            {
                            }
                        }
                    }
                } while (retries > 0);
            }
            catch (Exception ex)
            {
                ReportStatus(fileName, "Unexpected error during file system defragmentation.");
            }
        }

        private void UpdateExtFragmentationStats(string fileName, string fullPath)
        {
            if (CheckCancelled(fileName))
                return;
            var fileMap = DefragHelper.GetFileMap(fullPath);
            backgroundWorkerProcessing.ReportProgress(1, new ExtFragsProgress(fileName, fileMap.Count));
        }

        private static string GetDeviceForPath(string fullPath)
        {
            string device = Path.GetPathRoot(fullPath);
            device = device.Substring(0, device.IndexOf(Path.VolumeSeparatorChar) + 1);
            return device;
        }

        private bool FullDefrag(string device, string fileName, string fullPath, List<KeyValuePair<long, long>> fileMap, long fileSize, BitArray driveMap)
        {
            List<KeyValuePair<long, long>> freeMap = new List<KeyValuePair<long, long>>();
            int lastNonFree = -1;
            for (int i = 0; i < driveMap.Count; i++)
            {
                if (driveMap[i])
                {
                    if (lastNonFree < i - 1)
                    {
                        freeMap.Add(new KeyValuePair<long, long>(lastNonFree + 1, i - lastNonFree - 1));
                    }
                    lastNonFree = i;
                }
            }
            if (lastNonFree < driveMap.Count - 1)
            {
                freeMap.Add(new KeyValuePair<long, long>(lastNonFree + 1, driveMap.Count - lastNonFree - 1));
            }
            freeMap.Sort(delegate(KeyValuePair<long, long> a, KeyValuePair<long, long> b) { return -a.Value.CompareTo(b.Value); });
            if (DefragFileFromFreeMap(false, device, fileName, fullPath, fileMap, fileSize, freeMap))
            {
                if (DefragFileFromFreeMap(true, device, fileName, fullPath, fileMap, fileSize, freeMap))
                {
                    backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Defrag Complete"));
                    return true;
                }
            }
            return false;
        }

        private bool DefragFileFromFreeMap(bool moveFiles, string device, string fileName, string fullPath, List<KeyValuePair<long, long>> fileMap, long fileSize, List<KeyValuePair<long, long>> freeMap)
        {
            int count = 0;
            long offset = 0;
            int index = 0;
            int fileMapIndex = 0;
            long lastFileMapStart = 0;
            long overflow = 0;
            while (fileMapIndex < fileMap.Count && index < freeMap.Count && offset < fileSize)
            {
                // Skip pieces which are already done.
                while (fileMapIndex < fileMap.Count && fileMap[fileMapIndex].Key - lastFileMapStart <= overflow)
                {
                    overflow -= fileMap[fileMapIndex].Key - lastFileMapStart;
                    lastFileMapStart = fileMap[fileMapIndex].Key;
                    fileMapIndex++;
                }
                // If leftover piece is larger than the current slot, don't move it into the current slot.
                while (fileMapIndex < fileMap.Count && fileMap[fileMapIndex].Key - lastFileMapStart - overflow > freeMap[index].Value)
                {
                    offset += fileMap[fileMapIndex].Key - lastFileMapStart - overflow;
                    overflow = 0;
                    count++;
                    lastFileMapStart = fileMap[fileMapIndex].Key;
                    fileMapIndex++;
                }
                if (fileMapIndex < fileMap.Count)
                {
                    int lengthToMove = (int)Math.Min(fileSize - offset, freeMap[index].Value);
                    if (moveFiles)
                    {
                        if (CheckCancelled(fileName))
                            return false;
                        backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Moving file data."));
                        DefragHelper.MoveFile(device, fullPath, offset, freeMap[index].Key, lengthToMove);
                    }
                    offset += lengthToMove;
                    overflow += lengthToMove - (fileMap[fileMapIndex].Key - lastFileMapStart);
                    lastFileMapStart = fileMap[fileMapIndex].Key;
                    fileMapIndex++;
                    index++;
                    count++;
                }
            }
            if (!moveFiles)
            {
                if (offset < fileSize)
                {
                    backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Insufficient disk space to defragment."));
                    return false;
                }
                if (count >= fileMap.Count)
                {
                    backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Free space fragmentation too high to defragment."));
                    return false;
                }
            }
            return true;
        }

        private bool TrySimpleFileDefrag(string fileName, string fullPath, List<KeyValuePair<long, long>> fileMap, string device, BitArray driveMap, long fileSize)
        {
            long last = 0;
            foreach (var piece in fileMap)
            {
                long cur = last - 1;
                long logical = piece.Value - 1;
                bool success = true;
                while (cur >= 0)
                {
                    if (driveMap[(int)logical])
                    {
                        success = false;
                        break;
                    }
                    cur--;
                    logical--;
                }
                if (success)
                {
                    cur = piece.Key;
                    logical = piece.Value + (piece.Key - last);
                    while (cur < fileSize)
                    {
                        if (driveMap[(int)logical])
                        {
                            success = false;
                            break;
                        }
                        cur++;
                        logical++;
                    }
                    if (success)
                    {
                        backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Moving file data."));
                        if (last > 0)
                        {
                            if (CheckCancelled(fileName))
                                return false;
                            DefragHelper.MoveFile(device, fullPath, 0, piece.Value - last, (int)last);
                        }
                        if (piece.Key < fileSize)
                        {
                            if (CheckCancelled(fileName))
                                return false;
                            DefragHelper.MoveFile(device, fullPath, piece.Key, piece.Value + piece.Key - last, (int)(fileSize - piece.Key));
                        }
                        backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Defrag Complete"));
                        return true;
                    }
                }
                last = piece.Key;
            }
            return false;
        }

        delegate bool UserQuery();

        private void DatDefragFile(string fileName)
        {
            backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Starting Internal Defragmentation"));
            if (!ValidateFreeSpace(fileName))
            {
                return;
            }
            if (CheckCancelled(fileName))
                return;
            string defragFileName = string.Concat("defrag_", fileName);
            if (!CleanupOldTemp(fileName, defragFileName, string.Empty))
                return;
            if (!CleanupOldTemp(fileName, defragFileName, "_aux_1.datx"))
                return;
            try
            {
                DatFileInitParams initParams = new DatFileInitParams
                {
                    FileName = fileName,
                    IsReadOnly = true
                };
                try
                {
                    using (DatFile sourceFile = DatFile.OpenExisting(initParams))
                    {
                        DatFileInitParams params2 = new DatFileInitParams
                        {
                            FileName = defragFileName,
                            IsReadOnly = false
                        };
                        int lastPercentComplete = 0;
                        Stopwatch watch = new Stopwatch();
                        watch.Start();
                        try
                        {
                            if (CheckCancelled(fileName))
                                return;
                            using (DatFile destFile = DatFile.CreateNew(params2, sourceFile))
                            {
                                foreach (Subfile subfile in sourceFile.Subfiles)
                                {
                                    if (CheckCancelled(fileName))
                                        return;
                                    destFile.AddSubfile(subfile);
                                    subfile.Dispose();
                                    int percentComplete = sourceFile.CachedNumSubfiles == destFile.CachedNumSubfiles ? 100 : (int)(100.0 * (double)destFile.CachedSize / (double)sourceFile.CachedSize);
                                    if (percentComplete != lastPercentComplete || sourceFile.CachedNumSubfiles == destFile.CachedNumSubfiles)
                                    {
                                        backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, string.Format("{0}%, {1:0.00}MB/s", percentComplete, (double)destFile.CachedSize / (double)watch.ElapsedTicks * 10.0)));
                                        lastPercentComplete = percentComplete;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Unable to create new internally defragmented dat file."));
                            return;
                        }
                    }
                }
                catch
                {
                    backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Unable to open existing file."));
                    return;
                }
                if (CheckCancelled(fileName))
                    return;
                string backupFileName = string.Concat("backup_", fileName);
                if (!CleanupOldBackup(fileName, backupFileName, string.Empty))
                    return;
                if (!CleanupOldBackup(fileName, backupFileName, "_aux_1.datx"))
                    return;
                SwitchFiles(fileName, defragFileName, backupFileName, string.Empty);
            }
            finally
            {
                try
                {
                    if (File.Exists(defragFileName))
                    {
                        File.Delete(defragFileName);
                    }
                }
                catch
                {
                    backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Unexpected error while cleaning up temporary file left behind due to previous error."));
                }
            }
        }

        private bool SwitchFiles(string fileName, string defragFileName, string backupFileName, string postfix)
        {
            string targetFile = fileName;
            string targetDefagFile = defragFileName;
            string targetBackupFile = backupFileName;
            if (!string.IsNullOrEmpty(postfix))
            {
                targetFile = Path.GetFileNameWithoutExtension(fileName) + postfix;
                targetDefagFile = Path.GetFileNameWithoutExtension(defragFileName) + postfix;
                targetBackupFile = Path.GetFileNameWithoutExtension(backupFileName) + postfix;
                if (!File.Exists(targetFile))
                    return true;
            }
            try
            {
                backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Renaming old file out of the way."));
                File.Move(targetFile, targetBackupFile);
            }
            catch
            {
                backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Skipping due to unexpected error while renaming to create a backup file."));
                return false;
            }
            try
            {
                backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Moving new file in to place."));
                File.Move(targetDefagFile, targetFile);
                if (string.IsNullOrEmpty(postfix))
                    if (!SwitchFiles(fileName, defragFileName, backupFileName, "_aux_1.datx"))
                    {
                        backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Failed to switch auxillary file."));
                        throw new Exception("Failed to switch aux file.");
                    }
                backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Removing original."));
                File.Delete(targetBackupFile);
                backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Internal Defrag Complete"));
                try
                {
                    backgroundWorkerProcessing.ReportProgress(1, new DatFragsProgress(fileName));
                    if (CheckAdmin())
                    {
                        FileDefrag(fileName, postfix);
                    }
                }
                catch { }
            }
            catch
            {
                backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Unexpected error moving new file in to place, reverting to backup."));
                try
                {
                    if (File.Exists(targetBackupFile))
                    {
                        File.Move(targetBackupFile, targetFile);
                    }
                }
                catch
                {
                    backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "CRITICAL ERROR: Unexpected error reverting to backup, manual intervention may be required."));
                }
                try
                {
                    if (File.Exists(targetDefagFile))
                    {
                        File.Delete(targetDefagFile);
                    }
                }
                catch
                {
                    backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Unexpected error while cleaning up temporary file left behind due to previous error."));
                }
                return false;
            }
            return true;
        }

        private bool CleanupOldBackup(string fileName, string destFileName, string postfix)
        {
            string targetFile = destFileName;
            if (!string.IsNullOrEmpty(postfix))
            {
                targetFile = Path.GetFileNameWithoutExtension(destFileName) + postfix;
            }
            if (File.Exists(targetFile))
            {
                if (CheckCancelled(fileName))
                    return false;
                bool accepted = (bool)this.Invoke(new UserQuery(delegate { return MessageBox.Show(string.Format("Backup file {0} already exists, probably from an aborted previous defrag. If you are sure this file can be safely deleted, click yes to continue.", targetFile), "Delete Old Backup File?", MessageBoxButtons.YesNo) == DialogResult.Yes; }));
                if (accepted)
                {
                    try
                    {
                        File.Delete(targetFile);
                    }
                    catch
                    {
                        backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Unexpected error while deleting pre-existing backup file."));
                        return false;
                    }
                }
                else
                {
                    backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Skipping due to existing backup file."));
                    return false;
                }
            }
            return true;
        }

        private bool CleanupOldTemp(string fileName, string sourceFileName, string postfix)
        {
            string targetFile = sourceFileName;
            if (!string.IsNullOrEmpty(postfix))
            {
                targetFile = Path.GetFileNameWithoutExtension(sourceFileName) + postfix;
            }
            if (File.Exists(targetFile))
            {
                if (CheckCancelled(fileName))
                    return false;
                bool accepted = (bool)this.Invoke(new UserQuery(delegate { return MessageBox.Show(string.Format("File {0} already exists, probably from an aborted previous defrag.  Do you wish to delete it so defragmentation can continue?", targetFile), "Delete Temporary File?", MessageBoxButtons.YesNo) == DialogResult.Yes; }));
                if (accepted)
                {
                    try
                    {
                        File.Delete(targetFile);
                    }
                    catch
                    {
                        backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Unexpected error while deleting pre-existing temporary file."));
                        return false;
                    }
                }
                else
                {
                    backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Skipping due to existing temporary file."));
                    return false;
                }
            }
            return true;
        }

        private bool ValidateFreeSpace(string fileName)
        {
            try
            {
                string pathRoot = Path.GetPathRoot(Path.GetFullPath(fileName));
                int pos = pathRoot.IndexOf(Path.VolumeSeparatorChar);
                if (pos <= 0)
                {
                    backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Unable to determine free disk space."));
                    return false;
                }
                string driveName = pathRoot.Remove(pos);
                DriveInfo driveInfo = new DriveInfo(driveName);
                FileInfo fileInfo = new FileInfo(fileName);
                long num = (long)Math.Round((double)(fileInfo.Length * 1.3));
                if (driveInfo.AvailableFreeSpace < num)
                {
                    backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Insufficient Disk Space"));
                    return false;
                }
                return true;
            }
            catch
            {
                backgroundWorkerProcessing.ReportProgress(1, new StatusProgress(fileName, "Unexpected error while determining free disk space."));
                return false;
            }
        }

        private void backgroundWorkerProcessing_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonAnalyze.Text = "Analyze";
            if (comboBox1.SelectedIndex == 0)
            {
                buttonDefrag.Text = "Full Defrag";
            }
            else
            {
                buttonDefrag.Text = "Filesystem Defrag";
            }
            buttonAnalyze.Enabled = true;
            buttonDefrag.Enabled = true;
        }

        private void backgroundWorkerProcessing_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is AnalyzeProgress)
            {
                AnalyzeProgress arg = (AnalyzeProgress)e.UserState;
                for (int i = 0; i < dataGridViewFiles.Rows.Count; i++ )
                {
                    if ((string)dataGridViewFiles.Rows[i].Cells[1].Value == arg.Name)
                    {
                        bool selected = comboBox1.SelectedIndex == 0 ? arg.RecommendDefrag : arg.ShouldExtDefrag;
                        dataGridViewFiles.Rows[i].SetValues(selected, arg.Name, arg.NumFrags, arg.NumFiles, arg.RecommendDefrag, arg.ExternalFrags, arg.ShouldExtDefrag, arg.Status);
                        break;
                    }
                }
            }
            else if (e.UserState is StatusProgress)
            {
                StatusProgress arg = (StatusProgress)e.UserState;
                for (int i = 0; i < dataGridViewFiles.Rows.Count; i++)
                {
                    if ((string)dataGridViewFiles.Rows[i].Cells[1].Value == arg.Name)
                    {
                        dataGridViewFiles.Rows[i].Cells[7].Value = arg.NewStatus;
                    }
                }
            }
            else if (e.UserState is ExtFragsProgress)
            {
                ExtFragsProgress arg = (ExtFragsProgress)e.UserState;
                for (int i = 0; i < dataGridViewFiles.Rows.Count; i++)
                {
                    if ((string)dataGridViewFiles.Rows[i].Cells[1].Value == arg.Name)
                    {
                        dataGridViewFiles.Rows[i].Cells[5].Value = arg.NewFragCount;
                        bool newState = arg.NewFragCount > externalFragsThreshold;
                        dataGridViewFiles.Rows[i].Cells[6].Value = newState;
                        if (comboBox1.SelectedIndex == 1)
                            dataGridViewFiles.Rows[i].Cells[0].Value = newState;
                    }
                }

            }
            else if (e.UserState is DatFragsProgress)
            {
                DatFragsProgress arg = (DatFragsProgress)e.UserState;
                for (int i = 0; i < dataGridViewFiles.Rows.Count; i++)
                {
                    if ((string)dataGridViewFiles.Rows[i].Cells[1].Value == arg.Name)
                    {
                        dataGridViewFiles.Rows[i].Cells[2].Value = dataGridViewFiles.Rows[i].Cells[3].Value;
                        dataGridViewFiles.Rows[i].Cells[4].Value = false;
                        if (comboBox1.SelectedIndex == 0)
                            dataGridViewFiles.Rows[i].Cells[0].Value = false;
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateColumns();
            UpdateSelected();
            UpdateDefragButton();
        }

        private void UpdateDefragButton()
        {
            if (buttonDefrag.Text == "Stop" || buttonDefrag.Text == "Stopping")
                return;
            if (comboBox1.SelectedIndex == 0)
            {
                buttonDefrag.Text = "Full Defrag";
            }
            else
            {
                buttonDefrag.Text = "Filesystem Defrag";
            }
        }

        private void UpdateSelected()
        {
            for (int i = 0; i < dataGridViewFiles.Rows.Count; i++)
            {
                bool selected = comboBox1.SelectedIndex == 0 ? (bool)dataGridViewFiles.Rows[i].Cells[4].Value : (bool)dataGridViewFiles.Rows[i].Cells[6].Value;
                dataGridViewFiles.Rows[i].Cells[0].Value = selected;
            }
       }

        private void UpdateColumns()
        {
            bool advanced = checkBoxAdvanced.Checked;
            if (comboBox1.SelectedIndex == 0)
            {
                dataGridViewFiles.Columns[2].Visible = advanced;
                dataGridViewFiles.Columns[3].Visible = advanced;
                dataGridViewFiles.Columns[4].Visible = true;
                dataGridViewFiles.Columns[5].Visible = false;
                dataGridViewFiles.Columns[6].Visible = false;
            }
            else
            {
                dataGridViewFiles.Columns[2].Visible = false;
                dataGridViewFiles.Columns[3].Visible = false;
                dataGridViewFiles.Columns[4].Visible = false;
                dataGridViewFiles.Columns[5].Visible = advanced;
                dataGridViewFiles.Columns[6].Visible = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateColumns();
        }

        private void buttonDefrag_Click(object sender, EventArgs e)
        {
            if (backgroundWorkerProcessing.IsBusy)
            {
                backgroundWorkerProcessing.CancelAsync();
                buttonDefrag.Text = "Stopping";
            }
            else
            {
                Defrag(comboBox1.SelectedIndex == 0);
            }

        }

        private void Defrag(bool internalDefrag)
        {
            DefragArgument arg = new DefragArgument(dataGridViewFiles.Rows, internalDefrag);
            if (!arg.Files.GetEnumerator().MoveNext())
            {
                MessageBox.Show("No files are selected for defragmentation.");
                return;
            }
            if (!analyzedOnce)
            {
                if (MessageBox.Show("You have not run analysis yet, are you sure you wish to defragment without checking whether it is needed?", "Are you sure?", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
            }
            else
            {
                if (!CheckDefragSure(internalDefrag))
                {
                    return;
                }
            }
            if (!CheckAdmin())
            {
                if (internalDefrag)
                {
                    if (MessageBox.Show("LOTRO Defrag must be run as administrator in order fix file system fragmentation caused by internal defragmentation.  If you continue file system fragmentation will likely occur.", "Continue?", MessageBoxButtons.YesNo) != DialogResult.Yes) 
                        return;
                }
                else
                {
                    MessageBox.Show("LOTRO Defrag must be run as administrator in order to defragment files.");
                    return;
                }
            }
            buttonDefrag.Text = "Stop";
            buttonAnalyze.Enabled = false;
            backgroundWorkerProcessing.RunWorkerAsync(arg);
        }

        private bool CheckDefragSure(bool internalDefrag)
        {
            bool allMatch = true;
            for (int i = 0; i < dataGridViewFiles.Rows.Count; i++)
            {
                bool suggestedSelected = internalDefrag ? (bool)dataGridViewFiles.Rows[i].Cells[4].Value : (bool)dataGridViewFiles.Rows[i].Cells[6].Value;
                if ((bool)dataGridViewFiles.Rows[i].Cells[0].Value != suggestedSelected)
                {
                    allMatch = false;
                    break;
                }
            }
            if (!allMatch)
            {
                return MessageBox.Show("Selection does not match recommendations, are you sure you wish to defragment the selected files?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes;
            }
            return true;

        }

        private bool CheckAdmin()
        {
            try
            {
                DefragHelper.TryOpenDevice(GetDeviceForPath(Directory.GetCurrentDirectory()));
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void dataGridViewFiles_SelectionChanged(object sender, EventArgs e)
        {
            // Hack:  Can't work out how to disable selection highlighting which is pointless for the usage scenario.  So just unselect anytime something is selected.
            dataGridViewFiles.ClearSelection();
        }
    }
}
