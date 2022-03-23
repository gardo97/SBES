using Contracts;
using Manager;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            BazaPodataka db = BazaPodataka.Inicijalizacija();
            /// srvCertCN.SubjectName should be set to the service's username. .NET WindowsIdentity class provides information about Windows user running the given process
			string srvCertCN = Manager.Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
          //  string srvCertCN = "wcfService";
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string address = "net.tcp://localhost:7776/WCFService";
            ServiceHost serviceHost = new ServiceHost(typeof(WCFService));
            serviceHost.AddServiceEndpoint(typeof(IWCFService), binding, address);

           // serviceHost.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            //serviceHost.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
           
            //Custom validation mode enables creation of a custom validator - CustomCertificateValidator
            serviceHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            serviceHost.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();

            ///If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
           serviceHost.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            ///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
            serviceHost.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);
            
            serviceHost.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            serviceHost.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            
            serviceHost.Authorization.ServiceAuthorizationManager = new CustomServiceAuthorizationManager();

            serviceHost.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            policies.Add(new CustomAuthorizationPolicy());
            serviceHost.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();


            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;
            //newAudit.MessageAuthenticationAuditLevel = AuditLevel.
            newAudit.SuppressAuditFailure = true;

            serviceHost.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            serviceHost.Description.Behaviors.Add(newAudit);

            serviceHost.Open();
            Console.WriteLine("WCFService is opened. Press <enter> to finish and save databases...");
            Console.ReadLine();

            serviceHost.Close();

            db.SerializeData();
        }
    }
}
