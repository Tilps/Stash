using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Speech.Synthesis;
using LOTROChatNarrator.Properties;
using System.Runtime.InteropServices;
using System.Reflection;

namespace LOTROChatNarrator
{
    public partial class ChatSettingsForm : Form
    {
        public ChatSettingsForm(SpeechSynthesizer synth)
        {
            InitializeComponent();
            this.synth = synth;
            initialName = synth.Voice.Name;
        }

        SpeechSynthesizer synth;
        private static string initialName;

        private void trackBar2_Scroll(object sender, EventArgs e)
        {

        }

        [DllImport("winmm.dll")]
        internal static extern int waveOutGetNumDevs();

        private void ChatSettingsForm_Load(object sender, EventArgs e)
        {
            ConfigureVoice(synth, Settings.Default.VoiceName, Settings.Default.Speed, Settings.Default.Volume, Settings.Default.DefaultOutput, Settings.Default.CustomOutput);
            int rate = synth.Rate;
            listBox1.BeginUpdate();
            listBox1.Items.Clear();
            foreach (InstalledVoice voiceInfo in synth.GetInstalledVoices())
            {
                if (!voiceInfo.Enabled)
                    continue;
                listBox1.Items.Add(voiceInfo.VoiceInfo.Name);
                if (voiceInfo.VoiceInfo.Name == synth.Voice.Name)
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
            listBox1.EndUpdate();
            comboBox1.BeginUpdate();
            comboBox1.Items.Clear();
            int deviceCount = HackDeviceCount();
            for (int i = 0; i < deviceCount; i++)
            {
                comboBox1.Items.Add(HackDeviceName(i));
            }
            comboBox1.EndUpdate();
            checkBox1.Checked = Settings.Default.AccelerateVoice;
            trackBar1.Value = Settings.Default.Volume;
            trackBar2.Value = rate;
            checkBoxDefaultOutput.Checked = Settings.Default.DefaultOutput;
            if (!Settings.Default.DefaultOutput)
            {
                try
                {
                    comboBox1.SelectedIndex = Settings.Default.CustomOutput;
                }
                catch
                {
                }
            }
            else
            {
                if (comboBox1.Items.Count > 0)
                    comboBox1.SelectedIndex = 0;
            }
        }

        private string HackDeviceName(int i)
        {
            try
            {
                Type t1 = typeof(SpeechSynthesizer);
                Type t = t1.Assembly.GetType("System.Speech.Internal.Synthesis.AudioDeviceOut");
                object[] parameters = new object[] { i, null };
                int ret = (int)t.GetMethod("GetDeviceName", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, parameters);
                if (ret == 0)
                    return (string)parameters[1];
            }
            catch
            {
            }
            return "Unnamed device";
        }

        private int HackDeviceCount()
        {
            return waveOutGetNumDevs();
        }

        public static bool ConfigureVoice(SpeechSynthesizer synth, string name, int speed, int volume, bool defaultOutput, int customOutput)
        {
            try
            {
                synth.SetOutputToDefaultAudioDevice();
                if (!defaultOutput)
                {
                    HackCustomDevice(synth, customOutput);
                    // do stuff.
                }
            }
            catch
            {
            }
            if (string.IsNullOrEmpty(name))
                name = initialName;
            try
            {
                if (!string.IsNullOrEmpty(name))
                    synth.SelectVoice(name);
            }
            catch
            {
                return false;
            }
            synth.Rate = speed;
            synth.Volume = volume;
            return true;
        }

        private static void HackCustomDevice(SpeechSynthesizer synth, int customOutput)
        {
            try
            {
                Type t = typeof(SpeechSynthesizer);
                object voiceSynthesis = t.GetField("_voiceSynthesis", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(synth);
                Type t2 = voiceSynthesis.GetType();
                object asyncWorker = t2.GetField("_asyncWorker", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(voiceSynthesis);
                Type t1 = t.Assembly.GetType("System.Speech.Internal.Synthesis.AudioDeviceOut");
                object newWaveOut = t1.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(int), t.Assembly.GetType("System.Speech.Internal.IAsyncDispatch") }, null).Invoke(new object[] { customOutput, asyncWorker });
                t2.GetField("_waveOut", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(voiceSynthesis, newWaveOut);
            }
            catch
            {
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ConfigureVoice(synth, (string)listBox1.SelectedItem, trackBar2.Value, trackBar1.Value, checkBoxDefaultOutput.Checked, comboBox1.SelectedIndex))
            {
                synth.Speak("This is a test message.");
                ConfigureVoice(synth, Settings.Default.VoiceName, Settings.Default.Speed, Settings.Default.Volume, Settings.Default.DefaultOutput, Settings.Default.CustomOutput);
            }
            else
            {
                MessageBox.Show("The currently selected voice is not installed correctly and cannot be selected.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ConfigureVoice(synth, (string)listBox1.SelectedItem, trackBar2.Value, trackBar1.Value, checkBoxDefaultOutput.Checked, comboBox1.SelectedIndex))
            {
                Settings.Default.VoiceName = (string)listBox1.SelectedItem;
                Settings.Default.Speed = trackBar2.Value;
                Settings.Default.Volume = trackBar1.Value;
                Settings.Default.AccelerateVoice = checkBox1.Checked;
                Settings.Default.DefaultOutput = checkBoxDefaultOutput.Checked;
                Settings.Default.CustomOutput = comboBox1.SelectedIndex;
                initialName = Settings.Default.VoiceName;
            }
            else
            {
                MessageBox.Show("The currently selected voice is not installed correctly and cannot be selected.");
                this.DialogResult = System.Windows.Forms.DialogResult.None;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void checkBoxDefaultOutput_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = !checkBoxDefaultOutput.Checked;
        }
    }
}
