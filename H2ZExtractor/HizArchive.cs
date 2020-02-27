using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2ZExtractor
{
    class HizArchive
    {
        public char[] header;
        public int file_count;
        public uint max_compress_size;
        public HizFile[] hizFiles;
    }
}
