using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace WTTMW
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SpeechSynthesizer synth = new SpeechSynthesizer();
        SpeechRecognitionEngine recog = new SpeechRecognitionEngine();

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default.SavedMin.ToString();
            textBox2.Text = Properties.Settings.Default.SavedMax.ToString();
            textBox3.Text = Properties.Settings.Default.SavedInterval.ToString();
            CheckDayRollover();
            UpdateStatistics();
         }

        private void CheckDayRollover()
        {
            if (Properties.Settings.Default.TimeLastDayReset < DateTime.Today)
            {
                Properties.Settings.Default.AttemptsToday = 0;
                Properties.Settings.Default.CorrectToday = 0;
                Properties.Settings.Default.TimeLastDayReset = DateTime.Now;
                Properties.Settings.Default.Save();
            }
        }

        private void UpdateStatistics()
        {
            labelAttemptsToday.Text = Properties.Settings.Default.AttemptsToday.ToString();
            labelAttemptSetting.Text = Properties.Settings.Default.AttemptsSetting.ToString();
            labelAttemptsEver.Text = Properties.Settings.Default.AttemptsEver.ToString();
            labelCorrectEver.Text = Properties.Settings.Default.CorrectEver.ToString();
            labelCorrectSettings.Text = Properties.Settings.Default.CorrectSettings.ToString();
            labelCorrectToday.Text = Properties.Settings.Default.CorrectToday.ToString();
            labelPercentToday.Text = DeterminePercentDisplay(Properties.Settings.Default.CorrectToday, Properties.Settings.Default.AttemptsToday);
            labelPercentSettings.Text = DeterminePercentDisplay(Properties.Settings.Default.CorrectSettings, Properties.Settings.Default.AttemptsSetting);
            labelPercentEver.Text = DeterminePercentDisplay(Properties.Settings.Default.CorrectEver, Properties.Settings.Default.AttemptsEver);
        }

        private static string DeterminePercentDisplay(int correct, int attempts)
        {
            if (attempts == 0)
                return "N/A";
            return (100 * correct / attempts).ToString();
        }

        void grammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
        }

        void recog_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            textBox1.Text = e.AudioLevel.ToString();
        }

        void recog_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
        }

        void recog_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
        }

        void recog_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
        }
        Random rnd = new Random();
        int min, max, interval;

        private void button1_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out min) || !int.TryParse(textBox2.Text, out max) || !int.TryParse(textBox3.Text, out interval))
            {
                MessageBox.Show("Need numbers in those boxes!");
                return;
            }
            if (Properties.Settings.Default.SavedMin != min || Properties.Settings.Default.SavedMax != max || Properties.Settings.Default.SavedInterval != interval)
            {
                Properties.Settings.Default.SavedMin = min;
                Properties.Settings.Default.SavedMax = max;
                Properties.Settings.Default.SavedInterval = interval;
                Properties.Settings.Default.AttemptsSetting = 0;
                Properties.Settings.Default.CorrectSettings = 0;
                Properties.Settings.Default.Save();
                UpdateStatistics();
            }
            recog.InitialSilenceTimeout = TimeSpan.FromSeconds(20);
            recog.BabbleTimeout = TimeSpan.FromSeconds(20);
            recog.SetInputToDefaultAudioDevice();
            recog.UnloadAllGrammars();


            int range = max - min;
            int count = range / interval + 1;
            GrammarBuilder[] choices = new GrammarBuilder[count];
            int counter = 0;
            for (int i = min; i <= max; i+=interval)
            {
                choices[counter] = new GrammarBuilder(i.ToString());
                counter++;
            }

            GrammarBuilder builder = new GrammarBuilder(new Choices(choices));
            builder.Culture = recog.RecognizerInfo.Culture;
            Grammar grammar = new Grammar(builder);
            recog.LoadGrammar(grammar);
            timer1.Stop();
            timer1.Interval = (rnd.Next(0, count) * interval + min) * ratio;
            timer1.Start();
        }

        int ratio = 60000;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            synth.Speak("How long has it been?");
            RecognitionResult result = recog.Recognize();
            int guess;
            if (result != null && !string.IsNullOrEmpty(result.Text) && int.TryParse(result.Text, out guess))
            {
                Properties.Settings.Default.AttemptsEver = Properties.Settings.Default.AttemptsEver + 1;
                Properties.Settings.Default.AttemptsSetting = Properties.Settings.Default.AttemptsSetting + 1;
                Properties.Settings.Default.AttemptsToday = Properties.Settings.Default.AttemptsToday + 1;
                if (guess == timer1.Interval / ratio)
                {
                    synth.Speak("Correct, it was indeed " + (timer1.Interval / ratio) + " minutes");
                    Properties.Settings.Default.CorrectEver = Properties.Settings.Default.CorrectEver + 1;
                    Properties.Settings.Default.CorrectSettings = Properties.Settings.Default.CorrectSettings + 1;
                    Properties.Settings.Default.CorrectToday = Properties.Settings.Default.CorrectToday + 1;
                }
                else
                {
                    synth.Speak("Nup, it was " + (timer1.Interval / ratio) + " minutes");
                }
                UpdateStatistics();
                Properties.Settings.Default.Save();
            }
            else
            {
                synth.Speak("Nup, it was " + (timer1.Interval / ratio) + " minutes");
            }
            
            int range = max - min;
            int count = range / interval + 1;
            timer1.Interval = (rnd.Next(0, count) * interval + min) * ratio;
            timer1.Start();
        }
    }
}
