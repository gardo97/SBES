using Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Server
{
    public class BazaPodataka : IWCFService
    {
        public Dictionary<String, Dictionary<int, Informations>> BpList = new Dictionary<string, Dictionary<int, Informations>>();
        public String ImeBazePodataka = "bazaPodataka.xml";
        private static BazaPodataka instance = null;

        private BazaPodataka()
        {
            DeserializeData();
        }
        #region Serializacija
        public void SerializeData()
        {
            //podesavamo serializer za informacije (kako bismo sacuvali sve postojece nazive baza)
            XmlSerializer serializerDbNames = new XmlSerializer(typeof(List<String>));
            StringWriter swDbNames = new StringWriter();

            //pomocna promenljiva u koju cemo smestati sve informacije u tekucoj bazi
            List<Informations> tempdataitems;
            //pomocna promenljiva koja cuva nazive svih baza kako bismo ih sacuvali
            List<String> databaseNames = new List<String>();

            //prolazimo kroz sve baze u sistemu
            foreach (KeyValuePair<String, Dictionary<int, Informations>> kvp in BpList)
            {
                //podesavamo serializer za informacije (podatke unutar svake baze)
                XmlSerializer serializer = new XmlSerializer(typeof(List<Informations>));
                StringWriter sw = new StringWriter();

                //dodajemo nazive baza u pomocnu listu da ih sacuvamo u fajlu
                databaseNames.Add(kvp.Key);

                //za svaku bazu, napunimo je njenim podacima i to snimimo u fajl nakon fora
                tempdataitems = new List<Informations>();

                foreach (KeyValuePair<int, Informations> kvp1 in kvp.Value)
                {
                    tempdataitems.Add(kvp1.Value);  //dodajemo u pomocnu listu kako bismo to upisali u fajl
                }
                serializer.Serialize(sw, tempdataitems);
                //upisujemo u svaku bazu informacije koje su vezane za njih
                File.Delete(kvp.Key + ".xml");
                File.AppendAllText(kvp.Key + ".xml", sw.ToString());

            }
            //pravimo fajl sa svim nazivima baza podataka
            serializerDbNames.Serialize(swDbNames, databaseNames);
            File.Delete(ImeBazePodataka);
            File.AppendAllText(ImeBazePodataka, swDbNames.ToString());

        }

        public void DeserializeData()
        {
            if (!File.Exists(ImeBazePodataka))
            {
                return;
            }

            String xml = File.ReadAllText(ImeBazePodataka);   //sve baze podataka


            XmlSerializer xs = new XmlSerializer(typeof(List<String>));
            StringReader sr = new StringReader(xml);
            List<String> templist = (List<String>)xs.Deserialize(sr);

            foreach (var dbName in templist)
            {
                String xmlDb = File.ReadAllText(dbName + ".xml");

                XmlSerializer xsDb = new XmlSerializer(typeof(List<Informations>));
                StringReader srDb = new StringReader(xmlDb);
                List<Informations> templistDb = (List<Informations>)xsDb.Deserialize(srDb);

                Dictionary<int, Informations> currentDb = new Dictionary<int, Informations>();
                foreach (var info in templistDb)
                {
                    currentDb.Add(info.Id, info);
                }
                //kada smo procitali sve iz tekuce baze to dodajemo u DbList
                BpList.Add(dbName, currentDb);
            }

        }
        #endregion
        public static BazaPodataka Inicijalizacija()
        {
            if (instance == null)
            {
                instance = new BazaPodataka();
            }
            return instance;
        }

        #region Admin
        public string KreiranjeBaze(string bazaPodataka)
        {
         
            if (!BpList.ContainsKey(bazaPodataka))
            {
                BpList.Add(bazaPodataka, new Dictionary<int, Informations>());
                SerializeData();
                string inf = $"Baza podataka sa imenom '{bazaPodataka}' je uspesno dodata\n";
                Console.WriteLine(inf);
                return inf;
            }
            else
            {
                string inf = $"Baza podataka sa imenom '{bazaPodataka}' nije dodata\n";
                Console.WriteLine(inf);
                return inf;
            }
        }
        public string BrisanjeBaze(string bazaPodataka)
        {
         
            if (BpList.ContainsKey(bazaPodataka))
            {
                BpList.Remove(bazaPodataka); //brisem iz dictionary,ostaje mi arhiviran file
                string inf = $"Baza podataka sa imenom '{bazaPodataka}' je uspesno izbrisana\n";
                Console.WriteLine(inf);
                return inf;
            }
            else
            {
                string inf = $"Baza podataka sa imenom '{bazaPodataka}' nije izbrisana, ne postoji\n";
                Console.WriteLine(inf);
                return inf;
            }
        }
        #endregion
        #region Writer
        public string UpisUBazu(string poruka, byte[] promena,byte[] signature)
        {
            //parts[0] = imeBaze
            //parts[1] = drzava
            //parts[2] = grad
            //parts[3] = godina
            //parts[4] = starost
            //parts[5] = primanja
            

            string[] parts = poruka.Split(':');

            if (BpList.ContainsKey(parts[0]))
            {
                Informations informacije = new Informations() { Drzava = parts[1].Trim().ToLower(), Grad = parts[2].Trim().ToLower(), Godina = parts[3].Trim().ToLower(), Starost = parts[4].Trim().ToLower() ,MesecnaPrimanja = Double.Parse(parts[5]) };
                BpList[parts[0]].Add(informacije.Id, informacije);
                SerializeData();
                string info = $"Nov entitet je dodat u bazu podataka '{parts[0]}'.\n";
                Console.WriteLine(info);
                return info;
            }
            else
            {
                string kraj = $"Baza podataka sa imenom '{parts[0]}' ne postoji.\n";
                Console.WriteLine(kraj);
                return kraj;
            }
        }
        public string ModifikacijaBaze(string poruka, byte[] promena,byte [] signature)
        {
            //parts[0] = imeBaze
            //parts[1] = drzava
            //parts[2] = grad
            //parts[3] = godina
            //parts[4] = starost
            //parts[5] = primanja
            //parts[6] = id
           
            string[] parts = poruka.Split(':');
            int id = Int32.Parse(parts[6]);

            if (BpList.ContainsKey(parts[0]))
            {
                if (BpList[parts[0]].ContainsKey(id))
                {
                    BpList[parts[0]][id].Drzava = parts[1].Trim().ToLower();
                    BpList[parts[0]][id].Grad = parts[2].Trim().ToLower();
                    BpList[parts[0]][id].Godina = parts[3].Trim().ToLower();
                    BpList[parts[0]][id].Starost = parts[4].Trim().ToLower();
                    BpList[parts[0]][id].MesecnaPrimanja = Double.Parse(parts[5]);
                    SerializeData();

                    string info = $"Postojeci entitet sa id-em '{id}' u bazi podataka '{parts[0]}' je uspesno promenjen\n";
                    Console.WriteLine(info);
                    return info;
                }
                else
                {
                    string kraj = $"Entitet sa id-em '{id}' ne postoji u pazi podataka '{parts[0]}'\n";
                    Console.WriteLine(kraj);
                    return kraj;
                }
            }
            string kraj1 = $"Baza podataka sa imenom '{parts[0]}' nije pronadjena\n";
            Console.WriteLine(kraj1);
            return kraj1;
        }
        #endregion
        #region Reader
        public byte[] CitanjeBaze(string bazaPodataka)
        {
            string poruka = "-------------------------------------";
      
            if (BpList.ContainsKey(bazaPodataka))
            {
                foreach (Informations informacije in BpList[bazaPodataka].Values)
                {
                    poruka += informacije.ToString();
                }

                poruka += "--------------------------------------";
                Console.WriteLine(poruka);
                return ASCIIEncoding.ASCII.GetBytes(poruka);
            }
            else
            {
                string kraj = $"Baza podataka sa imenom '{bazaPodataka}' ne postoji\n";
                Console.WriteLine(kraj);
                return ASCIIEncoding.ASCII.GetBytes(kraj);
            }
        }
        #endregion
        #region All
        public string SrednjaVrednostZaDrzavuGodinu(string bazaPodataka, string drzava, string godina)
        {
            int count = 0;
            double value = 0;

            string poruka = $"-------------------------------------------\nProsecna potrosnja za {drzava}-u tokom {godina}: ";
            
            if (BpList.ContainsKey(bazaPodataka))
            {
                foreach (Informations info in BpList[bazaPodataka].Values)
                {
                    if (info.Drzava == drzava.Trim().ToLower() && info.Godina == godina.Trim().ToLower())
                    {
                        value += info.MesecnaPrimanja;
                        count++;
                    }
                }
                if(count > 0)
                {
                    value /= count;
                    return poruka + value.ToString() + "\n-------------------------------------------\n";
                }
                else
                {
                    return poruka + "0 \n-------------------------------------------\n";
                }
            }
            else
            {
                string kraj = $"Baza podataka sa imenom '{bazaPodataka}' ne postoji\n";
                Console.WriteLine(kraj);
                return kraj;
            }
        }

        public string SrednjaVrednostZaGradStarost(string bazaPodataka, string grad, string starostOd, string starostDo)
        {
            int count = 0;
            double value = 0;

            string poruka = $"-------------------------------------------\nProsecna potrosnja za {grad} za osobe starosti ({starostOd}-{starostDo}): ";

            if (BpList.ContainsKey(bazaPodataka))
            {
                foreach (Informations info in BpList[bazaPodataka].Values)
                {
                    if (info.Grad == grad.Trim().ToLower() && Int32.Parse(info.Starost)<Int32.Parse(starostDo) && Int32.Parse(info.Starost) > Int32.Parse(starostOd) )
                    {
                        value += info.MesecnaPrimanja;
                        count++;
                    }
                }
                if (count > 0)
                {
                    value /= count;
                    return poruka + value.ToString() + "\n-------------------------------------------\n";
                }
                else
                {
                    return poruka + "0 \n-------------------------------------------\n";
                }
            }
            else
            {
                string kraj = $"Baza podataka sa imenom '{bazaPodataka}' ne postoji\n";
                Console.WriteLine(kraj);
                return kraj;
            }
        }
        #endregion
        public string NajvecaPrimanjaPoDrzavi(string bazaPodataka)
        {
            Dictionary<string, List<Informations>> informacije = new Dictionary<string, List<Informations>>();

            string poruka = "-------------------------------------------\nNajveca primanja po drzavama: \n\n";
          

            if (BpList.ContainsKey(bazaPodataka))
            {
                foreach (Informations info in BpList[bazaPodataka].Values)
                {
                    if (informacije.ContainsKey(info.Drzava))
                    {
                        informacije[info.Drzava].Add(info);
                    }
                    else
                    {
                        informacije.Add(info.Drzava, new List<Informations>() { info });
                    }
                }
                foreach (var i in informacije)
                {
                    
                    poruka += $"{i.Key}:\t{i.Value.Max(x => x.MesecnaPrimanja).ToString()}\n";
                }
                return poruka += "\n---------------------------------\n";
            }
            else
            {
                string kraj = $"Baza podataka sa imenom '{bazaPodataka}' ne postoji\n";
                Console.WriteLine(kraj);
                return kraj;
            }

        }
    }
}
