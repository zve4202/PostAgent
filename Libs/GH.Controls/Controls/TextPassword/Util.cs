// Decompiled with JetBrains decompiler
// Type: GH.Forms.Util
// Assembly: GH.Forms, Version=0.4.0.33109, Culture=neutral, PublicKeyToken=null
// MVID: F2073A0E-CD4C-4D3D-A27E-14F95DDCAF7B
// Assembly location: E:\C#\PostAgent\bin\Debug\GH.Forms.dll

using System;
using System.Diagnostics;

namespace GH.Forms
{
    public static class Util
    {
        [Conditional("DEBUG")]
        public static void DebugStackTrace()
        {
            string str1 = new StackTrace().ToString();
            string[] separator = new string[1]
            {
        Environment.NewLine
            };
            foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                ;
        }

        public static void DebugWithIndentation(Action action)
        {
            Util.DebugWithIndentation<bool>((Func<bool>)(() =>
           {
               action();
               return false;
           }));
        }

        public static T DebugWithIndentation<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            finally
            {
            }
        }
    }
}
