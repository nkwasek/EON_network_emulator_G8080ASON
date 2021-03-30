using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subnetwork
{
    class RoutingConntroller
    {
        public int lastSlotIndex = 0;//gdzie mozna zajac szczeliny
        public int[,] localTopology;
        public int path;
        private const int eff = 2;
        private const int secure = 5;
        private const double slotMax = 12.5;
        private List<string> krosownice = new List<string>();
        private string gap = "                                        ";

        public List<string> RouteTableQuery(int startAddress, int endAddress)
        {
            /*
            for(int i=0;i<Math.Sqrt(localTopology.Length);i++)
          
            {
                for(int j=0;j<Math.Sqrt(localTopology.Length); j++)
                {
                    Console.Write(localTopology[i, j]+" ");
                }
                Console.Write("\n");
            }    
            */
            ShortestPath sp = new ShortestPath(localTopology, startAddress, endAddress);
            sp.policz();
            path = sp.getDlugosc();
            krosownice = new List<string>();
            krosownice = sp.getKrosownice();
            //NIECH ZWRACA POTEM LACZA LINKI CZY INNE BAJERY


            return krosownice;
        }
        public int reqSlots(int N, int capacity)
        {
            Console.WriteLine(gap + "--------------------------------------------");
            Console.WriteLine(gap + "|   Obliczanie wymaganej liczby szczelin   |");
            Console.WriteLine(gap + "--------------------------------------------");
            if (N==2)
                Console.WriteLine(gap + "| Przewidywana odleglosc: 100-200 km       |");
            else if (N==3)
                Console.WriteLine(gap + "| Przewidywana odleglosc: <100 km          |");
            int mod = (int)Math.Pow(2, N);
            Console.WriteLine(gap + "| Wybrana modulacja: " +mod+"-QAM                 |");
            int feef = ((capacity * eff)/N)+2*secure;
            Console.WriteLine(gap + "| Szerokosc sygnalu: " + feef + "GHz                 |");
            double slot = Math.Ceiling(feef / slotMax);
            Console.WriteLine(gap + "| Wymagana liczba szczelin: " + slot + "              |");
            Console.WriteLine(gap + "--------------------------------------------");
            return (int)slot;
            


        }
        public void LocalTopology(int[,] matrix)
        {
            localTopology = matrix;
            /*
            for (int i = 0; i < Math.Sqrt(matrix.Length); i++)
            {
                for (int j = 0; j < Math.Sqrt(matrix.Length); j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                Console.Write("\n");
            }
            */
        }
        public static void findShortestPath()
        {

        }

        public static void updateLocalTopology()
        {

        }

        public static void NetworkTopology()
        {

        }
    }
}
