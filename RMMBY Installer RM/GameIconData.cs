using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RMMBY_Installer_RM
{
    public class GameIconData
    {
        public aImages _aPreviewMedia { get; set; }

        public static GameIconData GetIconFromSchema(string schema)
        {
            var webRequest = (HttpWebRequest)HttpWebRequest.Create("https://raw.githubusercontent.com/Makarew/RMMBYInstallers/main/gbid.txt");
            var response = webRequest.GetResponse();
            var content = response.GetResponseStream();

            string gameID = "";

            using (var reader = new StreamReader(content))
            {
                string line;
                using (reader)
                {
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            string[] lineData = line.Split(';');

                            if (lineData[1] == schema)
                            {
                                gameID = lineData[2];
                                break;
                            }
                        }
                    }
                    while (line != null);
                }
            }

            return JsonConvert.DeserializeObject<GameIconData>(Uri.UnescapeDataString(Client.Get().GetStringAsync(string.Format("https://gamebanana.com/apiv11/Game/{0}/GetStartedPage", gameID)).Result));
        }
    }

    public class aImages
    {
        public URLType[] _aImages { get; set; }
    }

    public class URLType
    {
        public string _sType { get; set; }
        public string _sUrl { get; set; }
    }
}
