using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Manager;

namespace Client
{
    public class WCFClient : ChannelFactory<IWCFService>, IWCFService, IDisposable
    {

        IWCFService factory;

        public WCFClient(NetTcpBinding binding, EndpointAddress address)
           : base(binding, address)
        {

            /// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
			string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            //string cltCertCN = "clientAdmin";
            if (cltCertCN.Contains("clientWriter"))
            {
                cltCertCN += "_sign";
            }

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
            this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            /// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

            factory = this.CreateChannel();
        }

        #region Admin
        public string KreiranjeBaze(string bazaPodataka)
        {	
            
            try
            {
                return factory.KreiranjeBaze(bazaPodataka);
            }
           catch (SecurityAccessDeniedException e)
            {
               string info = $"Greska prilikom kreiranja baze podataka: {e.Message}";
                Console.WriteLine(info);
                return info;
            }
         
        }
        public string BrisanjeBaze(string bazaPodataka)
        {
            try
            {
                return factory.BrisanjeBaze(bazaPodataka);
            }
            catch (SecurityAccessDeniedException e)
            {
                string info = $"Greska prilikom brisanja baze podataka: {e.Message}";
                Console.WriteLine(info);
                return info;
            }
          
        }
      /* public string ArhiviranjeBaze(string bazaPodataka)
        {
            throw new NotImplementedException();
        }*/
        #endregion

        #region Writer
        public string ModifikacijaBaze(string poruka, byte[] promena, byte[] signature)
        {
            try
            {
                return factory.ModifikacijaBaze(poruka, promena,signature);
            }
            catch (SecurityAccessDeniedException e)
            {
                string info = $"Greska prilikom modifikacije baze podataka: {e.Message}";
                Console.WriteLine(info);
                return info;
            }
          
        }
        public string UpisUBazu(string poruka, byte[] promena, byte[] signature)
        {
          
            try
            {
                return factory.UpisUBazu(poruka, promena,signature);
            }
            catch (SecurityAccessDeniedException e)
            {
                string info = $"Greska prilikom upisa baze podataka: {e.Message}";
                Console.WriteLine(info);
                return info;
            }
        

        }

        #endregion

        #region Reader
        public byte[] CitanjeBaze(string bazaPodataka)
        {
            try
            {
                return factory.CitanjeBaze(bazaPodataka);
            }
            catch (SecurityAccessDeniedException e)
            {
                string info = $"Greska prilikom citanja baze podataka: {e.Message}";
                Console.WriteLine(info);
                return ASCIIEncoding.ASCII.GetBytes(info);
                
            }
       
        }
        #endregion

        #region All
        public string NajvecaPrimanjaPoDrzavi(string bazaPodataka)
        {
            try
            {
                return factory.NajvecaPrimanjaPoDrzavi(bazaPodataka);
            }
            catch (SecurityAccessDeniedException e)
            {
                string info = $"Greska prilikom citanja baze podataka: {e.Message}";
                Console.WriteLine(info);
                return info;

            }
            catch (FaultException e)
            {
                string info = $"Greska prilikom citanja baze podataka: {e.Message}";
                Console.WriteLine(info);
                return info;
            }
        }
        public string SrednjaVrednostZaDrzavuGodinu(string bazaPodataka, string drzava, string godina)
        {
            try
            {
                return factory.SrednjaVrednostZaDrzavuGodinu(bazaPodataka,drzava,godina);
            }
            catch (SecurityAccessDeniedException e)
            {
                string info = $"Greska prilikom citanja baze podataka: {e.Message}";
                Console.WriteLine(info);
                return info;

            }
            catch (FaultException e)
            {
                string info = $"Greska prilikom citanja baze podataka: {e.Message}";
                Console.WriteLine(info);
                return info;
            }
        }
        public string SrednjaVrednostZaGradStarost(string bazaPodataka, string grad, string starostOd, string starostDo)
        {
            try
            {
                return factory.SrednjaVrednostZaGradStarost(bazaPodataka,grad,starostOd,starostDo);
            }
            catch (SecurityAccessDeniedException e)
            {
                string info = $"Greska prilikom citanja baze podataka: {e.Message}";
                Console.WriteLine(info);
                return info;

            }
            catch (FaultException e)
            {
                string info = $"Greska prilikom citanja baze podataka: {e.Message}";
                Console.WriteLine(info);
                return info;
            }
        }
        #endregion
        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }

            this.Close();
        }
    }
}
