using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RMMBYLib
{
    public class GBSubmissionInfo
    {
        public string _sName { get; set; }
        public Submitter _aSubmitter { get; set; }
        public Game _aGame { get; set; }
        public Category _aCategory { get; set; }


        public class Submitter
        {
            public string _sName { get; set; }
        }
        public class Game
        {
            public string _sName { get; set; }
        }
        public class Category
        {
            public string _sName { get; set; }
        }

        public static GBSubmissionInfo GetMetadata(string modType, string modID)
        {
            return JsonConvert.DeserializeObject<GBSubmissionInfo>(Uri.UnescapeDataString(Client.Get().GetStringAsync(string.Format("https://gamebanana.com/apiv11/{0}/{1}/ProfilePage", modType, modID)).Result));
        }
    }
}
