using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMMBY_Installer_RM
{
    public class DisplayMod
    {
        public string title { get; private set; }
        public string author { get; private set; }
        public string version { get; private set; }

        public DisplayMod(string title, string author, string version)
        {
            this.title = title;
            this.author = author;
            this.version = version;
        }

        public static DisplayMod[] GetGameMods(string schema, string category)
        {
            List<DisplayMod> displayMods = new List<DisplayMod>();
            displayMods.Add(new DisplayMod("Hatsune Miku Sora", "Makarew", "1.0.0"));
            displayMods.Add(new DisplayMod("Kiara Fubuki", "MGbrad", "1.0.0"));
            displayMods.Add(new DisplayMod("Yoshino Himekawa", "Makarew", "1.0.0"));
            displayMods.Add(new DisplayMod("Mono Peko", "StickmanVT", "1.0.0"));
            return displayMods.ToArray();
        }
    }
}
