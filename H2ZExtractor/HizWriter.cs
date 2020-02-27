using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2ZExtractor
{
    class HizWriter
    {
        private Decoder decoder;

        public HizWriter(string key)
        {
            decoder = new Decoder(key);
        }
        public void Write(HizArchive hizArchive, string name)
        {
            BinaryWriter writer = new BinaryWriter(File.OpenWrite(name));
            Console.WriteLine("Write {0}:", name);
            decoder.Clear();
            byte[] hiz_data = WriteHiz(hizArchive);
            writer.Write("H2Z".ToCharArray());
            writer.Write((byte)0);// header
            writer.Write(0);//checksum
            writer.Write(hiz_data.Length);//buffer_size
            writer.Write(hiz_data);
            writer.Close();
        }

        private byte[] WriteHiz(HizArchive hizArchive)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(hizArchive.header);
            writer.Write((byte)0);// header
            writer.Write(hizArchive.file_count);
            writer.Write(hizArchive.max_compress_size);
            
            for (int i = 0; i < hizArchive.file_count; i++)
            {
                HizFile hizFile = hizArchive.hizFiles[i];
                Console.WriteLine("Packing {0}", hizFile.name);
                writer.Write(decoder.DecodeData(BitConverter.GetBytes((short)hizFile.type)));
                writer.Write(decoder.DecodeData(BitConverter.GetBytes(hizFile.compress_size)));
                writer.Write(decoder.DecodeData(BitConverter.GetBytes(hizFile.decompress_size)));
                writer.Write(decoder.DecodeData(BitConverter.GetBytes((short)hizFile.name.Length)));
                writer.Write(decoder.DecodeData(Encoding.UTF8.GetBytes(hizFile.name)));
                writer.Write(hizFile.data);
            }
            Console.WriteLine("\nPacked {0} files.", hizArchive.file_count);
            writer.Close();
            return stream.ToArray();
        }
    }
}
