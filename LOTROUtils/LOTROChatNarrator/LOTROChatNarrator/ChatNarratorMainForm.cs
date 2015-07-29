using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.IO;
using System.Text.RegularExpressions;
using LOTROChatNarrator.Properties;
using System.Threading;

namespace LOTROChatNarrator
{
    public partial class ChatNarratorMainForm : Form
    {
        public ChatNarratorMainForm()
        {
            InitializeComponent();
            this.Text = this.Text + " (" + Settings.Default.LastVersionSave + ")";
            synth.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(synth_SpeakCompleted);
            ChatSettingsForm.ConfigureVoice(synth, Settings.Default.VoiceName, Settings.Default.Speed, Settings.Default.Volume, Settings.Default.DefaultOutput, Settings.Default.CustomOutput);
        }

        private static int currentlySpeaking = 0;

        public static void Speak(SpeechSynthesizer synth, string message)
        {
            synth.SpeakAsync(message);
            Interlocked.Increment(ref currentlySpeaking);
        }

        void synth_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            Interlocked.Decrement(ref currentlySpeaking);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
            }
            else
            {
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    buttonSpeak.Text = "Stop Speaking";
                    backgroundWorker1.RunWorkerAsync(dialog.FileName);
                }
            }
        }
        System.Speech.Synthesis.SpeechSynthesizer synth = new SpeechSynthesizer();

        DateTime lastDPSRead = DateTime.UtcNow;
        int incomingDamage = 0;
        int outgoingDamage = 0;
        int incomingPetDamage = 0;
        int outgoingPetDamage = 0;
        int incomingHeals = 0;
        int outgoingHeals = 0;


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                lastDPSRead = DateTime.UtcNow;
                ResetCombatCounters();

                string filename = (string)e.Argument;
                long lastLoc = -1;
                int spins = 1;
                while (true)
                {
                    CheckForSpeed();
                    CheckForDPSRead();
                    CheckForDelay();
                    bool fastSpin = true;
                    using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            fastSpin = ProcessFile(reader, lastLoc);
                            if (backgroundWorker1.CancellationPending)
                                break;
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
                // If we're told to exit, kill the async messages or they'll go on forever...
                synth.SpeakAsyncCancelAll();
                currentlySpeaking = 0;
            }
            catch (Exception ex)
            {
                synth.SpeakAsyncCancelAll();
                currentlySpeaking = 0;
                Speak(synth, ex.ToString());
            }

        }

        private void ResetCombatCounters()
        {
            incomingDamage = 0;
            outgoingDamage = 0;
            incomingPetDamage = 0;
            outgoingPetDamage = 0;
            incomingHeals = 0;
            outgoingHeals = 0;
        }

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
                    Speak(synth, "Error: Partial line.");
                }
                if (initialRead)
                {
                    initialRead = false;
                }

                CheckForSpeed();
                CheckForDPSRead();
                CheckForDelay();
                if (combatSupport)
                {
                    int outgoingDamageDelta, incomingDamageDelta, outgoingPetDamageDelta, incomingPetDamageDelta, outgoingHealsDelta, incomingHealsDelta;
                    if (ParseCombat(line, out outgoingDamageDelta, out incomingDamageDelta, out outgoingPetDamageDelta, out incomingPetDamageDelta, out outgoingHealsDelta, out incomingHealsDelta))
                    {
                        if (line.ToLowerInvariant().Contains("(distributed)"))
                        {
                            string announce = Settings.Default.DistributedAnnounce;
                            if (!string.IsNullOrEmpty(announce))
                                Speak(synth, announce);
                        }
                        outgoingDamage += outgoingDamageDelta;
                        incomingDamage += incomingDamageDelta;
                        outgoingPetDamage += outgoingPetDamageDelta;
                        incomingPetDamage += incomingPetDamageDelta;
                        outgoingHeals += outgoingHealsDelta;
                        incomingHeals += incomingHealsDelta;
                        continue;
                    }
                }
                if (excludeIncludes != null)
                {
                    bool fail = false;
                    bool success = false;
                    CheckExcludesIncludes(line, ref fail, ref success);
                    if (fail || (whitelist && !success))
                    {
                        continue;
                    }
                }
                if (filterSelf && line.ToLowerInvariant().StartsWith("[to "))
                    continue;
                if (skipChannelNames && line.StartsWith("["))
                    line = line.Substring(line.IndexOf("]") + 1);
                if (replacements != null)
                {
                    line = HandleReplacements(line);
                }
                Speak(synth, line);
                if (backgroundWorker1.CancellationPending)
                    break;
            }
            return true;
        }

        private void CheckForDelay()
        {
            for (int i = pendingDelayedMessages.Count - 1; i >= 0; i--)
            {
                KeyValuePair<DateTime, string> kvp = pendingDelayedMessages[i];
                if (kvp.Key < DateTime.UtcNow)
                {
                    Speak(synth, kvp.Value);
                    pendingDelayedMessages.RemoveAt(i);
                }
            }
        }

        private void CheckForSpeed()
        {
            if (Settings.Default.AccelerateVoice)
            {
                synth.Rate = Math.Min(Settings.Default.Speed + currentlySpeaking, 10);
            }
            else
            {
                synth.Rate = Settings.Default.Speed;
            }
        }

        private void CheckExcludesIncludes(string line, ref bool fail, ref bool success)
        {
            foreach (string matcher in excludeIncludes)
            {
                if (string.IsNullOrEmpty(matcher))
                    continue;
                if (useRegex)
                {
                    try
                    {
                        Regex regex = new Regex(matcher, RegexOptions.IgnoreCase);
                        if (regex.IsMatch(line))
                        {
                            if (!whitelist)
                            {
                                fail = true;
                                break;
                            }
                            else
                            {
                                success = true;
                                break;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                else
                {
                    if (line.ToLowerInvariant().IndexOf(matcher.ToLowerInvariant()) != -1)
                    {
                        if (!whitelist)
                        {
                            fail = true;
                            break;
                        }
                        else
                        {
                            success = true;
                            break;
                        }
                    }
                }
            }
        }

        private string HandleReplacements(string line)
        {
            foreach (string rep in replacements)
            {
                string[] bits = rep.Split(':');
                if (bits.Length == 2)
                {
                    int offset = 0;
                    int found = 0;
                    do
                    {
                        found = line.IndexOf(bits[0], offset);
                        offset = found + 1;
                        if (found > 0 && char.IsLetter(line[found - 1]))
                        {
                            continue;
                        }
                        if (found + bits[0].Length < line.Length - 1 && char.IsLetter(line[found + bits[0].Length]))
                        {
                            continue;
                        }
                        if (found >= 0)
                        {
                            offset = found + bits[1].Length;
                            line = line.Substring(0, found) + bits[1] + line.Substring(found + bits[0].Length);
                        }
                    } while (found >= 0 && offset < line.Length);
                }
            }
            return line;
        }

        private DateTime lastNonZero = DateTime.UtcNow;

        private void CheckForDPSRead()
        {
            DateTime now = DateTime.UtcNow;
            // If its been 15 seconds of silence and we still haven't taken any damage or delt damage, start the timer from next time we do either instead of at random.
            if (outgoingDamage == 0 && incomingDamage == 0 && now.Subtract(lastNonZero) > TimeSpan.FromSeconds(Settings.Default.CombatReportPeriod+5))
            {
                lastDPSRead = now;
            }
            if (combatSupport)
            {
                TimeSpan ellapsed = now.Subtract(lastDPSRead);
                if (ellapsed > TimeSpan.FromSeconds(Settings.Default.CombatReportPeriod))
                {
                    string toSay = string.Empty;
                    if (Settings.Default.ReportDPS && outgoingDamage > 0)
                    {
                        toSay += ((int)(outgoingDamage / ellapsed.TotalSeconds)).ToString() + " D P S out";
                    }
                    if (Settings.Default.ReportDPSIn && incomingDamage > 0)
                    {
                        if (!string.IsNullOrEmpty(toSay))
                            toSay += " and ";
                        toSay += ((int)(incomingDamage / ellapsed.TotalSeconds)).ToString() + " D P S in";
                    }
                    if (Settings.Default.ReportPetDPS && outgoingPetDamage > 0)
                    {
                        if (!string.IsNullOrEmpty(toSay))
                            toSay += " and ";
                        toSay += ((int)(outgoingPetDamage / ellapsed.TotalSeconds)).ToString() + " pet D P S out";
                    }
                    if (Settings.Default.ReportPetDPSIn && incomingPetDamage > 0)
                    {
                        if (!string.IsNullOrEmpty(toSay))
                            toSay += " and ";
                        toSay += ((int)(incomingPetDamage / ellapsed.TotalSeconds)).ToString() + " pet D P S in";
                    }
                    if (Settings.Default.ReportHealingOut && outgoingHeals > 0)
                    {
                        if (!string.IsNullOrEmpty(toSay))
                            toSay += " and ";
                        toSay += ((int)(outgoingHeals / ellapsed.TotalSeconds)).ToString() + " H P S out";
                    }
                    if (Settings.Default.ReportHealingIn && incomingHeals > 0)
                    {
                        if (!string.IsNullOrEmpty(toSay))
                            toSay += " and ";
                        toSay += ((int)(incomingHeals / ellapsed.TotalSeconds)).ToString() + " H P S in";
                    }
                    if (!string.IsNullOrEmpty(toSay))
                    {
                        lastNonZero = now;
                        Speak(synth, toSay);
                    }
                    ResetCombatCounters();
                    lastDPSRead = now;
                }
            }
        }

        List<KeyValuePair<DateTime, string>> pendingDelayedMessages = new List<KeyValuePair<DateTime, string>>();

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
            string[] timedelays = Settings.Default.TimeDelayMessages.Split(new char[] { '\r','\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string timedelay in timedelays)
            {
                string[] bits = timedelay.Split(':');
                if (line.Contains(bits[0]))
                {
                    int delay;
                    if (int.TryParse(bits[1], out delay))
                    {
                        pendingDelayedMessages.Add(new KeyValuePair<DateTime, string>(DateTime.UtcNow.Add(TimeSpan.FromSeconds(delay)), bits[2]));
                    }
                }
            }
            string[] petNames = Settings.Default.PetNames.Split(',');
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
                    int start = pos+" wounds ".Length;
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
            return Settings.Default.AssumeNonChatIsCombat;
        }

        private static int ExtractNumber(string line)
        {
            return int.Parse(Regex.Match(line, "[0-9]?[0-9,]*[0-9]").Value, System.Globalization.NumberStyles.Number);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonSpeak.Text = "Start Speaking";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            filterSelf = checkBoxFilterSelf.Checked;
        }

        private bool filterSelf = false;

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            skipChannelNames = checkBoxFilterChannels.Checked;
        }

        private bool skipChannelNames = false;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            replacements = textBoxSubstitutions.Lines;
        }
        string[] replacements = null;

        private void button1_Click_1(object sender, EventArgs e)
        {
            ChatSettingsForm dialog = new ChatSettingsForm(synth);
            dialog.ShowDialog();
            //int count = 0;
            //foreach (InstalledVoice voice in synth.GetInstalledVoices())
            //{
            //    if (voice.VoiceInfo.Name == synth.Voice.Name)
            //        continue;
            //    count++;
            //    if (MessageBox.Show(voice.VoiceInfo.Name, "This voice?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            //    {
            //        try
            //        {
            //            synth.SelectVoice(voice.VoiceInfo.Name);
            //            break;
            //        }
            //        catch
            //        {
            //            MessageBox.Show("That voice is not compatible, sorry.");
            //        }
            //    }
            //}
            //if (count == 0)
            //{
            //    MessageBox.Show("There are no other voices installed on this system.");
            //}
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            excludeIncludes = textBox1.Lines;
        }
        string[] excludeIncludes = null;

        private void checkBoxIncludeOnly_CheckedChanged(object sender, EventArgs e)
        {
            whitelist = checkBoxIncludeOnly.Checked;
        }
        private bool whitelist = false;

        private void checkBoxRegex_CheckedChanged(object sender, EventArgs e)
        {
            useRegex = checkBoxRegex.Checked;
        }
        private bool useRegex = false;

        private void LogSpeakMainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.ExcludeChannel = checkBoxFilterChannels.Checked;
            Settings.Default.ExcludeIsWhitelist = checkBoxIncludeOnly.Checked;
            Settings.Default.ExcludeSelf = checkBoxFilterSelf.Checked;
            Settings.Default.ExcludeUsesRegex = checkBoxRegex.Checked;
            Settings.Default.Exclusions = textBox1.Text;
            Settings.Default.Replacements = textBoxSubstitutions.Text;
            Settings.Default.EnableCombatParsing = checkBoxCombat.Checked;
            Settings.Default.Save();
        }

        private void LogSpeakMainForm_Load(object sender, EventArgs e)
        {
            checkBoxFilterChannels.Checked = Settings.Default.ExcludeChannel;
            checkBoxIncludeOnly.Checked = Settings.Default.ExcludeIsWhitelist;
            checkBoxFilterSelf.Checked = Settings.Default.ExcludeSelf;
            checkBoxRegex.Checked = Settings.Default.ExcludeUsesRegex;
            textBox1.Text = Settings.Default.Exclusions;
            textBoxSubstitutions.Text = Settings.Default.Replacements;
            checkBoxCombat.Checked = Settings.Default.EnableCombatParsing;
        }

        private void checkBoxCombat_CheckedChanged(object sender, EventArgs e)
        {
            combatSupport = checkBoxCombat.Checked;
        }
        private bool combatSupport = false;

        private void button2_Click(object sender, EventArgs e)
        {
            CombatSettingsForm form = new CombatSettingsForm();
            form.ShowDialog();
        }

    }
}
