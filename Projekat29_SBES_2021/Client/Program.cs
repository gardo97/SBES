using Manager;
using SymetricAlgorithmAES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string serverCertCN = "wcfService";
            
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            
            string address = "net.tcp://localhost:7776/WCFService";
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, serverCertCN);

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(address), new X509CertificateEndpointIdentity(srvCert));

            using (WCFClient proxy = new WCFClient(binding,endpointAddress))
            {
                string option;

                do
                {
                    Meni();
                    option = Console.ReadLine();

                    if (Int32.TryParse(option, out int opt) && option != "9")
                        IzaberiOpciju(proxy, option);

                } while (option != "9");
            }
        }

        private static void IzaberiOpciju(WCFClient proxy, string opcija)
        {

            string imeBaze = String.Empty;
            string povratnaVrednost = String.Empty;
            string drzava = String.Empty;
            string grad = String.Empty;
            string maksimalnaPotrosnja = String.Empty;
            string starostOd = String.Empty;
            string starostDo = String.Empty;
            string godina = String.Empty;
            string poruka = String.Empty;
            string temp = String.Empty;
            byte[] promena = null;
            byte[] signature = null;
            byte[] encrypted = null;
            

            Console.Write("\nUnesite ime baze podataka: ");
            imeBaze = Console.ReadLine();


            switch (opcija)
            {
                case "1":
                    povratnaVrednost = proxy.KreiranjeBaze(imeBaze);
                    Console.WriteLine(Environment.NewLine + povratnaVrednost);
                    break;
                case "2":
                    povratnaVrednost = proxy.BrisanjeBaze(imeBaze);
                    Console.WriteLine(Environment.NewLine + povratnaVrednost);
                    break;
                case "3":
                    poruka = CreateMessage(imeBaze, "Write");
                    signature = SignedMessage(poruka);
                    povratnaVrednost = proxy.UpisUBazu(poruka, promena,signature);
                    Console.WriteLine(Environment.NewLine + povratnaVrednost);
                    break;
                case "4":
                    poruka = CreateMessage(imeBaze, "Edit");
                    signature = SignedMessage(poruka);
                    povratnaVrednost = proxy.ModifikacijaBaze(poruka, promena,signature);
                    Console.WriteLine(Environment.NewLine + povratnaVrednost);
                    break;
                case "5":
                    encrypted = proxy.CitanjeBaze(imeBaze);
                    if(ASCIIEncoding.ASCII.GetString(encrypted) == "User dont belong to Readers")
                    {
                        Console.WriteLine(Environment.NewLine + ASCIIEncoding.ASCII.GetString(encrypted));
                        break;
                    }
                    else
                    {
                        povratnaVrednost = ASCIIEncoding.ASCII.GetString(AESinCBC.Decrypt(encrypted, SecretKey.LoadKey("C:\\Users\\Korisnik\\Desktop\\SBES2021\\Sertifikati\\SecretKey"), CipherMode.CBC));
                        Console.WriteLine(Environment.NewLine + povratnaVrednost);
                        break;
                    }
                    
                case "6":
                    Console.WriteLine(Environment.NewLine + "Unesite grad: ");
                    grad = Console.ReadLine();
                    Console.WriteLine(Environment.NewLine + "Unesite raspon (Od): ");
                    starostOd = Console.ReadLine();
                    Console.WriteLine(Environment.NewLine + "Unesite raspon (Do): ");
                    starostDo = Console.ReadLine();
                    povratnaVrednost = proxy.SrednjaVrednostZaGradStarost(imeBaze, grad, starostOd,starostDo);
                    Console.WriteLine(Environment.NewLine + povratnaVrednost);
                    break;
                case "7":
                    Console.WriteLine(Environment.NewLine + "Unesite drzavu: ");
                    drzava = Console.ReadLine();
                    Console.WriteLine(Environment.NewLine + "Unesite godinu: ");
                    godina = Console.ReadLine();
                    povratnaVrednost = proxy.SrednjaVrednostZaDrzavuGodinu(imeBaze, drzava, godina);
                    Console.WriteLine(Environment.NewLine + povratnaVrednost);
                    break;
                case "8":
                    povratnaVrednost = proxy.NajvecaPrimanjaPoDrzavi(imeBaze);
                    Console.WriteLine(Environment.NewLine + povratnaVrednost);
                    break;
                case "9":
                    Console.WriteLine(Environment.NewLine + "Ej kafano necu vise drug mi nisi bila");
                    break;
                default:
                    Console.WriteLine(Environment.NewLine + "Nepoznata komanda");
                    break;
            }
        }

        private static string CreateMessage(string imeBazePodataka, string operation)
        {
            string temp;
            double primanja;
            int id = -1;

            if (operation == "Edit")
            {
                do
                {
                    Console.Write("ID: ");
                } while (!Int32.TryParse(Console.ReadLine(), out id));
            }

            Console.Write("Drzava: ");
            string country = Console.ReadLine();

            Console.Write("Grad: ");
            string city = Console.ReadLine();
            
            Console.Write("Godina: ");
            string godina = Console.ReadLine();

            Console.Write("Starost: ");
            string starost = Console.ReadLine();

            do
            {
                Console.Write("Primanja: ");
                temp = Console.ReadLine();
            } while (!Double.TryParse(temp, out primanja));

            if (operation == "Edit")
            {
                string message = $"{imeBazePodataka}:{country}:{city}:{godina}:{starost}:{primanja}:{id}";
                return message;
            }
            else
            {
                string message = $"{imeBazePodataka}:{country}:{city}:{godina}:{starost}:{primanja}";
                return message;
            }
        }
        private static byte[] SignedMessage(string poruka)
        {
            string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";
            if (signCertCN == "clientWriter_sign")
            {
                X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                       StoreLocation.LocalMachine, signCertCN);

                byte[] signature = DigitalSignature.Create(poruka, Manager.HashAlgorithm.SHA1, certificateSign);
                return signature;

            }
            else
            {
                return ASCIIEncoding.ASCII.GetBytes("Wrong");
            }
            
           
        }
        private static void Meni()
        {
            Console.WriteLine("Meni:");
            Console.WriteLine("\t1. Kreiraj bazu podataka");
            Console.WriteLine("\t2. Obrisi bazu podataka");
            Console.WriteLine("\t3. Dodaj nov entitet u bazu podataka");
            Console.WriteLine("\t4. Izmeni postojeci entitet u bazi podataka");
            Console.WriteLine("\t5. Izlistaj sve entitete iz baze podataka");
            Console.WriteLine("\t6. Prikazi srednju vrednost za odredjeni grad i starosnu dob(od/do) iz baze podataka");
            Console.WriteLine("\t7. Prikazi srednju vrednost za odredjenu drzavu tokom odredjene godine iz baze podataka");
            Console.WriteLine("\t8. Prikazi najveca primanja po drzavama");
            Console.WriteLine("\t9. Izlaz");
            Console.Write("\t>> ");
        }
    }

}
