using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2ZExtractor
{
    class Unzip
    {
        public static byte[] GetData(byte[] h2z_data, bool decompress)
        {
            MemoryStream input = new MemoryStream(h2z_data);
            MemoryStream output = new MemoryStream();
            DeflateStream deflateStream = new DeflateStream(input, decompress ? CompressionMode.Decompress : CompressionMode.Compress, true);
            deflateStream.CopyTo(output);
            return output.ToArray();
        }
    }
    
}