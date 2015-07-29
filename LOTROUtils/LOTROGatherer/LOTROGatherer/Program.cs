using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace LOTROGatherer
{
    class Program
    {
        static void Main(string[] args)
        {
            GatherData();
        }

        private static void GatherData()
        {
            HashSet<ServerQualName> players = new HashSet<ServerQualName>();
            HashSet<ServerQualName> kinships = new HashSet<ServerQualName>();
            SourceSkirmish(players, kinships);
            SourcePvMP(players, kinships);
            UpdateKinships(kinships);
            SourceKinships(players);
            UpdatePlayers(players);
        }

        private static void UpdatePlayers(HashSet<ServerQualName> players)
        {
            if (Directory.Exists("Players"))
            {
                string[] kins = Directory.GetFiles("Players");
                foreach (string file in kins)
                {
                    string id = Path.GetFileNameWithoutExtension(file);
                    string[] parts = id.Split('_');
                    players.Add(new ServerQualName { Server = parts[0], Name = parts[1] });
                }
            }
            else
            {
                Directory.CreateDirectory("Players");
            }
            foreach (ServerQualName player in players)
            {
                string results = GetPageBody("http://data.lotro.com/<InsertStuffHere>/charactersheet/w/" + Uri.EscapeDataString(player.Server) + "/c/" + Uri.EscapeDataString(player.Name) + "/");
                File.WriteAllText("Players\\" + player.Server + "_" + player.Name + ".xml", results);
            }
        }

        // cheap and dirty...
        private static Regex charXmlRegex = new Regex(Regex.Escape("<character name=\"") + "([^\"]*)" + Regex.Escape("\""));

        private static void SourceKinships(HashSet<ServerQualName> players)
        {
            if (Directory.Exists("Kinships"))
            {
                string[] kins = Directory.GetFiles("Kinships");
                foreach (string file in kins)
                {
                    string id = Path.GetFileNameWithoutExtension(file);
                    string[] parts = id.Split('_');
                    string fileContent = File.ReadAllText(file);
                    MatchCollection matches = charXmlRegex.Matches(fileContent);
                    foreach (Match match in matches)
                    {
                        players.Add(new ServerQualName { Server = parts[0], Name = match.Groups[1].Value });
                    }
                }
            }
        }

        private static void UpdateKinships(HashSet<ServerQualName> kinships)
        {
            if (Directory.Exists("Kinships"))
            {
                string[] kins = Directory.GetFiles("Kinships");
                foreach (string file in kins)
                {
                    string id = Path.GetFileNameWithoutExtension(file);
                    string[] parts = id.Split('_');
                    kinships.Add(new ServerQualName { Server = parts[0], Name = parts[1] });
                }
            }
            else
            {
                Directory.CreateDirectory("Kinships");
            }
            foreach (ServerQualName kin in kinships)
            {
                string results = GetPageBody("http://data.lotro.com/<InsertStuffHere>/guildroster/w/" + Uri.EscapeDataString(kin.Server) + "/g/" + Uri.EscapeDataString(kin.Name) + "/");
                File.WriteAllText("Kinships\\" + kin.Server + "_" + kin.Name + ".xml", results);
            }
        }

        private static void SourcePvMP(HashSet<ServerQualName> players, HashSet<ServerQualName> kinships)
        {
            /*
            for (int world = 1; world < 20; world++)
            {
                string serverName = servers[world - 1];
                int page = 1;
                bool failed = false;
                do
                {
                    string url = "http://my.lotro.com/home/leaderboard/pvmp?lp[f][wd]=" + world.ToString() + "&lp[pp]=" + page.ToString();
                    string result = GetPageBody(url);
                    failed = ExtractPlayersAndKinsFromHtml(players, kinships, failed, result);
                    page++;
                } while (!failed);
            }
             * */
        }
        private static Regex charRegex = new Regex(Regex.Escape("href=\"/home/character/") + "([^/]*)/([^\"]*)" + Regex.Escape("\""));
        private static Regex kinRegex = new Regex(Regex.Escape("href=\"http://my.lotro.com/kinship-") + "([^-]*)-([^/]*)" + Regex.Escape("/\""));

        private static void SourceSkirmish(HashSet<ServerQualName> players, HashSet<ServerQualName> kinships)
        {
            for (int world = 1; world < 20; world++)
            {
                string serverName = servers[world-1];
                int page = 1;
                bool failed=false;
                do
                {
                    string url = "http://my.lotro.com/home/leaderboard/skirmish?ls[f][wd]="+ world.ToString() + "&ls[pp]=" + page.ToString();
                    string result = GetPageBody(url);
                    failed = ExtractPlayersAndKinsFromHtml(players, kinships, failed, result);
                    page++;
                    // REMOVE THIS
                    if (page > 10)
                        break;
                } while (!failed);
            }

        }

        private static bool ExtractPlayersAndKinsFromHtml(HashSet<ServerQualName> players, HashSet<ServerQualName> kinships, bool failed, string result)
        {
            MatchCollection matches = charRegex.Matches(result);
            foreach (Match match in matches)
            {
                string server = match.Groups[1].Value;
                string name = match.Groups[2].Value;
                name = Uri.UnescapeDataString(name);
                players.Add(new ServerQualName { Server = server, Name = name });
            }
            MatchCollection kinMatches = kinRegex.Matches(result);
            foreach (Match match in kinMatches)
            {
                string server = match.Groups[1].Value;
                string name = match.Groups[2].Value.Replace("_", " ");
                name = Uri.UnescapeDataString(name);
                kinships.Add(new ServerQualName { Server = server, Name = name });
            }
            if (matches.Count == 0)
                failed = true;
            return failed;
        }

        private static int cacheDays = 100;

        private static string[] servers = new string[] { "Gladden","Meneldor","Brandywine","Elendilmir","Landroval","Silverlode","Windfola","Arkenstone", "Firefoot","Nimrodel", "Vilya", "Crickhollow", "Dwarrowdelf", "Imlardris", "", "", "" , "", "Riddermark"};

        private static string GetPageBody(string url)
        {
            string cacheLoc = url.Replace(':', '_').Replace('?', '_').Replace('&', '_').Replace('/', '_').Replace('.', '_').Replace('=', '_');
            if (File.Exists(cacheLoc))
            {
                DateTime writeTime = File.GetLastWriteTimeUtc(cacheLoc);
                if (writeTime.AddDays(cacheDays) > DateTime.UtcNow)
                {
                    return File.ReadAllText(cacheLoc);
                }
            }
            HttpWebRequest.DefaultMaximumErrorResponseLength = -1;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            Stream responseStream;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                {
                    // TODO: consider retrying...
                    return string.Empty;
                }
                responseStream = ex.Response.GetResponseStream();
            }
            StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            string result = reader.ReadToEnd();
            File.WriteAllText(cacheLoc, result);
            // Little sleep everytime we hit a website, don't want to look like we're harvesting ... too much.
            Thread.Sleep(900);
            return result;
        }

    }

    public class ServerQualName : IEquatable<ServerQualName>
    {
        public string Server;
        public string Name;

        #region IEquatable<PlayerId> Members

        public bool Equals(ServerQualName other)
        {
            return string.Equals(Server, other.Server, StringComparison.InvariantCultureIgnoreCase) && string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        public override bool Equals(object obj)
        {
            return Equals((ServerQualName)obj);
        }

        public override int GetHashCode()
        {
            int hashCode = Server == null ? 23:Server.ToLowerInvariant().GetHashCode();
            hashCode *= 33;
            hashCode ^= Name == null ? 64125 : Name.ToLowerInvariant().GetHashCode();
            return hashCode;
        }
    }
}
