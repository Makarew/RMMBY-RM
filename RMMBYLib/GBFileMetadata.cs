using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RMMBYLib
{
    public class GBFileMetadata
    {
        public FileData[] _aFiles { get; set; }

        public static GBFileMetadata GetMetadata(string modType, string modID)
        {
            return JsonConvert.DeserializeObject<GBFileMetadata>(Uri.UnescapeDataString(Client.Get().GetStringAsync(string.Format("https://gamebanana.com/apiv11/{0}/{1}?_csvProperties=_aFiles", modType, modID)).Result));
        }

        public class FileData
        {
            public int _idRow { get; set; }
            public string _sFile { get; set; }
            public int _nFilesize { get; set; }
            public string _sDescription { get; set; }
            public int _tsDateAdded { get; set; }
            public int _nDownloadCount { get; set; }
            public string _sAnalysisState { get; set; }
            public string _sDownloadUrl { get; set; }
            public string _sMd5Checksum { get; set; }
            public string _sClamAvResult { get; set; }
            public string _sAnalysisResult { get; set; }
            public bool _bContainsExe { get; set; }
        }
    }
}
