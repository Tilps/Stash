using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using LOTRODPSEvil;
using System.Net;
using System.Net.Sockets;
using System.Drawing.Imaging;

namespace LOTRODPSEvil
{
    /// <summary>
    /// Main DPSEvil form.
    /// </summary>
    public partial class DPSEvilForm : Form
    {

        private static readonly string lotroFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "The Lord of the Rings Online");
        /// <summary>
        /// Constructor.
        /// </summary>
        public DPSEvilForm()
        {
            InitializeComponent();
            FileSystemWatcher watcher = new FileSystemWatcher(lotroFolder, "*.txt");
            watcher.EnableRaisingEvents = true;
            watcher.Created += new FileSystemEventHandler(watcher_CreatedOrChanged);
            watcher.Changed += new FileSystemEventHandler(watcher_CreatedOrChanged);
            t = new System.Threading.Timer(Update);
            t.Change(0, 1000);
            frameWriter = new System.Threading.Timer(WriteFrame); 
            frameWriter.Change(0, 100);
            // Force display of settings to start with since we ned the name.
            button1_Click(this, EventArgs.Empty);
        }
        System.Threading.Timer t;
        int updatePeriod = 1000;
        int averagePeriod = 15000;
        System.Threading.Timer frameWriter;


        void WriteFrame(object state)
        {
            if (Directory.Exists(Path.Combine(lotroFolder, "Plugins\\TMD")))
            {
                if (dedicatedServer)
                    return;
                string hackData = FormatDetails();
                try
                {
                    using (FileStream file = File.Open(Path.Combine(lotroFolder, "Plugins\\TMD\\Hack.plugin"), FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                    using (StreamWriter writer = new StreamWriter(file))
                    {
                        writer.Write(string.Format(@"<?xml version=""1.0""?>
<Plugin>
	<Information>
		<Name>HackData</Name>
		<Author>{0}</Author>
		<Version>0.1</Version>
	</Information>
	<Package>TMD.DpsDisplay.Main</Package>
</Plugin>", hackData));
                    }
                }
                catch
                {
                }
            }
        }

        private string FormatDetails()
        {
            StringBuilder result = new StringBuilder();
            Format(result, "self", evilDPSArea1);
            lock (areas)
            {
                foreach (EvilDPSArea area in areas)
                {
                    Format(result, null, area);
                }
            }
            return result.ToString();
        }

        private void Format(StringBuilder result, string p, EvilDPSArea evilDPSArea1)
        {
            if (p == null)
            {
                result.Append(";");
                result.Append(evilDPSArea1.DisplayName);
            }
            else
            {
                result.Append(p);
            }
            result.Append(";");
            float dout, din, hout, hin, dpout, dpin;
            evilDPSArea1.GetCurrent(out dout, out din, out hout, out hin, out dpout, out dpin);
            result.Append(dout);
            result.Append(";");
            result.Append(din);
            result.Append(";");
            result.Append(hout);
            result.Append(";");
            result.Append(hin);
            result.Append(";");
            result.Append(dpout);
            result.Append(";");
            result.Append(dpin);
        }

        /// <summary>
        /// Updates the plugindata file with the latest averages.
        /// </summary>
        /// <param name="state">
        /// Unused Argument.
        /// </param>
        void Update(object state)
        {
            FixTotals();
            evilDPSArea1.UpdateAsync(
Average(outgoingDamageTotal),
   Average(incomingDamageTotal),
   Average(outgoingHealsTotal),
   Average(incomingHealsTotal),
   Average(outgoingPetDamageTotal),
   Average(incomingPetDamageTotal));
            lock (remoteClientLock)
            {
                if (remoteClient == null)
                {
                    if (remotePort > -1)
                        RemoteConnect();
                }
                if (remoteClient != null)
                {
                    try
                    {
                        BinaryWriter writer = new BinaryWriter(remoteClient.GetStream());
                        writer.Write(updatePeriod);
                        writer.Write(averagePeriod);
                        writer.Write(Average(outgoingDamageTotal));
                        writer.Write(Average(incomingDamageTotal));
                        writer.Write(Average(outgoingHealsTotal));
                        writer.Write(Average(incomingHealsTotal));
                        writer.Write(Average(outgoingPetDamageTotal));
                        writer.Write(Average(incomingPetDamageTotal));
                    }
                    catch
                    {
                        try
                        {
                            RemoteDisconnect();
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Converts a damage into an average.
        /// </summary>
        /// <param name="damage">
        /// Damageto average assuming it was collected over the current averagePeriod.
        /// </param>
        /// <returns>
        /// Average damage for the averaging period.
        /// </returns>
        private float Average(int damage)
        {
            return (float)damage / ((float)averagePeriod / 1000.0f);
        }

        /// <summary>
        /// Remove stale damage entries to give a rolling average.
        /// </summary>
        private void FixTotals()
        {
            lock (times)
            {
                while (times.Count > 0 && times.Peek() < DateTime.UtcNow.Subtract(TimeSpan.FromMilliseconds(averagePeriod)))
                {
                    times.Dequeue();
                    incomingDamageTotal -= incomingDamage.Dequeue();
                    outgoingDamageTotal -= outgoingDamage.Dequeue();
                    incomingPetDamageTotal -= incomingPetDamage.Dequeue();
                    outgoingPetDamageTotal -= outgoingPetDamage.Dequeue();
                    incomingHealsTotal -= incomingHeals.Dequeue();
                    outgoingHealsTotal -= outgoingHeals.Dequeue();
                }
            }
        }


        /// <summary>
        /// Ensures a thread exists to handle every chat log file which is currently being modified.
        /// </summary>
        /// <param name="sender">
        /// Event sender.
        /// </param>
        /// <param name="e">
        /// File system event args indicating which file has changed or been created.
        /// </param>
        void watcher_CreatedOrChanged(object sender, FileSystemEventArgs e)
        {
            lock (workers)
            {
                if (!workers.ContainsKey(e.FullPath))
                {
                    Thread t = new Thread(DoWork);
                    t.IsBackground = true;
                    t.Start(e.FullPath);
                    workers[e.FullPath] = t;
                }
            }
        }
        Dictionary<string, Thread> workers = new Dictionary<string, Thread>();


        Queue<int> incomingDamage =  new Queue<int>();
        int incomingDamageTotal = 0;
        Queue<int> outgoingDamage = new Queue<int>();
        int outgoingDamageTotal = 0;
        Queue<int> incomingPetDamage = new Queue<int>();
        int incomingPetDamageTotal = 0;
        Queue<int> outgoingPetDamage = new Queue<int>();
        int outgoingPetDamageTotal = 0;
        Queue<int> incomingHeals = new Queue<int>();
        int incomingHealsTotal = 0;
        Queue<int> outgoingHeals = new Queue<int>();
        int outgoingHealsTotal = 0;
        Queue<DateTime> times = new Queue<DateTime>();


        /// <summary>
        /// Background thread method to watch a chat log and process any updates.
        /// </summary>
        /// <remarks>
        /// Uses polling with increased sleep waits when no changes happened recently.
        /// </remarks>
        /// <param name="arg">
        /// Argument, contains the filename.
        /// </param>
        private void DoWork(object arg)
        {
            try
            {

                string filename = (string)arg;
                long lastLoc = -1;
                int spins = 1;
                while (true)
                {
                    bool fastSpin = true;
                    // Open file with full share since LOTRO might be writing to it...
                    using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            fastSpin = ProcessFile(reader, lastLoc);
                            lastLoc = reader.BaseStream.Position;
                        }
                    }
                    if (!fastSpin)
                    {
                        System.Threading.Thread.Sleep(spins);
                        if (spins < 100)
                        {
                            spins++;
                        }
                    }
                    else
                    {
                        spins = 1;
                    }
                }
            }
            catch
            {
            }

        }

        /// <summary>
        /// Clears the damage queues, useful for when the averaging period changes.
        /// </summary>
        private void ResetCombatCounters()
        {
            lock (times)
            {
                incomingDamage.Clear();
                incomingDamageTotal = 0;
                outgoingDamage.Clear();
                outgoingDamageTotal = 0;
                incomingPetDamage.Clear();
                incomingPetDamageTotal = 0;
                outgoingPetDamage.Clear();
                outgoingPetDamageTotal = 0;
                incomingHeals.Clear();
                incomingHealsTotal = 0;
                outgoingHeals.Clear();
                outgoingHealsTotal = 0;
                times.Clear();
            }
        }

        /// <summary>
        /// Gets the latest entries from a file.
        /// </summary>
        /// <param name="reader">
        /// Stream reader over the file we want to process.
        /// </param>
        /// <param name="lastLoc">
        /// Location of the end of the last entry that was processed.
        /// </param>
        /// <returns>
        /// True if any new entries were found, false otherwise.
        /// </returns>
        private bool ProcessFile(StreamReader reader, long lastLoc)
        {
            bool initialRead = true;
            // Skip whatever is in the file when it is selected.
            if (lastLoc == -1)
                reader.BaseStream.Seek(0, SeekOrigin.End);
            else
                reader.BaseStream.Seek(lastLoc, SeekOrigin.Begin);
            while (true)
            {
                long beforePos = reader.BaseStream.Position;
                string line = reader.ReadLine();
                long afterPos = reader.BaseStream.Position;
                if (string.IsNullOrEmpty(line))
                {
                    if (initialRead)
                    {
                        return false;
                    }
                    return true;
                }
                else if (beforePos + line.Length == afterPos)
                {
                    // TODO: error notification.
                }
                if (initialRead)
                {
                    initialRead = false;
                }

                int outgoingDamageDelta, incomingDamageDelta, outgoingPetDamageDelta, incomingPetDamageDelta, outgoingHealsDelta, incomingHealsDelta;
                if (ParseCombat(line, out outgoingDamageDelta, out incomingDamageDelta, out outgoingPetDamageDelta, out incomingPetDamageDelta, out outgoingHealsDelta, out incomingHealsDelta))
                {
                    lock (times)
                    {
                        times.Enqueue(DateTime.UtcNow);
                        outgoingDamageTotal += outgoingDamageDelta;
                        outgoingDamage.Enqueue(outgoingDamageDelta);
                        incomingDamageTotal += incomingDamageDelta;
                        incomingDamage.Enqueue(incomingDamageDelta);
                        outgoingPetDamageTotal += outgoingPetDamageDelta;
                        outgoingPetDamage.Enqueue(outgoingPetDamageDelta);
                        incomingPetDamageTotal += incomingPetDamageDelta;
                        incomingPetDamage.Enqueue(incomingPetDamageDelta);
                        outgoingHealsTotal += outgoingHealsDelta;
                        outgoingHeals.Enqueue(outgoingHealsDelta);
                        incomingHealsTotal += incomingHealsDelta;
                        incomingHeals.Enqueue(incomingHealsDelta);
                    }
                    continue;
                }
            }
        }

        /// <summary>
        /// Process a chat log file looking for combat information.
        /// </summary>
        /// <param name="line">
        /// Line to process.
        /// </param>
        /// <param name="outgoingDamageDelta">
        /// Receives the outgoing damage delt.
        /// </param>
        /// <param name="incomingDamageDelta">
        /// Receives the incoming damage received.
        /// </param>
        /// <param name="outgoingPetDamageDelta">
        /// Receives the outgoing damage delt by a pet.
        /// </param>
        /// <param name="incomingPetDamageDelta">
        /// Receives the incoming damage delt to a pet.
        /// </param>
        /// <param name="outgoingHealsDelta">
        /// Receives the outgoing healing done by self.
        /// </param>
        /// <param name="incomingHealsDelta">
        /// Receives the incoming healing done to self.
        /// </param>
        /// <returns>
        /// True if the input line looked like combat, false otherwise.
        /// </returns>
        private bool ParseCombat(string line, out int outgoingDamageDelta, out int incomingDamageDelta, out int outgoingPetDamageDelta, out int incomingPetDamageDelta, out int outgoingHealsDelta, out int incomingHealsDelta)
        {
            outgoingDamageDelta = 0;
            incomingDamageDelta = 0;
            outgoingPetDamageDelta = 0;
            incomingPetDamageDelta = 0;
            outgoingHealsDelta = 0;
            incomingHealsDelta = 0;
            if (line.Contains(":") || line.Contains("[") || line.Contains("]"))
                return false;

            string[] petNames = this.petNames.Split(',');

            Match result = Regex.Match(line, "(.*) scored a (.*)hit with (.*) on (.*) for (.*) to (.*).");
            if (result.Success)
            {
                string source = result.Groups[1].Value;
                string destination = result.Groups[4].Value;
                string damage = result.Groups[5].Value;
                string type = result.Groups[6].Value;
                int damageNum = ExtractNumber(damage);
                if (type == "Morale")
                {
                    if (string.Compare(source, characterName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        outgoingDamageDelta = damageNum;
                    }
                    else if (string.Compare(destination, characterName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        incomingDamageDelta = damageNum;
                    }
                    else if (IsPet(source, petNames))
                    {
                        outgoingPetDamageDelta = damageNum;
                    }
                    else if (IsPet(destination, petNames))
                    {
                        incomingPetDamageDelta = damageNum;
                    }
                }
                return true;
            }
            Match result2 = Regex.Match(line, "(.*) applied a (.*)heal with (.*) to (.*) restoring (.*) to (.*).");
            if (result2.Success)
            {
                string source = result2.Groups[1].Value;
                string destination = result2.Groups[4].Value;
                string damage = result2.Groups[5].Value;
                string type = result2.Groups[6].Value;
                int damageNum = ExtractNumber(damage);
                if (type == "Morale")
                {
                    if (string.Compare(source, characterName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        outgoingHealsDelta = damageNum;
                    }
                    if (string.Compare(destination, characterName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        incomingHealsDelta = damageNum;
                    }
                }
                return true;
            }

            if (line.StartsWith("You wound "))
            {
                outgoingDamageDelta = ExtractNumber(line);
                return true;
            }
            if (line.StartsWith("You hit ") && line.Contains("to Morale"))
            {
                outgoingDamageDelta = ExtractNumber(line);
                return true;
            }
            if (line.Contains(" wounds you "))
            {
                incomingDamageDelta = ExtractNumber(line);
                return true;
            }
            if (line.StartsWith("You are wounded "))
            {
                incomingDamageDelta = ExtractNumber(line);
                return true;
            }
            // Skirmish.
            if (line.Contains(" wounds "))
            {
                int pos = line.IndexOf(" wounds ");
                string name = line.Substring(0, pos);
                if (Array.IndexOf(petNames, name) != -1)
                {
                    outgoingPetDamageDelta = ExtractNumber(line);
                    return true;
                }
                else
                {
                    int start = pos + " wounds ".Length;
                    string nextWord = line.Substring(start, line.IndexOf(' ', start) - start);
                    if (Array.IndexOf(petNames, nextWord) != -1)
                    {
                        incomingPetDamageDelta = ExtractNumber(line);
                        return true;
                    }
                }

                return true;
            }
            // Skirmish.
            if (line.Contains(" is wounded "))
            {
                int pos = line.IndexOf(" is wounded ");
                string name = line.Substring(0, pos);
                if (Array.IndexOf(petNames, name) != -1)
                {
                    incomingPetDamageDelta = ExtractNumber(line);
                    return true;
                }
                return true;
            }
            if (line.StartsWith("You partially avoided "))
            {
                incomingDamageDelta = ExtractNumber(line);
                return true;
            }
            if (line.StartsWith("You partially hit "))
            {
                outgoingDamageDelta = ExtractNumber(line);
                return true;
            }
            if (line.StartsWith("You reflect "))
            {
                outgoingDamageDelta = ExtractNumber(line);
                return true;
            }
            if (line.Contains(" reflects ") && line.Contains(" Morale back to you."))
            {
                incomingDamageDelta = ExtractNumber(line);
                return true;
            }
            if (line.Contains(" reflects "))
            {
                return true;
            }
            // Skirmish.
            if (line.Contains(" attack was partially avoided by "))
            {
                int pos = line.IndexOf(" attack was partially avoided by ");
                string name = line.Substring(0, pos);
                if (Array.IndexOf(petNames, name) != -1)
                {
                    outgoingPetDamageDelta = ExtractNumber(line);
                    return true;
                }
                else
                {
                    int start = pos + " attack was partially avoided by ".Length;
                    string nextWord = line.Substring(start, line.IndexOf(' ', start) - start);
                    if (Array.IndexOf(petNames, nextWord) != -1)
                    {
                        incomingPetDamageDelta = ExtractNumber(line);
                        return true;
                    }
                }
                return true;
            }
            // Skirmish.
            if (line.Contains(" partially hits "))
            {
                int pos = line.IndexOf(" partially hits ");
                string name = line.Substring(0, pos);
                if (Array.IndexOf(petNames, name) != -1)
                {
                    outgoingPetDamageDelta = ExtractNumber(line);
                    return true;
                }
                else
                {
                    int start = pos + " partially hits ".Length;
                    string nextWord = line.Substring(start, line.IndexOf(' ', start) - start);
                    if (Array.IndexOf(petNames, nextWord) != -1)
                    {
                        incomingPetDamageDelta = ExtractNumber(line);
                        return true;
                    }
                }

                return true;
            }
            if (line.Contains(" heals you ") && line.Contains(" Morale"))
            {
                incomingHealsDelta = ExtractNumber(line);
                return true;
            }
            if (line.Contains(" heals you ") && line.Contains(" Power "))
            {
                return true;
            }
            // Heals.
            if (line.Contains(" heals "))
            {
                return true;
            }
            if (line.StartsWith("You heal ") && line.Contains(" of your wounds"))
            {
                outgoingHealsDelta = ExtractNumber(line);
                incomingHealsDelta = ExtractNumber(line);
                return true;
            }
            if (line.StartsWith("You heal ") && line.Contains(" points of wounds "))
            {
                outgoingHealsDelta = ExtractNumber(line);
                return true;
            }
            // Self heals.
            if (line.StartsWith("You heal ") && line.Contains(" Morale"))
            {
                outgoingHealsDelta = ExtractNumber(line);
                return true;
            }
            if (line.StartsWith("You heal ") && line.Contains(" Power "))
            {
                return true;
            }
            if (line.StartsWith("You heal "))
            {
                return true;
            }
            // Power drains.
            if (line.Contains(" hit") && line.Contains(" Power"))
            {
                return true;
            }
            return false;
        }

        private bool IsPet(string destination, string[] petNames)
        {
            return Array.FindIndex(petNames, a => string.Compare(destination, a, StringComparison.OrdinalIgnoreCase) == 0) >= 0;
        }

        private static int ExtractNumber(string line)
        {
            return int.Parse(Regex.Match(line, "[0-9]?[0-9,]*[0-9]").Value, System.Globalization.NumberStyles.Number);
        }

        private string petNames = "Archer,Warrior,Bannerguard,Protector,Sage,Herbalist";

        private void button1_Click(object sender, EventArgs e)
        {
            EvilSettings settings = new EvilSettings();
            settings.PetNames = petNames;
            settings.UpdatePeriod = updatePeriod;
            settings.AveragePeriod = averagePeriod;
            settings.ServerPort = serverPort;
            settings.RemotePort = remotePort;
            settings.RemoteHost = remoteHost;
            settings.RemoteName = remoteName;
            settings.DedicatedServerMode = dedicatedServer;
            settings.CharacterName = characterName;
            if (settings.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                updatePeriod = settings.UpdatePeriod;
                petNames = settings.PetNames;
                averagePeriod = settings.AveragePeriod;
                serverPort = settings.ServerPort;
                remotePort = settings.RemotePort;
                remoteHost = settings.RemoteHost;
                remoteName = settings.RemoteName;
                dedicatedServer = settings.DedicatedServerMode;
                characterName = settings.CharacterName;
                ResetCombatCounters();
                UpdateServing();
                t.Change(0, updatePeriod);
            }
            if (string.IsNullOrEmpty(characterName))
            {
                MessageBox.Show("DPS parsing will not function successfully until you specify a character name.");
            }
        }
        private string characterName = string.Empty;

        private bool dedicatedServer = false;

        private void UpdateServing()
        {
            if (serverPort > -1)
            {
                ServerConnect();
            }
            else
            {
                ServerDisconnect();
            }
            if (remotePort > -1)
            {
                RemoteConnect();
            }
            else
            {
                RemoteDisconnect();
            }
        }

        private void ServerDisconnect()
        {
            if (serverListen != null)
            {
                serverListen.Stop();
                serverListen = null;
            }
        }

        private void ServerConnect()
        {
            if (serverListen != null)
                ServerDisconnect();
            if (serverListen == null)
            {
                serverListen = new TcpListener(IPAddress.Any, serverPort);
                serverListen.Start();
                serverListen.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClient), serverListen);
            }
        }
        TcpListener serverListen;

        private void AcceptTcpClient(IAsyncResult result)
        {
            try
            {
                TcpListener listener = (TcpListener)result.AsyncState;
                TcpClient newRemote = listener.EndAcceptTcpClient(result);
                AddRemote(newRemote);
                if (serverListen != null)
                {
                    serverListen.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClient), serverListen);
                }
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private class RemoteReaderState
        {
            public byte[] buffer;
            public byte[] dataSoFar;
            public TcpClient client;
            public bool seenHeader;
            public EvilDPSArea area;
            public Guid connectId;
            public string name;
        }

        private void AddRemote(TcpClient newRemote)
        {
            newRemote.NoDelay = true;
            RemoteReaderState state = new RemoteReaderState();
            state.client = newRemote;
            state.buffer = new byte[1024];
            newRemote.GetStream().BeginRead(state.buffer, 0, 1024, ReceiveRemoteData, state);
        }

        private void ReceiveRemoteData(IAsyncResult result)
        {
            RemoteReaderState state = (RemoteReaderState)result.AsyncState;
            int length = 0;
            try
            {
                length = state.client.GetStream().EndRead(result);
                if (length == 0)
                {
                    CloseRemoteConnection(state);
                    return;
                }
            }
            catch
            {
                CloseRemoteConnection(state);
                return;
            }
            if (state.dataSoFar != null)
            {
                byte[] newData = new byte[state.dataSoFar.Length + length];
                Array.Copy(state.dataSoFar, newData, state.dataSoFar.Length);
                Array.Copy(state.buffer, 0, newData, state.dataSoFar.Length, length);
                state.dataSoFar = newData;
            }
            else
            {
                byte[] newData = new byte[length];
                Array.Copy(state.buffer, 0, newData, 0, length);
                state.dataSoFar = newData;
            }
            bool more = false;
            do
            {
                more = false;
                using (MemoryStream stream = new MemoryStream(state.dataSoFar))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    bool okay = true;
                    if (state.seenHeader)
                    {
                        try
                        {
                            int updatePeriod = reader.ReadInt32();
                            int averagePeriod = reader.ReadInt32();
                            float dpsOut = reader.ReadSingle();
                            float dpsIn = reader.ReadSingle();
                            float hOut = reader.ReadSingle();
                            float hIn = reader.ReadSingle();
                            float pdOut = reader.ReadSingle();
                            float pdIn = reader.ReadSingle();
                            if (dedicatedServer)
                            {
                                BroadcastDedicated(state, updatePeriod, averagePeriod, dpsOut, dpsIn, hOut, hIn, pdOut, pdIn);
                            }
                            else
                            {
                                UpdateAreaAsync(state.area, updatePeriod, averagePeriod, dpsOut, dpsIn, hOut, hIn, pdOut, pdIn);
                            }
                        }
                        catch
                        {
                            okay = false;
                        }
                    }
                    else
                    {
                        try
                        {
                            Guid shouldBeIntro = new Guid(reader.ReadBytes(16));
                            if (shouldBeIntro != introGuid)
                            {
                                state.client.Client.Close();
                                return;
                            }
                            string name = reader.ReadString();
                            state.connectId = Guid.NewGuid();
                            state.name = name;
                            if (dedicatedServer)
                            {
                                AddDedicatedRemoteConnection(state);
                            }
                            else
                            {
                                state.area = RegisterNewAreaSync(state.name);
                            }
                            state.seenHeader = true;
                        }
                        catch
                        {
                            okay = false;
                        }
                    }
                    if (okay)
                    {
                        if (stream.Position < state.dataSoFar.Length)
                        {
                            byte[] nextArray = new byte[state.dataSoFar.Length - stream.Position];
                            Array.Copy(state.dataSoFar, stream.Position, nextArray, 0, nextArray.Length);
                            state.dataSoFar = nextArray;
                            more = true;
                        }
                        else
                        {
                            state.dataSoFar = null;
                        }
                    }
                }
            } while (more);
            try
            {
                state.client.GetStream().BeginRead(state.buffer, 0, 1024, ReceiveRemoteData, state);
            }
            catch
            {
                CloseRemoteConnection(state);
            }
        }

        private void BroadcastDedicated(RemoteReaderState state, int updatePeriod, int averagePeriod, float dpsOut, float dpsIn, float hOut, float hIn, float pdOut, float pdIn)
        {
            lock (connections)
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    if (state.connectId != connections[i].connectId)
                    {
                        BinaryWriter writer = new BinaryWriter(connections[i].client.GetStream());
                        writer.Write(stateUpdateGuid.ToByteArray());
                        writer.Write(state.connectId.ToByteArray());
                        writer.Write(updatePeriod);
                        writer.Write(averagePeriod);
                        writer.Write(dpsOut);
                        writer.Write(dpsIn);
                        writer.Write(hOut);
                        writer.Write(hIn);
                        writer.Write(pdOut);
                        writer.Write(pdIn);
                    }
                }
            }
        }
        private static readonly Guid stateUpdateGuid = new Guid("{5364B8FA-BD21-4B35-BDD4-EBA088423659}");
        private List<RemoteReaderState> connections = new List<RemoteReaderState>();
        private void AddDedicatedRemoteConnection(RemoteReaderState state)
        {
            lock (connections)
            {
                connections.Add(state);
                BinaryWriter newWriter = new BinaryWriter(state.client.GetStream());
                for (int i = 0; i < connections.Count - 1; i++)
                {
                    BinaryWriter writer = new BinaryWriter(connections[i].client.GetStream());
                    writer.Write(helloGuid.ToByteArray());
                    writer.Write(state.connectId.ToByteArray());
                    writer.Write(state.name);
                    newWriter.Write(helloGuid.ToByteArray());
                    newWriter.Write(connections[i].connectId.ToByteArray());
                    newWriter.Write(connections[i].name);
                }
            }
        }
        private static readonly Guid helloGuid = new Guid("{E67EA613-FA13-4821-BA2C-1D50E1B72661}");

        private void CloseRemoteConnection(RemoteReaderState state)
        {
            if (dedicatedServer)
            {
                RemoveDedicatedRemoteConnection(state);
            }
            else
            {
                RemoveAreaAsync(state.area);
            }
        }

        private void RemoveDedicatedRemoteConnection(RemoteReaderState state)
        {
            lock (connections)
            {
                connections.Remove(state);
                for (int i = 0; i < connections.Count; i++)
                {
                    BinaryWriter writer = new BinaryWriter(connections[i].client.GetStream());
                    writer.Write(byeGuid.ToByteArray());
                    writer.Write(state.connectId.ToByteArray());
                }
            }
        }
        private static readonly Guid byeGuid = new Guid("{92828397-58EC-45C8-B2DF-6B97E61A98B1}");

        private void RemoveAreaAsync(EvilDPSArea evilDPSArea)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(delegate() { RemoveAreaAsync(evilDPSArea); }));
                return;
            }
            this.Controls.Remove(evilDPSArea);
            this.areas.Remove(evilDPSArea);
            RefreshLayout();
        }

        private void UpdateAreaAsync(EvilDPSArea evilDPSArea, int updatePeriod, int averagePeriod, float dpsOut, float dpsIn, float hOut, float hIn, float pdOut, float pdIn)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(delegate() { UpdateAreaAsync(evilDPSArea, updatePeriod, averagePeriod, dpsOut, dpsIn, hOut, hIn, pdOut, pdIn); }));
                return;
            }
            try
            {
                evilDPSArea.UpdateAsync(dpsOut, dpsIn, hOut, hIn, pdOut, pdIn);
            }
            catch
            {
            }
        }

