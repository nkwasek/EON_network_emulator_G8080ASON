using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace Subnetwork
{
    
    public class LRM
    {
        public const int MAX_SLOTS = 30;
        public List<string> srcAddress = new List<string>();//adres zrodla
        public List<string> dstAddress = new List<string>();//adres docelowy
        public List<string> distance = new List<string>();
        public List<bool> canBe = new List<bool>();
        public List<bool> canBe1 = new List<bool>();
        public List<bool> canBe2 = new List<bool>();

        public List<int> idsrc = new List<int>();
        public List<int> iddst = new List<int>();


        public List<string> address = new List<string>();
        public List<int> idAddress = new List<int>();


        
        public List<int[]> slots = new List<int[]>();
        public int matrix_max=0;
        public string LRMID;
        public int[,] matrix;

        public LRM(string id)
        {
            LRMID = id;
        }

        public List<int> ConnectionsId(string address)
        {
            List<int> conn = new List<int>();
            int last = 0;
            for (int i = 0; i < srcAddress.Count; i++)
            {
                if (address == srcAddress[i])
                {
                    for(int j=0;j<MAX_SLOTS;j++)
                    {
                        if(slots[i][j]!=0 && slots[i][j]!=last)
                        {
                            conn.Add(slots[i][j]);
                            last = slots[i][j];
                        }
                    }
                    return conn;
                }
                else
                if (address == dstAddress[i])
                {
                    for (int j = 0; j < MAX_SLOTS; j++)
                    {
                        if (slots[i][j] != 0 && slots[i][j] != last)
                        {
                            conn.Add(slots[i][j]);
                            last = slots[i][j];
                        }
                    }
                    return conn;
                }
            }
            return conn;
        }

        public bool findAddress(string a, string b)
        {
            bool tmpa = false;bool tmpb = false;
            for(int i=0;i<address.Count;i++)
            {
                if (a.Split(':')[0] == address[i].Split(':')[0])
                    tmpa = true;
                if (b.Split(':')[0] == address[i].Split(':')[0])
                    tmpb = true;
            }
            if (tmpa && tmpb)
                return true;
            else return false;
        }

        public void allAddresses()
        {
            address = new List<string>();
            idAddress = new List<int>();
            for(int i=0;i<srcAddress.Count;i++)
            {
                address.Add(srcAddress[i]);idAddress.Add(idsrc[i]);
                address.Add(dstAddress[i]); idAddress.Add(iddst[i]);
            }
        }


        public int[,] LocalTopology()
        {
            Program.ReturnLog("[LRM " + LRMID + "] -> RC " + LRMID + " : LocalTopology()");
            matrix = new int[matrix_max+1, matrix_max+1];
            for(int i=0;i<srcAddress.Count;i++)
            {
                if((matrix[idsrc[i], iddst[i]] > Convert.ToInt32(distance[i]) || matrix[idsrc[i], iddst[i]]==0) &&  canBe1[i]==true && canBe2[i]==true)
                {
                    for(int j=0;j<i;j++)
                    {
                        if(idsrc[i]==idsrc[j] && iddst[i]==iddst[j])
                        {
                            if(Convert.ToInt32(distance[i]) < Convert.ToInt32(distance[j]))
                            {
                                canBe[i] = true; canBe[j] = false;
                            }
                            else if(canBe1[j]==false || canBe2[j]==false)
                            {
                                canBe[i] = true;canBe[j] = false;
                            }
                        }
                    }
                    matrix[idsrc[i], iddst[i]] = Convert.ToInt32(distance[i]);
                    matrix[iddst[i], idsrc[i]] = Convert.ToInt32(distance[i]);
                } 
                else if(matrix[idsrc[i], iddst[i]] < Convert.ToInt32(distance[i]) && matrix[idsrc[i], iddst[i]] == 0)
                {
                    canBe[i] = false;
                }
            }
            /*
            Console.WriteLine("=====TESTY=====");
            for(int i=0;i<srcAddress.Count;i++)
            {
                Console.WriteLine(srcAddress[i] + " " + dstAddress[i] + " " + canBe[i] + " " + canBe1[i] + " " + canBe2[i]);
            }
            */
            /*
            for(int i=0;i< matrix_max+1; i++)
            {
                for (int j = 0; j < matrix_max+1; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                Console.Write("\n");
            }
            */
            allAddresses();
            return matrix;
            
        }
        public void CleanSlots(string a)
        {
            int b = getIndex(a);
            for (int i = 0; i < MAX_SLOTS; i++)
            {
                slots[b][i] = 0;
            }
        }
        public void AllocateSlots(int id, int slot, int index, int number)
        {
            for(int i=index;i<index+slot;i++)
            {
                slots[id][i] = number;
                
                
            }
            Console.Write("                       " + srcAddress[id] + " " + dstAddress[id] + " [");
            for (int k = 0; k < MAX_SLOTS; k++)
            {
                if(k == MAX_SLOTS-1)
                {
                    Console.Write(slots[id][k]);
                }
                else
                {
                    Console.Write(slots[id][k] + "|");
                }
                    
                

            }
            Console.Write("]\n");
        }
        public void AllocateSlots2(int id, int slot, int index, int number)
        {
            for (int i = index; i < index + slot; i++)
            {
                slots[id][i] = number;
            }       
        }
        public void DeallocateSlots(int number, int id)
        {
            bool pom = false;
            for (int i = 0; i < MAX_SLOTS; i++)
            {
                if(slots[id][i] == number)
                {
                    slots[id][i] = 0;
                    pom = true;
                }
            }
            if(pom==true)
            {
                Console.Write("                       " + srcAddress[id] + " " + dstAddress[id] + " [");
                for (int k = 0; k < MAX_SLOTS; k++)
                {
                    if (k == MAX_SLOTS - 1)
                    {
                        Console.Write(slots[id][k]);
                    }
                    else
                    {
                        Console.Write(slots[id][k] + "|");
                    }



                }
                Console.Write("]\n");
            }
            
        }
        public void DeallocateSlots(List<int> conn)
        {
            for(int j=0;j<srcAddress.Count;j++)
            {
                for (int i = 0; i < MAX_SLOTS; i++)
                {
                    for(int k=0;k<conn.Count;k++)
                    {
                       if(slots[j][i] == conn[k])
                            slots[j][i] = 0;                  
                    }                   
                }
            }
        }

        public void LinkConnectionRequest(List<string> Addresses, int slot, int index, int number)
        {
            int[] tmp = new int[2];
            for(int i=0;i<Addresses.Count;i++)
            {
                tmp[0] = Convert.ToInt32(Addresses[i].Split(' ')[0]);
                tmp[1] = Convert.ToInt32(Addresses[i].Split(' ')[1]);
                for(int j=0;j<idsrc.Count;j++)
                {
                    if(idsrc[j]==tmp[0]&&iddst[j]==tmp[1] && canBe[j]==true)
                    {
                        AllocateSlots(j, slot, index, number);
                    }
                    
                    else

                    if (idsrc[j] == tmp[1] && iddst[j] == tmp[0] && canBe[j] == true)
                    {
                        AllocateSlots(j, slot, index, number);
                    }
                }
            }
            Console.WriteLine("---------------------");
        }
        public string Visualisation(int a, int b)
        {
            for(int j=0;j<idsrc.Count;j++)
            {
                if (idsrc[j] == a && iddst[j] == b && canBe[j] == true)
                {
                    return "SNPP: " + srcAddress[j] + "\n                                         SNPP: " + dstAddress[j];
                }

                else

                if (idsrc[j] == b && iddst[j] == a && canBe[j] == true)
                {
                    return "SNPP: " + srcAddress[j] + "\n                                         SNPP: " + dstAddress[j];
                }
            }
            return String.Empty;
                  
            
        }
        public int getIndex(string a)
        {
            for (int j = 0; j < idsrc.Count; j++)
            {
                if (srcAddress[j].Split(':')[0] == a && canBe[j] == true)
                {
                    return idsrc[j];
                }
                else
                if ((dstAddress[j].Split(':')[0] == a && canBe[j] == true))
                {
                    return iddst[j];
                }
            }
            return -1;
        }
        public string getAddress(int a, int b)
        {
            for (int j = 0; j < idsrc.Count; j++)
            {
                if (idsrc[j] == a && iddst[j] == b && canBe[j] == true)
                {
                    return srcAddress[j] + " " + dstAddress[j];
                }

                else

                if (idsrc[j] == b && iddst[j] == a && canBe[j] == true)
                {
                    return srcAddress[j] + " " + dstAddress[j];
                }
            }
            return String.Empty;
        }
        public void LinkConnectionRequest(int slot, int index, int number)//na granicy domeny
        {
                for (int j = 0; j < idsrc.Count; j++)
                {
                    if (idsrc[j] == matrix_max && iddst[j] == matrix_max-1 && canBe[j] == true)
                    {
                        AllocateSlots2(j, slot, index, number);
                    }

                    else

                    if (idsrc[j] == matrix_max - 1 && iddst[j] == matrix_max && canBe[j] == true)
                    {
                        AllocateSlots2(j, slot, index, number);
                    }
                }
            
        }

        public string getLink()
        {
            for (int j = 0; j < idsrc.Count; j++)
            {
                if (idsrc[j] == matrix_max && iddst[j] == matrix_max - 1 && canBe[j] == true)
                {
                    return srcAddress[j] + " " + dstAddress[j];
                }

                else

                if (idsrc[j] == matrix_max - 1 && iddst[j] == matrix_max && canBe[j] == true)
                {
                    return srcAddress[j] + " " + dstAddress[j];
                }
            }
            return String.Empty;
        }




        public void LinkConnectionDeallocation(int id)
        {
            for (int i = 0; i < srcAddress.Count; i++)
            {
                DeallocateSlots(id, i);
            }
            Console.WriteLine("------------------------------------------");
        }
        public void LinkConnectionDeallocation2(int number, int n)
        {
            //WYSWIETLANIE
            //PARAMETRY
            bool pom = false;
            bool[] abc = new bool[srcAddress.Count];
            for(int i=0;i<srcAddress.Count;i++)
            {
                for (int j = 0; j < MAX_SLOTS; j++)
                {
                    if (slots[i][j] == number)
                    {
                        slots[i][j] = 0;
                        pom = true;
                        abc[i] = true;
                    }
                }
                
            }
            if(pom==true)
            {
                Program.ReturnLog("[CCParent] -> CC " + n + " : ConnectionRequest RELEASE");
                Program.ReturnLog("[CC "+n+"] -> LRM "+n+" : LinkConnectionDeallocation REQUEST");
                Console.WriteLine($"[{ DateTime.Now}]" + " [LRM " + n + "] -> CC " + n + " : LinkConnectionDeallocation RESPONSE: ");
                for(int i=0;i<srcAddress.Count;i++)
                {
                    if(abc[i]==true)
                    {
                        {
                            Console.Write("                       " + srcAddress[i] + " " + dstAddress[i] + " [");
                            for (int k = 0; k < MAX_SLOTS; k++)
                            {
                                if (k == MAX_SLOTS - 1)
                                {
                                    Console.Write(slots[i][k]);
                                }
                                else
                                {
                                    Console.Write(slots[i][k] + "|");
                                }
                            }
                            Console.Write("]\n");
                        }
                    }
                }
                Console.WriteLine("------------------------------------------");
                Program.ReturnLog("[LRM " + n + "] -> CC " + n + " : ConnectionRequest RELEASED");

            }
        }
        public bool turnOff(string a)
        {
            for(int i=0;i<srcAddress.Count;i++)
            {
                if(a==srcAddress[i])
                {
                    canBe1[i] = false;
                    return true;
                }
                else
                if(a == dstAddress[i])
                {
                    canBe2[i] = false;
                    return true;
                }
            }
            return false;
        }
        public bool turnOn(string a)
        {
            for (int i = 0; i < srcAddress.Count; i++)
            {
                if (a == srcAddress[i])
                {
                    canBe[i] = true;
                    canBe1[i] = true;
                    return true;
                }
                else
                if (a == dstAddress[i])
                {
                    canBe[i] = true;
                    canBe2[i] = true;
                    return true;
                }
            }
            return false;
        }
        public void LoadConfig(string path)
        {
            XmlReader reader = new XmlTextReader(path);
            string[] data;
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name.ToString())
                    {
                        case "link":
                            data = reader.ReadString().Split(' ');
                            canBe.Add(true);
                            canBe1.Add(true);
                            canBe2.Add(true);
                            srcAddress.Add(data[0]);
                            dstAddress.Add(data[1]);
                            distance.Add(data[2]);
                            slots.Add(new int[MAX_SLOTS]);
                            for (int i = 0; i < srcAddress.Count-1; i++)
                            {
                                if ((idsrc[i] == Convert.ToInt32(data[3]) && iddst[i] == Convert.ToInt32(data[4])) || (idsrc[i] == Convert.ToInt32(data[4]) && iddst[i] == Convert.ToInt32(data[3])))
                                {
                                    if (Convert.ToInt32(data[2]) < Convert.ToInt32(distance[i]))
                                        canBe[i] = false;
                                    else
                                        canBe[srcAddress.Count - 1] = false;
                                }
                                   
                            }
                            if (Convert.ToInt32(data[3]) > matrix_max)
                                matrix_max = Convert.ToInt32(data[3]);
                            idsrc.Add(Convert.ToInt32(data[3]));
                            if (Convert.ToInt32(data[4]) > matrix_max)
                                matrix_max = Convert.ToInt32(data[4]);
                            iddst.Add(Convert.ToInt32(data[4]));
                            
                            break;
                    }
                }
            }
            /*
            for(int i=0;i<srcAddress.Count;i++)
            {
                idsrc.Add(Included2(srcAddress[i].Split(':')[0]));
                iddst.Add(Included2(dstAddress[i].Split(':')[0]));
            }
            */
            Program.ReturnLog("[LRM " + LRMID + "] RECEIVED MESSAGE : Configuration()");
            //Console.WriteLine(idAddress.Count);
            //LocalTopology();
          
        }
    }
}
