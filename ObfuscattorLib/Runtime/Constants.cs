using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObfuscattorLib.Runtime
{
    public class Constants
    {
        public static string Get(string one, int key, int len)
        {
            StackTrace trace = new StackTrace();
            var data = Encoding.Default.GetBytes(trace.GetFrame(1).GetMethod().Name);
            const int p = 16777619;
            int hash = -2128831035;

            for (int i = 0; i < data.Length; i++)
                hash = (hash ^ data[i]) * p;

            hash += hash << 13;
            hash ^= hash >> 7;
            List<byte> shit = new List<byte>();
            key += hash;
            for (int i = 0; i < len; i++)
            {
                shit.Add(array[key + i]);
            }

            string ceasar = Encoding.UTF8.GetString(shit.ToArray());
            string result = string.Empty;
            foreach (char ch in ceasar.ToCharArray())
                result += (char)(ch - 29023);

            return result;
        }

        public static void Initialize(int len)
        {
            array = new byte[len];

            const int BUFFER_SIZE = 256;
            byte[] tempArray = new byte[BUFFER_SIZE];
            List<byte[]> tempList = new List<byte[]>();
            int count = 0, length = 0;
            MemoryStream ms = new MemoryStream(array);
            DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress);
            while ((count = ds.Read(tempArray, 0, BUFFER_SIZE)) > 0)
            {
                if (count == BUFFER_SIZE)
                {
                    tempList.Add(tempArray);
                    tempArray = new byte[BUFFER_SIZE];
                }
                else
                {
                    byte[] temp = new byte[count];
                    Array.Copy(tempArray, 0, temp, 0, count);
                    tempList.Add(temp);
                }
                length += count;
            }
            byte[] retVal = new byte[length];
            count = 0;
            foreach (byte[] temp in tempList)
            {
                Array.Copy(temp, 0, retVal, count, temp.Length);
                count += temp.Length;
            }
            array = retVal;
        }

        public static void Set()
        {
            array[0] = 0;
        }

        public static byte[] array = new byte[] { };
    }
}
