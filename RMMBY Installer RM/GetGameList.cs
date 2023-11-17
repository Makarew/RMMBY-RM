using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RMMBY_Installer_RM
{
    public class GetGameList
    {
        public static Game[] GitGameList()
        {
            // Request The Supported Mod Types For Each Game
            var webRequest = (HttpWebRequest)HttpWebRequest.Create("https://raw.githubusercontent.com/Makarew/RMMBYInstallers/main/managers.txt");

            var response = webRequest.GetResponse();
            var content = response.GetResponseStream();

            // Request The Mod Directories For Each Game
            var webRequest2 = (HttpWebRequest)HttpWebRequest.Create("https://raw.githubusercontent.com/Makarew/RMMBYInstallers/main/installers.txt");

            var response2 = webRequest2.GetResponse();
            var content2 = response2.GetResponseStream();

            List<string> installers = new List<string>();

            // Put Mod Directories For Each Game Into A List
            using (var reader = new StreamReader(content2))
            {
                string line;
                int id = 0;
                using (reader)
                {
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            string[] lineData = line.Split(';');
                            installers.Add(line);
                            gameID.Add(lineData[0], id);

                            id++;
                        }
                    }
                    while (line != null);
                }
            }

            // Store Data For Each Game
            List<Game> games = new List<Game>();

            using (var reader = new StreamReader(content))
            {
                string line;
                using (reader)
                {
                    do
                    {
                        //Read a line
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            //Divide line into basic structure
                            string[] lineData = line.Split(';');

                            Game game = new Game();

                            // Get The Game's Name
                            game.gameName = lineData[0];
                            // Get The Game's RMMBY Schema
                            game.gameSchema = lineData[1];

                            // Get All Supported Mod Types For The Game
                            List<string> types = new List<string>();

                            for (int i = 2; i < lineData.Length; i++)
                            {
                                types.Add(lineData[i]);
                            }

                            game.modTypes = types;

                            string baseLocation = GameInstallLocation(game.gameSchema);

                            int id = gameID.GetValueOrDefault(game.gameName);

                            lineData = installers[id].Split(';');
                            List<string> installLocations = new List<string>();
                            bool gotDir = false;

                            for (int i = 0; i < types.Count; i++)
                            {
                                gotDir = false;
                                for (int j = 3; j < lineData.Length; j++)
                                {
                                    if (lineData[j].Contains(types[i]))
                                    {
                                        installLocations.Add(Path.Combine(baseLocation, lineData[j + 1]));
                                        gotDir = true;
                                        break;
                                    }
                                    j++;
                                }

                                if (!gotDir)
                                {
                                    installLocations.Add(Path.Combine(baseLocation, lineData[2]));
                                }
                            }

                            game.typeDirectories = installLocations;

                            // Add The Game To The List Of Games
                            games.Add(game);
                        }
                    }
                    while (line != null);
                }
            }

            webRequest.Abort();
            webRequest2.Abort();

            return games.ToArray();
        }

        public static string GameInstallLocation(string schema)
        {
            string result = "LOCATIONNOTFOUND";

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(string.Format("SOFTWARE\\Classes\\{0}\\shell\\open\\command", schema)))
                {
                    if (key != null)
                    {
                        Object o = key.GetValue("");

                        if (o != null)
                        {
                            result = o as string;
                            result = result.Replace("\"", "");
                        }
                    }
                }
            }
            catch
            {

            }

            return result;
        }

        public struct Game
        {
            public string gameName;
            public string gameSchema;
            public List<string> modTypes;
            public List<string> typeDirectories;
        }

        private static Dictionary<string, int> gameID = new Dictionary<string, int>();
    }
}
