using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace KwonTomCollector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            Thread t = new Thread(new ThreadStart(Collect));
            t.IsBackground = true;
            t.Start();
        }

        public const int startOfTime = 2453000;

        private void Collect()
        {
            LoadData();
            string[] currentDayNames = new string[7];
            int[] currentDayNumbers = new int[7];
            string lastName = null;
            string lastDayString = null;
            string lastTimeString = null;
            CookieContainer sessionCookies = new CookieContainer();
            Cookie c = new Cookie("kwdat", "InsertYourLoginCookieHere", "/", ".kwontomloop.com");
            sessionCookies.Add(c);
            bool firstRun = true;

            while (true)
            {
                bool changed = false;
                UpdateLastHour(sessionCookies, ref changed);
                UpdateWrongLastHour(sessionCookies, ref changed);
                string webpage = RetrieveWebpage("mainpage", "http://www.kwontomloop.com/index.php?scores=1", sessionCookies); ;
                if (webpage != null)
                {
                    UpdateDayDetails(currentDayNames, currentDayNumbers, webpage);
                    UpdateLatest(currentDayNames, currentDayNumbers, ref lastName, ref lastDayString, ref lastTimeString, ref changed, webpage);
                    if (firstRun)
                    {
                        UpdateHistoric(currentDayNumbers, sessionCookies);
                        changed = true;
                        firstRun = false;
                    }
                }
                if (changed)
                {
                    Save();
                    PerformAnalysis(false);
                    PerformAnalysis(true);
                }
                Thread.Sleep(TimeSpan.FromMinutes(30));
            }
        }

        private void PerformAnalysis(bool wrong)
        {
            int maxDays;
            Dictionary<string, int> nameIndexes;
            int nameCount;
            int[,] scores;
            double[,] ratings;
            double[,] ratingsW;
            double[,] ratingsRW;
            double[,] ratingsD;
            this.BeginInvoke(new MethodInvoker(delegate { labelAction.Text = "Performing analysis."; }));
            LoadForAnalysis(wrong ? "WrongScores.txt" : "Scores.Txt", out maxDays, out nameIndexes, out nameCount, out scores, out ratings);
            ratingsW = (double[,])ratings.Clone();
            ratingsRW = (double[,])ratings.Clone();
            ratingsD = (double[,])ratings.Clone();
            CalculateRatings(maxDays, nameCount, scores, ratings);
            CalculateRatingsW(maxDays, nameCount, scores, ratingsW);
            CalculateRatingsRW(maxDays, nameCount, scores, ratingsRW);
            CalculateRatingsD(maxDays, nameCount, scores, ratingsD);
            this.BeginInvoke(new MethodInvoker(delegate { labelAction.Text = "Saving analysis."; }));
            if (maxDays > 7)
            {
                WriteStableRatings(maxDays, nameCount, ratings, nameIndexes, string.Empty, wrong);
                WriteStableRatings(maxDays, nameCount, ratingsW, nameIndexes, "-Weekly", wrong);
                WriteStableRatings(maxDays, nameCount, ratingsRW, nameIndexes, "-Rolling", wrong);
                WriteStableDailyRatings(maxDays, nameCount, ratingsD, nameIndexes,wrong);
                double[,] dif = new double[1, nameCount];
                double[,] difW = new double[1, nameCount];
                double[,] difRW = new double[1, nameCount];
                double[,] difD = new double[7, nameCount];
                for (int i = 0; i < nameCount; i++)
                {
                    dif[0, i] = ratings[maxDays - 1, i] - ratings[maxDays - 8, i];
                    difW[0, i] = ratingsW[maxDays - 1, i] - ratingsW[maxDays - 8, i];
                    difRW[0, i] = ratingsRW[maxDays - 1, i] - ratingsRW[maxDays - 8, i];
                    for (int j = 0; j < 7; j++)
                    {
                        if (maxDays > 13 - j)
                            difD[j, i] = ratingsD[maxDays - 7 + j, i] - ratingsD[maxDays - 14 + j, i];
                    }
                }
                WriteRatingsDif(nameCount, dif, nameIndexes, string.Empty, wrong);
                WriteRatingsDif(nameCount, difW, nameIndexes, "-Weekly", wrong);
                WriteRatingsDif(nameCount, difRW, nameIndexes, "-Rolling", wrong);
                WriteDailyRatingsDif(nameCount, difD, nameIndexes, maxDays, wrong);
            }
            if (maxDays > 1)
            {
                WriteLatestRatings(maxDays, nameCount, ratings, nameIndexes, string.Empty, wrong);
                WriteLatestRatings(maxDays, nameCount, ratingsW, nameIndexes, "-Weekly", wrong);
                WriteLatestRatings(maxDays, nameCount, ratingsRW, nameIndexes, "-Rolling", wrong);
                WriteLatestDailyRatings(maxDays, nameCount, ratingsD, nameIndexes, wrong);
            }
        }

        private void WriteLatestDailyRatings(int maxDays, int nameCount, double[,] ratings, Dictionary<string, int> nameIndexes, bool wrong)
        {
            for (int i = 0; i < 7; i++)
            {
                int index = maxDays - 7 + i;
                string tag = "-" + daysOfTheWeek[(index + offset) % 7];
                WriteRatings("Latest" + tag, index, nameCount, ratings, nameIndexes, wrong);
            }
        }

        private void WriteDailyRatingsDif(int nameCount, double[,] ratings, Dictionary<string, int> nameIndexes, int maxDays, bool wrong)
        {
            for (int i = 0; i < 7; i++)
            {
                int index = maxDays - 7 + i;
                string tag = "-" + daysOfTheWeek[(index + offset) % 7];
                WriteRatings("Movement" + tag, i, nameCount, ratings, nameIndexes, wrong);
            }
        }

        string[] daysOfTheWeek = new string[] {"Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        private const int offset = 4;

        private void WriteStableDailyRatings(int maxDays, int nameCount, double[,] ratings, Dictionary<string, int> nameIndexes, bool wrong)
        {
            for (int i = 0; i < 7; i++)
            {
                int index = maxDays - 14 + i;
                string tag = "-" + daysOfTheWeek[(index + offset) % 7];
                WriteRatings("Stable" + tag, index, nameCount, ratings, nameIndexes, wrong);
            }
        }

        private void WriteRatingsDif(int nameCount, double[,] dif, Dictionary<string, int> nameIndexes, string tag, bool wrong)
        {
            WriteRatings("Movement" + tag, 0, nameCount, dif, nameIndexes, wrong);
        }

        private void WriteStableRatings(int maxDays, int nameCount, double[,] ratings, Dictionary<string, int> nameIndexes, string tag, bool wrong)
        {
            WriteRatings("Stable"+tag, maxDays - 8, nameCount, ratings, nameIndexes, wrong);
        }

        private void WriteLatestRatings(int maxDays, int nameCount, double[,] ratings, Dictionary<string, int> nameIndexes, string tag, bool wrong)
        {
            WriteRatings("Latest"+tag, maxDays - 1, nameCount, ratings, nameIndexes, wrong);
        }

        private void WriteRatings(string filePrefix, int index, int nameCount, double[,] ratings, Dictionary<string, int> nameIndexes, bool wrong)
        {
            if (wrong)
                filePrefix = "Wrong"+filePrefix;
            string[] names = new string[nameCount];
            double[] matchratings = new double[nameCount];
            foreach (KeyValuePair<string, int> kvp in nameIndexes)
            {
                names[kvp.Value] = kvp.Key;
                matchratings[kvp.Value] = ratings[index, kvp.Value];
            }
            Array.Sort(matchratings, names);
            if (File.Exists(filePrefix + ".html"))
            {
                File.Delete(filePrefix + ".html");
            }
            using (StreamWriter writer = File.CreateText(filePrefix + ".html"))
            {
                using (HtmlTextWriter html = new HtmlTextWriter(writer))
                {
                    html.BeginRender();
                    html.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml-strict.dtd\">");
                    html.AddAttribute("xmlns", "http://www.w3.org/1999/xhtml");
                    html.AddAttribute("xml:lang", "en");
                    html.RenderBeginTag(HtmlTextWriterTag.Html);
                    html.RenderBeginTag(HtmlTextWriterTag.Head);
                    html.RenderBeginTag(HtmlTextWriterTag.Title);
                    html.WriteEncodedText(filePrefix + " Ratings by Tilps");
                    html.RenderEndTag();
                    html.RenderEndTag();
                    html.RenderBeginTag(HtmlTextWriterTag.Body);
                    html.AddStyleAttribute(HtmlTextWriterStyle.BorderColor, "#000000");
                    html.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, "solid");
                    html.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "1px");
                    html.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "#000000");
                    html.RenderBeginTag(HtmlTextWriterTag.Table);
                    html.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "#FFFFFF");
                    html.RenderBeginTag(HtmlTextWriterTag.Tr);
                    html.RenderBeginTag(HtmlTextWriterTag.Td);
                    html.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "bold");
                    html.RenderBeginTag(HtmlTextWriterTag.Span);
                    html.WriteEncodedText("Ranking");
                    html.RenderEndTag();
                    html.RenderEndTag();
                    html.RenderBeginTag(HtmlTextWriterTag.Td);
                    html.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "bold");
                    html.RenderBeginTag(HtmlTextWriterTag.Span);
                    html.WriteEncodedText("Name");
                    html.RenderEndTag();
                    html.RenderEndTag();
                    html.RenderBeginTag(HtmlTextWriterTag.Td);
                    html.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "bold");
                    html.RenderBeginTag(HtmlTextWriterTag.Span);
                    html.WriteEncodedText("Rating");
                    html.RenderEndTag();
                    html.RenderEndTag();
                    html.RenderEndTag();
                    for (int i = nameCount - 1; i >= 0; i--)
                    {
                        html.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "#FFFF" + (i * 256 / nameCount).ToString("X2"));
                        html.RenderBeginTag(HtmlTextWriterTag.Tr);
                        html.RenderBeginTag(HtmlTextWriterTag.Td);
                        html.WriteEncodedText((nameCount-i).ToString()+".");
                        html.RenderEndTag();
                        html.RenderBeginTag(HtmlTextWriterTag.Td);
                        html.WriteEncodedText(names[i]);
                        html.RenderEndTag();
                        html.RenderBeginTag(HtmlTextWriterTag.Td);
                        html.WriteEncodedText(((int)matchratings[i]).ToString());
                        html.RenderEndTag();
                        html.RenderEndTag();
                    }
                    html.RenderEndTag();
                    html.RenderEndTag();
                    html.RenderEndTag();
                }
            }

        }

        private static void CalculateRatings(int maxDays, int nameCount, int[,] scores, double[,] ratings)
        {
            CalculateRatings(maxDays, nameCount, scores, ratings, 1, 1);
        }
        private static void CalculateRatingsW(int maxDays, int nameCount, int[,] scores, double[,] ratings)
        {
            CalculateRatings(maxDays, nameCount, scores, ratings, 7, 7);
        }

        private static void CalculateRatings(int maxDays, int nameCount, int[,] scores, double[,] ratings, int ratingsPeriod, int ratingsPeriodicity)
        {
            for (int i = 7; i < maxDays; i++)
            {
                List<int> competitorsAll = new List<int>();
                List<int> noncomps = new List<int>();
                for (int j = 0; j < nameCount; j++)
                {
                    bool allEmpty = true;
                    for (int k = 0; k < ratingsPeriod; k++)
                        if (scores[i - k, j] != -1)
                        {
                            allEmpty = false;
                        }
                    if (allEmpty)
                    {
                        noncomps.Add(j);
                        continue;
                    }
                    competitorsAll.Add(j);
                }
                foreach (int noncomp in noncomps)
                {
                    ratings[i, noncomp] = ratings[i - ratingsPeriodicity, noncomp];
                }
                if (competitorsAll.Count > 1)
                {
                    int[] players = competitorsAll.ToArray();
                    int[] matchscores = new int[players.Length];
                    for (int j = 0; j < players.Length; j++)
                    {
                        for (int k = 0; k < ratingsPeriod; k++)
                        {
                            if (scores[i - k, players[j]] != -1)
                            {
                                matchscores[j] += scores[i - k, players[j]];
                            }
                            else
                                matchscores[j] += 100000000;
                        }
                    }
                    Array.Sort(matchscores, players);
                    double[] expectedPerformance = new double[players.Length];
                    for (int j = 0; j < players.Length; j++)
                    {
                        double total = 0.0;
                        double rating = ratings[i - ratingsPeriodicity, players[j]];
                        for (int k = 0; k < players.Length; k++)
                        {
                            double oprating = ratings[i - ratingsPeriodicity, players[k]];
                            double logistic = 1.0 / (1.0 + Math.Pow(10, (rating - oprating) / 400.0));
                            total += logistic;
                        }
                        total /= (double)players.Length;
                        expectedPerformance[j] = total;
                    }
                    double[] actualPerformance = new double[players.Length];
                    for (int j = 0; j < players.Length; j++)
                    {
                        int k = j + 1;
                        while (k < players.Length && matchscores[k] == matchscores[j])
                            k++;
                        k--;
                        for (int l = j; l <= k; l++)
                        {
                            actualPerformance[l] = (((double)j + (double)k) / 2.0 + 0.5) / (double)players.Length;
                        }
                        j = k;
                    }
                    for (int j = 0; j < players.Length; j++)
                    {
                        ratings[i, players[j]] = ratings[i - ratingsPeriodicity, players[j]] + 30 * (expectedPerformance[j] - actualPerformance[j]);
                    }
                }
                else if (competitorsAll.Count == 1)
                {
                    ratings[i, competitorsAll[0]] = ratings[i - ratingsPeriodicity, competitorsAll[0]];
                }
            }
        }

        private static void CalculateRatingsRW(int maxDays, int nameCount, int[,] scores, double[,] ratings)
        {
            CalculateRatings(maxDays, nameCount, scores, ratings, 7, 1);
        }

        private static void CalculateRatingsD(int maxDays, int nameCount, int[,] scores, double[,] ratings)
        {
            CalculateRatings(maxDays, nameCount, scores, ratings, 1, 7);
        }

        private static void LoadForAnalysis(string fileName, out int maxDays, out Dictionary<string, int> nameIndexes, out int nameCount, out int[,] scores, out double[,] ratings)
        {
            string[] lines = File.ReadAllLines(fileName);
            maxDays = 0;
            nameIndexes = new Dictionary<string, int>();
            nameCount = 0;
            foreach (string line in lines)
            {
                string[] sections = line.Split(' ');
                if (sections.Length > 1)
                {
                    string name = sections[0];
                    nameIndexes.Add(name, nameCount);
                    nameCount++;
                    int timeStep = 0;
                    for (int i = 1; i < sections.Length; i++)
                    {
                        int val = int.Parse(sections[i]);
                        if (val < 0)
                        {
                            // -1 is an empty spot, -2 is 1 extra ... etc.
                            timeStep += -val - 1;
                        }
                        timeStep++;
                    }
                    if (timeStep > maxDays)
                        maxDays = timeStep;
                }
            }
            scores = new int[maxDays, nameCount];
            for (int i = 0; i < maxDays; i++)
                for (int j = 0; j < nameCount; j++)
                    scores[i, j] = -1;
            ratings = new double[maxDays, nameCount];
            for (int i = 0; i < maxDays; i++)
                for (int j = 0; j < nameCount; j++)
                    ratings[i, j] = 1500;
            // double[,] volatilities = new double[maxDays, nameCount];
            foreach (string line in lines)
            {
                string[] sections = line.Split(' ');
                if (sections.Length > 1)
                {
                    string name = sections[0];
                    int nameIndex = nameIndexes[name];
                    int timeStep = 0;
                    for (int i = 1; i < sections.Length; i++)
                    {
                        int val = int.Parse(sections[i]);
                        if (val >= 0)
                        {
                            scores[timeStep, nameIndex] = val;
                        }
                        else
                        {
                            // -1 is an empty spot, -2 is 1 extra ... etc.
                            timeStep += -val - 1;
                        }
                        timeStep++;
                    }
                }
            }
        }

        private void UpdateLastHour(CookieContainer sessionCookies, ref bool changed)
        {
            string webpage = RetrieveWebpage("Last Hour: " + DateTime.UtcNow.ToString(), "http://www.kwontomloop.com/lasthour.php", sessionCookies);
            if (webpage != null)
            {
                MatchCollection matches = Regex.Matches(webpage, Regex.Escape("<td>") +
                    "([^<]*)" +
                    Regex.Escape("</td><td><!--") +
                    "([^-]*)" +
                    Regex.Escape("-->") +
                    "([^<]*)" +
                    Regex.Escape("</td><td>") +
                    "([^<]*)" +
                    Regex.Escape("</td></tr>"));
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        string name = match.Groups[1].Value.Trim().ToLower();
                        int time = ConvertTimeString(match.Groups[4].Value.Trim());
                        int dayNumber;
                        if (!int.TryParse(match.Groups[2].Value.Trim(), out dayNumber))
                        {
                            this.BeginInvoke(new MethodInvoker(delegate { labelAction.Text = "Last hour score format not-recognized.."; }));

                            dayNumber = -1;
                        }
                        else
                        {
                            dayNumber = dayNumber - startOfTime;
                        }
                        if (dayNumber != -1)
                        {
                            if (AddEntry(name, dayNumber, time))
                                changed = true;
                        }
                    }
                }
                else
                {
                    this.BeginInvoke(new MethodInvoker(delegate { labelAction.Text = "Failed to find any last hour scores."; }));
                }
            }
        }

        private void UpdateWrongLastHour(CookieContainer sessionCookies, ref bool changed)
        {
            string webpage = RetrieveWebpage("Wrong Last Hour: " + DateTime.UtcNow.ToString(), "http://www.kwontomloop.com/lastwronghour.php", sessionCookies);
            if (webpage != null)
            {
                MatchCollection matches = Regex.Matches(webpage, Regex.Escape("<td>") +
                    "([^<]*)" +
                    Regex.Escape("</td><td><!--") +
                    "([^-]*)" +
                    Regex.Escape("-->") +
                    "([^<]*)" +
                    Regex.Escape("</td><td>") +
                    "([^<]*)" +
                    Regex.Escape("</td></tr>"));
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        string name = match.Groups[1].Value.Trim().ToLower();
                        int time = ConvertWrongScoreString(match.Groups[4].Value.Trim());
                        int dayNumber;
                        if (!int.TryParse(match.Groups[2].Value.Trim(), out dayNumber))
                        {
                            this.BeginInvoke(new MethodInvoker(delegate { labelWarning.Text = "Last hour score format not-recognized.."; }));

                            dayNumber = -1;
                        }
                        else
                        {
                            dayNumber = dayNumber - startOfTime;
                        }
                        if (dayNumber != -1)
                        {
                            if (AddWrongEntry(name, dayNumber, time))
                                changed = true;
                        }
                    }
                }
                else
                {
                    this.BeginInvoke(new MethodInvoker(delegate { labelAction.Text = "Failed to find any last hour scores."; }));
                }
            }
        }

        private void LoadData()
        {
            if (File.Exists("Scores.txt"))
            {
                string[] lines = File.ReadAllLines("Scores.txt");
                foreach (string line in lines)
                {
                    string[] sections = line.Split(' ');
                    if (sections.Length > 1)
                    {
                        string name = sections[0].ToLower();
                        int timeStep = 0;
                        for (int i = 1; i < sections.Length; i++)
                        {
                            int val = int.Parse(sections[i]);
                            if (val >= 0)
                            {
                                AddEntry(name, timeStep, val, true);
                            }
                            else
                            {
                                // -1 is an empty spot, -2 is 1 extra ... etc.
                                timeStep += -val - 1;
                            }
                            timeStep++;
                        }
                    }
                }
            }
            if (File.Exists("WrongScores.txt"))
            {
                string[] lines = File.ReadAllLines("WrongScores.txt");
                foreach (string line in lines)
                {
                    string[] sections = line.Split(' ');
                    if (sections.Length > 1)
                    {
                        string name = sections[0].ToLower();
                        int timeStep = 0;
                        for (int i = 1; i < sections.Length; i++)
                        {
                            int val = int.Parse(sections[i]);
                            if (val >= 0)
                            {
                                AddWrongEntry(name, timeStep, val, true);
                            }
                            else
                            {
                                // -1 is an empty spot, -2 is 1 extra ... etc.
                                timeStep += -val - 1;
                            }
                            timeStep++;
                        }
                    }
                }
            }
        }

        private void UpdateHistoric(int[] currentDayNumbers, CookieContainer sessionCookies)
        {
            for (int i = 0; i < currentDayNumbers.Length; i++)
            {
                int pageDayNumber = startOfTime + currentDayNumbers[i];
                string webpage = RetrieveWebpage("Day Page " + pageDayNumber, "http://www.kwontomloop.com/index.php?scday=" + pageDayNumber.ToString()+"&f=1", sessionCookies);
                if (webpage != null)
                {
                    MatchCollection matches = Regex.Matches(webpage, Regex.Escape("<td align=right>") +
                        "([^<]*)" +
                        Regex.Escape("</td><td>") +
                        "([^<]*)" +
                        Regex.Escape("</td><td>") +
                        "([^<]*)" +
                        Regex.Escape("</td></tr>"));
                    if (matches.Count != 0)
                    {
                        foreach (Match match in matches)
                        {
                            string name = match.Groups[2].Value.Trim().ToLower();
                            int time = ConvertTimeString(match.Groups[3].Value.Trim());
                            AddEntry(name, currentDayNumbers[i], time);
                        }
                    }
                    else
                    {
                        this.BeginInvoke(new MethodInvoker(delegate { labelAction.Text = "Failed to find high scores for a day."; }));
                    }
                }
            }
        }

        private void UpdateLatest(string[] currentDayNames, int[] currentDayNumbers, ref string lastName, ref string lastDayString, ref string lastTimeString, ref bool changed, string webpage)
        {

            Match result = Regex.Match(webpage,
                Regex.Escape("Latest daily puzzle solved: ") +
                "(.*)" +
                Regex.Escape(" solved ") +
                "(.*)" +
                Regex.Escape(" puzzle in ") +
                "([^<]*)" +
                Regex.Escape("<br>"));
            if (result.Success)
            {
                string name = result.Groups[1].Value;
                name = name.Trim().ToLower();
                string dayString = result.Groups[2].Value;
                dayString = dayString.Trim();
                if (dayString.EndsWith("'s"))
                    dayString = dayString.Substring(0, dayString.Length - 2);
                dayString = dayString.ToLower();
                string timeString = result.Groups[3].Value;
                timeString = timeString.Trim();
                timeString = timeString.ToLower();
                if (name != lastName || dayString != lastDayString || timeString != lastTimeString)
                {
                    int dayNumber = DayNameToDayNumber(currentDayNames, currentDayNumbers, dayString); ;
                    if (dayNumber == -1)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate { labelAction.Text = "Failed to determine which day is which."; }));
                    }
                    else
                    {
                        if (AddEntry(name, dayNumber, ConvertTimeString(timeString)))
                            changed = true;
                    }
                    lastName = name;
                    lastDayString = dayString;
                    lastTimeString = timeString;
                }
            }
            else
            {
                this.BeginInvoke(new MethodInvoker(delegate { labelAction.Text = "Failed to find latest daily in current page."; }));
            }
        }

        private static int DayNameToDayNumber(string[] currentDayNames, int[] currentDayNumbers, string dayString)
        {
            int dayNumber = -1;
            if (dayString == "today")
                dayNumber = currentDayNumbers[0];
            else if (dayString == "yesterday")
                dayNumber = currentDayNumbers[1];
            else if (dayString == currentDayNames[0]) // specal case - today is used consistantly for today, if the day name is the same as today, it must be a week ago.
                dayNumber = currentDayNumbers[0] - 7;
            else
            {
                for (int i = 0; i < currentDayNames.Length; i++)
                {
                    if (dayString == currentDayNames[i])
                        dayNumber = currentDayNumbers[i];
                }
            }
            return dayNumber;
        }

        private void UpdateDayDetails(string[] currentDayNames, int[] currentDayNumbers, string webpage)
        {
            MatchCollection dayPages = Regex.Matches(webpage, Regex.Escape("\"index.php?scday=") + "([^\"]*)" + Regex.Escape("\">") + "([^<]*)" + Regex.Escape("</a>"));
            if (dayPages.Count == 7)
            {
                int i = 0;
                foreach (Match match in dayPages)
                {
                    currentDayNames[i] = match.Groups[2].Value.Trim().ToLower();
                    currentDayNumbers[i] = int.Parse(match.Groups[1].Value.Trim()) - startOfTime;
                    i++;
                }
            }
            else
            {
                this.BeginInvoke(new MethodInvoker(delegate { labelAction.Text = "Failed to find urls for individual score pages."; }));
            }
        }

        private string RetrieveWebpage(string pageName, string pageUrl, CookieContainer sessionCookies)
        {
            string webpage = null;
            this.BeginInvoke(new MethodInvoker(delegate { labelAction.Text = "Downloading: " + pageName; }));
            CookieContainer firstCookies = sessionCookies;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(pageUrl);
            request.AllowAutoRedirect = false;
            request.CookieContainer = firstCookies;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                while (response.StatusCode == HttpStatusCode.Found)
                {
                    pageUrl = response.Headers["Location"] as string;
                    request = (HttpWebRequest)WebRequest.Create(pageUrl);
                    request.AllowAutoRedirect = false;
                    request.CookieContainer = firstCookies;
                    firstCookies.Add(response.Cookies);
                    foreach (Cookie cookie in response.Cookies)
                        firstCookies.Add(cookie);
                    response = (HttpWebResponse)request.GetResponse();
                }
                firstCookies.Add(response.Cookies);
                try
                {
                    Stream file = response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(file, Encoding.ASCII))
                    {
                        webpage = reader.ReadToEnd();
                    }
                    this.BeginInvoke(new MethodInvoker(delegate { labelAction.Text = pageName + " retrieved."; }));
                }
                finally
                {
                    response.Close();
                }
            }
            catch (Exception e)
            {
                // Net connection may have died - handle it.
                this.BeginInvoke(new MethodInvoker(delegate { labelWarning.Text = e.ToString(); }));
            }
            return webpage;
        }

        private void Save()
        {
            if (File.Exists("Scores.txt"))
            {
                try
                {
                    if (!Directory.Exists("backup"))
                        Directory.CreateDirectory("backup");
                }
                catch
                {
                }
                string dir = string.Empty;
                if (Directory.Exists("backup"))
                    dir = "backup";
                string path = Path.Combine(dir, "Scores" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".txt");
                File.Move("Scores.txt", path);
            }
            using (StreamWriter writer = File.CreateText("Scores.txt"))
            {
                foreach (KeyValuePair<string, List<int>> person in scores)
                {
                    writer.Write(person.Key);
                    int negativeAccumulations = 0;
                    foreach (int score in person.Value)
                    {
                        if (score >= 0)
                        {
                            if (negativeAccumulations > 0)
                            {
                                writer.Write(" ");
                                writer.Write(-negativeAccumulations);
                                negativeAccumulations = 0;
                            }
                            writer.Write(" ");
                            writer.Write(score);
                        }
                        else
                            negativeAccumulations++;
                    }
                    if (negativeAccumulations > 0)
                    {
                        writer.Write(" ");
                        writer.Write(-negativeAccumulations);
                        negativeAccumulations = 0;
                    }
                    writer.WriteLine();
                }
            }
            if (File.Exists("WrongScores.txt"))
            {
                try
                {
                    if (!Directory.Exists("backup"))
                        Directory.CreateDirectory("backup");
                }
                catch
                {
                }
                string dir = string.Empty;
                if (Directory.Exists("backup"))
                    dir = "backup";
                string path = Path.Combine(dir, "WrongScores" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".txt");
                File.Move("WrongScores.txt", path);
            }
            using (StreamWriter writer = File.CreateText("WrongScores.txt"))
            {
                foreach (KeyValuePair<string, List<int>> person in wrongScores)
                {
                    writer.Write(person.Key);
                    int negativeAccumulations = 0;
                    foreach (int score in person.Value)
                    {
                        if (score >= 0)
                        {
                            if (negativeAccumulations > 0)
                            {
                                writer.Write(" ");
                                writer.Write(-negativeAccumulations);
                                negativeAccumulations = 0;
                            }
                            writer.Write(" ");
                            writer.Write(score);
                        }
                        else
                            negativeAccumulations++;
                    }
                    if (negativeAccumulations > 0)
                    {
                        writer.Write(" ");
                        writer.Write(-negativeAccumulations);
                        negativeAccumulations = 0;
                    }
                    writer.WriteLine();
                }
            }
        }
        public Dictionary<string, List<int>> scores = new Dictionary<string, List<int>>();
        public Dictionary<string, List<int>> wrongScores = new Dictionary<string, List<int>>();
        int totalScores = 0;
        int totalNames = 0;
        int totalWrongScores = 0;
        int totalWrongNames = 0;
        private bool AddEntry(string name, int dayNumber, int timeInSeconds)
        {
            return AddEntry(name, dayNumber, timeInSeconds, false);
        }

        private bool AddEntry(string name, int dayNumber, int timeInSeconds, bool useSmallest)
        {
            if (!scores.ContainsKey(name))
            {
                scores.Add(name, new List<int>());
                totalNames++;
            }
            while (scores[name].Count <= dayNumber)
            {
                scores[name].Add(-1);
            }
            if (scores[name][dayNumber] == -1)
            {
                scores[name][dayNumber] = timeInSeconds;
                totalScores++;
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    labelNames.Text = "Names: " + totalNames.ToString();
                    labelTimes.Text = "Times: " + totalScores.ToString();
                }));
                return true;
            }
            else if (useSmallest)
            {
                if (timeInSeconds < scores[name][dayNumber])
                {
                    scores[name][dayNumber] = timeInSeconds;
                    totalScores++;
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        labelNames.Text = "Names: " + totalNames.ToString();
                        labelTimes.Text = "Times: " + totalScores.ToString();
                        labelWarning.Text = "Merged scores with smallest value during load.";
                    }));
                    return true;
                }
            }
            if (timeInSeconds != scores[name][dayNumber])
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    labelWarning.Text = "Not merging score which didn't match already known value.";
                }));
            }
            return false;
        }
        private bool AddWrongEntry(string name, int dayNumber, int timeInSeconds)
        {
            return AddWrongEntry(name, dayNumber, timeInSeconds, false);
        }

        private bool AddWrongEntry(string name, int dayNumber, int timeInSeconds, bool useSmallest)
        {
            if (!wrongScores.ContainsKey(name))
            {
                wrongScores.Add(name, new List<int>());
                totalWrongNames++;
            }
            while (wrongScores[name].Count <= dayNumber)
            {
                wrongScores[name].Add(-1);
            }
            if (wrongScores[name][dayNumber] == -1)
            {
                wrongScores[name][dayNumber] = timeInSeconds;
                totalWrongScores++;
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    labelWrongNames.Text = "Names: " + totalWrongNames.ToString();
                    labelWrongScores.Text = "Scores: " + totalWrongScores.ToString();
                }));
                return true;
            }
            else if (useSmallest)
            {
                if (timeInSeconds < wrongScores[name][dayNumber])
                {
                    wrongScores[name][dayNumber] = timeInSeconds;
                    totalWrongScores++;
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        labelWrongNames.Text = "Names: " + totalWrongNames.ToString();
                        labelWrongScores.Text = "Scores: " + totalWrongScores.ToString();
                        labelWarning.Text = "Merged scores with smallest value during load.";
                    }));
                    return true;
                }
            }
            if (timeInSeconds != wrongScores[name][dayNumber])
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    labelWarning.Text = "Not merging score which didn't match already known value.";
                }));
            }
            return false;
        }

        private int ConvertTimeString(string timeString)
        {
            string[] bits = timeString.Split(' ');
            int total = 0;
            foreach (string bit in bits)
            {
                string trimed = bit.Trim();
                if (trimed.EndsWith("m"))
                {
                    total += 60 * int.Parse(trimed.Substring(0, trimed.Length - 1));
                }
                else if (trimed.EndsWith("s"))
                {
                    total += int.Parse(trimed.Substring(0, trimed.Length - 1));
                }
            }
            return total;
        }

        private int ConvertWrongScoreString(string timeString)
        {
            string[] bits = timeString.Split(' ');
            int total = 0;
            foreach (string bit in bits)
            {
                string trimed = bit.Trim();
                trimed = bit.Trim('(', ')', '+');
                if (!trimed.EndsWith("s"))
                {
                    total += 60 * int.Parse(trimed);
                }
                else if (trimed.EndsWith("s"))
                {
                    total += int.Parse(trimed.Substring(0, trimed.Length - 1));
                }
            }
            return total;
        }
    }
}