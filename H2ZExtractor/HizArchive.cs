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
        //WRITE CalcChecksum()
        //file ID
        //DecodeBuffer 0x10C8DF

        /*
         * ALL CODE REVERSED FROM ANDROID VER RE4
         * 
         * Hiz structure:
         * 
         */

        private char[] DECRYPT_KEY;
        private int DECRYPT_INDEX = 0;
        private HizFile[] hizFiles;
        private BinaryReader reader;


        public void InitFromHiz(string filename, string key)
        {
            DECRYPT_KEY = key.ToCharArray();
            DECRYPT_INDEX = 0;
            reader = new BinaryReader(File.OpenRead(filename));
            string header = new string(reader.ReadChars(4));
            if (string.Compare(header, "H2Z") > 0)
                throw new Exception("Uncorrect H2Z header.");
            int checksum = reader.ReadInt32(); //Ignoring
            int calc_checksum = reader.ReadInt32(); //data?
            string arc_header = new string(reader.ReadChars(4));
            if (string.Compare(arc_header, "HIZ") > 0)
                throw new Exception("Uncorrect HIZ archive header.");
            int file_count = reader.ReadInt32();
            uint max_compress_size = reader.ReadUInt32(); //Ignoring

            Console.WriteLine("HizArchive {0}:\nFile count: {1}\nChecksum: 0x{2}\nMax compress size: {3}\n",
                filename, file_count, checksum.ToString("X"), max_compress_size);

            hizFiles = new HizFile[file_count];
            for (int i = 0; i < file_count; i++)
            {
                HizFile hizFile = new HizFile();
                hizFile.type = ReadDecodeInt16(reader);
                hizFile.compress_size = ReadDecodeInt32(reader);
                hizFile.decompress_size = ReadDecodeInt32(reader);
                int name_len = ReadDecodeInt16(reader);
                hizFile.name = Encoding.UTF8.GetString(Decode_data(reader.ReadBytes(name_len)));
                if (hizFile.isCompressed())
                    hizFile.data = Unzip.GetData(reader.ReadBytes(hizFile.compress_size));
                else
                    hizFile.data = reader.ReadBytes(hizFile.decompress_size);
                hizFiles[i] = hizFile;
            }
        }

        public void InitFromDir(string dirname, string key)
        {
            //UNFINISHED
            DECRYPT_KEY = key.ToCharArray();
            string _dir = dirname;
            List<HizFile> hizFiles = new List<HizFile>();
            while (true)
            {
                HizFile hizFile = new HizFile();
                string[] files = Directory.GetFiles(_dir);
                string[] dirs = Directory.GetDirectories(_dir);
                //hizFile.data = 
                if (dirs.Length == 0)
                    break;
            }
        }

        public void UnpackAll(string dir)
        {
            Directory.CreateDirectory(dir);
            for (int i = 0; i < hizFiles.Length; i++)
            {
                HizFile hizFile = hizFiles[i];
                if (hizFile.data.Length == 0) //dir
                {
                    Directory.CreateDirectory(dir + "/" + hizFiles[i].name);
                }
                else
                {
                    Console.WriteLine("{0} {1}KB", hizFile.name, hizFile.data.Length / 1024);
                    File.WriteAllBytes(dir + "/" + hizFiles[i].name, hizFiles[i].data);
                }
            }
            Console.WriteLine("Done.");
        }

        public void PackAll(string file)
        {
            //write
        }

        public static byte[] CalcChecksum(byte[] data)
        {
            return null;
        }

        private byte[] Decode_data(byte[] data)
        {
            if (DECRYPT_KEY == null)
                throw new Exception("Decrypting error: decrypt key null.");
            for (int i = 0; i < data.Length; i++)
            {
                if (DECRYPT_INDEX >= DECRYPT_KEY.Length)
                    DECRYPT_INDEX = 0;
                data[i] ^= (byte)DECRYPT_KEY[DECRYPT_INDEX];
                DECRYPT_INDEX++;
            }
            return data;
        }

        private int ReadDecodeInt32(BinaryReader reader)
        {
            byte[] data = Decode_data(reader.ReadBytes(4));
            return (data[3] << 24) | (data[2] << 16) | (data[1] << 8) | data[0];
        }

        private int ReadDecodeInt16(BinaryReader reader)
        {
            byte[] data = Decode_data(reader.ReadBytes(2));
            return (data[1] << 8) | data[0];
        }
    }
}
