using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [Serializable]
    public class Informations
    {
        public static int count = 0;
        public int Id { get; set; }
        public String Grad { get; set; }
        public String Drzava { get; set; }
        public String Starost { get; set; }
        public double MesecnaPrimanja { get; set; }
        public String Godina { get; set; }

        public Informations()
        {
            Id = ++count;
        }

        public override string ToString()
        {
            return $"\nID: {Id}\nGrad: {Grad}\nDrzava: {Drzava}\nStarost: {Starost}\nGodina: {Godina}\nMesecna primanja: {MesecnaPrimanja}" + Environment.NewLine + Environment.NewLine;
        }

    }
}
