using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMMBYLib
{
    public static class Extract
    {
        public static string To(string[] data)
        {
            try
            {
                using (ArchiveFile archive = new ArchiveFile(data[0]))
                {
                    archive.Extract(data[1]);
                }

                return "Success";
            } catch { return "Error"; }
        }
    }
}
