using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMMBYLib
{
    public abstract class OneClick
    {
        public event Action UpdateMainText;
        public event Action UpdateLogText;

        public string nextMainText;
        public string nextLogText;

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
    }
}
