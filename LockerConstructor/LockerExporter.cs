using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockerConstructor
{
    public class LockerExporter
    {
        private string _pathToFile;


        private List<string> Refends { get; set; }

        private Locker? LeftLocker { get; set; }

        private Locker? RightLocker { get; set; }

        private List<Locker> Lockers;
        public Locker Infos => Lockers[0];

        public LockerExporter(string pathToFile)
        {
            _pathToFile = pathToFile;
            Refends = new List<string>();
            LeftLocker = null;
            RightLocker = null;
            Lockers = new List<Locker>() {
                
            };
        }

        public string FindLockerRefend(Locker lock1, Locker lock2)
        {
            if (lock1.Type == lock2.Type)
                return lock1.Type;
            return (lock1.Type + "P/" + lock2.Type + "G");
        }

        public void AddLocker(Locker locker)
        {
            Lockers.Add(locker);

            if (Lockers.Count == 1)
            {
                LeftLocker = locker;
                return;
            }

            RightLocker = locker;
        }

        public void InsertLocker(int id, Locker locker)
        {
            Lockers.Insert(id, locker);

            if (id == 0)
            {
                LeftLocker = locker;
            }

            if (id == Lockers.Count - 1)
            {
                RightLocker = locker;
            }
        }

        public void EditLocker(int id, Locker locker)
        {
            Lockers[id] = locker;

            if (id == 0)
            {
                LeftLocker = locker;
            }

            if (id == Lockers.Count - 1)
            {
                RightLocker = locker;
            }
        }

        public void DeleteLocker(int id)
        {
            Lockers.RemoveAt(id);

            if (id == 0 && Lockers.Count > 0)
            {
                LeftLocker = Lockers[0];
            }

            if (id == Lockers.Count)
            {
                RightLocker = Lockers[id - 1];
            }
        }

        public List<Locker> CopyLockers()
        {
            return new List<Locker>(Lockers);
        }

        public bool ExportToFile(string path)
        {
            if (Lockers.Count < 1)
                return (false);
            for (int i = 1; i < Lockers.Count; i++)
                Refends.Add(FindLockerRefend(Lockers[i - 1], Lockers[i]));

            List<string> lines = new List<string>();

            lines.Add("resultats:");
            lines.Add(LeftLocker?.Type);
            lines.Add(RightLocker?.Type);

            lines.Add(".infos");
            foreach (var locker in Lockers)
                lines.Add(ToFileFormat(locker));

            lines.Add(".refends");
            for (int i = 0; i < Refends.Count; i++)
                lines.Add(Refends[i]);

            File.WriteAllLines(path, lines);

            return (true);
        }

        private string ToFileFormat(Locker locker)
        {
            return $"{locker.Type};{locker.Entraxe};{locker.KitPiedPat};{locker.IsPend};{locker.TypeSerr};{locker.TypePlaq}";
        }

        public void Swap(int idx1, int idx2)
        {
            Locker tmp = Lockers[idx1];
            Lockers[idx1] = Lockers[idx2];
            Lockers[idx2] = tmp;
        }

        public bool Export()
        {
            bool ret = ExportToFile(_pathToFile);
            Refends.Clear();
            Lockers.Clear();
            return ret;
        }

        public Locker GetLockerAt(int selectedLockerIdx)
        {
            return Lockers[selectedLockerIdx];
        }

        public void Set(int idx, Locker locker)
        {
            Lockers[idx] = locker;
        }
    }

    public struct LockInfos
    {
        public string Name { get; set; }
        public string Code1 { get; set; }
        public string Code2 { get; set; }
    }

    public struct Locker
    {
        public Locker(string type, int entraxe, string typeSerr, int kitPiedPat, int isPend, int typePlaq)
        {
            Type = type;
            Entraxe = entraxe;
            TypeSerr = typeSerr;
            KitPiedPat = kitPiedPat;
            IsPend = isPend;
            TypePlaq = typePlaq;
        }

        public override string ToString()
        {
            string res = "";
            res += "type : " + Type + ",";
            res += "entraxe : " + Entraxe + ",";
            res += "typeSerr : " + TypeSerr + ",";
            res += "kitPiedPat : " + KitPiedPat + ",";
            res += "IsPend : " + IsPend + ",";
            res += "TypePlaq : " + TypePlaq + ",";
            return res;
        }

        /*
         * Valeurs possibles : "H1","H2","H3","H4"
         */
        public string Type { get; set; }
        public int Entraxe { get; set; }
        /*
         * Format : "Nom;Code1;Code2"
         */
        public string TypeSerr { get; set; }
        /*
         * 0 Aucun
         * 1 Pieds
         * 2 Patere
         * 3 Kit pieds + pateres
         */
        public int KitPiedPat { get; set; }
        /*
         * 0 Non
         * 1 Oui avec cintre
         * 2 Oui sans cintre
         * 
         */
        public int IsPend { get; set; }
        /*
         * 0 Non
         * 1 Plaquette gravée 80x30 Gravoply
         * 2 Plaquette 70x40 Ojmar marquage Kalysse
         */
        public int TypePlaq { get; set; }
    }
}
