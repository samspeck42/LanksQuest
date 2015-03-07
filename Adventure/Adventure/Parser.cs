using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public static class Parser
    {
        public static Dictionary<string, string> ParseAttributeData(string attributeData)
        {
            Dictionary<string, string> dataDict = new Dictionary<string, string>();

            int depth = 0;
            char c;
            string str = "";
            char[] dataCharArray = attributeData.ToCharArray();
            for (int i = 0; i < attributeData.Length; i++)
            {
                c = dataCharArray[i];

                if (c == '(')
                {
                    depth++;
                    if (depth == 1)
                        str = "";
                    else
                        str += c;
                }
                else if (c == ')')
                {
                    depth--;
                    if (depth == 0)
                    {
                        string key = str.Substring(0, str.IndexOf(':')).Trim();
                        string value = str.Substring(str.IndexOf(':') + 1).Trim();
                        dataDict.Add(key, value);
                    }
                    else
                        str += c;
                }
                else
                {
                    str += c;
                }
            }

            return dataDict;
        }

        public static Point ParsePoint(string pointString)
        {
            string[] pointArray = pointString.Split(',');
            Point point = new Point(int.Parse(pointArray[0].Trim()), int.Parse(pointArray[1].Trim()));
            return point;
        }
    }
}
