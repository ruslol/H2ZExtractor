using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2ZExtractor
{
    class Decoder
    {
        private int key_index;
        private char[] key;

        public Decoder(string str_key)
        {
            key = str_key.ToCharArray();
            Clear();
        }

        public void Clear()
        {
            key_index = 0;
        }

        public short DecodeInt16(BinaryReader reader)
        {
            byte[] data = DecodeData(reader.ReadBytes(2));
            return (short)((data[1] << 8) | data[0]);
        }

        public int DecodeInt32(BinaryReader reader)
        {
            byte[] data = DecodeData(reader.ReadBytes(4));
            return (data[3] << 24) | (data[2] << 16) | (data[1] << 8) | data[0];
        }

        public byte[] DecodeData(byte[] data)
        {
            if (key == null)
                throw new Exception("Decrypting error: decrypt key null.");
            for (int i = 0; i < data.Length; i++)
            {
                if (key_index >= key.Length)
                    key_index = 0;
                data[i] ^= (byte)key[key_index];
                key_index++;
            }
            return data;
        }
    }
}
