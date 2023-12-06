using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMMBYLib
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class RMMBYOneClickAttribute : Attribute
    {
        /// <summary>
        /// System.Type of the one click installer.
        /// </summary>
        public Type SystemType { get; internal set; }

        /// <summary>
        /// Name of the one click installer.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Version of the one click installer.
        /// </summary>
        public string Version { get; internal set; }

        /// <summary>
        /// Author of the one click installer.
        /// </summary>
        public string Author { get; internal set; }

        /// <summary>
        /// RMMBYOneClick constructor.
        /// </summary>
        /// <param name="type">The main type of the one click installer</param>
        /// <param name="name">Name of the one click installer</param>
        /// <param name="version">Version of the one click installer</param>
        /// <param name="author">Author of the one click installer</param>
        public RMMBYOneClickAttribute(Type type, string name, string version, string author)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            SystemType = type;
            Name = name ?? "Unknown";
            Author = author ?? "Unknown";

            Version = version ?? "1.0.0";
        }
    }
}
