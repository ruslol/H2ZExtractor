using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2ZExtractor
{
    class Program
    {
        private static string version = "1.1";
        private static string key = "bio4";
        private static HizReader reader;
        private static HizWriter writer;

        static void Main(string[] args)
        {
            //byte[] d = File.ReadAllBytes("file");
           // Console.WriteLine(Tools.CalcChecksum(d, d.Length));
            //Console.Read();
            
            Console.WriteLine("H2Z Extractor v{0} by rus_lol_\n", version);
            if (args.Length == 0)
                Help();
            else
                ProcedureArgs(args);
            Console.Read();
            
        }

        private static void Help()
        {
            Console.WriteLine("Uncorrect args: Drop h2z files to exe for unpack or folder for pack,\nor use \"H2ZExtractor.exe [files]\"");
        }

        private static void ProcedureArgs(string[] args)
        {
            reader = new HizReader(key);
            writer = new HizWriter(key);
            foreach (String path in args)
            {
                if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                    ProcedureDir(path);
                else
                    ProcedureFile(path);
            }
        }
        private static void ProcedureDir(string dir)
        {
            reader.ReadFromDir(dir);
            if (reader.HizArchive.hizFiles != null)
            {
                string outfile = dir + ".h2z";
                writer.Write(reader.HizArchive, outfile);
            }
        }

        private static void ProcedureFile(string filename)
        {
            reader.ReadFromH2Z(filename);
            if (reader.HizArchive.hizFiles != null)
            {
                string outdir = Path.GetFileNameWithoutExtension(filename);
                UnpackAll(reader.HizArchive.hizFiles, outdir);
            }
        }

        public static void UnpackAll(HizFile[] hiz_files, string dir)
        {
            Directory.CreateDirectory(dir);
            for (int i = 0; i < hiz_files.Length; i++)
            {
                HizFile hizFile = hiz_files[i];
                if (hizFile.data.Length == 0) //dir
                {
                    Directory.CreateDirectory(dir + "/" + hizFile.name);
                }
                else
                {
                    Console.WriteLine("{0} {1}KB", hizFile.name, hizFile.data.Length / 1024);
                    File.WriteAllBytes(dir + "/" + hizFile.name, hizFile.data);
                }
            }
            Console.WriteLine("Done.");
        }
    }
}
