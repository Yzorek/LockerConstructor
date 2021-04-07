using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockerConstructor
{
    class LockerReader
    {
        /*
         * Read list of locker from path
         */
        public static List<Locker> Read(string path)
        {
            List<Locker> lockers = new List<Locker>();
            string[] lines = File.ReadAllLines(path);
            int idxInfosSection = 0;
            
            // get the index of the section .infos
            foreach (var line in lines)
            {
                
                if (line == ".infos")
                {
                    break;
                }

                 idxInfosSection++;
            }
           
            // read each locker line by line 
            for (int i = idxInfosSection+1; i < lines.Length; i++)
            {
                if (lines[i] == ".refends")
                    break;
                lockers.Add(ReadLockerFromFileFormat(lines[i]));
            }

            return lockers;
        }

        private static Locker ReadLockerFromFileFormat(string line)
        {
            string[] lockerParts = line.Split(';');
            return new Locker()
            {
                Type = lockerParts[0],
                Entraxe = int.Parse(lockerParts[1]),
                TypeSerr = lockerParts[4],
                IsPend = int.Parse(lockerParts[3]),
                KitPiedPat = int.Parse(lockerParts[2]),
                TypePlaq = int.Parse(lockerParts[5])
            };
        }
    }
}
