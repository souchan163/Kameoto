using System;
using Raylib_cs;

namespace Kameoto
{
    /// <summary>
    /// ログクラス。
    /// </summary>
    public class Logger
    {
        private static int indentLevel = 0;

        private static string CurrentIndent => new string('\t', indentLevel);

        /// <summary>
        /// ログを追加する。
        /// </summary>
        /// <param name="str">内容。</param>
        /// <returns>このクラスのインスタンス。</returns>
        public Logger Add(string str)
        {
            Raylib.TraceLog(TraceLogLevel.Trace, CurrentIndent + str + Environment.NewLine);
            return this;
        }

        /// <summary>
        /// ログを追加する。
        /// </summary>
        /// <param name="strArray">内容。</param>
        /// <returns>このクラスのインスタンス。</returns>
        public Logger Add(string[] strArray)
        {
            foreach (var s in strArray)
            {
                Raylib.TraceLog(TraceLogLevel.Trace, CurrentIndent + s + Environment.NewLine);
            }
            return this;
        }

        /// <summary>
        /// ログのインデントを増やす。
        /// </summary>
        /// <returns>このクラスのインスタンス。</returns>
        public Logger Indent()
        {
            indentLevel++;
            return this;
        }

        /// <summary>
        /// ログのインデントを減らす。
        /// </summary>
        /// <returns>このクラスのインスタンス。</returns>
        public Logger UnIndent()
        {
            indentLevel = Math.Max(0, indentLevel - 1);
            return this;
        }
    }
}
