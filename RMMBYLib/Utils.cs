using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace RMMBYLib
{
    public class Utils
    {
        public static T PullAttribute<T>(Assembly asm, bool inherit = false) where T : Attribute
        {
            Attribute[] attr = Attribute.GetCustomAttributes(asm, inherit);

            if (attr == null || attr.Length <= 0) return null;

            Type requestedType = typeof(T);
            foreach(Attribute att in attr)
            {
                Type attType = att.GetType();

                if (attType == requestedType)
                    return att as T;
            }

            return null;
        }
    }
}
