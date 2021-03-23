using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockerConstructor
{
    public class LockerExporter
    {
        private string _pathToFile;

        private List<string> Lockers { get; set; }

        private List<string> Refends { get; set; }

        private string LeftLocker { get; set; }

        private string RightLocker { get; set; }

        public LockerInfos Infos;

        public LockerExporter(string pathToFile)
        {
            _pathToFile = pathToFile;
            Lockers = new List<string>();
            Refends = new List<string>();
            LeftLocker = null;
            RightLocker = null;
            Infos = new LockerInfos(300, "Serrure monnayeur;129.001;209.001", 0, 0, 0);
        }

        public string FindTypeRefend(string lock1, string lock2)
        {
            return (lock1 + "P/" + lock2 + "G");
        }

        public void AddLocker(string type)
        {
            Lockers.Add(type);

            if (Lockers.Count == 1)
            {
                LeftLocker = type;
                return;
            }

            RightLocker = type;
        }

        public void InsertLocker(int id, string type)
        {
            Lockers.Insert(id, type);

            if (id == 0)
            {
                LeftLocker = type;
            }

            if (id == Lockers.Count - 1)
            {
                RightLocker = type;
            }
        }

        public void EditLocker(int id, string type)
        {
            Lockers[id] = type;

            if (id == 0)
            {
                LeftLocker = type;
            }

            if (id == Lockers.Count - 1)
            {
                RightLocker = type;
            }
        }

        public void DeleteLocker(int id)
        {
            Lockers.RemoveAt(id);

            if (id == 0)
            {
                LeftLocker = Lockers[0];
            }

            if (id == Lockers.Count)
            {
                RightLocker = Lockers[id - 1];
            }
        }

        public bool Export()
        {
            if (Lockers.Count <= 1)
                return (false);
            for (int i = 1; i < Lockers.Count; i++) 
                Refends.Add(FindTypeRefend(Lockers[i - 1], Lockers[i]));

            List<string> lines = new List<string>();

            lines.Add("resultats:");
            lines.Add(LeftLocker);
            lines.Add(RightLocker);
            lines.Add(Infos.Entraxe.ToString());
            lines.Add(Infos.TypeSerr);
            lines.Add(Infos.KitPiedPat.ToString());
            lines.Add(Infos.IsPend.ToString());
            lines.Add(Infos.TypePlaq.ToString());
            lines.Add(".lockers");
            for (int i = 0; i < Lockers.Count; i++)
                lines.Add(Lockers[i]);
            lines.Add(".refends");
            for (int i = 0; i < Refends.Count; i++)
                lines.Add(Refends[i]);
            File.WriteAllLines(_pathToFile, lines);

            return (true);
        }
    }

    public struct LockerInfos
    {
        public LockerInfos(int entraxe, string typeSerr, int kitPiedPat, int isPend, int typePlaq)
        {
            Entraxe = entraxe;
            TypeSerr = typeSerr;
            KitPiedPat = kitPiedPat;
            IsPend = isPend;
            TypePlaq = typePlaq;
        }
        
        public int Entraxe { get; set; }
        public string TypeSerr { get; set; }
        public int KitPiedPat { get; set; }
        public int IsPend { get; set; }
        public int TypePlaq { get; set; }
    }
}
