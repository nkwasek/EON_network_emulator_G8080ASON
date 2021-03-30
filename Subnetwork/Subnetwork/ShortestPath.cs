using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subnetwork
{
   class ShortestPath
    {
        public ShortestPath(int[,] macierz, int pocz, int kon)
        {
            int tmp21 = Convert.ToInt32(Math.Sqrt(macierz.Length));
            this.macierz = new int[tmp21, tmp21];
            for(int i=0;i<tmp21;i++)
            {
                for(int j=0;j<tmp21;j++)
                {
                    this.macierz[i, j] = macierz[i, j];
                }
            }
            
            
            
            
            this.pocz = pocz;this.kon = kon;
        }
        private int[,] macierz;
        private List<string> krosownice = new List<string>();
        private int pocz;
        private int kon;
        private int dlugosc = int.MaxValue;
        public int getDlugosc() { return dlugosc; }
        public List<string> getKrosownice() { return krosownice; }
        public void policz()
        {
            int max = (int)Math.Sqrt(macierz.Length);
            int[] a = new int[max];
            int[] wartosc = new int[max];
            bool[] zawarte = new bool[max];
            for (int i = 0; i < max; i++)
            {
                wartosc[i] = int.MaxValue;
                zawarte[i] = false;
            }
            wartosc[pocz] = 0;//odleglosc od zrodla
            a[0] = pocz;
            for (int i = 0; i < max - 1; i++)
            {
                int pom = najmniejsze(wartosc, zawarte);
                zawarte[pom] = true;
                for (int j = 0; j < max; j++)
                {
                    if (macierz[pom, j] > 0 && zawarte[j] == false && wartosc[pom] + macierz[pom, j] < wartosc[j] && wartosc[pom] != int.MaxValue)
                    {
                        wartosc[j] = wartosc[pom] + macierz[pom, j];
                        a[j] = pom;
                    }
                }
            }
            wynik(wartosc, max);
            
            int m = kon;
            int cos;
            int[,] ab = new int[max, max];
            while (m != pocz)
            {

                ab[a[m], m] = 1;
                ab[m, a[m]] = 1;
                //Console.WriteLine((a[m]+1) + " " + (m+1));
                
                krosownice.Add(a[m] + " " + m);
                if (a[m] != m)
                {
                    macierz[a[m], m] = 0;//zerowanie miejsc ktore odwiedzilismy (zbudowane sciezki)
                    macierz[m, a[m]] = 0;
                }
                cos = a[m];
                m = cos;

            }
            


        }
        private int najmniejsze(int[] wartosc, bool[] zawarte)
        {
            int max = wartosc.Length;
            int min = int.MaxValue;
            int index = -1;
            for (int i = 0; i < max; i++)
            {
                if (zawarte[i] == false && wartosc[i] <= min)
                {
                    min = wartosc[i];
                    index = i;
                }
            }
            return index;
        }

       
        public void wynik(int[] wartosc, int x)
        {
             //Console.WriteLine("==========DIJKSTRA==========");
            // Console.WriteLine("Wezly   Odleglosc od "+ tmp + " do " +tmp2);
            for (int i = 0; i < x; i++)
            {
               // Console.WriteLine(string.Format("{0}       {1} ", i + 1, wartosc[i]));
                if (i == kon)
                    dlugosc = wartosc[i];
                //Console.WriteLine("doleglosc: " + dlugosc);
            }


        }
    }
}
