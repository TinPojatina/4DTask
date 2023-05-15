using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace AssetManager
{
    public class Asset
    {
        public string šifra { get; }
        public string naziv { get; }
        public string mjesto_troška { get; }
        public DateTime datum_aktivacije { get; }
        public string smještaj { get; }

        public Asset(string šifra, string naziv, string mjesto_troška, DateTime datum_aktivacije, string smještaj)
        {
            this.šifra = šifra;
            this.naziv = naziv;
            this.mjesto_troška = mjesto_troška;
            this.datum_aktivacije = datum_aktivacije;
            this.smještaj = smještaj;
        }
    }

    class Program
    {
        static void Main()
        {
            string inDirectory = @"C:\Users\tinp9\Desktop\4DevProject\Rjesenje\IN\";
            string outDirectory = @"C:\Users\tinp9\Desktop\4DevProject\Rjesenje\OUT\";
            string invalidAssetsFile = Path.Combine(outDirectory, "Neispravna_Imovina.csv");

            var assets = new Dictionary<string, Asset>();

            foreach (string file in Directory.GetFiles(inDirectory))
            {
                string fileExtension = Path.GetExtension(file);
                if (fileExtension == ".csv")
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] parts = line.Split(';');
                            ProcessLine(parts, invalidAssetsFile, assets);
                        }
                    }
                }
                else if (fileExtension == ".xml")
                {
                    XDocument doc = XDocument.Load(file);
                    foreach (XElement element in doc.Descendants("Imovina"))
                    {
                        string[] parts = new string[5];
                        parts[0] = (string)element.Element("šifra");
                        parts[1] = (string)element.Element("naziv");
                        parts[2] = (string)element.Element("mjesto_troška");
                        parts[3] = (string)element.Element("datum_aktivacije");
                        parts[4] = (string)element.Element("smještaj");
                        ProcessLine(parts, invalidAssetsFile, assets);
                    }
                }
            }

            string outFile = Path.Combine(outDirectory, "Imovina.csv");
            using (StreamWriter sw = new StreamWriter(outFile))
            {
                sw.WriteLine("šifra;naziv;mjesto_troška;datum_aktivacije;smještaj");

                foreach (Asset asset in assets.Values.OrderBy(a => a.naziv))
                {
                    sw.WriteLine($"{asset.šifra};{asset.naziv};{asset.mjesto_troška};{asset.datum_aktivacije:dd.MM.yyyy};{asset.smještaj}");
                }
            }
        }

        static void ProcessLine(string[] parts, StreamWriter invalidAssetsWriter, Dictionary<string, Asset> assets)
        {
            if (parts.Length != 5 || !DateTime.TryParseExact(parts[3], "dd.MM.yyyy", null, DateTimeStyles.None, out DateTime date) || assets.ContainsKey(parts[0]))
            {
                invalidAssetsWriter.WriteLine(string.Join(';', parts));
                return;
            }

            assets[parts[0]] = new Asset(parts[0], parts[1], parts[2], date, parts[4]);
        }
    }
}