        private EvilDPSArea RegisterNewAreaSync(string name)
        {
            if (this.InvokeRequired)
            {
                EvilDPSArea result = null;
                this.Invoke(new MethodInvoker(delegate() { result = RegisterNewAreaSync(name); }));
                return result;
            }
            EvilDPSArea newArea = new EvilDPSArea();
            newArea.DisplayName = name;
            this.Controls.Add(newArea);
            newArea.Left = evilDPSArea1.Left;
            newArea.Width = evilDPSArea1.Width;
            newArea.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            newArea.SizeChanged += new EventHandler(newArea_SizeChanged);
            areas.Add(newArea);
            RefreshLayout();
            return newArea;
        }

        void newArea_SizeChanged(object sender, EventArgs e)
        {
            RefreshLayout();
        }

        private void RefreshLayout()
        {
            int lastTop = evilDPSArea1.Bottom;
            foreach (EvilDPSArea area in areas)
            {
                area.Top = lastTop;
                lastTop = area.Bottom;
            }
            this.MaximumSize = new Size(this.MaximumSize.Width, lastTop + this.Size.Height - this.ClientSize.Height);
            this.MinimumSize = new Size(this.MinimumSize.Width, lastTop + this.Size.Height - this.ClientSize.Height);
            this.ClientSize = new Size(this.ClientSize.Width, lastTop);
        }
        List<EvilDPSArea> areas = new List<EvilDPSArea>();


