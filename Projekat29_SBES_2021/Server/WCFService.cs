using Contracts;
using LoggerService;
using Manager;
using SecurityManager;
using SymetricAlgorithmAES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class WCFService :  IWCFService
    {
        private BazaPodataka bp = BazaPodataka.Inicijalizacija();
        public WCFService()
        {
        }

        #region Admin
        public string KreiranjeBaze(string bazaPodataka)
        {
              CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
           // if (principal.Group == "Admins")
        //    {
                if (principal.IsInRole("CreateDB"))
               {
                     Audit.AuthenticationSuccess(principal.Identity.Name);

                    try
                    {
                        string info = bp.KreiranjeBaze(bazaPodataka);
                        Audit.AuthorizationSuccess(principal.Identity.Name, OperationContext.Current.IncomingMessageHeaders.Action);
                        Audit.CreationSuccess(principal.Identity.Name);
                        return info;
                    }
                    catch (ArgumentException ae)
                    {
                         Audit.CreationFailed(principal.Identity.Name);   
                         string info = ae.Message;
                         Console.WriteLine(info);
                         return info;
                    }
                }
               else
               {
                   string kraj = "User dont belong to Admins";
                   Audit.AuthorizationFailed(principal.Identity.Name, OperationContext.Current.IncomingMessageHeaders.Action, "User dont have DeleteDB permission");
                   return kraj;
               }
           
       
        }
        public string BrisanjeBaze(string bazaPodataka)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            Audit.AuthenticationSuccess(principal.Identity.Name);
            if (principal.IsInRole("DeleteDB")) 
            {

                try
                {
                    Audit.AuthorizationSuccess(principal.Identity.Name, OperationContext.Current.IncomingMessageHeaders.Action);                   
                    string info = bp.BrisanjeBaze(bazaPodataka);
                    //Audit.DeleteSuccess(principal.Identity.Name);
                    return info;
                }
                catch (ArgumentException ae)
                {
                   // Audit.DeleteFailed(principal.Identity.Name);
                    string info = ae.Message;
                    Console.WriteLine(info);
                    return info;
                }
            }
            else
            {
               
                string kraj = "User dont belong to Admins";
                Audit.AuthorizationFailed(principal.Identity.Name, OperationContext.Current.IncomingMessageHeaders.Action, "User dont have DeleteDB permission");
                return kraj;
            }
        }
        #endregion 
        #region Writer
        public string UpisUBazu(string poruka, byte[] promena,byte[] signature)
        {
            if (ASCIIEncoding.ASCII.GetString(signature) != "Wrong")
            {
                //ne vraca samo clientWriter,baca null error...

                string clientNameSign = SecurityManager.Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
                if (clientNameSign.Contains("clientWriter"))
                {
                    clientNameSign = "clientWriter_sign";
                }
                X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                   StoreLocation.LocalMachine, clientNameSign);
                if (DigitalSignature.Verify(poruka, Manager.HashAlgorithm.SHA1, signature, certificate))
                {
                    Audit.AuthenticationSuccess("clientWriter");
                    try
                    {
                        Audit.AuthorizationSuccess("clientWriter", OperationContext.Current.IncomingMessageHeaders.Action);
                        string info = bp.UpisUBazu(poruka, promena, signature);
                        Audit.AddSuccess("clientWriter");
                        return info;
                    }
                    catch (ArgumentException ae)
                    {
                        string info = ae.Message;
                        Audit.AddFailed("clientWriter");
                        return info;
                    }

                }
                else
                {
                    string kraj = "User dont belong to Writers";
                    Audit.AuthorizationFailed("clientWriter", OperationContext.Current.IncomingMessageHeaders.Action, "Digital signature failed");
                    return kraj;
                }

           }
           else
           {
                string kraj = "User dont belong to Writers";
                Audit.AuthorizationFailed("clientWriter", OperationContext.Current.IncomingMessageHeaders.Action, "Digital signature failed");
                return kraj;
            }

        }
        public string ModifikacijaBaze(string poruka, byte[] promena, byte[] signature)
        {
            if (ASCIIEncoding.ASCII.GetString(signature) != "Wrong")
            {
                string clientNameSign = SecurityManager.Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
                if (clientNameSign.Contains("clientWriter"))
                {
                    clientNameSign = "clientWriter_sign";
                }

                X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                    StoreLocation.LocalMachine, clientNameSign);
               if (DigitalSignature.Verify(poruka, Manager.HashAlgorithm.SHA1, signature, certificate))
               {
                    Audit.AuthenticationSuccess("clientWriter");
                    try
                    {
                        Audit.AuthorizationSuccess("clientWriter", OperationContext.Current.IncomingMessageHeaders.Action);
                        string info = bp.ModifikacijaBaze(poruka, promena,signature);
                        Audit.AddSuccess("clientWriter");
                        return info;
                    }
                    catch (ArgumentException ae)
                    {
                        string info = ae.Message;
                        Audit.AddFailed("clientWriter");
                        return info;
                    }
               }
               else
               {
                    string kraj = "User dont belong to Writers";
                    Audit.AuthorizationFailed("clientWriter", OperationContext.Current.IncomingMessageHeaders.Action, "Digital signature failed");
                    return kraj;
               }
            }
            else
            {
            string kraj = "User dont belong to Writers";
            Audit.AuthorizationFailed("clientWriter", OperationContext.Current.IncomingMessageHeaders.Action, "Digital signature failed");
              return kraj;
            }
        }
        #endregion
        #region Reader
        public byte[] CitanjeBaze(string bazaPodataka)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            //ne mogu da vrsim proveru,onda baca gresku kod kriptovanja
            if (principal.IsInRole("Read"))
               {
                Audit.AuthenticationSuccess(Manager.Formatter.ParseName(principal.Identity.Name));
                try
                {
                    string eSecretKeyAes = SecretKey.GenerateKey();
                    SecretKey.StoreKey(eSecretKeyAes, "C:\\Users\\Korisnik\\Desktop\\SBES2021\\Sertifikati\\SecretKey");
                    //string info = ASCIIEncoding.ASCII.GetString(bp.CitanjeBaze(bazaPodataka));
                    // Aes aes = Aes.Create();
                    Audit.AuthorizationSuccess(principal.Identity.Name, OperationContext.Current.IncomingMessageHeaders.Action);                              
                    byte[] encrypted = AESinCBC.Encrypt(bp.CitanjeBaze(bazaPodataka), eSecretKeyAes,CipherMode.CBC);                    
                      
                    return encrypted;
                }
                catch (ArgumentException ae)
                {
                    string info = ae.Message;
                    byte[] infoB = ASCIIEncoding.ASCII.GetBytes(info);
                    Audit.AuthorizationFailed(principal.Identity.Name, OperationContext.Current.IncomingMessageHeaders.Action,"Google rekao ne moze");

                    return infoB;
                }
           }
            else
            {
                string kraj = "User dont belong to Readers";
                byte[] infoB = ASCIIEncoding.ASCII.GetBytes(kraj);
                return infoB;
            }
        }
        #endregion
        #region All
        public string SrednjaVrednostZaDrzavuGodinu(string bazaPodataka, string drzava, string godina)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            Audit.AuthenticationSuccess(principal.Identity.Name);
            if (principal.IsInRole2("Work"))
            {
                try
                {
                    Audit.AuthorizationSuccess(principal.Identity.Name, OperationContext.Current.IncomingMessageHeaders.Action);
                    string info = bp.SrednjaVrednostZaDrzavuGodinu(bazaPodataka, drzava, godina);
                    return info;
                }
                catch (ArgumentException ae)
                {
                    string info = ae.Message;
                    return info;
                }
            }
            else
            {
                string kraj = "User dont belong to specified groups";
                Audit.AuthorizationFailed(principal.Identity.Name, OperationContext.Current.IncomingMessageHeaders.Action, "Google rekao ne moze");
                return kraj;
            }

        }
        public string SrednjaVrednostZaGradStarost(string bazaPodataka, string grad, string starostOd, string starostDo)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            Audit.AuthenticationSuccess(principal.Identity.Name);

            if (principal.IsInRole2("Work"))
            {
                try
                {
                    Audit.AuthorizationSuccess(principal.Identity.Name, OperationContext.Current.IncomingMessageHeaders.Action);

                    string info = bp.SrednjaVrednostZaGradStarost(bazaPodataka,grad,starostOd,starostDo);
                    return info;
                }
                catch (ArgumentException ae)
                {
                    string info = ae.Message;
                    return info;
                }
            }
            else
            {
                string kraj = "User dont belong to specified groups";
                Audit.AuthorizationFailed(principal.Identity.Name, OperationContext.Current.IncomingMessageHeaders.Action, "Google rekao ne moze");
                return kraj;
            }
        }
        public string NajvecaPrimanjaPoDrzavi(string bazaPodataka)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            Audit.AuthenticationSuccess(principal.Identity.Name);

            if (principal.IsInRole2("Work"))
                {
                    try
                    {
                        Audit.AuthorizationSuccess(principal.Identity.Name, OperationContext.Current.IncomingMessageHeaders.Action);

                        string info = bp.NajvecaPrimanjaPoDrzavi(bazaPodataka);
                        
                        return info;
                    }
                    catch (ArgumentException ae)
                    {
                        string info = ae.Message;
                        return info;
                    }
                }
                else
                {

                    string kraj = "User dont belong to specified groups";
                    Audit.AuthorizationFailed(principal.Identity.Name, OperationContext.Current.IncomingMessageHeaders.Action, "Google rekao ne moze");
                    return kraj;
                }

            }
        #endregion
    }
}
