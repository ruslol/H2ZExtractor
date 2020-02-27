using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2ZExtractor
{
    class Tools
    {
        public static long CalcChecksum(byte[] buffer, int buffer_size)
        {
            /*
            long v9 = 0;
            long pos = 0;
            int a5 = 1100000;
            byte[] v6 = new byte[a5];
            int a3 = buffer_size;
            long v8, v7, v16, v17, v19, v18;
            int i, j, k;
            if (a3 > a5)
            {
                v8 = a5 & 0xFFFFFFFC;
                v7 = a3 / (a5 & 0xFFFFFFFC);
                v16 = a3 & 3;
                v17 = a3 - (a5 & 0xFFFFFFFC) * v7 - v16;
            }
            else
            {
                v7 = 0;
                v16 = a3 & 3;
                v17 = a3 & 0xFFFFFFFC;
                v8 = 0;
            }
            for (i = 0; v7 > i; ++i)
            {
                Array.Copy(buffer, pos, v6, 0, v8);
                //pos += v8;
                int v11 = 0;
                while (v11 < (v8 >> 2))
                {
                    v9 += v6[4 * v11++];
                }
            }
            Array.Copy(buffer, pos, v6, 0, v17);
            //pos += v17;
            int v13 = 0;
            while (v13 < (v17 >> 2))
            {
                v9 += v6[4 * v13++];
            }
            Console.WriteLine(v16+"!!!");
            return v9;
            */

            return 0;
        }
        public static string[] ReadAllPaths(string dir)
        {
            List<string> paths = new List<string>();
            paths.AddRange(Directory.GetFiles(dir));
            foreach(string _dir in Directory.GetDirectories(dir))
            {
                paths.Add(_dir);
                paths.AddRange(ReadAllPaths(_dir));
            }
            return paths.ToArray();
        }
    }
}
