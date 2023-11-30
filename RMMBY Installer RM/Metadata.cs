using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RMMBY_Installer_RM
{
    public class Metadata
    {
        public string Title { get; set; } = "N/A";
        public string Version { get; set; } = "N/A";
        public string Author { get; set; } = "N/A";
        public string Description { get; set; } = "N/A";
        public string Type { get; set; } = "N/A";
        public int GamebananaID { get; set; } = -1;
        public string Location { get; set; }

        public static T Load<T>(string path) where T : Metadata
        {
            T t;

            try
            {
                t = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                t.Location = Path.GetDirectoryName(path);
            }
            catch (JsonReaderException exception)
            {
                t = default(T);
                t.Title = Path.GetFileName(Path.GetDirectoryName(path));
            }

            return t;
        }

        public static Metadata Load(string path)
        {
            return Metadata.Load<Metadata>(path);
        }
    }
}
