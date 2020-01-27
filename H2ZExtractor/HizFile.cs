using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2ZExtractor
{
    class HizFile
    {
        public int type;
        public int compress_size;
        public int decompress_size;
        public string name;
        public byte[] data;

        public bool isCompressed() //only for unpack (deflate)
        {
            return type == 8;
        }
    }
}
