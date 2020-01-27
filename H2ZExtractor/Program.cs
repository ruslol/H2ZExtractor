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
        private static string version = "1.0";

        static void Main(string[] args)
        {
            Console.WriteLine("H2Z Extractor v{0} by rus_lol_\n", version);
            if (args.Length == 0)
                Help();
            else
                ProcedureArgs(args);
            Console.Read();
        }

        private static void Help()
        {
            Console.WriteLine("Uncorrect args: Drop h2z files to exe.");
        }

        private static void ProcedureArgs(string[] args)
        {
            HizArchive parser = new HizArchive();
            //parser.InitFromDir("common", "bio4");
            foreach(String filename in args)
            {
                parser.InitFromHiz(filename, "bio4");
                string outdir = Path.GetFileNameWithoutExtension(filename);
                parser.UnpackAll(outdir);
            }
        }
    }
}
