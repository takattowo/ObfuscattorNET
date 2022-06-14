using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObfuscattorLib
{
    public class Logger
    {
        public static void LogMessage(string pre, string past, ConsoleColor PastColor)
        {
            Worker.Log += "\n" + pre + "\n" + past;
        }
    }
}
