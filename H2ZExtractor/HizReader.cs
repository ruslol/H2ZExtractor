using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2ZExtractor
{
    class HizReader
    {
        public HizArchive HizArchive;
        private Decoder decoder;

        private int checksum;
        private int hiz_buffer_size;

        public HizReader(string str_key)
        {
            decoder = new Decoder(str_key);
        }

        public void ReadFromH2Z(string h2z)
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(h2z));
            Console.WriteLine("Reading {0}", Path.GetFileName(h2z));
            decoder.Clear();
            if(!ReadH2ZHeader(reader))
                return;
            byte[] hiz_buffer = reader.ReadBytes(hiz_buffer_size);
            /*
            if(Tools.CalcChecksum(hiz_buffer, hiz_buffer_size) != checksum)
            {
                Console.WriteLine("Warning! Checksum uncorrect.");
            }
            */
            ReadHizArchive(hiz_buffer);
            reader.Close();
        }

        private void ReadHizArchive(byte[] hiz_buffer)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(hiz_buffer));
            HizArchive = new HizArchive();
            if (!ReadHizHeader(reader))
                return;
            Console.WriteLine("File count: {0}\nChecksum: 0x{1}\nMax compress size: {2}\n",
                HizArchive.file_count, checksum.ToString("X"), HizArchive.max_compress_size);

            HizArchive.hizFiles = new HizFile[HizArchive.file_count];
            for (int i = 0; i < HizArchive.file_count; i++)
            {
                HizFile hizFile = new HizFile();
                hizFile.type = decoder.DecodeInt16(reader);
                hizFile.compress_size = decoder.DecodeInt32(reader);
                hizFile.decompress_size = decoder.DecodeInt32(reader);
                int name_len = decoder.DecodeInt16(reader);
                byte[] name = decoder.DecodeData(reader.ReadBytes(name_len));
                hizFile.name = Encoding.UTF8.GetString(name);
                if (hizFile.isCompressed())
                    hizFile.data = Unzip.GetData(reader.ReadBytes(hizFile.compress_size), true);
                else
                    hizFile.data = reader.ReadBytes(hizFile.decompress_size);
                HizArchive.hizFiles[i] = hizFile;
            }
        }

        public void ReadFromDir(string dir)
        {
            HizArchive = new HizArchive();
            HizFile[] hizFiles = ReadHizFilesFromDir(dir);
            if(hizFiles.Length == 0)
            {
                Console.WriteLine("Hiz files count = 0.");
                return;
            }
            HizArchive.header = "HIZ".ToCharArray();
            HizArchive.file_count = hizFiles.Length;
            HizArchive.max_compress_size = 0;
            HizArchive.hizFiles = hizFiles;
        }

        private HizFile[] ReadHizFilesFromDir(string dir)
        {
            string[] paths = Tools.ReadAllPaths(dir);
            HizFile[] hizFiles = new HizFile[paths.Length];
            for(int i = 0; i < paths.Length; i++)
            {
                HizFile hizFile = new HizFile();
                string path = paths[i];
                hizFile.name = path.Replace(dir + Path.DirectorySeparatorChar, string.Empty);
                if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                {
                    hizFile.type = 0;
                    hizFile.compress_size = 0;
                    hizFile.decompress_size = 0;
                    hizFile.data = new byte[0];
                }
                else
                {
                    FileInfo fileInfo = new FileInfo(path);
                    hizFile.type = 0;
                    hizFile.compress_size = (int)fileInfo.Length;
                    hizFile.decompress_size = (int)fileInfo.Length;
                    hizFile.data = File.ReadAllBytes(path);
                }
                hizFiles[i] = hizFile;
            }
            return hizFiles;
        }

        private bool ReadH2ZHeader(BinaryReader reader)
        {
            string header = new string(reader.ReadChars(4));
            if (string.Compare(header, "H2Z") != 0)
            {
                Console.WriteLine("Unknown H2Z header!");
                return false;
            }
            checksum = reader.ReadInt32();
            hiz_buffer_size = reader.ReadInt32();
            return true;
        }

        private bool ReadHizHeader(BinaryReader reader)
        {
            HizArchive.header = reader.ReadChars(4);
            string s_header = new string(HizArchive.header);
            if (string.Compare(s_header, "HIZ") != 0)
            {
                Console.WriteLine("Uncorrect HIZ archive header!");
                return false;
            }
            HizArchive.file_count = reader.ReadInt32();
            HizArchive.max_compress_size = reader.ReadUInt32();
            return true;
        }
    }
}
