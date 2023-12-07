using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RMMBYLib
{
    public static class GameBanana
    {
        private static string testURL = "rmmby://https://gamebanana.com/mmdl/1081324,Mod,477131";

        public static string Download(string baseUrl, string destination)
        {
            baseUrl = testURL;

            string[] paths = GetPaths(baseUrl);
            GBFileMetadata md = GBFileMetadata.GetMetadata(paths[1], paths[2]);
            string downloadUrl = "";

            GBFileMetadata.FileData myData = null;
            foreach (GBFileMetadata.FileData data in md._aFiles)
            {
                if (data._idRow.ToString() == paths[0])
                {
                    myData = data;
                    downloadUrl = myData._sDownloadUrl;
                    break;
                }
            }

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(downloadUrl);
            httpRequest.Method = "GET";

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            Stream httpResponseStream = httpResponse.GetResponseStream();

            int bufferSize = myData._nFilesize;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;

            FileStream fileStream = File.Create(Path.Combine(destination, myData._sFile));
            while ((bytesRead = httpResponseStream.Read(buffer, 0, bufferSize)) != 0)
            {
                fileStream.Write(buffer, 0, bytesRead);
            }

            fileStream.Close();

            return Path.Combine(destination, myData._sFile);
        }

        public static string[] GetPaths(string url)
        {
            url = url.Replace("rmmby://", "");

            if (url.StartsWith("https://")) url = url.Replace("https://gamebanana.com/", "");
            else if (url.StartsWith("http://")) url = url.Replace("http://gamebanana.com/", "");

            string[] paths = url.Split('/');
            url = paths[1];

            return url.Split(',');
        }
    }
}