        private void RemoteDisconnect()
        {
            lock (remoteClientLock)
            {
                if (remoteClient != null)
                {
                    try
                    {
                        remoteClient.Client.Close();
                    }
                    catch
                    {
                    }
                    remoteClient = null;
                }
            }
        }

        private void RemoteConnect()
        {
            try
            {
                lock (remoteClientLock)
                {
                    if (remoteClient != null)
                    {
                        // TODO: only do this if hostname/port has changed or connection is dead.
                        RemoteDisconnect();
                    }
                    if (remoteClient == null)
                    {
                        TcpClient newClient = new TcpClient(remoteHost, remotePort);
                        newClient.NoDelay = true;
                        BinaryWriter writer = new BinaryWriter(newClient.GetStream());
                        writer.Write(introGuid.ToByteArray());
                        writer.Write(remoteName);
                        remoteClient = newClient;
                        RemoteReaderState state = new RemoteReaderState();
                        state.buffer = new byte[1024];
                        state.client = remoteClient;
                        newClient.GetStream().BeginRead(state.buffer, 0, 1024, new AsyncCallback(RemoteClientReceive), state);
                    }
                }
            }
            catch
            {
            }
        }

        private void RemoteClientReceive(IAsyncResult result)
        {
            RemoteReaderState state = (RemoteReaderState)result.AsyncState;
            int length = 0;
            try
            {
                length = state.client.GetStream().EndRead(result);
                if (length == 0)
                {
                    CloseRemoteClientConnection(state);
                    return;
                }
            }
            catch
            {
                CloseRemoteClientConnection(state);
                return;
            }
            if (state.dataSoFar != null)
            {
                byte[] newData = new byte[state.dataSoFar.Length + length];
                Array.Copy(state.dataSoFar, newData, state.dataSoFar.Length);
                Array.Copy(state.buffer, 0, newData, state.dataSoFar.Length, length);
                state.dataSoFar = newData;
            }
            else
            {
                byte[] newData = new byte[length];
                Array.Copy(state.buffer, 0, newData, 0, length);
                state.dataSoFar = newData;
            }
            bool more = false;
            do
            {
                more = false;
                using (MemoryStream stream = new MemoryStream(state.dataSoFar))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    bool okay = true;
                    try
                    {
                        Guid packetId = new Guid(reader.ReadBytes(16));
                        Guid connectId = new Guid(reader.ReadBytes(16));
                        if (packetId == helloGuid)
                        {
                            string name = reader.ReadString();
                            AddToAreaDictionary(connectId, RegisterNewAreaSync(name));
                        }
                        else if (packetId == byeGuid)
                        {
                            RemoveAreaAsync(GetAreaForConnect(connectId));
                            RemoveFromAreaDictionary(connectId);
                        }
                        else if (packetId == stateUpdateGuid)
                        {
                            int updatePeriod = reader.ReadInt32();
                            int averagePeriod = reader.ReadInt32();
                            float dpsOut = reader.ReadSingle();
                            float dpsIn = reader.ReadSingle();
                            float hOut = reader.ReadSingle();
                            float hIn = reader.ReadSingle();
                            float pdOut = reader.ReadSingle();
                            float pdIn = reader.ReadSingle();
                            UpdateAreaAsync(GetAreaForConnect(connectId), updatePeriod, averagePeriod, dpsOut, dpsIn, hOut, hIn, pdOut, pdIn);
                        }
                    }
                    catch
                    {
                        okay = false;
                    }
                    if (okay)
                    {
                        if (stream.Position < state.dataSoFar.Length)
                        {
                            byte[] nextArray = new byte[state.dataSoFar.Length - stream.Position];
                            Array.Copy(state.dataSoFar, stream.Position, nextArray, 0, nextArray.Length);
                            state.dataSoFar = nextArray;
                            more = true;
                        }
                        else
                        {
                            state.dataSoFar = null;
                        }
                    }
                }
            } while (more);
            try
            {
                state.client.GetStream().BeginRead(state.buffer, 0, 1024, RemoteClientReceive, state);
            }
            catch
            {
                CloseRemoteClientConnection(state);
            }
        }

        private Dictionary<Guid, EvilDPSArea> areaLookup = new Dictionary<Guid, EvilDPSArea>();

        private void RemoveFromAreaDictionary(Guid connectId)
        {
            lock (areaLookup)
            {
                areaLookup.Remove(connectId);
            }
        }

        private EvilDPSArea GetAreaForConnect(Guid connectId)
        {
            lock (areaLookup)
            {
                return areaLookup[connectId];
            }
        }

        private void AddToAreaDictionary(Guid connectId, EvilDPSArea evilDPSArea)
        {
            lock (areaLookup)
            {
                areaLookup.Add(connectId, evilDPSArea);
            }
        }

        private void CloseRemoteClientConnection(RemoteReaderState state)
        {
            lock (areaLookup)
            {
                foreach (KeyValuePair<Guid, EvilDPSArea> kvp in areaLookup)
                {
                    RemoveAreaAsync(kvp.Value);
                }
                areaLookup.Clear();
            }
        }     
        
        TcpClient remoteClient;
        object remoteClientLock = new object();

        private static readonly Guid introGuid = new Guid("{AE841BD3-3B44-443B-9B4A-7F24907E1E27}");

        private int serverPort = -1;
        private int remotePort = -1;
        private string remoteHost = null;
        private string remoteName = null;

        private void evilDPSArea1_SizeChanged(object sender, EventArgs e)
        {
            RefreshLayout();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form simpleForm = new Form();
            simpleForm.Text = "Graphs";
            simpleForm.Width = 361;
            simpleForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            int currentX = 5;
            List<EvilDPSGraph> allGraphs = new List<EvilDPSGraph>();
            currentX = AddAreaToGraphForm(simpleForm, currentX, evilDPSArea1, allGraphs);
            foreach (EvilDPSArea area in areas)
            {
                currentX = AddAreaToGraphForm(simpleForm, currentX, area, allGraphs);
            }
            simpleForm.Height = currentX + simpleForm.Height-simpleForm.ClientSize.Height + 2;
            foreach (EvilDPSGraph graph in allGraphs)
            {
                EvilDPSGraph localGraph = graph;
                graph.Click += delegate(object senderNest, EventArgs args)
                {
                    foreach (EvilDPSGraph graph2 in allGraphs)
                    {
                        if (localGraph == graph2)
                            continue;
                        graph2.SimulClick();
                    }
                };
            }
            EventHandler invalidator = new EventHandler(delegate(object senderNest, EventArgs args)
            {
                foreach (EvilDPSGraph graph in allGraphs)
                {
                    graph.Invalidate();
                }
            });
            evilDPSArea1.Updated += invalidator;
            foreach (EvilDPSArea area in areas)
            {
                area.Updated += invalidator;
            }
            simpleForm.Show();
            simpleForm.Closed += delegate(object senderNest, EventArgs args)
            {
                evilDPSArea1.Updated -= invalidator;
                foreach (EvilDPSArea area in areas)
                {
                    area.Updated -= invalidator;
                }
            };
        }

        private static int AddAreaToGraphForm(Form simpleForm, int currentX, EvilDPSArea area, List<EvilDPSGraph> allGraphs)
        {
            Label l = new Label();
            l.Text = string.IsNullOrEmpty(area.DisplayName) ? "Self" : area.DisplayName;
            simpleForm.Controls.Add(l);
            l.Top = currentX + 1;
            l.Left = 3;
            currentX += l.Height;
            foreach (var entry in area.GetShownAsGraph())
            {
                string name = entry.Key;
                Label l2 = new Label();
                l2.Text = name;
                simpleForm.Controls.Add(l2);
                l2.Top = currentX + 1;
                l2.Left = 3;
                simpleForm.Controls.Add(entry.Value);
                entry.Value.Top = currentX;
                entry.Value.Left = 51;
                entry.Value.Width = 300;
                entry.Value.Height = 80;
                currentX += entry.Value.Height+2;
                allGraphs.Add(entry.Value);
            }
            return currentX;
        }
    }
}
