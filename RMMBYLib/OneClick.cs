using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMMBYLib
{
    public abstract class OneClick
    {
        public event Action UpdateMainText;
        public event Action UpdateLogText;
        public event Action InstallerFinished;

        public string nextMainText;
        public string nextLogText;

        public string downloadedFile;
        public string destination;

        public virtual async Task<OneClickDownload> StartInstall()
        {
            return new OneClickDownload();
        }

        public virtual void StartUp() { }
        public virtual void OnEnd() { }

        protected virtual void UpdateLog(string text)
        {
            nextLogText = text;
            UpdateLogText();
        }

        protected virtual void UpdateTextLine(string text)
        {
            nextMainText = text;
            UpdateMainText();
        }

        protected virtual void UpdateLogAndText(string text)
        {
            UpdateLog(text);
            UpdateTextLine(text);
        }

        protected virtual void Finished()
        {
            InstallerFinished();
        }
    }
}
